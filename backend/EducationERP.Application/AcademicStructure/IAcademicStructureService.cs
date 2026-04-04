namespace EducationERP.Application.AcademicStructure;

public interface IAcademicStructureService
{
    Task<IReadOnlyList<AcademicYearDto>> GetAcademicYearsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchoolClassDto>> GetClassesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SectionDto>> GetSectionsAsync(CancellationToken cancellationToken = default);
    Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default);
}
