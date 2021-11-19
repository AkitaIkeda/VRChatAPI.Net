using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using librsync.net;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Serialization;
using static VRChatAPI.Utils.JsonUtilityExtensions;

namespace VRChatAPI.Extentions
{
	public static class VRCFileUtilities
	{
		public static VRCFilePath GetFilePath(this VRCFileID file, int version, EFileType fileType = EFileType.file) =>
			new VRCFilePath{ FileID = file, FileType = fileType, Version = version };
		public static VRCFilePath GetFilePath(this VRCFile file, int version, EFileType fileType = EFileType.file) =>
			file.Id.GetFilePath(version, fileType);

		public static VRCImagePath GetImagePath(this VRCFileID file, int version, int imageSize) =>
			new VRCImagePath{ FileID = file, Version = version, ImageSize = imageSize };
		public static VRCImagePath GetImagePath(this VRCFile file, int version, int imageSize) =>
			file.Id.GetImagePath(version, imageSize);

		public static async Task<VRCFile> CreateNewVersionAndUploadFile(
			this ISession session,
			VRCFile file,
			Stream data, 
			CancellationToken ct = default)
		{
			var DeltaEnabled = session.RemoteConfig.SdkEnableDeltaCompression ?? false;

			if(!data.CanSeek){
				Stream ts = new MemoryStream();
				await data.CopyToAsync(ts);
				data.Dispose();
				data = ts;
			}

			var filedata = data;
			var fileSize = filedata.Length;
			var fileMd5 = filedata.GetMD5();
			filedata.Seek(0, SeekOrigin.Begin);

			if(ct.IsCancellationRequested) throw new OperationCanceledException();

			var sig = filedata.ComputeSignature();
			if(ct.IsCancellationRequested) throw new OperationCanceledException();
			var sigSize = sig.Length;
			var sigMd5 = sig.GetMD5();
			filedata.Seek(0, SeekOrigin.Begin);
			sig.Seek(0, SeekOrigin.Begin);

			bool uploadDelta = default;
			if(DeltaEnabled && (file.Versions.LastOrDefault() is var version 
				&& version?.Status == EUploadStatus.complete 
				&& version?.Signature?.Status == EUploadStatus.complete))
			{
				var delta = filedata.ComputeDelta(await 
					(await session.APIHttpClient.Get(
						file.GetFilePath(version.Version, EFileType.signature).GetUrl(), ct))
					.Content.ReadAsStreamAsync());

				var deltaSize = delta.Length;

				if(ct.IsCancellationRequested) throw new OperationCanceledException();

				uploadDelta = DeltaEnabled && deltaSize < fileSize;
				if(uploadDelta)
				{
					filedata.Dispose();
					filedata = delta;
					fileSize = deltaSize;
					fileMd5 = delta.GetMD5();
					filedata.Seek(0, SeekOrigin.Begin);
				}
			}
			var v = file.Versions.LastOrDefault();
			if(v.Status == EUploadStatus.waiting)
			{
				if (v.Signature.Md5 != sigMd5 && (uploadDelta ? v.Delta.Status : v.File.Status) != EUploadStatus.waiting)
				{
					file = await session.DeleteFileVersion(file, v.Version, ct);
					file = await session.CreateNewFileVersion(file, uploadDelta, sigMd5, sigSize, fileMd5, fileSize);
				}
			}
			else{
				file = await session.CreateNewFileVersion(file, uploadDelta, sigMd5, sigSize, fileMd5, fileSize);
			}

			try
			{
				ct.Register(() => _ = session.DeleteLatestFileVersion(file, ct));
				v = file.Versions.Last();
				if((uploadDelta ? v.Delta.Status : v.File.Status) == EUploadStatus.waiting){
					file = await session.UploadComponent(file,
						uploadDelta ? EFileType.delta : EFileType.file, filedata, 
						uploadDelta ? EMimeType.application_x_rsync_delta : (EMimeType)file.MimeType, fileMd5, ct);
					filedata.Seek(0, SeekOrigin.Begin);
				}
				if(v.Signature.Status == EUploadStatus.waiting){
					file = await session.UploadComponent(file, EFileType.signature, sig, EMimeType.application_x_rsync_signature, sigMd5, ct);
				}
			}
			catch (Exception e)
			{
				await session.DeleteLatestFileVersion(file);
				throw e;
			}
			return file;
		}

		internal static async Task<VRCFile> UploadComponent(
			this ISession session,
			VRCFile file,
			EFileType fileType, 
			Stream dataStream,
			EMimeType contentType,
			string md5Base64,
			CancellationToken ct = default
		)
		{
			var version = file.Versions.Last().Version;
			var url = await session.StartUpload(file, version, fileType, 1, ct);
			var etag = await session.UploadData(url, dataStream, contentType, md5Base64, ct);
			//ETag supposed to be sent with finish request but 'null' works well
			return await session.FinishUpload(file, version, fileType, null, ct);
		}

		public static Task<VRCFile> DeleteLatestFileVersion(
			this ISession session,
			VRCFile file,
			CancellationToken ct = default) => 
			session.DeleteFileVersion(file, file.Versions.Last().Version, ct);

		private static async Task<string> UploadData(
			this ISession session,
			string url, 
			Stream s, 
			EMimeType contentType, 
			string md5Base64,
			CancellationToken ct = default)
		{
			var r = new HttpRequestMessage(HttpMethod.Post, url);
			var c= new StreamContent(s);
			c.Headers.ContentType = MediaTypeHeaderValue.Parse(JsonStringFromObject(contentType, new JsonSerializerOptions{
				Converters = {
					new StringEnumConverter(),
				}
			}));
			c.Headers.ContentMD5 = Convert.FromBase64String(md5Base64);
			r.Content = c;
			var response = await session.APIHttpClient.Send(r, ct);
			return response.Headers.ETag.Tag;
		}

		public static string GetMD5(this Stream s) => Convert.ToBase64String(MD5.Create().ComputeHash(s));
		public static Stream ComputeSignature(this Stream s) => Librsync.ComputeSignature(s);
		public static Stream ComputeDelta(this Stream s, Stream previousSignature) => Librsync.ComputeDelta(previousSignature, s);
	}
}