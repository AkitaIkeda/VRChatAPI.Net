using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IPermissionAPI :
		IGet<VRCPermission, IVRCPermission>
	{
		Task<IEnumerable<VRCPermission>> GetPermissions(CancellationToken ct = default);
	}
}