using System.Collections.Generic;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class MessageID : IDAbstract, IMessage
	{
		public override IEnumerable<string> Prefixes => new List<string> { "invm" };
		public static MessageID Parse(string id) => Parse<MessageID>(id);
	}
}