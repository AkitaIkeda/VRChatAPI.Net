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
	public class FileAPI
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<FileAPI>();

		/// <summary>
		/// Create file
		/// </summary>
		/// <param name="name">Name of the file</param>
		/// <param name="mimeType">File type</param>
		/// <param name="extension">extension of the file</param>
		/// <param name="tags">File tags</param>
		/// <returns>Created File object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<File> Create(
			string name,
			MimeType mimeType,
			string extension,
			List<string> tags = null
		)
		{
			var p = new Dictionary<string, object>{
				{ "name", name },
				{ "mimeType", mimeType },
				{ "extension", extension },
				{ "tags", tags },
			};
			Logger.LogDebug("Create file {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var content = new StringContent(JObject.FromObject(p.Where(v => !(v.Value is null))).ToString(), Encoding.UTF8);
			var response = await Global.httpClient.PostAsync("file", content);
			return await Utils.UtilFunctions.ParseResponse<File>(response);
		}

		/// <summary>
		/// Search files
		/// </summary>
		/// <param name="tag">Filter by tag</param>
		/// <param name="userId">UserId, will allways generate a 500 permissions error</param>
		/// <param name="n">The number of objects to return</param>
		/// <param name="offset">Offset from 0</param>
		/// <returns>List of files</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<List<File>> Search(
			string tag = null,
			UserId userId = null,
			[Range(1, 100)] uint? n = null,
			uint? offset = null
		)
		{
			var p = new Dictionary<string, object>{
				{ "tag", tag },
				{ "userId", userId },
				{ "n", n },
				{ "offset", offset },
			};
			Logger.LogDebug("Search files {params}", Utils.UtilFunctions.MakeQuery(p, ", "));
			var response = await Global.httpClient.GetAsync($"files?{Utils.UtilFunctions.MakeQuery(p)}");
			return await Utils.UtilFunctions.ParseResponse<List<File>>(response);
		}

		/// <summary>
		/// Search files
		/// </summary>
		/// <param name="tag">Filter by tag</param>
		/// <param name="userId">UserId, will allways generate a 500 permissions error</param>
		/// <param name="bufferSize">Buffer size</param>
		/// <returns>List of files</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public AsyncSequentialReader<File> SearchSequential(
			string tag = null,
			UserId userId = null,
			[Range(1, 100)] uint bufferSize = 100
		) => new AsyncSequentialReader<File>(
			async (uint offset, uint n) => await Search(tag, userId, n, offset),
			bufferSize);
	}
}