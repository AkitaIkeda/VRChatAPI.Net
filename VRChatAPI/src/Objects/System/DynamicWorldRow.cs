using System.Collections.Generic;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class DynamicWorldRow : SerializableObjectAbstract
	{
		/// <summary>
		/// Name of row
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Sort Heading
		/// </summary>
		public EWorldSortOption SortHeading { get; set; }
		/// <summary>
		/// Ownership filter
		/// </summary>
		public string SortOwnership { get; set; }
		/// <summary>
		/// Sort Order
		/// </summary>
		public EOrderOption SortOrder { get; set; }
		/// <summary>
		/// Supported Platform
		/// </summary>
		public EWorldRowPlatform Platform { get; set; }
		/// <summary>
		/// Index of row
		/// </summary>
		public int Index { get; set; }
		/// <summary>
		/// Tag filter
		/// </summary>
		public string Tag { get; set; }
	}
}