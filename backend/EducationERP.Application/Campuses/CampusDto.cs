namespace EducationERP.Application.Campuses;

public sealed record CampusDto(
    int Id,
    string Code,
    string Name,
    string City,
    string State,
    string Country,
    string BoardAffiliation);
