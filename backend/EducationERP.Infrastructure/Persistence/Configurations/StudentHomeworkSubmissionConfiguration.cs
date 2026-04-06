using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class StudentHomeworkSubmissionConfiguration : IEntityTypeConfiguration<StudentHomeworkSubmission>
{
    public void Configure(EntityTypeBuilder<StudentHomeworkSubmission> builder)
    {
        builder.ToTable("StudentHomeworkSubmissions");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(item => item.Remarks)
            .HasMaxLength(160);

        builder.HasIndex(item => new { item.HomeworkAssignmentId, item.StudentId })
            .IsUnique();

        builder.HasOne(item => item.HomeworkAssignment)
            .WithMany(assignment => assignment.StudentSubmissions)
            .HasForeignKey(item => item.HomeworkAssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(item => item.Student)
            .WithMany()
            .HasForeignKey(item => item.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            CreateSubmission(1, 1, 1, "Assigned", null, "Yet to be submitted"),
            CreateSubmission(2, 2, 1, "Submitted", new DateOnly(2026, 4, 6), "Completed neatly"),
            CreateSubmission(3, 3, 1, "Reviewed", new DateOnly(2026, 4, 6), "Good observation detail"));
    }

    private static StudentHomeworkSubmission CreateSubmission(int id, int homeworkAssignmentId, int studentId, string status, DateOnly? submittedOn, string remarks)
    {
        return new StudentHomeworkSubmission(homeworkAssignmentId, studentId, status, submittedOn, remarks)
        {
            Id = id,
            CreatedAt = new DateTime(2026, 4, 6, 9, 15, 0, DateTimeKind.Local)
        };
    }
}
