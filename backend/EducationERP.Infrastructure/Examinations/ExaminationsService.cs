using EducationERP.Application.Examinations;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Examinations;

internal sealed class ExaminationsService(EducationErpDbContext dbContext) : IExaminationsService
{
    public async Task<IReadOnlyList<ExamTermDto>> GetExamTermsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.ExamTerms
            .AsNoTracking()
            .OrderByDescending(term => term.StartDate)
            .Select(term => MapTerm(term))
            .ToListAsync(cancellationToken);
    }

    public async Task<ExamTermDto> CreateExamTermAsync(CreateExamTermDto dto, CancellationToken cancellationToken = default)
    {
        var term = new Domain.Entities.ExamTerm(dto.CampusId, dto.AcademicYearId, dto.Name, dto.ExamType, dto.StartDate, dto.EndDate, dto.Status);
        dbContext.ExamTerms.Add(term);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapTerm(term);
    }

    public async Task<ExamTermDto> UpdateExamTermAsync(int examTermId, UpdateExamTermDto dto, CancellationToken cancellationToken = default)
    {
        var term = await dbContext.ExamTerms.FirstOrDefaultAsync(item => item.Id == examTermId, cancellationToken);
        if (term is null)
        {
            throw new InvalidOperationException("Exam term not found.");
        }

        term.UpdateDetails(dto.CampusId, dto.AcademicYearId, dto.Name, dto.ExamType, dto.StartDate, dto.EndDate, dto.Status);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapTerm(term);
    }

    public async Task DeleteExamTermAsync(int examTermId, CancellationToken cancellationToken = default)
    {
        var term = await dbContext.ExamTerms.FirstOrDefaultAsync(item => item.Id == examTermId, cancellationToken);
        if (term is null)
        {
            throw new InvalidOperationException("Exam term not found.");
        }

        dbContext.ExamTerms.Remove(term);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExamScheduleDto>> GetExamScheduleAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var context = await ResolveContextAsync(examTermId, classId, sectionId, cancellationToken);

        return await dbContext.ExamSchedules
            .AsNoTracking()
            .Include(schedule => schedule.ExamTerm)
            .Include(schedule => schedule.SchoolClass)
            .Include(schedule => schedule.Section)
            .Include(schedule => schedule.Subject)
            .Where(schedule => schedule.ExamTermId == context.ExamTermId && schedule.SchoolClassId == context.ClassId && schedule.SectionId == context.SectionId)
            .OrderBy(schedule => schedule.ExamDate)
            .ThenBy(schedule => schedule.StartTime)
            .Select(schedule => MapSchedule(schedule))
            .ToListAsync(cancellationToken);
    }

    public async Task<ExamScheduleDto> CreateExamScheduleAsync(CreateExamScheduleDto dto, CancellationToken cancellationToken = default)
    {
        var schedule = new Domain.Entities.ExamSchedule(dto.ExamTermId, dto.ClassId, dto.SectionId, dto.SubjectId, dto.ExamDate, dto.StartTime, dto.DurationMinutes, dto.MaxMarks, dto.PassMarks);
        dbContext.ExamSchedules.Add(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);

        await LoadScheduleReferencesAsync(schedule, cancellationToken);
        return MapSchedule(schedule);
    }

    public async Task<ExamScheduleDto> UpdateExamScheduleAsync(int examScheduleId, UpdateExamScheduleDto dto, CancellationToken cancellationToken = default)
    {
        var schedule = await dbContext.ExamSchedules.FirstOrDefaultAsync(item => item.Id == examScheduleId, cancellationToken);
        if (schedule is null)
        {
            throw new InvalidOperationException("Exam schedule not found.");
        }

        schedule.UpdateDetails(dto.ExamTermId, dto.ClassId, dto.SectionId, dto.SubjectId, dto.ExamDate, dto.StartTime, dto.DurationMinutes, dto.MaxMarks, dto.PassMarks);
        await dbContext.SaveChangesAsync(cancellationToken);

        await LoadScheduleReferencesAsync(schedule, cancellationToken);
        return MapSchedule(schedule);
    }

    public async Task DeleteExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        var schedule = await dbContext.ExamSchedules.FirstOrDefaultAsync(item => item.Id == examScheduleId, cancellationToken);
        if (schedule is null)
        {
            throw new InvalidOperationException("Exam schedule not found.");
        }

        dbContext.ExamSchedules.Remove(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ExaminationsDashboardDto> GetDashboardAsync(int? examTermId = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var examTerms = await GetExamTermsAsync(cancellationToken);
        var context = await ResolveContextAsync(examTermId, classId, sectionId, cancellationToken);
        var schedule = await GetExamScheduleAsync(context.ExamTermId, context.ClassId, context.SectionId, cancellationToken);

        var scores = await dbContext.StudentExamScores
            .AsNoTracking()
            .Include(score => score.Student)
            .Include(score => score.ExamSchedule)
            .ThenInclude(examSchedule => examSchedule!.Subject)
            .Where(score => score.ExamSchedule!.ExamTermId == context.ExamTermId
                && score.ExamSchedule.SchoolClassId == context.ClassId
                && score.ExamSchedule.SectionId == context.SectionId)
            .ToListAsync(cancellationToken);

        var reportCards = scores
            .GroupBy(score => score.StudentId)
            .Select(group =>
            {
                var student = group.First().Student!;
                var subjectResults = group
                    .OrderBy(score => score.ExamSchedule!.Subject!.Name)
                    .Select(score => new ExamSubjectResultDto(
                        score.ExamSchedule!.SubjectId,
                        score.ExamSchedule.Subject!.Name,
                        score.ExamSchedule.MaxMarks,
                        score.ExamSchedule.PassMarks,
                        score.MarksObtained,
                        score.Grade,
                        score.ResultStatus))
                    .ToList();

                var totalMarks = group.Sum(score => score.ExamSchedule!.MaxMarks);
                var marksObtained = group.Sum(score => score.MarksObtained);
                var percentage = totalMarks == 0 ? 0 : decimal.Round(marksObtained * 100 / totalMarks, 1);
                var resultStatus = subjectResults.All(result => result.ResultStatus == "Pass") ? "Pass" : "Needs Support";

                return new StudentReportCardDto(
                    student.Id,
                    $"{student.FirstName} {student.LastName}",
                    student.AdmissionNumber,
                    student.SchoolClass!.Name,
                    student.Section!.Name,
                    totalMarks,
                    marksObtained,
                    percentage,
                    resultStatus,
                    subjectResults);
            })
            .OrderBy(report => report.StudentName)
            .ToList();

        return new ExaminationsDashboardDto(
            context.ExamTermId,
            context.ExamTermName,
            context.ClassId,
            context.ClassName,
            context.SectionId,
            context.SectionName,
            examTerms,
            schedule,
            reportCards);
    }

    private async Task<(int ExamTermId, string ExamTermName, int ClassId, string ClassName, int SectionId, string SectionName)> ResolveContextAsync(
        int? examTermId,
        int? classId,
        int? sectionId,
        CancellationToken cancellationToken)
    {
        var terms = await dbContext.ExamTerms
            .AsNoTracking()
            .OrderByDescending(term => term.StartDate)
            .ToListAsync(cancellationToken);

        var selectedTerm = terms.FirstOrDefault(term => !examTermId.HasValue || term.Id == examTermId.Value)
            ?? throw new InvalidOperationException("No examination term found.");

        var schedules = await dbContext.ExamSchedules
            .AsNoTracking()
            .Include(schedule => schedule.SchoolClass)
            .Include(schedule => schedule.Section)
            .Where(schedule => schedule.ExamTermId == selectedTerm.Id)
            .Where(schedule => !classId.HasValue || schedule.SchoolClassId == classId.Value)
            .Where(schedule => !sectionId.HasValue || schedule.SectionId == sectionId.Value)
            .OrderBy(schedule => schedule.SchoolClass!.DisplayOrder)
            .ThenBy(schedule => schedule.Section!.Name)
            .ToListAsync(cancellationToken);

        var selectedSchedule = schedules.FirstOrDefault()
            ?? throw new InvalidOperationException("No examination schedule found for the selected filters.");

        return (
            selectedTerm.Id,
            selectedTerm.Name,
            selectedSchedule.SchoolClassId,
            selectedSchedule.SchoolClass!.Name,
            selectedSchedule.SectionId,
            selectedSchedule.Section!.Name);
    }

    private async Task LoadScheduleReferencesAsync(Domain.Entities.ExamSchedule schedule, CancellationToken cancellationToken)
    {
        await dbContext.Entry(schedule).Reference(item => item.ExamTerm).LoadAsync(cancellationToken);
        await dbContext.Entry(schedule).Reference(item => item.SchoolClass).LoadAsync(cancellationToken);
        await dbContext.Entry(schedule).Reference(item => item.Section).LoadAsync(cancellationToken);
        await dbContext.Entry(schedule).Reference(item => item.Subject).LoadAsync(cancellationToken);
    }

    private static ExamTermDto MapTerm(Domain.Entities.ExamTerm term) =>
        new(term.Id, term.Name, term.ExamType, term.StartDate, term.EndDate, term.Status);

    private static ExamScheduleDto MapSchedule(Domain.Entities.ExamSchedule schedule) =>
        new(
            schedule.Id,
            schedule.ExamTermId,
            schedule.ExamTerm!.Name,
            schedule.SchoolClassId,
            schedule.SchoolClass!.Name,
            schedule.SectionId,
            schedule.Section!.Name,
            schedule.SubjectId,
            schedule.Subject!.Name,
            schedule.ExamDate,
            schedule.StartTime,
            schedule.DurationMinutes,
            schedule.MaxMarks,
            schedule.PassMarks);
}
