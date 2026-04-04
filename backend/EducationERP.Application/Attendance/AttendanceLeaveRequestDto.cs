namespace EducationERP.Application.Attendance;

public sealed record AttendanceLeaveRequestDto(
    int Id,
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string ClassName,
    string SectionName,
    DateOnly LeaveDate,
    string LeaveType,
    string Reason,
    string Status);
