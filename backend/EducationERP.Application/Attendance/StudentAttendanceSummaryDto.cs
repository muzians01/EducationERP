namespace EducationERP.Application.Attendance;

public sealed record StudentAttendanceSummaryDto(
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string ClassName,
    string SectionName,
    int PresentDays,
    int AbsentDays,
    int LateDays,
    decimal AttendancePercentage);
