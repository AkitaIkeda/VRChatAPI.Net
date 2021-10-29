using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IWorldAPI :
		IGet<World, LimitedWorld, IWorld, WorldSearchParams>,
		IGet<Instance, IInstance>, IDelete<World, IWorld>,
		ICreate<World, WorldCreateParams>, IUpdate<IWorld, World>
	{
		Task<bool> GetPublishStatus(IWorld world, CancellationToken ct = default);
		Task /*TODO: Impl Response*/ Publish(IWorld world, CancellationToken ct = default);
		Task /*TODO: Impl Response*/ UnPublish(IWorld world, CancellationToken ct = default);
		Task<IEnumerable<LimitedWorld>> Get(DynamicWorldRow row, EPlatform? currentPlatform, int n, int offset, CancellationToken ct = default);
	}
}