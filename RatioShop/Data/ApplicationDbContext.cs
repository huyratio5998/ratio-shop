using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RatioShop.Data.Models;

namespace RatioShop.Data
{
    public class ApplicationDbContext : IdentityDbContext<ShopUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ProductCategory>().HasKey(x=> new {x.ProductId, x.CategoryId});
            builder.Entity<ProductVariantStock>().HasKey(x => new { x.StockId, x.ProductVariantId });            

        }
        public DbSet<RatioShop.Data.Models.Product> Product { get; set; }
        public DbSet<RatioShop.Data.Models.Address>? Address { get; set; }
        public DbSet<RatioShop.Data.Models.Payment>? Payment { get; set; }
        public DbSet<RatioShop.Data.Models.Stock>? Stock { get; set; }
        public DbSet<RatioShop.Data.Models.Order>? Order { get; set; }
        public DbSet<RatioShop.Data.Models.Cart>? Cart { get; set; }
        public DbSet<RatioShop.Data.Models.ProductVariant>? ProductVariant { get; set; }
    }
}