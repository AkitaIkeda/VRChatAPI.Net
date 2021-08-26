using System;
using System.Threading.Tasks;
using VRChatAPI;
using Microsoft.Extensions.Logging;

namespace TestClient
{
	class Program
	{
		static async Task Main(string[] args)
		{
			Console.Write("username: ");
			var username = Console.ReadLine();
			Console.Write("password: ");
			var pass = ReadPassword();
			Console.WriteLine("\nLogging in...");
			var api = new VRChatAPIClient(username, pass);
			pass = null;
			VRChatAPIClient.LoggerFactory = LoggerFactory.Create(builder => {
				builder.AddJsonConsole(options => {
					options.IncludeScopes = true;
				});
			});

			var config = await api.SystemApi.RemoteConfig();
			var currentUser = await api.UserApi.GetCurrentUser();
			var currentAvatar = await currentUser.currentAvatar.Get();
			var friends = await api.UserApi.GetFriends(offline: true);
			var notifications = await api.UserApi.GetAllNotifications();
			var worlds = await api.WorldApi.Search();
			Console.ReadKey();
		}

		static string ReadPassword(){
			var ret = "";
			while(true){
				var cki = Console.ReadKey(true);
				if(cki.KeyChar == '\r') break;
				ret += cki.KeyChar;
			}
			return ret;
		}
	}
}
