using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IModerationAPI :
		IGet<Moderation, IModeration>,
		ICreate<Moderation, ModerationCreateParams>,
		IDelete<ResponseMessage, IModeration>
	{
		Task<IEnumerable<Moderation>> Get(ModerationSearchParams option, CancellationToken ct = default);
		Task<ResponseMessage> ClearAllModerations(CancellationToken ct = default);
		Task<ResponseMessage> Unmoderate(
			IUser user, EModerationType type,
			CancellationToken ct = default);
	}
}