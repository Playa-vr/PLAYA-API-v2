using Newtonsoft.Json;

using System.Collections.Generic;
using System.Linq;

namespace PlayaApiV2.Model
{
    public class TransparencyInfo
    {
        [JsonProperty("m")]
        public TransparencyMode Mode { get; set; }

        [JsonProperty("i")]
        public bool ChromaInversed { get; set; }

        [JsonProperty("c")]
        public List<ChromaMask> ChromaMasks { get; set; }

        public TransparencyInfo Clone()
        {
            var clone = (TransparencyInfo)MemberwiseClone();
            clone.ChromaMasks = ChromaMasks?.Select(m => m?.Clone()).ToList();
            return clone;
        }
    }

    public enum TransparencyMode
    {
        None = 0,
        Mask = 1,
        Chroma = 2,
        ExternalMask = 3,
    }

    public class ChromaMask
    {
        [JsonProperty("e")]
        public bool Enabled { get; set; }

        [JsonProperty("r")]
        public float Range { get; set; }

        [JsonProperty("f")]
        public float Fallof { get; set; }

        [JsonProperty("c")]
        public Color24 Color { get; set; }

        [JsonProperty("h")]
        public float Hue { get; set; }

        [JsonProperty("s")]
        public float Saturation { get; set; }

        [JsonProperty("v")]
        public float Value { get; set; }

        public ChromaMask Clone()
        {
            var clone = (ChromaMask)MemberwiseClone();
            return clone;
        }
    }
}
