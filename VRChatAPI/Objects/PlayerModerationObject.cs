using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ModerationType
	{
		mute,
		unmute,
		block,
		unblock,
		hideAvatar,
		showAvatar
	}

	public class PlayerModeration
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<PlayerModeration>();

		public string id { get; set; }
		public ModerationType type { get; set; }
		public string sourceUserId { get; set; }
		public string sourceDisplayName { get; set; }
		public string targetUserId { get; set; }
		public string targetDisplayName { get; set; }
		public DateTime? created { get; set; }

		/// <summary>
		/// Delete moderation
		/// </summary>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task Delete()
		{
			Logger.LogDebug("Delete moderation {id} of {targetUserId}", id, targetUserId);
			await Global.httpClient.DeleteAsync($"user/{targetUserId}/moderations/{id}");
		}
	}
}
