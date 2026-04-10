namespace EducationERP.Application.AcademicStructure;

public sealed record InstitutionDto(
    int Id,
    string Code,
    string Name,
    string City,
    string State,
    string Country);
