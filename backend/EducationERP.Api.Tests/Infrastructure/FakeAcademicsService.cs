using EducationERP.Application.Academics;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAcademicsService : IAcademicsService
{
    private readonly List<SubjectDto> _subjects =
    [
        new SubjectDto(1, "ENG", "English", "Language", 6),
        new SubjectDto(2, "MAT", "Mathematics", "Core", 7),
        new SubjectDto(3, "SCI", "Science", "Core", 5)
    ];

    private readonly List<TimetablePeriodDto> _timetable =
    [
        new TimetablePeriodDto(1, 1, "Grade 1", 2, "B", 1, "English", "ENG", "Monday", 1, new TimeOnly(8, 30), new TimeOnly(9, 10), "Anita Rao", "G1-B01"),
        new TimetablePeriodDto(2, 1, "Grade 1", 2, "B", 2, "Mathematics", "MAT", "Monday", 2, new TimeOnly(9, 10), new TimeOnly(9, 50), "Rahul Mehta", "G1-B01")
    ];

    public Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<SubjectDto>>(_subjects.OrderBy(subject => subject.Name).ToList());

    public Task<IReadOnlyList<TimetablePeriodDto>> GetTimetableAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var timetable = _timetable
            .Where(period => (!classId.HasValue || period.ClassId == classId.Value) && (!sectionId.HasValue || period.SectionId == sectionId.Value))
            .ToList();

        return Task.FromResult<IReadOnlyList<TimetablePeriodDto>>(timetable);
    }

    public async Task<AcademicsDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var subjects = await GetSubjectsAsync(cancellationToken);
        var timetable = await GetTimetableAsync(classId, sectionId, cancellationToken);

        return new AcademicsDashboardDto(
            classId ?? 1,
            "Grade 1",
            sectionId ?? 2,
            "B",
            subjects.Count,
            timetable.Count,
            subjects,
            [
                new TimetableDayDto("Monday", timetable)
            ]);
    }

    public Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _subjects.Max(subject => subject.Id) + 1;
        var subject = new SubjectDto(nextId, dto.Code.ToUpperInvariant(), dto.Name, dto.Category, dto.WeeklyPeriods);
        _subjects.Add(subject);
        return Task.FromResult(subject);
    }

    public Task<SubjectDto> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var index = _subjects.FindIndex(subject => subject.Id == subjectId);
        if (index < 0)
        {
            throw new InvalidOperationException("Subject not found.");
        }

        var updated = new SubjectDto(subjectId, dto.Code.ToUpperInvariant(), dto.Name, dto.Category, dto.WeeklyPeriods);
        _subjects[index] = updated;
        return Task.FromResult(updated);
    }

    public Task DeleteSubjectAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        var removedCount = _subjects.RemoveAll(subject => subject.Id == subjectId);
        if (removedCount == 0)
        {
            throw new InvalidOperationException("Subject not found.");
        }

        return Task.CompletedTask;
    }

    public Task<TimetablePeriodDto> CreateTimetablePeriodAsync(CreateTimetablePeriodDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _timetable.Max(period => period.Id) + 1;
        var subject = _subjects.FirstOrDefault(item => item.Id == dto.SubjectId) ?? _subjects[0];
        var period = new TimetablePeriodDto(
            nextId,
            dto.ClassId,
            "Grade 1",
            dto.SectionId,
            "B",
            dto.SubjectId,
            subject.Name,
            subject.Code,
            dto.DayOfWeek,
            dto.PeriodNumber,
            dto.StartTime,
            dto.EndTime,
            dto.TeacherName,
            dto.RoomNumber);

        _timetable.Add(period);
        return Task.FromResult(period);
    }

    public Task<TimetablePeriodDto> UpdateTimetablePeriodAsync(int timetablePeriodId, UpdateTimetablePeriodDto dto, CancellationToken cancellationToken = default)
    {
        var index = _timetable.FindIndex(period => period.Id == timetablePeriodId);
        if (index < 0)
        {
            throw new InvalidOperationException("Timetable period not found.");
        }

        var subject = _subjects.FirstOrDefault(item => item.Id == dto.SubjectId) ?? _subjects[0];
        var updated = new TimetablePeriodDto(
            timetablePeriodId,
            dto.ClassId,
            "Grade 1",
            dto.SectionId,
            "B",
            dto.SubjectId,
            subject.Name,
            subject.Code,
            dto.DayOfWeek,
            dto.PeriodNumber,
            dto.StartTime,
            dto.EndTime,
            dto.TeacherName,
            dto.RoomNumber);

        _timetable[index] = updated;
        return Task.FromResult(updated);
    }

    public Task DeleteTimetablePeriodAsync(int timetablePeriodId, CancellationToken cancellationToken = default)
    {
        var removedCount = _timetable.RemoveAll(period => period.Id == timetablePeriodId);
        if (removedCount == 0)
        {
            throw new InvalidOperationException("Timetable period not found.");
        }

        return Task.CompletedTask;
    }
}
