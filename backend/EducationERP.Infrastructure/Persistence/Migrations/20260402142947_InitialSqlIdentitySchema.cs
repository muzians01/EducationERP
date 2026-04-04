using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialSqlIdentitySchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Campuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    State = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    BoardAffiliation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AcademicYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcademicYears", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AcademicYears_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classes_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Guardians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Relationship = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guardians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Guardians_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    ApplicationNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StudentFirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    StudentLastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    AppliedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    RegistrationFee = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionApplications_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionApplications_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionApplications_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionApplications_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AdmissionApplications_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SectionId = table.Column<int>(type: "int", nullable: false),
                    GuardianId = table.Column<int>(type: "int", nullable: false),
                    AdmissionApplicationId = table.Column<int>(type: "int", nullable: false),
                    AdmissionNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    EnrolledOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_AdmissionApplications_AdmissionApplicationId",
                        column: x => x.AdmissionApplicationId,
                        principalTable: "AdmissionApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Guardians_GuardianId",
                        column: x => x.GuardianId,
                        principalTable: "Guardians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Campuses",
                columns: new[] { "Id", "BoardAffiliation", "City", "Code", "Country", "CreatedUtc", "Name", "State", "UpdatedUtc" },
                values: new object[,]
                {
                    { 1, "CBSE", "Bengaluru", "HQ", "India", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Greenfield Public School", "Karnataka", null },
                    { 2, "ICSE", "Mysuru", "WEST", "India", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Greenfield West Campus", "Karnataka", null }
                });

            migrationBuilder.InsertData(
                table: "AcademicYears",
                columns: new[] { "Id", "CampusId", "CreatedUtc", "EndDate", "IsActive", "Name", "StartDate", "UpdatedUtc" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2027, 3, 31), true, "2026-2027", new DateOnly(2026, 6, 1), null },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2027, 3, 31), true, "2026-2027", new DateOnly(2026, 6, 1), null }
                });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "CampusId", "Code", "CreatedUtc", "DisplayOrder", "Name", "UpdatedUtc" },
                values: new object[,]
                {
                    { 1, 1, "GRADE-1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Grade 1", null },
                    { 2, 1, "GRADE-2", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, "Grade 2", null },
                    { 3, 2, "GRADE-1", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, "Grade 1", null }
                });

            migrationBuilder.InsertData(
                table: "Guardians",
                columns: new[] { "Id", "CampusId", "CreatedUtc", "Email", "FullName", "Occupation", "PhoneNumber", "Relationship", "UpdatedUtc" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "ananya.sharma@example.com", "Ananya Sharma", "Architect", "9876500001", "Mother", null },
                    { 2, 1, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "rahul.verma@example.com", "Rahul Verma", "Business Owner", "9876500002", "Father", null },
                    { 3, 2, new DateTime(2026, 1, 2, 0, 0, 0, 0, DateTimeKind.Utc), "meera.nair@example.com", "Meera Nair", "Doctor", "9876500003", "Mother", null }
                });

            migrationBuilder.InsertData(
                table: "Sections",
                columns: new[] { "Id", "Capacity", "CreatedUtc", "Name", "RoomNumber", "SchoolClassId", "UpdatedUtc" },
                values: new object[,]
                {
                    { 1, 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A", "G1-A01", 1, null },
                    { 2, 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "B", "G1-B01", 1, null },
                    { 3, 35, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A", "G2-A01", 2, null },
                    { 4, 30, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "A", "W-G1-A01", 3, null }
                });

            migrationBuilder.InsertData(
                table: "AdmissionApplications",
                columns: new[] { "Id", "AcademicYearId", "ApplicationNumber", "AppliedOn", "CampusId", "CreatedUtc", "DateOfBirth", "Gender", "GuardianId", "RegistrationFee", "SchoolClassId", "SectionId", "Status", "StudentFirstName", "StudentLastName", "UpdatedUtc" },
                values: new object[,]
                {
                    { 1, 1, "ADM-2026-001", new DateOnly(2026, 3, 25), 1, new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2020, 8, 12), "Male", 1, 1500m, 1, 1, "New", "Aarav", "Sharma", null },
                    { 2, 1, "ADM-2026-002", new DateOnly(2026, 3, 22), 1, new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2020, 11, 3), "Female", 2, 1500m, 1, 2, "Approved", "Ishita", "Verma", null },
                    { 3, 2, "ADM-2026-003", new DateOnly(2026, 3, 27), 2, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2020, 4, 19), "Female", 3, 1500m, 3, 4, "Waitlisted", "Diya", "Nair", null }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "AcademicYearId", "AdmissionApplicationId", "AdmissionNumber", "CampusId", "CreatedUtc", "DateOfBirth", "EnrolledOn", "FirstName", "GuardianId", "LastName", "SchoolClassId", "SectionId", "Status", "UpdatedUtc" },
                values: new object[] { 1, 1, 2, "STU-2026-001", 1, new DateTime(2026, 4, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2020, 11, 3), new DateOnly(2026, 4, 1), "Ishita", 2, "Verma", 1, 2, "Active", null });

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_CampusId_Name",
                table: "AcademicYears",
                columns: new[] { "CampusId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_AcademicYearId",
                table: "AdmissionApplications",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_ApplicationNumber",
                table: "AdmissionApplications",
                column: "ApplicationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_CampusId",
                table: "AdmissionApplications",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_GuardianId",
                table: "AdmissionApplications",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_SchoolClassId",
                table: "AdmissionApplications",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionApplications_SectionId",
                table: "AdmissionApplications",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_Code",
                table: "Campuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_CampusId_Code",
                table: "Classes",
                columns: new[] { "CampusId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_CampusId",
                table: "Guardians",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Guardians_Email",
                table: "Guardians",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_SchoolClassId_Name",
                table: "Sections",
                columns: new[] { "SchoolClassId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_AcademicYearId",
                table: "Students",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AdmissionApplicationId",
                table: "Students",
                column: "AdmissionApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_AdmissionNumber",
                table: "Students",
                column: "AdmissionNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_CampusId",
                table: "Students",
                column: "CampusId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_GuardianId",
                table: "Students",
                column: "GuardianId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SchoolClassId",
                table: "Students",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SectionId",
                table: "Students",
                column: "SectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "AdmissionApplications");

            migrationBuilder.DropTable(
                name: "AcademicYears");

            migrationBuilder.DropTable(
                name: "Guardians");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Campuses");
        }
    }
}
