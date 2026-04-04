using EducationERP.Application.Admissions;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAdmissionsService : IAdmissionsService
{
    public Task<IReadOnlyList<AdmissionApplicationDto>> GetApplicationsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AdmissionApplicationDto> applications =
        [
            new AdmissionApplicationDto(
                1,
                "ADM-TEST-001",
                "Aarav Sharma",
                "New",
                new DateOnly(2026, 4, 1),
                "Test Campus",
                "2026-2027",
                "Grade 1",
                "A",
                "Ananya Sharma",
                "9876500001",
                1500m)
        ];

        return Task.FromResult(applications);
    }

    public Task<IReadOnlyList<GuardianDto>> GetGuardiansAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<GuardianDto> guardians =
        [
            new GuardianDto(
                1,
                "Ananya Sharma",
                "Mother",
                "9876500001",
                "ananya.sharma@example.com",
                "Architect",
                "Test Campus")
        ];

        return Task.FromResult(guardians);
    }

    public async Task<AdmissionsDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var applications = await GetApplicationsAsync(cancellationToken);
        var guardians = await GetGuardiansAsync(cancellationToken);

        return new AdmissionsDashboardDto(
            1,
            1,
            0,
            0,
            1500m,
            applications,
            guardians);
    }
}
