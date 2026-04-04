using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentProfilesAndDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressLine",
                table: "Students",
                type: "nvarchar(160)",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BloodGroup",
                table: "Students",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Students",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicalNotes",
                table: "Students",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryContactNumber",
                table: "Students",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Students",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StudentDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    SubmittedOn = table.Column<DateOnly>(type: "date", nullable: false),
                    VerifiedOn = table.Column<DateOnly>(type: "date", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentDocuments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "StudentDocuments",
                columns: new[] { "Id", "CreatedAt", "DocumentType", "Remarks", "Status", "StudentId", "SubmittedOn", "UpdatedAt", "VerifiedOn" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 1, 9, 0, 0, 0, DateTimeKind.Local), "Birth Certificate", "Original verified at front desk", "Verified", 1, new DateOnly(2026, 4, 1), null, new DateOnly(2026, 4, 1) },
                    { 2, new DateTime(2026, 4, 1, 9, 30, 0, 0, DateTimeKind.Local), "Transfer Certificate", "Awaiting previous school submission", "Pending", 1, new DateOnly(2026, 4, 1), null, null },
                    { 3, new DateTime(2026, 4, 2, 10, 15, 0, 0, DateTimeKind.Local), "Immunization Record", "Uploaded by guardian and reviewed", "Verified", 1, new DateOnly(2026, 4, 2), null, new DateOnly(2026, 4, 2) }
                });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AddressLine", "BloodGroup", "City", "Gender", "MedicalNotes", "PostalCode", "PrimaryContactNumber", "State" },
                values: new object[] { "45 Green Residency, Maple Street", "B+", "Bengaluru", "Female", "Dust allergy", "560001", "9876500002", "Karnataka" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDocuments_StudentId",
                table: "StudentDocuments",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentDocuments");

            migrationBuilder.DropColumn(
                name: "AddressLine",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "BloodGroup",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "MedicalNotes",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "PrimaryContactNumber",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Students");
        }
    }
}
