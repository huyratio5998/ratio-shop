﻿using RatioShop.Enums;
using System.ComponentModel.DataAnnotations;

namespace RatioShop.Data.Models
{
    public class ProductVariant : BaseProduct
    {
        public string? Code { get; set; }        
        public int? Number { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? Price { get; set; }
        public double? DiscountRate { get; set; }
        public bool IsDelete { get; set; }
        public string? Images { get; set; }
        public VariantType Type { get; set; }        
        public string? Icon { get; set; }

        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public List<ProductVariantStock>? ProductVariantStocks { get; set; }
        public List<ProductVariantCart>? ProductVariantCarts { get; set; }
        public List<ProductVariantPackage>? ProductVariantPackage { get; set; }
    }
}