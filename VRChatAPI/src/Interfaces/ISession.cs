using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface ISession :
		IAuthAPI, ISystemAPI, IUserAPI, IInviteAPI,
		IAvatarAPI, IWorldAPI, IFavoriteAPI, INotificationAPI,
		IFileAPI, IModerationAPI, IPermissionAPI
	{
		bool IsLoggedIn { get; }
		IWSEventHandler EventHandler { get; }
		ITokenCredential Credential { get; }
		IAPIHttpClient APIHttpClient { get; }
		CurrentUser User { get; }
		APIConfig RemoteConfig { get; }
		LoginInfo LoginInfo { get; }
		void StartWSEventHandling();
		void StopWSEventHandling();
		Task<LoginInfo> Login(ICredential credential, CancellationToken ct = default);
	}
}