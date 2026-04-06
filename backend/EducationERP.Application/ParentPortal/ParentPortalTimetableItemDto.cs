namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalTimetableItemDto(
    string DayOfWeek,
    int PeriodNumber,
    string SubjectName,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string TeacherName);
