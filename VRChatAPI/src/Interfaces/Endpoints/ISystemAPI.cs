using System;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface ISystemAPI
	{
		Task<APIConfig> GetAPIConfig(CancellationToken ct = default);
		Task<DateTime> GetServerSystemTime(CancellationToken ct = default);
		Task<uint> GetOnlineUserNum(CancellationToken ct = default);
	}
}