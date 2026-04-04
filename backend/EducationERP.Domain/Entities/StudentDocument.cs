using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class StudentDocument : BaseEntity
{
    private StudentDocument()
    {
    }

    public StudentDocument(
        int studentId,
        string documentType,
        string status,
        DateOnly submittedOn,
        DateOnly? verifiedOn = null,
        string? remarks = null)
    {
        StudentId = studentId;
        DocumentType = documentType.Trim();
        Status = status.Trim();
        SubmittedOn = submittedOn;
        VerifiedOn = verifiedOn;
        Remarks = remarks?.Trim();
    }

    public int StudentId { get; private set; }
    public string DocumentType { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateOnly SubmittedOn { get; private set; }
    public DateOnly? VerifiedOn { get; private set; }
    public string? Remarks { get; private set; }

    public Student? Student { get; private set; }
}
