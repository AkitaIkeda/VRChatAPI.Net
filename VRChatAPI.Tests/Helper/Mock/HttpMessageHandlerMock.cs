using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace VRChatAPI.Tests.Helper.Mock{
	public static class HttpMessageHandlerMock{
		public static Mock<HttpClientHandler> Create(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> resp){
			var mock = new Mock<HttpClientHandler>();
			mock.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.Returns(resp);
			return mock;
		}
		public static Mock<HttpClientHandler> Create(HttpResponseMessage resp) =>
			Create((_, _) => Task.FromResult(resp));
	}
}