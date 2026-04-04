namespace EducationERP.Application.Students;

public sealed record StudentDocumentDto(
    int Id,
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string DocumentType,
    string Status,
    DateOnly SubmittedOn,
    DateOnly? VerifiedOn,
    string? Remarks);
