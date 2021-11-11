using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCPermissionID : IDAbstract, IVRCPermission
	{
		public override IEnumerable<string> Prefixes => new List<string> { "prms" };
		public static VRCPermissionID Parse(string id) => Parse<VRCPermissionID>(id);
	}
}