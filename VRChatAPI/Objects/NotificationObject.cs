using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	public class Details
	{
		public Location worldId { get; set; }
		public PlatformEnum? platform { get; set; }
		public UserId inResponseToUser { get; set; }
		public string responseMessage { get; set; }
		public UserId userToKickId { get; set; }
		public UserId initiatorUserId { get; set; }
		public WorldId halpId { get; set; } // Unknown
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum NotificationTypes
	{
		all,
		broadcast,
		message,
		friendRequest,
		invite,
		requestInvite,
		votetokick,
		halp, // Unknown
		hidden, // Unknown
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class NotificationId
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<NotificationId>();

		public string id { get; set; }

		public NotificationId(string id) => this.id = id;

		public static implicit operator string(NotificationId notificationId) => notificationId.ToString();
		public static implicit operator NotificationId(string s) => new NotificationId(s);
		public override string ToString() => id;

		/// <summary>
		/// Hide notification
		/// </summary>
		/// <remarks>
		/// The endpoint can suggest the notification still exists, but it can't be gotten by get all method
		/// </remarks>
		/// <returns>Updated Notification object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Notification> Hide()
		{
			Logger.LogDebug("Hide notification {id}", id);
			var response = await Global.httpClient.PutAsync($"auth/user/notifications/{id}/hide", null);
			return await Utils.UtilFunctions.ParseResponse<Notification>(response);
		}

		/// <summary>
		/// Mark the notification as read
		/// </summary>
		/// <returns>Updated Notification object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Notification> MarkAsRead()
		{
			Logger.LogDebug("Mark {id} as read", id);
			var response = await Global.httpClient.PutAsync($"auth/user/notifications/{id}/see", null);
			return await Utils.UtilFunctions.ParseResponse<Notification>(response);
		}
	}

	public class Notification
	{
		private ILogger Logger => Global.LoggerFactory.CreateLogger<Notification>();

		public NotificationId id { get; set; }
		public string senderUserId { get; set; }
		public string senderUsername { get; set; }
		public NotificationTypes type { get; set; }
		public string message { get; set; }
		[JsonProperty(PropertyName = "details")]
		private string _details { get; set; }
		[JsonIgnore]
		public Details details => JsonConvert.DeserializeObject<Details>(_details);
		public bool seen { get; set; }
		public DateTime? created_at { get; set; }
		public string receiverUserId { get; set; }
		public string jobName { get; set; }
		public string jobColor { get; set; }

		/// <summary>
		/// Accept friend request
		/// To ignore, use <see cref="NotificationId.DeleteNotification">DeleteNotification</see>
		/// </summary>
		/// <returns>Response from server</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		/// <exception cref="InvalidOperationException"/>
		public async Task<JObject> AcceptFriendRequest()
		{
			Logger.LogDebug("Accept Friend Request {id}", id);
			if(type != NotificationTypes.friendRequest)
				throw new InvalidOperationException($"{nameof(type)} must be {NotificationTypes.friendRequest} but is {type}");
			var response = await Global.httpClient.PutAsync($"auth/user/notifications/{id}/accept", null);
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}
	}
}
