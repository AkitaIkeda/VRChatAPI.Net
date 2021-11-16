using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Extentions;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	[Serializable]
	public class TokenCredential : ITokenCredential
	{
		public TokenCredential(){}

		public TokenCredential(string AuthToken, string TFAToken = null)
		{
			this.AuthToken = AuthToken;
			this.TFAToken = TFAToken;
		}

		public string AuthToken { get; set; }

		public string TFAToken { get; set; }

		public async Task<LoginInfo> Login(IAPIHttpClient session, JsonSerializerOptions option, CancellationToken ct = default)
		{
			session.Client.DefaultRequestHeaders.Add("cookie", $"auth={AuthToken}");
			if (!(TFAToken is null))
				session.Client.DefaultRequestHeaders.Add("cookie", $"twoFactorAuth={TFAToken}");
			var req = new HttpRequestMessage(HttpMethod.Get, "auth/user");
			var r = await session.Send(req, ct);
			var j = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync());
			if (!j.TryGetProperty("requiresTwoFactorAuth", out var _))
				return new LoginInfo { User=j.Deserialize<CurrentUser>(option) };
			return LoginInfo.TFARequiredInfo;
		}
	}
}