using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class DownloadUrls : SerializableObjectAbstract
	{
		public string Sdk2 { get; set; }
		[JsonPropertyName("sdk3-worlds")]
		public string Sdk3Worlds { get; set; }
		[JsonPropertyName("sdk3-avatars")]
		public string Sdk3Avatars { get; set; }
	}
}