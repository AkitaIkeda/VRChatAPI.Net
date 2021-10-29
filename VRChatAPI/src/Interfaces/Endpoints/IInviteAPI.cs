using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IInviteAPI :
		IGet<Message, Message>,　IDelete<IEnumerable<Message>, Message>
	{
		Task<Message> Get(IUser user, Message obj, CancellationToken ct = default);
		Task<IEnumerable<Message >> Get(EMessageType type, CancellationToken ct = default);
		Task<IEnumerable<Message>> Get(IUser user, EMessageType type, CancellationToken ct = default);
		Task<Notification> Invite(
			IUser user,
			IInstance location,
			int? messageSlot = null,
			CancellationToken ct = default);
		Task<Notification> RequestInvite(
			IUser user,
			int? messageSlot = null,
			CancellationToken ct = default);
		Task<Notification> RespondInvite(
			INotification invite,
			int? messageSlot = null,
			CancellationToken ct = default);
		Task<Notification> RespondRequestInvite(
			INotification requestInvite,
			int? messageSlot = null,
			CancellationToken ct = default);
		Task<IEnumerable<Message>> Update(Message from, Message to, CancellationToken ct = default);
		Task<IEnumerable<Message>> Update(IUser user, Message from, Message to, CancellationToken ct = default);
	}
}