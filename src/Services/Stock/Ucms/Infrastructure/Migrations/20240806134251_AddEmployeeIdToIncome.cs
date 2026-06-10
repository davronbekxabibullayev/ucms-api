using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeIdToIncome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "StockDemands",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "StockDemands",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Outcomes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "Outcomes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EmployeeId",
                table: "Incomes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "Incomes",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "StockDemands");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "StockDemands");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Outcomes");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "Outcomes");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Incomes");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "Incomes");
        }
    }
}
