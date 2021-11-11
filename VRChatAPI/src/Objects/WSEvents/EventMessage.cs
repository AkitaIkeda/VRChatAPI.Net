using VRChatAPI.Enums;

namespace VRChatAPI.Objects
{
	public class EventMessage
	{
		public string content { get; set; }
		public EEventType type { get; set; }
	}
}