using Newtonsoft.Json;

using System.Collections.Generic;

namespace PlayaApiV2.Model
{
    public class CategoriesGroup
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("items")]
        public List<CategoryListView> Items { get; set; }
    }
}
