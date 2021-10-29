using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Location : IInstance
	{
		public WorldID WorldID { get; }
		public InstanceID InstanceID { get; }

		public string GetIDString(int prefixIndex = 0) =>
			GetLocationString();

		public string GetInstanceIDString() =>
			WorldID.GetIDString();

		public string GetLocationString() =>
			$"{GetWorldIDString()}:{GetInstanceIDString()}";

		public string GetWorldIDString() => 
			InstanceID.GetIDString();
	}
}