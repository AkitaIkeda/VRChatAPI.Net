using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCPermission : IVRCPermission
	{
		VRCPermissionID Id { get; set; }
		string name { get; set; }
		UserID OwnerId { get; set; }

		//public PermissionData Data { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			Id.GetIDString(prefixIndex);
	}
}
