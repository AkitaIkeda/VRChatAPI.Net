using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace VRChatAPI.Utils
{
	internal static class QueryConstructor
	{
		public static string MakeQuery(Dictionary<string, object> p, JsonSerializerOptions opt) =>
			string.Join("&", p
				.Where(v => !(v.Value is null))
				.ToDictionary(v => v.Key, v => v.Value)
				.Select(v => $"{v.Key}={HttpUtility.UrlEncode(JsonSerializer.Serialize(v.Value, options: opt))}"));

		public static string MakeQuery<T>(T obj, JsonSerializerOptions opt) =>
			string.Join("&", JsonSerializer.Deserialize<JsonElement>(JsonSerializer.Serialize(obj, opt))
				.EnumerateObject()
				.Where(v => v.Value.ValueKind != JsonValueKind.Null)
				.ToDictionary(v => v.Name, v => v.Value.GetRawText())
				.Select(v => $"{v.Key}={HttpUtility.UrlEncode(JsonSerializer.Serialize(v.Value, options: opt))}"));
	}
}
