using EducationERP.Application.Homework;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeHomeworkService : IHomeworkService
{
    public Task<IReadOnlyList<HomeworkAssignmentDto>> GetAssignmentsAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<HomeworkAssignmentDto> assignments =
        [
            new HomeworkAssignmentDto(1, classId ?? 1, "Grade 1", sectionId ?? 2, "B", 1, "English", new DateOnly(2026, 4, 6), new DateOnly(2026, 4, 8), "Reading journal", "Read chapter 4 and write five new vocabulary words.", "Anita Rao")
        ];

        return Task.FromResult(assignments);
    }

    public Task<IReadOnlyList<StudentHomeworkProgressDto>> GetProgressAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<StudentHomeworkProgressDto> progress =
        [
            new StudentHomeworkProgressDto(1, 1, "Ishita Verma", "STU-2026-001", "Reading journal", new DateOnly(2026, 4, 8), "Assigned", null, "Yet to be submitted")
        ];

        return Task.FromResult(progress);
    }

    public async Task<HomeworkDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var assignments = await GetAssignmentsAsync(classId, sectionId, cancellationToken);
        var progress = await GetProgressAsync(classId, sectionId, cancellationToken);

        return new HomeworkDashboardDto(
            classId ?? 1,
            "Grade 1",
            sectionId ?? 2,
            "B",
            assignments.Count,
            progress.Count(item => item.Status != "Submitted"),
            assignments,
            progress);
    }
}
