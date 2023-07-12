using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RatioShop.Data.Migrations
{
    public partial class removeseeddata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Payment",
                keyColumn: "Id",
                keyValue: new Guid("fb7c7b80-a9c8-41f2-a0d9-9fc240a3d330"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Payment",
                columns: new[] { "Id", "CreatedDate", "Description", "DisplayName", "IsActive", "Logo", "ModifiedDate", "Name", "Type" },
                values: new object[] { new Guid("fb7c7b80-a9c8-41f2-a0d9-9fc240a3d330"), new DateTime(2023, 6, 28, 11, 15, 32, 970, DateTimeKind.Utc).AddTicks(9836), null, "Cash on delivery", true, null, new DateTime(2023, 6, 28, 11, 15, 32, 970, DateTimeKind.Utc).AddTicks(9839), "Cash on delivery", null });
        }
    }
}
