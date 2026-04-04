using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class AdmissionApplicationConfiguration : IEntityTypeConfiguration<AdmissionApplication>
{
    public void Configure(EntityTypeBuilder<AdmissionApplication> builder)
    {
        builder.ToTable("AdmissionApplications");

        builder.HasKey(application => application.Id);

        builder.Property(application => application.ApplicationNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(application => application.StudentFirstName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(application => application.StudentLastName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(application => application.Gender)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(application => application.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(application => application.RegistrationFee)
            .HasPrecision(12, 2)
            .IsRequired();

        builder.HasIndex(application => application.ApplicationNumber)
            .IsUnique();

        builder.HasOne(application => application.Campus)
            .WithMany(campus => campus.AdmissionApplications)
            .HasForeignKey(application => application.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(application => application.AcademicYear)
            .WithMany(academicYear => academicYear.AdmissionApplications)
            .HasForeignKey(application => application.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(application => application.SchoolClass)
            .WithMany(schoolClass => schoolClass.AdmissionApplications)
            .HasForeignKey(application => application.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(application => application.Section)
            .WithMany(section => section.AdmissionApplications)
            .HasForeignKey(application => application.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(application => application.Guardian)
            .WithMany(guardian => guardian.AdmissionApplications)
            .HasForeignKey(application => application.GuardianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new AdmissionApplication(
                1,
                1,
                1,
                1,
                1,
                "ADM-2026-001",
                "Aarav",
                "Sharma",
                new DateOnly(2020, 8, 12),
                "Male",
                "New",
                new DateOnly(2026, 3, 25),
                1500m)
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 3, 25, 0, 0, 0, DateTimeKind.Local)
            },
            new AdmissionApplication(
                1,
                1,
                1,
                2,
                2,
                "ADM-2026-002",
                "Ishita",
                "Verma",
                new DateOnly(2020, 11, 3),
                "Female",
                "Approved",
                new DateOnly(2026, 3, 22),
                1500m)
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 3, 22, 0, 0, 0, DateTimeKind.Local)
            },
            new AdmissionApplication(
                2,
                2,
                3,
                4,
                3,
                "ADM-2026-003",
                "Diya",
                "Nair",
                new DateOnly(2020, 4, 19),
                "Female",
                "Waitlisted",
                new DateOnly(2026, 3, 27),
                1500m)
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 3, 27, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
