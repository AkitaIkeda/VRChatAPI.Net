using System;

namespace VRChatAPI.Objects
{
	public class LoginInfo
	{
		public bool TFARequired => User is null;
		private CurrentUser cu;
		public CurrentUser User { 
			get => cu;
			set{
				cu = value;
				OnLoginStateChangedCallback?.Invoke(this, null);
			}
		}
		public event EventHandler OnLoginStateChangedCallback;
		internal static LoginInfo TFARequiredInfo => new LoginInfo();
		internal LoginInfo CopyTo(LoginInfo info){
			info.User = this.User;
			info.OnLoginStateChangedCallback = this.OnLoginStateChangedCallback;
			return info;
		}
	}
}