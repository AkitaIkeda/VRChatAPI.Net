using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;

namespace VRChatAPI.Endpoints
{
	public class SystemAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<SystemAPI>();

		internal SystemAPI(){}

		/// <summary>
		/// Get RemoteConfig and set ApiKey
		/// </summary>
		/// <returns>RemoteConfig object</returns>
		public async Task<ConfigResponse> RemoteConfig()
		{
			Logger.LogDebug("Getting remote config");

			HttpResponseMessage response = await Global.httpClient.GetAsync("config");

			ConfigResponse res = await Utils.UtilFunctions.ParseResponse<ConfigResponse>(response);

			return res;
		}
		/// <summary>
		/// Get current online users count
		/// </summary>
		/// <returns>The number of currently online users</returns>
		public async Task<int> Visits()
		{
			Logger.LogDebug("Getting Current online users count");
			var response = await Global.httpClient.GetAsync("visits");
			return int.Parse(await response.Content.ReadAsStringAsync());
		}
		/// <summary>
		/// Get current online users count
		/// </summary>
		/// <returns>The number of currently online users</returns>
		public async Task<int> GetOnlineUsersCount() => await Visits();
		/// <summary>
		/// Get the current time from VRChat server
		/// </summary>
		/// <returns>DateTime.Now in server</returns>
		public async Task<DateTime> SystemTime()
		{
			Logger.LogDebug("Getting System Time of VRChat server");
			var response = await Global.httpClient.GetAsync("time");
			var t = await response.Content.ReadAsStringAsync();
			return DateTime.ParseExact(t, "yyyy-MM-ddThh:mm:ss+00:00", null);
		}
		/// <summary>
		/// Verify the auth cookie
		/// </summary>
		/// <param name="ok">Is verified</param>
		/// <param name="token">auth token</param>
		/// <returns>Tuple of <paramref name="ok"/> and <paramref name="token"/></returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<(bool ok, string token)> VerifyAuth()
		{
			Logger.LogDebug("Verifying Auth");
			var response = await Global.httpClient.GetAsync("auth");
			return await Utils.UtilFunctions.ParseResponse<(bool ok, string token)>(response);
		}
	}
}
