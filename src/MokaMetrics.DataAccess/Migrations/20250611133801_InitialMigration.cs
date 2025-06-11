using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MokaMetrics.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    Country = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    ZipCode = table.Column<string>(type: "varchar", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "varchar", maxLength: 20, nullable: true),
                    FiscalId = table.Column<string>(type: "varchar", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IndustrialFacilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    Country = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    ZipCode = table.Column<string>(type: "varchar", maxLength: 20, nullable: true),
                    City = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "varchar", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustrialFacilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    QuantityMachines = table.Column<int>(type: "integer", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Deadline = table.Column<DateTime>(type: "timestamp", nullable: true),
                    FullfilledDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Machines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Model = table.Column<string>(type: "varchar", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IndustrialFacilityId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Machines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Machines_IndustrialFacilities_IndustrialFacilityId",
                        column: x => x.IndustrialFacilityId,
                        principalTable: "IndustrialFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OrderId = table.Column<int>(type: "integer", nullable: false),
                    TotalQuantity = table.Column<int>(type: "integer", nullable: false),
                    ManufacturedQuantity = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp", nullable: true),
                    IndustrialFacilityId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lots_IndustrialFacilities_IndustrialFacilityId",
                        column: x => x.IndustrialFacilityId,
                        principalTable: "IndustrialFacilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lots_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MachineActivityStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MachineId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ErrorMessage = table.Column<string>(type: "varchar", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineActivityStatuses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineActivityStatuses_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_IndustrialFacilityId",
                table: "Lots",
                column: "IndustrialFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Lots_OrderId",
                table: "Lots",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineActivityStatuses_MachineId",
                table: "MachineActivityStatuses",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_IndustrialFacilityId",
                table: "Machines",
                column: "IndustrialFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "MachineActivityStatuses");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Machines");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "IndustrialFacilities");
        }
    }
}
