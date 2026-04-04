using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SchoolHolidays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    HolidayDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolHolidays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolHolidays_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentLeaveRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    LeaveDate = table.Column<DateOnly>(type: "date", nullable: false),
                    LeaveType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentLeaveRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentLeaveRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "SchoolHolidays",
                columns: new[] { "Id", "CampusId", "Category", "CreatedAt", "HolidayDate", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "National Holiday", new DateTime(2026, 4, 5, 9, 0, 0, 0, DateTimeKind.Local), new DateOnly(2026, 4, 14), "Ambedkar Jayanti", null },
                    { 2, 1, "School Event", new DateTime(2026, 4, 5, 9, 10, 0, 0, DateTimeKind.Local), new DateOnly(2026, 4, 18), "PTM Preparation Day", null }
                });

            migrationBuilder.InsertData(
                table: "StudentLeaveRequests",
                columns: new[] { "Id", "CreatedAt", "LeaveDate", "LeaveType", "Reason", "Status", "StudentId", "UpdatedAt" },
                values: new object[] { 1, new DateTime(2026, 4, 3, 7, 30, 0, 0, DateTimeKind.Local), new DateOnly(2026, 4, 3), "Sick Leave", "Fever and rest advised", "Approved", 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolHolidays_CampusId_HolidayDate_Title",
                table: "SchoolHolidays",
                columns: new[] { "CampusId", "HolidayDate", "Title" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentLeaveRequests_StudentId_LeaveDate",
                table: "StudentLeaveRequests",
                columns: new[] { "StudentId", "LeaveDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolHolidays");

            migrationBuilder.DropTable(
                name: "StudentLeaveRequests");
        }
    }
}
