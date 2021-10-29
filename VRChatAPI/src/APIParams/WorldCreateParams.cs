using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.APIParams
{
	public class WorldCreateParams
	{
		[Required]
		public VRCFilePath AssetUrl { get; set; }
		[Required]
		public VRCFilePath ImageUr { get; set; }
		[Required]
		public string Name { get; set; }
		[MinLength(1)]
		public string AssetVersion { get; set; }
		[MinLength(1)]
		public string AuthorName { get; set; }
		[Range(1, 40)]
		public int? Capacity { get; set; }
		public string Description { get; set; }
		public WorldID Id { get; set; }
		public EPlatform? Platform { get; set; }
		public EReleaseState? ReleaseStatus { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public string UnityPackageUrl { get; set; }
		public string UnityVersion { get; set; }
	}
}