using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class addcolumnisDeletetoproductvariantdiscount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DiscountRate",
                table: "ProductVariantCart",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemPrice",
                table: "ProductVariantCart",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "ProductVariant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Discount",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountRate",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "ItemPrice",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Discount");
        }
    }
}
