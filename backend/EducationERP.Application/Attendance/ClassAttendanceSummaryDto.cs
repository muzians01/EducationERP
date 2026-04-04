namespace EducationERP.Application.Attendance;

public sealed record ClassAttendanceSummaryDto(
    string ClassName,
    string SectionName,
    int StudentsMarked,
    int PresentCount,
    int AbsentCount,
    int LateCount,
    decimal AttendancePercentage);
