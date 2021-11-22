using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IUserAPI
	{
		private const string usersEndpoint = "users";
		private const string friendsEndpoint = "friends";

		public Task<CurrentUser> AddTags(IEnumerable<string> tags, CancellationToken ct = default) =>
			AddTags(User, tags, ct);

		public Task<CurrentUser> AddTags(IUser user, IEnumerable<string> tags, CancellationToken ct = default) =>
			client.Post<CurrentUser, Dictionary<string, object>>(
				$"{usersEndpoint}/{user.GetIDString()}/addTags", 
				new Dictionary<string, object>{{ "tags", tags }}, ct);

		public Task<ResponseMessage> CancelFriendRequest(IUser user, CancellationToken ct = default) =>
			client.Delete<ResponseMessage>($"{userEndpoint}/{user.GetIDString()}/friendRequest", ct);

		public Task<User> Get(string username, CancellationToken ct = default) =>
			client.Get<User>($"{usersEndpoint}/{username}/name", ct);

		public Task<IEnumerable<LimitedUser>> Get(UserSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<LimitedUser>>($"{usersEndpoint}?{QueryConstructor.MakeQuery(option, serializerOption)}&n={n}&offset={offset}", ct);

		public Task<User> Get(IUser obj, CancellationToken ct = default) =>
			client.Get<User>($"{usersEndpoint}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<LimitedUser>> GetFriends(bool? offline, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<LimitedUser>>($@"{authEndpoint}/{userEndpoint}/{friendsEndpoint}?{QueryConstructor.MakeQuery(
				new Dictionary<string, object>
				{
					{ "offline", offline },
					{ "n", n },
					{ "offset", offset },
				}, serializerOption)}", ct);

		public async Task<EFriendStatus> GetFriendStatus(IUser user, CancellationToken ct = default)
		{
			var r = await client.Get<JsonElement>($"{userEndpoint}/{user.GetIDString()}/friendStatus", ct);
			switch (r)
			{
				case var _ when r.GetProperty("isFriend").GetBoolean():
					return EFriendStatus.Friend;
				case var _ when r.GetProperty("incomingRequest").GetBoolean():
					return EFriendStatus.IncomingRequest;
				case var _ when r.GetProperty("outgoingRequest").GetBoolean():
					return EFriendStatus.OutgoingRequest;
				default:
					return EFriendStatus.NotFriend;
			}
		}

		public Task<CurrentUser> RemoveTags(IEnumerable<string> tags, CancellationToken ct = default) =>
			RemoveTags(User, tags, ct);

		public Task<CurrentUser> RemoveTags(IUser user, IEnumerable<string> tags, CancellationToken ct = default) =>
			client.Post<CurrentUser, Dictionary<string, object>>(
				$"{usersEndpoint}/{User.GetIDString()}/addTags", 
				new Dictionary<string, object>{{ "tags", tags }}, ct);

		public Task<Notification> SendFriendRequest(IUser user, CancellationToken ct = default) =>
			client.Post<Notification>($"user/{user.GetIDString()}/friendRequest", ct);

		public Task<ResponseMessage> Unfriend(IUser user, CancellationToken ct = default) =>
			client.Delete<ResponseMessage>($"{authEndpoint}/{userEndpoint}/{friendsEndpoint}", ct);

		public Task<CurrentUser> Update(IUser from, CurrentUser to, CancellationToken ct = default) =>
			client.Put<CurrentUser, CurrentUser>($"users/{from.GetIDString()}", to, ct);
	}
}
