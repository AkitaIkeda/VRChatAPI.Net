using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Objects;

namespace VRChatAPI.Utils
{
	[Serializable]
	internal class VRCAPIRequestException : HttpRequestException
	{
		public readonly ResponseMessage ErrorMessage;
		public readonly HttpStatusCode StatusCode;

		public VRCAPIRequestException(HttpResponseMessage r) : base(r.Content.ReadAsStringAsync().Result)
		{
			string response = r.Content.ReadAsStringAsync().Result;
			StatusCode = r.StatusCode;
			Data.Add("Request", r.RequestMessage);
			try
			{
				ErrorMessage = JsonSerializer.Deserialize<ResponseMessage>(
				response, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					Converters =
					{
						new JsonStringEnumConverter()
					}
				});
				Data.Add("ErrorMessage", ErrorMessage);
			}
			catch
			{
			}
		}
		//public VRCAPIRequestException() : base()
		//{
		//}
		public VRCAPIRequestException(string message) : base(message)
		{
		}
		//public VRCAPIRequestException(string message, Exception innerException) : base(message, innerException)
		//{
		//}
	}
}