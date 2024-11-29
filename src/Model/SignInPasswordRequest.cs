using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class SignInPasswordRequest
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
