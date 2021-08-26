using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;
using System.Globalization;
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
		/// <exception cref="UnauthorizedRequestException"/>
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
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task Logout()
		{
			Logger.LogDebug("Logging out");
			await Global.httpClient.PutAsync("logout", null);
		}

		/// <summary>
		/// Verify Two Factor Authentication if needed
		/// </summary>
		/// <param name="code">2AF code</param>
		/// <returns>Is verified</returns>
		/// <exception cref="UnauthorizedRequestException"/>
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
		/// Get CurrentUser object
		/// </summary>
		/// <returns>CurrentUser object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<CurrentUser> CurrentUserDetails() => await Login();

		/// <summary>
		/// Get friends of current user
		/// </summary>
		/// <param name="n">Num of friends that will be returned</param>
		/// <param name="offset">Offset from 0</param>
		/// <param name="offline">Include offline friends</param>
		/// <returns>List of friends' LimitedUser object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<List<LimitedUser>> GetFriends([Range(1, 100)] uint? n = null, uint? offset = null, bool? offline = null)
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
		/// <param name="offline">Include offline friends</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <returns>All friends</returns>
		/// <exception cref="UnauthorizedRequestException"/>
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
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<User> GetByName(string username)
		{
			Logger.LogDebug("Get User info by name");
			var response = await Global.httpClient.GetAsync($"user/{username}/name");
			return await Utils.UtilFunctions.ParseResponse<User>(response);
		}

		/// <summary>
		/// Search users
		/// </summary>
		/// <param name="name">username</param>
		/// <param name="n">num of response</param>
		/// <param name="offset">offest of response</param>
		/// <param name="developerType">for Dev only</param>
		/// <returns>List of LimitedUser[] object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<List<LimitedUser>> Search(string name, [Range(1, 100) ] uint? n = null, uint? offset = null, bool? developerType = null)
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
		/// <exception cref="UnauthorizedRequestException"/>
		public AsyncSequentialReader<LimitedUser> SearchSequential(string name, bool? developerType = null, [Range(1, 100)] uint bufferSize = 100) =>
		 new AsyncSequentialReader<LimitedUser>(
			async (uint offset, uint n) => await Search(name, n, offset, developerType),
			bufferSize
		);

		/// <summary>
		/// Get all notifications
		/// </summary>
		/// <returns>List of Notification object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<List<Notification>> GetAllNotifications(NotificationTypes? type = null, bool? sent = null, DateTime? after = null)
		{
			Logger.LogDebug("Get all notifications");
			var param = new Dictionary<string, object>{
				{"type", type},
				{"sent", sent},
				{"after", after},
			};
			var response = await Global.httpClient.GetAsync($"auth/user/notifications{Utils.UtilFunctions.MakeQuery(param)}");
			return await Utils.UtilFunctions.ParseResponse<List<Notification>>(response);
		}

		// public async Task<CurrentUser> Register(string username, string password, string email, string birthday = null, string acceptedTOSVersion = null)
		// {
		//     Logger.LogDebug(() => $"Registering new user with {nameof(username)} = {username}, {nameof(email)} = {email}, {nameof(birthday)} = {birthday}, {nameof(acceptedTOSVersion)} = {acceptedTOSVersion}");

		//     JObject json = new JObject()
		//     {
		//         { "username", username },
		//         { "password", password }
		//     };

		//     json.AddIfNotNull("email", email);
		//     json.AddIfNotNull("birthday", birthday);
		//     json.AddIfNotNull("acceptedTOSVersion", acceptedTOSVersion);

		//     Logger.LogDebug(() => $"Prepared JSON to post: {json}");

		//     StringContent content = new StringContent(json.ToString(), Encoding.UTF8);

		//     content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

		//     HttpResponseMessage response = await Global.httpClient.PostAsync($"auth/register", content);

		//     return await Utils.UtilFunctions.ParseResponse<CurrentUser>(response);
		// }
	}
}
