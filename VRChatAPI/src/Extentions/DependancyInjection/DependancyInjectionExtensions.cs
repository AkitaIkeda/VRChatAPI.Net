using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Implementations;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Extentions.DependancyInjection
{
	public static class DependancyInjectionExtensions
	{
		public static IServiceCollection AddVRCAPI(
			this IServiceCollection services)
		{
			services.AddOptions<VRCAPIOptions>()
				.Configure(c =>
			{
				c.APIEndpointBaseAddress = "https://vrchat.com/api/1/";
				c.WSEndpoint = "wss://pipeline.vrchat.cloud/";
				c.EventHandlerBufferSize = 0x10000;
				c.StopWSHandlerOnException = true;
				c.SerializerOption = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					Converters = 
					{
						new JsonStringEnumConverter()
					}
				};
			});
			services.AddSingleton<ISessionFactory, SessionFactory>();
			services.AddScoped<IAPIHttpClient, APIHttpClient>();
			services.AddScoped<IWSEventHandler, WSEventHandler>();
			return services;
		}

		public static IServiceCollection AddVRCAPI(
			this IServiceCollection services,
			Action<VRCAPIOptions> configureOptions)
		{
			services.PostConfigure(configureOptions);
			return services.AddVRCAPI();
		}
	}
}
