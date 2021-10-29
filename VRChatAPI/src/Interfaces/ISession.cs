using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface ISession :
		IAuthAPI, ISystemAPI, IUserAPI, IInviteAPI,
		IAvatarAPI, IWorldAPI, IFavoriteAPI, INotificationAPI,
		IFileAPI, IModerationAPI, IPermissionAPI
	{
		bool HandlingWSEvents { get; }
		bool TFARequired { get; }
		IWSEventHandler EventHandler { get; }
		ITokenCredential Credential { get; }
		IAPIHttpClient APIHttpClient { get; }
		CurrentUser User { get; }
		APIConfig RemoteConfig { get; }
	}
}