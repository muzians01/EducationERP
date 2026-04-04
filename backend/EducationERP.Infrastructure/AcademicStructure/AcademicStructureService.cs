using EducationERP.Application.AcademicStructure;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.AcademicStructure;

internal sealed class AcademicStructureService(EducationErpDbContext dbContext) : IAcademicStructureService
{
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

    public async Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default)
    {
        var academicYears = await GetAcademicYearsAsync(cancellationToken);
        var classes = await GetClassesAsync(cancellationToken);
        var sections = await GetSectionsAsync(cancellationToken);

        return new AcademicStructureDto(academicYears, classes, sections);
    }
}
