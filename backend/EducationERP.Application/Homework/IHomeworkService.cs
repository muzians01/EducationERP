namespace EducationERP.Application.Homework;

public interface IHomeworkService
{
    Task<IReadOnlyList<HomeworkAssignmentDto>> GetAssignmentsAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentHomeworkProgressDto>> GetProgressAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<HomeworkDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
