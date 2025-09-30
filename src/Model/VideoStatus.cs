using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class VideoStatus
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
