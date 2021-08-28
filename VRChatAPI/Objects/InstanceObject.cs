using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using VRChatAPI.Converters;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	public class Platforms 
	{
		public int standalonewindows { get; set; }
		public int android { get; set; }
	}

	public class WorldInstance
	{
		public Location location { get; set; }
		public int occupants { get; set; }
	}

	[JsonConverter(typeof(AsTConverter<string>))]
	public class Location
	{
		private ILogger Logger => Global.LoggerFactory.CreateLogger<Location>();

		public Location(string location)
		{
			if(location.Split(':') is var l && l.Length > 1)
			{
				WorldId = l[0];
			 	InstanceId = l[1];
			}
		}

		public Location(string worldId, string instanceId)
		{
			WorldId = worldId;
			InstanceId = instanceId;
		}

		public WorldId WorldId { get; set; }
		public string InstanceId { get; set; }

		public static implicit operator string(Location location) => location.ToString();
		public static implicit operator Location(string s) => new Location(s);

		public override string ToString() => $"{WorldId}:{InstanceId}";

		/// <summary>
		/// Get Instance object from Location
		/// </summary>
		/// <returns>Instance object</returns>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task<Instance> GetInstance()
		{
			Logger.LogDebug("Get instance {location}", this.ToString());
			var response = await Global.httpClient.GetAsync($"worlds/{WorldId}/{InstanceId}");
			return await Utils.UtilFunctions.ParseResponse<Instance>(response);
		}

		/// <summary>
		/// Send Votekick to the user in this location
		/// </summary>
		/// <param name="id">User id to send vote kick for</param>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task VoteKick(UserId id)
		{
			Logger.LogDebug("Vote kick to {userId} in {location}", id, this.ToString());
			var json = new JObject();
			json.Add("worldId", WorldId.ToString());
			json.Add("instanceId", InstanceId.ToString());
			var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
			await Global.httpClient.PostAsync($"user/{id}/votekick", content);
		}
	}

	public class Instance
	{
		public bool active { get; set; }
		public bool canRequestInvite { get; set; }
		public int capacity { get; set; }
		public string clientNumber { get; set; }
		public bool full { get; set; }
		public Location id { get; set; }
		public string instanceId { get; set; }
		public Location location { get; set; }
		public int n_users { get; set; }
		public string name { get; set; }
		public string nonce { get; set; }
		public UserId ownerId { get; set; }
		public bool permanent { get; set; }
		public string photonRegion { get; set; }
		public Platforms platforms { get; set; } 
		public string region { get; set; }
		public string shortName { get; set; }
		public List<string> tags { get; set; }
		public InstanceType type { get; set; }
		public WorldId worldId { get; set; }
	}
}
