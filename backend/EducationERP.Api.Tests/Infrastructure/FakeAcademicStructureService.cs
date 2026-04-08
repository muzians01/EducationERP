using EducationERP.Application.AcademicStructure;
using EducationERP.Application.Campuses;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAcademicStructureService : IAcademicStructureService
{
    private readonly List<CampusDto> _campuses =
    [
        new CampusDto(1, "TEST", "Test Campus", "Bengaluru", "Karnataka", "India", "CBSE")
    ];

    private readonly List<AcademicYearDto> _academicYears =
    [
        new AcademicYearDto(1, 1, "2026-2027", new DateOnly(2026, 6, 1), new DateOnly(2027, 3, 31), true)
    ];

    private readonly List<SchoolClassDto> _classes =
    [
        new SchoolClassDto(1, 1, "GRADE-1", "Grade 1", 1)
    ];

    private readonly List<SectionDto> _sections =
    [
        new SectionDto(1, 1, "Grade 1", "A", 35, "G1-A01")
    ];

    public Task<CampusDto> CreateCampusAsync(CreateCampusDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _campuses.Max(item => item.Id) + 1;
        var campus = new CampusDto(nextId, dto.Code.Trim().ToUpperInvariant(), dto.Name.Trim(), dto.City.Trim(), dto.State.Trim(), dto.Country.Trim(), dto.BoardAffiliation.Trim());
        _campuses.Add(campus);
        return Task.FromResult(campus);
    }

    public Task<CampusDto> UpdateCampusAsync(int campusId, UpdateCampusDto dto, CancellationToken cancellationToken = default)
    {
        var index = _campuses.FindIndex(item => item.Id == campusId);
        if (index < 0)
        {
            throw new InvalidOperationException("Campus not found.");
        }

        var campus = new CampusDto(campusId, dto.Code.Trim().ToUpperInvariant(), dto.Name.Trim(), dto.City.Trim(), dto.State.Trim(), dto.Country.Trim(), dto.BoardAffiliation.Trim());
        _campuses[index] = campus;
        return Task.FromResult(campus);
    }

    public Task DeleteCampusAsync(int campusId, CancellationToken cancellationToken = default)
    {
        _campuses.RemoveAll(item => item.Id == campusId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AcademicYearDto>> GetAcademicYearsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AcademicYearDto>>(_academicYears.ToList());

    public Task<AcademicYearDto> CreateAcademicYearAsync(CreateAcademicYearDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _academicYears.Max(item => item.Id) + 1;
        var academicYear = new AcademicYearDto(nextId, dto.CampusId, dto.Name.Trim(), dto.StartDate, dto.EndDate, dto.IsActive);
        _academicYears.Add(academicYear);
        return Task.FromResult(academicYear);
    }

    public Task<AcademicYearDto> UpdateAcademicYearAsync(int academicYearId, UpdateAcademicYearDto dto, CancellationToken cancellationToken = default)
    {
        var index = _academicYears.FindIndex(item => item.Id == academicYearId);
        if (index < 0)
        {
            throw new InvalidOperationException("Academic year not found.");
        }

        var academicYear = new AcademicYearDto(academicYearId, dto.CampusId, dto.Name.Trim(), dto.StartDate, dto.EndDate, dto.IsActive);
        _academicYears[index] = academicYear;
        return Task.FromResult(academicYear);
    }

    public Task DeleteAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        _academicYears.RemoveAll(item => item.Id == academicYearId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<SchoolClassDto>> GetClassesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SchoolClassDto>>(_classes.ToList());

    public Task<SchoolClassDto> CreateSchoolClassAsync(CreateSchoolClassDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _classes.Max(item => item.Id) + 1;
        var schoolClass = new SchoolClassDto(nextId, dto.CampusId, dto.Code.Trim().ToUpperInvariant(), dto.Name.Trim(), dto.DisplayOrder);
        _classes.Add(schoolClass);
        return Task.FromResult(schoolClass);
    }

    public Task<SchoolClassDto> UpdateSchoolClassAsync(int schoolClassId, UpdateSchoolClassDto dto, CancellationToken cancellationToken = default)
    {
        var index = _classes.FindIndex(item => item.Id == schoolClassId);
        if (index < 0)
        {
            throw new InvalidOperationException("Class not found.");
        }

        var schoolClass = new SchoolClassDto(schoolClassId, dto.CampusId, dto.Code.Trim().ToUpperInvariant(), dto.Name.Trim(), dto.DisplayOrder);
        _classes[index] = schoolClass;
        return Task.FromResult(schoolClass);
    }

    public Task DeleteSchoolClassAsync(int schoolClassId, CancellationToken cancellationToken = default)
    {
        _classes.RemoveAll(item => item.Id == schoolClassId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<SectionDto>> GetSectionsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SectionDto>>(_sections.ToList());

    public Task<SectionDto> CreateSectionAsync(CreateSectionDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _sections.Max(item => item.Id) + 1;
        var className = _classes.FirstOrDefault(item => item.Id == dto.SchoolClassId)?.Name ?? "Grade 1";
        var section = new SectionDto(nextId, dto.SchoolClassId, className, dto.Name.Trim().ToUpperInvariant(), dto.Capacity, dto.RoomNumber.Trim().ToUpperInvariant());
        _sections.Add(section);
        return Task.FromResult(section);
    }

    public Task<SectionDto> UpdateSectionAsync(int sectionId, UpdateSectionDto dto, CancellationToken cancellationToken = default)
    {
        var index = _sections.FindIndex(item => item.Id == sectionId);
        if (index < 0)
        {
            throw new InvalidOperationException("Section not found.");
        }

        var className = _classes.FirstOrDefault(item => item.Id == dto.SchoolClassId)?.Name ?? "Grade 1";
        var section = new SectionDto(sectionId, dto.SchoolClassId, className, dto.Name.Trim().ToUpperInvariant(), dto.Capacity, dto.RoomNumber.Trim().ToUpperInvariant());
        _sections[index] = section;
        return Task.FromResult(section);
    }

    public Task DeleteSectionAsync(int sectionId, CancellationToken cancellationToken = default)
    {
        _sections.RemoveAll(item => item.Id == sectionId);
        return Task.CompletedTask;
    }

    public Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(new AcademicStructureDto(_campuses.ToList(), _academicYears.ToList(), _classes.ToList(), _sections.ToList()));
}
