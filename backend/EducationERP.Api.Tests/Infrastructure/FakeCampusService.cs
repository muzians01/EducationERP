using EducationERP.Application.Campuses;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeCampusService : ICampusService
{
    public Task<IReadOnlyList<CampusDto>> GetCampusesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<CampusDto> campuses =
        [
            new CampusDto(
                1,
                1,
                "Test Education Trust",
                "TEST",
                "Test Campus",
                "Bengaluru",
                "Karnataka",
                "India",
                "CBSE")
        ];

        return Task.FromResult(campuses);
    }
}
