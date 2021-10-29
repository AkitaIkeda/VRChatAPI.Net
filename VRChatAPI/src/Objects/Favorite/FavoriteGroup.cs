using System.Collections.Generic;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class FavoriteGroup : IFavoriteGroup
	{
		public FavoriteGroupID Id { get; set; }
		public UserID OwnerId { get; set; }
		public string OwnerDisplayName { get; set; }
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public EFavoriteType Type { get; set; }
		public EFavoriteGroupVisibility Visibility { get; set; }
		public IEnumerable<string> Tags { get; set; }

		public EFavoriteType FavoriteGroupType => Type;

		public string FavoriteGroupName => Name;

		public UserID UserID => OwnerId;

		public string GetIDString(int prefixIndex = 0) => Id.GetIDString(prefixIndex);
	}
}