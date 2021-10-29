using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class FavoriteID : IDAbstract, IFavorite
	{
		public override IEnumerable<string> Prefixes => new List<string> { "fvrt" };
		public static FavoriteID Parse(string str) => Parse<FavoriteID>(str);
	}
}