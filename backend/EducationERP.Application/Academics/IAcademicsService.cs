namespace EducationERP.Application.Academics;

public interface IAcademicsService
{
    Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimetablePeriodDto>> GetTimetableAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<AcademicsDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
