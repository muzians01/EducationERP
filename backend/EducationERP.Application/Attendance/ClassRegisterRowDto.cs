namespace EducationERP.Application.Attendance;

public sealed record ClassRegisterRowDto(
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    IReadOnlyDictionary<string, string> DailyStatus,
    int PresentDays,
    int AbsentDays,
    int LateDays);
