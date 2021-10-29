using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Enums;
using VRChatAPI.Extentions;
using VRChatAPI.Extentions.DependancyInjection;
using VRChatAPI.Interfaces;
using VRChatAPI.Logging;
using VRChatAPI.Objects;
using static VRChatAPI.Objects.EventHandlerDelegates;

namespace VRChatAPI.Implementations
{
	internal class WSEventHandler : IWSEventHandler, IDisposable
	{
		private readonly IOptions<VRCAPIOptions> options;
		private readonly ILogger logger;
		private Task listenerTask;
		private CancellationTokenSource listenerCTS;
		private JsonSerializerOptions serializerOption;

		public WSEventHandler(
			IOptions<VRCAPIOptions> options,
			ILogger<WSEventHandler> logger)
		{
			logger.LogInformation(LogEventID.SystemInitialize, "Initialize.");
			this.options = options;
			serializerOption = options.Value.SerializerOption;
			this.logger = (ILogger)logger ?? NullLogger.Instance;
		}

		#region interface impl
		public bool IsHandling => !(listenerTask is null);

		public event VRCEventHandler OnEvent;
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

		public void Dispose()
		{
			logger.LogInformation(LogEventID.SystemDispose, "Dispose.");
			listenerTask?.Dispose();
			listenerCTS?.Dispose();
		}

		public void StartHandling(ITokenCredential cred)
		{
			logger.LogInformation(LogEventID.SystemStart, "Start Handling.");
			lock (listenerTask)
			{
				if (IsHandling) throw new InvalidOperationException("It's already started.");
				var uri = $"{options.Value.WSEndpoint}?authToken={cred.AuthToken}";
				listenerCTS = new CancellationTokenSource();
				listenerTask = Listener(new Uri(uri), listenerCTS.Token);
				listenerTask.Start();
			}
		}

		public void StopHandling()
		{
			logger.LogInformation(LogEventID.SystemStart, "Stop Handling.");
			lock (listenerTask)
			{
				if (!IsHandling) throw new InvalidOperationException("It hasn't been started.");
				listenerCTS.Cancel();
				try
				{
					listenerTask.Wait();
				}
				catch (AggregateException e)
				{
					if (!(e.InnerException is TaskCanceledException))
						throw e;
				}
				finally
				{
					listenerTask.Dispose();
					listenerTask = null;
				}
			}
		}
		#endregion

		private async Task Listener(Uri uri, CancellationToken ct)
		{
			using (logger.BeginScope(nameof(Listener))) 
			{
				logger.LogInformation(LogEventID.SystemStart, "Start Listenning to WS Endpoint. Url: {Url}", uri);
				using (var ws = new ClientWebSocket())
				{
					logger.LogDebug(LogEventID.Connect, "Connect to {Url}", uri);
					await ws.ConnectAsync(uri, ct);
					var buffer = new ArraySegment<byte>(new byte[options.Value.EventHandlerBufferSize]);
					while (!ct.IsCancellationRequested)
					{
						var r = await ws.ReceiveAsync(buffer, ct);
						string received = Encoding.UTF8.GetString(buffer.Array);
						
						EventMessage message;
						try
						{
							message = JsonSerializer.Deserialize<EventMessage>(received, serializerOption);
						}
						catch (JsonException e)
						{
							logger.LogError(LogEventID.DeserializationError, e, $"Couldn't deserialize received text, probably due to the changes of API Implementation. Please Send Issue to the {ProjectDescription.RepoURL}.");
							if (options.Value.StopWSHandlerOnException)
							{
								logger.LogInformation(LogEventID.SystemStop, "Stop Listenning.");
								break;
							}
							else continue;
						}
						logger.LogDebug(LogEventID.Receive, "Received: {Message}", message);

						logger.LogInformation(LogEventID.EventIgnition, "Event received. Type: {Type}", message.type);
						OnEvent(message);

						try
						{
							JsonElement content;
							switch (message.type)
							{
								case EEventType.SeeNotification:
								case EEventType.HideNotification:
									content = default;
									break;
								default:
									using (var jsonDocument = JsonDocument.Parse(message.content))
										content = jsonDocument.RootElement.Clone();
									break;
							}
							switch (message.type)
							{
								case EEventType.FriendOnline:
									OnFriendOnline(content.GetProperty("user").Deserialize<User>(serializerOption));
									break;
								case EEventType.FriendOffline:
									OnFriendOffline(content.GetProperty("userId").Deserialize<UserID>(serializerOption));
									break;
								case EEventType.FriendActive:
									OnFriendActive(content.GetProperty("user").Deserialize<User>(serializerOption));
									break;
								case EEventType.FriendAdd:
									OnFriendAdd(content.GetProperty("user").Deserialize<User>(serializerOption));
									break;
								case EEventType.FriendDelete:
									OnFriendDelete(content.GetProperty("userId").Deserialize<UserID>(serializerOption));
									break;
								case EEventType.FriendUpdate:
									OnFriendUpdate(content.GetProperty("user").Deserialize<User>(serializerOption));
									break;
								case EEventType.FriendLocation:
									OnFriendLocation(
										content.GetProperty("user").Deserialize<User>(serializerOption),
										content.GetProperty("world").Deserialize<World>(serializerOption),
										content.GetProperty("location").Deserialize<Location>(serializerOption),
										content.GetProperty("canRequestInvite").GetBoolean());
									break;
								case EEventType.Notification:
									OnNotification(content.Deserialize<Notification>(serializerOption));
									break;
								case EEventType.SeeNotification:
									OnSeeNotification(NotificationID.Parse(message.content));
									break;
								case EEventType.HideNotification:
									OnHideNotification(NotificationID.Parse(message.content));
									break;
								case EEventType.ClearNotification:
									OnClearNotification();
									break;
								case EEventType.UserUpdate:
									OnUserUpdate(content.GetProperty("user").Deserialize<CurrentUser>(serializerOption));
									break;
							}
						}
						catch (Exception e)
						{
							logger.LogError(e, "Error occerd during processing event. Message: {Message}", message);
							if (options.Value.StopWSHandlerOnException)
							{
								logger.LogInformation(LogEventID.SystemStop, "Stop Listenning.");
								break;
							}
							else continue;
						}
					}
				}
				listenerTask = null;
			}
		}
	}
}