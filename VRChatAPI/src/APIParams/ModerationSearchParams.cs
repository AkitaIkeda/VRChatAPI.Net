using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.APIParams
{
	public class ModerationSearchParams
	{
		public EModerationType? type { get; set; }
		public UserID TargetUserId { get; set; }
	}
}