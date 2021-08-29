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

		public virtual bool CanParse(string s)
		{
			var t = s.Split('_');
			return t.Length >= 2 && t[0] == prefix;
		}

		public virtual void Parse(string s)
		{
			if (!CanParse(s)) throw new ArgumentException($"Invalid id string: {s}");
			var t = s.Split('_');
			guid = Guid.ParseExact(t[1], "D");
		}
		public override string ToString() => id;
	}
}