using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class LimitedUser : SerializableObjectAbstract, IUser
	{
		public UserID Id { get; set; }
		public string Username { get; set; }
		public string DisplayName { get; set; }
		public string Bio { get; set; }
		public VRCFilePath UserIcon { get; set; }
		public VRCFilePath ProfilePicOverride { get; set; }
		public string StatusDescription { get; set; }
		public VRCFilePath CurrentAvatarImageUrl { get; set; }
		public VRCImagePath CurrentAvatarThumbnailImageUrl { get; set; }
		public AvatarID FallbackAvatar { get; set; }
		public EDeveloperType? DeveloperType { get; set; }
		[JsonPropertyName("last_platform")]
		public EPlatform? LastPlatform{ get; set; }
		public EUserState? Status { get; set; }
		public bool? IsFriend { get; set; }
		public string Location { get; set; } // If empty, User is not in the game.
		public IEnumerable<string> Tags { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			Id.GetIDString(prefixIndex);
	}
}