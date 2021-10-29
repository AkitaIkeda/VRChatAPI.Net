using VRChatAPI.Enums;

namespace VRChatAPI.Objects
{
	public class VRCFileData
	{
		public EFileCategory Category { get; set; }
		public string FileName { get; set; }
		public string Md5 { get; set; }
		public int SizeInBytes { get; set; }
		public EUploadStatus Status { get; set; }
		public string UploadId { get; set; }
		public string Url { get; set; }
	}
}