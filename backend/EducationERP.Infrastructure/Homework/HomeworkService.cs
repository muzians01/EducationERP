using EducationERP.Application.Homework;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Homework;

internal sealed class HomeworkService(EducationErpDbContext dbContext) : IHomeworkService
{
    public async Task<IReadOnlyList<HomeworkAssignmentDto>> GetAssignmentsAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var roster = await ResolveRosterAsync(classId, sectionId, cancellationToken);

        return await dbContext.HomeworkAssignments
            .AsNoTracking()
            .Include(item => item.SchoolClass)
            .Include(item => item.Section)
            .Include(item => item.Subject)
            .Where(item => item.SchoolClassId == roster.ClassId && item.SectionId == roster.SectionId)
            .OrderByDescending(item => item.AssignedOn)
            .Select(item => new HomeworkAssignmentDto(
                item.Id,
                item.SchoolClassId,
                item.SchoolClass!.Name,
                item.SectionId,
                item.Section!.Name,
                item.SubjectId,
                item.Subject!.Name,
                item.AssignedOn,
                item.DueOn,
                item.Title,
                item.Instructions,
                item.AssignedBy))
            .ToListAsync(cancellationToken);
    }

    public async Task<HomeworkAssignmentDto> CreateAssignmentAsync(CreateHomeworkAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var assignment = new Domain.Entities.HomeworkAssignment(
            dto.AcademicYearId,
            dto.ClassId,
            dto.SectionId,
            dto.SubjectId,
            dto.AssignedOn,
            dto.DueOn,
            dto.Title,
            dto.Instructions,
            dto.AssignedBy);

        dbContext.HomeworkAssignments.Add(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);
        await LoadAssignmentReferencesAsync(assignment, cancellationToken);

        return MapAssignment(assignment);
    }

    public async Task<HomeworkAssignmentDto> UpdateAssignmentAsync(int homeworkAssignmentId, UpdateHomeworkAssignmentDto dto, CancellationToken cancellationToken = default)
    {
        var assignment = await dbContext.HomeworkAssignments.FirstOrDefaultAsync(item => item.Id == homeworkAssignmentId, cancellationToken);
        if (assignment is null)
        {
            throw new InvalidOperationException("Homework assignment not found.");
        }

        assignment.UpdateDetails(dto.AcademicYearId, dto.ClassId, dto.SectionId, dto.SubjectId, dto.AssignedOn, dto.DueOn, dto.Title, dto.Instructions, dto.AssignedBy);
        await dbContext.SaveChangesAsync(cancellationToken);
        await LoadAssignmentReferencesAsync(assignment, cancellationToken);

        return MapAssignment(assignment);
    }

    public async Task DeleteAssignmentAsync(int homeworkAssignmentId, CancellationToken cancellationToken = default)
    {
        var assignment = await dbContext.HomeworkAssignments.FirstOrDefaultAsync(item => item.Id == homeworkAssignmentId, cancellationToken);
        if (assignment is null)
        {
            throw new InvalidOperationException("Homework assignment not found.");
        }

        dbContext.HomeworkAssignments.Remove(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentHomeworkProgressDto>> GetProgressAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var roster = await ResolveRosterAsync(classId, sectionId, cancellationToken);

        return await dbContext.StudentHomeworkSubmissions
            .AsNoTracking()
            .Include(item => item.Student)
            .Include(item => item.HomeworkAssignment)
            .Where(item => item.HomeworkAssignment!.SchoolClassId == roster.ClassId && item.HomeworkAssignment.SectionId == roster.SectionId)
            .OrderBy(item => item.HomeworkAssignment!.DueOn)
            .ThenBy(item => item.Student!.AdmissionNumber)
            .Select(item => new StudentHomeworkProgressDto(
                item.HomeworkAssignmentId,
                item.StudentId,
                $"{item.Student!.FirstName} {item.Student.LastName}",
                item.Student.AdmissionNumber,
                item.HomeworkAssignment!.Title,
                item.HomeworkAssignment.DueOn,
                item.Status,
                item.SubmittedOn,
                item.Remarks))
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentHomeworkProgressDto> UpdateSubmissionAsync(UpdateHomeworkSubmissionDto dto, CancellationToken cancellationToken = default)
    {
        var submission = await dbContext.StudentHomeworkSubmissions
            .Include(item => item.Student)
            .Include(item => item.HomeworkAssignment)
            .FirstOrDefaultAsync(item => item.HomeworkAssignmentId == dto.HomeworkAssignmentId && item.StudentId == dto.StudentId, cancellationToken);

        if (submission is null)
        {
            submission = new Domain.Entities.StudentHomeworkSubmission(dto.HomeworkAssignmentId, dto.StudentId, dto.Status, dto.SubmittedOn, dto.Remarks);
            dbContext.StudentHomeworkSubmissions.Add(submission);
            await dbContext.SaveChangesAsync(cancellationToken);
            await dbContext.Entry(submission).Reference(item => item.Student).LoadAsync(cancellationToken);
            await dbContext.Entry(submission).Reference(item => item.HomeworkAssignment).LoadAsync(cancellationToken);
        }
        else
        {
            submission.UpdateSubmission(dto.Status, dto.SubmittedOn, dto.Remarks);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new StudentHomeworkProgressDto(
            submission.HomeworkAssignmentId,
            submission.StudentId,
            $"{submission.Student!.FirstName} {submission.Student.LastName}",
            submission.Student.AdmissionNumber,
            submission.HomeworkAssignment!.Title,
            submission.HomeworkAssignment.DueOn,
            submission.Status,
            submission.SubmittedOn,
            submission.Remarks);
    }

    public async Task<HomeworkDashboardDto> GetDashboardAsync(int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var roster = await ResolveRosterAsync(classId, sectionId, cancellationToken);
        var assignments = await GetAssignmentsAsync(roster.ClassId, roster.SectionId, cancellationToken);
        var progress = await GetProgressAsync(roster.ClassId, roster.SectionId, cancellationToken);

        return new HomeworkDashboardDto(
            roster.ClassId,
            roster.ClassName,
            roster.SectionId,
            roster.SectionName,
            assignments.Count,
            progress.Count(item => item.Status != "Submitted"),
            assignments,
            progress);
    }

    private async Task LoadAssignmentReferencesAsync(Domain.Entities.HomeworkAssignment assignment, CancellationToken cancellationToken)
    {
        await dbContext.Entry(assignment).Reference(item => item.SchoolClass).LoadAsync(cancellationToken);
        await dbContext.Entry(assignment).Reference(item => item.Section).LoadAsync(cancellationToken);
        await dbContext.Entry(assignment).Reference(item => item.Subject).LoadAsync(cancellationToken);
    }

    private static HomeworkAssignmentDto MapAssignment(Domain.Entities.HomeworkAssignment item) =>
        new(
            item.Id,
            item.SchoolClassId,
            item.SchoolClass!.Name,
            item.SectionId,
            item.Section!.Name,
            item.SubjectId,
            item.Subject!.Name,
            item.AssignedOn,
            item.DueOn,
            item.Title,
            item.Instructions,
            item.AssignedBy);

    private async Task<(int ClassId, string ClassName, int SectionId, string SectionName)> ResolveRosterAsync(int? classId, int? sectionId, CancellationToken cancellationToken)
    {
        var assignments = await dbContext.HomeworkAssignments
            .AsNoTracking()
            .Include(item => item.SchoolClass)
            .Include(item => item.Section)
            .Where(item => !classId.HasValue || item.SchoolClassId == classId.Value)
            .Where(item => !sectionId.HasValue || item.SectionId == sectionId.Value)
            .OrderBy(item => item.SchoolClass!.DisplayOrder)
            .ThenBy(item => item.Section!.Name)
            .ToListAsync(cancellationToken);

        var first = assignments.FirstOrDefault()
            ?? throw new InvalidOperationException("No homework roster found.");

        return (first.SchoolClassId, first.SchoolClass!.Name, first.SectionId, first.Section!.Name);
    }
}
