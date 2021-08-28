using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using VRChatAPI.Abstracts;
using librsync.net;
using System.Security.Cryptography;
using System.Threading;

namespace VRChatAPI.Objects
{
	[JsonConverter(typeof(StringEnumConverter))]
	[DataContract]
	public enum MimeType
	{
		[EnumMember(Value = "image/jpeg")]
		image_jpeg,
		[EnumMember(Value = "image/jpg")]
		image_jpg,
		[EnumMember(Value = "image/png")]
		image_png,
		[EnumMember(Value = "image/webp")]
		image_webp,
		[EnumMember(Value = "image/gif")]
		image_gif,
		[EnumMember(Value = "image/bmp")]
		image_bmp,
		[EnumMember(Value = "image/svg+xml")]
		image_svg_xml,
		[EnumMember(Value = "image/tiff")]
		image_tiff,
		[EnumMember(Value = "application/x-avatar")]
		application_x_avatar,
		[EnumMember(Value = "application/x-world")]
		application_x_world,
		[EnumMember(Value = "application/gzip")]
		application_gzip,
		[EnumMember(Value = "application/x-rsync-signature")]
		application_x_rsync_signature,
		[EnumMember(Value = "application/x-rsync-delta")]
		application_x_rsync_delta,
		[EnumMember(Value = "application/octet-stream")]
		application_octet_stream,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum Status
	{
		waiting,
		complete,
		none,
		queued,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum Category
	{
		multipart,
		queued,
		simple,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum FileType
	{
		file,
		signature,
		delta
	}

	public class FileInfo
	{
		public string fileName { get; set; }
		public string url { get; set; }
		public string md5 { get; set; }
		public uint sizeInBytes { get; set; }
		public Status status { get; set; }
		public Category category { get; set; }
		public string uploadId { get; set; }
	}

	public class Version
	{
		public uint version { get; set; }
		public Status status { get; set; }
		public DateTime created_at { get; set; }
		public FileInfo file { get; set; }
		public FileInfo delta { get; set; }
		public FileInfo signature { get; set; }
		public bool? deleted { get; set; }
	}

	[JsonConverter(typeof(Converters.AsTConverter<string>))]
	public class FileId : IdAbstract<FileId>
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<FileId>();
		public FileId() => guid = Guid.NewGuid();
		public FileId(string id) => this.id = id;
		public override string prefix => "file";
		public static implicit operator FileId(string id) => new FileId(id);
		public static implicit operator string(FileId file) => file.id;

		/// <summary>
		/// Get file object
		/// </summary>
		/// <returns>File object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<File> Get()
		{
			Logger.LogDebug("Get file {id}", id);
			var response = await Global.httpClient.GetAsync($"file/{id}");
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		/// <summary>
		/// Create new version of the file
		/// </summary>
		/// <param name="signatureMd5">md5 of signature</param>
		/// <param name="signatureSizeInBytes">size of signature</param>
		/// <param name="fileMd5">md5 of file</param>
		/// <param name="fileSizeInBytes">size of file</param>
		/// <returns>Updated File object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		internal async Task<File> CreateNewVersion(
			string signatureMd5,
			long signatureSizeInBytes,
			bool deltaUpload,
			string fileMd5,
			long fileSizeInBytes
		)
		{
			var p = new Dictionary<string, object>{
				{ "signatureMd5", signatureMd5 },
				{ "signatureSizeInBytes", signatureSizeInBytes },
				{ deltaUpload ? "deltaMd5" : "fileMd5", fileMd5 },
				{ deltaUpload ? "deltaSizeInBytes" : "fileSizeInBytes", fileSizeInBytes },
			};
			Logger.LogDebug("Creating new version {id}: {params}", id, Utils.UtilFunctions.MakeQuery(p, ", "));
			var content = new StringContent(JObject.FromObject(p.Where(v => !(v.Value is null)).ToDictionary(v => v.Key, v => v.Value)).ToString(), Encoding.UTF8, "application/json");
			var response = await Global.httpClient.PostAsync($"file/{id}", content);
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		/// <summary>
		/// Delete file
		/// </summary>
		/// <returns>Response message</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> Delete()
		{
			Logger.LogDebug("Delete file {id}", id);
			var response = await Global.httpClient.DeleteAsync($"file/{id}");
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}
	}
	public class File
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<File>();

		public FileId id { get; set; } 
		public string name { get; set; }
		public UserId ownerId { get; set; }
		public MimeType mimeType { get; set; }
		public string extension { get; set; }
		public List<string> tags { get; set; }
		public List<Version> versions { get; set; }

		public uint GetLatestVersionNum() => versions.Max(v => v.version);
		public Version GetVersion(uint versionNum) => versions.First(v => v.version == versionNum);

		/// <summary>
		/// Download file with the provided version num
		/// </summary>
		/// <param name="version">Version num. If not provided, download latest version</param>
		/// <returns>IO Stream for reading raw file bytes</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Stream> Download(uint? version = null)
		{
			Logger.LogDebug("Download {id} v{version}", id, version);
			version ??= GetLatestVersionNum();
			var response = await Global.httpClient.GetAsync($"file/{id}/{version}");
			return await response.Content.ReadAsStreamAsync();
		}

		/// <summary>
		/// Delete the latest version
		/// </summary>
		/// <remarks>
		/// You can only delete the latest version. To delete file object, call <see cref="FileId.Delete"/>
		/// </remarks>
		/// <returns>Updated File object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<File> DeleteLatestVersion()
		{
			var v = GetLatestVersionNum();
			Logger.LogDebug("Delete Version {id} v{version}", id, v);
			var response = await Global.httpClient.DeleteAsync($"file/{id}/{v}");
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		public async Task<File> CreateNewVersionAndUploadFile(Stream s, CancellationToken ct)
		{
			Logger.BeginScope("CreateNewVersionAndUploadFile");
			Logger.LogDebug("Prepare Files");
			var f = this;
			var DeltaEnabled = Global.RemoteConfig.sdkEnableDeltaCompression ?? false;
			Logger.LogDebug("DeltaEnabled: {enabled}", DeltaEnabled);

			if(!s.CanSeek){
				Stream ts = new MemoryStream();
				await s.CopyToAsync(ts);
				_ = s.DisposeAsync();
				s = ts;
			}

			var file = s;
			var fileSize = file.Length;
			var fileMd5 = file.GetMD5();
			file.Seek(0, SeekOrigin.Begin);

			if(ct.IsCancellationRequested) return f;

			Logger.LogDebug("Compute Signature");
			var sig = UploadHelper.ComputeSignature(file);
			if(ct.IsCancellationRequested) return f;
			var sigSize = sig.Length;
			var sigMd5 = sig.GetMD5();
			file.Seek(0, SeekOrigin.Begin);
			sig.Seek(0, SeekOrigin.Begin);

			bool uploadDelta = default;
			if(DeltaEnabled && f.GetIfPreviousVersionExist())
			{
				Logger.LogDebug("Compute Delta");
				var delta = await f.CreateDeltaFromPreviousVersionAsync(file);

				var deltaSize = delta.Length;

				if(ct.IsCancellationRequested) return f;

				uploadDelta = DeltaEnabled && deltaSize < fileSize;
				if(uploadDelta)
				{
					_ = file.DisposeAsync();
					file = delta;
					fileSize = deltaSize;
					fileMd5 = delta.GetMD5();
					file.Seek(0, SeekOrigin.Begin);
				}
			}
			Logger.LogDebug("uploadDelta: {uploadDelta}", uploadDelta);
			var v = f.GetVersion(f.GetLatestVersionNum());
			if(v.status == Status.waiting)
			{
				if(v.signature.md5 == sigMd5 || (uploadDelta ? v.delta.status : v.file.status) == Status.waiting)
					Logger.LogDebug("Waiting version found. Resume uploading file instead.");
				else{
					Logger.LogDebug("Version found that is not finished upload proess. Deleting it before creating new version.");
					f = await f.DeleteLatestVersion();
					f = await f.id.CreateNewVersion(sigMd5, sigSize, uploadDelta, fileMd5, fileSize);
				}
			}
			else{
				Logger.LogDebug("Creating new version");
				f = await f.id.CreateNewVersion(sigMd5, sigSize, uploadDelta, fileMd5, fileSize);
			}

			try
			{
				ct.Register(() => _ = f.DeleteLatestVersion());
				v = f.GetVersion(f.GetLatestVersionNum());
				if((uploadDelta ? v.delta.status : v.file.status) == Status.waiting){
					Logger.LogDebug("Upload File data");
					f = await f.UploadComponent(
						uploadDelta ? FileType.delta : FileType.file, file, 
						uploadDelta ? MimeType.application_x_rsync_delta : mimeType, fileMd5, ct);
					file.Seek(0, SeekOrigin.Begin);
				}
				if(v.signature.status == Status.waiting){
					Logger.LogDebug("Upload Signature");
					f = await f.UploadComponent(FileType.signature, sig, MimeType.application_x_rsync_signature, sigMd5, ct);
				}
			}
			catch (Exception e)
			{
				Logger.LogError("Caused exception during Uploading Process. Revert changes.");
				await f.DeleteLatestVersion();
				throw e;
			}
			return f;
		}
	}

	internal static class UploadHelper
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger("UploadHelper");

		/// <summary>
		/// Start Upload process
		/// </summary>
		/// <remarks>
		/// You need to call this and receive a new AWS API URL for each partNumber
		/// </remarks>
		/// <param name="versionId">version to upload</param>
		/// <param name="fileType">filetype to upload</param>
		/// <param name="partNumber">part id</param>
		/// <returns></returns>
		public static async Task<string> StartUpload(
			this File file,
			uint versionId,
			FileType fileType,
			uint partNumber = 1
		)
		{
			Logger.LogDebug("Start Uploading file {id} v{versionId}", file.id, versionId);
			var json = new JObject();
			json.Add("partNumber", partNumber);
			var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
			var response = await Global.httpClient.PutAsync($"file/{file.id}/{versionId}/{fileType}/start", content);
			return JObject.Parse(await response.Content.ReadAsStringAsync())["url"].ToString();
		}
		
		/// <summary>
		/// Start Upload process
		/// </summary>
		/// <remarks>
		/// You need to call this and receive a new AWS API URL for each partNumber
		/// </remarks>
		/// <param name="versionId">version to upload</param>
		/// <param name="fileType">filetype to upload</param>
		/// <param name="partNumber">part id</param>
		/// <param name="etgas">Array of ETags uploaded</param>
		/// <returns></returns>
		public static async Task<File> FinishtUpload(
			this File file,
			uint versionId,
			FileType fileType,
			List<string> etags,
			uint nextPartNumber = 0,
			uint maxParts = 0
		)
		{
			var p = new Dictionary<string, object>{
				{ "etags", etags },
				{ "nextPartNumber", nextPartNumber },
				{ "maxParts", maxParts },
			};
			Logger.LogDebug("Start Uploading file {id} v{versionId}", file.id, versionId);
			var json = JObject.FromObject(p.Where(v => !(v.Value is null)).ToDictionary(v => v.Key, v => v.Value));
			var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
			var response = await Global.httpClient.PutAsync($"file/{file.id}/{versionId}/{fileType}/finish", content);
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		public static async Task<string> UploadDataAsync(
			string url, 
			Stream s, 
			MimeType contentType, 
			string md5Base64, CancellationToken ct)
		{
			Logger.LogDebug("Upload data to {url}", url);
			var content = new StreamContent(s);
			content.Headers.Add("content-type", JToken.FromObject(contentType).ToString());
			content.Headers.Add("content-md5", md5Base64);
			var response = await Global.httpClient.PutAsync(url, content, ct);
			return response.Headers.ETag.Tag;
		}

		public static Stream ComputeSignature(Stream s) => Librsync.ComputeSignature(s);
		public static Stream ComputeDelta(Stream s, Stream previousSignature) => Librsync.ComputeDelta(previousSignature, s);
		public static bool GetIfPreviousVersionExist(this File f) => f.GetVersion(f.GetLatestVersionNum()) is var v && 
			v.status == Status.complete && v.signature?.status == Status.complete;

		public static async Task<Stream> CreateDeltaFromPreviousVersionAsync(this File f, Stream s)
		{
			var sigUrl = f.GetVersion(f.GetLatestVersionNum()).signature.url;
			var response = await Global.httpClient.GetAsync(sigUrl);
			await using(var prevSignature = await response.Content.ReadAsStreamAsync())
			return ComputeDelta(s, prevSignature);
		}
		public static string GetMD5(this Stream s) => Convert.ToBase64String(MD5.Create().ComputeHash(s));

		internal static async Task<File> UploadComponent(
			this File f,
			FileType fileType, 
			Stream dataStream,
			MimeType contentType,
			string md5Base64,
			CancellationToken ct
		)
		{
			var version = f.GetLatestVersionNum();
			Logger.LogDebug("Upload file {id} v{version}", f.id, version);
			var url = await f.StartUpload((uint)version, fileType);
			var etag = await UploadHelper.UploadDataAsync(url, dataStream, contentType, md5Base64, ct);
			//ETag supposed to be sent with finish request but 'null' works well
			return await f.FinishtUpload((uint)version, fileType, null);
		}
	}
}