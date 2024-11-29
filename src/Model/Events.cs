using Newtonsoft.Json;

namespace PlayaApiV2.Model.Events
{
    public class VideoStreamEnd
    {
        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("video_quality")]
        public string VideoQuality { get; set; }

        [JsonProperty("duration")]
        public double TotalSeconds { get; set; }
    }

    public class VideoDownloaded
    {
        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("video_quality")]
        public string VideoQuality { get; set; }
    }
}
