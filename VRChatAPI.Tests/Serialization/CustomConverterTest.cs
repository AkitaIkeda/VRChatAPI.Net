using System.Linq;
using System.Text.Json;
using VRChatAPI.Objects;
using VRChatAPI.Tests.Helper.Object;
using Xunit;
using FluentAssertions;
using VRChatAPI.Tests.Helper;
using System;
using VRChatAPI.Interfaces;
using VRChatAPI.Enums;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace VRChatAPI.Tests
{
	public class CustomConverterTest : UseDI
	{
		private static DummyObjectGenerator generator = new DummyObjectGenerator();
		public CustomConverterTest() : base(){
		}

		public static object[][] ResponseMessageTestObjects => new object[][]
		{
			new object[]
			{ 
				new ResponseMessage { 
					MessageType = Enums.EResponseType.success, 
					StatusCode = 200, 
					Message = "success test", 
				},
				@"{""success"":{""message"":""success test"",""status_code"":200}}",
			},
			new object[]
			{ 
				new ResponseMessage { 
					MessageType = Enums.EResponseType.error, 
					StatusCode = 401, 
					Message = "error test", 
				},
				@"{""error"":{""message"":""error test"",""status_code"":401}}",
			},
		};

		private static object[] GenerateParsableTestObjects<T>() where T : IParsable =>
			CreateObjectStringPair(generator.GetDefaultObject(typeof(T)));
		private static object[] CreateObjectStringPair(object o) => new object[] { o, $"\"{o.ToString()}\"" };
		public static object[][] ParsableTestObjects => new object[][]{
			GenerateParsableTestObjects<UserID>(),
			GenerateParsableTestObjects<WorldID>(),
			GenerateParsableTestObjects<VRCFilePath>(),
			GenerateParsableTestObjects<VRCImagePath>(),
		};

		public static object[][] SameObject => new object[][]{new object[]{
			JsonSerializer.Deserialize<JsonElement>(@"{""test"":""test""}"),
			@"{""test"":""test""}",
		}};

		public static object[][] InstanceIDTestObject => new object[][]{
			CreateObjectStringPair(new InstanceID{ Name = "test"}),
			CreateObjectStringPair(new InstanceID{ 
				Name = "test", 
				Type = EInstanceType.Friends, 
				Owner = generator.GetDefaultObject(typeof(UserID)) as UserID,
				Region = ERegion.Japan,
				Nonce = Guid.NewGuid(),
			}),
			CreateObjectStringPair(new InstanceID{ 
				Name = "test", 
				Type = EInstanceType.InvitePlus, 
				Owner = generator.GetDefaultObject(typeof(UserID)) as UserID,
				Region = ERegion.Europe,
				Nonce = Guid.NewGuid(),
			}),
		};

		public static object[][] OtherTestObjects => new object[][] {
			new object[] {
				new InstanceInfo{
					InstanceID = new InstanceID{ Name = "test"},
				},
				"[\"test\",0]",
			}, 
		};

		[Theory]
		[MemberData(nameof(SameObject))]
		[MemberData(nameof(ResponseMessageTestObjects))]
		[MemberData(nameof(ParsableTestObjects))]
		[MemberData(nameof(InstanceIDTestObject))]
		[MemberData(nameof(OtherTestObjects))]
		public void SerializationTest(object resp, string target)
		{
			var s = JsonSerializer.Serialize(resp, serializerOptions);
			s.Should().Be(target);
		}

		[Theory]
		[MemberData(nameof(SameObject))]
		[MemberData(nameof(ResponseMessageTestObjects))]
		[MemberData(nameof(ParsableTestObjects))]
		[MemberData(nameof(InstanceIDTestObject))]
		[MemberData(nameof(OtherTestObjects))]
		public void DeserializationTest(object target, string obj){
			var o = JsonSerializer.Deserialize(obj, target.GetType(), serializerOptions);
			o.Should().BeEquivalentTo(target,
				v => v.ComparingByMembers(target.GetType()));
		}

		[Fact]
		public void NullableDateTimeTest(){

			var n = DateTime.UtcNow;
			string json(string v) => $"{{\"d\":\"{v}\"}}";
			string t = json(n.ToString("s"));
			var o = JsonSerializer.Deserialize<test>(t, serializerOptions);
			o.d.Should().NotBeNull().And.BeCloseTo(n, TimeSpan.FromSeconds(1));
			JsonSerializer.Serialize(o).Should().Be(t);
			o = JsonSerializer.Deserialize<test>(json("none"), serializerOptions);
			o.d.Should().BeNull();
		}
		private class test{
			public DateTime? d { get; set; }
		}
	}
}
