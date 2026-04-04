namespace EducationERP.Application.Attendance;

public interface IAttendanceService
{
    Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentAttendanceSummaryDto>> GetStudentAttendanceSummaryAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ClassAttendanceSummaryDto>> GetClassAttendanceSummaryAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<AttendanceMonthlyReportDto> GetMonthlyReportAsync(DateOnly? referenceDate = null, CancellationToken cancellationToken = default);
    Task<AttendanceEntryBoardDto> GetEntryBoardAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<ClassAttendanceRegisterDto> GetClassRegisterAsync(DateOnly? referenceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<AttendanceDashboardDto> GetDashboardAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<AttendanceEntryBoardDto> SaveEntryBoardAsync(AttendanceEntryBoardSaveRequestDto request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceLeaveRequestDto>> GetLeaveRequestsAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AttendanceLeaveRequestDto>> UpdateLeaveRequestStatusAsync(int leaveRequestId, AttendanceLeaveDecisionRequestDto request, DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
