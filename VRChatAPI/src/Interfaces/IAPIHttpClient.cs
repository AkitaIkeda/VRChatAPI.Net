using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.Objects;

namespace VRChatAPI.Interfaces
{
	public interface IAPIHttpClient
	{
		Task<HttpResponseMessage> Get(string url, CancellationToken ct);
		Task<HttpResponseMessage> Delete(string url, CancellationToken ct);
		Task<HttpResponseMessage> Post(string url, CancellationToken ct);
		Task<HttpResponseMessage> Put(string url, CancellationToken ct);
		Task<HttpResponseMessage> Post<TContent>(string url, TContent content, CancellationToken ct);
		Task<HttpResponseMessage> Put<TContent>(string url, TContent content, CancellationToken ct);

		Task<TResult> Get<TResult>(string url, CancellationToken ct);
		Task<TResult> Delete<TResult>(string url, CancellationToken ct);
		Task<TResult> Post<TResult>(string url, CancellationToken ct);
		Task<TResult> Put<TResult>(string url, CancellationToken ct);
		Task<TResult> Post<TResult, TContent>(string url, TContent content, CancellationToken ct);
		Task<TResult> Put<TResult, TContent>(string url, TContent content, CancellationToken ct);
		Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken ct);
		ITokenCredential GetCredential();
		HttpClient Client { get; }
		event EventHandler<ResponseMessage> OnRequestFailedWithResponseMessage;
		event EventHandler<HttpResponseMessage> OnRequestFailed;
		void AddCookie(Cookie cookie);
		void AddCookie(CookieCollection collection);
	}
}