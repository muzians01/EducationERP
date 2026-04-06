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
