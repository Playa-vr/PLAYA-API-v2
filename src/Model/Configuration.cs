using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class Configuration
    {
        [JsonProperty("site_name")]
        public string SiteName { get; set; }

        [JsonProperty("site_logo")]
        public string SiteLogo { get; set; }

        [JsonProperty("actors")]
        public bool Actors { get; set; }

        [JsonProperty("categories")]
        public bool Categories { get; set; }

        [JsonProperty("categories_groups")]
        public bool CategoriesGroups { get; set; }

        [JsonProperty("studios")]
        public bool Studios { get; set; }

        [JsonProperty("analytics")]
        public bool Analytics { get; set; }

        [JsonProperty("theme")]
        public long? Theme { get; set; }

        [JsonIgnore]
        public bool Auth { get => _auth ?? true; set => _auth = value; }

        [JsonProperty("auth")]
        private bool? _auth { get; set; }

        [JsonIgnore]
        public bool NSFW { get => _nswf ?? true; set => _nswf = value; }

        [JsonProperty("nsfw")]
        private bool? _nswf { get; set; }
    }
}
