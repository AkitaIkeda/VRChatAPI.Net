using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class User : LimitedUser
	{
		public IEnumerable<string> BioLinks { get; set; }
		public EUserOnlineState? State { get; set; }
		[JsonPropertyName("last_login")]
		public DateTime? LastLogin { get; set; }
		public bool AllowAvatarCopying { get; set; }
		[JsonPropertyName("date_joined")]
		public DateTime? DateJoined { get; set; }
		public string WorldId { get; set; }
		public string FriendKey { get; set; }
		public string InstanceId { get; set; }
	}
}