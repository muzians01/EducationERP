using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAcademicsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    WeeklyPeriods = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimetablePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    PeriodNumber = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    TeacherName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimetablePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimetablePeriods_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetablePeriods_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetablePeriods_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TimetablePeriods_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "CampusId", "Category", "Code", "CreatedAt", "Name", "UpdatedAt", "WeeklyPeriods" },
                values: new object[,]
                {
                    { 1, 1, "Language", "ENG", new DateTime(2026, 4, 4, 8, 0, 0, 0, DateTimeKind.Local), "English", null, 6 },
                    { 2, 1, "Core", "MAT", new DateTime(2026, 4, 4, 8, 0, 0, 0, DateTimeKind.Local), "Mathematics", null, 7 },
                    { 3, 1, "Core", "SCI", new DateTime(2026, 4, 4, 8, 0, 0, 0, DateTimeKind.Local), "Science", null, 5 },
                    { 4, 1, "Humanities", "SST", new DateTime(2026, 4, 4, 8, 0, 0, 0, DateTimeKind.Local), "Social Studies", null, 4 },
                    { 5, 1, "Activity", "ART", new DateTime(2026, 4, 4, 8, 0, 0, 0, DateTimeKind.Local), "Art & Craft", null, 2 }
                });

            migrationBuilder.InsertData(
                table: "TimetablePeriods",
                columns: new[] { "Id", "AcademicYearId", "CreatedAt", "DayOfWeek", "EndTime", "PeriodNumber", "RoomNumber", "SchoolClassId", "SectionId", "StartTime", "SubjectId", "TeacherName", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 1, new TimeOnly(9, 10, 0), 1, "G1-B01", 1, 2, new TimeOnly(8, 30, 0), 1, "Anita Rao", null },
                    { 2, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 1, new TimeOnly(9, 50, 0), 2, "G1-B01", 1, 2, new TimeOnly(9, 10, 0), 2, "Rahul Mehta", null },
                    { 3, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 1, new TimeOnly(10, 45, 0), 3, "SCIENCE LAB", 1, 2, new TimeOnly(10, 5, 0), 3, "Priya Nair", null },
                    { 4, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 2, new TimeOnly(9, 10, 0), 1, "G1-B01", 1, 2, new TimeOnly(8, 30, 0), 2, "Rahul Mehta", null },
                    { 5, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 2, new TimeOnly(9, 50, 0), 2, "G1-B01", 1, 2, new TimeOnly(9, 10, 0), 1, "Anita Rao", null },
                    { 6, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 2, new TimeOnly(10, 45, 0), 3, "G1-B01", 1, 2, new TimeOnly(10, 5, 0), 4, "Kavya Iyer", null },
                    { 7, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 3, new TimeOnly(9, 10, 0), 1, "SCIENCE LAB", 1, 2, new TimeOnly(8, 30, 0), 3, "Priya Nair", null },
                    { 8, 1, new DateTime(2026, 4, 4, 8, 15, 0, 0, DateTimeKind.Local), 3, new TimeOnly(9, 50, 0), 2, "ART ROOM", 1, 2, new TimeOnly(9, 10, 0), 5, "Neha Kapoor", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CampusId_Code",
                table: "Subjects",
                columns: new[] { "CampusId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetablePeriods_AcademicYearId_SchoolClassId_SectionId_DayOfWeek_PeriodNumber",
                table: "TimetablePeriods",
                columns: new[] { "AcademicYearId", "SchoolClassId", "SectionId", "DayOfWeek", "PeriodNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimetablePeriods_SchoolClassId",
                table: "TimetablePeriods",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetablePeriods_SectionId",
                table: "TimetablePeriods",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_TimetablePeriods_SubjectId",
                table: "TimetablePeriods",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TimetablePeriods");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
