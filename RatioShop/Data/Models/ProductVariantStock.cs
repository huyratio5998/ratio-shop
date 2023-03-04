using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RatioShop.Data.Models
{
    public class ProductVariantStock
    {        
        public int StockId { get; set; }
        public Stock Stock { get; set; }        
        public Guid ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; }
    }
}