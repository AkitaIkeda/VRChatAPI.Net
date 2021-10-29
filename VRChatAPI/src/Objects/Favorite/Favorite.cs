using System.Collections.Generic;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Favorite : IFavorite
	{
		public FavoriteID Id { get; set; }
		public EFavoriteType? Type { get; set; }
		public IVRCObject FavoriteId { get; set; }
		public IEnumerable<string> Tags { get; set; }

		public string GetIDString(int prefixIndex = 0) => Id.GetIDString(prefixIndex);
	}
}