using System;
using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class PastDisplayName : SerializableObjectAbstract
	{
		public string DisplayName { get; set; }
		[JsonPropertyName("updated_at")]
		public DateTime? UpdatedAt { get; set; }
	}
}