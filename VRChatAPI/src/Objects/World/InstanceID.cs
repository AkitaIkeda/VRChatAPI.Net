using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	[JsonConverter(typeof(ObjectAsStringConverter))]
	public class InstanceID : IVRCObject, IParsable
	{
		public string Name { get; set; }
		public EInstanceType Type { get; set; }
		public ERegion Region { get; set; }
		public UserID Owner { get; set; }
		public Guid? Nonce { get; set; }

		private static readonly Dictionary<EInstanceType, string> typeDict =
			new Dictionary<EInstanceType, string>
			{
				{ EInstanceType.FriendsPlus, "hidden" },
				{ EInstanceType.Friends, "friends" },
				{ EInstanceType.InvitePlus, "private" },
				{ EInstanceType.Invite, "private" },
			};
		private static readonly Dictionary<ERegion, string> regionDict =
			new Dictionary<ERegion, string>
			{
				{ ERegion.Europe, "eu" },
				{ ERegion.Japan, "jp" },
				{ ERegion.UnitedStates, "us" },
			};

		public string GetIDString(int prefixIndex = 0) => GetInstanceIDString();

		public string GetInstanceIDString(bool omitRegion = true) =>
			$@"{(string.IsNullOrEmpty(Name) ? 
					$"{new Random().Next(0, 99999):D05}" :
					Name)
				}{(Type == EInstanceType.Public ?
					string.Empty :
					$"~{typeDict[Type]}({Owner.GetIDString()})")
				}{(Type == EInstanceType.InvitePlus ?
					"~canRequestInvite" :
					string.Empty)
				}{(omitRegion && Region == ERegion.UnitedStates ?
					string.Empty :
					$"~region({regionDict[Region]})")
				}{(Type == EInstanceType.Public ? 
					string.Empty : 
					$"~nonce({(Nonce = Nonce is null ? Guid.NewGuid() : Nonce):D})")}";

		public void ParseFromString(string id)
		{
			var raw = id.Split('~');
			bool canRI = !(raw.FirstOrDefault(v => v == "canRequestInvite") is null);
			var keyValue = raw.Skip(1)
				.Select(v => v.Trim(')').Split('('))
				.Where(v => v.Length > 1)
				.ToDictionary(v => v.First(), v => v.ElementAt(1));
			var type = keyValue.FirstOrDefault(v => typeDict.Values.Contains(v.Key));
			Name = raw.First();
			Type =
				typeDict
					.Where(v => v.Value == type.Key)
					.Select(v => v.Key) is var t 
				&& t.Count() == 0
					? EInstanceType.Public
					:	t.Count() > 1 
						? (canRI ? EInstanceType.InvitePlus : EInstanceType.Invite) 
						: t.Single();
			Region = keyValue.ContainsKey("region")
				? regionDict.First(v => v.Value == keyValue["region"]).Key
				: ERegion.UnitedStates;
			Owner = type.Value is null ? null : UserID.Parse(type.Value);
			Nonce = Type != EInstanceType.Public
				? (Guid?)Guid.ParseExact(keyValue["nonce"], "D")
				: null;
		}
		public static InstanceID Parse(string id) {
			var t = new InstanceID();
			t.ParseFromString(id);
			return t;
		}

		public override string ToString() => GetInstanceIDString();
	}
}