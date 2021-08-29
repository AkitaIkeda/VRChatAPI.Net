using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using VRChatAPI.Objects;

namespace VRChatAPI
{
    static class Global
    {
		internal static readonly string APIUrl = "https://api.vrchat.cloud/api/1/";
		internal static HttpClient httpClient;
		internal static ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
            builder => {
            }
        );

		internal static ConfigResponse RemoteConfig;
    }
}
