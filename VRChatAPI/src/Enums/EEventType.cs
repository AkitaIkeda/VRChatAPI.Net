using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EEventType
	{
		[EnumMember(Value = "friend-online")]
		FriendOnline,
		[EnumMember(Value = "friend-offline")]
		FriendOffline,
		[EnumMember(Value = "friend-active")]
		FriendActive,
		[EnumMember(Value = "friend-add")]
		FriendAdd,
		[EnumMember(Value = "friend-delete")]
		FriendDelete,
		[EnumMember(Value = "friend-update")]
		FriendUpdate,
		[EnumMember(Value = "friend-location")]
		FriendLocation,
		[EnumMember(Value = "notification")]
		Notification,
		[EnumMember(Value = "see-notification")]
		SeeNotification,
		[EnumMember(Value = "hide-notification")]
		HideNotification,
		[EnumMember(Value = "clear-notification")]
		ClearNotification,
		[EnumMember(Value = "user-update")]
		UserUpdate,
	}
}
