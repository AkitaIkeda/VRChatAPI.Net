using System;

namespace VRChatAPI.Enums
{
	[Flags]
	public enum EContentTag : uint
	{
		sex = 0b1,
		violence = 0b10,
		gore = 0b100,
		other = 0b1000,
	}
}