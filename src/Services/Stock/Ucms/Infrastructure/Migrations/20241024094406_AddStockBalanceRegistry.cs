using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStockBalanceRegistry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockBalanceRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousAmount = table.Column<double>(type: "double precision", nullable: false),
                    CurrentAmount = table.Column<double>(type: "double precision", nullable: false),
                    VariableAmount = table.Column<double>(type: "double precision", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBalanceRegistry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegistry_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegistry_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegistry_Skus_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Skus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegistry_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcomes_IncomeStockId",
                table: "IncomeOutcomes",
                column: "IncomeStockId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcomes_OutcomeStockId",
                table: "IncomeOutcomes",
                column: "OutcomeStockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegistry_Id",
                table: "StockBalanceRegistry",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegistry_MeasurementUnitId",
                table: "StockBalanceRegistry",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegistry_ProductId",
                table: "StockBalanceRegistry",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegistry_SkuId",
                table: "StockBalanceRegistry",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegistry_StockId",
                table: "StockBalanceRegistry",
                column: "StockId");

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeOutcomes_Stocks_IncomeStockId",
                table: "IncomeOutcomes",
                column: "IncomeStockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeOutcomes_Stocks_OutcomeStockId",
                table: "IncomeOutcomes",
                column: "OutcomeStockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeOutcomes_Stocks_IncomeStockId",
                table: "IncomeOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_IncomeOutcomes_Stocks_OutcomeStockId",
                table: "IncomeOutcomes");

            migrationBuilder.DropTable(
                name: "StockBalanceRegistry");

            migrationBuilder.DropIndex(
                name: "IX_IncomeOutcomes_IncomeStockId",
                table: "IncomeOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_IncomeOutcomes_OutcomeStockId",
                table: "IncomeOutcomes");
        }
    }
}
