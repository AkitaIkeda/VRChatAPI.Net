using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Endpoints
{
	public class AvatarAPI
	{
	  private static ILogger Logger => Global.LoggerFactory.CreateLogger<AvatarAPI>();

		public async Task<Avatar> Create(
			string name, 
			File imageUrl, 
			AvatarId id = null, 
			string description = null,
			List<string> tags = null,
			ReleaseStatus? releaseStatus = null,
			int? version = null,
			string unityPackageUrl = null)
		{
			var p = new Dictionary<string, object>{
				{ "name", name },
				{ "imageUrl", imageUrl },
				{ "id", id },
				{ "description", description },
				{ "tags", tags },
				{ "releaseStatus", releaseStatus },
				{ "version", version },
				{ "unityPackageUrl", unityPackageUrl },
			};
			Logger.LogDebug("Create avatar {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var content = new StringContent(JObject.FromObject(p.Where(x => !(x.Value is null)).ToDictionary(v => v.Key, v => v.Value)).ToString(), Encoding.UTF8, "application/json");
			var response = await Global.httpClient.PutAsync("avatars", content);
			return await Utils.UtilFunctions.ParseResponse<Avatar>(response);
		}
		
		/// <summary>
		/// Search Avatars
		/// </summary>
		/// <param name="featured">Filters on featured results</param>
		/// <param name="sort">Sort method</param>
		/// <param name="mine">Search own avatars</param>
		/// <param name="userId">Filter by author UserID</param>
		/// <param name="n">The number of objects to return</param>
		/// <param name="order">Order</param>
		/// <param name="offset">Offset from 0</param>
		/// <param name="tag">Tags to include (comma-separated)</param>
		/// <param name="notag">Tags to exclude (comma-separated)</param>
		/// <param name="releaseStatus">Filter by ReleaseStatus</param>
		/// <param name="maxUnityVersion">The maximum Unity version supported by the asset</param>
		/// <param name="minUnityVersion">The minimum Unity version supported by the asset</param>
		/// <param name="platform">The platform the asset supports</param>
		/// <returns>List of results</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<Avatar>> Search(
			bool? featured = null,
			AvatarSortOptions? sort = null,
			bool mine = false,
			UserId userId = null,
			[Range(1, 100)] uint ? n = null,
			OrderOptions? order = null,
			uint? offset = null,
			string tag = null,
			string notag = null,
			ReleaseStatus? releaseStatus = null,
			string maxUnityVersion = null,
			string minUnityVersion = null,
			PlatformEnum? platform = null
		)
		{
			var p = new Dictionary<string, object>{
				{ "featured", featured },
				{ "sort", sort },
				{ "userId", userId },
				{ "n", n }, 
				{ "order", order },
				{ "offset", offset },
				{ "tag", tag },
				{ "notag", notag },
				{ "releaseStatus", releaseStatus },
				{ "maxUnityVersion", maxUnityVersion },
				{ "minUnityVersion", minUnityVersion },
				{ "platform", platform },
			};
			if(mine) p["user"] = mine;
			Logger.LogDebug("Search Avatar: {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var response = await Global.httpClient.GetAsync($"avatars?{Utils.UtilFunctions.MakeQuery(p)}");
			return await Utils.UtilFunctions.ParseResponse<List<Avatar>>(response);
		}

		/// <summary>
		/// Search Avatars and get all results
		/// </summary>
		/// <param name="featured">Filters on featured results</param>
		/// <param name="sort">Sort method</param>
		/// <param name="mine">Search own avatars</param>
		/// <param name="userId">Filter by author UserID</param>
		/// <param name="order">Order</param>
		/// <param name="tag">Tags to include (comma-separated)</param>
		/// <param name="notag">Tags to exclude (comma-separated)</param>
		/// <param name="releaseStatus">Filter by ReleaseStatus</param>
		/// <param name="maxUnityVersion">The maximum Unity version supported by the asset</param>
		/// <param name="minUnityVersion">The minimum Unity version supported by the asset</param>
		/// <param name="platform">The platform the asset supports</param>
		/// <param name="bufferSize">buffer size</param>
		/// <returns>List of results</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public AsyncSequentialReader<Avatar> SearchSequential(
			bool? featured = null,
			AvatarSortOptions? sort = null,
			bool mine = false,
			UserId userId = null,
			OrderOptions? order = null,
			string tag = null,
			string notag = null,
			ReleaseStatus? releaseStatus = null,
			string maxUnityVersion = null,
			string minUnityVersion = null,
			PlatformEnum? platform = null,
			[Range(1, 100)] uint bufferSize = 100
		) =>
			new AsyncSequentialReader<Avatar>(async (uint offset, uint n) => await Search(
				featured: featured,
				sort: sort,
				mine: mine,
				userId: userId,
				n: n,
				order: order,
				offset: offset,
				tag: tag,
				notag: notag,
				releaseStatus: releaseStatus,
				maxUnityVersion: maxUnityVersion,
				minUnityVersion: minUnityVersion,
				platform: platform
			), bufferSize);

		/// <summary>
		/// Search Favorite Avatars
		/// </summary>
		/// <param name="featured">Filters on featured results</param>
		/// <param name="sort">Sort method</param>
		/// <param name="mine">Search own avatars</param>
		/// <param name="userId">Filter by author UserID</param>
		/// <param name="n">The number of objects to return</param>
		/// <param name="order">Order</param>
		/// <param name="offset">Offset from 0</param>
		/// <param name="tag">Tags to include (comma-separated)</param>
		/// <param name="notag">Tags to exclude (comma-separated)</param>
		/// <param name="releaseStatus">Filter by ReleaseStatus</param>
		/// <param name="maxUnityVersion">The maximum Unity version supported by the asset</param>
		/// <param name="minUnityVersion">The minimum Unity version supported by the asset</param>
		/// <param name="platform">The platform the asset supports</param>
		/// <returns>List of results</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<Avatar>> SearchFavorite(
			string featured = null,
			AvatarSortOptions? sort = null,
			bool mine = false,
			UserId userId = null,
			[Range(1, 100)] uint ? n = null,
			OrderOptions? order = null,
			uint? offset = null,
			string tag = null,
			string notag = null,
			ReleaseStatus? releaseStatus = null,
			string maxUnityVersion = null,
			string minUnityVersion = null,
			PlatformEnum? platform = null
		)
		{
			var p = new Dictionary<string, object>{
				{ "featured", featured },
				{ "sort", sort },
				{ "userId", userId },
				{ "n", n }, 
				{ "order", order },
				{ "offset", offset },
				{ "tag", tag },
				{ "notag", notag },
				{ "releaseStatus", releaseStatus },
				{ "maxUnityVersion", maxUnityVersion },
				{ "minUnityVersion", minUnityVersion },
				{ "platform", platform },
			};
			if(mine) p["user"] = mine;
			Logger.LogDebug("Search Avatar: {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var response = await Global.httpClient.GetAsync($"avatars/favorites?{Utils.UtilFunctions.MakeQuery(p)}");
			return await Utils.UtilFunctions.ParseResponse<List<Avatar>>(response);
		}

		/// <summary>
		/// Search Favorite Avatars and get all results
		/// </summary>
		/// <param name="featured">Filters on featured results</param>
		/// <param name="sort">Sort method</param>
		/// <param name="mine">Search own avatars</param>
		/// <param name="userId">Filter by author UserID</param>
		/// <param name="order">Order</param>
		/// <param name="tag">Tags to include (comma-separated)</param>
		/// <param name="notag">Tags to exclude (comma-separated)</param>
		/// <param name="releaseStatus">Filter by ReleaseStatus</param>
		/// <param name="maxUnityVersion">The maximum Unity version supported by the asset</param>
		/// <param name="minUnityVersion">The minimum Unity version supported by the asset</param>
		/// <param name="platform">The platform the asset supports</param>
		/// <param name="bufferSize">buffer size</param>
		/// <returns>List of results</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public AsyncSequentialReader<Avatar> SearchFavoriteSequential(
			string featured = null,
			AvatarSortOptions? sort = null,
			bool mine = false,
			UserId userId = null,
			OrderOptions? order = null,
			string tag = null,
			string notag = null,
			ReleaseStatus? releaseStatus = null,
			string maxUnityVersion = null,
			string minUnityVersion = null,
			PlatformEnum? platform = null,
			[Range(1, 100)] uint bufferSize = 100
		) =>
			new AsyncSequentialReader<Avatar>(async (uint offset, uint n) => await SearchFavorite(
				featured: featured,
				sort: sort,
				mine: mine,
				userId: userId,
				n: n,
				order: order,
				offset: offset,
				tag: tag,
				notag: notag,
				releaseStatus: releaseStatus,
				maxUnityVersion: maxUnityVersion,
				minUnityVersion: minUnityVersion,
				platform: platform
			), bufferSize);
	}
}
