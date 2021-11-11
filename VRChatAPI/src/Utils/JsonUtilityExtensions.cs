using Microsoft.Extensions.Logging;
using System.Text.Json;
using VRChatAPI.Logging;

namespace VRChatAPI.Utils
{
	internal static class JsonUtilityExtensions
	{
		public static T Deserialize<T>(this JsonElement element, JsonSerializerOptions option)
		{
			var t = element.GetRawText();
			return JsonSerializer.Deserialize<T>(t, option);
		}
		public static T Deserialize<T>(this JsonElement element, JsonSerializerOptions options, ILogger logger)
		{
			logger.LogDebug(LogEventID.Deserialize, "Deserialize from JsonElement: {0}", element);
			return element.Deserialize<T>(options);
		}
	}
}
