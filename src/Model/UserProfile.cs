using Newtonsoft.Json;

using System.Collections.Generic;

namespace PlayaApiV2.Model
{
    public class UserProfile
    {
        [JsonProperty("display_name")]
        public string Name { get; set; }

        [JsonProperty("role")]
        public string Role { get; set; }

        public const string RoleFree = "free";
        public const string RolePremium = "premium";
        public static readonly IReadOnlyList<string> Roles = new[] { RoleFree, RolePremium };
    }
}
