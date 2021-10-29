using System.Text.Json;

namespace VRChatAPI.Extentions.DependancyInjection
{
	public class VRCAPIOptions
	{
		public string APIEndpointBaseAddress { get; set; }
		public string WSEndpoint { get; set; }
		public int EventHandlerBufferSize { get; set; }
		public JsonSerializerOptions SerializerOption { get; set; }
		public bool StopWSHandlerOnException { get; set; }
	}
}