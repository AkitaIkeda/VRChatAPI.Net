namespace VRChatAPI.Interfaces
{
	public interface IParsableID : IVRCObject
	{
		void ParseFromString(string id);
	}
}