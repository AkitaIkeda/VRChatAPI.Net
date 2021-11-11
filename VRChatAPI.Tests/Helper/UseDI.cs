using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using VRChatAPI.Extentions.DependencyInjection;
using VRChatAPI.Implementations;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Tests.Helper
{
	public class UseDI : IDisposable
	{
		protected readonly IHost host;
		protected IServiceProvider services => host.Services;
		protected readonly IOptions<VRCAPIOptions> options;
		protected JsonSerializerOptions serializerOptions => options.Value.SerializerOption;

		static IHostBuilder CreateHostBuilder(Action<IServiceCollection> f) =>
			Host.CreateDefaultBuilder()
				.ConfigureServices(services =>
				{
					services
						.AddDefaultVRCAPIOption()
						.AddScoped<IAPIHttpClient, APIHttpClient>()
						.AddScoped<Mock<HttpClientHandler>>()
						.AddScoped((s) => s.GetRequiredService<Mock<HttpClientHandler>>().Object);
					if (f is not null)
						f(services);
				});

		public virtual void Dispose() => 
			host?.Dispose();

		public UseDI(Action<IServiceCollection> f = null)
		{
			host = CreateHostBuilder(f).Build();
			options = services.GetRequiredService<IOptions<VRCAPIOptions>>();
		}
	}
}