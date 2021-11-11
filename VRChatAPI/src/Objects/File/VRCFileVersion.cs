using System;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCFileVersion : SerializableObjectAbstract
	{
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }
		public bool? Deleted { get; set; }
		public VRCFileData File { get; set; }
		public VRCFileData Delta { get; set; }
		public VRCFileData Signature { get; set; }
		public EUploadStatus Status { get; set; }
		public int Version { get; set; }
	}
}