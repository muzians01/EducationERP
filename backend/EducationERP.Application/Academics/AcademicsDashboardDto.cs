namespace EducationERP.Application.Academics;

public sealed record AcademicsDashboardDto(
    int SelectedClassId,
    string SelectedClassName,
    int SelectedSectionId,
    string SelectedSectionName,
    int SubjectCount,
    int WeeklyPeriodsPlanned,
    IReadOnlyList<SubjectDto> Subjects,
    IReadOnlyList<TimetableDayDto> WeeklyTimetable);
