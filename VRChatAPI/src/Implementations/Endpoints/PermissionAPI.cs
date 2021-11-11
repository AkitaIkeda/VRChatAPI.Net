using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;

namespace VRChatAPI.Implementations
{
	partial class Session : IPermissionAPI
	{
		private const string permissionEndpoint = "permissions";
		public Task<VRCPermission> Get(IVRCPermission obj, CancellationToken ct = default) =>
			client.Get<VRCPermission>($"{permissionEndpoint}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<VRCPermission>> GetPermissions(CancellationToken ct = default) =>
			client.Get<IEnumerable<VRCPermission>>($"{authEndpoint}/{permissionEndpoint}", ct);
	}
}
