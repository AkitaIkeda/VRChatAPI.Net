using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : INotificationAPI
	{
		private const string notificationEndpoint = "auth/user/notifications";
		public Task<ResponseMessage> AcceptFriendRequest(INotification obj, CancellationToken ct = default) =>
			client.Put<ResponseMessage>($"{notificationEndpoint}/{obj.GetIDString()}/accept", ct);

		public Task<ResponseMessage> ClearNotification(CancellationToken ct = default) =>
			client.Put<ResponseMessage>($"{notificationEndpoint}/clear", ct);

		public Task<IEnumerable<Notification>> Get(NotificationSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<Notification>>(
				$@"{notificationEndpoint}?{
					QueryConstructor.MakeQuery(option, serializerOption)}&n={n}&offset={offset}", ct);

		public Task<Notification> Get(INotification obj, CancellationToken ct = default) =>
			client.Get<Notification>($"{notificationEndpoint}/{obj.GetIDString()}", ct);

		public Task<Notification> Hide(INotification obj, CancellationToken ct = default) =>
			client.Put<Notification>($"{notificationEndpoint}/{obj.GetIDString()}/hide", ct);

		public Task<Notification> MarkAsRead(INotification obj, CancellationToken ct = default) =>
			client.Put<Notification>($"{notificationEndpoint}/{obj.GetIDString()}/see", ct);
	}
}
