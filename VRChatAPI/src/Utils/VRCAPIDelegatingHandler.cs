using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Exceptions;
using VRChatAPI.Objects;

namespace VRChatAPI.Utils
{
	internal class VRCAPIDelegatingHandler : DelegatingHandler
	{
		public event EventHandler<ResponseMessage> OnRequestFailedWithResponseMessage;
		public event EventHandler<HttpResponseMessage> OnRequestFailed; 
		public VRCAPIDelegatingHandler() : base() { }
		public VRCAPIDelegatingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var r = await base.SendAsync(request, cancellationToken);
			if (!r.IsSuccessStatusCode)
			{
				OnRequestFailed?.Invoke(this, r);
				VRCAPIRequestException ex = new VRCAPIRequestException(r);
				if(ex.Data.Contains("ErrorMessage"))
					OnRequestFailedWithResponseMessage?.Invoke(this, ex.Data["ErrorMessage"] as ResponseMessage);
				throw ex;
			}

			return r;
		}
	}
}
