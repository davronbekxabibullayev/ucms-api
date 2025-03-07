using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    public partial class AddStockDemand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockDemands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DemandDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DemandStatus = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDemands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockDemands_Stocks_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockDemands_Stocks_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockDemandItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockDemandId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NotApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDemandItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockDemandItems_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockDemandItems_StockDemands_StockDemandId",
                        column: x => x.StockDemandId,
                        principalTable: "StockDemands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItems_Id",
                table: "StockDemandItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItems_MeasurementUnitId",
                table: "StockDemandItems",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItems_StockDemandId",
                table: "StockDemandItems",
                column: "StockDemandId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemands_Id",
                table: "StockDemands",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemands_RecipientId",
                table: "StockDemands",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemands_SenderId",
                table: "StockDemands",
                column: "SenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockDemandItems");

            migrationBuilder.DropTable(
                name: "StockDemands");
        }
    }
}
