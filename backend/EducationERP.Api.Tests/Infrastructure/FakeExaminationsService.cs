using EducationERP.Application.Examinations;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeExaminationsService : IExaminationsService
{
    public Task<IReadOnlyList<ExamTermDto>> GetExamTermsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ExamTermDto> terms =
        [
            new ExamTermDto(1, "Term 1 Assessment", "Scholastic", new DateOnly(2026, 9, 14), new DateOnly(2026, 9, 18), "Scheduled")
        ];

        return Task.FromResult(terms);
    }

    public Task<IReadOnlyList<ExamScheduleDto>> GetExamScheduleAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ExamScheduleDto> schedule =
        [
            new ExamScheduleDto(1, examTermId ?? 1, "Term 1 Assessment", classId ?? 1, "Grade 1", sectionId ?? 2, "B", 1, "English", new DateOnly(2026, 9, 14), new TimeOnly(9, 0), 90, 100, 35),
            new ExamScheduleDto(2, examTermId ?? 1, "Term 1 Assessment", classId ?? 1, "Grade 1", sectionId ?? 2, "B", 2, "Mathematics", new DateOnly(2026, 9, 15), new TimeOnly(9, 0), 90, 100, 35)
        ];

        return Task.FromResult(schedule);
    }

    public async Task<ExaminationsDashboardDto> GetDashboardAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var terms = await GetExamTermsAsync(cancellationToken);
        var schedule = await GetExamScheduleAsync(examTermId, classId, sectionId, cancellationToken);

        return new ExaminationsDashboardDto(
            examTermId ?? 1,
            "Term 1 Assessment",
            classId ?? 1,
            "Grade 1",
            sectionId ?? 2,
            "B",
            terms,
            schedule,
            [
                new StudentReportCardDto(
                    1,
                    "Ishita Verma",
                    "STU-2026-001",
                    "Grade 1",
                    "B",
                    200,
                    177m,
                    88.5m,
                    "Pass",
                    [
                        new ExamSubjectResultDto(1, "English", 100, 35, 86m, "A", "Pass"),
                        new ExamSubjectResultDto(2, "Mathematics", 100, 35, 91m, "A+", "Pass")
                    ])
            ]);
    }
}
