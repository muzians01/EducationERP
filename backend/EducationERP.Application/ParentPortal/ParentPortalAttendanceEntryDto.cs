namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalAttendanceEntryDto(
    DateOnly AttendanceDate,
    string Status,
    string? Remarks);
