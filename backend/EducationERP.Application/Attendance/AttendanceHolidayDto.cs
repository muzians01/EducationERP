namespace EducationERP.Application.Attendance;

public sealed record AttendanceHolidayDto(
    int Id,
    DateOnly HolidayDate,
    string Title,
    string Category);
