namespace EducationERP.Application.Academics;

public sealed record TimetablePeriodDto(
    int Id,
    int ClassId,
    string ClassName,
    int SectionId,
    string SectionName,
    int SubjectId,
    string SubjectName,
    string SubjectCode,
    string DayOfWeek,
    int PeriodNumber,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string TeacherName,
    string RoomNumber);
