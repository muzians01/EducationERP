namespace EducationERP.Application.Homework;

public interface IHomeworkService
{
    Task<IReadOnlyList<HomeworkAssignmentDto>> GetAssignmentsAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<HomeworkAssignmentDto> CreateAssignmentAsync(CreateHomeworkAssignmentDto dto, CancellationToken cancellationToken = default);
    Task<HomeworkAssignmentDto> UpdateAssignmentAsync(int homeworkAssignmentId, UpdateHomeworkAssignmentDto dto, CancellationToken cancellationToken = default);
    Task DeleteAssignmentAsync(int homeworkAssignmentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentHomeworkProgressDto>> GetProgressAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<StudentHomeworkProgressDto> UpdateSubmissionAsync(UpdateHomeworkSubmissionDto dto, CancellationToken cancellationToken = default);
    Task<HomeworkDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
