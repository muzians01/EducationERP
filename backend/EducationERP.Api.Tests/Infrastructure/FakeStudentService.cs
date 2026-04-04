using EducationERP.Application.Students;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeStudentService : IStudentService
{
    public Task<IReadOnlyList<StudentDto>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<StudentDto> students =
        [
            new StudentDto(
                1,
                "STU-2026-001",
                "Ishita Verma",
                "Test Campus",
                "2026-2027",
                "Grade 1",
                "B",
                "Rahul Verma",
                new DateOnly(2026, 4, 1),
                "Active")
        ];

        return Task.FromResult(students);
    }

    public Task<IReadOnlyList<StudentProfileOverviewDto>> GetStudentProfileOverviewAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<StudentProfileOverviewDto> profiles =
        [
            new StudentProfileOverviewDto(
                1,
                "STU-2026-001",
                "Ishita Verma",
                "Grade 1",
                "B",
                "Rahul Verma",
                "9876500002",
                "45 Green Residency, Bengaluru, Karnataka 560001",
                "Female",
                "B+",
                "Dust allergy",
                100,
                1)
        ];

        return Task.FromResult(profiles);
    }

    public Task<IReadOnlyList<StudentDocumentDto>> GetStudentDocumentsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<StudentDocumentDto> documents =
        [
            new StudentDocumentDto(
                1,
                1,
                "Ishita Verma",
                "STU-2026-001",
                "Birth Certificate",
                "Verified",
                new DateOnly(2026, 4, 1),
                new DateOnly(2026, 4, 1),
                "Original verified at front desk"),
            new StudentDocumentDto(
                2,
                1,
                "Ishita Verma",
                "STU-2026-001",
                "Transfer Certificate",
                "Pending",
                new DateOnly(2026, 4, 1),
                null,
                "Awaiting previous school submission")
        ];

        return Task.FromResult(documents);
    }
}
