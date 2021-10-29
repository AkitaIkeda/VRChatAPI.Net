namespace VRChatAPI.Objects
{
	public struct LoginInfo
	{
		public bool TFARequired => User is null;
		public CurrentUser User { get; set; }
		internal static LoginInfo TFARequiredInfo => default;
	}
}