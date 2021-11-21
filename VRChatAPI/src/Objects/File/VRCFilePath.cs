using System;
using System.Collections.Generic;
using System.Linq;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCFilePath : VRCPathBase, IParsable
	{
		public override string Endpoint => "file";
		public EFileType? FileType { get; set; }
		protected override void ParseFromStringL(string[] ls)
		{
			base.ParseFromStringL(ls);
			if (ls.Length >= 4 && Enum.TryParse<EFileType>(ls[3], out var r)) FileType = r;
			else FileType = null;
		}
		public override string GetUrl(){
			var ret = base.GetUrl();
			if(!(FileType is null)) ret += $"/{FileType.ToString()}";
			return ret;
		}

		public static VRCFilePath Parse(string path){
			var r = new VRCFilePath();
			r.ParseFromString(path);
			return r;
		}
	}
}