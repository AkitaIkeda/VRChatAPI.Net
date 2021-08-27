using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VRChatAPI.Utils
{
	static class UtilFunctions
	{
		public static void AddIfNotNull(this JObject jObject, string key, string value)
		{
			if (!string.IsNullOrEmpty(value))
				jObject[key] = value;
		}

		public static void AddIfNotNull(this JObject jObject, string key, JToken value)
		{
			if (value.HasValues)
				jObject[key] = value;
		}

		public static async Task<T> ParseResponse<T>(HttpResponseMessage responseMessage)
		{
			var receivedJson = await responseMessage.Content.ReadAsStringAsync();
			responseMessage.Dispose();
			Global.LoggerFactory.CreateLogger("VRChatAPI.Utils.UtilFunctions.ParseResponse").LogDebug("JSON received: {receivedJson}", receivedJson);
			return  JsonConvert.DeserializeObject<T>(receivedJson);
		}

		public static string MakeQuery(Dictionary<string, object> p, string delimiter = "&", string connection = "=") => string.Join(delimiter, p
			.Where(v => !(v.Value is null))
			.Select(v =>$"{v.Key}{connection}{System.Web.HttpUtility.UrlEncode(JsonConvert.SerializeObject(v.Value))}"));
		}
}
