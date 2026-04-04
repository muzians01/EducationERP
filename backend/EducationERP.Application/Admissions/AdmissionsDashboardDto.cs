namespace EducationERP.Application.Admissions;

public sealed record AdmissionsDashboardDto(
    int TotalApplications,
    int NewApplications,
    int ApprovedApplications,
    int WaitlistedApplications,
    decimal TotalRegistrationFees,
    IReadOnlyList<AdmissionApplicationDto> RecentApplications,
    IReadOnlyList<GuardianDto> Guardians);
