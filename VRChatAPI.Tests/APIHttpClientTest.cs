using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VRChatAPI.APIParams;
using VRChatAPI.Enums;
using VRChatAPI.Implementations;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Tests.Helper;
using VRChatAPI.Tests.Helper.Mock;
using VRChatAPI.Utils;
using Xunit;

namespace VRChatAPI.Tests
{
	public class APIHttpClientTest : UseDI
	{
		private readonly Mock<HttpClientHandler> mock;
		private readonly IAPIHttpClient client;

		public APIHttpClientTest() : base()
		{
			mock = services.GetRequiredService<Mock<HttpClientHandler>>();
			client = services.GetRequiredService<IAPIHttpClient>();
		}

		[Fact]
		public void GetCredentialTest()
		{
			CookieContainer cc = new CookieContainer();
			var cl = new HttpClientHandler{
				CookieContainer = cc,
			};
			var client = new APIHttpClient(options, null, cl);
			cc.Add(new Cookie("auth", "test", "/", "api.vrchat.cloud"));
			var cred = client.GetCredential();
			cred.AuthToken.Should().NotBeNull().And.Be("test");
			cred.TFAToken.Should().BeNull();
			cc.Add(new Cookie("twoFactorAuth", "test", "/", "api.vrchat.cloud"));
			cred = client.GetCredential();
			cred.AuthToken.Should().NotBeNull().And.Be("test");
			cred.TFAToken.Should().NotBeNull().And.Be("test");
		}

		[Fact]
		public async Task RequestTestAsync(){
			ResponseMessage value = new ResponseMessage
			{
				Message = "test",
				StatusCode = (int)HttpStatusCode.OK,
				MessageType = EResponseType.success,
			};
			var s = JsonSerializer.Serialize(value, serializerOptions);
			mock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(), 
				ItExpr.IsAny<CancellationToken>())
				.Returns<HttpRequestMessage, CancellationToken>((req, _) => Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					RequestMessage = req,
					Content = new StringContent(s),
				}));
			
			(await client.Get("https://example.com", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
			(await client.Delete("https://example.com", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
			(await client.Post("https://example.com", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
			(await client.Put("https://example.com", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
			(await client.Post("https://example.com", "test", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
			(await client.Put("https://example.com", "test", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
			(await client.Get<ResponseMessage>("https://example.com", default)).Should().BeEquivalentTo(value);
			(await client.Delete<ResponseMessage>("https://example.com", default)).Should().BeEquivalentTo(value);
			(await client.Post<ResponseMessage>("https://example.com", default)).Should().BeEquivalentTo(value);
			(await client.Put<ResponseMessage>("https://example.com", default)).Should().BeEquivalentTo(value);
			(await client.Post<ResponseMessage, string>("https://example.com", "test", default)).Should().BeEquivalentTo(value);
			(await client.Put<ResponseMessage, string>("https://example.com", "test", default)).Should().BeEquivalentTo(value);
			(await client.Get("https://example.com", default)).Content.ReadAsStringAsync().Result.Should().Be(s);
		}

		[Fact]
		public void QueryConstructorTest(){
			QueryConstructor.MakeQuery(new VRCFileSearchParams(), serializerOptions).Should().BeNullOrEmpty();
			QueryConstructor.MakeQuery(new VRCFileSearchParams(){Tag="test"}, serializerOptions).Should().Be("tag=test");
		}
	}
}