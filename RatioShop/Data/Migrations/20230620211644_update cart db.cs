using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class updatecartdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_ProductVariantCart_ProductVariantCartId",
                table: "Cart");

            migrationBuilder.DropIndex(
                name: "IX_Cart_ProductVariantCartId",
                table: "Cart");

            migrationBuilder.DropColumn(
                name: "ProductVariantCartId",
                table: "Cart");

            migrationBuilder.AddColumn<Guid>(
                name: "CartId",
                table: "ProductVariantCart",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ItemNumber",
                table: "ProductVariantCart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantCart_CartId",
                table: "ProductVariantCart",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantCart_Cart_CartId",
                table: "ProductVariantCart",
                column: "CartId",
                principalTable: "Cart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantCart_Cart_CartId",
                table: "ProductVariantCart");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariantCart_CartId",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "CartId",
                table: "ProductVariantCart");

            migrationBuilder.DropColumn(
                name: "ItemNumber",
                table: "ProductVariantCart");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductVariantCartId",
                table: "Cart",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Cart_ProductVariantCartId",
                table: "Cart",
                column: "ProductVariantCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_ProductVariantCart_ProductVariantCartId",
                table: "Cart",
                column: "ProductVariantCartId",
                principalTable: "ProductVariantCart",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
