namespace EducationERP.Application.Attendance;

public sealed record ClassAttendanceRegisterDto(
    string MonthLabel,
    int ClassId,
    string ClassName,
    int SectionId,
    string SectionName,
    IReadOnlyList<string> WorkingDayLabels,
    IReadOnlyList<ClassRegisterRowDto> Rows);
