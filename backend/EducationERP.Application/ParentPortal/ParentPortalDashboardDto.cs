namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalDashboardDto(
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string ClassName,
    string SectionName,
    string GuardianName,
    string GuardianPhoneNumber,
    decimal AttendancePercentage,
    decimal OutstandingFees,
    string CurrentExamTerm,
    decimal LatestExamPercentage,
    IReadOnlyList<ParentPortalAnnouncementDto> Announcements,
    IReadOnlyList<ParentPortalHomeworkDto> UpcomingHomework,
    IReadOnlyList<ParentPortalFeeItemDto> OutstandingFeeItems,
    IReadOnlyList<ParentPortalExamResultDto> ExamResults,
    IReadOnlyList<ParentPortalTimetableItemDto> TodayTimetable,
    IReadOnlyList<ParentPortalAttendanceEntryDto> RecentAttendance);
