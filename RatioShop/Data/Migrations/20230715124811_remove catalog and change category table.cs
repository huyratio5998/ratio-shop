using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class removecatalogandchangecategorytable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Catalog_CatalogId",
                table: "Category");

            migrationBuilder.DropTable(
                name: "Catalog");

            migrationBuilder.DropIndex(
                name: "IX_Category_CatalogId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CatalogId",
                table: "Category");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Category",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_ParentId",
                table: "Category",
                column: "ParentId",
                unique: true,
                filter: "[ParentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Category_ParentId",
                table: "Category",
                column: "ParentId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Category_Category_ParentId",
                table: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Category_ParentId",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Category");

            migrationBuilder.AddColumn<int>(
                name: "CatalogId",
                table: "Category",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Catalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Category_CatalogId",
                table: "Category",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Category_Catalog_CatalogId",
                table: "Category",
                column: "CatalogId",
                principalTable: "Catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
