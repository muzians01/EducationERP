namespace EducationERP.Application.Academics;

public sealed record CreateSubjectDto(
    int CampusId,
    string Code,
    string Name,
    string Category,
    int WeeklyPeriods);
