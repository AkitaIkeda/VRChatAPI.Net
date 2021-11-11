using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Interfaces
{
	public interface ICreate<TResult, TParam>
	{
		Task<TResult> Create(TParam param, CancellationToken ct = default);
	}
}