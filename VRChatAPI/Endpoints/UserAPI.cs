using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using VRChatAPI.Utils;

namespace VRChatAPI.Endpoints
{
	/// <summary>
	/// /users/ endpoint
	/// </summary>
	public class UserAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<UserAPI>();

		internal UserAPI(){}

		public async Task<CurrentUser> GetCurrentUser() => await Login();

		public async Task<CurrentUser> Login(){
			Logger.LogDebug("Getting current user details");
			HttpResponseMessage response = await Global.httpClient.GetAsync($"auth/user");
			return await Utils.UtilFunctions.ParseResponse<CurrentUser>(response);
		}
		/// <summary>
		/// Login and Get CurrentUser object 
		/// </summary>
		/// <param name="username">username</param>
		/// <param name="password">password</param>
		/// <returns>CurrentUser object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<CurrentUser> Login(string username, string password)
		{
			Logger.LogDebug("Getting current user details");

			Global.httpClient.DefaultRequestHeaders.Add(
				"Authorization",
				$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"))}"
			);
			var r = await Login();
			Global.httpClient.DefaultRequestHeaders.Remove("Authorization");
			return r;
		}

		/// <summary>
		/// Logout from current user
		/// </summary>
		/// <returns>the response from server</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> Logout()
		{
			Logger.LogDebug("Logging out");
			var response = await Global.httpClient.PutAsync("logout", null);
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Verify Two Factor Authentication if needed
		/// </summary>
		/// <param name="code">2AF code</param>
		/// <returns>Is verified</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<bool> Verify2FactorAuth(string code)
		{
			Logger.LogDebug("Verifying 2FactorAuth");
			var json = new JObject(){
				{ "code", code },
			};
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var response =    await Global.httpClient.PutAsync("auth/twofactorauth/totp/verify", content);
			return ((bool)JObject.Parse(await response.Content.ReadAsStringAsync())["verified"]);
		}
		
		/// <summary>
		/// Verify Two Factor Authentication with recovery code
		/// </summary>
		/// <param name="code">2AF recovery code</param>
		/// <returns>Is verified</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<bool> Verify2FactorAuthWithRecoveryCode(string code)
		{
			Logger.LogDebug("Verifying 2FactorAuth");
			var json = new JObject(){
				{ "code", code },
			};
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			var response =    await Global.httpClient.PutAsync("auth/twofactorauth/otp/verify", content);
			return ((bool)JObject.Parse(await response.Content.ReadAsStringAsync())["verified"]);
		}

		/// <summary>
		/// Get CurrentUser object
		/// </summary>
		/// <returns>CurrentUser object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<CurrentUser> CurrentUserDetails() => await Login();

		/// <summary>
		/// Get friends of current user
		/// </summary>
		/// <param name="n">Num of friends that will be returned</param>
		/// <param name="offset">Offset from 0</param>
		/// <param name="offline">Returns only offline users if true, returns only online and active users if false</param>
		/// <returns>List of friends' LimitedUser object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<LimitedUser>> GetFriends(
			[Range(1, 100)] uint? n = null, 
			uint? offset = null, 
			bool? offline = null)
		{
			Dictionary<string, object> p = new Dictionary<string, object> { { "n", n }, { "offset", offset }, { "offline", offline } };
			Logger.LogDebug("GetFriends with {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			if(n > 100)
				throw new ArgumentException($"{nameof(n)} must be <= 100");
			var response = await Global.httpClient.GetAsync($"auth/user/friends?{Utils.UtilFunctions.MakeQuery(p)}");
			return await Utils.UtilFunctions.ParseResponse<List<LimitedUser>>(response);
		}

		/// <summary>
		/// Get all friends
		/// </summary>
		/// <param name="offline">Returns only offline users if true, returns only online and active users if false</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <returns>All friends</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public AsyncSequentialReader<LimitedUser> GetFriendsSequential(bool? offline = null, [Range(1, 100)] uint bufferSize = 100) =>
			new AsyncSequentialReader<LimitedUser>(
				async (uint offset, uint n) => await GetFriends(n, offset, offline),
				bufferSize
			);

		/// <summary>
		/// Get user info by username
		/// </summary>
		/// <param name="username">username</param>
		/// <returns>User info</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<User> GetByUsername(string username)
		{
			Logger.LogDebug("Get User info by uesrname: {name}", username);
			var response = await Global.httpClient.GetAsync($"user/{username}/name");
			return await Utils.UtilFunctions.ParseResponse<User>(response);
		}

		/// <summary>
		/// Search users
		/// </summary>
		/// <param name="name">display name</param>
		/// <param name="n">num of response</param>
		/// <param name="offset">offest of response</param>
		/// <param name="developerType">for Dev only</param>
		/// <returns>List of LimitedUser[] object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<LimitedUser>> Search(string name, [Range(1, 100) ] uint? n = null, uint? offset = null, DeveloperType? developerType = null)
		{
			Dictionary<string, object> p = new Dictionary<string, object> { { "search", name }, { "developerType", developerType }, { "n", n }, { "offset", offset } };
			Logger.LogDebug("Search users by {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var response = await Global.httpClient.GetAsync($"users?{Utils.UtilFunctions.MakeQuery(p)}");
			return await Utils.UtilFunctions.ParseResponse<List<LimitedUser>>(response);
		}

		/// <summary>
		/// Search users and Get all results
		/// </summary>
		/// <param name="name">username</param>
		/// <param name="developerType">for Dev only</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <returns>All results</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public AsyncSequentialReader<LimitedUser> SearchSequential(string name, DeveloperType? developerType = null, [Range(1, 100)] uint bufferSize = 100) =>
		 new AsyncSequentialReader<LimitedUser>(
			async (uint offset, uint n) => await Search(name, n, offset, developerType),
			bufferSize
		);

		/// <summary>
		/// Get notifications
		/// </summary>
		/// <param name="type">Notification type to get</param>
		/// <param name="sent">Return notifications sent by the user. Must be false or omitted</param>
		/// <param name="hidden">Whether to return hidden or non-hidden notifications</param>
		/// <param name="after">Only return notifications sent after this Date</param>
		/// <param name="n">The number of objects to return</param>
		/// <param name="offset">Offset from 0</param>
		/// <returns></returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<Notification>> GetNotifications(
			NotificationTypes? type = null, 
			bool? sent = null, 
			bool? hidden = null, 
			DateTime? after = null,
			[Range(1, 100)] uint? n = null,
			uint? offset = null)
		{
			Logger.LogDebug("Get all notifications");
			var param = new Dictionary<string, object>{
				{"type", type},
				{"sent", sent},
				{ "hidden", hidden},
				{"after", after},
				{ "n", n },
				{ "offset", offset },
			};
			var response = await Global.httpClient.GetAsync($"auth/user/notifications{Utils.UtilFunctions.MakeQuery(param)}");
			return await Utils.UtilFunctions.ParseResponse<List<Notification>>(response);
		}

		/// <summary>
		/// Get notifications
		/// </summary>
		/// <param name="type">Notification type to get</param>
		/// <param name="sent">Return notifications sent by the user. Must be false or omitted</param>
		/// <param name="hidden">Whether to return hidden or non-hidden notifications</param>
		/// <param name="after">Only return notifications sent after this Date</param>
		/// <param name="bufferSize">Buffer Size</param>
		/// <returns></returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public AsyncSequentialReader<Notification> GetNotificationsSequential(
			NotificationTypes? type = null, 
			bool? sent = null, 
			bool? hidden = null, 
			DateTime? after = null,
			[Range(1, 100)] uint bufferSize = 100
		) => new AsyncSequentialReader<Notification>(
			async (uint offset, uint n) => await GetNotifications(type, sent, hidden, after, n, offset), 
			bufferSize);

		/// <summary>
		/// Clear all notifications
		/// </summary>
		/// <returns>Response message</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> ClearNotification()
		{
			Logger.LogDebug("Clear all notifications");
			var response = await Global.httpClient.PutAsync("auth/user/notifications/clear", null);
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Verify the auth cookie
		/// </summary>
		/// <param name="ok">Is verified</param>
		/// <param name="token">auth token</param>
		/// <returns>Tuple of <paramref name="ok"/> and <paramref name="token"/></returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<(bool ok, string token)> VerifyAuth()
		{
			Logger.LogDebug("Verifying Auth");
			var response = await Global.httpClient.GetAsync("auth");
			return await Utils.UtilFunctions.ParseResponse<(bool ok, string token)>(response);
		}

		/// <summary>
		/// Returns a list of all permissions currently granted by the user
		/// </summary>
		/// <returns>list of permissions currently granted</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<Permission>> GetPermissions()
		{
			Logger.LogDebug("Get assigned permissions");
			var response = await Global.httpClient.GetAsync("auth/permissions");
			return await Utils.UtilFunctions.ParseResponse<List<Permission>>(response);
		}
	}
}
