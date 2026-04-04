namespace EducationERP.Application.Academics;

public sealed record SubjectDto(
    int Id,
    string Code,
    string Name,
    string Category,
    int WeeklyPeriods);
