using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    public partial class AddTransportEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransportRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RouteName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportRoutes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportVehicles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    AssignedRouteId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportVehicles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransportVehicles_TransportRoutes_AssignedRouteId",
                        column: x => x.AssignedRouteId,
                        principalTable: "TransportRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "TransportRoutes",
                columns: new[] { "Id", "CreatedAt", "Destination", "Origin", "RouteName", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), "North Campus", "Main Campus", "North Campus Shuttle", "Active", null },
                    { 2, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), "East Campus", "Main Campus", "East Campus Express", "Active", null },
                    { 3, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), "Main Campus", "Downtown", "City School Link", "Inactive", null }
                });

            migrationBuilder.InsertData(
                table: "TransportVehicles",
                columns: new[] { "Id", "AssignedRouteId", "Capacity", "CreatedAt", "Status", "UpdatedAt", "VehicleNumber", "VehicleType" },
                values: new object[,]
                {
                    { 1, 1, 50, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), "On Route", null, "ERP-101", "Bus" },
                    { 2, 2, 18, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), "On Route", null, "ERP-102", "Van" },
                    { 3, 3, 52, new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local), "Idle", null, "ERP-103", "Bus" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransportVehicles_AssignedRouteId",
                table: "TransportVehicles",
                column: "AssignedRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_TransportVehicles_VehicleNumber",
                table: "TransportVehicles",
                column: "VehicleNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransportVehicles");

            migrationBuilder.DropTable(
                name: "TransportRoutes");
        }
    }
}
