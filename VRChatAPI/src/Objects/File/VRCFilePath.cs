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
		public EFileType FileType { get; set; }
		protected override void ParseFromStringL(string[] ls)
		{
			if (ls.Length < 4)
				throw new ArgumentException($"Arg does not have enough information");
			base.ParseFromStringL(ls);
			if (!Enum.TryParse<EFileType>(ls[3], out var r))
				throw new ArgumentException($"FileType is invalid, got {ls[3]}, expected {Enum.GetNames(typeof(EFileType))}");
			else FileType = r;
		}
		public override string GetUrl() =>
			$"{base.GetUrl()}/{FileType.ToString()}";

		public static VRCFilePath Parse(string path){
			var r = new VRCFilePath();
			r.ParseFromString(path);
			return r;
		}
	}
}