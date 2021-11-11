using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class NotificationDetail : SerializableObjectAbstract
	{
		public Location WorldId { get; set; }
		public EPlatform? Platform { get; set; }
		public UserID InResponseToUser { get; set; }
		public string InviteMessage { get; set; }
		public string ResponseMessage { get; set; }
		public string RequestMessage { get; set; }
		public string RequestResponseMessage { get; set; } // probably
		public UserID UserToKickId { get; set; }
		public UserID InitiatorUserId { get; set; }
		public WorldID HalpId { get; set; } // Unknown
	}
}