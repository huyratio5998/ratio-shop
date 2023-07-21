using Newtonsoft.Json;

namespace RatioShop.Data.ViewModels.SearchViewModel
{
    public class FacetFilterItem
    {
        [JsonProperty("fieldName")]
        public string FieldName { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
        
        [JsonProperty("type")]
        public string Type { get; set; }

    }
}
