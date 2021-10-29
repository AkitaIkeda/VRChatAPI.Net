using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VRChatAPI.Enums;

namespace VRChatAPI.APIParams
{
	public class VRCFileCreateParams
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public EMimeType MimeType { get; set; }
		[Required]
		public string Extension { get; set; }
		public IEnumerable<string> Tags { get; set; }
	}
}