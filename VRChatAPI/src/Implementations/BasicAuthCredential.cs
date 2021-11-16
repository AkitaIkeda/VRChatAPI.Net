using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Extentions;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Utils;

namespace VRChatAPI.Implementations
{
	public class BasicAuthCredential : ICredential
	{
		private string auth;
		private string tfaToken;
		public BasicAuthCredential(string id, string pass, string TFAToken = null)
		{
			auth = Convert.ToBase64String(
				Encoding.UTF8.GetBytes($"{id}:{pass}"));
			this.tfaToken = TFAToken;
		}

		public async Task<LoginInfo> Login(IAPIHttpClient session, JsonSerializerOptions option, CancellationToken ct = default)
		{
			if (auth is null)
				throw new InvalidOperationException($"{nameof(BasicAuthCredential)} can be used to login only once.");
			if (!(tfaToken is null))
				session.Client.DefaultRequestHeaders.Add("cookie", $"twoFactorAuth={tfaToken}");

			var req = new HttpRequestMessage(HttpMethod.Get, "auth/user");
			req.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
			var r = await session.Send(req, ct);
			var j = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync());
			auth = null;
			if (!j.TryGetProperty("requiresTwoFactorAuth", out var _))
				return new LoginInfo { User = j.Deserialize<CurrentUser>(option) };
			return LoginInfo.TFARequiredInfo;
		}
	}
}
