using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using FluentAssertions.Json;
using Newtonsoft.Json.Linq;
using VRChatAPI.Interfaces;
using VRChatAPI.Objects;
using VRChatAPI.Tests.Helper;
using VRChatAPI.Tests.Helper.Object;
using Xunit;

namespace VRChatAPI.Tests{
	public class ObjectSerializationTest : UseDI
	{
		private static DummyObjectGenerator d = new DummyObjectGenerator(new Dictionary<Type, Func<Type, object>>{
			{ typeof(IDAbstract), t => {
					var e = Activator.CreateInstance(t);
					t.GetProperty("guid", BindingFlags.NonPublic | BindingFlags.Instance)
						.SetValue(e, Guid.Empty);
					return e;
				}}
		});
		public static object[][] DataPears => new object[][]{
			new object[]{
				d.GetDefaultObject(typeof(CurrentUser)),
				"{\"id\":\"usr_00000000-0000-0000-0000-000000000000\",\"username\":\"test\",\"displayName\":\"test\",\"userIcon\":\"https://api.vrchat.cloud/api/1/file/file_00000000-0000-0000-0000-000000000000/0/file\",\"bio\":\"test\",\"bioLinks\":[\"test\"],\"profilePicOverride\":\"https://api.vrchat.cloud/api/1/file/file_00000000-0000-0000-0000-000000000000/0/file\",\"statusDescription\":\"test\",\"pastDisplayNames\":[{\"displayName\":\"test\",\"updated_at\":\"0001-01-01T00:00:00.000Z\"}],\"hasEmail\":false,\"hasPendingEmail\":false,\"obfuscatedEmail\":\"test\",\"obfuscatedPendingEmail\":\"test\",\"emailVerified\":false,\"hasBirthday\":false,\"unsubscribe\":false,\"statusHistory\":[\"test\"],\"statusFirstTime\":false,\"friends\":[\"usr_00000000-0000-0000-0000-000000000000\"],\"friendGroupNames\":[\"test\"],\"currentAvatarImageUrl\":\"https://api.vrchat.cloud/api/1/file/file_00000000-0000-0000-0000-000000000000/0/file\",\"currentAvatarThumbnailImageUrl\":\"test\",\"currentAvatar\":\"avtr_00000000-0000-0000-0000-000000000000\",\"currentAvatarAssetUrl\":\"https://api.vrchat.cloud/api/1/file/file_00000000-0000-0000-0000-000000000000/0/file\",\"fallbackAvatar\":\"avtr_00000000-0000-0000-0000-000000000000\",\"accountDeletionDate\":\"0001-01-01T00:00:00.000Z\",\"acceptedTOSVersion\":0,\"steamId\":\"test\",\"steamDetails\":{},\"oculusId\":\"test\",\"hasLoggedInFromClient\":false,\"homeLocation\":\"wrld_00000000-0000-0000-0000-000000000000\",\"twoFactorAuthEnabled\":false,\"state\":\"offline\",\"tags\":[\"test\"],\"developerType\":\"none\",\"last_login\":\"0001-01-01T00:00:00.000Z\",\"last_platform\":\"standalonewindows\",\"allowAvatarCopying\":false,\"status\":\"offline\",\"date_joined\":\"0001-01-01T00:00:00.000Z\",\"isFriend\":false,\"friendKey\":\"test\",\"onlineFriends\":[\"usr_00000000-0000-0000-0000-000000000000\"],\"activeFriends\":[\"usr_00000000-0000-0000-0000-000000000000\"],\"offlineFriends\":[\"usr_00000000-0000-0000-0000-000000000000\"],\"worldId\":\"test\",\"instanceId\":\"test\",\"location\":\"test\"}"
			},
		};

		[Theory]
		[MemberData(nameof(DataPears))]
		public void ObjectDeserialize(object target, string s){
			var o = JsonSerializer.Deserialize(s, target.GetType(), serializerOptions);
			o.Should().BeEquivalentTo(target, o => 
				o.ComparingByMembers<JsonElement>());
		}
		[Theory]
		[MemberData(nameof(DataPears))]
		public void ObjectSerialize(object obj, string target){
			var s = JsonSerializer.Serialize(obj, serializerOptions);
			JToken.Parse(s)
				.Should().BeEquivalentTo(JToken.Parse(target))
				.And.NotBeEquivalentTo(JToken.Parse("{}"));
		}

