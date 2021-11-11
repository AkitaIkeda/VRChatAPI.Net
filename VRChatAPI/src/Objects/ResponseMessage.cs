using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	public class ResponseMessageInstance : SerializableObjectAbstract
	{
		public string Message { get; set; }
		[JsonPropertyName("status_code")]
		public int StatusCode { get; set; }
	}

	[JsonConverter(typeof(ResponseMessageConverter))]
	public class ResponseMessage : ResponseMessageInstance
	{
		public EResponseType? MessageType { get; set; }
	}
}