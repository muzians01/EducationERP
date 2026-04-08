using EducationERP.Application.Academics;
using EducationERP.Domain.Entities;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Academics;

internal sealed class AcademicsService(EducationErpDbContext dbContext) : IAcademicsService
{
    public async Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Subjects
            .AsNoTracking()
            .OrderBy(subject => subject.Name)
            .Select(subject => new SubjectDto(
                subject.Id,
                subject.Code,
                subject.Name,
                subject.Category,
                subject.WeeklyPeriods))
            .ToListAsync(cancellationToken);
    }

    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var subject = new Domain.Entities.Subject(
            dto.CampusId,
            dto.Code,
            dto.Name,
            dto.Category,
            dto.WeeklyPeriods);

        dbContext.Subjects.Add(subject);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SubjectDto(subject.Id, subject.Code, subject.Name, subject.Category, subject.WeeklyPeriods);
    }

    public async Task<SubjectDto> UpdateSubjectAsync(int subjectId, UpdateSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var subject = await dbContext.Subjects.FindAsync(new object?[] { subjectId }, cancellationToken);
        if (subject is null)
        {
            throw new InvalidOperationException("Subject not found.");
        }

        subject.UpdateDetails(dto.Code, dto.Name, dto.Category, dto.WeeklyPeriods);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new SubjectDto(subject.Id, subject.Code, subject.Name, subject.Category, subject.WeeklyPeriods);
    }

    public async Task DeleteSubjectAsync(int subjectId, CancellationToken cancellationToken = default)
    {
        var subject = await dbContext.Subjects.FindAsync(new object?[] { subjectId }, cancellationToken);
        if (subject is null)
        {
            throw new InvalidOperationException("Subject not found.");
        }

        dbContext.Subjects.Remove(subject);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TimetablePeriodDto>> GetTimetableAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var roster = await ResolveRosterAsync(classId, sectionId, cancellationToken);

        return await dbContext.TimetablePeriods
            .AsNoTracking()
            .Include(period => period.SchoolClass)
            .Include(period => period.Section)
            .Include(period => period.Subject)
            .Where(period => period.SchoolClassId == roster.ClassId && period.SectionId == roster.SectionId)
            .OrderBy(period => period.DayOfWeek)
            .ThenBy(period => period.PeriodNumber)
            .Select(period => new TimetablePeriodDto(
                period.Id,
                period.SchoolClassId,
                period.SchoolClass!.Name,
                period.SectionId,
                period.Section!.Name,
                period.SubjectId,
                period.Subject!.Name,
                period.Subject.Code,
                period.DayOfWeek.ToString(),
                period.PeriodNumber,
                period.StartTime,
                period.EndTime,
                period.TeacherName,
                period.RoomNumber))
            .ToListAsync(cancellationToken);
    }

    public async Task<TimetablePeriodDto> CreateTimetablePeriodAsync(CreateTimetablePeriodDto dto, CancellationToken cancellationToken = default)
    {
        var period = new Domain.Entities.TimetablePeriod(
            dto.AcademicYearId,
            dto.ClassId,
            dto.SectionId,
            dto.SubjectId,
            Enum.Parse<DayOfWeek>(dto.DayOfWeek, true),
            dto.PeriodNumber,
            dto.StartTime,
            dto.EndTime,
            dto.TeacherName,
            dto.RoomNumber);

        dbContext.TimetablePeriods.Add(period);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(period).Reference(p => p.Subject).LoadAsync(cancellationToken);
        await dbContext.Entry(period).Reference(p => p.Section).LoadAsync(cancellationToken);
        await dbContext.Entry(period).Reference(p => p.SchoolClass).LoadAsync(cancellationToken);

        return new TimetablePeriodDto(
            period.Id,
            period.SchoolClassId,
            period.SchoolClass?.Name ?? string.Empty,
            period.SectionId,
            period.Section?.Name ?? string.Empty,
            period.SubjectId,
            period.Subject?.Name ?? string.Empty,
            period.Subject?.Code ?? string.Empty,
            period.DayOfWeek.ToString(),
            period.PeriodNumber,
            period.StartTime,
            period.EndTime,
            period.TeacherName,
            period.RoomNumber);
    }

    public async Task<TimetablePeriodDto> UpdateTimetablePeriodAsync(int timetablePeriodId, UpdateTimetablePeriodDto dto, CancellationToken cancellationToken = default)
    {
        var period = await dbContext.TimetablePeriods.FindAsync(new object?[] { timetablePeriodId }, cancellationToken);
        if (period is null)
        {
            throw new InvalidOperationException("Timetable period not found.");
        }

        period.UpdatePeriod(
            dto.AcademicYearId,
            dto.ClassId,
            dto.SectionId,
            dto.SubjectId,
            Enum.Parse<DayOfWeek>(dto.DayOfWeek, true),
            dto.PeriodNumber,
            dto.StartTime,
            dto.EndTime,
            dto.TeacherName,
            dto.RoomNumber);

        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.Entry(period).Reference(p => p.Subject).LoadAsync(cancellationToken);
        await dbContext.Entry(period).Reference(p => p.Section).LoadAsync(cancellationToken);
        await dbContext.Entry(period).Reference(p => p.SchoolClass).LoadAsync(cancellationToken);

        return new TimetablePeriodDto(
            period.Id,
            period.SchoolClassId,
            period.SchoolClass?.Name ?? string.Empty,
            period.SectionId,
            period.Section?.Name ?? string.Empty,
            period.SubjectId,
            period.Subject?.Name ?? string.Empty,
            period.Subject?.Code ?? string.Empty,
            period.DayOfWeek.ToString(),
            period.PeriodNumber,
            period.StartTime,
            period.EndTime,
            period.TeacherName,
            period.RoomNumber);
    }

    public async Task DeleteTimetablePeriodAsync(int timetablePeriodId, CancellationToken cancellationToken = default)
    {
        var period = await dbContext.TimetablePeriods.FindAsync(new object?[] { timetablePeriodId }, cancellationToken);
        if (period is null)
        {
            throw new InvalidOperationException("Timetable period not found.");
        }

        dbContext.TimetablePeriods.Remove(period);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<AcademicsDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var roster = await ResolveRosterAsync(classId, sectionId, cancellationToken);
        var subjects = await GetSubjectsAsync(cancellationToken);
        var timetable = await GetTimetableAsync(roster.ClassId, roster.SectionId, cancellationToken);

        var weeklyTimetable = timetable
            .GroupBy(period => period.DayOfWeek)
            .Select(group => new TimetableDayDto(group.Key, group.OrderBy(period => period.PeriodNumber).ToList()))
            .OrderBy(day => DayOrder(day.DayOfWeek))
            .ToList();

        return new AcademicsDashboardDto(
            roster.ClassId,
            roster.ClassName,
            roster.SectionId,
            roster.SectionName,
            subjects.Count,
            timetable.Count,
            subjects,
            weeklyTimetable);
    }

    private async Task<(int ClassId, string ClassName, int SectionId, string SectionName)> ResolveRosterAsync(int? classId, int? sectionId, CancellationToken cancellationToken)
    {
        var scheduledSectionIds = await dbContext.TimetablePeriods
            .AsNoTracking()
            .Select(period => period.SectionId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var sections = await dbContext.Sections
            .AsNoTracking()
            .Include(section => section.SchoolClass)
            .Where(section => scheduledSectionIds.Contains(section.Id))
            .Where(section => !classId.HasValue || section.SchoolClassId == classId.Value)
            .Where(section => !sectionId.HasValue || section.Id == sectionId.Value)
            .OrderBy(section => section.SchoolClass!.DisplayOrder)
            .ThenBy(section => section.Name)
            .ToListAsync(cancellationToken);

        var section = sections.FirstOrDefault()
            ?? throw new InvalidOperationException("No timetable roster found for the selected class and section.");

        return (section.SchoolClassId, section.SchoolClass!.Name, section.Id, section.Name);
    }

    private static int DayOrder(string dayOfWeek) => dayOfWeek switch
    {
        "Monday" => 1,
        "Tuesday" => 2,
        "Wednesday" => 3,
        "Thursday" => 4,
        "Friday" => 5,
        "Saturday" => 6,
        _ => 7
    };
}
