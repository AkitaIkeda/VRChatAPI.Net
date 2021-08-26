using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace VRChatAPI
{
    static class Global
    {
        internal static HttpClient httpClient;
        internal static HttpClientHandler httpClientHandler;
		internal static ILoggerFactory LoggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(
            builder => {
            }
        );

		internal static CookieContainer CookieContainer => httpClientHandler.CookieContainer;
    }
}
