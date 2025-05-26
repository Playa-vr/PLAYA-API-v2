using Newtonsoft.Json;

using System.Collections.Generic;

namespace PlayaApiV2.Model
{
    public class VideoView
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("preview_image")]
        public string Preview { get; set; }

        [JsonProperty("release_date")]
        public Timestamp? ReleaseDate { get; set; }

        [JsonProperty("views")]
        public long? Views { get; set; }

        [JsonProperty("studio")]
        public StudioRef Studio { get; set; }

        [JsonProperty("categories")]
        public List<CategoryRef> Categories { get; set; }

        [JsonProperty("actors")]
        public List<ActorRef> Actors { get; set; }

        [JsonProperty("details")]
        public List<VideoDetails> Details { get; set; }

        [JsonProperty("transparency")]
        public TransparencyInfo Transparency { get; set; }

        public class StudioRef
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public class CategoryRef
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public class ActorRef
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public class VideoDetails
        {
            [JsonProperty("type")]
            public string Type { get; set; }

            [JsonProperty("duration_seconds")]
            public long? DurationSeconds { get; set; }
            
            [JsonProperty("timeline_atlas")]
            public TimelineAtlas TimelineAtlas { get; set; }

            [JsonProperty("timeline_markers")]
            public Timeline Timeline { get; set; }

            [JsonProperty("links")]
            public List<VideoLinkView> Links { get; set; }

            [JsonProperty("script_info")]
            public VideoScriptInfo ScriptInfo { get; set; }
        }
    }
}
