using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Interfaces
{
	public interface IGet<TGetResult, TObject>
	where TGetResult : IVRCObject
	{
		Task<TGetResult> Get(TObject obj, CancellationToken ct = default);
	}

	public interface IGet<TGetResult, TListResult, TObject, TSearchParam>
		: IGet<TGetResult, TObject>
		where TGetResult : IVRCObject, TListResult
		where TListResult : IVRCObject
		where TObject : IVRCObject
	{
		Task<IEnumerable<TListResult>> Get(TSearchParam option, int n, int offset, CancellationToken ct = default);
	}

	public interface IGet<TGetResult, TObject, TSearchParam>
		: IGet<TGetResult, TGetResult, TObject, TSearchParam>
		where TGetResult : IVRCObject
		where TObject : IVRCObject
	{
	}
}