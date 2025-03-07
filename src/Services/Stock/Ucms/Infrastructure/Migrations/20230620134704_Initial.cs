using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ucms.Stock.Api.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Multiplier = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NameKa = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StorageCondition = table.Column<int>(type: "integer", nullable: false),
                    StockType = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NameKa = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_Stocks_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Stocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Skus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SeriaNumber = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameRu = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    NameEn = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    NameKa = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skus_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    OutcomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IncomeType = table.Column<int>(type: "integer", nullable: false),
                    IncomeStatus = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incomes_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Outcomes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    OutcomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OutcomeType = table.Column<int>(type: "integer", nullable: false),
                    OutcomeStatus = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outcomes_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockSkus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<double>(type: "double precision", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSkus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockSkus_Skus_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Skus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockSkus_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeItems_Incomes_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Incomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomeItems_Skus_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Skus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutcomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomeItems_Outcomes_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcomes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeItems_Skus_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Skus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItems_Id",
                table: "IncomeItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItems_IncomeId",
                table: "IncomeItems",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItems_SkuId",
                table: "IncomeItems",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_Id",
                table: "Incomes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Incomes_StockId",
                table: "Incomes",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUnits_Id",
                table: "MeasurementUnits",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItems_Id",
                table: "OutcomeItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItems_OutcomeId",
                table: "OutcomeItems",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItems_SkuId",
                table: "OutcomeItems",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_Id",
                table: "Outcomes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Outcomes_StockId",
                table: "Outcomes",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Skus_Id",
                table: "Skus",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Skus_MeasurementUnitId",
                table: "Skus",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Id",
                table: "Stocks",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ParentId",
                table: "Stocks",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockSkus_Id",
                table: "StockSkus",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockSkus_SkuId",
                table: "StockSkus",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_StockSkus_StockId",
                table: "StockSkus",
                column: "StockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IncomeItems");

            migrationBuilder.DropTable(
                name: "OutcomeItems");

            migrationBuilder.DropTable(
                name: "StockSkus");

            migrationBuilder.DropTable(
                name: "Incomes");

            migrationBuilder.DropTable(
                name: "Outcomes");

            migrationBuilder.DropTable(
                name: "Skus");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");
        }
    }
}
