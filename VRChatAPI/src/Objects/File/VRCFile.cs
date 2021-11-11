using System.Collections.Generic;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class VRCFile : SerializableObjectAbstract, IVRCFile
	{
		public VRCFileID Id { get; set; }
		public string Name { get; set; }
		public UserID OwnerId { get; set; }
		public EMimeType? MimeType { get; set; }
		public string Extension { get; set; }
		public IEnumerable<string> Tags { get; set; }
		public IEnumerable<VRCFileVersion> Versions { get; set; }
		public bool IsImage { get; }

		public string GetIDString(int prefixIndex = 0) =>
			Id.GetIDString(prefixIndex);
	}
}
