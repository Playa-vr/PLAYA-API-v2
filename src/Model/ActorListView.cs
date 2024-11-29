using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class ActorListView
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }
    }
}
