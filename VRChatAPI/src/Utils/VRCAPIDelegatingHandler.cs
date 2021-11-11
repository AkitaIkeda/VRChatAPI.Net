using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VRChatAPI.Utils
{
	internal class VRCAPIDelegatingHandler : DelegatingHandler
	{
		public VRCAPIDelegatingHandler() : base() { }
		public VRCAPIDelegatingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var r = await base.SendAsync(request, cancellationToken);
			if (!r.IsSuccessStatusCode)
				throw new VRCAPIRequestException(r);
			return r;
		}
	}
}
