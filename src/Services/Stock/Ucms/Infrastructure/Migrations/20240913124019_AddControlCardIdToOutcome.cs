using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddControlCardIdToOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ControlCardId",
                table: "Outcomes",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItems_ProductId",
                table: "StockDemandItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockDemandItems_Products_ProductId",
                table: "StockDemandItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockDemandItems_Products_ProductId",
                table: "StockDemandItems");

            migrationBuilder.DropIndex(
                name: "IX_StockDemandItems_ProductId",
                table: "StockDemandItems");

            migrationBuilder.DropColumn(
                name: "ControlCardId",
                table: "Outcomes");
        }
    }
}
