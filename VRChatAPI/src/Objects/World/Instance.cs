using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Instance : SerializableObjectAbstract, IInstance
	{
		public bool Active { get; set; }
		public bool CanRequestInvite { get; set; }
		public int Capacity { get; set; }
		public string ClientNumber { get; set; }
		public bool Full { get; set; }
		public Location Id { get; set; }
		public InstanceID InstanceId { get; set; }
		public Location Location { get; set; }
		[JsonPropertyName("n_users")]
		public int NUsers { get; set; }
		public string Name { get; set; }
		public string Nonce { get; set; }
		public UserID OwnerId { get; set; }
		public bool Permanent { get; set; }
		public string PhotonRegion { get; set; }
		public EPlatform Platforms { get; set; }
		public string Region { get; set; }
		public string ShortName { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public EInstanceType Type { get; set; }
		public WorldID WorldId { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			GetLocationString();

		public string GetInstanceIDString() =>
			Id.GetInstanceIDString();

		public string GetLocationString() =>
			Id.GetLocationString();

		public string GetWorldIDString() =>
			Id.GetWorldIDString();
	}
}