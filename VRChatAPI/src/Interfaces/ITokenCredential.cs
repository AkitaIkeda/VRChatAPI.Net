namespace VRChatAPI.Interfaces
{
	public interface ITokenCredential : ICredential
	{
		string AuthToken { get; }
		string TFAToken { get; }
	}
}