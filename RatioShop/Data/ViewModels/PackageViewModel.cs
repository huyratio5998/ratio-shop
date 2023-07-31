﻿namespace RatioShop.Data.ViewModels
{
    public class PackageViewModel
    {
        public Guid Id { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? ProductFriendlyName { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Image { get; set; }        
        public decimal? Price { get; set; }
        public bool IsDelete { get; set; }
        public bool IsNew { get; set; }
        public bool IsSoldOnline { get; set; }

        public double? DiscountRate { get; set; }
        public List<ProductVariantViewModel>? PackageItems { get; set; }        
    }
}
