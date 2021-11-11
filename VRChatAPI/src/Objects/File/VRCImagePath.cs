using System;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCImagePath : VRCPathBase, IParsable
	{
		public override string Endpoint => "image";
		public int ImageSize { get; set; }
		protected override void ParseFromStringL(string[] ls)
		{
			if (ls.Length < 4)
				throw new ArgumentException($"Arg does not have enough information");
			base.ParseFromStringL(ls);
	 		ImageSize = int.Parse(ls[3]);
		}
		public override string GetUrl() =>
			$"{base.GetUrl()}/{ImageSize}";

		public static VRCImagePath Parse(string path){
			var r = new VRCImagePath();
			r.ParseFromString(path);
			return r;
		}
	}
}