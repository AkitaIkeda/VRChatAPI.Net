using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IFileAPI :
		ICreate<VRCFile, VRCFileCreateParams>,
		IGet<VRCFile, IVRCFile, VRCFileSearchParams>,
		IDelete<ResponseMessage, IVRCFile>
	{
		Task<VRCFile> CreateNewFileVersion(
			IVRCFile file,
			bool deltaUpload,
			string fileMd5,
			long fileSizeInBytes,
			string signatureMd5,
			long signatureSizeInBytes,
			CancellationToken ct = default);
		Task<VRCFile> DeleteFileVersion(
			IVRCFile file,
			int version,
			CancellationToken ct = default);
		Task<string> StartUpload(
			IVRCFile file,
			int version,
			EFileType fileType,
			uint partNumber = 1,
			CancellationToken ct = default);
		Task<VRCFile> FinishUpload(
			IVRCFile file,
			int version,
			EFileType fileType,
			IEnumerable<string> etags,
			CancellationToken ct = default);
		Task<FileUploadStatus> GetUploadStatus(
			IVRCFile file,
			int version,
			EFileType fileType,
			CancellationToken ct = default);
	}
}