using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteFeesAndAddAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AttendanceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MarkedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeeConcessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentFeeId = table.Column<int>(type: "int", nullable: false),
                    ConcessionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ApprovedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    ApprovedBy = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeConcessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeConcessions_StudentFees_StudentFeeId",
                        column: x => x.StudentFeeId,
                        principalTable: "StudentFees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AttendanceRecords",
                columns: new[] { "Id", "AttendanceDate", "CreatedAt", "MarkedOn", "Remarks", "Status", "StudentId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 4, 1), new DateTime(2026, 4, 1, 8, 5, 0, 0, DateTimeKind.Local), new DateTime(2026, 4, 1, 8, 5, 0, 0, DateTimeKind.Local), null, "Present", 1, null },
                    { 2, new DateOnly(2026, 4, 2), new DateTime(2026, 4, 2, 8, 20, 0, 0, DateTimeKind.Local), new DateTime(2026, 4, 2, 8, 20, 0, 0, DateTimeKind.Local), "Arrived after assembly", "Late", 1, null },
                    { 3, new DateOnly(2026, 4, 3), new DateTime(2026, 4, 3, 8, 30, 0, 0, DateTimeKind.Local), new DateTime(2026, 4, 3, 8, 30, 0, 0, DateTimeKind.Local), "Parent informed class teacher", "Absent", 1, null }
                });

            migrationBuilder.InsertData(
                table: "FeeConcessions",
                columns: new[] { "Id", "Amount", "ApprovedBy", "ApprovedOn", "ConcessionType", "CreatedAt", "Remarks", "StudentFeeId", "UpdatedAt" },
                values: new object[] { 1, 6000m, "Bursar", new DateOnly(2026, 4, 4), "Sibling Concession", new DateTime(2026, 4, 4, 10, 0, 0, 0, DateTimeKind.Local), "Approved for sibling admission", 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_StudentId_AttendanceDate",
                table: "AttendanceRecords",
                columns: new[] { "StudentId", "AttendanceDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeConcessions_StudentFeeId",
                table: "FeeConcessions",
                column: "StudentFeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "FeeConcessions");
        }
    }
}
