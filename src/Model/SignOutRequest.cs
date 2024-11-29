using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class SignOutRequest
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
