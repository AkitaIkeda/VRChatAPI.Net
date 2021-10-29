using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class AvatarID : IDAbstract, IAvatar
	{
		public override IEnumerable<string> Prefixes => new List<string> { "avtr" };
		public static AvatarID Parse(string str) => Parse<AvatarID>(str);
	}
}