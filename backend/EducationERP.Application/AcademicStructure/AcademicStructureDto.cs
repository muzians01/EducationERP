using EducationERP.Application.Campuses;

namespace EducationERP.Application.AcademicStructure;

public sealed record AcademicStructureDto(
    IReadOnlyList<CampusDto> Campuses,
    IReadOnlyList<AcademicYearDto> AcademicYears,
    IReadOnlyList<SchoolClassDto> Classes,
    IReadOnlyList<SectionDto> Sections);
