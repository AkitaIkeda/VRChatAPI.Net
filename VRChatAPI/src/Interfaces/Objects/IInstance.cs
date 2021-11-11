namespace VRChatAPI.Interfaces
{
	public interface IInstance : IVRCObject
	{
		string GetWorldIDString();
		string GetInstanceIDString();
		string GetLocationString();
	}
}