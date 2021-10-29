using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class UserID : IDAbstract, IUser
	{
		public override IEnumerable<string> Prefixes => new string[] { "usr" };
		public static UserID Parse(string id) => Parse<UserID>(id);
	}
}