namespace EducationERP.Application.Attendance;

public sealed record AttendanceEntrySaveItemDto(
    int StudentId,
    string Status,
    string? Remarks);
