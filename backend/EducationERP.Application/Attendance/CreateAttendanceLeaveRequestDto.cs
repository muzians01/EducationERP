namespace EducationERP.Application.Attendance;

public sealed record CreateAttendanceLeaveRequestDto(
    int StudentId,
    DateOnly LeaveDate,
    string LeaveType,
    string Reason);
