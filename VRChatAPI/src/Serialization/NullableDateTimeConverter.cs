using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRChatAPI.Serialization{
	public class NullableDateTimeConverter : JsonConverter<DateTime?>
	{
		public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
			reader.TryGetDateTime(out DateTime datetime)
				? datetime
				: (DateTime?)null;

		public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
		{
			if(value is DateTime v)
				writer.WriteStringValue(v);
			else 
				writer.WriteNullValue();
		}
	}
}