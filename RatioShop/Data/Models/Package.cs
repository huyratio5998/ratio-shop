using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.Models
{
    public class Package : BaseProduct
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ProductFriendlyName { get; set; }
        public string? Image { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Price { get; set; }
        public double? DiscountRate { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }
        public bool IsSoldOnline { get; set; }

        public List<ProductVariantPackage>? ProductVariantPackage { get; set; }
    }
}