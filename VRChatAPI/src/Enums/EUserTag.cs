using System;

namespace VRChatAPI.Enums
{
	[Flags]
	public enum EUserTag
	{
		AvatarAccess = 0b1,
		WorldAccess = 0b10,
		FeedbackAccess = 0b100,
		ProbableTroll = 0b1000,
		Troll = 0b10000,
		Supporter = 0b100000,
		EarlyAdopter = 0b1000000,
		Legend = 0b10000000,
		OfficialThumbnail = 0b100000000,
		LevelLocked = 0b1000000000,
		TagsLocked = 0b10000000000,
		Moderator = 0b100000000000,
		ScriptingAccess = 0b1000000000000,
	}
}
