namespace EducationERP.Application.Academics;

public interface IAcademicsService
{
    Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default);
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, CancellationToken cancellationToken = default);
    Task<SubjectDto> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto, CancellationToken cancellationToken = default);
    Task DeleteSubjectAsync(int subjectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TimetablePeriodDto>> GetTimetableAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
    Task<TimetablePeriodDto> CreateTimetablePeriodAsync(CreateTimetablePeriodDto dto, CancellationToken cancellationToken = default);
    Task<TimetablePeriodDto> UpdateTimetablePeriodAsync(int timetablePeriodId, UpdateTimetablePeriodDto dto, CancellationToken cancellationToken = default);
    Task DeleteTimetablePeriodAsync(int timetablePeriodId, CancellationToken cancellationToken = default);
    Task<AcademicsDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default);
}
