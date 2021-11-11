using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;
using VRChatAPI.Serialization;

namespace VRChatAPI.Objects
{
	public abstract class IDAbstract : IVRCObject, IParsable
	{
		//TODO : better impl
		protected string specialID;
		protected Guid guid { get; set; }

		public Guid Guid => guid;
		public bool IsSpecialID => !(specialID is null);

		public abstract IEnumerable<string> Prefixes { get; }

		public string GetIDString(int prefixIndex = 0) => 
			IsSpecialID 
				? specialID
				: $"{Prefixes.ElementAt(prefixIndex)}_{Guid.ToString("D")}";
				
		public virtual void ParseFromString(string id)
		{
			try{
				var t = id.Split('_');
				if (t.Length < 2) throw new ArgumentException($"Invalid id string: {id}");
				if (!Prefixes.Contains(t[0])) throw new ArgumentException($"Invalid id string: {id}");
				guid = Guid.ParseExact(t[1], "D");
			}
			catch(ArgumentException){
				specialID = id;
			}
		}

		protected static T Parse<T>(string id) where T : IParsable, new()
		{
			var t = new T();
			t.ParseFromString(id);
			return t;
		}

		public override string ToString() => GetIDString(0);
	}
}
