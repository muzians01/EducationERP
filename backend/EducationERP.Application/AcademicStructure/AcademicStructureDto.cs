using EducationERP.Application.Campuses;

namespace EducationERP.Application.AcademicStructure;

public sealed record AcademicStructureDto(
    IReadOnlyList<InstitutionDto> Institutions,
    IReadOnlyList<CampusDto> Campuses,
    IReadOnlyList<AcademicYearDto> AcademicYears,
    IReadOnlyList<SchoolClassDto> Classes,
    IReadOnlyList<SectionDto> Sections);
