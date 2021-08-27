using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VRChatAPI.Abstracts;
using VRChatAPI.Converters;

namespace VRChatAPI.Objects
{
	using Data = Newtonsoft.Json.Linq.JToken;

	[JsonConverter(typeof(AsTConverter<string>))]
	public class PermissionId : IdAbstract<PermissionId>
	{
		public PermissionId() => guid = Guid.NewGuid();
		private ILogger Logger => Global.LoggerFactory.CreateLogger<PermissionId>();
		public override string prefix => "prms";
		public PermissionId(string s) => this.id = s;
		public static implicit operator PermissionId(string s) => new PermissionId(s);
		public static implicit operator string(PermissionId id) => id.ToString();

		public async Task<Permission> Get()
		{
			Logger.LogDebug("Get permission {id}", id);
			var response = await Global.httpClient.GetAsync($"permissions/{id}");
			return await Utils.UtilFunctions.ParseResponse<Permission>(response);
		}
	}

	public class Permission
	{
		public PermissionId id { get; set; }
		public UserId ownerId { get; set; }
		public string name { get; set; }
		public Data data { get; set; }
	}
}