using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VRChatAPI.Objects;
using static VRChatAPI.Utils.UtilFunctions;
using Microsoft.Extensions.Logging;
using VRChatAPI.Utils;

namespace VRChatAPI.Endpoints
{
	public class FavoriteAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<FavoriteAPI>();

		internal FavoriteAPI(){}

		/// <summary>
		/// Add object to favorite
		/// </summary>
		/// <param name="type">ObjectType</param>
		/// <param name="objectId">Object Id</param>
		/// <param name="tags">Group tags where object will be added to </param>
		/// <returns>New Favorite object</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<Favorite> AddToFavorite(FavoriteType type, string objectId, params FavoriteGroupId[] tags)
		{
			Logger.LogDebug("Add an {objectId}({type}) to favorite {tags}", objectId, type.ToString(), tags);
			JObject json = new JObject()
			{
				{ "type", type.ToString() },
				{ "favoriteId", objectId },
				{ "tags", new JArray(tags)}
			};

			StringContent content = new StringContent(json.ToString(), Encoding.UTF8);

			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

			HttpResponseMessage response = await Global.httpClient.PostAsync($"favorites", content);

			return await ParseResponse<Favorite>(response);
		}

		/// <summary>
		/// Get list of favorite
		/// </summary>
		/// <param name="favoriteType">Type of object</param>
		/// <param name="n">Max num of returns</param>
		/// <param name="offset">Offset from 0</param>
		/// <returns>List of favorite objects</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<List<Favorite>> ListFavorite(FavoriteType? favoriteType = null, [Range(1, 100)] uint? n = null, uint? offset = null)
		{
			Logger.LogDebug("Get Favorite {params}", MakeQuery(new Dictionary<string, object>(){{"type", favoriteType}, {"n", n}, {"offset", offset}}, ", "));
			HttpResponseMessage response = await Global.httpClient.GetAsync(
				$"favorites?{MakeQuery(new Dictionary<string, object>(){{"type", favoriteType}, {"n", n}, {"offset", offset}})}");
			return await ParseResponse<List<Favorite>>(response);
		}

		/// <summary>
		/// Get all favorites
		/// </summary>
		/// <param name="favoriteType">Type of object</param>
		/// <param name="bufferSize">Buffer Size</param>
		/// <returns>IEnumerable of all favorites</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public AsyncSequentialReader<Favorite> ListFavoriteSequential(FavoriteType? favoriteType = null, [Range(1, 100)] uint bufferSize = 100) => 
			new AsyncSequentialReader<Favorite>(
				async (uint offset, uint n) => await ListFavorite(favoriteType, n, offset),
				bufferSize
			);

		/// <summary>
		/// Get Favorite gropus
		/// </summary>
		/// <param name="n">Max num of return</param>
		/// <param name="offset">Offset from 0</param>
		/// <returns>List of FavoriteGroup</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public async Task<List<FavoriteGroup>> GetFavoriteGroups([Range(1, 100)] uint? n = null, uint? offset = null)
		{
			Logger.LogDebug("Get Favorite Groups");
			var response = await Global.httpClient.GetAsync(
				$"favorite/groups?{MakeQuery(new Dictionary<string, object>(){{"n", n}, {"offset", offset}})}");
			return await ParseResponse<List<FavoriteGroup>>(response);
		}

		/// <summary>
		/// Get all Favorite groups
		/// </summary>
		/// <param name="bufferSize">Buffer size</param>
		/// <returns>IEnumerable of all favorite groups</returns>
		/// <exception cref="UnauthorizedRequestException"/>
		public AsyncSequentialReader<FavoriteGroup> GetFavoriteGroupSequential([Range(1, 100)] uint bufferSize = 100) =>
		 new AsyncSequentialReader<FavoriteGroup>(
			 async (uint offset, uint n) => await GetFavoriteGroups(n, offset), 
			 bufferSize
			);
	}
}
