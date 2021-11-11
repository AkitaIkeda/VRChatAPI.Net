using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EUserState
	{
		[EnumMember]
		offline,
		[EnumMember]
		busy,
		[EnumMember(Value = "ask me")]
		ask_me,
		[EnumMember]
		active,
		[EnumMember(Value = "join me")]
		join_me,
	}
}