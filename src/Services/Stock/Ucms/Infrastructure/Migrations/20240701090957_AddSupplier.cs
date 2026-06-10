using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupplierId",
                table: "Skus",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Outcomes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Incomes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NameKa = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skus_SupplierId",
                table: "Skus",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Id",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skus_Suppliers_SupplierId",
                table: "Skus",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skus_Suppliers_SupplierId",
                table: "Skus");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Skus_SupplierId",
                table: "Skus");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Skus");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Outcomes");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Incomes");
        }
    }
}
