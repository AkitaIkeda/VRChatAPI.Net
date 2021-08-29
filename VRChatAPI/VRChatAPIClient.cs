using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;
using VRChatAPI.Endpoints;
using Microsoft.Extensions.Logging;
using VRChatAPI.Utils;

namespace VRChatAPI
{
	/// <summary>
	/// VRChat API wrapper <br/>
	/// Documentation: <see href="https://vrchatapi.github.io/"/>
	/// </summary>
	public class VRChatAPIClient
	{
		/// <summary>
		/// Get or set logger factory that will be used in client
		/// </summary>
		/// <value></value>
		public static ILoggerFactory LoggerFactory { get => Global.LoggerFactory; set => Global.LoggerFactory = value; }
		
		private static ILogger Logger => LoggerFactory.CreateLogger<VRChatAPIClient>();
		
		public HttpClientHandler HttpClientHandler => Global.httpClientHandler;
		public CookieContainer CookieContainer => HttpClientHandler.CookieContainer;

		public SystemAPI SystemAPI { get; private set; }
		public UserAPI UserAPI { get; private set; }
		public WorldAPI WorldAPI { get; private set; }
		public ModerationsAPI ModerationsAPI { get; private set; }
		// public AvatarApi AvatarApi { get; private set; }
		public FavoriteAPI FavoriteAPI { get; private set; }
		public FileAPI FileAPI { get; private set; }

		public WSListener WSListener { get; private set; }

		/// <summary>
		/// Instantiate VRChatApi client without auth
		/// </summary>
		/// <remarks>You will need to Login first in order to call most of methods
		public VRChatAPIClient(){
			Logger.LogDebug($"Entering {nameof(VRChatAPIClient)} constructor with no args");
			initEndpoints();
			initHttpClient(null);
		}
		
		/// <summary>
		/// Instantiate VRChatApi client with auth
		/// </summary>
		/// <param name="username">UserID</param>
		/// <param name="password">Password</param>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public VRChatAPIClient(string username, string password){
			Logger.LogDebug($"Entering {nameof(VRChatAPIClient)} constructor with ID&PASS");
			preInit();
			initHttpClient(username, password).Wait();
			postInitAsync().Wait();
		}
		
		/// <summary>
		/// Instantiate VRChatApi client with Cookie
		/// </summary>
		/// <param name="cookie">CookieContainer that has ApiKey And auth token</param>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public VRChatAPIClient(CookieContainer cookie)
		{
			Logger.LogDebug($"Entering {nameof(VRChatAPIClient)} constructor with CookieContainer");
			preInit();
			initHttpClient(cookie);
			VerifyAuth().Wait();
			postInitAsync().Wait();
		}

		private void preInit(){
			initEndpoints();
			WSListener = new WSListener();
		}
		private async Task postInitAsync(){
			Global.RemoteConfig = await SystemAPI.RemoteConfig();
		}

		/// <summary>
		/// Start to listen for Webssocket API
		/// </summary>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		public async Task WSListen(CancellationToken ct) => await WSListen((await VerifyAuth()).token, ct);

		/// <summary>
		/// Start to listen for Websocket API with specific authToken
		/// </summary>
		/// <param name="authToken">auth token</param>
		public async Task WSListen(string authToken, CancellationToken ct) => await WSListener.Listen(authToken, ct);

		private void initEndpoints(){
			// initialize endpoint classes
			SystemAPI = new SystemAPI();
			UserAPI = new UserAPI();
			WorldAPI = new WorldAPI();
			ModerationsAPI = new ModerationsAPI();
			// AvatarApi = new AvatarApi();
			FavoriteAPI = new FavoriteAPI();
			FileAPI = new FileAPI();
		}

		/// <summary>
		/// Initializes Global.HttpClient
		/// </summary>
		/// <param name="client">HttpClient to use</param>
		/// <exception cref="Exceptions.UnauthorizedRequestException"/>
		private async Task initHttpClient(string username, string password)
		{
			initHttpClient(null);
			await Login(username, password);
		}
		
		/// <summary>
		/// Initializes Global.HttpClient
		/// </summary>
		/// <param name="c">CookieContainer that has "auth" and "apiKey"</param>
		private void initHttpClient(CookieContainer c){
			Logger.LogDebug($"Initializing {nameof(HttpClient)}");
			CustomHttpClientHandler handler;
			if(c is null)
				handler = new CustomHttpClientHandler {
					UseCookies = true,
				};
			else
				handler = new CustomHttpClientHandler {
					CookieContainer = c,
					UseCookies = true,
				};
			Global.httpClientHandler = handler;
			Global.httpClient = new HttpClient(handler);
			Global.httpClient.DefaultRequestHeaders.Add("User-Agent", "C# VRChat API Client");
			Global.httpClient.BaseAddress = new Uri(Global.APIUrl);
			Logger.LogDebug($"VRChat API base address set to {Global.httpClient.BaseAddress}");
		}
		public async Task<Objects.CurrentUser> Login(string username, string password) => await UserAPI.Login(username, password);
		public async Task<(bool ok, string token)> VerifyAuth() => await UserAPI.VerifyAuth();
		public ConfigResponse RemoteConfig => Global.RemoteConfig;
	}
}
