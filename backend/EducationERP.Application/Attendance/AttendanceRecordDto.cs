namespace EducationERP.Application.Attendance;

public sealed record AttendanceRecordDto(
    int Id,
    string StudentName,
    string AdmissionNumber,
    string ClassName,
    string SectionName,
    DateOnly AttendanceDate,
    string Status,
    DateTime MarkedOn,
    string? Remarks);
