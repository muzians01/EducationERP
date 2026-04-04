namespace EducationERP.Application.Admissions;

public sealed record GuardianDto(
    int Id,
    string FullName,
    string Relationship,
    string PhoneNumber,
    string Email,
    string Occupation,
    string CampusName);
