using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	[JsonConverter(typeof(VRCIDConverter))]
	public abstract class IDAbstract : IVRCObject, IParsableID
	{
		protected Guid guid { get; set; }

		public Guid Guid => guid;

		public abstract IEnumerable<string> Prefixes { get; }

		public string GetIDString(int prefixIndex = 0) => $"{Prefixes.ElementAt(prefixIndex)}_{Guid.ToString("D")}";
		public virtual void ParseFromString(string id)
		{
			var t = id.Split('_');
			if (t.Length < 2) throw new ArgumentException($"Invalid id string: {id}");
			if (!Prefixes.Contains(t[0])) throw new ArgumentException($"Invalid id string: {id}");
			guid = Guid.ParseExact(t[1], "D");
		}

		internal static T Parse<T>(string id) where T : IParsableID, new()
		{
			var t = new T();
			t.ParseFromString(id);
			return t;
		}

		public override string ToString() => GetIDString(0);
	}
}
