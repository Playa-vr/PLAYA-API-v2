using Newtonsoft.Json;

using System.Collections.Generic;

namespace PlayaApiV2.Model
{
    public class VideoListView
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("preview_image")]
        public string Preview { get; set; }

        [JsonProperty("has_scripts")]
        public bool HasScripts { get; set; }

        [JsonProperty("release_date")]
        public Timestamp? ReleaseDate { get; set; }

        [JsonProperty("details")]
        public List<VideoDetails> Details { get; set; }

        public class VideoDetails
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("duration_seconds")]
            public long? DurationSeconds { get; set; }

            [JsonProperty("transparency_mode")]
            public TransparencyMode Mode { get; set; }

            [JsonProperty("has_scripts")]
            public bool HasScripts { get; set; }
        }
    }
}
