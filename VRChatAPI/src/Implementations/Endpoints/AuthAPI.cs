using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	partial class Session : IAuthAPI
	{
		private const string authEndpoint = "auth";
		private const string userEndpoint = "user";
		public Task<CurrentUser> DeleteAccount(CancellationToken ct = default) =>
			client.Put<CurrentUser>($"{userEndpoint}/{User.GetIDString()}/delete", ct);

		public async Task<bool> IfExist(
			string email = null,
			string displayName = null,
			UserID userId = null,
			UserID excludeUserId = null,
			CancellationToken ct = default) =>
			(await client.Get<JsonElement>($@"{authEndpoint}/exists?{QueryConstructor.MakeQuery(
				new Dictionary<string, object>()
				{
					{ "email", email },
					{ "displayName", displayName },
					{ "userId", userId },
					{ "excludeUserId", excludeUserId },
				}, serializerOption)}", ct))
				.GetProperty("userExists").GetBoolean();

		public Task<CurrentUser> GetCurrentUser(CancellationToken ct = default) =>
			client.Get<CurrentUser>($"{authEndpoint}/{userEndpoint}", ct);

		public Task<ResponseMessage> Logout(CancellationToken ct = default) =>
			client.Put<ResponseMessage>("logout", ct);

		public async Task<(bool Ok, ITokenCredential Token)> Verify(CancellationToken ct = default)
		{
			var r = await client.Get<JsonElement>($"{authEndpoint}", ct);
			return (
				r.GetProperty("ok").GetBoolean(), 
				new TokenCredential(r.GetProperty("token").GetString())
				);
		}

		public async Task<bool> VerifyTwoFactorAuth(string code, CancellationToken ct = default) =>
			(await client.Post<JsonElement, Dictionary<string, object>>(
					$"{authEndpoint}/twofactorauth/totp/verify",
					new Dictionary<string, object> { { "code", code } }, ct)
			).GetProperty("verified").GetBoolean();

		public async Task<bool> VerifyTwoFactorAuthWithRecoveryCode(string code, CancellationToken ct = default) =>
			(await client.Post<JsonElement, Dictionary<string, object>>(
					$"{authEndpoint}/twofactorauth/otp/verify",
					new Dictionary<string, object> { { "code", code } }, ct)
			).GetProperty("verified").GetBoolean();
	}
}
