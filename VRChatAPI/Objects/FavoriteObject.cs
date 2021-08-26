using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

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
		Private,
		[EnumMember(Value = "public")]
		Public,
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class FavoriteGroupId
	{
		public string name { get; set; }
		
		public static implicit operator string(FavoriteGroupId favorite) => favorite.ToString();
		public static implicit operator FavoriteGroupId(string s) => new FavoriteGroupId(){name = s};
		public override string ToString() => name;
	}

	public class FavoriteGroup : FavoriteGroupId
	{
		public string id { get; set; }
		public string displayName { get; set; }
		public string ownerDisplayName { get; set; }
		public UserId ownerId { get; set; }
		public List<string> tags { get; set; }
		public FavoriteType type { get; set; }
		public FavoriteGroupVisibility visibility { get; set; }
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class FavoriteId
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<FavoriteId>();

		public FavoriteId(string id) => this.id = id;

		public string id { get; set; }
		
		public static implicit operator string(FavoriteId favorite) => favorite.ToString();
		public static implicit operator FavoriteId(string s) => new FavoriteId(s);
		public override string ToString() => id;

		/// <summary>
		/// Get Favorite object from id
		/// </summary>
		/// <returns>Favorite object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Favorite> GetFavorite()
		{
			Logger.LogDebug("Get Favorite {id}", id);
			HttpResponseMessage response = await Global.httpClient.GetAsync($"favorites/{id}");
			return await Utils.UtilFunctions.ParseResponse<Favorite>(response);
		}

		/// <summary>
		/// Delete favorite
		/// </summary>
		/// <returns>Success message</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<JObject> DeleteFavorite()
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
