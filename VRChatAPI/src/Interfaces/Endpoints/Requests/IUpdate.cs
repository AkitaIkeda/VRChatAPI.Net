using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Interfaces
{
	public interface IUpdate<TFrom, TTo>
		where TFrom : IVRCObject
		where TTo: TFrom
	{
		Task<TTo> Update(TFrom from, TTo to, CancellationToken ct = default);
	}
}