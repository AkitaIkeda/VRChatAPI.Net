using System.Text.Json.Serialization;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	[JsonConverter(typeof(InstanceInfoConverter))]
	public class InstanceInfo
	{
		public InstanceID InstanceID { get; set; }
		public int PlayersNum { get; set; }
	}
}