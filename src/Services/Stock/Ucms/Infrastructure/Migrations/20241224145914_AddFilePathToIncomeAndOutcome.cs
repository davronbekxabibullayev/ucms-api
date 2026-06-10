using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathToIncomeAndOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Outcomes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Incomes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Outcomes");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Incomes");
        }
    }
}
