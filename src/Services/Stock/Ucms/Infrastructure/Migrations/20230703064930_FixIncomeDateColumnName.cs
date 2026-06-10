using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    public partial class FixIncomeDateColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeriaNumber",
                table: "Skus",
                newName: "SerialNumber");

            migrationBuilder.RenameColumn(
                name: "OutcomeDate",
                table: "Incomes",
                newName: "IncomeDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "Skus",
                newName: "SeriaNumber");

            migrationBuilder.RenameColumn(
                name: "IncomeDate",
                table: "Incomes",
                newName: "OutcomeDate");
        }
    }
}
