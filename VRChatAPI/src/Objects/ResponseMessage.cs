using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	//TODO: Impl custom converter
	[JsonConverter(typeof(ResponseMessageConverter))]
	public class ResponseMessage
	{
		public EResponseType? MessageType { get; set; }
		public string Message { get; set; }
		public int StatusCode { get; set; }
	}
}