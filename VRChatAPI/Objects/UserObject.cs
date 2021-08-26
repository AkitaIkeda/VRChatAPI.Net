using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using VRChatAPI.Utils;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	using steamDetails = JToken;

	public class PastDisplayName
	{
		public string displayName { get; set; }
		public DateTime? updated_at { get; set; }
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum PlatformEnum
	{
		standalonewindows,
		android,
	}

	/// <summary>
	/// Tags that User can have
	/// Tags that starts with admin are granted manually by admin
	/// Tags that starts with system are granted automatically by the system
	/// </summary>
	[Flags]
	public enum UserTags : uint
	{
		admin_scripting_access      = 0b0000000000000001,
		system_scripting_access     = 0b0000000000000010,
		admin_avatar_access         = 0b0000000000000100,
		system_avatar_access        = 0b0000000000001000,
		admin_world_access          = 0b0000000000010000,
		system_world_access         = 0b0000000000100000,
		admin_moderator             = 0b0000000001000000,
		system_feedback_access      = 0b0000000010000000,
		system_probable_troll       = 0b0000000100000000,
		system_troll                = 0b0000001000000000,
		system_supporter            = 0b0000010000000000,
		system_early_adopter        = 0b0000100000000000,
		admin_official_thumbnail    = 0b0001000000000000,
		show_social_rank            = 0b0010000000000000,
		// system_UE4_dev_access
		// system_neuralink_beta
		// system_extremely_cool_guy
		// system_stop_being_nosy
		// system_notamod
		// system_no_seriously_im_not_a_mod_how_many_times_do_i_have_to_tell_people
		// system_the_tag_is_just_named_that
		// system_haha_you_have_to_document_this_one_too
	}

	/// <summary>
	/// Trust Level
	/// Users have every lower level tags
	/// For example, known user's will be 0b0111
	/// </summary>
	[Flags]
	public enum TrustLevel : uint
	{
		visitor                 = 0,
		system_trust_basic      = 0b00000001,
		system_trust_known      = 0b00000010,
		system_trust_trusted    = 0b00000100,
		system_trust_veteran    = 0b00001000,
		system_trust_legend     = 0b00010000, // Unused
		system_legend           = 0b00100000, // Unused
	}

	/// <summary>
	/// User Status
	/// </summary>
	[JsonConverter(typeof(StringEnumConverter))]
	[DataContract]
	public enum UserStatus
	{
		[EnumMember(Value = "join me")]
		join_me, 
		[EnumMember]
		active,
		[EnumMember(Value = "ask me")]
		ask_me,
		[EnumMember]
		busy,
		[EnumMember]
		offline,
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class UserId
	{
		ILogger Logger => Global.LoggerFactory.CreateLogger<UserId>();

		public UserId(string id) => this.id = id;

		public string id { get; set; }

		public static implicit operator UserId(string id) => new UserId(id);
		public static implicit operator string(UserId userId) => userId.ToString(); 

		public override string ToString() => id;

		/// <summary>
		/// Get the friend status of the user
		/// </summary>
		/// <param name="isFriend">Is the user a friend</param>
		/// <param name="outgoingRequest">There is an pending friend request you have sent</param>
		/// <param name="incomingRequest">You have an incoming friend request</param>
		/// <returns>Tuple of <paramref name="isFriend"/>, <paramref name="outgoingRequest"/>, and <paramref name="incomingRequest"/></returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<(bool isFriend, bool outgoingRequest, bool incomingRequest)> GetFriendStatus()
		{
			Logger.LogDebug("Get FriendStatus of {id}", id);
			var response = await Global.httpClient.GetAsync($"user/{id}/friendStatus");
			return await  Utils.UtilFunctions.ParseResponse<(bool isFriend, bool outgoingRequest, bool incomingRequest)>(response);
		}

		/// <summary>
		/// Send a friend request to the user
		/// </summary>
		/// <returns>Notification object with type of friendRequest</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Notification> SendFriendRequest()
		{
			Logger.LogDebug("Send a friend request to {id}", id);
			var response = await Global.httpClient.PostAsync($"user/{id}/friendRequest", null);
			return await Utils.UtilFunctions.ParseResponse<Notification>(response);
		}

		/// <summary>
		/// Destroy friendship with the user
		/// </summary>
		/// <returns>json response</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<JObject> Unfriend()
		{
			Logger.LogDebug("Destroying friendship with {id}", id);
			var response = await Global.httpClient.DeleteAsync($"auth/user/friends/{id}");
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Get User by User ID
		/// </summary>
		/// <returns>User object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<User> GetUser()
		{
			Logger.LogDebug("Getting user info with ID: {id}", id);
			HttpResponseMessage response = await Global.httpClient.GetAsync($"users/{id}");
			return await Utils.UtilFunctions.ParseResponse<User>(response);
		}

		/// <summary>
		/// Add user to favorites
		/// </summary>
		/// <param name="groups">Indexes of favorite groups</param>
		/// <returns>Favorite object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Favorite> AddToFavorites(params FavoriteGroupId[] groups)
		{
			var api = new Endpoints.FavoriteAPI();
			return await api.AddToFavorite(FavoriteType.friend, id, groups);
		}

		/// <summary>
		/// Block Player
		/// </summary>
		/// <returns>Player Moderation object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<PlayerModeration> Block()
		{
			Logger.LogDebug("Block {id}", id);
			var json = new JObject();
			json.Add("blocked", id);
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PostAsync("auth/user/blocks", content);
			return await Utils.UtilFunctions.ParseResponse<PlayerModeration>(response);
		}

		/// <summary>
		/// UnBlock Player
		/// </summary>
		/// <returns>Player Moderation object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<PlayerModeration> UnBlock()
		{
			Logger.LogDebug("UnBlock {id}", id);
			var json = new JObject();
			json.Add("blocked", id);
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PostAsync("auth/user/unblocks", content);
			return await Utils.UtilFunctions.ParseResponse<PlayerModeration>(response);
		}

		/// <summary>
		/// Mute Player
		/// </summary>
		/// <returns>Player Moderation object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<PlayerModeration> Mute()
		{
			Logger.LogDebug("Mute {id}", id);
			var json = new JObject();
			json.Add("type", "mute");
			json.Add("moderated", id);
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PostAsync("auth/user/playermoderations", content);
			return await Utils.UtilFunctions.ParseResponse<PlayerModeration>(response);
		}
		
		/// <summary>
		/// UnMute Player
		/// </summary>
		/// <returns>Player Moderation object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<PlayerModeration> UnMute()
		{
			Logger.LogDebug("UnMute {id}", id);
			var json = new JObject();
			json.Add("type", "unmute");
			json.Add("moderated", id);
			var content = new StringContent(json.ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PostAsync("auth/user/playermoderations", content);
			return await Utils.UtilFunctions.ParseResponse<PlayerModeration>(response);
		}

		/// <summary>
		/// Clear player's moderations
		/// </summary>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task ClearModeration()
		{
			Logger.LogDebug("Clear modification of {id}", id);
			await Global.httpClient.DeleteAsync($"auth/user/{id}/moderations");
		}
	}
	public class LimitedUser
	{
		public UserId id { get; set; }
		public string username { get; set; }
		public string displayName { get; set; }
		public string bio { get; set; }
		public string currentAvatarImageUrl { get; set; }
		public string currentAvatarThumbnailImageUrl { get; set; }
		public AvatarId fallbackAvatar { get; set; }
		public string userIcon { get; set; }
		public string profilePicOverride { get; set; }
		public PlatformEnum last_platform { get; set; }
		public List<string> tags { get; set; }
		public string developerType { get; set; }
		public UserStatus status { get; set; }
		public string statusDescription { get; set; }
		public string friendKey { get; set; }
		public bool isFriend { get; set; }
		public Location location { get; set; } // If empty, User is not in the game.

		public TrustLevel GetTrustLevel(){
			TrustLevel ret = 0;
			foreach (var t in tags)
				if(Enum.TryParse<TrustLevel>(t, out var o))
					ret |= o;
			return ret;
		}

		public UserTags GetUserTags(){
			UserTags ret = 0;
			foreach (var t in tags)
				if(Enum.TryParse<UserTags>(t, out var o))
					ret |= o;
			return ret;
		}

		public List<string> GetLanguages() => tags
			.Where(t => t.IndexOf("langage_") == 0)
			.Select(v => v[("langage_".Length)..])
			.ToList();
	}

	public class User : LimitedUser
	{
		public List<string> bioLinks { get; set; }
		public string state { get; set; }
		public DateTime? last_login { get; set; }
		public bool allowAvatarCopying { get; set; }
		public DateTime? date_joined { get; set; }
		public string worldId { get; set; }
		public string instanceID { get; set; }
	}

	public class CurrentUser : User
	{
		ILogger Logger => Global.LoggerFactory.CreateLogger<CurrentUser>();

		public List<PastDisplayName> pastDisplayNames { get; set; }
		public bool hasEmail { get; set; }
		public bool hasPendingEmail { get; set; }
		public string obfuscatedEmail { get; set; }
		public string obfuscatedPendingEmail { get; set; }
		public bool emailVerified { get; set; }
		public bool hasBirthday { get; set; }
		public bool unsubscribe { get; set; }
		public List<string> statusHistry { get; set; }
		public bool statusFirstTime { get; set; }
		public List<string> friends { get; set; }
		public List<string> friendGroupNames { get; set; }
		public AvatarId currentAvatar { get; set; }
		public string currentAvatarAssetUrl { get; set; }
		public DateTime? accountDeletionDate { get; set; }
		public int acceptedTOSVersion { get; set; }
		public string steamId { get; set; }
		public steamDetails steamDetails { get; set; }
		public string oculusId { get; set; }
		public bool hasLoggedInFromClient { get; set; }
		public string homeLocation { get; set; }
		public bool twoFactorAuthEnabled { get; set; }
		public List<string> onlineFriends { get; set; }
		public List<string> activeFriends { get; set; }
		public List<string> offlineFriends { get; set; }

		/// <summary>
		/// Update Current User Information
		/// </summary>
		/// <param name="email">Email Address</param>
		/// <param name="birthday">Birthday</param>
		/// <param name="acceptedTOSVersion">Last accepted Term of Service version</param>
		/// <param name="tags">User Tags</param>
		/// <param name="networkSessionId">Network session id, from Photon</param>
		/// <param name="status">Current status</param>
		/// <param name="statusDescription">The message seen in game with <paramref name="status"/></param>
		/// <param name="bio">User Bio</param>
		/// <param name="bioLinks">Bio Links</param>
		/// <returns>Updated Current User Info</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<CurrentUser> UpdateUserInfo(
			string email = null, 
			string birthday = null, 
			string acceptedTOSVersion = null, 
			List<string> tags = null,
			string networkSessionId = null,
			UserStatus? status = null,
			string statusDescription = null,
			string bio = null,
			List<string> bioLinks = null)
		{
			Logger.LogDebug("Updating user info for {id}", id);

			JObject json = new JObject();
			json.AddIfNotNull("email", email);
			json.AddIfNotNull("birthday", birthday);
			json.AddIfNotNull("acceptedTOSVersion", acceptedTOSVersion);
			json.AddIfNotNull("tags", JToken.FromObject(tags));
			json.AddIfNotNull("networkSessionId", networkSessionId);
			json.AddIfNotNull("status", status?.ToString());
			json.AddIfNotNull("statusDescription", statusDescription);
			json.AddIfNotNull("bio", bio);
			json.AddIfNotNull("bioLinks", JToken.FromObject(bioLinks));

			Logger.LogDebug("Prepared JSON to put: {json}", json);

			StringContent content = new StringContent(json.ToString(), Encoding.UTF8);

			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			HttpResponseMessage response = await Global.httpClient.PutAsync($"users/{id}", content);
			return await Utils.UtilFunctions.ParseResponse<CurrentUser>(response);
		}
	}
}
