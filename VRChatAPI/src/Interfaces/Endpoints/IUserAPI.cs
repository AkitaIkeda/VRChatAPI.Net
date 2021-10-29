using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IUserAPI : 
		IGet<User, LimitedUser, IUser, UserSearchParams>, 
		IUpdate<IUser, CurrentUser>
	{
		Task<User> Get(string username, CancellationToken ct = default);
		Task<IEnumerable<LimitedUser>> GetFriends(bool? online, int n, int offset, CancellationToken ct = default);
		Task<ResponseMessage> Unfriend(IUser user, CancellationToken ct = default);
		Task<Notification> SendFriendRequest(IUser user, CancellationToken ct = default);
		Task<ResponseMessage> CancelFriendRequest(IUser user, CancellationToken ct = default);
		Task<EFriendStatus> GetFriendStatus(IUser user, CancellationToken ct = default);
	}
}