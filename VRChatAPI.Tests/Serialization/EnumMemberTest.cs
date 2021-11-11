using System.Runtime.Serialization;
using System.Text.Json;
using FluentAssertions;
using VRChatAPI.Tests.Helper;
using Xunit;

namespace VRChatAPI.Tests{
	public class EnumMemberTests : UseDI{
		[Fact]
		public void EnumMemberTest(){
			var s = JsonSerializer.Deserialize<string>(
				JsonSerializer.SerializeToUtf8Bytes(TestEnum.t, serializerOptions), 
				serializerOptions);
			s.Should().Be("test");
		}
		enum TestEnum{
			[EnumMember(Value="test")]
			t
		}
	}
}