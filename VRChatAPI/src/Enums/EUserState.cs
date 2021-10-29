using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EUserState
	{
		[EnumMember(Value = "join me")]
		join_me,
		[EnumMember]
		active,
		[EnumMember(Value = "ask me")]
		ask_me,
		[EnumMember]
		busy,
		[EnumMember]
		offline,
	}
}