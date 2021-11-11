using VRChatAPI.Objects;
using VRChatAPI.APIParams;
using System.Threading.Tasks;
using System.Threading;

namespace VRChatAPI.Interfaces
{
	public interface IAvatarAPI : 
		IGet<Avatar, IAvatar, AvatarSearchParams>,
		ICreate<Avatar, AvatarCreateParams>,
		IUpdate<IAvatar, Avatar>, IDelete<Avatar, IAvatar>
	{
		Task<CurrentUser> Select(IAvatar id, CancellationToken ct = default);
	}
}