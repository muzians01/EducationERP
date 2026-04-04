namespace EducationERP.Application.Attendance;

public sealed record AttendanceDashboardDto(
    DateOnly AttendanceDate,
    int TotalStudentsMarked,
    int PresentCount,
    int AbsentCount,
    int LateCount,
    IReadOnlyList<AttendanceRecordDto> TodayRecords,
    IReadOnlyList<StudentAttendanceSummaryDto> StudentSummaries,
    IReadOnlyList<ClassAttendanceSummaryDto> ClassSummaries);
