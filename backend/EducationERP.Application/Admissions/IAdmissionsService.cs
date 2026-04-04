namespace EducationERP.Application.Admissions;

public interface IAdmissionsService
{
    Task<IReadOnlyList<AdmissionApplicationDto>> GetApplicationsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GuardianDto>> GetGuardiansAsync(CancellationToken cancellationToken = default);
    Task<AdmissionsDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
