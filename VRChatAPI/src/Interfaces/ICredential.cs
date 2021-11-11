using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface ICredential
	{
		Task<LoginInfo> Login(
			IAPIHttpClient client, 
			JsonSerializerOptions option,
			CancellationToken ct = default);
	}
}