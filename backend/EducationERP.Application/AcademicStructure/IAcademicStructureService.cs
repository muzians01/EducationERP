using EducationERP.Application.Campuses;

namespace EducationERP.Application.AcademicStructure;

public interface IAcademicStructureService
{
    Task<IReadOnlyList<InstitutionDto>> GetInstitutionsAsync(CancellationToken cancellationToken = default);
    Task<InstitutionDto> CreateInstitutionAsync(CreateInstitutionDto dto, CancellationToken cancellationToken = default);
    Task<InstitutionDto> UpdateInstitutionAsync(int institutionId, UpdateInstitutionDto dto, CancellationToken cancellationToken = default);
    Task DeleteInstitutionAsync(int institutionId, CancellationToken cancellationToken = default);
    Task<CampusDto> CreateCampusAsync(CreateCampusDto dto, CancellationToken cancellationToken = default);
    Task<CampusDto> UpdateCampusAsync(int campusId, UpdateCampusDto dto, CancellationToken cancellationToken = default);
    Task DeleteCampusAsync(int campusId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AcademicYearDto>> GetAcademicYearsAsync(CancellationToken cancellationToken = default);
    Task<AcademicYearDto> CreateAcademicYearAsync(CreateAcademicYearDto dto, CancellationToken cancellationToken = default);
    Task<AcademicYearDto> UpdateAcademicYearAsync(int academicYearId, UpdateAcademicYearDto dto, CancellationToken cancellationToken = default);
    Task DeleteAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SchoolClassDto>> GetClassesAsync(CancellationToken cancellationToken = default);
    Task<SchoolClassDto> CreateSchoolClassAsync(CreateSchoolClassDto dto, CancellationToken cancellationToken = default);
    Task<SchoolClassDto> UpdateSchoolClassAsync(int schoolClassId, UpdateSchoolClassDto dto, CancellationToken cancellationToken = default);
    Task DeleteSchoolClassAsync(int schoolClassId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SectionDto>> GetSectionsAsync(CancellationToken cancellationToken = default);
    Task<SectionDto> CreateSectionAsync(CreateSectionDto dto, CancellationToken cancellationToken = default);
    Task<SectionDto> UpdateSectionAsync(int sectionId, UpdateSectionDto dto, CancellationToken cancellationToken = default);
    Task DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default);
    Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default);
}
