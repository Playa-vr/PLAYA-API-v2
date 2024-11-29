using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class RefreshTokenRequest
    {
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public RefreshTokenRequest(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
