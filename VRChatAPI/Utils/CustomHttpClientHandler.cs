using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VRChatAPI.Exceptions;

namespace VRChatAPI.Utils{
	internal class CustomHttpClientHandler : HttpClientHandler
	{
		private static ILogger Logger => Global.LoggerFactory.CreateLogger<CustomHttpClientHandler>();

		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			Logger.LogDebug("Send http request {request}", request);
			var r = await base.SendAsync(request, cancellationToken);
			if(!r.IsSuccessStatusCode)
				Logger.LogError("Response status code was not 'success' {response}", await r.Content.ReadAsStringAsync());
			if(r.StatusCode == HttpStatusCode.Unauthorized){
				throw new UnauthorizedRequestException(await r.Content.ReadAsStringAsync());
			}
			if(!r.IsSuccessStatusCode){
				throw new HttpRequestException($"Url: {request.RequestUri}, StatsCode: {r.StatusCode}, Content: {await r.Content.ReadAsStringAsync()}");
			}
			return r;
		}
	}
}