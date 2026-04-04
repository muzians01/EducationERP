using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EducationERP.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFeesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeeStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampusId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    FeeCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    BillingCycle = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeStructures_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeStructures_Campuses_CampusId",
                        column: x => x.CampusId,
                        principalTable: "Campuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeeStructures_Classes_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentFees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    FeeStructureId = table.Column<int>(type: "int", nullable: false),
                    DueOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentFees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentFees_FeeStructures_FeeStructureId",
                        column: x => x.FeeStructureId,
                        principalTable: "FeeStructures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentFees_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeePayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentFeeId = table.Column<int>(type: "int", nullable: false),
                    PaymentReference = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PaidOn = table.Column<DateOnly>(type: "date", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeePayments_StudentFees_StudentFeeId",
                        column: x => x.StudentFeeId,
                        principalTable: "StudentFees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FeePayments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "FeeStructures",
                columns: new[] { "Id", "AcademicYearId", "Amount", "BillingCycle", "CampusId", "CreatedAt", "FeeCode", "FeeName", "SchoolClassId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, 48000m, "Quarterly", 1, new DateTime(2026, 4, 1, 8, 0, 0, 0, DateTimeKind.Local), "TUITION", "Tuition Fee", 1, null },
                    { 2, 1, 12000m, "Quarterly", 1, new DateTime(2026, 4, 1, 8, 15, 0, 0, DateTimeKind.Local), "TRANSPORT", "Transport Fee", 1, null },
                    { 3, 2, 42000m, "Quarterly", 2, new DateTime(2026, 4, 1, 8, 30, 0, 0, DateTimeKind.Local), "TUITION", "Tuition Fee", 3, null }
                });

            migrationBuilder.InsertData(
                table: "StudentFees",
                columns: new[] { "Id", "Amount", "AmountPaid", "CreatedAt", "DueOn", "FeeStructureId", "Status", "StudentId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 48000m, 24000m, new DateTime(2026, 4, 2, 9, 0, 0, 0, DateTimeKind.Local), new DateOnly(2026, 6, 10), 1, "Partially Paid", 1, null },
                    { 2, 12000m, 0m, new DateTime(2026, 4, 2, 9, 10, 0, 0, DateTimeKind.Local), new DateOnly(2026, 6, 10), 2, "Pending", 1, null }
                });

            migrationBuilder.InsertData(
                table: "FeePayments",
                columns: new[] { "Id", "Amount", "CreatedAt", "PaidOn", "PaymentMethod", "PaymentReference", "Status", "StudentFeeId", "StudentId", "UpdatedAt" },
                values: new object[] { 1, 24000m, new DateTime(2026, 4, 3, 11, 0, 0, 0, DateTimeKind.Local), new DateOnly(2026, 4, 3), "UPI", "RCPT-2026-001", "Posted", 1, 1, null });

            migrationBuilder.CreateIndex(
                name: "IX_FeePayments_PaymentReference",
                table: "FeePayments",
                column: "PaymentReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeePayments_StudentFeeId",
                table: "FeePayments",
                column: "StudentFeeId");

            migrationBuilder.CreateIndex(
                name: "IX_FeePayments_StudentId",
                table: "FeePayments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_AcademicYearId",
                table: "FeeStructures",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_CampusId_AcademicYearId_SchoolClassId_FeeCode",
                table: "FeeStructures",
                columns: new[] { "CampusId", "AcademicYearId", "SchoolClassId", "FeeCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_SchoolClassId",
                table: "FeeStructures",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFees_FeeStructureId",
                table: "StudentFees",
                column: "FeeStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentFees_StudentId",
                table: "StudentFees",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeePayments");

            migrationBuilder.DropTable(
                name: "StudentFees");

            migrationBuilder.DropTable(
                name: "FeeStructures");
        }
    }
}
