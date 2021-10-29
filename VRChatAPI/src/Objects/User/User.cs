using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;

namespace VRChatAPI.Objects
{
	public class User : LimitedUser
	{
		public IEnumerable<string> BioLinks { get; set; }
		public EUserState? State { get; set; }
		[JsonPropertyName("last_login")]
		public DateTime? LastLogin { get; set; }
		public bool AllowAvatarCopying { get; set; }
		[JsonPropertyName("date_joined")]
		public DateTime? DateJoined { get; set; }
		public World WorldId { get; set; }
		public string FriendKey { get; set; }
		public InstanceID InstanceId { get; set; }
	}
}