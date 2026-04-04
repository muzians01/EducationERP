using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class FeeStructureConfiguration : IEntityTypeConfiguration<FeeStructure>
{
    public void Configure(EntityTypeBuilder<FeeStructure> builder)
    {
        builder.ToTable("FeeStructures");

        builder.HasKey(feeStructure => feeStructure.Id);

        builder.Property(feeStructure => feeStructure.FeeCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(feeStructure => feeStructure.FeeName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(feeStructure => feeStructure.BillingCycle)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(feeStructure => feeStructure.Amount)
            .HasPrecision(12, 2);

        builder.HasIndex(feeStructure => new { feeStructure.CampusId, feeStructure.AcademicYearId, feeStructure.SchoolClassId, feeStructure.FeeCode })
            .IsUnique();

        builder.HasOne(feeStructure => feeStructure.Campus)
            .WithMany(campus => campus.FeeStructures)
            .HasForeignKey(feeStructure => feeStructure.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(feeStructure => feeStructure.AcademicYear)
            .WithMany(academicYear => academicYear.FeeStructures)
            .HasForeignKey(feeStructure => feeStructure.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(feeStructure => feeStructure.SchoolClass)
            .WithMany(schoolClass => schoolClass.FeeStructures)
            .HasForeignKey(feeStructure => feeStructure.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new FeeStructure(1, 1, 1, "TUITION", "Tuition Fee", 48000m, "Quarterly")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 1, 8, 0, 0, DateTimeKind.Local)
            },
            new FeeStructure(1, 1, 1, "TRANSPORT", "Transport Fee", 12000m, "Quarterly")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 1, 8, 15, 0, DateTimeKind.Local)
            },
            new FeeStructure(2, 2, 3, "TUITION", "Tuition Fee", 42000m, "Quarterly")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 4, 1, 8, 30, 0, DateTimeKind.Local)
            });
    }
}
