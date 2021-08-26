using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Exceptions;

namespace VRChatAPI.Utils{
	internal class CustomHttpClientHandler : HttpClientHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var r = await base.SendAsync(request, cancellationToken);
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