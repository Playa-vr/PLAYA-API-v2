using Newtonsoft.Json;

using System.Collections.Generic;

namespace PlayaApiV2.Model
{
    public class ActorView
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("preview")]
        public string Preview { get; set; }

        [JsonProperty("views")]
        public long? Views { get; set; }

        [JsonProperty("banner")]
        public string Banner { get; set; }

        [JsonProperty("studios")]
        public List<Studio> Studios { get; set; }

        [JsonProperty("properties")]
        public List<Property> Properties { get; set; }

        [JsonProperty("aliases")]
        public List<string> Aliases { get; set; }

        public class Studio
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("title")]
            public string Title { get; set; }
        }

        public class Property
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }
    }
}
