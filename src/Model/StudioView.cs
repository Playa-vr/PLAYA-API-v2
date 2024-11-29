using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class StudioView
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("views")]
        public long? Views { get; set; }
    }
}
