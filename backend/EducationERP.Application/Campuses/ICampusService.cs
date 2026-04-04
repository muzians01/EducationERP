namespace EducationERP.Application.Campuses;

public interface ICampusService
{
    Task<IReadOnlyList<CampusDto>> GetCampusesAsync(CancellationToken cancellationToken = default);
}
