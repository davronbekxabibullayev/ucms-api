using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Ucms.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "Manufacturer",
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
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric", nullable: false),
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
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    TaxId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Address = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    InternationalCode = table.Column<string>(type: "text", nullable: true),
                    InternationalName = table.Column<string>(type: "text", nullable: true),
                    AlternativeName = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameKa = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    Code = table.Column<string>(type: "text", nullable: false),
                    StorageCondition = table.Column<int>(type: "integer", nullable: false),
                    StockType = table.Column<int>(type: "integer", nullable: false),
                    StockCategory = table.Column<int>(type: "integer", nullable: false),
                    EmployeeIds = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameKa = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stock_Stock_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Stock",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
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
                    table.PrimaryKey("PK_Supplier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationMeasurementUnit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationMeasurementUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationMeasurementUnit_MeasurementUnits_MeasurementUni~",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Brigades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ForemanName = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    Phone = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brigades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brigades_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Address = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Description = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    ContractNumber = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ContractDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Income",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    IncomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IncomeType = table.Column<int>(type: "integer", nullable: false),
                    IncomeStatus = table.Column<int>(type: "integer", nullable: false),
                    IncomeTransferStatus = table.Column<int>(type: "integer", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Income", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Income_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Outcome",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    OutcomeDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OutcomeType = table.Column<int>(type: "integer", nullable: false),
                    OutcomeStatus = table.Column<int>(type: "integer", nullable: false),
                    OutcomeTransferStatus = table.Column<int>(type: "integer", nullable: true),
                    PaymentType = table.Column<int>(type: "integer", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    FilePath = table.Column<string>(type: "text", nullable: true),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false),
                    ExecutionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outcome", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Outcome_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sku",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SerialNumber = table.Column<string>(type: "text", nullable: false),
                    ExpirationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManufacturerId = table.Column<Guid>(type: "uuid", nullable: true),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    NameRu = table.Column<string>(type: "text", nullable: false),
                    NameEn = table.Column<string>(type: "text", nullable: true),
                    NameKa = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sku", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sku_Manufacturer_ManufacturerId",
                        column: x => x.ManufacturerId,
                        principalTable: "Manufacturer",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sku_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sku_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sku_Supplier_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Supplier",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeviceInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                schema: "Identity",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BrigadePayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrigadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrigadePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrigadePayments_Brigades_BrigadeId",
                        column: x => x.BrigadeId,
                        principalTable: "Brigades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrigadePayments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientActs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActNumber = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ActDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientActs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientActs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimateSections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateSections_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeOutcome",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OutcomeId = table.Column<Guid>(type: "uuid", nullable: true),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutcomeStockId = table.Column<Guid>(type: "uuid", nullable: false),
                    IncomeStockId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeOutcome", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeOutcome_Income_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Income",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncomeOutcome_Outcome_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcome",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncomeOutcome_Stock_IncomeStockId",
                        column: x => x.IncomeStockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomeOutcome_Stock_OutcomeStockId",
                        column: x => x.OutcomeStockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockDemand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    SenderId = table.Column<Guid>(type: "uuid", nullable: false),
                    RecipientId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DemandDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DemandStatus = table.Column<int>(type: "integer", nullable: false),
                    BroadcastStatus = table.Column<int>(type: "integer", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: true),
                    OutcomeId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDemand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockDemand_Outcome_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcome",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockDemand_Stock_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockDemand_Stock_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IncomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncomeItem_Income_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "Income",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IncomeItem_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IncomeItem_Sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationSku",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationSku", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationSku_Sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OutcomeItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OutcomeId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ActualAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutcomeItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutcomeItem_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OutcomeItem_Outcome_OutcomeId",
                        column: x => x.OutcomeId,
                        principalTable: "Outcome",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutcomeItem_Sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockBalanceRegister",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    VariableAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBalanceRegister", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegister_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegister_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegister_Sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockBalanceRegister_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockSku",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false),
                    SkuId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: true),
                    Location = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockSku", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockSku_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockSku_Sku_SkuId",
                        column: x => x.SkuId,
                        principalTable: "Sku",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockSku_Stock_StockId",
                        column: x => x.StockId,
                        principalTable: "Stock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActId = table.Column<Guid>(type: "uuid", nullable: true),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientPayments_ClientActs_ActId",
                        column: x => x.ActId,
                        principalTable: "ClientActs",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClientPayments_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimateItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Unit = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Volume = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    ClientUnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    BrigadeUnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimateItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimateItems_EstimateSections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "EstimateSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockDemandItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockDemandId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    MeasurementUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    NotApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDemandItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockDemandItem_MeasurementUnits_MeasurementUnitId",
                        column: x => x.MeasurementUnitId,
                        principalTable: "MeasurementUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockDemandItem_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockDemandItem_StockDemand_StockDemandId",
                        column: x => x.StockDemandId,
                        principalTable: "StockDemand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientActItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimateItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Volume = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    UnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientActItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientActItems_ClientActs_ActId",
                        column: x => x.ActId,
                        principalTable: "ClientActs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientActItems_EstimateItems_EstimateItemId",
                        column: x => x.EstimateItemId,
                        principalTable: "EstimateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    BrigadeId = table.Column<Guid>(type: "uuid", nullable: false),
                    EstimateItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Volume = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    BrigadeUnitPrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    BrigadePaymentId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_BrigadePayments_BrigadePaymentId",
                        column: x => x.BrigadePaymentId,
                        principalTable: "BrigadePayments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkLogs_Brigades_BrigadeId",
                        column: x => x.BrigadeId,
                        principalTable: "Brigades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogs_EstimateItems_EstimateItemId",
                        column: x => x.EstimateItemId,
                        principalTable: "EstimateItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrigadePayments_BrigadeId",
                table: "BrigadePayments",
                column: "BrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_BrigadePayments_Id",
                table: "BrigadePayments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BrigadePayments_ProjectId",
                table: "BrigadePayments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Brigades_Id",
                table: "Brigades",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Brigades_OrganizationId",
                table: "Brigades",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientActItems_ActId",
                table: "ClientActItems",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientActItems_EstimateItemId",
                table: "ClientActItems",
                column: "EstimateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientActs_Id",
                table: "ClientActs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientActs_ProjectId",
                table: "ClientActs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPayments_ActId",
                table: "ClientPayments",
                column: "ActId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPayments_Id",
                table: "ClientPayments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPayments_ProjectId",
                table: "ClientPayments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateItems_Id",
                table: "EstimateItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateItems_SectionId",
                table: "EstimateItems",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateSections_Id",
                table: "EstimateSections",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EstimateSections_ProjectId",
                table: "EstimateSections",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Income_Id",
                table: "Income",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Income_StockId",
                table: "Income",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItem_Id",
                table: "IncomeItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItem_IncomeId",
                table: "IncomeItem",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItem_MeasurementUnitId",
                table: "IncomeItem",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeItem_SkuId",
                table: "IncomeItem",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcome_Id",
                table: "IncomeOutcome",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcome_IncomeId",
                table: "IncomeOutcome",
                column: "IncomeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcome_IncomeStockId",
                table: "IncomeOutcome",
                column: "IncomeStockId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcome_OutcomeId",
                table: "IncomeOutcome",
                column: "OutcomeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IncomeOutcome_OutcomeStockId",
                table: "IncomeOutcome",
                column: "OutcomeStockId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturer_Id",
                table: "Manufacturer",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementUnits_Id",
                table: "MeasurementUnits",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMeasurementUnit_Id",
                table: "OrganizationMeasurementUnit",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationMeasurementUnit_MeasurementUnitId",
                table: "OrganizationMeasurementUnit",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Organizations_Id",
                table: "Organizations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSku_Id",
                table: "OrganizationSku",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationSku_SkuId",
                table: "OrganizationSku",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_Outcome_Id",
                table: "Outcome",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Outcome_StockId",
                table: "Outcome",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItem_Id",
                table: "OutcomeItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItem_MeasurementUnitId",
                table: "OutcomeItem",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItem_OutcomeId",
                table: "OutcomeItem",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_OutcomeItem_SkuId",
                table: "OutcomeItem",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Id",
                table: "Projects",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                schema: "Identity",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Identity",
                table: "Roles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sku_ManufacturerId",
                table: "Sku",
                column: "ManufacturerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sku_MeasurementUnitId",
                table: "Sku",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Sku_ProductId",
                table: "Sku",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Sku_SupplierId",
                table: "Sku",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Stock_ParentId",
                table: "Stock",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegister_MeasurementUnitId",
                table: "StockBalanceRegister",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegister_ProductId",
                table: "StockBalanceRegister",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegister_SkuId",
                table: "StockBalanceRegister",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_StockBalanceRegister_StockId",
                table: "StockBalanceRegister",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemand_Id",
                table: "StockDemand",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemand_OutcomeId",
                table: "StockDemand",
                column: "OutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemand_RecipientId",
                table: "StockDemand",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemand_SenderId",
                table: "StockDemand",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItem_Id",
                table: "StockDemandItem",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItem_MeasurementUnitId",
                table: "StockDemandItem",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItem_ProductId",
                table: "StockDemandItem",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockDemandItem_StockDemandId",
                table: "StockDemandItem",
                column: "StockDemandId");

            migrationBuilder.CreateIndex(
                name: "IX_StockSku_Id",
                table: "StockSku",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StockSku_MeasurementUnitId",
                table: "StockSku",
                column: "MeasurementUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StockSku_SkuId",
                table: "StockSku",
                column: "SkuId");

            migrationBuilder.CreateIndex(
                name: "IX_StockSku_StockId",
                table: "StockSku",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_Supplier_Id",
                table: "Supplier",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                schema: "Identity",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                schema: "Identity",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                schema: "Identity",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Identity",
                table: "Users",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_BrigadeId",
                table: "WorkLogs",
                column: "BrigadeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_BrigadePaymentId",
                table: "WorkLogs",
                column: "BrigadePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_EstimateItemId",
                table: "WorkLogs",
                column: "EstimateItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_Id",
                table: "WorkLogs",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientActItems");

            migrationBuilder.DropTable(
                name: "ClientPayments");

            migrationBuilder.DropTable(
                name: "IncomeItem");

            migrationBuilder.DropTable(
                name: "IncomeOutcome");

            migrationBuilder.DropTable(
                name: "OrganizationMeasurementUnit");

            migrationBuilder.DropTable(
                name: "OrganizationSku");

            migrationBuilder.DropTable(
                name: "OutcomeItem");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RoleClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "StockBalanceRegister");

            migrationBuilder.DropTable(
                name: "StockDemandItem");

            migrationBuilder.DropTable(
                name: "StockSku");

            migrationBuilder.DropTable(
                name: "UserClaims",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserLogins",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserTokens",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "ClientActs");

            migrationBuilder.DropTable(
                name: "Income");

            migrationBuilder.DropTable(
                name: "StockDemand");

            migrationBuilder.DropTable(
                name: "Sku");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "BrigadePayments");

            migrationBuilder.DropTable(
                name: "EstimateItems");

            migrationBuilder.DropTable(
                name: "Outcome");

            migrationBuilder.DropTable(
                name: "Manufacturer");

            migrationBuilder.DropTable(
                name: "MeasurementUnits");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Brigades");

            migrationBuilder.DropTable(
                name: "EstimateSections");

            migrationBuilder.DropTable(
                name: "Stock");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
