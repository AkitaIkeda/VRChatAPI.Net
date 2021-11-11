using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface INotificationAPI :
		IGet<Notification, INotification, NotificationSearchParams>
	{
		Task<Notification> MarkAsRead(INotification obj, CancellationToken ct = default);
		Task<Notification> Hide(INotification obj, CancellationToken ct = default);
		Task<ResponseMessage> ClearNotification(CancellationToken ct = default);
		Task<ResponseMessage> AcceptFriendRequest(INotification obj, CancellationToken ct = default);
	}
}