using System;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;

namespace VRChatAPI.Objects
{
	public class UnityPackage
	{
		public string Id { get; set; }
		public string AssetUrl { get; set; }
		//public AssetUrlObject AssetUrlObject { get; set; }
		public string PluginUrl { get; set; }
		//public PluginUrlObject PluginUrlObject { get; set; }
		public string UnityVersion { get; set; }
		public long? UnitySortNumber { get; set; }
		public int? AssetVersion { get; set; }
		public EPlatform? Platform { get; set; }
		[JsonPropertyName("created_at")]
		public DateTime? CreatedAt { get; set; }
	}
}