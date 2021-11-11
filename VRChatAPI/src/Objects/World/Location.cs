using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Location : IInstance , IParsable
	{
		public WorldID WorldID { get; set; }
		public InstanceID InstanceID { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			GetLocationString();

		public string GetInstanceIDString() =>
			InstanceID.GetIDString();

		public string GetLocationString() =>
			$"{GetWorldIDString()}:{GetInstanceIDString()}";

		public string GetWorldIDString() => 
			WorldID.GetIDString();

		public void ParseFromString(string id)
		{
			var t = id.Split(':');
			WorldID = WorldID.Parse(t[0]);
			InstanceID = InstanceID.Parse(t[1]);
		}
		public override string ToString() => GetLocationString();
		public static Location Parse(string id){
			var r = new Location();
			r.ParseFromString(id);
			return r; 
		}
	}
}