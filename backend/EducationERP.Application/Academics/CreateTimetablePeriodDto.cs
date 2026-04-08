namespace EducationERP.Application.Academics;

public sealed record CreateTimetablePeriodDto(
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
