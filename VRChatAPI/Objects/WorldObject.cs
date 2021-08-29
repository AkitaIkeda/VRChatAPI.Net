using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using VRChatAPI.Converters;
using VRChatAPI.Abstracts;
using System.Net.Http;
using System.ComponentModel.DataAnnotations;
using System.Text;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	using AssetUrlObject = Newtonsoft.Json.Linq.JToken;
	using PluginUrlObject = Newtonsoft.Json.Linq.JToken;

	#region enums

	[JsonConverter(typeof(StringEnumConverter))]
	public enum UserOptions
	{
		me,
		friends,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum WorldSortOptions
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
		labs,
		active,
		publication,
		recent,
		trending,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum OrderOptions
	{
		ascending,
		descending,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	[DataContract]
	public enum ReleaseStatus
	{
		[EnumMember(Value = "public")]
		_public,
		[EnumMember(Value = "private")]
		_private,
		[EnumMember]
		hidden,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum WorldGroups
	{
		Any,
		Active,
		Recent,
		Favorite,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	[DataContract]
	public enum InstanceType
	{
		[EnumMember(Value = " hidden")]
		Hidden,
		[EnumMember(Value = "friends")]
		Friends,
		[EnumMember(Value = "public")]
		Public,
	}
	#endregion


	[JsonConverter(typeof(AsTConverter<string>))]
	public class WorldId : IdAbstract<WorldId>
	{
		public WorldId() => guid = Guid.NewGuid();
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<WorldId>();

		public static string DefaultPrefix { get; set; } = "wrld";
		public static string[] Prefixes { get; set; } = new string[] { "wrld", "wld", "offline" };

		public override string prefix => DefaultPrefix;
		public WorldId(string id) => this.id = id;

		public override bool CanParse(string s)
		{
			var t = s.Split('_');
			return t.Length >= 2 && Prefixes.Contains(t[0]);
		}

		public static implicit operator string(WorldId worldId) => worldId.ToString();
		public static implicit operator WorldId(string s) => new WorldId(s);

		/// <summary>
		/// Add the world to favorites
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
		/// Get world object from id
		/// </summary>
		/// <returns>World object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<World> Get()
		{
			Logger.LogDebug($"Get world {id}");
			var response = await Global.httpClient.GetAsync($"worlds/{id}");
			return await Utils.UtilFunctions.ParseResponse<World>(response);
		}

		/// <summary>
		/// Delete the world
		/// </summary>
		/// <remarks>
		/// You have to own it to delete the world <br/>
		/// The returned world object does NOT have these 5 keys
		/// <list type="bullet">
		/// 	<item>publicOccupants</item>
		/// 	<item>privateOccupants</item>
		/// 	<item>occupants</item>
		/// 	<item>instances</item>
		/// 	<item>favorites</item>
		/// </list>
		/// </remarks>
		/// <returns>Deleted world</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<World> Delete()
		{
			Logger.LogDebug($"Delete world {id}");
			var response = await Global.httpClient.DeleteAsync($"worlds/{id}");
			return await Utils.UtilFunctions.ParseResponse<World>(response);
		}

		/// <summary>
		/// Update world
		/// </summary>
		/// <param name="to"><see cref=" World"/> object which this object will be updated to. Null fields will be ignored.</param>
		/// <returns>Updated World object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<World> Update(World to)
		{
			var json = JsonConvert.SerializeObject(to, new JsonSerializerSettings(){
				NullValueHandling = NullValueHandling.Ignore,
			});
			Logger.LogDebug("update world {id}: {params}", id, json);
			var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
			var response = await Global.httpClient.PutAsync($"worlds/{id}", content);
			return await Utils.UtilFunctions.ParseResponse<World>(response);
		}

		/// <summary>
		/// Get publish status
		/// </summary>
		/// <returns>Returns a worlds publish status</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<bool> GetPublishStatus()
		{
			Logger.LogDebug("Get publish status {id}", id);
			var response = await Global.httpClient.GetAsync($"worlds/{id}/publish");
			return ((bool)JObject.Parse(await response.Content.ReadAsStringAsync())["canPublish"]);
		}

		/// <summary>
		/// Publish World
		/// </summary>
		/// <remarks>
		/// You can only publish one world per week
		/// </remarks>
		/// <returns>Unknown</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> Publish()
		{
			Logger.LogDebug("Publish world {id}", id);
			var response = await Global.httpClient.PutAsync($"worlds/{id}/publish", null);
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// UnPublish world
		/// </summary>
		/// <exception cref="Exceptions.UnauthorizedRequestException">
		public async Task UnPublish()
		{
			Logger.LogDebug("UnPublishing world {id}", id);
			await Global.httpClient.DeleteAsync($"worlds/{id}/publish");
		}
	}

	public class LimitedWorld
	{
		public WorldId id { get; set; }
		public string name { get; set; }
		public string authorId { get; set; }
		public string authorName { get; set; }
		public int capacity { get; set; }
		public string imageUrl { get; set; }
		public string thumbnailImageUrl { get; set; }
		public ReleaseStatus releaseStatus { get; set; }
		public string organization { get; set; }
		public List<string> tags { get; set; }
		public int favorites { get; set; }
		public DateTime? created_at { get; set; }
		public DateTime? updated_at { get; set; }
		[JsonConverter(typeof(AcceptNoneDatatimeConverter))]
		public DateTime? publicationDate { get; set; }
		[JsonConverter(typeof(AcceptNoneDatatimeConverter))]
		public DateTime? labsPublicationDate { get; set; }
		public List<UnityPackage> unityPackages { get; set; }
		public int popularity { get; set; }
		public int heat { get; set; }
		public int occupants { get; set; }
	}
	public class World : LimitedWorld
	{
		public string assetUrl { get; set; }
		public AssetUrlObject assetUrlObject { get; set; }
		public PluginUrlObject pluginUrlObject { get; set; }
		public string description { get; set; }
		public bool featured { get; set; }
		public int totalLikes { get; set; }
		public int totalVisits { get; set; }
		public UnityPackageUrlObject unityPackageUrlObject { get; set; }
		public string nameSpace { get; set; } // Un
		public int version { get; set; }
		public string previewYoutubeId { get; set; }
		public int visits { get; set; }
		public int publicOccupants { get; set; }
		public int privateOccupants { get; set; }
		[JsonProperty(PropertyName = "instances")]
		internal List<JArray> _instances { get; set; }
		[JsonIgnore]
		public List<WorldInstance> instances => _instances.Select(v => new WorldInstance{ location = ((string)v[0]), occupants = ((int)v[1])}).ToList();
	}
}
