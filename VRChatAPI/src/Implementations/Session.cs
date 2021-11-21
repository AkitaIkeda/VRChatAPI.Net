using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Extentions.DependencyInjection;
using VRChatAPI.Interfaces;
using VRChatAPI.Logging;
using VRChatAPI.Objects;

namespace VRChatAPI.Implementations
{
	public partial class Session : ISession
	{
		private readonly ILogger logger;
		private readonly IAPIHttpClient client;
		private readonly IWSEventHandler eventHandler;
		private readonly IOptions<VRCAPIOptions> options;
		private APIConfig remoteConfig;
		private readonly LoginInfo loginInfo;
		private JsonSerializerOptions serializerOption => options.Value.SerializerOption;

		public Session(
			IAPIHttpClient client,
			IWSEventHandler eventHandler,
			IOptions<VRCAPIOptions> options,
			ILogger<Session> logger)
		{
			this.logger = (ILogger)logger ?? NullLogger.Instance;
			logger.LogInformation(LogEventID.SystemInitialize, "Initializing.");
			this.client = client;
			this.eventHandler = eventHandler;
			this.options = options;
			this.loginInfo = new LoginInfo();

			GetAPIConfig().ContinueWith(v => remoteConfig = v.Result);
			
			eventHandler.OnUserUpdate += UpdateUserInfo;
			client.OnRequestFailedWithResponseMessage += OnRequestFailedWithResponseMessage;
		}

		private void OnRequestFailedWithResponseMessage(object sender, ResponseMessage e)
		{
			if(e.StatusCode == 401) loginInfo.User = null;
		}

		private void UpdateUserInfo(CurrentUser user) => loginInfo.User = user;

		#region interface implementations
		public bool HandlingWSEvents => eventHandler.IsHandling;

		public IWSEventHandler EventHandler => eventHandler;
		public IAPIHttpClient APIHttpClient => client;

		public ITokenCredential Credential => client.GetCredential();

		public CurrentUser User => loginInfo.User;
		public bool IsLoggedIn => loginInfo.TFARequired;

		public APIConfig RemoteConfig => remoteConfig;
		public LoginInfo LoginInfo => loginInfo;

		public void StartWSEventHandling() => 
			EventHandler.StartHandling(Credential);

		public void StopWSEventHandling() => 
			EventHandler.StopHandling();

		public async Task<LoginInfo> Login(ICredential credential, CancellationToken ct = default)
		{
			var t = await credential.Login(this.client, serializerOption, ct);
			loginInfo.User = t.User;
			return loginInfo;
		} 

		#endregion
	}
}