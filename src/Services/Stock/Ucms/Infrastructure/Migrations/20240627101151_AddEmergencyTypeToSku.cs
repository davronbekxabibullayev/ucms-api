using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmergencyTypeToSku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmergencyType",
                table: "Skus",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmergencyType",
                table: "Skus");
        }
    }
}
