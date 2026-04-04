namespace EducationERP.Application.AcademicStructure;

public sealed record SchoolClassDto(
    int Id,
    int CampusId,
    string Code,
    string Name,
    int DisplayOrder);
