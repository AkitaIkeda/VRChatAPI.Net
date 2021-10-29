namespace VRChatAPI.Objects
{
	public class FileUploadStatus
	{
		string UploadId { get; set; }
		string FileName { get; set; }
		int NextPartNumber { get; set; }
		int MaxParts { get; set; }
		//TODO: find out
		//IEnumerable<Part> Parts { get; set; }
		//IEnumerable<Etag> Etags { get; set; }
	}
}