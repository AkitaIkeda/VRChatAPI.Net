using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using VRChatAPI.Implementations;
using VRChatAPI.Interfaces;
using VRChatAPI.Serialization;

namespace VRChatAPI.Extentions.DependencyInjection
{
	public static class DependencyInjectionExtensions
	{
		public static IServiceCollection AddVRCAPI(
			this IServiceCollection services)
		{
			services
				.AddDefaultVRCAPIOption()
				.AddScoped<ISession, Session>()
				.AddScoped<IAPIHttpClient, APIHttpClient>()
				.AddScoped<IWSEventHandler, WSEventHandler>()
				.AddScoped(_ =>	new HttpClientHandler { UseCookies = true })
				.AddScoped<IAPIWebSocketProvider, APIWebSocketProvider>();
			return services;
		}

		public static IServiceCollection AddDefaultVRCAPIOption(this IServiceCollection services)
		{
			services.AddOptions<VRCAPIOptions>()
				.Configure(c =>
			{
				c.APIEndpointBaseAddress = "https://api.vrchat.cloud/api/1/";
				c.WSEndpoint = "wss://pipeline.vrchat.cloud/";
				c.EventHandlerBufferSize = 0x10000;
				c.StopWSHandlerOnException = true;
				c.SerializerOption = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true,
					AllowTrailingCommas = true,
					IgnoreNullValues = true,
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					Converters = 
					{
						new StringEnumConverter(),
						new ObjectAsStringConverter(),
						new NullableDateTimeConverter(),
					},
				};
			});
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
