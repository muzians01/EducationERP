using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class TimetablePeriodConfiguration : IEntityTypeConfiguration<TimetablePeriod>
{
    public void Configure(EntityTypeBuilder<TimetablePeriod> builder)
    {
        builder.ToTable("TimetablePeriods");

        builder.HasKey(period => period.Id);

        builder.Property(period => period.TeacherName)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(period => period.RoomNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(period => new { period.AcademicYearId, period.SchoolClassId, period.SectionId, period.DayOfWeek, period.PeriodNumber })
            .IsUnique();

        builder.HasOne(period => period.AcademicYear)
            .WithMany()
            .HasForeignKey(period => period.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(period => period.SchoolClass)
            .WithMany()
            .HasForeignKey(period => period.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(period => period.Section)
            .WithMany()
            .HasForeignKey(period => period.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(period => period.Subject)
            .WithMany(subject => subject.TimetablePeriods)
            .HasForeignKey(period => period.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            CreatePeriod(1, 2, DayOfWeek.Monday, 1, 1, "08:30", "09:10", "Anita Rao", "G1-B01"),
            CreatePeriod(2, 2, DayOfWeek.Monday, 2, 2, "09:10", "09:50", "Rahul Mehta", "G1-B01"),
            CreatePeriod(3, 2, DayOfWeek.Monday, 3, 3, "10:05", "10:45", "Priya Nair", "Science Lab"),
            CreatePeriod(4, 2, DayOfWeek.Tuesday, 1, 2, "08:30", "09:10", "Rahul Mehta", "G1-B01"),
            CreatePeriod(5, 2, DayOfWeek.Tuesday, 2, 1, "09:10", "09:50", "Anita Rao", "G1-B01"),
            CreatePeriod(6, 2, DayOfWeek.Tuesday, 3, 4, "10:05", "10:45", "Kavya Iyer", "G1-B01"),
            CreatePeriod(7, 2, DayOfWeek.Wednesday, 1, 3, "08:30", "09:10", "Priya Nair", "Science Lab"),
            CreatePeriod(8, 2, DayOfWeek.Wednesday, 2, 5, "09:10", "09:50", "Neha Kapoor", "Art Room"));
    }

    private static TimetablePeriod CreatePeriod(
        int id,
        int sectionId,
        DayOfWeek dayOfWeek,
        int periodNumber,
        int subjectId,
        string startTime,
        string endTime,
        string teacherName,
        string roomNumber)
    {
        return new TimetablePeriod(
            1,
            1,
            sectionId,
            subjectId,
            dayOfWeek,
            periodNumber,
            TimeOnly.Parse(startTime),
            TimeOnly.Parse(endTime),
            teacherName,
            roomNumber)
        {
            Id = id,
            CreatedAt = new DateTime(2026, 4, 4, 8, 15, 0, DateTimeKind.Local)
        };
    }
}
