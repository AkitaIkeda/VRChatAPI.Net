using System;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Message : IMessage
	{
		public MessageID Id { get; set; }
		public bool CanBeUodated { get; set; }
		public string message { get; set; }
		public EMessageType MessageType { get; set; }
		public int RemainingCooldownMinutes { get; set; }
		public int Slot { get; set; }
		public DateTime UpdatedAt { get; set; }

		public string GetIDString(int prefixIndex = 0) => Id.GetIDString(prefixIndex);
	}
}