namespace EducationERP.Application.Examinations;

public interface IExaminationsService
{
    Task<IReadOnlyList<ExamTermDto>> GetExamTermsAsync(CancellationToken cancellationToken = default);
    Task<ExamTermDto> CreateExamTermAsync(CreateExamTermDto dto, CancellationToken cancellationToken = default);
    Task<ExamTermDto> UpdateExamTermAsync(int examTermId, UpdateExamTermDto dto, CancellationToken cancellationToken = default);
    Task DeleteExamTermAsync(int examTermId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExamScheduleDto>> GetExamScheduleAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<ExamScheduleDto> CreateExamScheduleAsync(CreateExamScheduleDto dto, CancellationToken cancellationToken = default);
    Task<ExamScheduleDto> UpdateExamScheduleAsync(int examScheduleId, UpdateExamScheduleDto dto, CancellationToken cancellationToken = default);
    Task DeleteExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default);
    Task<ExaminationsDashboardDto> GetDashboardAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
