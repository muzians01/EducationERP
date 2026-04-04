using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("Students");

        builder.HasKey(student => student.Id);

        builder.Property(student => student.AdmissionNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(student => student.FirstName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(student => student.LastName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(student => student.Gender)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(student => student.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(student => student.PrimaryContactNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(student => student.AddressLine)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(student => student.City)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(student => student.State)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(student => student.PostalCode)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(student => student.BloodGroup)
            .HasMaxLength(10);

        builder.Property(student => student.MedicalNotes)
            .HasMaxLength(200);

        builder.HasIndex(student => student.AdmissionNumber)
            .IsUnique();

        builder.HasOne(student => student.Campus)
            .WithMany(campus => campus.Students)
            .HasForeignKey(student => student.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(student => student.AcademicYear)
            .WithMany(academicYear => academicYear.Students)
            .HasForeignKey(student => student.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(student => student.SchoolClass)
            .WithMany(schoolClass => schoolClass.Students)
            .HasForeignKey(student => student.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(student => student.Section)
            .WithMany(section => section.Students)
            .HasForeignKey(student => student.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(student => student.Guardian)
            .WithMany(guardian => guardian.Students)
            .HasForeignKey(student => student.GuardianId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(student => student.AdmissionApplication)
            .WithMany(application => application.Students)
            .HasForeignKey(student => student.AdmissionApplicationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Student(
                1,
                1,
                1,
                2,
                2,
                2,
                "STU-2026-001",
                "Ishita",
                "Verma",
                new DateOnly(2020, 11, 3),
                "Female",
                new DateOnly(2026, 4, 1),
                "Active",
                "9876500002",
                "45 Green Residency, Maple Street",
                "Bengaluru",
                "Karnataka",
                "560001",
                "B+",
                "Dust allergy")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
