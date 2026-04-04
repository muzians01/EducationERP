namespace EducationERP.Application.Examinations;

public interface IExaminationsService
{
    Task<IReadOnlyList<ExamTermDto>> GetExamTermsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExamScheduleDto>> GetExamScheduleAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<ExaminationsDashboardDto> GetDashboardAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
