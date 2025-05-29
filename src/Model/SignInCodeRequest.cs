using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class SignInCodeRequest
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }
}
