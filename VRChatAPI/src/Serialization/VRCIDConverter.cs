using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Serialization
{
	public class VRCIDConverter : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert) =>
			typeToConvert.GetInterface(nameof(IParsableID)) != null 
			&& typeToConvert.GetConstructor(Type.EmptyTypes) != null;

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions _) =>
			(JsonConverter)Activator.CreateInstance(
				typeof(VRCIDConverterInner<>).MakeGenericType(
					new Type[] { typeToConvert }));

		private class VRCIDConverterInner<T> : JsonConverter<T> where T : IParsableID, new()
		{
			public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions _)
			{
				if (reader.TokenType != JsonTokenType.String)
					throw new JsonException("ID must be a string object in order to be read.");
				var r = new T();
				r.ParseFromString(reader.GetString());
				return r;
			}

			public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions _) => 
				writer.WriteStringValue(value.GetIDString());
		}
	}
}