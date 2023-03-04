using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.Models
{
    public class BaseProduct
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
