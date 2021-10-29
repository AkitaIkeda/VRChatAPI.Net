using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EReleaseState
	{
		[EnumMember(Value = "public")]
		Public,
		[EnumMember(Value = "private")]
		Private,
		[EnumMember(Value = "hidden")]
		Hidden,
	}
}
