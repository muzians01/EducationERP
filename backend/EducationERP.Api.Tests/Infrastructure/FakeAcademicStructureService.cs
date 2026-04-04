using EducationERP.Application.AcademicStructure;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAcademicStructureService : IAcademicStructureService
{
    public Task<IReadOnlyList<AcademicYearDto>> GetAcademicYearsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AcademicYearDto> years =
        [
            new AcademicYearDto(
                1,
                1,
                "2026-2027",
                new DateOnly(2026, 6, 1),
                new DateOnly(2027, 3, 31),
                true)
        ];

        return Task.FromResult(years);
    }

    public Task<IReadOnlyList<SchoolClassDto>> GetClassesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SchoolClassDto> classes =
        [
            new SchoolClassDto(
                1,
                1,
                "GRADE-1",
                "Grade 1",
                1)
        ];

        return Task.FromResult(classes);
    }

    public Task<IReadOnlyList<SectionDto>> GetSectionsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<SectionDto> sections =
        [
            new SectionDto(
                1,
                1,
                "Grade 1",
                "A",
                35,
                "G1-A01")
        ];

        return Task.FromResult(sections);
    }

    public async Task<AcademicStructureDto> GetAcademicStructureAsync(CancellationToken cancellationToken = default)
    {
        var years = await GetAcademicYearsAsync(cancellationToken);
        var classes = await GetClassesAsync(cancellationToken);
        var sections = await GetSectionsAsync(cancellationToken);

        return new AcademicStructureDto(years, classes, sections);
    }
}
