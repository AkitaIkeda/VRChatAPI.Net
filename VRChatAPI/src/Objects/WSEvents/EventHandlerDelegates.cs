namespace VRChatAPI.Objects
{
	public static class EventHandlerDelegates
	{
		public delegate void VRCEventHandler(EventMessage message);
		public delegate void FriendOnlineEventHandler(User user);
		public delegate void FriendOfflineEventHandler(UserID userId);
		public delegate void FriendActiveEventHandler(User user);
		public delegate void FriendAddEventHandler(User user);
		public delegate void FriendDeleteEventHandler(UserID userId);
		public delegate void FriendUpdateEventHandler(User user);
		public delegate void FriendLocationEventHandler(User user, World world, Location location, bool canRequestInvite);
		public delegate void NotificationEventHandler(Notification notification);
		public delegate void SeeNotificationEventHandler(NotificationID notificationId);
		public delegate void HideNotificationEventHandler(NotificationID notificationId);
		public delegate void ClearNotificationEventHandler();
		public delegate void UserUpdateEventHandler(CurrentUser user);
	}
}
