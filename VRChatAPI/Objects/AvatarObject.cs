using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Linq;
using VRChatAPI.Converters;
using VRChatAPI.Abstracts;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	using AssetUrlObject = Newtonsoft.Json.Linq.JToken;
	using PluginUrlObject = Newtonsoft.Json.Linq.JToken;

	[JsonConverter(typeof(StringEnumConverter))]
	public enum AvatarSortOptions
	{
		popularity,
		heat,
		trust,
		shuffle,
		random,
		favorites,
		reportScore,
		reportCount,
		publicationDate,
		labsPublicationDate,
		created,
		_created_at,
		updated,
		_updated_at,
		order,
		relevance,
		magic,
		name,
	}

	public class UnityPackage
	{
		public string id { get; set; }
		public string assetUrl { get; set; }
		public AssetUrlObject assetUrlObject { get; set; }
		public string pluginUrl { get; set; }
		public PluginUrlObject pluginUrlObject { get; set; }
		public string unityVersion { get; set; }
		public long? unitySortNumber { get; set; }
		public int? assetVersion { get; set; }
		public PlatformEnum? platform { get; set; }
		public DateTime? created_at { get; set; }
	}

	public class UnityPackageUrlObject
	{
		public string unityPackageUrl { get; set; }
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class AvatarId : IdAbstract<AvatarId>
	{
		public AvatarId() => guid = Guid.NewGuid();

		private static ILogger Logger => Global.LoggerFactory.CreateLogger<AvatarId>();

		public override string prefix => "avtr";

		public AvatarId(string id) => this.id = id;

		public static implicit operator AvatarId(string s) => new AvatarId(s);
		public static implicit operator string(AvatarId avatarId) => avatarId.ToString();

		/// <summary>
		/// Get avatar by id
		/// </summary>
		/// <returns>Avatar object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Avatar> Get()
		{
			Logger.LogDebug("Getting avatar details using ID: {id}", id);
			var response = await Global.httpClient.GetAsync($"avatars/{id}");
			return await Utils.UtilFunctions.ParseResponse<Avatar>(response);
		}

		/// <summary>
		/// Update avatar information
		/// </summary>
		/// <param name="to"><see cref="Avatar"> object which this object will be updated to. Null fields will be ignored.</param>
		/// <returns>Updated Avatar object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Avatar> Update(Avatar to)
		{
			var json = JsonConvert.SerializeObject(to, new JsonSerializerSettings(){
				NullValueHandling = NullValueHandling.Ignore,
			});
			Logger.LogDebug("Update avatar {id}: {params}");
			StringContent content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
			var response = await Global.httpClient.PutAsync($"avatars/{id}", content);
			return await Utils.UtilFunctions.ParseResponse<Avatar>(response);
		}

		/// <summary>
		/// Add avatar to favorites
		/// </summary>
		/// <param name="groups">Indexes of favorite groups</param>
		/// <returns>Favorite object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Favorite> AddToFavorite(params FavoriteGroupName[] groups)
		{
			var api = new Endpoints.FavoriteAPI();
			return await api.AddToFavorite(id, groups);
		}

		/// <summary>
		/// Choose avatar to use
		/// </summary>
		/// <returns>Updated Current User object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<CurrentUser> Select()
		{
			Logger.LogDebug("Choose Avatar {id}", id);
			var response = await Global.httpClient.PutAsync($"Avatars/{id}/select", null);
			return await Utils.UtilFunctions.ParseResponse<CurrentUser>(response);
		}

		/// <summary>
		/// Delete avatar
		/// </summary>
		/// <remarks>
		/// You have to own the avatar to delete
		/// </remarks>
		/// <returns>Deleted avatar object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Avatar> Delete()
		{
			Logger.LogDebug("Delete Avatar {id}", id);
			var response = await Global.httpClient.DeleteAsync($"avatars/{id}");
			return await Utils.UtilFunctions.ParseResponse<Avatar>(response);
		}
	}

	public class Avatar
	{
		public string assetUrl { get; set; }
		public AssetUrlObject assetUrlObject { get; set; }
		public UserId authorId { get; set; }
		public string authorName { get; set; }
		public DateTime? created_at { get; set; }
		public string description { get; set; }
		public bool? featured { get; set; }
		public AvatarId id { get; set; }
		public string imageUrl { get; set; }
		public string name { get; set; }
		public ReleaseStatus? releaseStatus { get; set; }
		public List<string> tags { get; set; }
		public string thumbnailImageUrl { get; set; }
		public List<UnityPackage> unityPackages { get; set; }
		public string unityPackageUrl { get; set; }
		public UnityPackageUrlObject unityPackageUrlObject { get; set; }
		public DateTime? updated_at { get; set; }
		public int? version { get; set; }
	}
}
