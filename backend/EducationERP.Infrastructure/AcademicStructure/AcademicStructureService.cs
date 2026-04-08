using EducationERP.Application.AcademicStructure;
using EducationERP.Application.Campuses;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.AcademicStructure;

internal sealed class AcademicStructureService(EducationErpDbContext dbContext) : IAcademicStructureService
{
    public async Task<CampusDto> CreateCampusAsync(CreateCampusDto dto, CancellationToken cancellationToken = default)
    {
        var campus = new Domain.Entities.Campus(dto.Code, dto.Name, dto.City, dto.State, dto.Country, dto.BoardAffiliation);
        dbContext.Campuses.Add(campus);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapCampus(campus);
    }

    public async Task<CampusDto> UpdateCampusAsync(int campusId, UpdateCampusDto dto, CancellationToken cancellationToken = default)
    {
        var campus = await dbContext.Campuses.FirstOrDefaultAsync(item => item.Id == campusId, cancellationToken)
            ?? throw new InvalidOperationException($"Campus {campusId} was not found.");

        campus.UpdateDetails(dto.Code, dto.Name, dto.City, dto.State, dto.Country, dto.BoardAffiliation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapCampus(campus);
    }

    public async Task DeleteCampusAsync(int campusId, CancellationToken cancellationToken = default)
    {
        var campus = await dbContext.Campuses.FirstOrDefaultAsync(item => item.Id == campusId, cancellationToken)
            ?? throw new InvalidOperationException($"Campus {campusId} was not found.");

        dbContext.Campuses.Remove(campus);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AcademicYearDto>> GetAcademicYearsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AcademicYears
            .AsNoTracking()
            .OrderByDescending(academicYear => academicYear.StartDate)
            .ThenBy(academicYear => academicYear.Name)
            .Select(academicYear => new AcademicYearDto(
                academicYear.Id,
                academicYear.CampusId,
                academicYear.Name,
                academicYear.StartDate,
                academicYear.EndDate,
                academicYear.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<AcademicYearDto> CreateAcademicYearAsync(CreateAcademicYearDto dto, CancellationToken cancellationToken = default)
    {
        var academicYear = new Domain.Entities.AcademicYear(dto.CampusId, dto.Name, dto.StartDate, dto.EndDate, dto.IsActive);
        dbContext.AcademicYears.Add(academicYear);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapAcademicYear(academicYear);
    }

    public async Task<AcademicYearDto> UpdateAcademicYearAsync(int academicYearId, UpdateAcademicYearDto dto, CancellationToken cancellationToken = default)
    {
        var academicYear = await dbContext.AcademicYears.FirstOrDefaultAsync(item => item.Id == academicYearId, cancellationToken)
            ?? throw new InvalidOperationException($"Academic year {academicYearId} was not found.");

        academicYear.UpdateDetails(dto.CampusId, dto.Name, dto.StartDate, dto.EndDate, dto.IsActive);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapAcademicYear(academicYear);
    }

    public async Task DeleteAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var academicYear = await dbContext.AcademicYears.FirstOrDefaultAsync(item => item.Id == academicYearId, cancellationToken)
            ?? throw new InvalidOperationException($"Academic year {academicYearId} was not found.");

        dbContext.AcademicYears.Remove(academicYear);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SchoolClassDto>> GetClassesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Classes
            .AsNoTracking()
            .OrderBy(schoolClass => schoolClass.DisplayOrder)
            .ThenBy(schoolClass => schoolClass.Name)
            .Select(schoolClass => new SchoolClassDto(
                schoolClass.Id,
                schoolClass.CampusId,
                schoolClass.Code,
                schoolClass.Name,
                schoolClass.DisplayOrder))
            .ToListAsync(cancellationToken);
    }

    public async Task<SchoolClassDto> CreateSchoolClassAsync(CreateSchoolClassDto dto, CancellationToken cancellationToken = default)
    {
        var schoolClass = new Domain.Entities.SchoolClass(dto.CampusId, dto.Code, dto.Name, dto.DisplayOrder);
        dbContext.Classes.Add(schoolClass);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapSchoolClass(schoolClass);
    }

    public async Task<SchoolClassDto> UpdateSchoolClassAsync(int schoolClassId, UpdateSchoolClassDto dto, CancellationToken cancellationToken = default)
    {
        var schoolClass = await dbContext.Classes.FirstOrDefaultAsync(item => item.Id == schoolClassId, cancellationToken)
            ?? throw new InvalidOperationException($"Class {schoolClassId} was not found.");

        schoolClass.UpdateDetails(dto.CampusId, dto.Code, dto.Name, dto.DisplayOrder);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapSchoolClass(schoolClass);
    }

    public async Task DeleteSchoolClassAsync(int schoolClassId, CancellationToken cancellationToken = default)
    {
        var schoolClass = await dbContext.Classes.FirstOrDefaultAsync(item => item.Id == schoolClassId, cancellationToken)
            ?? throw new InvalidOperationException($"Class {schoolClassId} was not found.");

        dbContext.Classes.Remove(schoolClass);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SectionDto>> GetSectionsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Sections
            .AsNoTracking()
            .Include(section => section.SchoolClass)
            .OrderBy(section => section.SchoolClass!.DisplayOrder)
            .ThenBy(section => section.Name)
            .Select(section => new SectionDto(
                section.Id,
                section.SchoolClassId,
                section.SchoolClass!.Name,
                section.Name,
                section.Capacity,
                section.RoomNumber))
            .ToListAsync(cancellationToken);
    }

    public async Task<SectionDto> CreateSectionAsync(CreateSectionDto dto, CancellationToken cancellationToken = default)
    {
        var section = new Domain.Entities.Section(dto.SchoolClassId, dto.Name, dto.Capacity, dto.RoomNumber);
        dbContext.Sections.Add(section);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(section).Reference(item => item.SchoolClass).LoadAsync(cancellationToken);
        return MapSection(section);
    }

    public async Task<SectionDto> UpdateSectionAsync(int sectionId, UpdateSectionDto dto, CancellationToken cancellationToken = default)
    {
        var section = await dbContext.Sections
            .Include(item => item.SchoolClass)
            .FirstOrDefaultAsync(item => item.Id == sectionId, cancellationToken)
            ?? throw new InvalidOperationException($"Section {sectionId} was not found.");

        section.UpdateDetails(dto.SchoolClassId, dto.Name, dto.Capacity, dto.RoomNumber);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(section).Reference(item => item.SchoolClass).LoadAsync(cancellationToken);

        return MapSection(section);
    }

    public async Task DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        var section = await dbContext.Sections.FirstOrDefaultAsync(item => item.Id == sectionId, cancellationToken)
            ?? throw new InvalidOperationException($"Section {sectionId} was not found.");

        dbContext.Sections.Remove(section);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default)
    {
        var campuses = await dbContext.Campuses
            .AsNoTracking()
            .OrderBy(campus => campus.Name)
            .Select(campus => new CampusDto(
                campus.Id,
                campus.Code,
                campus.Name,
                campus.City,
                campus.State,
                campus.Country,
                campus.BoardAffiliation))
            .ToListAsync(cancellationToken);

        var academicYears = await GetAcademicYearsAsync(cancellationToken);
        var classes = await GetClassesAsync(cancellationToken);
        var sections = await GetSectionsAsync(cancellationToken);

        return new AcademicStructureDto(campuses, academicYears, classes, sections);
    }

    private static CampusDto MapCampus(Domain.Entities.Campus campus)
        => new(
            campus.Id,
            campus.Code,
            campus.Name,
            campus.City,
            campus.State,
            campus.Country,
            campus.BoardAffiliation);

    private static AcademicYearDto MapAcademicYear(Domain.Entities.AcademicYear academicYear)
        => new(
            academicYear.Id,
            academicYear.CampusId,
            academicYear.Name,
            academicYear.StartDate,
            academicYear.EndDate,
            academicYear.IsActive);

    private static SchoolClassDto MapSchoolClass(Domain.Entities.SchoolClass schoolClass)
        => new(
            schoolClass.Id,
            schoolClass.CampusId,
            schoolClass.Code,
            schoolClass.Name,
            schoolClass.DisplayOrder);

    private static SectionDto MapSection(Domain.Entities.Section section)
        => new(
            section.Id,
            section.SchoolClassId,
            section.SchoolClass?.Name ?? string.Empty,
            section.Name,
            section.Capacity,
            section.RoomNumber);
}
