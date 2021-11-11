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
		private readonly APIConfig remoteConfig;
		private readonly ILogger logger;
		private readonly IAPIHttpClient client;
		private readonly IWSEventHandler eventHandler;
		private readonly IOptions<VRCAPIOptions> options;
		private LoginInfo loginInfo;
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
			
			var tr = GetAPIConfig();
			remoteConfig = tr.Result;

			eventHandler.OnUserUpdate += UpdateUserInfo;
		}

		private void UpdateUserInfo(CurrentUser user) => loginInfo.User = user;

		#region interface implementations
		public bool HandlingWSEvents => eventHandler.IsHandling;

		public IWSEventHandler EventHandler => eventHandler;
		public IAPIHttpClient APIHttpClient => client;

		public ITokenCredential Credential => client.GetCredential();

		public CurrentUser User => loginInfo.User;
		public bool TFARequired => loginInfo.TFARequired;

		public APIConfig RemoteConfig => remoteConfig;

		public void StartWSEventHandling() => 
			EventHandler.StartHandling(Credential);

		public void StopWSEventHandling() => 
			EventHandler.StopHandling();

		public async Task<LoginInfo> Login(ICredential credential, CancellationToken ct = default) => 
			loginInfo = await credential.Login(this.client, serializerOption, ct);

		#endregion
	}
}