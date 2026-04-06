using EducationERP.Application.ParentPortal;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.ParentPortal;

internal sealed class ParentPortalService(EducationErpDbContext dbContext) : IParentPortalService
{
    public async Task<ParentPortalDashboardDto> GetDashboardAsync(int? studentId = null, CancellationToken cancellationToken = default)
    {
        var student = await dbContext.Students
            .AsNoTracking()
            .Include(item => item.SchoolClass)
            .Include(item => item.Section)
            .Include(item => item.Guardian)
            .OrderBy(item => item.AdmissionNumber)
            .FirstOrDefaultAsync(item => !studentId.HasValue || item.Id == studentId.Value, cancellationToken)
            ?? throw new InvalidOperationException("No student found for the parent portal.");

        var attendanceRecords = await dbContext.AttendanceRecords
            .AsNoTracking()
            .Where(record => record.StudentId == student.Id)
            .OrderByDescending(record => record.AttendanceDate)
            .ToListAsync(cancellationToken);

        var attendancePercentage = attendanceRecords.Count == 0
            ? 0
            : decimal.Round((attendanceRecords.Count(record => record.Status == "Present") + attendanceRecords.Count(record => record.Status == "Late") * 0.5m) * 100 / attendanceRecords.Count, 1);

        var fees = await dbContext.StudentFees
            .AsNoTracking()
            .Include(fee => fee.FeeStructure)
            .Include(fee => fee.Concessions)
            .Where(fee => fee.StudentId == student.Id)
            .OrderBy(fee => fee.DueOn)
            .ToListAsync(cancellationToken);

        var outstandingFeeItems = fees
            .Select(fee => new
            {
                FeeName = fee.FeeStructure!.FeeName,
                fee.DueOn,
                BalanceAmount = fee.Amount - fee.AmountPaid - fee.Concessions.Sum(concession => concession.Amount),
                fee.Status
            })
            .Where(fee => fee.BalanceAmount > 0)
            .ToList();

        var latestExamTerm = await dbContext.ExamTerms
            .AsNoTracking()
            .OrderByDescending(term => term.StartDate)
            .FirstOrDefaultAsync(cancellationToken);

        var examResults = await dbContext.StudentExamScores
            .AsNoTracking()
            .Include(score => score.ExamSchedule)
            .ThenInclude(schedule => schedule!.ExamTerm)
            .Include(score => score.ExamSchedule)
            .ThenInclude(schedule => schedule!.Subject)
            .Where(score => score.StudentId == student.Id)
            .OrderByDescending(score => score.ExamSchedule!.ExamDate)
            .ToListAsync(cancellationToken);

        var homework = await dbContext.StudentHomeworkSubmissions
            .AsNoTracking()
            .Include(item => item.HomeworkAssignment)
            .ThenInclude(assignment => assignment!.Subject)
            .Where(item => item.StudentId == student.Id)
            .OrderBy(item => item.HomeworkAssignment!.DueOn)
            .ToListAsync(cancellationToken);

        var latestExamPercentage = 0m;
        if (latestExamTerm is not null)
        {
            var latestScores = examResults.Where(score => score.ExamSchedule!.ExamTermId == latestExamTerm.Id).ToList();
            var totalMarks = latestScores.Sum(score => score.ExamSchedule!.MaxMarks);
            var obtainedMarks = latestScores.Sum(score => score.MarksObtained);
            latestExamPercentage = totalMarks == 0 ? 0 : decimal.Round(obtainedMarks * 100 / totalMarks, 1);
        }

        var todayName = DateTime.Now.DayOfWeek.ToString();
        var todayTimetable = await dbContext.TimetablePeriods
            .AsNoTracking()
            .Include(period => period.Subject)
            .Where(period => period.SchoolClassId == student.SchoolClassId && period.SectionId == student.SectionId && period.DayOfWeek.ToString() == todayName)
            .OrderBy(period => period.PeriodNumber)
            .Select(period => new ParentPortalTimetableItemDto(
                period.DayOfWeek.ToString(),
                period.PeriodNumber,
                period.Subject!.Name,
                period.StartTime,
                period.EndTime,
                period.TeacherName))
            .ToListAsync(cancellationToken);

        return new ParentPortalDashboardDto(
            student.Id,
            $"{student.FirstName} {student.LastName}",
            student.AdmissionNumber,
            student.SchoolClass!.Name,
            student.Section!.Name,
            student.Guardian!.FullName,
            student.Guardian.PhoneNumber,
            attendancePercentage,
            outstandingFeeItems.Sum(fee => fee.BalanceAmount),
            latestExamTerm?.Name ?? "No exam term",
            latestExamPercentage,
            [
                new ParentPortalAnnouncementDto("Parent-teacher meeting", "Meet class teachers on the second Saturday for progress review.", DateOnly.FromDateTime(DateTime.Today)),
                new ParentPortalAnnouncementDto("Fee reminder", "Clear outstanding tuition before the next assessment cycle.", DateOnly.FromDateTime(DateTime.Today.AddDays(-3)))
            ],
            homework.Select(item => new ParentPortalHomeworkDto(
                item.HomeworkAssignment!.Subject!.Name,
                item.HomeworkAssignment.Title,
                item.HomeworkAssignment.DueOn,
                item.Status,
                item.HomeworkAssignment.Instructions)).ToList(),
            outstandingFeeItems.Select(fee => new ParentPortalFeeItemDto(fee.FeeName, fee.DueOn, fee.BalanceAmount, fee.Status)).ToList(),
            examResults.Select(score => new ParentPortalExamResultDto(
                score.ExamSchedule!.ExamTerm!.Name,
                score.ExamSchedule.Subject!.Name,
                score.MarksObtained,
                score.ExamSchedule.MaxMarks,
                score.Grade,
                score.ResultStatus)).ToList(),
            todayTimetable,
            attendanceRecords.Take(5).Select(record => new ParentPortalAttendanceEntryDto(record.AttendanceDate, record.Status, record.Remarks)).ToList());
    }
}
