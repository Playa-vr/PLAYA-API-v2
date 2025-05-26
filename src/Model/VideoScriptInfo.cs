using Newtonsoft.Json;

namespace PlayaApiV2.Model
{
    public class VideoScriptInfo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("generation_source")]
        public ScriptGenerationSource ScriptGenerationSource { get; set; }
    }

    public enum ScriptGenerationSource
    {
        Manual = 0,
        AI = 1
    }
}
