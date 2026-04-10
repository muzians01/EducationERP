using EducationERP.Application.Campuses;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Campuses;

internal sealed class CampusService(EducationErpDbContext dbContext) : ICampusService
{
    public async Task<IReadOnlyList<CampusDto>> GetCampusesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Campuses
            .AsNoTracking()
            .Include(campus => campus.Institution)
            .OrderBy(campus => campus.Name)
            .Select(campus => new CampusDto(
                campus.Id,
                campus.InstitutionId,
                campus.Institution!.Name,
                campus.Code,
                campus.Name,
                campus.City,
                campus.State,
                campus.Country,
                campus.BoardAffiliation))
            .ToListAsync(cancellationToken);
    }
}
