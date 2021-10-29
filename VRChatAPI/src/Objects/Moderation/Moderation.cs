using System;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Moderation : IModeration
	{
		public ModerationID Id { get; set; }
		public EModerationType? Type { get; set; }
		public UserID SourceUserId { get; set; }
		public string SourceDisplayName { get; set; }
		public UserID TargetUserId { get; set; }
		public string TargetDisplayName { get; set; }
		public DateTime? Created { get; set; }

		public string GetIDString(int prefixIndex = 0) =>
			Id.GetIDString(prefixIndex);
	}
}
