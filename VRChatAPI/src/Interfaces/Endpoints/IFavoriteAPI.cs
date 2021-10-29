using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IFavoriteAPI :
		IGet<Favorite, IFavorite, FavoriteSearchParams>,
		IDelete<ResponseMessage, IFavorite>,
		IGet<FavoriteGroup, IFavoriteGroup, FavoriteGroupSearchParams>,
		IUpdate<IFavoriteGroup, FavoriteGroup>, IDelete<ResponseMessage, IFavoriteGroup>
	{
		Task<Favorite> AddFavorite(
			IAvatar avatar,
			string[] tags,
			CancellationToken ct = default);
		Task<Favorite> AddFavorite(
			IWorld world,
			string[] tags,
			CancellationToken ct = default);
		Task<Favorite> AddFavorite(
			IUser friend,
			string[] tags,
			CancellationToken ct = default);
	}
}