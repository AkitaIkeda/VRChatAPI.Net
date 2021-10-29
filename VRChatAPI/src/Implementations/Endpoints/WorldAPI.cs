using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IWorldAPI
	{
		private const string worldEndpoint = "worlds";
		private const bool instanceEndpointSwitch = true;
		public Task<World> Create(WorldCreateParams param, CancellationToken ct = default) => 
			client.Post<World, WorldCreateParams>(worldEndpoint, param, ct);

		public Task<World> Delete(IWorld obj, CancellationToken ct = default) => 
			client.Delete<World>($"{worldEndpoint}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<LimitedWorld>> Get(WorldSearchParams option, int n, int offset, CancellationToken ct = default) =>
			client.Get<IEnumerable<LimitedWorld>>($"{worldEndpoint}/{option.WorldCategory?.ToString().ToLower()}?{QueryConstructor.MakeQuery(option, serializerOption)}", ct);

		public Task<World> Get(IWorld obj, CancellationToken ct = default) =>
			client.Get<World>($"{worldEndpoint}/{obj.GetIDString()}", ct);

		public Task<Instance> Get(IInstance obj, CancellationToken ct = default) =>
			client.Get<Instance>(instanceEndpointSwitch ? 
				$"{worldEndpoint}/{obj.GetWorldIDString()}/{obj.GetInstanceIDString()}" :
				$"instances/{obj.GetLocationString()}", ct);

		public async Task<bool> GetPublishStatus(IWorld world, CancellationToken ct = default) =>
			(await client.Get<JsonElement>($"{worldEndpoint}/{world.GetIDString()}/publish", ct))
				.GetProperty("canPubilsh").GetBoolean();

		public Task<IEnumerable<LimitedWorld>> Get(DynamicWorldRow row, EPlatform? currentPlatform, int n, int offset, CancellationToken ct = default) =>
			Get(new WorldSearchParams
			{
				Sort = row.SortHeading,
				User = row.SortOwnership.ToLower() == "mine" ? EUserCategory.me : (EUserCategory?)null,
				Order = row.SortOrder,
				Tag = row.Tag,
				Platform = currentPlatform,
			}, n, offset, ct);

		public Task Publish(IWorld world, CancellationToken ct = default) =>
			client.Put($"{worldEndpoint}/{world.GetIDString()}/publish", ct);

		public Task UnPublish(IWorld world, CancellationToken ct = default) =>
			client.Delete($"{worldEndpoint}/{world.GetIDString()}/publish", ct);

		public Task<World> Update(IWorld from, World to, CancellationToken ct = default) =>
			client.Put<World, World>($"{worldEndpoint}/{from.GetIDString()}", to, ct);
	}
}
