using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class ModerationID : IDAbstract, IModeration
	{
		public override IEnumerable<string> Prefixes => new List<string> { "pmod" };
		public static ModerationID Parse(string id) => Parse<ModerationID>(id);
	}
}