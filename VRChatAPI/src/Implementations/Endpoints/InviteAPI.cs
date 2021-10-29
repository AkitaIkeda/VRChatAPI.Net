using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;

namespace VRChatAPI.Implementations
{
	partial class Session : IInviteAPI
	{
		private const string messageEndpoint = "message";
		public Task<IEnumerable<Message>> Delete(Message obj, CancellationToken ct = default) =>
			client.Delete<IEnumerable<Message>>(
				$"{messageEndpoint}/{User.GetIDString()}/{obj.MessageType}/{obj.GetIDString()}", ct);

		public Task<Message> Get(Message obj, CancellationToken ct = default) =>
			Get(User, obj, ct);
		public Task<Message> Get(IUser user, Message obj, CancellationToken ct = default) =>
			client.Get<Message>($"{messageEndpoint}/{user.GetIDString()}/{obj.MessageType}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<Message>> Get(EMessageType type, CancellationToken ct = default) =>
			Get(User, type, ct);
		public Task<IEnumerable<Message>> Get(IUser user, EMessageType type, CancellationToken ct = default) =>
			client.Get<IEnumerable<Message>>($"{messageEndpoint}/{user.GetIDString()}/{type}", ct);

		public Task<Notification> Invite(IUser user, IInstance location, int? messageSlot = null, CancellationToken ct = default) =>
			client.Post<Notification, Dictionary<string, object>>($"invite/{user.GetIDString()}", new Dictionary<string, object>{
				{ "instanceId", location.GetLocationString() },
				{ "messageSlot", messageSlot },
			}, ct);

		public Task<Notification> RequestInvite(IUser user, int? messageSlot = null, CancellationToken ct = default) =>
			client.Post<Notification, Dictionary<string, object>>($"requestInvite/{user.GetIDString()}", new Dictionary<string, object>{
				{ "requestSlot", messageSlot },
			}, ct);

		public Task<Notification> RespondInvite(INotification invite, int? messageSlot = null, CancellationToken ct = default) =>
			client.Post<Notification, Dictionary<string, object>>($"invite/{invite.GetIDString()}/response", new Dictionary<string, object>{
				{ "responseSlot", messageSlot },
			}, ct);

		public Task<Notification> RespondRequestInvite(INotification requestInvite, int? messageSlot = null, CancellationToken ct = default)
		{
			throw new NotImplementedException("WIP");
		}

		public Task<IEnumerable<Message>> Update(Message from, Message to, CancellationToken ct = default) =>
			Update(User, from, to, ct);
		public Task<IEnumerable<Message>> Update(IUser user, Message from, Message to, CancellationToken ct = default) =>
		client.Put<IEnumerable<Message>, Message>(
			$"{messageEndpoint}/{user.GetIDString()}/{from.MessageType}/{from.GetIDString()}",
			to, ct);
	}
}
