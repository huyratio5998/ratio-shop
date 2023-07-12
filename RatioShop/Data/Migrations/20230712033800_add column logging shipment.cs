using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class addcolumnloggingshipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Request",
                table: "Shipment",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UpdateStatus",
                table: "Shipment",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Request",
                table: "Shipment");

            migrationBuilder.DropColumn(
                name: "UpdateStatus",
                table: "Shipment");
        }
    }
}
