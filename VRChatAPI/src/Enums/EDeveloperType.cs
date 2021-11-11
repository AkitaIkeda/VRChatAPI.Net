using System.Runtime.Serialization;

namespace VRChatAPI.Enums
{
	[DataContract]
	public enum EDeveloperType
	{
		[EnumMember]
		none,
		[EnumMember]
		trusted,
		[EnumMember(Value = "internal")]
		_internal,
		[EnumMember]
		moderator
	}
}