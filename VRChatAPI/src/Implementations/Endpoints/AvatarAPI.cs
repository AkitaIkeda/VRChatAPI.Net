using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IAvatarAPI
	{
		private const string avatarEndpoint = "avatars";
		public Task<Avatar> Create(AvatarCreateParams param, CancellationToken ct = default) =>
			client.Post<Avatar, AvatarCreateParams>(avatarEndpoint, param, ct);

		public Task<Avatar> Delete(IAvatar obj, CancellationToken ct = default) =>
			client.Delete<Avatar>($"{avatarEndpoint}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<Avatar>> Get(AvatarSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<Avatar>>(
				$@"{avatarEndpoint}{(option.Favorites ? "/favorites" : "")}?{
					QueryConstructor.MakeQuery(option, serializerOption)}&n={n}&offset={offset}", ct);


		public Task<Avatar> Get(IAvatar obj, CancellationToken ct = default) =>
			client.Get<Avatar>($"{avatarEndpoint}/{obj.GetIDString()}", ct);

		public Task<CurrentUser> Select(IAvatar id, CancellationToken ct = default) =>
			client.Put<CurrentUser>($"{avatarEndpoint}/{id.GetIDString()}/select", ct);

		public Task<CurrentUser> SelectAsFallback(IAvatar id, CancellationToken ct = default) =>
			client.Put<CurrentUser>($"{avatarEndpoint}/{id.GetIDString()}/selectfallback", ct);

		public Task<Avatar> Update(IAvatar from, Avatar to, CancellationToken ct = default) =>
			client.Put<Avatar, Avatar>($"{avatarEndpoint}/{from.GetIDString()}", to, ct);
	}
}
