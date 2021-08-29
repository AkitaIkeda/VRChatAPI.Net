using System;
using System.Threading.Tasks;
using VRChatAPI;
using Microsoft.Extensions.Logging;
using VRChatAPI.Objects;
using VRChatAPI.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Linq;

namespace TestClient
{
	class Program
	{
		static async Task Main(string[] args)
		{
			init:
			VRChatAPIClient api;
			Console.Write("username: ");
			var username = Console.ReadLine();
			Console.Write("password: ");
			var pass = ReadPassword();
			Console.WriteLine("\nLogging in...");
			try
			{
				api = new VRChatAPIClient(username, pass);
			}
			catch (Exception)
			{
				goto init;
			}
			pass = null;
			VRChatAPIClient.LoggerFactory = LoggerFactory.Create(builder => {
				builder.AddConsole(options => {});
				builder.SetMinimumLevel(LogLevel.Information);
			});

			var config = await api.SystemAPI.RemoteConfig();
			var currentUser = await api.UserAPI.GetCurrentUser();
			var currentAvatar = await currentUser.currentAvatar.Get();
			var friends = await api.UserAPI.GetFriends(offline: true);
			var notifications = await api.UserAPI.GetNotifications();
			var worlds = await api.WorldAPI.Search();
			var files = await api.FileAPI.Search();

			// File upload test
			var f = (await api.FileAPI.Search(tag: "test")).FirstOrDefault();
			f ??= await api.FileAPI.Create("test", MimeType.image_png, ".png", new List<string>(){"test"});
			using (var s = System.IO.File.OpenRead("res/test.png"))
			{
				var c = new CancellationTokenSource();
				f = await f.CreateNewVersionAndUploadFile(s, c.Token);
			}
			await f.id.Delete();

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