		public static object[][] VRCObjectData => new object[][]{
			new object[]{
				typeof(World),
				"{\"id\":\"wrld_4432ea9b-729c-46e3-8eaf-846aa0a37fdd\",\"name\":\"VRChat Home\",\"description\":\"VRChat Home\",\"featured\":false,\"authorId\":\"8JoV9XEdpo\",\"authorName\":\"vrchat\",\"capacity\":8,\"tags\":[\"system_updated_recently\",\"admin_internal_world\",\"system_approved\",\"admin_approved\"],\"releaseStatus\":\"public\",\"imageUrl\":\"https://api.vrchat.cloud/api/1/file/file_c2b7ad18-5276-4e9b-aef7-8e18555e5030/6/file\",\"thumbnailImageUrl\":\"https://api.vrchat.cloud/api/1/image/file_c2b7ad18-5276-4e9b-aef7-8e18555e5030/6/256\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/111/file\",\"unityPackageUrlObject\":{},\"namespace\":\"\",\"unityPackages\":[{\"id\":\"unp_5bf9073e-0186-4522-a8c1-6d79b15e89f1\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/43/file\",\"assetUrlObject\":{},\"pluginUrl\":\"https://api.vrchat.cloud/api/1/file/file_14b04cc5-5bd8-45c5-8edc-1c34b75e712c/29/file\",\"pluginUrlObject\":{},\"unityVersion\":\"2017.4.15f1\",\"unitySortNumber\":20170415000,\"assetVersion\":3,\"platform\":\"android\",\"created_at\":\"2019-05-15T18:51:39.085Z\"},{\"id\":\"unp_7f196eac-3362-45f7-8dba-4dac15c25804\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/44/file\",\"assetUrlObject\":{},\"pluginUrl\":\"https://api.vrchat.cloud/api/1/file/file_9c0556cf-7df8-4b0e-9781-739cc13804fd/13/file\",\"pluginUrlObject\":{},\"unityVersion\":\"2017.4.15f1\",\"unitySortNumber\":20170415000,\"assetVersion\":3,\"platform\":\"standalonewindows\",\"created_at\":\"2019-05-15T19:01:02.030Z\"},{\"id\":\"unp_4564bc5d-ae80-4e49-9a93-74364d97e009\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/53/file\",\"assetUrlObject\":{},\"pluginUrl\":\"https://api.vrchat.cloud/api/1/file/file_14b04cc5-5bd8-45c5-8edc-1c34b75e712c/34/file\",\"pluginUrlObject\":{},\"unityVersion\":\"2017.4.28f1\",\"unitySortNumber\":20170428000,\"assetVersion\":3,\"platform\":\"android\",\"created_at\":\"2019-11-16T00:41:51.628Z\"},{\"id\":\"unp_65c3d4bf-7291-4646-88f6-a6bd32e68b57\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/52/file\",\"assetUrlObject\":{},\"pluginUrl\":\"https://api.vrchat.cloud/api/1/file/file_9c0556cf-7df8-4b0e-9781-739cc13804fd/17/file\",\"pluginUrlObject\":{},\"unityVersion\":\"2017.4.28f1\",\"unitySortNumber\":20170428000,\"assetVersion\":3,\"platform\":\"standalonewindows\",\"created_at\":\"2019-11-08T01:49:10.213Z\"},{\"id\":\"unp_e3494e56-fad4-4d5b-9342-f90776cb1403\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/56/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2018.4.12f1\",\"unitySortNumber\":20180412000,\"assetVersion\":4,\"platform\":\"android\",\"created_at\":\"2019-12-08T00:16:28.349Z\"},{\"id\":\"unp_aee8f76f-85db-4148-b2b7-bfd16a515c79\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/55/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2018.4.12f1\",\"unitySortNumber\":20180412000,\"assetVersion\":4,\"platform\":\"standalonewindows\",\"created_at\":\"2019-12-07T23:41:04.986Z\"},{\"id\":\"unp_0773a931-183e-42a6-b6e6-cc9bce0546a6\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/100/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2018.4.14f1\",\"unitySortNumber\":20180414000,\"assetVersion\":4,\"platform\":\"android\",\"created_at\":\"2021-08-19T02:56:51.138Z\"},{\"id\":\"unp_f7986549-a591-4e75-b966-3178f1b019f8\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/101/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2018.4.14f1\",\"unitySortNumber\":20180414000,\"assetVersion\":4,\"platform\":\"standalonewindows\",\"created_at\":\"2021-08-19T03:08:50.302Z\"},{\"id\":\"unp_7aa9f5e4-0691-4c93-b727-4c210c5dd77a\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/103/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2019.4.29f1\",\"unitySortNumber\":20190429000,\"assetVersion\":4,\"platform\":\"android\",\"created_at\":\"2021-09-10T07:40:37.786Z\"},{\"id\":\"unp_dba5a7cc-ecf0-4117-87e7-5f2a4c26cc0c\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/105/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2019.4.29f1\",\"unitySortNumber\":20190429000,\"assetVersion\":4,\"platform\":\"standalonewindows\",\"created_at\":\"2021-09-10T08:22:13.049Z\"},{\"id\":\"unp_36dbdcd3-c00a-4620-bc78-97dea961c955\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/110/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2019.4.30f1\",\"unitySortNumber\":20190430000,\"assetVersion\":4,\"platform\":\"android\",\"created_at\":\"2021-11-05T17:32:49.384Z\"},{\"id\":\"unp_455b962d-372a-4045-84c6-79537a6b3601\",\"assetUrl\":\"https://api.vrchat.cloud/api/1/file/file_3caaf07f-363e-4b84-89d1-ee827f12afb5/111/file\",\"assetUrlObject\":{},\"pluginUrl\":\"\",\"pluginUrlObject\":{},\"unityVersion\":\"2019.4.30f1\",\"unitySortNumber\":20190430000,\"assetVersion\":4,\"platform\":\"standalonewindows\",\"created_at\":\"2021-11-05T17:40:39.485Z\"}],\"version\":166,\"organization\":\"vrchat\",\"previewYoutubeId\":null,\"favorites\":32995,\"created_at\":\"2019-02-22T17:33:03.865Z\",\"updated_at\":\"2021-11-05T17:40:57.771Z\",\"publicationDate\":\"2021-11-05T17:41:49.966Z\",\"labsPublicationDate\":\"none\",\"visits\":105935731,\"popularity\":9,\"heat\":7,\"publicOccupants\":41,\"privateOccupants\":1049,\"occupants\":1090,\"instances\":[[\"1337\",7],[\"22384\",7],[\"25594\",6],[\"56911~region(eu)\",2],[\"09066\",1]]}"
			},
			new object[]{
				typeof(Avatar),
				"{\"id\":\"avtr_c38a1615-5bf5-42b4-84eb-a8b6c37cbd11\",\"name\":\"Robot\",\"description\":\"Beep Boop\",\"authorId\":\"8JoV9XEdpo\",\"authorName\":\"vrchat\",\"tags\":[\"admin_featured_quest\"],\"imageUrl\":\"https://api.vrchat.cloud/api/1/file/file_0e8c4e32-7444-44ea-ade4-313c010d4bae/1/file\",\"thumbnailImageUrl\":\"https://api.vrchat.cloud/api/1/image/file_0e8c4e32-7444-44ea-ade4-313c010d4bae/1/256\",\"releaseStatus\":\"public\",\"version\":13,\"featured\":true,\"unityPackages\":[{\"id\":\"unp_78931757-c699-4ca5-b70e-e8bd4d6d1e9b\",\"unityVersion\":\"5.6.3p1\",\"assetVersion\":1,\"platform\":\"standalonewindows\",\"created_at\":\"2018-01-26T16:00:23.586Z\"},{\"id\":\"unp_79d11a9b-cc69-45a5-a63c-7e0c5489db1a\",\"unityVersion\":\"2017.4.15f1\",\"assetVersion\":1,\"platform\":\"android\",\"created_at\":\"2019-05-09T20:20:35.354Z\"},{\"id\":\"unp_8b767c59-d0a0-4a7c-bce4-aeb3884255f6\",\"unityVersion\":\"2017.4.15f1\",\"assetVersion\":1,\"platform\":\"standalonewindows\",\"created_at\":\"2019-05-09T20:22:14.724Z\"},{\"id\":\"unp_1a11a20c-0446-4820-a231-625f0362db1c\",\"unityVersion\":\"2018.4.17f1\",\"assetVersion\":1,\"platform\":\"android\",\"created_at\":\"2020-10-01T19:47:27.538Z\"}],\"unityPackageUrl\":\"\",\"unityPackageUrlObject\":{},\"created_at\":\"2016-11-30T03:47:35.000Z\",\"updated_at\":\"2020-10-01T21:06:25.403Z\"}"
			},
		};

		[Theory]
		[MemberData(nameof(VRCObjectData))]
		public void SerializeRealObject(Type t, string s){
			var o = JsonSerializer.Deserialize(s, t, serializerOptions) as SerializableObjectAbstract;
			o.Should().NotBeNull();
			o.ExtensionData.Should().BeNull();
		}
	}
}