using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using VRChatAPI.Enums;
using VRChatAPI.Objects;
using Xunit;

namespace VRChatAPI.Tests.Helper.Object{
	public class DummyObjectGeneratorTest{
		[Theory]
		[InlineData(typeof(int), default(int))]
		[InlineData(typeof(int?), default(int))]
		[InlineData(typeof(bool), default(bool))]
		[InlineData(typeof(bool?), default(bool))]
		[InlineData(typeof(EUserState), default(EUserState))]
		[InlineData(typeof(EUserState?), default(EUserState))]
		public void GenerateValues(Type type, object target){
			var generator = new DummyObjectGenerator();
			Assert.Equal(target, generator.GetDefaultObject(type));
		}

		public static object[][] CustomGeneratorTestData =>
			new object[][]{
				new object[]{typeof(string), (Func<Type, object>)(t => "OverrideTest"), "OverrideTest"},
				new object[]{typeof(DateTime), (Func<Type, object>)(t => DateTime.MinValue), DateTime.MinValue},
			};

		[Theory]
		[MemberData(nameof(CustomGeneratorTestData))]
		public void UseCustomGeneration(
			Type type, 
			Func<Type, object> def,
			object target){
			var generator = new DummyObjectGenerator(new Dictionary<Type, Func<Type, object>>{
				{type, def},
			});
			Assert.Equal(generator.GetDefaultObject(type), target);
		}

		[Fact]
		public void IDAbstractGeneration(){
			var generator = new DummyObjectGenerator();
			var v = generator.GetDefaultObject(typeof(UserID)) as UserID;
			Assert.True(v.Guid != Guid.Empty);
		}

		[Theory]
		[InlineData(typeof(IEnumerable<string>), "test")]
		[InlineData(typeof(List<string>), "test")]
		public void IEnumerableGeneration(Type t, object target){
			var generator = new DummyObjectGenerator();
			var v = generator.GetDefaultObject(t);
			v.Should().NotBeNull();
			v.As<IEnumerable<object>>().Should().NotBeEmpty()
				.And.Subject.First().Should().Be(target);
		}

		[Fact]
		public void GeneralPOCOGeneration(){
			const string test = "test";

			var generator = new DummyObjectGenerator();
			var v = generator.GetDefaultObject(typeof(LimitedUser)) as LimitedUser;
			v.Should().NotBeNull();
			v.Id.Should().NotBeNull();
			v.Id.Guid.Should().NotBeEmpty();
			v.Username.Should().Be(test);
			v.DisplayName.Should().Be(test);
			v.Bio.Should().Be(test);
			v.StatusDescription.Should().Be(test);
			v.UserIcon.Should().NotBeNull();
			v.ProfilePicOverride.Should().NotBeNull();
			v.CurrentAvatarImageUrl.Should().NotBeNull();
			v.CurrentAvatarThumbnailImageUrl.Should().NotBeNull();
			v.FallbackAvatar.Should().NotBeNull();
			v.DeveloperType.Should().NotBeNull();
			v.DeveloperType.Should().Be(default);
			v.LastPlatform.Should().NotBeNull();
			v.LastPlatform.Should().Be(default);
			v.Status.Should().NotBeNull();
			v.Status.Should().Be(default);
			v.IsFriend.Should().NotBeNull();
			v.IsFriend.Should().Be(default(bool));
			v.Location.Should().NotBeNull();
			v.Tags.Should().NotBeNull();
			v.Tags.Should().NotBeEmpty();
		}
	}
}