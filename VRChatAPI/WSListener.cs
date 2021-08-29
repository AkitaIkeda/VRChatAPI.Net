using System;
using System.Net.WebSockets;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace VRChatAPI
{
	[JsonConverter(typeof(StringEnumConverter))]
	[DataContract]
	public enum EventTypes
	{
		[EnumMember(Value="friend-online")]
		FriendOnline,
		[EnumMember(Value="friend-offline")]
		FriendOffline,
		[EnumMember(Value="friend-active")]
		FriendActive,
		[EnumMember(Value="friend-add")]
		FriendAdd,
		[EnumMember(Value="friend-delete")]
		FriendDelete,
		[EnumMember(Value="friend-update")]
		FriendUpdate,
		[EnumMember(Value="friend-location")]
		FriendLocation,
		[EnumMember(Value="notification")]
		Notification,
		[EnumMember(Value="see-notification")]
		SeeNotification,
		[EnumMember(Value="hide-notification")]
		HideNotification,
		[EnumMember(Value="clear-notification")]
		ClearNotification,
		[EnumMember(Value="user-update")]
		UserUpdate,
	}

	public class EventMessage
	{
		public string content { get; set; }
		public EventTypes type { get; set; }
	}

	/// <summary>
	/// Lister for VRC API websocket pipeline
	/// </summary>
	/// <remarks>
	/// Assign EventHander to "On" events
	/// </remarks>
	public class WSListener
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<WSListener>();
		public delegate void EventHandler(EventMessage message);
		public delegate void FriendOnlineEventHandler(User user);
		public delegate void FriendOfflineEventHandler(UserId userId);
		public delegate void FriendActiveEventHandler(User user);
		public delegate void FriendAddEventHandler(User user);
		public delegate void FriendDeleteEventHandler(UserId userId);
		public delegate void FriendUpdateEventHandler(User user);
		public delegate void FriendLocationEventHandler(User user, World world, Location location, bool canRequestInvite);
		public delegate void NotificationEventHandler(Notification notification);
		public delegate void SeeNotificationEventHandler(NotificationId notificationId);
		public delegate void HideNotificationEventHandler(NotificationId notificationId);
		public delegate void ClearNotificationEventHandler();
		public delegate void UserUpdateEventHandler(CurrentUser user);

		public event EventHandler OnEvent;
		public event FriendOnlineEventHandler OnFriendOnline;
		public event FriendOfflineEventHandler OnFriendOffline;
		public event FriendActiveEventHandler OnFriendActive;
		public event FriendAddEventHandler OnFriendAdd;
		public event FriendDeleteEventHandler OnFriendDelete;
		public event FriendUpdateEventHandler OnFriendUpdate;
		public event FriendLocationEventHandler OnFriendLocation;
		public event NotificationEventHandler OnNotification;
		public event SeeNotificationEventHandler OnSeeNotification;
		public event HideNotificationEventHandler OnHideNotification;
		public event ClearNotificationEventHandler OnClearNotification;
		public event UserUpdateEventHandler OnUserUpdate;

		internal WSListener(){}

		public async Task Listen(
			string authToken, 
			CancellationToken ct, 
			EventHandler logging = null)
		{
			Logger.LogDebug("Start Listenning to websocket pipeline");

			if(logging is null)
				logging = (EventHandler) (m => Logger.LogInformation(JObject.FromObject(m).ToString()));
			if(!(logging is null)) OnEvent += logging;

			using(var ws = new ClientWebSocket())
			{
				await ws.ConnectAsync(new Uri($"wss://pipeline.vrchat.cloud/?authToken={authToken}"), ct);

				var buffer = new ArraySegment<byte>(new byte[0x10000]);
				while (!ct.IsCancellationRequested)
				{
					var r = await ws.ReceiveAsync(buffer, ct);
					var message = JsonConvert.DeserializeObject<EventMessage>(Encoding.UTF8.GetString(buffer.ToArray()));
					
					OnEvent(message);
					JObject content;
					switch (message.type)
					{
						case EventTypes.SeeNotification:
						case EventTypes.HideNotification:
							content = default(JObject);
							break;
						default:
							content = JObject.Parse(message.content);
							break;
					}
					switch (message.type)
					{
						case EventTypes.FriendOnline:
							OnFriendOnline(content["user"].ToObject<User>());
							break;
						case EventTypes.FriendOffline:
							OnFriendOffline(content["userId"].ToObject<UserId>());
							break;
						case EventTypes.FriendActive:
							OnFriendActive(content["user"].ToObject<User>());
							break;
						case EventTypes.FriendAdd:
							OnFriendAdd(content["user"].ToObject<User>());
							break;
						case EventTypes.FriendDelete:
							OnFriendDelete(content["userId"].ToObject<UserId>());
							break;
						case EventTypes.FriendUpdate:
							OnFriendUpdate(content["user"].ToObject<User>());
							break;
						case EventTypes.FriendLocation:
							OnFriendLocation(
								content["user"].ToObject<User>(), 
								content["world"].ToObject<World>(),
								content["location"].ToObject<Location>(),
								((bool)content["canRequestInvite"]));
							break;
						case EventTypes.Notification:
							OnNotification(content.ToObject<Notification>());
							break;
						case EventTypes.SeeNotification:
							OnSeeNotification(message.content);
							break;
						case EventTypes.HideNotification:
							OnHideNotification(message.content);
							break;
						case EventTypes.ClearNotification:
							OnClearNotification();
							break;
						case EventTypes.UserUpdate:
							OnUserUpdate(content["user"].ToObject<CurrentUser>());
							break;
						default:
							Logger.LogError($"Unknown event type");
							break;
					}
				}
			}
		}
	}
}