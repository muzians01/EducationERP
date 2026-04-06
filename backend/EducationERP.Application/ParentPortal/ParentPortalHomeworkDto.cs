namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalHomeworkDto(
    string SubjectName,
    string Title,
    DateOnly DueOn,
    string Status,
    string Instructions);
