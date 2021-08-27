using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using VRChatAPI.Converters;
using VRChatAPI.Abstracts;
using System;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FavoriteType
	{
		world,
		friend,
		avatar,
	}

	[JsonConverter(typeof(StringEnumConverter))]
	[DataContract]
	public enum FavoriteGroupVisibility
	{
		[EnumMember(Value = "private")]
		_private,
		[EnumMember(Value = "public")]
		_public,
		[EnumMember]
		friends,
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class FavoriteGroupName
	{
		public string name { get; set; }

		public FavoriteGroupName(string name) => this.name = name;
		public static implicit operator string(FavoriteGroupName favorite) => favorite.ToString();
		public static implicit operator FavoriteGroupName(string s) => new FavoriteGroupName(s);
		public override string ToString() => name;
	}

	public class FavoriteGroup
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<FavoriteGroup>();
		public FavoriteId id { get; set; }
		public UserId ownerId { get; set; }
		public string ownerDisplayName { get; set; }
		public FavoriteGroupName name { get; set; }
		public string displayName { get; set; }
		public FavoriteType type { get; set; }
		public FavoriteGroupVisibility visibility { get; set; }
		public List<string> tags { get; set; }

		/// <summary>
		/// Get Favorite Group
		/// </summary>
		/// <returns>Favorite group</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<FavoriteGroup> Get()
		{
			Logger.LogDebug("Getting {owner} Favorite Group of {type} named {name}", ownerId, type, name);
			var response = await Global.httpClient.GetAsync($"favorite/group/{type}/{name}/{ownerId}");
			return await Utils.UtilFunctions.ParseResponse<FavoriteGroup>(response);
		}

		/// <summary>
		/// Update Favorite Group
		/// </summary>
		/// <param name="displayName">Display name</param>
		/// <param name="visibility">visibility of the favorite group</param>
		/// <param name="tags">tags to apply</param>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task Update(
			string displayName = null,
			FavoriteGroupVisibility? visibility = null,
			List<string> tags = null
		)
		{
			var p = new Dictionary<string, object>{
				{ "displayName", displayName },
				{ "visibility", visibility },
				{ "tags", tags },
			};
			Logger.LogDebug("Update {owner} Favorite Group of {type} named {name}: {param}", ownerId, type, name, Utils.UtilFunctions.MakeQuery(p, ", "));
			StringContent content = new StringContent(JObject.FromObject(p.Where(v => !(v.Value is null))).ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PutAsync($"favorite/group/{type}/{name}/{ownerId}", content);
		}

		/// <summary>
		/// Clear Favorite Group
		/// </summary>
		/// <returns>Response message</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> Clear()
		{
			Logger.LogDebug("Clear {owner} Favorite Group of {type} named {name}", ownerId, type, name);
			var response = await Global.httpClient.DeleteAsync($"favorite/group/{type}/{name}/{ownerId}");
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class FavoriteId: IdAbstract<FavoriteId>
	{
		public FavoriteId() => guid = Guid.NewGuid();

		private static ILogger Logger => Global.LoggerFactory.CreateLogger<FavoriteId>();

		public override string prefix => "fvrt";

		public FavoriteId(string id) => this.id = id;

		
		public static implicit operator string(FavoriteId favorite) => favorite.ToString();
		public static implicit operator FavoriteId(string s) => new FavoriteId(s);

		/// <summary>
		/// Get Favorite object from id
		/// </summary>
		/// <returns>Favorite object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Favorite> Get()
		{
			Logger.LogDebug("Get Favorite {id}", id);
			var response = await Global.httpClient.GetAsync($"favorites/{id}");
			return await Utils.UtilFunctions.ParseResponse<Favorite>(response);
		}

		/// <summary>
		/// Delete favorite
		/// </summary>
		/// <returns>Success message</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<JObject> Delete()
		{
			Logger.LogDebug("Delete favorite {id}", id);
			var response = await Global.httpClient.DeleteAsync($"favorites/{id}");
			return JObject.Parse(await response.Content.ReadAsStringAsync());
		}
	}

	public class Favorite
	{
		public FavoriteId id { get; set; }
		public FavoriteType type { get; set; }
		public string favoriteId { get; set; }
		public List<string> tags { get; set; }
	}
}
