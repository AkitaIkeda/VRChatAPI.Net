using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using VRChatAPI.Extentions.DependancyInjection;
using VRChatAPI.Interfaces;
using VRChatAPI.Logging;
using VRChatAPI.Objects;

namespace VRChatAPI.Implementations
{
	public partial class Session : ISession, IDisposable
	{
		private readonly IServiceScope serviceScope;
		private IServiceProvider serviceProvider => serviceScope.ServiceProvider;
		private readonly IAPIHttpClient client;
		private readonly APIConfig remoteConfig;
		private readonly IWSEventHandler eventHandler;
		private readonly ILogger logger;
		private readonly IOptions<VRCAPIOptions> options;
		private LoginInfo loginInfo;
		private JsonSerializerOptions serializerOption => options.Value.SerializerOption;

		public Session(IServiceScope serviceScope, ICredential cred)
		{
			this.serviceScope = serviceScope;
			logger = (ILogger)serviceProvider.GetRequiredService<ILogger<Session>>() ?? NullLogger.Instance;
			logger.LogInformation(LogEventID.SystemInitialize, "Initializing.");
			options = serviceProvider.GetRequiredService<IOptions<VRCAPIOptions>>();
			client = serviceProvider.GetRequiredService<IAPIHttpClient>();
			var tr = GetAPIConfig();
			var tc = cred.Login(client, options.Value.SerializerOption);
			Task.WaitAll(tr, tc);
			remoteConfig = tr.Result;
			loginInfo = tc.Result;
			if (loginInfo.TFARequired)
				logger.LogInformation(LogEventID.SystemInitialize, "Login process is not finished. You need to verify with 2FA.");
			else
				logger.LogInformation(LogEventID.SystemInitialize, "Loged into {CurrentUser}", loginInfo.User);
			eventHandler = serviceProvider.GetRequiredService<IWSEventHandler>();
		}

		#region interface implementations
		public bool HandlingWSEvents => !(eventHandler is null);

		public IWSEventHandler EventHandler => eventHandler;
		public IAPIHttpClient APIHttpClient => client;

		public ITokenCredential Credential => client.GetCredential();

		public CurrentUser User => loginInfo.User;
		public bool TFARequired => loginInfo.TFARequired;

		public APIConfig RemoteConfig => remoteConfig;

		public void Dispose()
		{
			logger.LogInformation(LogEventID.SystemDispose, "Disposing.");
			serviceScope.Dispose();
		}
		#endregion
	}
}