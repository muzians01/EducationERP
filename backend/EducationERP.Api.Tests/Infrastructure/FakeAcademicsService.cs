using EducationERP.Application.Academics;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAcademicsService : IAcademicsService
{
    public Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SubjectDto> subjects =
        [
            new SubjectDto(1, "ENG", "English", "Language", 6),
            new SubjectDto(2, "MAT", "Mathematics", "Core", 7),
            new SubjectDto(3, "SCI", "Science", "Core", 5)
        ];

        return Task.FromResult(subjects);
    }

    public Task<IReadOnlyList<TimetablePeriodDto>> GetTimetableAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<TimetablePeriodDto> timetable =
        [
            new TimetablePeriodDto(1, classId ?? 1, "Grade 1", sectionId ?? 2, "B", 1, "English", "ENG", "Monday", 1, new TimeOnly(8, 30), new TimeOnly(9, 10), "Anita Rao", "G1-B01"),
            new TimetablePeriodDto(2, classId ?? 1, "Grade 1", sectionId ?? 2, "B", 2, "Mathematics", "MAT", "Monday", 2, new TimeOnly(9, 10), new TimeOnly(9, 50), "Rahul Mehta", "G1-B01")
        ];

        return Task.FromResult(timetable);
    }

    public async Task<AcademicsDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var subjects = await GetSubjectsAsync(cancellationToken);
        var timetable = await GetTimetableAsync(classId, sectionId, cancellationToken);

        return new AcademicsDashboardDto(
            classId ?? 1,
            "Grade 1",
            sectionId ?? 2,
            "B",
            subjects.Count,
            timetable.Count,
            subjects,
            [
                new TimetableDayDto("Monday", timetable)
            ]);
    }
}
