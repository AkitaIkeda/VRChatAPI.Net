using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Objects;

namespace VRChatAPI.Tests.Helper.Object{
	internal class DummyObjectGenerator
	{
		public DummyObjectGenerator(){
			DefaultObjectGenerator = new Dictionary<Type, Func<Type, object>>{
				{ typeof(IDAbstract), t => {
					var e = Activator.CreateInstance(t);
					t.GetProperty("guid", BindingFlags.NonPublic | BindingFlags.Instance)
						.SetValue(e, Guid.NewGuid());
					return e;
				}},
				{ typeof(string), _ => "test" },
				{ typeof(IEnumerable<object>), t => 
					Activator.CreateInstance(
						typeof(List<>).MakeGenericType(t.GetGenericArguments()), 
						Generate(t.GetGenericArguments()[0], 1))},
				{ typeof(JsonElement?), _ => 
					JsonSerializer.Deserialize<JsonElement>("{}")}
			};
		}
		public DummyObjectGenerator(Dictionary<Type, Func<Type, object>> generators) : this()
		{
			DefaultObjectGenerator = 
				DefaultObjectGenerator.Where(k => !generators.ContainsKey(k.Key))
					.Concat(generators).ToDictionary(v => v.Key, v => v.Value);
		}

		public Dictionary<Type, Func<Type, object>> DefaultObjectGenerator;
		public IEnumerable<object> Generate(Type type, int num){
			IList ret = (IList)Activator.CreateInstance(
				typeof(List<>).MakeGenericType(type));
			for (int i = 0; i < num; i++)
				ret.Add(GetDefaultObject(type));
			return (IEnumerable<object>)ret;
		}

		internal object GetDefaultObject(Type type)
		{
			if(GetGenerator(type) is var g && !(g is null))
				return g(type);
			if(Nullable.GetUnderlyingType(type) is var t && t != null)
				type = t;

			var tmp = Activator.CreateInstance(type);
			if(type.IsValueType || 
				type.Assembly != typeof(VRChatAPI.ProjectDescription).Assembly)
				return tmp;

			var properties = type.GetProperties(
				BindingFlags.Public | BindingFlags.Instance
			).Where(p => p.CanRead && p.CanWrite);
			foreach (var p in properties)
				if(p.GetCustomAttribute(typeof(JsonExtensionDataAttribute)) is null)
					p.SetValue(tmp, GetDefaultObject(p.PropertyType));
			return tmp;
		}

		private Func<Type, object> GetGenerator(Type type) => 
			DefaultObjectGenerator?.FirstOrDefault(v => v.Key.IsAssignableFrom(type)).Value;
	}
}