using EducationERP.Application.Admissions;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAdmissionsService : IAdmissionsService
{
    private readonly List<AdmissionApplicationDto> _applications =
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

    public Task<IReadOnlyList<AdmissionApplicationDto>> GetApplicationsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AdmissionApplicationDto>>(_applications.ToList());

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

    public Task<int> CreateApplicationAsync(CreateAdmissionApplicationDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _applications.Max(application => application.Id) + 1;
        _applications.Add(new AdmissionApplicationDto(
            nextId,
            $"ADM-TEST-{nextId:000}",
            $"{dto.StudentFirstName} {dto.StudentLastName}".Trim(),
            "New",
            new DateOnly(2026, 4, 8),
            "Test Campus",
            "2026-2027",
            "Grade 1",
            "A",
            "Ananya Sharma",
            "9876500001",
            dto.RegistrationFee));

        return Task.FromResult(nextId);
    }

    public Task UpdateApplicationStatusAsync(int applicationId, string status, CancellationToken cancellationToken = default)
    {
        var index = _applications.FindIndex(application => application.Id == applicationId);
        if (index < 0)
        {
            throw new InvalidOperationException("Admission application not found.");
        }

        var current = _applications[index];
        _applications[index] = current with { Status = status };
        return Task.CompletedTask;
    }
}
