namespace EducationERP.Application.Attendance;

public sealed record AttendanceEntryBoardDto(
    DateOnly AttendanceDate,
    int ClassId,
    string ClassName,
    int SectionId,
    string SectionName,
    int StudentsOnRoll,
    int StudentsMarked,
    IReadOnlyList<AttendanceEntryStudentDto> Students,
    IReadOnlyList<AttendanceHolidayDto> UpcomingHolidays);
