using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;

namespace VRChatAPI.Endpoints
{
	public class ModerationsAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<ModerationsAPI>();

		/// <summary>
		/// Get all player moderations current user have sent 
		/// </summary>
		/// <returns>List of player moderations</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<List<PlayerModeration>> GetPlayerModerations()
		{
			Logger.LogDebug("Get list of moderations made by current user");
			var response = await Global.httpClient.GetAsync("auth/user/playermoderations");
			return await Utils.UtilFunctions.ParseResponse<List<PlayerModeration>>(response);
		}
	}
}
