using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Extensions.Logging;

#pragma warning disable IDE1006

namespace VRChatAPI.Objects
{
    using AssetUrlObject = Newtonsoft.Json.Linq.JToken;

    public class UnityPackage
    {
        public string id { get; set; }
        public string assetUrl { get; set; }
        public AssetUrlObject assetUrlObject { get; set; }
        public string unityVersion { get; set; }
        public long unitySortNumber { get; set; }
        public int assetVersion { get; set; }
        public PlatformEnum platform { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class UnityPackageUrlObject
    {
        public string unityPackageUrl { get; set; }
    }

    [JsonConverter(typeof(AsTConverter<string>))]
    public class AvatarId
    {
        private static ILogger Logger => Global.LoggerFactory.CreateLogger<AvatarId>();

        public AvatarId(string id) => this.id = id;

        public string id { get; set; }

        public static implicit operator AvatarId(string s) => new AvatarId(s);
        public static implicit operator string(AvatarId avatarId) => avatarId.ToString();
        public override string ToString() => id;

        /// <summary>
        /// Get avatar by id
        /// </summary>
        /// <returns>Avatar object</returns>
        /// <exception cref="UnauthorizedRequestException"/>
        public async Task<Avatar> Get()
        {
            Logger.LogDebug("Getting avatar details using ID: {id}", id);
            var response = await Global.httpClient.GetAsync($"avatars/{id}");
            return await Utils.UtilFunctions.ParseResponse<Avatar>(response);
        }

        /// <summary>
        /// Add avatar to favorites
        /// </summary>
        /// <param name="groups">Indexes of favorite groups</param>
        /// <returns>Favorite object</returns>
        /// <exception cref="UnauthorizedRequestException"/>
        public async Task<Favorite> AddToFavorite(params FavoriteGroupId[] groups)
        {
            var api = new Endpoints.FavoriteAPI();
            return await api.AddToFavorite(FavoriteType.avatar, id, groups);
        }

        /// <summary>
        /// Choose avatar to use
        /// </summary>
        /// <returns>Updated Current User object</returns>
        /// <exception cref="UnauthorizedRequestException"/>
        public async Task<CurrentUser> Choose()
        {
            Logger.LogDebug("Choose Avatar {id}", id);
            var response = await Global.httpClient.PutAsync($"Avatars/{id}", null);
            return await Utils.UtilFunctions.ParseResponse<CurrentUser>(response);
        }

        /// <summary>
        /// Delete avatar
        /// </summary>
        /// <remarks>
        /// You have to own the avatar to delete
        /// </remarks>
        /// <returns>Deleted avatar object</returns>
        /// <exception cref="UnauthorizedRequestException"/>
        public async Task<Avatar> Delete()
        {
            Logger.LogDebug("Delete Avatar {id}", id);
            var response = await Global.httpClient.DeleteAsync($"avatars/{id}");
            return await Utils.UtilFunctions.ParseResponse<Avatar>(response);
        }
    }

    public class Avatar
    {
        public AvatarId id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string authorId { get; set; }
        public string authorName { get; set; }
        public List<string> tags { get; set; }
        public string assetUrl { get; set; }
        public AssetUrlObject assetUrlObject { get; set; }
        public string imageUrl { get; set; }
        public string thumbnailImageUrl { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ReleaseStatus releaseStatus { get; set; }
        public int version { get; set; }
        public bool featured { get; set; }
        public List<UnityPackage> unityPackages { get; set; }
        // public bool unityPackageUpdated { get; set; }
        public string unityPackageUrl { get; set; }
        public UnityPackageUrlObject unityPackageUrlObject { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
    }
}
