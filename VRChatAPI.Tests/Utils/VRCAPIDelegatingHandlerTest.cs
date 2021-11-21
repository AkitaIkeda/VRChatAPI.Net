using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.Protected;
using VRChatAPI.Enums;
using VRChatAPI.Objects;
using VRChatAPI.Tests.Helper;
using VRChatAPI.Tests.Helper.Mock;
using VRChatAPI.Utils;
using Xunit;

namespace VRChatAPI.Tests{
	public class VRCAPIDelegatingHandlerTest : UseDI
	{
		[Fact]
		public async Task ThrowExceptionWhenNotSuccessWithErrorResponse(){
			ResponseMessage value = new ResponseMessage
			{
				Message = "test",
				StatusCode = (int)HttpStatusCode.Forbidden,
				MessageType = EResponseType.error,
			};
			string content = JsonSerializer.Serialize(value, serializerOptions);
			var m = HttpMessageHandlerMock.Create((req, _) =>
			{
				return Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.Forbidden,
					RequestMessage = req,
					Content = new StringContent(content)
				});
			});
			var d = new HttpClient(new VRCAPIDelegatingHandler(m.Object));
			var t = await d.Invoking(v => v.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com")))
				.Should().ThrowAsync<Exceptions.VRCAPIRequestException>().WithMessage(content);
			t.Which.Data.Should()
				.Match(v => v.As<IDictionary>().Contains("ErrorMessage"))
				.And
				.Subject.As<IDictionary>()["ErrorMessage"]
				.Should().BeEquivalentTo(value);
		}
		[Fact]
		public async Task ThrowExceptionWhenNotSuccessWithoutErrorResponse(){
			var m = HttpMessageHandlerMock.Create((req, _) =>
			{
				return Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.Forbidden,
					RequestMessage = req,
					Content = new StringContent("test")
				});
			});
			var d = new HttpClient(new VRCAPIDelegatingHandler(m.Object));
			var t = await d.Invoking(v => v.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com")))
				.Should().ThrowAsync<Exceptions.VRCAPIRequestException>().WithMessage("test");
		}
		
		[Fact]
		public async Task DoesPassSuccessResponse(){
			var m = HttpMessageHandlerMock.Create((req, _) => Task.FromResult(new HttpResponseMessage{
				StatusCode = HttpStatusCode.OK,
				RequestMessage = req,
				Content = new StringContent("test")
			}));
			var d = new HttpClient(new VRCAPIDelegatingHandler(m.Object));
			var t = await d.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com"));
			(await t.Content.ReadAsStringAsync()).Should().Be("test");
		}
	} 
}