using VRChatAPI.Enums;

namespace VRChatAPI.APIParams
{
	public class NotificationSearchParams
	{
		public ENotificationType? Type { get; set; }
		public bool? Sent { get; set; }
		public bool? Hiddent { get; set; }
		public string After { get; set; }
	}
}