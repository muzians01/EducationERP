using EducationERP.Application.Examinations;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeExaminationsService : IExaminationsService
{
    private readonly List<ExamTermDto> _terms =
    [
        new ExamTermDto(1, "Term 1 Assessment", "Scholastic", new DateOnly(2026, 9, 14), new DateOnly(2026, 9, 18), "Scheduled")
    ];

    private readonly List<ExamScheduleDto> _schedule =
    [
        new ExamScheduleDto(1, 1, "Term 1 Assessment", 1, "Grade 1", 2, "B", 1, "English", new DateOnly(2026, 9, 14), new TimeOnly(9, 0), 90, 100, 35),
        new ExamScheduleDto(2, 1, "Term 1 Assessment", 1, "Grade 1", 2, "B", 2, "Mathematics", new DateOnly(2026, 9, 15), new TimeOnly(9, 0), 90, 100, 35)
    ];

    public Task<IReadOnlyList<ExamTermDto>> GetExamTermsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ExamTermDto>>(_terms.OrderByDescending(term => term.StartDate).ToList());

    public Task<ExamTermDto> CreateExamTermAsync(CreateExamTermDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _terms.Max(term => term.Id) + 1;
        var term = new ExamTermDto(nextId, dto.Name, dto.ExamType, dto.StartDate, dto.EndDate, dto.Status);
        _terms.Add(term);
        return Task.FromResult(term);
    }

    public Task<ExamTermDto> UpdateExamTermAsync(int examTermId, UpdateExamTermDto dto, CancellationToken cancellationToken = default)
    {
        var index = _terms.FindIndex(term => term.Id == examTermId);
        if (index < 0)
        {
            throw new InvalidOperationException("Exam term not found.");
        }

        var updated = new ExamTermDto(examTermId, dto.Name, dto.ExamType, dto.StartDate, dto.EndDate, dto.Status);
        _terms[index] = updated;
        return Task.FromResult(updated);
    }

    public Task DeleteExamTermAsync(int examTermId, CancellationToken cancellationToken = default)
    {
        _terms.RemoveAll(term => term.Id == examTermId);
        _schedule.RemoveAll(schedule => schedule.ExamTermId == examTermId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ExamScheduleDto>> GetExamScheduleAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var schedule = _schedule
            .Where(item => (!examTermId.HasValue || item.ExamTermId == examTermId.Value)
                && (!classId.HasValue || item.ClassId == classId.Value)
                && (!sectionId.HasValue || item.SectionId == sectionId.Value))
            .ToList();

        return Task.FromResult<IReadOnlyList<ExamScheduleDto>>(schedule);
    }

    public Task<ExamScheduleDto> CreateExamScheduleAsync(CreateExamScheduleDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _schedule.Max(schedule => schedule.Id) + 1;
        var term = _terms.FirstOrDefault(item => item.Id == dto.ExamTermId) ?? _terms[0];
        var subjectName = dto.SubjectId == 1 ? "English" : dto.SubjectId == 2 ? "Mathematics" : "Science";
        var schedule = new ExamScheduleDto(nextId, dto.ExamTermId, term.Name, dto.ClassId, "Grade 1", dto.SectionId, "B", dto.SubjectId, subjectName, dto.ExamDate, dto.StartTime, dto.DurationMinutes, dto.MaxMarks, dto.PassMarks);
        _schedule.Add(schedule);
        return Task.FromResult(schedule);
    }

    public Task<ExamScheduleDto> UpdateExamScheduleAsync(int examScheduleId, UpdateExamScheduleDto dto, CancellationToken cancellationToken = default)
    {
        var index = _schedule.FindIndex(schedule => schedule.Id == examScheduleId);
        if (index < 0)
        {
            throw new InvalidOperationException("Exam schedule not found.");
        }

        var term = _terms.FirstOrDefault(item => item.Id == dto.ExamTermId) ?? _terms[0];
        var subjectName = dto.SubjectId == 1 ? "English" : dto.SubjectId == 2 ? "Mathematics" : "Science";
        var updated = new ExamScheduleDto(examScheduleId, dto.ExamTermId, term.Name, dto.ClassId, "Grade 1", dto.SectionId, "B", dto.SubjectId, subjectName, dto.ExamDate, dto.StartTime, dto.DurationMinutes, dto.MaxMarks, dto.PassMarks);
        _schedule[index] = updated;
        return Task.FromResult(updated);
    }

    public Task DeleteExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        _schedule.RemoveAll(schedule => schedule.Id == examScheduleId);
        return Task.CompletedTask;
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
