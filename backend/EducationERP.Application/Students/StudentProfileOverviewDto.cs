namespace EducationERP.Application.Students;

public sealed record StudentProfileOverviewDto(
    int Id,
    string AdmissionNumber,
    string StudentName,
    string ClassName,
    string SectionName,
    string GuardianName,
    string PrimaryContactNumber,
    string Address,
    string Gender,
    string? BloodGroup,
    string? MedicalNotes,
    int ProfileCompletionPercentage,
    int PendingDocumentCount);
