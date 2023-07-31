using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class addmultiimagefeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "ProductVariant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "ProductVariant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ProductVariant",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProductVariant");
        }
    }
}
