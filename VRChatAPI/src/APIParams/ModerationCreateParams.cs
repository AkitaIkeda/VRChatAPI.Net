using System.ComponentModel.DataAnnotations;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.APIParams
{
	public class ModerationCreateParams
	{
		[Required]
		public UserID Moderated { get; set; }
		[Required]
		public EModerationType Type { get; set; }
	}
}