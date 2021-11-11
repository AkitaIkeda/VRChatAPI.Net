using System;
using System.Collections.Generic;
using System.Text.Json;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class CurrentUser : User
	{
		public IEnumerable<PastDisplayName> PastDisplayNames { get; set; }
		public bool? HasEmail { get; set; }
		public bool? HasPendingEmail { get; set; }
		public string ObfuscatedEmail { get; set; }
		public string ObfuscatedPendingEmail { get; set; }
		public bool? EmailVerified { get; set; }
		public bool? HasBirthday { get; set; }
		public bool? Unsubscribe { get; set; }
		public IEnumerable<string> StatusHistory { get; set; }
		public bool? StatusFirstTime { get; set; }
		public IEnumerable<UserID> Friends { get; set; }
		public IEnumerable<string> FriendGroupNames { get; set; }
		public AvatarID CurrentAvatar { get; set; }
		public VRCFilePath CurrentAvatarAssetUrl { get; set; }
		public DateTime? AccountDeletionDate { get; set; }
		public int? AcceptedTOSVersion { get; set; }
		public string SteamId { get; set; }
		public JsonElement? SteamDetails { get; set; }
		public string OculusId { get; set; }
		public bool? HasLoggedInFromClient { get; set; }
		public WorldID HomeLocation { get; set; }
		public bool? TwoFactorAuthEnabled { get; set; }
		public IEnumerable<UserID> OnlineFriends { get; set; }
		public IEnumerable<UserID> ActiveFriends { get; set; }
		public IEnumerable<UserID> OfflineFriends { get; set; }
	}
}