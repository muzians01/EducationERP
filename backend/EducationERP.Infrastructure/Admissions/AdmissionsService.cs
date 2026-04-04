using EducationERP.Application.Admissions;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Admissions;

internal sealed class AdmissionsService(EducationErpDbContext dbContext) : IAdmissionsService
{
    public async Task<IReadOnlyList<AdmissionApplicationDto>> GetApplicationsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AdmissionApplications
            .AsNoTracking()
            .Include(application => application.Campus)
            .Include(application => application.AcademicYear)
            .Include(application => application.SchoolClass)
            .Include(application => application.Section)
            .Include(application => application.Guardian)
            .OrderByDescending(application => application.AppliedOn)
            .ThenBy(application => application.ApplicationNumber)
            .Select(application => new AdmissionApplicationDto(
                application.Id,
                application.ApplicationNumber,
                $"{application.StudentFirstName} {application.StudentLastName}",
                application.Status,
                application.AppliedOn,
                application.Campus!.Name,
                application.AcademicYear!.Name,
                application.SchoolClass!.Name,
                application.Section!.Name,
                application.Guardian!.FullName,
                application.Guardian.PhoneNumber,
                application.RegistrationFee))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GuardianDto>> GetGuardiansAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Guardians
            .AsNoTracking()
            .Include(guardian => guardian.Campus)
            .OrderBy(guardian => guardian.FullName)
            .Select(guardian => new GuardianDto(
                guardian.Id,
                guardian.FullName,
                guardian.Relationship,
                guardian.PhoneNumber,
                guardian.Email,
                guardian.Occupation,
                guardian.Campus!.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task<AdmissionsDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var applications = await GetApplicationsAsync(cancellationToken);
        var guardians = await GetGuardiansAsync(cancellationToken);

        return new AdmissionsDashboardDto(
            applications.Count,
            applications.Count(application => application.Status.Equals("New", StringComparison.OrdinalIgnoreCase)),
            applications.Count(application => application.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase)),
            applications.Count(application => application.Status.Equals("Waitlisted", StringComparison.OrdinalIgnoreCase)),
            applications.Sum(application => application.RegistrationFee),
            applications.Take(5).ToList(),
            guardians.Take(5).ToList());
    }
}
