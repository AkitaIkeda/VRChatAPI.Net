using System;
using System.Buffers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using VRChatAPI.Extentions.DependancyInjection;

namespace TestClient
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var t = CreateHostBuilder(args).Build().RunAsync();
			await t;
		}
		static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices(services =>
					services.AddVRCAPI());
	}
}
