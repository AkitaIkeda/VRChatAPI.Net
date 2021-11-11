using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class World : LimitedWorld
	{
		public string AssetUrl { get; set; }
		//public IAssetUrlObject assetUrlObject { get; set; }
		//public IPluginUrlObject pluginUrlObject { get; set; }
		public string Description { get; set; }
		public bool Featured { get; set; }
		public int TotalLikes { get; set; }
		public int TotalVisits { get; set; }
		public UnityPackageUrlObject UnityPackageUrlObject { get; set; }
		public string NameSpace { get; set; } // Unused
		public int Version { get; set; }
		public string PreviewYoutubeId { get; set; }
		public int Visits { get; set; }
		public int PublicOccupants { get; set; }
		public int PrivateOccupants { get; set; }
		public IEnumerable<InstanceInfo> Instances { get; set; }
	}
}