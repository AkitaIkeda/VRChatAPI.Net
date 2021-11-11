using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using VRChatAPI.Enums;
using VRChatAPI.Interfaces;

namespace VRChatAPI.Objects
{
	public class APIConfig : SerializableObjectAbstract
	{/// <summary>
	 /// Address of the VRChat's office
	 /// </summary>
		public string Address { get; set; }
		/// <summary>
		/// Announcements from VRChat
		/// </summary>
		public IEnumerable<Announcement> Announcements { get; set; }
		/// <summary>
		/// ApiKey that is needed to call api
		/// </summary>
		/// <remarks>this value is the same as <see cref="ClientApiKey"/></remarks>
		public string ApiKey { get; set; }
		public string AppName { get; set; }
		public string BuildVersionTag { get; set; }
		/// <summary>
		/// ApiKey that is needed to call api
		/// </summary>
		/// <remarks>this value is the same as <see cref="ApiKey"/></remarks>
		public string ClientApiKey { get; set; }
		public int ClientBPSCeiling { get; set; }
		public int ClientDisconnectTimeout { get; set; }
		public int ClientReserverPlayerBPS { get; set; }

		/// <summary>
		/// VRChat's contact email
		/// </summary>
		public string ContactEmail { get; set; }
		/// <summary>
		/// VRChat's email for copyright issues
		/// </summary>
		public string CopyrightEmail { get; set; }
		/// <summary>
		/// Current version of Term of Service
		/// </summary>
		public int CurrentTOSVersion { get; set; }
		/// <summary>
		/// Default Avatar's Id
		/// </summary>
		/// <value></value>
		public AvatarID DefaultAvatar { get; set; }
		/// <summary>
		/// Used to identify which API deployment cluster is currently responding.
		/// </summary>
		public EDeploymentGroup DeploymentGroup { get; set; }
		public string DevAppVersionStandalone { get; set; }
		public string DevDownloadLinkWindows { get; set; }
		/// <summary>
		/// Download link of Sdk
		/// </summary>
		public string DevSdkUrl { get; set; }
		public string DevSdkVersion { get; set; }
		public string DevServerVersionStandalone { get; set; }

		#region disables
		public bool DisableAvatarCopying { get; set; }
		public bool DisableAvatarGating { get; set; }
		public bool DisableCommunityLabs { get; set; }
		public bool DisableCommunityLabsPromotion { get; set; }
		public bool DisableEmail { get; set; }
		public bool DisableEventStream { get; set; }
		public bool DisableFeedbackGating { get; set; }
		public bool DisableHello { get; set; }
		public bool DisableRegistration { get; set; }
		public bool DisableSteamNetworking { get; set; }
		public bool DisableTwoFactorAuth { get; set; }
		public bool DisableUdon { get; set; }
		public bool DisableUpgradeAccount { get; set; }
		#endregion

		public string DownloadLinkWindows { get; set; }
		/// <summary>
		/// Direct Links of SDK
		/// </summary>
		public DownloadUrls DownloadUrls { get; set; }
		/// <summary>
		/// Rows to display in the "world" tab in game menu
		/// </summary>
		public IEnumerable<DynamicWorldRow> DynamicWorldRows { get; set; }
		public Event Events { get; set; }
		public string GearDemoRoomId { get; set; }
		/// <summary>
		/// Redirect Url of VRChat website
		/// </summary>
		public string HomepageRedirectTarget { get; set; }
		/// <summary>
		/// Home world Id
		/// </summary>
		public WorldID HomeWorldId { get; set; }
		/// <summary>
		/// Hub World Id
		/// </summary>
		public WorldID HubWorldId { get; set; }
		/// <summary>
		/// VRChat's job application email
		/// </summary>
		public string JobsEmail { get; set; }
		/// <summary>
		/// Daily message from VRChat
		/// </summary>
		/// <remarks>This message seems to have never changed</remarks>
		public string MessageOfTheDay { get; set; }
		/// <summary>
		/// VRChat's moderation related email
		/// </summary>
		public string ModerationEmail { get; set; }
		public int ModerationQueryPeriod { get; set; }
		public string NotAllowedToSelectAvatarInPrivateWorldMessage { get; set; }
		public string Plugin { get; set; }
		public string ReleaseAppVersionStandalone { get; set; }
		public string ReleaseSdkUrl { get; set; }
		public string ReleaseSdkVersion { get; set; }
		public string ReleaseServerVersionStandalone { get; set; }
		public string SdkDeveloperFaqUrl { get; set; }
		/// <summary>
		/// Discord invitation Url
		/// </summary>
		public string SdkDiscordUrl { get; set; }
		public string SdkNotAllowedToPublishMessage { get; set; }
		/// <summary>
		/// Unity Version for VRChat dev
		/// </summary>
		public string SdkUnityVersion { get; set; }
		public string ServerName { get; set; }
		/// <summary>
		/// VRChat's support email
		/// </summary>
		public string SupportEmail { get; set; }
		/// <summary>
		/// Timeout World Id
		/// </summary>
		public WorldID TimeOutWorldId { get; set; }
		/// <summary>
		/// Tutorial World Id
		/// </summary>
		public WorldID TutorialWorldId { get; set; }
		public int UpdateRateMsMaximum { get; set; }
		public int UpdateRateMsMinimum { get; set; }
		public int UpdateRateMsNormal { get; set; }
		public int UpdateRateMsUdonManual { get; set; }
		public int UploadAnalysisPercent { get; set; }
		public IEnumerable<string> UrlList { get; set; }
		public bool UseReliableUdpForVoice { get; set; }
		public int UserUpdatePeriod { get; set; }
		public int UserVerificationDelay { get; set; }
		public int UserVerificationRetry { get; set; }
		public int UserVerificationTimeout { get; set; }
		public string ViveWindowsUrl { get; set; }
		public IEnumerable<string> WhiteListedAssetUrls { get; set; }
		public int WorldUpdatePeriod { get; set; }
		[JsonPropertyName("youtubedl-hash")]
		public string Youtubedl_hash { get; set; }
		[JsonPropertyName("youtubedl-version")]
		public DateTime? Youtubedl_version { get; set; }

		public bool? SdkEnableDeltaCompression { get; set; }
	}
}
