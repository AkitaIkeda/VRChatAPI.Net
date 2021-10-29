using System;
using System.Text.Json.Serialization;

namespace VRChatAPI.Objects
{
	public class PastDisplayName
	{
		public string DisplayName { get; set; }
		[JsonPropertyName("updated_at")]
		public DateTime? UpdatedAt { get; set; }
	}
}