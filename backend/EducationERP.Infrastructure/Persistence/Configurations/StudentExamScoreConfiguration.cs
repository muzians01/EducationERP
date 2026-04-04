using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class StudentExamScoreConfiguration : IEntityTypeConfiguration<StudentExamScore>
{
    public void Configure(EntityTypeBuilder<StudentExamScore> builder)
    {
        builder.ToTable("StudentExamScores");

        builder.HasKey(score => score.Id);

        builder.Property(score => score.MarksObtained)
            .HasColumnType("decimal(5,2)");

        builder.Property(score => score.Grade)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(score => score.ResultStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(score => score.Remarks)
            .HasMaxLength(160);

        builder.HasIndex(score => new { score.ExamScheduleId, score.StudentId })
            .IsUnique();

        builder.HasOne(score => score.ExamSchedule)
            .WithMany(schedule => schedule.StudentExamScores)
            .HasForeignKey(score => score.ExamScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(score => score.Student)
            .WithMany()
            .HasForeignKey(score => score.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            CreateScore(1, 1, 86m, "A", "Pass", "Strong comprehension"),
            CreateScore(2, 1, 91m, "A+", "Pass", "Excellent problem solving"),
            CreateScore(3, 1, 78m, "B+", "Pass", "Good scientific reasoning"));
    }

    private static StudentExamScore CreateScore(int examScheduleId, int studentId, decimal marksObtained, string grade, string resultStatus, string remarks)
    {
        return new StudentExamScore(examScheduleId, studentId, marksObtained, grade, resultStatus, remarks)
        {
            Id = examScheduleId,
            CreatedAt = new DateTime(2026, 4, 4, 12, 30, 0, DateTimeKind.Local)
        };
    }
}
