using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Avatar : SerializableObjectAbstract, IAvatar
	{
		public AvatarID Id { get; set; }
		public string AssetUrl { get; set; }
		//public IAssetUrlObject assetUrlObject { get; set; }
		public UserID AuthorId { get; set; }
		public string AuthorName { get; set; }
		public string Description { get; set; }
		public bool? Featured { get; set; }
		public VRCFilePath ImageUrl { get; set; }
		public string Name { get; set; }
		public EReleaseState? ReleaseStatus { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public string ThumbnailImageUrl { get; set; }
		public IEnumerable<UnityPackage> UnityPackages { get; set; }
		public string UnityPackageUrl { get; set; }
		public UnityPackageUrlObject UnityPackageUrlObject { get; set; }
		[JsonPropertyName("updated_at")]
		public DateTime? UpdatedAt { get; set; }
		[JsonPropertyName("created_at")]
		public DateTime? CreatedAt { get; set; }
		public int? Version { get; set; }

		public string GetIDString(int prefixIndex = 0) => Id.GetIDString(prefixIndex);
	}
}