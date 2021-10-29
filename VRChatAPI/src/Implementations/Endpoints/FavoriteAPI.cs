using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IFavoriteAPI
	{
		private const string favoriteEndpoint = "favorites";
		private const string favoriteGroupEndpoint = "favorite/group";
		private Task<Favorite> AddFavorite(IVRCObject obj, string[] tags, EFavoriteType type, CancellationToken ct) =>
			client.Post<Favorite, Dictionary<string, object>>(favoriteEndpoint, new Dictionary<string, object> {
				{ "type", type },
				{ "favoriteId", obj.GetIDString() },
				{ "tags", tags },
			}, ct);
		public Task<Favorite> AddFavorite(IAvatar avatar, string[] tags, CancellationToken ct = default) =>
			AddFavorite(avatar, tags, EFavoriteType.avatar, ct);

		public Task<Favorite> AddFavorite(IWorld world, string[] tags, CancellationToken ct = default) =>
			AddFavorite(world, tags, EFavoriteType.world, ct);

		public Task<Favorite> AddFavorite(IUser friend, string[] tags, CancellationToken ct = default) =>
			AddFavorite(friend, tags, EFavoriteType.friend, ct);

		public Task<ResponseMessage> Delete(IFavorite obj, CancellationToken ct = default) =>
			client.Delete<ResponseMessage>($"{favoriteEndpoint}/{obj.GetIDString()}", ct);

		public Task<ResponseMessage> Delete(IFavoriteGroup obj, CancellationToken ct = default) =>
			client.Delete<ResponseMessage>($"{favoriteGroupEndpoint}/{obj.FavoriteGroupType}/{obj.FavoriteGroupName}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<Favorite>> Get(FavoriteSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<Favorite>>(
				$@"{favoriteEndpoint}?{
					QueryConstructor.MakeQuery(option, serializerOption)}&n={n}&offset={offset}", ct);

		public Task<Favorite> Get(IFavorite obj, CancellationToken ct = default) =>
			client.Get<Favorite>($"{favoriteEndpoint}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<FavoriteGroup>> Get(FavoriteGroupSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<FavoriteGroup>>(
				$@"{favoriteGroupEndpoint}?{
					QueryConstructor.MakeQuery(option, serializerOption)}&n={n}&offset={offset}", ct);

		public Task<FavoriteGroup> Get(IFavoriteGroup obj, CancellationToken ct = default) =>
			client.Get<FavoriteGroup>($"{favoriteGroupEndpoint}/{obj.FavoriteGroupType}/{obj.FavoriteGroupName}/{obj.GetIDString()}", ct);

		public Task<FavoriteGroup> Update(IFavoriteGroup from, FavoriteGroup to, CancellationToken ct = default) =>
			client.Put<FavoriteGroup, FavoriteGroup>($"{favoriteGroupEndpoint}/{from.FavoriteGroupType}/{from.FavoriteGroupName}/{from.GetIDString()}", to, ct);
	}
}
