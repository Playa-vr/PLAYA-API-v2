using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string Access { get; set; }

        [JsonProperty("refresh_token")]
        public string Refresh { get; set; }
    }
}
