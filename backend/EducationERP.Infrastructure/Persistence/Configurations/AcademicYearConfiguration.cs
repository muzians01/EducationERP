using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class AcademicYearConfiguration : IEntityTypeConfiguration<AcademicYear>
{
    public void Configure(EntityTypeBuilder<AcademicYear> builder)
    {
        builder.ToTable("AcademicYears");

        builder.HasKey(academicYear => academicYear.Id);

        builder.Property(academicYear => academicYear.Name)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(academicYear => academicYear.IsActive)
            .IsRequired();

        builder.HasIndex(academicYear => new { academicYear.CampusId, academicYear.Name })
            .IsUnique();

        builder.HasOne(academicYear => academicYear.Campus)
            .WithMany(campus => campus.AcademicYears)
            .HasForeignKey(academicYear => academicYear.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AcademicYear(
                1,
                "2026-2027",
                new DateOnly(2026, 6, 1),
                new DateOnly(2027, 3, 31),
                true)
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new AcademicYear(
                2,
                "2026-2027",
                new DateOnly(2026, 6, 1),
                new DateOnly(2027, 3, 31),
                true)
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
