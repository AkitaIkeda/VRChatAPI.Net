using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class WorldID : IDAbstract, IWorld
	{
		public override IEnumerable<string> Prefixes => new List<string> { "wrld", "wld" };
		public static WorldID Parse(string id) => Parse<WorldID>(id);
	}
}