using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IFavoriteGroup : IVRCObject
	{
		EFavoriteType FavoriteGroupType { get; }
		string FavoriteGroupName { get; }
		UserID UserID { get; }
	}
}