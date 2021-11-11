using System;
using static VRChatAPI.Objects.EventHandlerDelegates;

namespace VRChatAPI.Interfaces
{
	public interface IWSEventHandler
	{
		event VRCEventHandler OnEvent;
		event FriendOnlineEventHandler OnFriendOnline;
		event FriendOfflineEventHandler OnFriendOffline;
		event FriendActiveEventHandler OnFriendActive;
		event FriendAddEventHandler OnFriendAdd;
		event FriendDeleteEventHandler OnFriendDelete;
		event FriendUpdateEventHandler OnFriendUpdate;
		event FriendLocationEventHandler OnFriendLocation;
		event NotificationEventHandler OnNotification;
		event SeeNotificationEventHandler OnSeeNotification;
		event HideNotificationEventHandler OnHideNotification;
		event ClearNotificationEventHandler OnClearNotification;
		event UserUpdateEventHandler OnUserUpdate;
		event EventHandler OnStopHandling;

		void StartHandling(ITokenCredential cred);
		void StopHandling();
		bool IsHandling { get; }
	}
}