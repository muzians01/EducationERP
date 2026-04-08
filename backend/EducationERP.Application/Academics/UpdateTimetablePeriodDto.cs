namespace EducationERP.Application.Academics;

public sealed record UpdateTimetablePeriodDto(
    int AcademicYearId,
    int ClassId,
    int SectionId,
    int SubjectId,
    string DayOfWeek,
    int PeriodNumber,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string TeacherName,
    string RoomNumber);
