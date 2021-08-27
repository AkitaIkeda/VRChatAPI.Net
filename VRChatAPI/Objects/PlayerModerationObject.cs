using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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
		showAvatar,
	}

	public class PlayerModeration
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<PlayerModeration>();

		// TODO: Implement PlayerModerationId
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
		/// <returns>Response message</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> Delete()
		{
			Logger.LogDebug("Delete moderation {id} of {targetUserId}", id, targetUserId);
			var response = await Global.httpClient.DeleteAsync($"/auth/user/playermoderations/{id}");
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Returns a single Player Moderation
		/// </summary>
		/// <returns>Player moderation</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<PlayerModeration> Get()
		{
			Logger.LogDebug("Get moderation");
			var request = await Global.httpClient.GetAsync($"auth/user/playermoderations/{id}");
			return await Utils.UtilFunctions.ParseResponse<PlayerModeration>(request);
		}
	}
}
