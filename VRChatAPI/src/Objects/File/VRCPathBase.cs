using System;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public abstract class VRCPathBase : IParsable
	{
		public VRCFileID FileID { get; set; }
		public int Version { get; set; }
		public virtual string API_URL => "https://api.vrchat.cloud/api/1/";
		public abstract string Endpoint { get; }

		public virtual string GetUrl() =>
			$"{API_URL}{Endpoint}/{FileID.GetIDString()}/{Version}";
		public void ParseFromString(string id)
		{
			if (!id.StartsWith(API_URL))
				throw new ArgumentException($"Arg must start with {API_URL}");
			var t = id.Substring(API_URL.Length).Split('/');
			ParseFromStringL(t);
		}

		protected virtual void ParseFromStringL(string[] ls){
			if (ls.Length < 3)
				throw new ArgumentException($"Arg does not have enough information");
			if (ls[0] != Endpoint)
				throw new ArgumentException($"Endpoint must be {Endpoint}, got {ls[0]}");
			FileID = VRCFileID.Parse(ls[1]);
			Version = int.Parse(ls[2]);
		}

		public override string ToString() => GetUrl();
	}
}