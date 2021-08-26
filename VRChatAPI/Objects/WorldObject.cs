using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	using AssetUrlObject = Newtonsoft.Json.Linq.JToken;
	using PluginUrlObject = Newtonsoft.Json.Linq.JToken;

	#region enums

	[JsonConverter(typeof(StringEnumConverter))]
	public enum UserOptions
	{
		Me,
		Friends,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum SortOptions
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
	public enum ReleaseStatus
	{
		Public,
		Private,
		All,
		Hidden,
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
	public class WorldId
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<WorldId>();
		public string id { get; set; }

		public WorldId(string id) => this.id = id;

		public static implicit operator string(WorldId worldId) => worldId.ToString();
		public static implicit operator WorldId(string s) => new WorldId(s);
		public override string ToString() => id;

		/// <summary>
		/// Add the world to favorites
		/// </summary>
		/// <param name="groups">Indexes of favorite groups</param>
		/// <returns>Favorite object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Favorite> AddToFavorite(params FavoriteGroupId[] groups)
		{
			var api = new Endpoints.FavoriteAPI();
			return await api.AddToFavorite(FavoriteType.world, id, groups);
		}

		/// <summary>
		/// Get world object from id
		/// </summary>
		/// <returns>World object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<World> GetWorld()
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
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<World> DeleteWorld()
		{
			Logger.LogDebug($"Delete world {id}");
			var response = await Global.httpClient.DeleteAsync($"worlds/{id}");
			return await Utils.UtilFunctions.ParseResponse<World>(response);
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
		public string description { get; set; }
		public bool featured { get; set; }
		public int totalLikes { get; set; }
		public int totalVisits { get; set; }
		public string assetUrl { get; set; }
		public AssetUrlObject assetUrlObject { get; set; }
		public PluginUrlObject pluginUrlObject { get; set; }
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
