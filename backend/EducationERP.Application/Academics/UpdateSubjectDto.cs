namespace EducationERP.Application.Academics;

public sealed record UpdateSubjectDto(
    string Code,
    string Name,
    string Category,
    int WeeklyPeriods);
