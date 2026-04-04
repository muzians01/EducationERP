namespace EducationERP.Application.Attendance;

public sealed record AttendanceEntryStudentDto(
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string Status,
    bool HasApprovedLeave,
    string? LeaveType,
    string? Remarks);
