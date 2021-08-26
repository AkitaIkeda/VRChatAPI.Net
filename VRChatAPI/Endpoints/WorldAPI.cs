using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRChatAPI.Objects;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using VRChatAPI.Utils;

namespace VRChatAPI.Endpoints
{
	public class WorldAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<WorldAPI>();

		/// <summary>
		/// Search worlds
		/// </summary>
		/// <param name="endpoint">Endpoint to call</param>
		/// <param name="featured">Is the world featured</param>
		/// <param name="sort">Sort method</param>
		/// <param name="user">Filter by user category</param>
		/// <param name="userId">Filter by userId</param>
		/// <param name="n">Max num of returns</param>
		/// <param name="order">Result ordering</param>
		/// <param name="offset">Offset from 0</param>
		/// <param name="search">Filter by name</param>
		/// <param name="tags">Filter by tags (Comma seperated string)</param>
		/// <param name="notag">Tags to exclude (Comma seperated string)</param>
		/// <param name="releaseStatus">Filter by release status</param>
		/// <param name="maxUnityVersion">Max unity version the world supports</param>
		/// <param name="minUnityVersion">Min unity version the world supports</param>
		/// <param name="maxAssetVersion">Max asset version the world supports</param>
		/// <param name="minAssetVersion">Min asset version the world supports</param>
		/// <param name="platform">The platform the world supports</param>
		/// <returns>List of worlds</returns>
		public async Task<List<LimitedWorld>> Search(
			WorldGroups endpoint = WorldGroups.Any, 
			bool? featured = null,
			SortOptions? sort = null, 
			UserOptions? user = null,
			UserId userId = null, 
			[Range(1, 100)] uint? n = null, 
			OrderOptions? order = null,
			uint? offset = null,
			string search = null, 
			string tags = null, 
			string notag = null,
			ReleaseStatus? releaseStatus = null,
			string maxUnityVersion = null,
			string minUnityVersion = null,
			string maxAssetVersion = null,
			string minAssetVersion = null,
			PlatformEnum? platform = null)
		{
			var qDict = new Dictionary<string, object>{
				{"featured", sort?.ToString().ToLowerInvariant()},
				{"sort", featured},
				{"user", user?.ToString().ToLowerInvariant()},
				{"userId", userId},
				{"n", n},
				{"order", order?.ToString().ToLowerInvariant()},
				{"offset", offset},
				{"search", search},
				{"tag", tags},
				{"notag", notag},
				{"releaseStatus", releaseStatus?.ToString().ToLowerInvariant()},
				{"maxUnityVersion", maxUnityVersion},
				{"minUnityVersion", minUnityVersion},
				{"maxAssetVersion", maxAssetVersion},
				{"minAssetVersion", minAssetVersion},
			};

			Logger.LogDebug("Getting world list {params}", Utils.UtilFunctions.MakeQuery(qDict, ", "));
			var param = Utils.UtilFunctions.MakeQuery(qDict);

			var map = new Dictionary<WorldGroups, string>
			{
				{WorldGroups.Any, ""},
				{WorldGroups.Active, "/active"},
				{WorldGroups.Recent, "/recent"},
				{WorldGroups.Favorite, "/favorites"},
			};
			HttpResponseMessage response = await Global.httpClient.GetAsync($"worlds{map[endpoint]}{param}");
			return await Utils.UtilFunctions.ParseResponse<List<LimitedWorld>>(response);
		}

		/// <summary>
		/// Search world and get all result
		/// </summary>
		/// <param name="endpoint">Endpoint to call</param>
		/// <param name="featured">Is the world featured</param>
		/// <param name="sort">Sort method</param>
		/// <param name="user">Filter by user category</param>
		/// <param name="userId">Filter by userId</param>
		/// <param name="order">Result ordering</param>
		/// <param name="search">Filter by name</param>
		/// <param name="tags">Filter by tags (Comma seperated string)</param>
		/// <param name="notag">Tags to exclude (Comma seperated string)</param>
		/// <param name="releaseStatus">Filter by release status</param>
		/// <param name="maxUnityVersion">Max unity version the world supports</param>
		/// <param name="minUnityVersion">Min unity version the world supports</param>
		/// <param name="maxAssetVersion">Max asset version the world supports</param>
		/// <param name="minAssetVersion">Min asset version the world supports</param>
		/// <param name="platform">The platform the world supports</param>
		/// <param name="bufferSize">Buffer Size</param>
		/// <returns>All result</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public AsyncSequentialReader<LimitedWorld> SearchSequential(
			WorldGroups endpoint = WorldGroups.Any, 
			bool? featured = null,
			SortOptions? sort = null, 
			UserOptions? user = null,
			UserId userId = null, 
			OrderOptions? order = null,
			string search = null, 
			string tags = null, 
			string notag = null,
			ReleaseStatus? releaseStatus = null,
			string maxUnityVersion = null,
			string minUnityVersion = null,
			string maxAssetVersion = null,
			string minAssetVersion = null,
			PlatformEnum? platform = null,
			[Range(1, 100)] uint bufferSize = 100
		) =>
			new AsyncSequentialReader<LimitedWorld>(
				async (uint offset, uint n) => await Search(
					endpoint: endpoint,
					featured: featured,
					sort: sort,
					user: user,
					userId: userId,
					order: order,
					search: search,
					tags: tags,
					notag: notag,
					releaseStatus: releaseStatus,
					maxUnityVersion: maxUnityVersion,
					minUnityVersion: minUnityVersion,
					maxAssetVersion: maxAssetVersion,
					minAssetVersion: minAssetVersion,
					platform: platform,
					n: n,
					offset: offset
				),
				bufferSize
			);
	}
}
