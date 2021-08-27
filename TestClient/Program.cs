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

			var config = await api.SystemAPI.RemoteConfig();
			var currentUser = await api.UserAPI.GetCurrentUser();
			var currentAvatar = await currentUser.currentAvatar.Get();
			var friends = await api.UserAPI.GetFriends(offline: true);
			var notifications = await api.UserAPI.GetNotifications();
			var worlds = await api.WorldAPI.Search();
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
