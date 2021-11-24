using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moq.Protected;
using VRChatAPI.Implementations;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Tests.Helper.Object;
using Xunit;

namespace VRChatAPI.Tests{
	public class CredentialTest : Helper.UseDI
	{
		private readonly Mock<HttpClientHandler> mock;
		private readonly IAPIHttpClient client;
		private readonly DummyObjectGenerator generator;

		public CredentialTest() : base()
		{
			mock = services.GetRequiredService<Mock<HttpClientHandler>>();
			client = services.GetRequiredService<IAPIHttpClient>();
			generator = new DummyObjectGenerator();
		}

		[Fact]
		public async Task TokenCredentialTest()
		{
			await LoginTest(() => new TokenCredential("test"), r => true);
			mock.Object.CookieContainer.GetCookies(new Uri(options.Value.APIEndpointBaseAddress)).Should().Contain(c => 
				c.Name == "auth" && c.Value == "test");
		}

		[Fact]
		public async Task BasicAuthCredentialTest(){
			await LoginTest(() => new BasicAuthCredential("test", "pass"), r=> 
				r.Headers.Authorization.Scheme == "Basic"
				&& r.Headers.Authorization.Parameter == "dGVzdDpwYXNz");
		}

		private async Task LoginTest(Func<ICredential> cred, System.Linq.Expressions.Expression<Func<HttpRequestMessage, bool>>  f)
		{
			mock.CallBase = true;
			var setup = mock.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",
							ItExpr.Is<HttpRequestMessage>(f),
							ItExpr.IsAny<CancellationToken>());
			setup.Returns<HttpRequestMessage, CancellationToken>((req, _) =>
				Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					RequestMessage = req,
					Content = new StringContent(@"{""requiresTwoFactorAuth"":[""totp"",""otp""]}"),
				}));
			
			var r = await cred().Login(client, serializerOptions);
			r.TFARequired.Should().BeTrue();

			var cu = generator.GetDefaultObject(typeof(CurrentUser));
			setup.Returns<HttpRequestMessage, CancellationToken>((req, _) =>
				Task.FromResult(new HttpResponseMessage
				{
					StatusCode = HttpStatusCode.OK,
					RequestMessage = req,
					Content = new StringContent(
						JsonSerializer.Serialize(
							cu, serializerOptions)),
				}));
			
			r = await cred().Login(client, serializerOptions);
			r.TFARequired.Should().BeFalse();
			r.User.Should().NotBeNull().And.BeEquivalentTo(cu, 
				c => c.ComparingByMembers<JsonElement>());
		}
	}
}