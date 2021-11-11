using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class NotificationID : IDAbstract, INotification
	{
		public override IEnumerable<string> Prefixes => new List<string> { "frq", "not" };
		public static NotificationID Parse(string id) => Parse<NotificationID>(id);
	}
}