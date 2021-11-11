using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Objects;

namespace VRChatAPI.Serialization
{
	internal class InstanceInfoConverter : JsonConverter<InstanceInfo>
	{
		public override InstanceInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var v = (options.GetConverter(typeof(JsonElement)) as JsonConverter<JsonElement>)
				.Read(ref reader, typeToConvert, options);
			if(v.ValueKind != JsonValueKind.Array)
				throw new JsonException();
			return new InstanceInfo{
				InstanceID = InstanceID.Parse(v[0].GetString()),
				PlayersNum = v[1].GetInt32(),
			};
		}

		public override void Write(Utf8JsonWriter writer, InstanceInfo value, JsonSerializerOptions options)
		{
			writer.WriteStartArray();
				writer.WriteStringValue(value.InstanceID.GetInstanceIDString());
				writer.WriteNumberValue(value.PlayersNum);
			writer.WriteEndArray();
		}
	}
}