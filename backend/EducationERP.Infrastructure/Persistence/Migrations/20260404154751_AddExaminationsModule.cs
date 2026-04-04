using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminationsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamTerms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    ExamType = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamTerms_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamTerms_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExamSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamTermId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    ExamDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    MaxMarks = table.Column<int>(type: "int", nullable: false),
                    PassMarks = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_ExamTerms_ExamTermId",
                        column: x => x.ExamTermId,
                        principalTable: "ExamTerms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExamSchedules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentExamScores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExamScheduleId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    MarksObtained = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ResultStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentExamScores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentExamScores_ExamSchedules_ExamScheduleId",
                        column: x => x.ExamScheduleId,
                        principalTable: "ExamSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentExamScores_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ExamTerms",
                columns: new[] { "Id", "AcademicYearId", "CampusId", "CreatedAt", "EndDate", "ExamType", "Name", "StartDate", "Status", "UpdatedAt" },
                values: new object[] { 1, 1, 1, new DateTime(2026, 4, 4, 12, 0, 0, 0, DateTimeKind.Local), new DateOnly(2026, 9, 18), "Scholastic", "Term 1 Assessment", new DateOnly(2026, 9, 14), "Scheduled", null });

            migrationBuilder.InsertData(
                table: "ExamSchedules",
                columns: new[] { "Id", "CreatedAt", "DurationMinutes", "ExamDate", "ExamTermId", "MaxMarks", "PassMarks", "SchoolClassId", "SectionId", "StartTime", "SubjectId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 4, 12, 15, 0, 0, DateTimeKind.Local), 90, new DateOnly(2026, 9, 14), 1, 100, 35, 1, 2, new TimeOnly(9, 0, 0), 1, null },
                    { 2, new DateTime(2026, 4, 4, 12, 15, 0, 0, DateTimeKind.Local), 90, new DateOnly(2026, 9, 15), 1, 100, 35, 1, 2, new TimeOnly(9, 0, 0), 2, null },
                    { 3, new DateTime(2026, 4, 4, 12, 15, 0, 0, DateTimeKind.Local), 90, new DateOnly(2026, 9, 16), 1, 100, 35, 1, 2, new TimeOnly(9, 0, 0), 3, null }
                });

            migrationBuilder.InsertData(
                table: "StudentExamScores",
                columns: new[] { "Id", "CreatedAt", "ExamScheduleId", "Grade", "MarksObtained", "Remarks", "ResultStatus", "StudentId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 4, 12, 30, 0, 0, DateTimeKind.Local), 1, "A", 86m, "Strong comprehension", "Pass", 1, null },
                    { 2, new DateTime(2026, 4, 4, 12, 30, 0, 0, DateTimeKind.Local), 2, "A+", 91m, "Excellent problem solving", "Pass", 1, null },
                    { 3, new DateTime(2026, 4, 4, 12, 30, 0, 0, DateTimeKind.Local), 3, "B+", 78m, "Good scientific reasoning", "Pass", 1, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_ExamTermId_SchoolClassId_SectionId_SubjectId",
                table: "ExamSchedules",
                columns: new[] { "ExamTermId", "SchoolClassId", "SectionId", "SubjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_SchoolClassId",
                table: "ExamSchedules",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_SectionId",
                table: "ExamSchedules",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSchedules_SubjectId",
                table: "ExamSchedules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTerms_AcademicYearId",
                table: "ExamTerms",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamTerms_CampusId",
                table: "ExamTerms",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentExamScores_ExamScheduleId_StudentId",
                table: "StudentExamScores",
                columns: new[] { "ExamScheduleId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentExamScores_StudentId",
                table: "StudentExamScores",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentExamScores");

            migrationBuilder.DropTable(
                name: "ExamSchedules");

            migrationBuilder.DropTable(
                name: "ExamTerms");
        }
    }
}
