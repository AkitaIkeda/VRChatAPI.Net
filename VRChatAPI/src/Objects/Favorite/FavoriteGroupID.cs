using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class FavoriteGroupID : IDAbstract, IVRCObject
	{
		public override IEnumerable<string> Prefixes => new List<string> { "fvgrp" };
		public static FavoriteGroupID Parse(string str) => Parse<FavoriteGroupID>(str);
	}
}