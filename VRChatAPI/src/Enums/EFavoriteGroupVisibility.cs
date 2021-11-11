using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EFavoriteGroupVisibility
	{
		[EnumMember(Value = "private")]
		Private,
		[EnumMember(Value = "public")]
		Public,
		[EnumMember(Value = "friends")]
		Friends,
	}
}
