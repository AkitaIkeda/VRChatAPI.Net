using System.Collections.Generic;
using System.Threading.Tasks;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace VRChatAPI.Endpoints
{
	public class ModerationsAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<ModerationsAPI>();

		/// <summary>
		/// Get all player moderations current user have sent 
		/// </summary>
		/// <returns>List of player moderations</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<PlayerModeration>> GetPlayerModerations(ModerationType? type = null, UserId targetUserId = null)
		{
			var p = new Dictionary<string, object>{
				{ "type", type },
				{ "targetUserId", targetUserId },
			};
			Logger.LogDebug("Get list of moderations made by current user: {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var response = await Global.httpClient.GetAsync($"auth/user/playermoderations?{Utils.UtilFunctions.MakeQuery(p)}");
			return await Utils.UtilFunctions.ParseResponse<List<PlayerModeration>>(response);
		}

		public async Task<JObject> ClearAllModerations()
		{
			Logger.LogDebug("Clear all moderations");
			var response = await Global.httpClient.DeleteAsync("auth/user/playermoderations");
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}
	}
}
