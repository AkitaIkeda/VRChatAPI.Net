using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VRChatAPI.Serialization{
	public class StringEnumConverter : JsonConverterFactory
	{
		public override bool CanConvert(Type type) => type.IsEnum;

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) => 
			(JsonConverter)Activator.CreateInstance(
				typeof(StringEnumConverterInner<>).MakeGenericType(typeToConvert));

		class StringEnumConverterInner<T> : JsonConverter<T> where T : struct, Enum
		{
			private static IEnumerable<(string Value, string Name)> PropertyNameMap =
				typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Static)
					.Where(v => v.GetCustomAttribute<EnumMemberAttribute>()?.Value != null)
					.Select(v => (v.GetCustomAttribute<EnumMemberAttribute>().Value, v.Name));
			public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
			{
				var enumString = reader.GetString();
				var m = PropertyNameMap.ToDictionary(v => v.Value, v => v.Name);
				if(m.ContainsKey(enumString))
					enumString = m[enumString];

				if (!Enum.TryParse(enumString, out T value)
						&& !Enum.TryParse(enumString, ignoreCase: true, out value))
						throw new JsonException();

				return value;
			}

			public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
			{
				var m = PropertyNameMap.ToDictionary(v => v.Name, v => v.Value);
				var s = value.ToString();
				writer.WriteStringValue(m.ContainsKey(s) ? m[s] : s);
			}
		}
	}
}