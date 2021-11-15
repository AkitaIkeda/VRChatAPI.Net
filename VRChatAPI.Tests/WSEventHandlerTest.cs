using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using VRChatAPI.Implementations;
using VRChatAPI.Interfaces;
using VRChatAPI.Tests.Helper;
using Xunit;
using FluentAssertions;
using VRChatAPI.Enums;
using System.Text.Json;
using VRChatAPI.Objects;
using VRChatAPI.Tests.Helper.Object;

namespace VRChatAPI.Tests{
	public sealed class WSEventHandlerTest : UseDI
	{
		private BlockingCollection<string> WSResponse;
		private IWSEventHandler handler;

		public WSEventHandlerTest() : base(s => 
			s	.AddScoped<Mock<IAPIWebSocketProvider>>()
				.AddScoped<IAPIWebSocketProvider>(_ => 
					_.GetRequiredService<Mock<IAPIWebSocketProvider>>().Object)
				.AddScoped<IWSEventHandler, WSEventHandler>())
		{
			WSResponse = new BlockingCollection<string>();
			var m = services.GetRequiredService<Mock<IAPIWebSocketProvider>>();
			m.Setup(x => x.Create(It.IsAny<Uri>(), It.IsAny<CancellationToken>()))
				.Returns(() =>
				{
					var mo = new Mock<IAPIWebSocket>();
					mo.Setup(x => x.Receive(
						It.IsAny<ArraySegment<byte>>(),
						It.IsAny<CancellationToken>()))
						.Returns<ArraySegment<byte>, CancellationToken>((seg, ct) =>
						{
							return Task.Run<WebSocketReceiveResult>(() => {
								var s = WSResponse.Take(ct);
								new ArraySegment<byte>(Encoding.UTF8.GetBytes(s)).CopyTo(seg);
								return new WebSocketReceiveResult(s.Length, WebSocketMessageType.Text, true);
							});
						});
					return Task.FromResult(mo.Object);
				});
			handler = services.GetRequiredService<IWSEventHandler>();
		}

		[Fact]
		public void StartAndStopTest(){
			handler.IsHandling.Should().BeFalse();
			handler.StartHandling(new TokenCredential("test"));
			handler.IsHandling.Should().BeTrue();
			handler.StopHandling();
			handler.IsHandling.Should().BeFalse();
		}

		[Fact]
		public void EventHandlerTest(){
			var monitor = handler.Monitor();
			handler.StartHandling(new TokenCredential("test"));
			var d = new DummyObjectGenerator();
			var m = monitor.Should();
			m.NotRaise(nameof(handler.OnEvent));
			m.NotRaise(nameof(handler.OnFriendOnline));
			m.NotRaise(nameof(handler.OnFriendOffline));
			m.NotRaise(nameof(handler.OnFriendActive));
			m.NotRaise(nameof(handler.OnFriendAdd));
			m.NotRaise(nameof(handler.OnFriendDelete));
			m.NotRaise(nameof(handler.OnFriendUpdate));
			m.NotRaise(nameof(handler.OnFriendLocation));
			m.NotRaise(nameof(handler.OnNotification));
			m.NotRaise(nameof(handler.OnSeeNotification));
			m.NotRaise(nameof(handler.OnHideNotification));
			m.NotRaise(nameof(handler.OnClearNotification));
			m.NotRaise(nameof(handler.OnUserUpdate));
			m.NotRaise(nameof(handler.OnStopHandling));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendOnline , content = $@"{{""user"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(User)), serializerOptions)}}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendOffline , content = $@"{{""userId"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(UserID)), serializerOptions)}}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendActive , content = $@"{{""user"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(User)), serializerOptions)}}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendAdd , content = $@"{{""user"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(User)), serializerOptions)}}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendDelete , content = $@"{{""userId"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(UserID)), serializerOptions)}}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendUpdate , content = $@"{{""user"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(User)), serializerOptions)}}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.FriendLocation , content = $@"{{""user"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(User)), serializerOptions)}, ""world"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(World)), serializerOptions)}, ""location"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(Location)), serializerOptions)}, ""canRequestInvite"":false}}" }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.Notification , content = JsonSerializer.Serialize(d.GetDefaultObject(typeof(Notification)), serializerOptions) }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.SeeNotification , content = JsonSerializer.Serialize(d.GetDefaultObject(typeof(NotificationID)), serializerOptions) }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.HideNotification , content = JsonSerializer.Serialize(d.GetDefaultObject(typeof(NotificationID)), serializerOptions) }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.ClearNotification , content = string.Empty }, serializerOptions));
			WSResponse.Add(JsonSerializer.Serialize(new EventMessage{ type = EEventType.UserUpdate , content = $@"{{""user"":{JsonSerializer.Serialize(d.GetDefaultObject(typeof(CurrentUser)), serializerOptions)}}}" }, serializerOptions));
			handler.IsHandling.Should().BeTrue();
			m.NotRaise(nameof(handler.OnStopHandling));
			while(WSResponse.Count != 0) Thread.Sleep(100);
			handler.StopHandling();
			m.Raise(nameof(handler.OnEvent));
			m.Raise(nameof(handler.OnFriendOnline));
			m.Raise(nameof(handler.OnFriendOffline));
			m.Raise(nameof(handler.OnFriendActive));
			m.Raise(nameof(handler.OnFriendAdd));
			m.Raise(nameof(handler.OnFriendDelete));
			m.Raise(nameof(handler.OnFriendUpdate));
			m.Raise(nameof(handler.OnFriendLocation));
			m.Raise(nameof(handler.OnNotification));
			m.Raise(nameof(handler.OnSeeNotification));
			m.Raise(nameof(handler.OnHideNotification));
			m.Raise(nameof(handler.OnClearNotification));
			m.Raise(nameof(handler.OnUserUpdate));
			m.Raise(nameof(handler.OnStopHandling));
			handler.IsHandling.Should().BeFalse();
		}

		public override void Dispose()
		{
			WSResponse?.Dispose();
			base.Dispose();
		}
	}
}