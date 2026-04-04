namespace EducationERP.Application.AcademicStructure;

public sealed record SectionDto(
    int Id,
    int SchoolClassId,
    string SchoolClassName,
    string Name,
    int Capacity,
    string RoomNumber);
