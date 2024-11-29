using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class VideoLinkView
    {
        [JsonProperty("is_stream")]
        public bool IsStream { get; set; }

        [JsonProperty("is_download")]
        public bool IsDownload { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("unavailable_reason")]
        public string UnavailableReason { get; set; }

        [JsonProperty("projection")]
        public string ProjectionString { get; set; }

        [JsonProperty("stereo")]
        public string StereoString { get; set; }

        [JsonProperty("quality_name")]
        public string QualityName { get; set; }

        [JsonProperty("quality_order")]
        public long QualityOrder { get; set; }
    }
}
