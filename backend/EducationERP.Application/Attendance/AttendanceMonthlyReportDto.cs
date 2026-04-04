namespace EducationERP.Application.Attendance;

public sealed record AttendanceMonthlyReportDto(
    string MonthLabel,
    int WorkingDays,
    int StudentsCovered,
    decimal OverallAttendancePercentage,
    IReadOnlyList<ClassAttendanceSummaryDto> ClassSummaries,
    IReadOnlyList<StudentAttendanceSummaryDto> StudentsNeedingAttention);
