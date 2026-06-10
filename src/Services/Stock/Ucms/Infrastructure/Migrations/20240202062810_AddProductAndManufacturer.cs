using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductAndManufacturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpirationDate",
                table: "Skus",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "ManufacturerId",
                table: "Skus",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Manufacturers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NameKa = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NameKa = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Skus_ManufacturerId",
                table: "Skus",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Skus_ProductId",
                table: "Skus",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturers_Id",
                table: "Manufacturers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Id",
                table: "Products",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Skus_Manufacturers_ManufacturerId",
                table: "Skus",
                column: "ManufacturerId",
                principalTable: "Manufacturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Skus_Products_ProductId",
                table: "Skus",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Skus_Manufacturers_ManufacturerId",
                table: "Skus");

            migrationBuilder.DropForeignKey(
                name: "FK_Skus_Products_ProductId",
                table: "Skus");

            migrationBuilder.DropTable(
                name: "Manufacturers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Skus_ManufacturerId",
                table: "Skus");

            migrationBuilder.DropIndex(
                name: "IX_Skus_ProductId",
                table: "Skus");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Skus");

            migrationBuilder.DropColumn(
                name: "ManufacturerId",
                table: "Skus");
        }
    }
}
