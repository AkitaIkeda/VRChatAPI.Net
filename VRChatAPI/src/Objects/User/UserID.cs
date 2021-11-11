using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	public class UserID : IDAbstract, IUser
	{
		public override IEnumerable<string> Prefixes => new List<string>{ "usr" };
		public static UserID Parse(string id) => Parse<UserID>(id);
	}
}