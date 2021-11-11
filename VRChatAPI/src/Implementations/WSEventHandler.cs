using Microsoft.Extensions.DependencyInjection;
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
using VRChatAPI.Extentions.DependencyInjection;
using VRChatAPI.Interfaces;
using VRChatAPI.Logging;
using VRChatAPI.Objects;
using VRChatAPI.Utils;
using static VRChatAPI.Objects.EventHandlerDelegates;

namespace VRChatAPI.Implementations
{
	internal class WSEventHandler : IWSEventHandler, IDisposable
	{
		private readonly IOptions<VRCAPIOptions> options;
		private readonly IAPIWebSocketProvider wsFactory;
		private readonly ILogger logger;
		private Task listenerTask;
		private CancellationTokenSource listenerCTS;
		private JsonSerializerOptions serializerOption;

		public WSEventHandler(
			IAPIWebSocketProvider wsFactory,
			IOptions<VRCAPIOptions> options,
			ILogger<WSEventHandler> logger)
		{
			logger.LogInformation(LogEventID.SystemInitialize, "Initialize.");
			this.wsFactory = wsFactory;
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
		public event EventHandler OnStopHandling;

		public void Dispose()
		{
			logger.LogInformation(LogEventID.SystemDispose, "Dispose.");
			listenerTask?.Dispose();
			listenerCTS?.Dispose();
		}

		public void StartHandling(ITokenCredential cred)
		{
			logger.LogInformation(LogEventID.SystemStart, "Start Handling.");
			if (IsHandling) throw new InvalidOperationException("It's already started.");
			var uri = $"{options.Value.WSEndpoint}?authToken={cred.AuthToken}";
			listenerCTS = new CancellationTokenSource();
			listenerTask = Listener(new Uri(uri), listenerCTS.Token);
		}

		public void StopHandling()
		{
			logger.LogInformation(LogEventID.SystemStart, "Stop Handling.");
			if (!IsHandling) throw new InvalidOperationException("It hasn't been started.");
			listenerCTS.Cancel();
			try
			{
				listenerTask.Wait();
			}
			catch (AggregateException e)
			{
				if (!(e.InnerException is TaskCanceledException 
							|| e.InnerException is OperationCanceledException))
					throw e;
			}
			finally
			{
				listenerTask?.Dispose();
			}
		}
		#endregion

		private EventMessage DeserializeMessage(string m){
			var message = JsonSerializer.Deserialize<EventMessage>(m, serializerOption);
			logger.LogDebug(LogEventID.Receive, "Received: {Message}", message);
			return message;
		}

		private void InvokeEvent(EventMessage message){
			OnEvent.Invoke(message);
			JsonElement content;
			switch (message.type)
			{
				case EEventType.SeeNotification:
				case EEventType.HideNotification:
				case EEventType.ClearNotification:
					content = default;
					break;
				default:
					content = JsonSerializer.Deserialize<JsonElement>(message.content, serializerOption);
					break;
			}
			switch (message.type)
			{
				case EEventType.FriendOnline:
					OnFriendOnline?.Invoke(content.GetProperty("user").Deserialize<User>(serializerOption));
					break;
				case EEventType.FriendOffline:
					OnFriendOffline?.Invoke(content.GetProperty("userId").Deserialize<UserID>(serializerOption));
					break;
				case EEventType.FriendActive:
					OnFriendActive?.Invoke(content.GetProperty("user").Deserialize<User>(serializerOption));
					break;
				case EEventType.FriendAdd:
					OnFriendAdd?.Invoke(content.GetProperty("user").Deserialize<User>(serializerOption));
					break;
				case EEventType.FriendDelete:
					OnFriendDelete?.Invoke(content.GetProperty("userId").Deserialize<UserID>(serializerOption));
					break;
				case EEventType.FriendUpdate:
					OnFriendUpdate?.Invoke(content.GetProperty("user").Deserialize<User>(serializerOption));
					break;
				case EEventType.FriendLocation:
					OnFriendLocation?.Invoke(
						content.GetProperty("user").Deserialize<User>(serializerOption),
						content.GetProperty("world").Deserialize<World>(serializerOption),
						content.GetProperty("location").Deserialize<Location>(serializerOption),
						content.GetProperty("canRequestInvite").GetBoolean());
					break;
				case EEventType.Notification:
					OnNotification?.Invoke(content.Deserialize<Notification>(serializerOption));
					break;
				case EEventType.SeeNotification:
					OnSeeNotification?.Invoke(NotificationID.Parse(message.content));
					break;
				case EEventType.HideNotification:
					OnHideNotification?.Invoke(NotificationID.Parse(message.content));
					break;
				case EEventType.ClearNotification:
					OnClearNotification?.Invoke();
					break;
				case EEventType.UserUpdate:
					OnUserUpdate?.Invoke(content.GetProperty("user").Deserialize<CurrentUser>(serializerOption));
					break;
			}
		}

		private async Task Listener(Uri uri, CancellationToken ct)
		{
			using (logger.BeginScope(nameof(Listener))) 
			{
				try
				{
					logger.LogInformation(LogEventID.SystemStart, "Start Listenning to WS Endpoint. Url: {Url}", uri);
					IAPIWebSocket ws;
					try
					{
						ws = await wsFactory.Create(uri, ct);
					}
					catch(Exception e)
					{
						logger.LogCritical(e, "Couldn't connect to {Url}. Stop Listener.", uri);
						return;
					}
					try{
						logger.LogDebug(LogEventID.Connect, "Connect to {Url}", uri);
						var buffer = new ArraySegment<byte>(new byte[options.Value.EventHandlerBufferSize]);
						while (!ct.IsCancellationRequested)
						{
							WebSocketReceiveResult r;
							try
							{
								r = await ws.Receive(buffer, ct);
							}
							catch(Exception e)
							{
								logger.LogCritical(e, "Couldn't receive message. Stop Listener.", uri);
								throw;
							}
							string received = Encoding.UTF8.GetString(buffer.Array, 0, r.Count);
							
							ProcessEvent(received);
						}
					}
					finally{
						wsFactory.DisposeWS();
					}
				}
				finally{
					logger.LogInformation(LogEventID.SystemStop, "Stop Listenning.");
					OnStopHandling?.Invoke(this, EventArgs.Empty);
					listenerTask = null;
				}
			}
		}

		private void ProcessEvent(string received)
		{
			EventMessage message;
			try
			{
				message = DeserializeMessage(received);
			}
			catch (JsonException e)
			{
				logger.LogError(LogEventID.DeserializationError, e, $"Couldn't deserialize received text, probably due to the changes of API Implementation. Please Send a Issue to the {ProjectDescription.RepoURL}.");
				if(options.Value.StopWSHandlerOnException) throw;
				else return;
			}
			catch (Exception e)
			{
				logger.LogCritical(LogEventID.DeserializationError, e, $"Couldn't deserialize received. Please Send a Issue to the {ProjectDescription.RepoURL}.");
				throw;
			}

			logger.LogInformation(LogEventID.EventIgnition, "Event received. Type: {Type}", message.type);

			try{
				InvokeEvent(message);

			}
			catch (Exception e){
				logger.LogError(e, "Error occurred during processing event. Message: {Message}", message);
				if (options.Value.StopWSHandlerOnException) throw;
			}
		}
	}
}