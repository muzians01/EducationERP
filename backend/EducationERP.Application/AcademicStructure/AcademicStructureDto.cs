namespace EducationERP.Application.AcademicStructure;

public sealed record AcademicStructureDto(
    IReadOnlyList<AcademicYearDto> AcademicYears,
    IReadOnlyList<SchoolClassDto> Classes,
    IReadOnlyList<SectionDto> Sections);
