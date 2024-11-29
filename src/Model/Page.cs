using Newtonsoft.Json;

using System.Collections.Generic;

namespace PlayaApiV2.Model
{
    public class Page<T>
    {
        [JsonProperty("page_index")]
        public long PageIndex { get; set; }

        [JsonProperty("page_size")]
        public long PageSize { get; set; }

        [JsonProperty("page_total")]
        public long PageTotal { get; set; }

        [JsonProperty("item_total")]
        public long ItemTotal { get; set; }

        [JsonProperty("content")]
        public List<T> Content { get; set; }

        public Page()
        {

        }

        public Page(List<T> content)
        {
            Content = content ?? throw new System.ArgumentNullException(nameof(content));
            PageIndex = 0;
            PageSize = content.Count;
            PageTotal = 1;
            ItemTotal = content.Count;
        }
    }
}
