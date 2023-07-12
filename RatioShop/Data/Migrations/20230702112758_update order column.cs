using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class updateordercolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_AspNetUsers_ShipperId",
                table: "Shipment");

            migrationBuilder.DropIndex(
                name: "IX_Shipment_ShipperId",
                table: "Shipment");

            migrationBuilder.AlterColumn<Guid>(
                name: "ShipperId",
                table: "Shipment",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipperId1",
                table: "Shipment",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_ShipperId1",
                table: "Shipment",
                column: "ShipperId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_AspNetUsers_ShipperId1",
                table: "Shipment",
                column: "ShipperId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipment_AspNetUsers_ShipperId1",
                table: "Shipment");

            migrationBuilder.DropIndex(
                name: "IX_Shipment_ShipperId1",
                table: "Shipment");

            migrationBuilder.DropColumn(
                name: "ShipperId1",
                table: "Shipment");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Cart");

            migrationBuilder.AlterColumn<string>(
                name: "ShipperId",
                table: "Shipment",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_ShipperId",
                table: "Shipment",
                column: "ShipperId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipment_AspNetUsers_ShipperId",
                table: "Shipment",
                column: "ShipperId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
