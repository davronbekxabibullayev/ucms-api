using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIncomeOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MeasurementUnitId",
                table: "StockSkus",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MeasurementUnitId",
                table: "OutcomeItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MeasurementUnitId",
                table: "IncomeItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IncomeOutcomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OutcomeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutcomeStockId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncomeStockId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeOutcomes_Incomes_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Incomes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncomeOutcomes_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockSkus_MeasurementUnitId",
                table: "StockSkus",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItems_MeasurementUnitId",
                table: "OutcomeItems",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItems_MeasurementUnitId",
                table: "IncomeItems",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcomes_Id",
                table: "IncomeOutcomes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcomes_IncomeId",
                table: "IncomeOutcomes",
                column: "IncomeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcomes_OutcomeId",
                table: "IncomeOutcomes",
                column: "OutcomeId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_IncomeItems_MeasurementUnits_MeasurementUnitId",
                table: "IncomeItems",
                column: "MeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutcomeItems_MeasurementUnits_MeasurementUnitId",
                table: "OutcomeItems",
                column: "MeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockSkus_MeasurementUnits_MeasurementUnitId",
                table: "StockSkus",
                column: "MeasurementUnitId",
                principalTable: "MeasurementUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IncomeItems_MeasurementUnits_MeasurementUnitId",
                table: "IncomeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OutcomeItems_MeasurementUnits_MeasurementUnitId",
                table: "OutcomeItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockSkus_MeasurementUnits_MeasurementUnitId",
                table: "StockSkus");

            migrationBuilder.DropTable(
                name: "IncomeOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_StockSkus_MeasurementUnitId",
                table: "StockSkus");

            migrationBuilder.DropIndex(
                name: "IX_OutcomeItems_MeasurementUnitId",
                table: "OutcomeItems");

            migrationBuilder.DropIndex(
                name: "IX_IncomeItems_MeasurementUnitId",
                table: "IncomeItems");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "StockSkus");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "OutcomeItems");

            migrationBuilder.DropColumn(
                name: "MeasurementUnitId",
                table: "IncomeItems");
        }
    }
}
