using EducationERP.Application.ParentPortal;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeParentPortalService : IParentPortalService
{
    public Task<ParentPortalDashboardDto> GetDashboardAsync(int? studentId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ParentPortalDashboardDto(
            studentId ?? 1,
            "Ishita Verma",
            "STU-2026-001",
            "Grade 1",
            "B",
            "Anjali Verma",
            "9876500001",
            83.5m,
            12000m,
            "Term 1 Assessment",
            88.5m,
            [
                new ParentPortalAnnouncementDto("Parent-teacher meeting", "Meet class teachers this Saturday.", new DateOnly(2026, 4, 6))
            ],
            [
                new ParentPortalHomeworkDto("English", "Reading journal", new DateOnly(2026, 4, 8), "Assigned", "Read chapter 4 and write five new vocabulary words.")
            ],
            [
                new ParentPortalFeeItemDto("Tuition Fee", new DateOnly(2026, 4, 15), 12000m, "Pending")
            ],
            [
                new ParentPortalExamResultDto("Term 1 Assessment", "Mathematics", 91m, 100, "A+", "Pass")
            ],
            [
                new ParentPortalTimetableItemDto("Monday", 1, "English", new TimeOnly(8, 30), new TimeOnly(9, 10), "Anita Rao")
            ],
            [
                new ParentPortalAttendanceEntryDto(new DateOnly(2026, 4, 4), "Present", null),
                new ParentPortalAttendanceEntryDto(new DateOnly(2026, 4, 3), "Absent", "Parent informed class teacher")
            ]));
    }
}
