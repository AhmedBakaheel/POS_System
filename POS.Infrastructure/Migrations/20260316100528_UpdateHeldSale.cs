using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHeldSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HeldSaleId",
                table: "SaleItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_HeldSaleId",
                table: "SaleItems",
                column: "HeldSaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_HeldSales_HeldSaleId",
                table: "SaleItems",
                column: "HeldSaleId",
                principalTable: "HeldSales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_HeldSales_HeldSaleId",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_HeldSaleId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "HeldSaleId",
                table: "SaleItems");
        }
    }
}
