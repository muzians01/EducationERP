namespace EducationERP.Application.ParentPortal;

public interface IParentPortalService
{
    Task<ParentPortalDashboardDto> GetDashboardAsync(int? studentId = null, CancellationToken cancellationToken = default);
}
