using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Event : SerializableObjectAbstract
	{
		public int DistanceClose { get; set; }
		public int DistanceFactor { get; set; }
		public int DistanceFar { get; set; }
		public int GroupDistance { get; set; }
		public int MaximumBunchSize { get; set; }
		public int NotVisibleFactor { get; set; }
		public int PlayerOrderBucketSize { get; set; }
		public int PlayerOrderFactor { get; set; }
		public int SlowUpdateFactorThreshold { get; set; }
		public int ViewSegmentLength { get; set; }
	}
}