using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace VRChatAPI.Converters
{
	internal class AsTConverter<T> : JsonConverter
	{
		public override bool CanConvert(Type typeToConvert) => typeof(T).IsCastableTo(typeToConvert);

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var t = serializer.Deserialize<T>(reader);
			if(t == null) return null;
			return typeof(T).GetCastFunction(objectType).Invoke(null, new object[]{t});
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, (T)value);
		}
	}
	static class TypeExtensions {
		public static bool IsCastableTo(this Type from, Type to) {
			return !(from.GetCastFunction(to) is null);
		}
		public static MethodInfo GetCastFunction(this Type from, Type to) {
			return
				from.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.FirstOrDefault(m => m.ReturnType == to && (m.Name == "op_Implicit" || m.Name == "op_Explicit")) ??
				to.GetMethods(BindingFlags.Public | BindingFlags.Static)
					.FirstOrDefault(m => m.GetParameters().Any(p => p.ParameterType == from) && (m.Name == "op_Implicit" || m.Name == "op_Explicit"));
		}
	}
}