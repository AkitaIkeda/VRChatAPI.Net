using System;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Notification : SerializableObjectAbstract, INotification
	{
		public NotificationID Id { get; set; }
		public UserID SenderUserId { get; set; }
		public string SenderUsername { get; set; }
		public ENotificationType Type { get; set; }
		public string Message { get; set; }
		public NotificationDetail Details { get; set; }
		public bool Seen { get; set; }
		[JsonPropertyName("created_at")]
		public DateTime? CreatedAt { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			Id.GetIDString(prefixIndex);
	}
}