using Newtonsoft.Json;

namespace RatioShop.Areas.Admin.Models
{
    public class ShipmentTrackingResponse
    {
        [JsonProperty("allowUpdateShipment")]
        public bool AllowUpdateShipment { get; set; }
        
        [JsonProperty("allowUpdateShipmentOnOrder")]
        public bool AllowUpdateShipmentOnOrder { get; set; }
        
        [JsonProperty("status")]
        public bool Status { get; set; }        
    }
}
