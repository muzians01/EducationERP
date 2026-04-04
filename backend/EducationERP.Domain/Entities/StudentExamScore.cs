using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class StudentExamScore : BaseEntity
{
    private StudentExamScore()
    {
    }

    public StudentExamScore(
        int examScheduleId,
        int studentId,
        decimal marksObtained,
        string grade,
        string resultStatus,
        string? remarks = null)
    {
        ExamScheduleId = examScheduleId;
        StudentId = studentId;
        MarksObtained = marksObtained;
        Grade = grade.Trim().ToUpperInvariant();
        ResultStatus = resultStatus.Trim();
        Remarks = remarks?.Trim();
    }

    public int ExamScheduleId { get; private set; }
    public int StudentId { get; private set; }
    public decimal MarksObtained { get; private set; }
    public string Grade { get; private set; } = string.Empty;
    public string ResultStatus { get; private set; } = string.Empty;
    public string? Remarks { get; private set; }

    public ExamSchedule? ExamSchedule { get; private set; }
    public Student? Student { get; private set; }
}
