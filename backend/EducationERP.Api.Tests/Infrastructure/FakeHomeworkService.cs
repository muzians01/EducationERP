using EducationERP.Application.Homework;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeHomeworkService : IHomeworkService
{
    private readonly List<HomeworkAssignmentDto> _assignments =
    [
        new HomeworkAssignmentDto(1, 1, "Grade 1", 2, "B", 1, "English", new DateOnly(2026, 4, 6), new DateOnly(2026, 4, 8), "Reading journal", "Read chapter 4 and write five new vocabulary words.", "Anita Rao")
    ];

    private readonly List<StudentHomeworkProgressDto> _progress =
    [
        new StudentHomeworkProgressDto(1, 1, "Ishita Verma", "STU-2026-001", "Reading journal", new DateOnly(2026, 4, 8), "Assigned", null, "Yet to be submitted")
    ];

    public Task<IReadOnlyList<HomeworkAssignmentDto>> GetAssignmentsAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<HomeworkAssignmentDto>>(_assignments.ToList());

    public Task<HomeworkAssignmentDto> CreateAssignmentAsync(CreateHomeworkAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _assignments.Max(item => item.Id) + 1;
        var assignment = new HomeworkAssignmentDto(nextId, dto.ClassId, "Grade 1", dto.SectionId, "B", dto.SubjectId, dto.SubjectId == 1 ? "English" : "Mathematics", dto.AssignedOn, dto.DueOn, dto.Title, dto.Instructions, dto.AssignedBy);
        _assignments.Add(assignment);
        return Task.FromResult(assignment);
    }

    public Task<HomeworkAssignmentDto> UpdateAssignmentAsync(int homeworkAssignmentId, UpdateHomeworkAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var index = _assignments.FindIndex(item => item.Id == homeworkAssignmentId);
        if (index < 0)
        {
            throw new InvalidOperationException("Homework assignment not found.");
        }

        var assignment = new HomeworkAssignmentDto(homeworkAssignmentId, dto.ClassId, "Grade 1", dto.SectionId, "B", dto.SubjectId, dto.SubjectId == 1 ? "English" : "Mathematics", dto.AssignedOn, dto.DueOn, dto.Title, dto.Instructions, dto.AssignedBy);
        _assignments[index] = assignment;
        return Task.FromResult(assignment);
    }

    public Task DeleteAssignmentAsync(int homeworkAssignmentId, CancellationToken cancellationToken = default)
    {
        _assignments.RemoveAll(item => item.Id == homeworkAssignmentId);
        _progress.RemoveAll(item => item.HomeworkAssignmentId == homeworkAssignmentId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<StudentHomeworkProgressDto>> GetProgressAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<StudentHomeworkProgressDto>>(_progress.ToList());

    public Task<StudentHomeworkProgressDto> UpdateSubmissionAsync(UpdateHomeworkSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var index = _progress.FindIndex(item => item.HomeworkAssignmentId == dto.HomeworkAssignmentId && item.StudentId == dto.StudentId);
        var assignment = _assignments.FirstOrDefault(item => item.Id == dto.HomeworkAssignmentId) ?? _assignments[0];
        var progress = new StudentHomeworkProgressDto(dto.HomeworkAssignmentId, dto.StudentId, "Ishita Verma", "STU-2026-001", assignment.Title, assignment.DueOn, dto.Status, dto.SubmittedOn, dto.Remarks);

        if (index < 0)
        {
            _progress.Add(progress);
        }
        else
        {
            _progress[index] = progress;
        }

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
