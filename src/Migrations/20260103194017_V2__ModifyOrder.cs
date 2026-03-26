using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceDatabaseBenchmark.Migrations
{
    /// <inheritdoc />
    public partial class V2__ModifyOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId_CreatedAt",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId_CreatedAt_OrderId",
                table: "Orders",
                columns: new[] { "CustomerId", "CreatedAt", "OrderId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_CustomerId_CreatedAt_OrderId",
                table: "Orders");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId_CreatedAt",
                table: "Orders",
                columns: new[] { "CustomerId", "CreatedAt" });
        }
    }
}
