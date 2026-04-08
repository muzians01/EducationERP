namespace EducationERP.Application.Admissions;

public sealed record CreateAdmissionApplicationDto(
    int CampusId,
    int AcademicYearId,
    int SchoolClassId,
    int SectionId,
    int GuardianId,
    string StudentFirstName,
    string StudentLastName,
    DateOnly DateOfBirth,
    string Gender,
    decimal RegistrationFee);

public sealed record UpdateAdmissionApplicationStatusDto(string Status);
