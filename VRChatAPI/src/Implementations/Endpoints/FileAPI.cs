using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IFileAPI
	{
		private const string fileEndpoint = "file";
		public Task<VRCFile> Create(VRCFileCreateParams param, CancellationToken ct = default) =>
			client.Post<VRCFile, VRCFileCreateParams>(fileEndpoint, param, ct);

		public Task<VRCFile> CreateNewFileVersion(
			IVRCFile file,
			bool deltaUpload,
			string fileMd5,
			long fileSizeInBytes,
			string signatureMd5,
			long signatureSizeInBytes,
			CancellationToken ct = default) =>
			client.Post<VRCFile, Dictionary<string, object>>($"{fileEndpoint}/{file.GetIDString()}",
				new Dictionary<string, object>
				{
					{ deltaUpload ? "deltaMd5" : "fileMd5", fileMd5 },
					{ deltaUpload ? "deltaSizeInBytes" : "fileSizeInBytes", fileSizeInBytes },
					{ "signatureMd5", signatureMd5 },
					{ "signatureSizeInBytes", signatureSizeInBytes },
				}, ct);

		public Task<ResponseMessage> Delete(IVRCFile obj, CancellationToken ct = default) =>
			client.Delete<ResponseMessage>($"{fileEndpoint}/{obj.GetIDString()}", ct);

		public Task<VRCFile> DeleteFileVersion(IVRCFile file, int version, CancellationToken ct = default) =>
			client.Delete<VRCFile>($"{fileEndpoint}/{file.GetIDString()}/{version}", ct);

		public Task<VRCFile> FinishUpload(IVRCFile file, int version, EFileType fileType, IEnumerable<string> etags, CancellationToken ct = default) =>
			client.Put<VRCFile, Dictionary<string, object>>($"{fileEndpoint}/{file.GetIDString()}/{version}/{fileType}/finish",
				new Dictionary<string, object>{
					{ "etags", etags },
				}, ct);

		public Task<IEnumerable<VRCFile>> Get(VRCFileSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<VRCFile>>(
				$@"files?{
					QueryConstructor.MakeQuery(option, serializerOption)}&n={n}&offset={offset}", ct);

		public Task<VRCFile> Get(IVRCFile obj, CancellationToken ct = default) =>
			client.Get<VRCFile>($"{fileEndpoint}/{obj.GetIDString()}", ct);

		public Task<FileUploadStatus> GetUploadStatus(IVRCFile file, int version, EFileType fileType, CancellationToken ct = default) =>
			client.Get<FileUploadStatus>($"{fileEndpoint}/{file.GetIDString()}/{version}/{fileType}/status", ct);

		public async Task<string> StartUpload(IVRCFile file, int version, EFileType fileType, uint partNumber = 1, CancellationToken ct = default) =>
			(await client.Put<JsonElement>($"{fileEndpoint}/{file.GetIDString()}/{version}/{fileType}/start?partNumber={partNumber}", ct))
				.GetProperty("url").GetString();
		
	}
}
