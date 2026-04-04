namespace EducationERP.Application.Admissions;

public sealed record AdmissionApplicationDto(
    int Id,
    string ApplicationNumber,
    string StudentName,
    string Status,
    DateOnly AppliedOn,
    string CampusName,
    string AcademicYearName,
    string ClassName,
    string SectionName,
    string GuardianName,
    string GuardianPhoneNumber,
    decimal RegistrationFee);
