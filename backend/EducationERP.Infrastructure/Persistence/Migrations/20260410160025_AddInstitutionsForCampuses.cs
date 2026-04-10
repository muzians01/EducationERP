using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInstitutionsForCampuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Campuses_Code",
                table: "Campuses");

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "Campuses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    City = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    State = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Institutions",
                columns: new[] { "Id", "City", "Code", "Country", "CreatedAt", "Name", "State", "UpdatedAt" },
                values: new object[] { 1, "Bengaluru", "GF-EDU", "India", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Local), "Greenfield Education Trust", "Karnataka", null });

            migrationBuilder.Sql("""
                UPDATE [Campuses]
                SET [InstitutionId] = 1
                WHERE [InstitutionId] = 0;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_InstitutionId_Code",
                table: "Campuses",
                columns: new[] { "InstitutionId", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_Code",
                table: "Institutions",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Campuses_Institutions_InstitutionId",
                table: "Campuses",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Campuses_Institutions_InstitutionId",
                table: "Campuses");

            migrationBuilder.DropTable(
                name: "Institutions");

            migrationBuilder.DropIndex(
                name: "IX_Campuses_InstitutionId_Code",
                table: "Campuses");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "Campuses");

            migrationBuilder.CreateIndex(
                name: "IX_Campuses_Code",
                table: "Campuses",
                column: "Code",
                unique: true);
        }
    }
}
