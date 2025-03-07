using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferStatusToIncomeOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutcomeTransferStatus",
                table: "Outcomes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncomeTransferStatus",
                table: "Incomes",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutcomeTransferStatus",
                table: "Outcomes");

            migrationBuilder.DropColumn(
                name: "IncomeTransferStatus",
                table: "Incomes");
        }
    }
}
