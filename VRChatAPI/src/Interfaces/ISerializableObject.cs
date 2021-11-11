using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRChatAPI.Objects{
	public abstract class SerializableObjectAbstract{
		[JsonExtensionData]
		public Dictionary<string, JsonElement> ExtensionData { get; set; }
	}
}