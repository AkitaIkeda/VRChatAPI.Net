using System;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;

namespace VRChatAPI.Implementations
{
	partial class Session : ISystemAPI
	{
		public Task<APIConfig> GetAPIConfig(CancellationToken ct = default) =>
			client.Get<APIConfig>("config", ct);

		public Task<uint> GetOnlineUserNum(CancellationToken ct = default) =>
			client.Get<uint>("vicits", ct);

		public Task<DateTime> GetServerSystemTime(CancellationToken ct = default) =>
			client.Get<DateTime>("time", ct);
	}
}
