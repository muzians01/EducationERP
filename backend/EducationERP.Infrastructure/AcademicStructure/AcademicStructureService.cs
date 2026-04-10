using EducationERP.Application.AcademicStructure;
using EducationERP.Application.Campuses;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.AcademicStructure;

internal sealed class AcademicStructureService(EducationErpDbContext dbContext) : IAcademicStructureService
{
    public async Task<IReadOnlyList<InstitutionDto>> GetInstitutionsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Institutions
            .AsNoTracking()
            .OrderBy(institution => institution.Name)
            .Select(institution => new InstitutionDto(
                institution.Id,
                institution.Code,
                institution.Name,
                institution.City,
                institution.State,
                institution.Country))
            .ToListAsync(cancellationToken);
    }

    public async Task<InstitutionDto> CreateInstitutionAsync(CreateInstitutionDto dto, CancellationToken cancellationToken = default)
    {
        ValidateInstitution(dto.Code, dto.Name);
        var institution = new Domain.Entities.Institution(dto.Code, dto.Name, dto.City, dto.State, dto.Country);
        dbContext.Institutions.Add(institution);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapInstitution(institution);
    }

    public async Task<InstitutionDto> UpdateInstitutionAsync(int institutionId, UpdateInstitutionDto dto, CancellationToken cancellationToken = default)
    {
        ValidateInstitution(dto.Code, dto.Name);
        var institution = await dbContext.Institutions.FirstOrDefaultAsync(item => item.Id == institutionId, cancellationToken)
            ?? throw new InvalidOperationException($"Institution {institutionId} was not found.");

        institution.UpdateDetails(dto.Code, dto.Name, dto.City, dto.State, dto.Country);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapInstitution(institution);
    }

    public async Task DeleteInstitutionAsync(int institutionId, CancellationToken cancellationToken = default)
    {
        var institution = await dbContext.Institutions.FirstOrDefaultAsync(item => item.Id == institutionId, cancellationToken)
            ?? throw new InvalidOperationException($"Institution {institutionId} was not found.");

        var isInUse = await dbContext.Campuses.AnyAsync(item => item.InstitutionId == institutionId, cancellationToken);
        if (isInUse)
        {
            throw new InvalidOperationException($"Institution {institution.Name} cannot be deleted because campuses already exist under it.");
        }

        dbContext.Institutions.Remove(institution);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<CampusDto> CreateCampusAsync(CreateCampusDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureInstitutionExistsAsync(dto.InstitutionId, cancellationToken);
        ValidateCampus(dto.Code, dto.Name);
        var campus = new Domain.Entities.Campus(dto.InstitutionId, dto.Code, dto.Name, dto.City, dto.State, dto.Country, dto.BoardAffiliation);
        dbContext.Campuses.Add(campus);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(campus).Reference(item => item.Institution).LoadAsync(cancellationToken);
        return MapCampus(campus);
    }

    public async Task<CampusDto> UpdateCampusAsync(int campusId, UpdateCampusDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureInstitutionExistsAsync(dto.InstitutionId, cancellationToken);
        ValidateCampus(dto.Code, dto.Name);
        var campus = await dbContext.Campuses
            .Include(item => item.Institution)
            .FirstOrDefaultAsync(item => item.Id == campusId, cancellationToken)
            ?? throw new InvalidOperationException($"Campus {campusId} was not found.");

        campus.UpdateDetails(dto.InstitutionId, dto.Code, dto.Name, dto.City, dto.State, dto.Country, dto.BoardAffiliation);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(campus).Reference(item => item.Institution).LoadAsync(cancellationToken);

        return MapCampus(campus);
    }

    public async Task DeleteCampusAsync(int campusId, CancellationToken cancellationToken = default)
    {
        var campus = await dbContext.Campuses.FirstOrDefaultAsync(item => item.Id == campusId, cancellationToken)
            ?? throw new InvalidOperationException($"Campus {campusId} was not found.");

        var isInUse = await dbContext.AcademicYears.AnyAsync(item => item.CampusId == campusId, cancellationToken)
            || await dbContext.Classes.AnyAsync(item => item.CampusId == campusId, cancellationToken)
            || await dbContext.Guardians.AnyAsync(item => item.CampusId == campusId, cancellationToken)
            || await dbContext.AdmissionApplications.AnyAsync(item => item.CampusId == campusId, cancellationToken)
            || await dbContext.Students.AnyAsync(item => item.CampusId == campusId, cancellationToken)
            || await dbContext.FeeStructures.AnyAsync(item => item.CampusId == campusId, cancellationToken)
            || await dbContext.SchoolHolidays.AnyAsync(item => item.CampusId == campusId, cancellationToken);

        if (isInUse)
        {
            throw new InvalidOperationException($"Campus {campus.Name} cannot be deleted because dependent records already exist.");
        }

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
        await EnsureCampusExistsAsync(dto.CampusId, cancellationToken);
        ValidateAcademicYear(dto.Name, dto.StartDate, dto.EndDate);
        var academicYear = new Domain.Entities.AcademicYear(dto.CampusId, dto.Name, dto.StartDate, dto.EndDate, dto.IsActive);
        dbContext.AcademicYears.Add(academicYear);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapAcademicYear(academicYear);
    }

    public async Task<AcademicYearDto> UpdateAcademicYearAsync(int academicYearId, UpdateAcademicYearDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCampusExistsAsync(dto.CampusId, cancellationToken);
        ValidateAcademicYear(dto.Name, dto.StartDate, dto.EndDate);
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

        var isInUse = await dbContext.AdmissionApplications.AnyAsync(item => item.AcademicYearId == academicYearId, cancellationToken)
            || await dbContext.Students.AnyAsync(item => item.AcademicYearId == academicYearId, cancellationToken)
            || await dbContext.FeeStructures.AnyAsync(item => item.AcademicYearId == academicYearId, cancellationToken)
            || await dbContext.ExamTerms.AnyAsync(item => item.AcademicYearId == academicYearId, cancellationToken)
            || await dbContext.TimetablePeriods.AnyAsync(item => item.AcademicYearId == academicYearId, cancellationToken);

        if (isInUse)
        {
            throw new InvalidOperationException($"Academic year {academicYear.Name} cannot be deleted because dependent records already exist.");
        }

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
        await EnsureCampusExistsAsync(dto.CampusId, cancellationToken);
        ValidateSchoolClass(dto.Code, dto.Name, dto.DisplayOrder);
        var schoolClass = new Domain.Entities.SchoolClass(dto.CampusId, dto.Code, dto.Name, dto.DisplayOrder);
        dbContext.Classes.Add(schoolClass);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapSchoolClass(schoolClass);
    }

    public async Task<SchoolClassDto> UpdateSchoolClassAsync(int schoolClassId, UpdateSchoolClassDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureCampusExistsAsync(dto.CampusId, cancellationToken);
        ValidateSchoolClass(dto.Code, dto.Name, dto.DisplayOrder);
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

        var isInUse = await dbContext.Sections.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken)
            || await dbContext.AdmissionApplications.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken)
            || await dbContext.Students.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken)
            || await dbContext.FeeStructures.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken)
            || await dbContext.ExamSchedules.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken)
            || await dbContext.HomeworkAssignments.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken)
            || await dbContext.TimetablePeriods.AnyAsync(item => item.SchoolClassId == schoolClassId, cancellationToken);

        if (isInUse)
        {
            throw new InvalidOperationException($"Class {schoolClass.Name} cannot be deleted because dependent records already exist.");
        }

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
        await EnsureSchoolClassExistsAsync(dto.SchoolClassId, cancellationToken);
        ValidateSection(dto.Name, dto.Capacity);
        var section = new Domain.Entities.Section(dto.SchoolClassId, dto.Name, dto.Capacity, dto.RoomNumber);
        dbContext.Sections.Add(section);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(section).Reference(item => item.SchoolClass).LoadAsync(cancellationToken);
        return MapSection(section);
    }

    public async Task<SectionDto> UpdateSectionAsync(int sectionId, UpdateSectionDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureSchoolClassExistsAsync(dto.SchoolClassId, cancellationToken);
        ValidateSection(dto.Name, dto.Capacity);
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

        var isInUse = await dbContext.AdmissionApplications.AnyAsync(item => item.SectionId == sectionId, cancellationToken)
            || await dbContext.Students.AnyAsync(item => item.SectionId == sectionId, cancellationToken)
            || await dbContext.ExamSchedules.AnyAsync(item => item.SectionId == sectionId, cancellationToken)
            || await dbContext.HomeworkAssignments.AnyAsync(item => item.SectionId == sectionId, cancellationToken)
            || await dbContext.TimetablePeriods.AnyAsync(item => item.SectionId == sectionId, cancellationToken);

        if (isInUse)
        {
            throw new InvalidOperationException($"Section {section.Name} cannot be deleted because dependent records already exist.");
        }

        dbContext.Sections.Remove(section);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default)
    {
        var institutions = await GetInstitutionsAsync(cancellationToken);
        var campuses = await dbContext.Campuses
            .AsNoTracking()
            .Include(campus => campus.Institution)
            .OrderBy(campus => campus.Name)
            .Select(campus => new CampusDto(
                campus.Id,
                campus.InstitutionId,
                campus.Institution!.Name,
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

        return new AcademicStructureDto(institutions, campuses, academicYears, classes, sections);
    }

    private static InstitutionDto MapInstitution(Domain.Entities.Institution institution)
        => new(
            institution.Id,
            institution.Code,
            institution.Name,
            institution.City,
            institution.State,
            institution.Country);

    private static CampusDto MapCampus(Domain.Entities.Campus campus)
        => new(
            campus.Id,
            campus.InstitutionId,
            campus.Institution?.Name ?? string.Empty,
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

    private async Task EnsureInstitutionExistsAsync(int institutionId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Institutions.AnyAsync(item => item.Id == institutionId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException($"Institution {institutionId} was not found.");
        }
    }

    private async Task EnsureCampusExistsAsync(int campusId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Campuses.AnyAsync(item => item.Id == campusId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException($"Campus {campusId} was not found.");
        }
    }

    private async Task EnsureSchoolClassExistsAsync(int schoolClassId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Classes.AnyAsync(item => item.Id == schoolClassId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException($"Class {schoolClassId} was not found.");
        }
    }

    private static void ValidateInstitution(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Institution code and name are required.");
        }
    }

    private static void ValidateCampus(string code, string name)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Campus code and name are required.");
        }
    }

    private static void ValidateAcademicYear(string name, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Academic year name is required.");
        }

        if (startDate > endDate)
        {
            throw new InvalidOperationException("Academic year start date cannot be after the end date.");
        }
    }

    private static void ValidateSchoolClass(string code, string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Class code and name are required.");
        }

        if (displayOrder <= 0)
        {
            throw new InvalidOperationException("Class display order must be greater than zero.");
        }
    }

    private static void ValidateSection(string name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Section name is required.");
        }

        if (capacity <= 0)
        {
            throw new InvalidOperationException("Section capacity must be greater than zero.");
        }
    }
}
