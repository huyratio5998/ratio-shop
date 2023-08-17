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
            builder.Entity<ProductCategory>().HasKey(x => new { x.ProductId, x.CategoryId });
            builder.Entity<ProductVariantPackage>().HasKey(x => new { x.ProductVariantId, x.PackageId });
            builder.Entity<ProductVariantStock>().HasKey(x => new { x.StockId, x.ProductVariantId });
            builder.Entity<Category>()
            .HasOne(j => j.ParentCategory)
            .WithMany(x => x.Children)
            .HasForeignKey(f => f.ParentId);
        }
        public DbSet<Product> Product { get; set; }
        public DbSet<Address>? Address { get; set; }
        public DbSet<Payment>? Payment { get; set; }
        public DbSet<Stock>? Stock { get; set; }
        public DbSet<Order>? Order { get; set; }
        public DbSet<Cart>? Cart { get; set; }
        public DbSet<ProductVariant>? ProductVariant { get; set; }
        public DbSet<ProductVariantStock>? ProductVariantStock { get; set; }
        public DbSet<Discount>? Discount { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Shipment>? Shipment { get; set; }
        public DbSet<Package>? Package { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }

    }
}