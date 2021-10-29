using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IModerationAPI
	{
		private const string moderationEndpoint = "/auth/user/playermoderations";

		public Task<ResponseMessage> ClearAllModerations(CancellationToken ct = default) =>
			client.Delete<ResponseMessage>(moderationEndpoint, ct);

		public Task<Moderation> Create(ModerationCreateParams param, CancellationToken ct = default) =>
			client.Post<Moderation, ModerationCreateParams>(moderationEndpoint, param, ct);

		public Task<ResponseMessage> Delete(IModeration obj, CancellationToken ct = default) =>
			client.Delete<ResponseMessage>($"{moderationEndpoint}/{obj.GetIDString()}", ct);

		public Task<IEnumerable<Moderation>> Get(ModerationSearchParams option, CancellationToken ct = default) =>
			client.Get<IEnumerable<Moderation>>($"{moderationEndpoint}?{QueryConstructor.MakeQuery(option, serializerOption)}", ct);

		public Task<Moderation> Get(IModeration obj, CancellationToken ct = default) =>
			client.Get<Moderation>($"{moderationEndpoint}/{obj.GetIDString()}", ct);

		public Task<ResponseMessage> Unmoderate(IUser user, EModerationType type, CancellationToken ct = default) =>
			client.Put<ResponseMessage, Dictionary<string, object>>(moderationEndpoint, new Dictionary<string, object>
			{
				{ "moderated", user.GetIDString() },
				{ "type", type },
			}, ct);
	}
}
