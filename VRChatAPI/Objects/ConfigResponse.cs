using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VRChatAPI.Utils;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
	public enum DeploymentGroup
	{
		/// <summary>
		/// Production
		/// </summary>
		blue,
		/// <summary>
		/// Production
		/// </summary>
		green,
		/// <summary>
		/// Development
		/// </summary>
		grape,
		/// <summary>
		/// Development
		/// </summary>
		cherry,
	}

	public enum WorldRowPlatformEnum
	{
		ThisPlatformSupported,
		any,
		AllPlatforms,
		ThisPlatformOnly,
	}

	public class DynamicWorldRow
	{
		/// <summary>
		/// Name of row
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Sort Heading
		/// </summary>
		public WorldSortOptions sortHeading { get; set; }
		/// <summary>
		/// Ownership filter
		/// </summary>
		public string sortOwnership { get; set; }
		/// <summary>
		/// Sort Order
		/// </summary>
		public OrderOptions sortOrder { get; set; }
		/// <summary>
		/// Supported Platform
		/// </summary>
		public WorldRowPlatformEnum platform { get; set; }
		/// <summary>
		/// Index of row
		/// </summary>
		public int index { get; set; }
		/// <summary>
		/// Tag filter
		/// </summary>
		public string tag { get; set; }

		/// <summary>
		/// Get worlds
		/// </summary>
		/// <returns>List of Limited World object</returns>
		public AsyncSequentialReader<LimitedWorld> GetWorlds(PlatformEnum currentPlatform)
		{
			var api = new Endpoints.WorldAPI();
			return api.SearchSequential(
				sort: sortHeading, 
				tags: tag, 
				platform: currentPlatform, 
				order: sortOrder, 
				user: (sortOwnership.ToLowerInvariant() == "mine") ? (UserOptions?)UserOptions.me : null);
		}
	}

	public class Announcement
	{
		/// <summary>
		/// Announcement Name
		/// </summary>
		public string name { get; set; }
		/// <summary>
		/// Announcement Body
		/// </summary>
		public string text { get; set; }
	}


	public class DownloadUrls
	{
		public string sdk2 { get; set; }
		[JsonProperty("sdk3-worlds")]
		public string sdk3_worlds { get; set; }
		[JsonProperty("sdk3-avatars")]
		public string sdk3_avatars { get; set; }
	}
	
	public class Events
	{
		public int distanceClose { get; set; }
		public int distanceFactor { get; set; }
		public int distanceFar { get; set; }
		public int groupDistance { get; set; }
		public int maximumBunchSize { get; set; }
		public int notVisibleFactor { get; set; }
		public int playerOrderBucketSize { get; set; }
		public int playerOrderFactor { get; set; }
		public int slowUpdateFactorThreshold { get; set; }
		public int viewSegmentLength { get; set; }
	}
	

	/// <summary>
	/// Response of Get /config
	/// For more information, see <see href="https://vrchatapi.github.io/docs/api#get-/config" />
	/// </summary>
	public class ConfigResponse
	{
		/// <summary>
		/// Address of the VRChat's office
		/// </summary>
		public string address { get; set; }
		/// <summary>
		/// Announcements from VRChat
		/// </summary>
		public List<Announcement> announcements { get; set; }
		/// <summary>
		/// ApiKey that is needed to call api
		/// </summary>
		/// <remarks>this value is the same as <see cref="clientApiKey"/></remarks>
		public string apiKey { get; set; }
		public string appName { get; set; }
		public string buildVersionTag { get; set; }
		/// <summary>
		/// ApiKey that is needed to call api
		/// </summary>
		/// <remarks>this value is the same as <see cref="apiKey"/></remarks>
		public string clientApiKey { get; set; }
		public int clientBPSCeiling { get; set; }
		public int clientDisconnectTimeout { get; set; }
		public int clientReserverPlayerBPS { get; set; }
		public int clientSentCountAllowance { get; set; }
		/// <summary>
		/// VRChat's contact email
		/// </summary>
		public string contactEmail { get; set; }
		/// <summary>
		/// VRChat's email for copyright issues
		/// </summary>
		public string copyrightEmail { get; set; }
		/// <summary>
		/// Current version of Term of Service
		/// </summary>
		public int currentTOSVersion { get; set; }
		/// <summary>
		/// Default Avatar's Id
		/// </summary>
		/// <value></value>
		public AvatarId defaultAvatar { get; set; }
		/// <summary>
		/// Used to identify which API deployment cluster is currently responding.
		/// </summary>
		public DeploymentGroup deploymentGroup { get; set; }
		public string devAppVersionStandalone { get; set; }
		public string devDownloadLinkWindows { get; set; }
		/// <summary>
		/// Download link of Sdk
		/// </summary>
		public string devSdkUrl { get; set; }
		public string devSdkVersion { get; set; }
		public string devServerVersionStandalone { get; set; }

		#region disables
		public bool disableAvatarCopying { get; set; }
		public bool disableAvatarGating { get; set; }
		public bool disableCommunityLabs { get; set; }
		public bool disableCommunityLabsPromotion { get; set; }
		public bool disableEmail { get; set; }
		public bool disableEventStream { get; set; }
		public bool disableFeedbackGating { get; set; }
		public bool disableHello { get; set; }
		public bool disableRegistration { get; set; }
		public bool disableSteamNetworking { get; set; }
		public bool disableTwoFactorAuth { get; set; }
		public bool disableUdon { get; set; }
		public bool disableUpgradeAccount { get; set; }
		#endregion

		public string downloadLinkWindows { get; set; }
		/// <summary>
		/// Direct Links of SDK
		/// </summary>
		public DownloadUrls downloadUrls { get; set; }
		/// <summary>
		/// Rows to display in the "world" tab in game menu
		/// </summary>
		public List<DynamicWorldRow> dynamicWorldRows { get; set; }
		public Events events { get; set; }
		public string gearDemoRoomId { get; set; }
		/// <summary>
		/// Redirect Url of VRChat website
		/// </summary>
		public string homepageRedirectTarget { get; set; }
		/// <summary>
		/// Home world Id
		/// </summary>
		public WorldId homeWorldId { get; set; }
		/// <summary>
		/// Hub World Id
		/// </summary>
		public WorldId hubWorldId { get; set; }
		/// <summary>
		/// VRChat's job application email
		/// </summary>
		public string jobsEmail { get; set; }
		/// <summary>
		/// Daily message from VRChat
		/// </summary>
		/// <remarks>This message seems to have never changed</remarks>
		public string messageOfTheDay { get; set; }
		/// <summary>
		/// VRChat's moderation related email
		/// </summary>
		public string moderationEmail { get; set; }
		public int moderationQueryPeriod { get; set; }
		public string notAllowedToSelectAvatarInPrivateWorldMessage { get; set; }
		public string plugin { get; set; }
		public string releaseAppVersionStandalone { get; set; }
		public string releaseSdkUrl { get; set; }
		public string releaseSdkVersion { get; set; }
		public string releaseServerVersionStandalone { get; set; }
		public string sdkDeveloperFaqUrl { get; set; }
		/// <summary>
		/// Discord invitation Url
		/// </summary>
		public string sdkDiscordUrl { get; set; }
		public string sdkNotAllowedToPublishMessage { get; set; }
		/// <summary>
		/// Unity Version for VRChat dev
		/// </summary>
		public string sdkUnityVersion { get; set; }
		public string serverName { get; set; }
		/// <summary>
		/// VRChat's support email
		/// </summary>
		public string supportEmail { get; set; }
		/// <summary>
		/// Timeout World Id
		/// </summary>
		public WorldId timeOutWorldId { get; set; }
		/// <summary>
		/// Tutorial World Id
		/// </summary>
		public WorldId tutorialWorldId { get; set; }
		public int updateRateMsMaximum { get; set; }
		public int updateRateMsMinimum { get; set; }
		public int updateRateMsNormal { get; set; }
		public int updateRateMsUdonManual { get; set; }
		public int uploadAnalysisPercent { get; set; }
		public List<string> urlList { get; set; }
		public bool useReliableUdpForVoice { get; set; }
		public int userUpdatePeriod { get; set; }
		public int userVerificationDelay { get; set; }
		public int userVerificationRetry { get; set; }
		public int userVerificationTimeout { get; set; }
		public string viveWindowsUrl { get; set; }
		public List<string> whiteListedAssetUrls { get; set; }
		public int worldUpdatePeriod { get; set; }
		[JsonProperty("youtubedl-hash")]
		public string youtubedl_hash { get; set; }
		[JsonProperty("youtubedl-version")]
		public DateTime? youtubedl_version { get; set; }

		public bool? sdkEnableDeltaCompression { get; set; }
	}
}
