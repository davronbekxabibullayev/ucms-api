using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOutcomeIdToDemand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BroadcastStatus",
                table: "StockDemands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OutcomeId",
                table: "StockDemands",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockDemands_OutcomeId",
                table: "StockDemands",
                column: "OutcomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockDemands_Outcomes_OutcomeId",
                table: "StockDemands",
                column: "OutcomeId",
                principalTable: "Outcomes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockDemands_Outcomes_OutcomeId",
                table: "StockDemands");

            migrationBuilder.DropIndex(
                name: "IX_StockDemands_OutcomeId",
                table: "StockDemands");

            migrationBuilder.DropColumn(
                name: "BroadcastStatus",
                table: "StockDemands");

            migrationBuilder.DropColumn(
                name: "OutcomeId",
                table: "StockDemands");
        }
    }
}
