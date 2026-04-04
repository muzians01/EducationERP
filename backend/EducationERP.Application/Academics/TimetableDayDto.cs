namespace EducationERP.Application.Academics;

public sealed record TimetableDayDto(
    string DayOfWeek,
    IReadOnlyList<TimetablePeriodDto> Periods);
