using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class StudentHomeworkSubmission : BaseEntity
{
    private StudentHomeworkSubmission()
    {
    }

    public StudentHomeworkSubmission(
        int homeworkAssignmentId,
        int studentId,
        string status,
        DateOnly? submittedOn = null,
        string? remarks = null)
    {
        HomeworkAssignmentId = homeworkAssignmentId;
        StudentId = studentId;
        Status = status.Trim();
        SubmittedOn = submittedOn;
        Remarks = remarks?.Trim();
    }

    public int HomeworkAssignmentId { get; private set; }
    public int StudentId { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateOnly? SubmittedOn { get; private set; }
    public string? Remarks { get; private set; }

    public HomeworkAssignment? HomeworkAssignment { get; private set; }
    public Student? Student { get; private set; }

    public void UpdateSubmission(string status, DateOnly? submittedOn, string? remarks)
    {
        Status = status.Trim();
        SubmittedOn = submittedOn;
        Remarks = remarks?.Trim();
        Touch();
    }
}
