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
	internal class TokenCredential : ITokenCredential
	{
		private readonly Cookie _Token;
		private readonly Cookie _TFAToken;

		public TokenCredential(string AuthToken, Uri baseAddress)
		{
			_Token = new Cookie("auth", AuthToken, baseAddress.PathAndQuery, baseAddress.Host);
			_TFAToken = null;
		}
		public TokenCredential(Cookie AuthToken, Cookie TFAToken = null)
		{
			if(AuthToken == null)
				throw new ArgumentNullException(nameof(AuthToken));
			_Token = AuthToken;
			_TFAToken = TFAToken;
		}

		public string AuthToken => _Token?.Value;

		public string TFAToken => _TFAToken?.Value;

		public async Task<LoginInfo> Login(IAPIHttpClient session, JsonSerializerOptions option, CancellationToken ct = default)
		{
			var req = new HttpRequestMessage(HttpMethod.Get, "auth/user");
			req.Headers.Add("cookie", $"{_Token.Name}={_Token.Value}");
			if (!(TFAToken is null))
				req.Headers.Add("cookie", $"{_TFAToken.Name}={_TFAToken.Value}");
			var r = await session.Send(req, ct);
			var j = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync());
			if (!j.TryGetProperty("requiresTwoFactorAuth", out var _))
				return new LoginInfo { User=j.Deserialize<CurrentUser>(option) };
			return LoginInfo.TFARequiredInfo;
		}
	}
}