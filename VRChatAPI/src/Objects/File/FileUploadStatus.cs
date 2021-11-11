using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class FileUploadStatus : SerializableObjectAbstract
	{
		string UploadId { get; set; }
		string FileName { get; set; }
		int NextPartNumber { get; set; }
		int MaxParts { get; set; }
		//IEnumerable<Part> Parts { get; set; }
		//IEnumerable<Etag> Etags { get; set; }
	}
}