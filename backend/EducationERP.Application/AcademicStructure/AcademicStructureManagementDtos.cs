using EducationERP.Application.Campuses;

namespace EducationERP.Application.AcademicStructure;

public sealed record CreateInstitutionDto(
    string Code,
    string Name,
    string City,
    string State,
    string Country);

public sealed record UpdateInstitutionDto(
    string Code,
    string Name,
    string City,
    string State,
    string Country);

public sealed record CreateCampusDto(
    int InstitutionId,
    string Code,
    string Name,
    string City,
    string State,
    string Country,
    string BoardAffiliation);

public sealed record UpdateCampusDto(
    int InstitutionId,
    string Code,
    string Name,
    string City,
    string State,
    string Country,
    string BoardAffiliation);

public sealed record CreateAcademicYearDto(
    int CampusId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsActive);

public sealed record UpdateAcademicYearDto(
    int CampusId,
    string Name,
    DateOnly StartDate,
    DateOnly EndDate,
    bool IsActive);

public sealed record CreateSchoolClassDto(
    int CampusId,
    string Code,
    string Name,
    int DisplayOrder);

public sealed record UpdateSchoolClassDto(
    int CampusId,
    string Code,
    string Name,
    int DisplayOrder);

public sealed record CreateSectionDto(
    int SchoolClassId,
    string Name,
    int Capacity,
    string RoomNumber);

public sealed record UpdateSectionDto(
    int SchoolClassId,
    string Name,
    int Capacity,
    string RoomNumber);
