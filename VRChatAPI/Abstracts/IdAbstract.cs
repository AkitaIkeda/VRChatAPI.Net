using System;

namespace VRChatAPI.Abstracts
{
	public abstract class IdAbstract<T> where T : IdAbstract<T>, new()
	{
		/// <summary>
		/// prefix without "_"
		/// </summary>
		public abstract string prefix { get; }
		public virtual string id { get => this.GetId(); set => Parse(value); }
		public Guid guid { get; protected set; }

		public virtual string GetId()
		{
			return $"{prefix}_{guid.ToString("D")}";
		}

		public virtual void Parse(string s)
		{
			var t = s.Split("_");
			if (t.Length < 2) throw new ArgumentException($"Invalid id string: {s}");
			if (t[0] != prefix) throw new ArgumentException($"Invalid id string: {s}");
			guid = Guid.ParseExact(t[1], "D");
		}
		public override string ToString() => id;
	}
}