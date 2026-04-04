using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class ExamScheduleConfiguration : IEntityTypeConfiguration<ExamSchedule>
{
    public void Configure(EntityTypeBuilder<ExamSchedule> builder)
    {
        builder.ToTable("ExamSchedules");

        builder.HasKey(schedule => schedule.Id);

        builder.HasIndex(schedule => new { schedule.ExamTermId, schedule.SchoolClassId, schedule.SectionId, schedule.SubjectId })
            .IsUnique();

        builder.HasOne(schedule => schedule.ExamTerm)
            .WithMany(term => term.ExamSchedules)
            .HasForeignKey(schedule => schedule.ExamTermId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(schedule => schedule.SchoolClass)
            .WithMany()
            .HasForeignKey(schedule => schedule.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(schedule => schedule.Section)
            .WithMany()
            .HasForeignKey(schedule => schedule.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(schedule => schedule.Subject)
            .WithMany(subject => subject.ExamSchedules)
            .HasForeignKey(schedule => schedule.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            CreateSchedule(1, 1, 2, 1, new DateOnly(2026, 9, 14), "09:00", 90, 100, 35),
            CreateSchedule(2, 1, 2, 2, new DateOnly(2026, 9, 15), "09:00", 90, 100, 35),
            CreateSchedule(3, 1, 2, 3, new DateOnly(2026, 9, 16), "09:00", 90, 100, 35));
    }

    private static ExamSchedule CreateSchedule(
        int id,
        int schoolClassId,
        int sectionId,
        int subjectId,
        DateOnly examDate,
        string startTime,
        int durationMinutes,
        int maxMarks,
        int passMarks)
    {
        return new ExamSchedule(
            1,
            schoolClassId,
            sectionId,
            subjectId,
            examDate,
            TimeOnly.Parse(startTime),
            durationMinutes,
            maxMarks,
            passMarks)
        {
            Id = id,
            CreatedAt = new DateTime(2026, 4, 4, 12, 15, 0, DateTimeKind.Local)
        };
    }
}
