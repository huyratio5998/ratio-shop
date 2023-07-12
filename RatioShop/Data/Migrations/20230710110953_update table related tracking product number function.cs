using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class updatetablerelatedtrackingproductnumberfunction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReverted",
                table: "ProductVariantCart",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StockItems",
                table: "ProductVariantCart",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StockTrackingStatus",
                table: "ProductVariantCart",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TrackUpdated",
                table: "ProductVariantCart",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReverted",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "StockItems",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "StockTrackingStatus",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "TrackUpdated",
                table: "ProductVariantCart");
        }
    }
}
