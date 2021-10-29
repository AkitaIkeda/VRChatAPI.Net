using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.APIParams
{
	public class AvatarCreateParams
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public VRCFilePath ImageUrl { get; set; }
		public VRCFilePath AssetUrl { get; set; }
		public AvatarID Id { get; set; }
		public string Description { get; set; }
		public IEnumerable<string> tags { get; set; }
		public EReleaseState? releaseStatus { get; set; }
		public int? Version { get; set; }
		public VRCFilePath UnityPackageUrl { get; set; }
	}
}