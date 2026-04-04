using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameUtcColumnsToLocalTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Students",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Students",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Sections",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Sections",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Guardians",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Guardians",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Classes",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Classes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "Campuses",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "Campuses",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "AdmissionApplications",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "AdmissionApplications",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "UpdatedUtc",
                table: "AcademicYears",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "AcademicYears",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Students",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Students",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Sections",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Sections",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Guardians",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Guardians",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Classes",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Classes",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Campuses",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Campuses",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "AdmissionApplications",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AdmissionApplications",
                newName: "CreatedUtc");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "AcademicYears",
                newName: "UpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AcademicYears",
                newName: "CreatedUtc");
        }
    }
}
