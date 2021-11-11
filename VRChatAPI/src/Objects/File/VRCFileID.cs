using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCFileID : IDAbstract, IVRCFile
	{
		public override IEnumerable<string> Prefixes => new List<string> { "file" };
		public static VRCFileID Parse(string str) => Parse<VRCFileID>(str);
	}
}