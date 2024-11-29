using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TimelineAtlas
    {
        [JsonProperty("version")] public long Version { get; set; }

        [JsonProperty("url")] public string Url { get; set; }

        [JsonProperty("frame_width")] public int? FrameWidth { get; set; }
        [JsonProperty("frame_height")] public int? FrameHeight { get; set; }

        [JsonProperty("columns")] public int? Columns { get; set; }
        [JsonProperty("rows")] public int? Rows { get; set; }

        [JsonProperty("frames")] public int? Frames { get; set; }

        [JsonProperty("interval_ms")] public int? IntervalMs { get; set; }

        [JsonProperty("aspect_ratio")] public float? AspectRatio { get; set; }

        public TimelineAtlas() { }

        public TimelineAtlas(long version, string url, int? frameWidth, int? frameHeight, int? columns, int? rows, int? frames, int? intervalMs, float? aspectRatio)
        {
            Version = version;
            Url = url;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            Columns = columns;
            Rows = rows;
            Frames = frames;
            IntervalMs = intervalMs;
            AspectRatio = aspectRatio;
        }
    }
}
