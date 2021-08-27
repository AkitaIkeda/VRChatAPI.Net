using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using VRChatAPI.Abstracts;

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
		public async Task<File> CreateNewVersion(
			string signatureMd5,
			uint signatureSizeInBytes,
			string fileMd5 = null,
			uint? fileSizeInBytes = null
		)
		{
			var p = new Dictionary<string, object>{
				{ "signatureMd5", signatureMd5 },
				{ "signatureSizeInBytes", signatureSizeInBytes },
				{ "fileMd5", fileMd5 },
				{ "fileSizeInBytes", fileSizeInBytes },
			};
			Logger.LogDebug("Creating new version {id}: {params}", id, Utils.UtilFunctions.MakeQuery(p, ", "));
			var content = new StringContent(JObject.FromObject(p.Where(v => !(v.Value is null))).ToString(), Encoding.UTF8);
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
		public UserId userId { get; set; }
		public MimeType mimeType { get; set; }
		public string extension { get; set; }
		public List<string> tags { get; set; }
		public List<Version> versions { get; set; }

		public uint GetLatestVersionNum() => versions.Max(v => v.version);

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
		public async Task<File> DeleteVersion()
		{
			var v = GetLatestVersionNum();
			Logger.LogDebug("Delete Version {id} v{version}", id, v);
			var response = await Global.httpClient.DeleteAsync($"file/{id}/{v}");
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		public async Task<File> Upload(
			FileType fileType, 
			Stream dataStream,
			uint? version = null
		)
		{
			version ??= GetLatestVersionNum();
			Logger.LogDebug("Upload file {id} v{version}", id, version);
			var url = await this.StartUpload((uint)version, fileType);
			var etag = await UploadHelper.UploadDataAsync(url, dataStream);
			return await this.FinishtUpload((uint)version, fileType, new List<string>(){etag});
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
		internal static async Task<string> StartUpload(
			this File file,
			uint versionId,
			FileType fileType,
			uint partNumber = 1
		)
		{
			Logger.LogDebug("Start Uploading file {id} v{versionId}", file.id, versionId);
			var json = new JObject();
			json.Add("partNumber", partNumber);
			var content = new StringContent(json.ToString(), Encoding.UTF8);
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
		internal static async Task<File> FinishtUpload(
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
			var json = JObject.FromObject(p.Where(v => !(v.Value is null)));
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PutAsync($"file/{file.id}/{versionId}/{fileType}/start", content);
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		internal static async Task<string> UploadDataAsync(string url, Stream s)
		{
			Logger.LogDebug("Upload data to {url}", url);
			var content = new StreamContent(s);
			var response = await Global.httpClient.PutAsync(url, content);
			return response.Headers.GetValues("ETags").First();
		}
	}
}