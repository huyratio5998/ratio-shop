using RatioShop.Data.Models;

namespace RatioShop.Data.ViewModels
{
    public class UpdateProductAdditionalInformationResponse
    {
        public UpdateProductAdditionalInformationResponse()
        {

        }
        public UpdateProductAdditionalInformationResponse(bool status, List<ProductVariant>? variants)
        {
            Status = status;
            Variants = variants;
        }

        public bool Status { get; set; }
        public List<ProductVariant>? Variants { get; set; }
    }
}
