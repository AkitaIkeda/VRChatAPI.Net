using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Interfaces
{
	public interface IDelete<TResult, TObject>
	{
		Task<TResult> Delete(TObject obj, CancellationToken ct = default);
	}
}