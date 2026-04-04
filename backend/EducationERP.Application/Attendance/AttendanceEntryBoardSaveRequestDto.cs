namespace EducationERP.Application.Attendance;

public sealed record AttendanceEntryBoardSaveRequestDto(
    DateOnly AttendanceDate,
    IReadOnlyList<AttendanceEntrySaveItemDto> Students);
