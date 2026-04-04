namespace EducationERP.Application.AcademicStructure;

public sealed record AcademicYearDto(
    int Id,
    int CampusId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsActive);
