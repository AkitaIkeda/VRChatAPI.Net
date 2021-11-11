using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class Announcement : SerializableObjectAbstract
	{
		/// <summary>
		/// Announcement Name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Announcement Body
		/// </summary>
		public string Text { get; set; }
	}
}