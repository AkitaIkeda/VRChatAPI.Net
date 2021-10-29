using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class LimitedWorld : IWorld
	{
		public WorldID Id { get; set; }
		public string Name { get; set; }
		public UserID AuthorId { get; set; }
		public string AuthorName { get; set; }
		public int? Capacity { get; set; }
		public string ImageUrl { get; set; }
		public string ThumbnailImageUrl { get; set; }
		public EReleaseState ReleaseStatus { get; set; }
		public string Organization { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public int? Favorites { get; set; }
		[JsonPropertyName("created_at")]
		public DateTime? CreatedAt { get; set; }
		[JsonPropertyName("updated_at")]
		public DateTime? UpdatedAt { get; set; }
		public DateTime? PublicationDate { get; set; }
		public DateTime? LabsPublicationDate { get; set; }
		public IEnumerable<UnityPackage> UnityPackages { get; set; }
		public int? Popularity { get; set; }
		public int? Heat { get; set; }
		public int? Occupants { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			Id.GetIDString(prefixIndex);
	}
}