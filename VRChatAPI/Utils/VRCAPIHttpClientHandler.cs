using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VRChatAPI.Exceptions;

namespace VRChatAPI.Utils{
	public class VRCAPIHttpClientHandler : HttpClientHandler
	{
		protected static ILogger Logger => Global.LoggerFactory.CreateLogger<VRCAPIHttpClientHandler>();

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