﻿using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.APIParams
{
	public class AvatarSearchParams
	{
		[JsonIgnore]
		public bool Favorites { get; set; }
		public bool? Featured { get; set; }
		public ESortOption? Sort { get; set; }
		public EUserCategory? User { get; set; }
		public UserID UserId { get; set; }
		public EOrderOption? Order { get; set; }
		public string Tag { get; set; }
		public string Notag { get; set; }
		public EReleaseState? ReleaseStatus { get; set; }
		public string MaxUnityVersion { get; set; }
		public string MinUnityVersion { get; set; }
		public EPlatform? Platform { get; set; }
	}
}