using EducationERP.Application.Attendance;
using EducationERP.Domain.Entities;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Attendance;

internal sealed class AttendanceService(EducationErpDbContext dbContext) : IAttendanceService
{
    private static readonly string[] SupportedStatuses = ["Present", "Absent", "Late"];
    private static readonly string[] SupportedLeaveStatuses = ["Pending", "Approved", "Rejected"];

    public async Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Student)
            .ThenInclude(student => student!.SchoolClass)
            .Include(record => record.Student)
            .ThenInclude(student => student!.Section)
            .OrderByDescending(record => record.AttendanceDate)
            .ThenBy(record => record.Student!.AdmissionNumber)
            .Select(record => new AttendanceRecordDto(
                record.Id,
                $"{record.Student!.FirstName} {record.Student.LastName}",
                record.Student.AdmissionNumber,
                record.Student.SchoolClass!.Name,
                record.Student.Section!.Name,
                record.AttendanceDate,
                record.Status,
                record.MarkedOn,
                record.Remarks))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentAttendanceSummaryDto>> GetStudentAttendanceSummaryAsync(CancellationToken cancellationToken = default)
    {
        var records = await dbContext.AttendanceRecords
            .AsNoTracking()
            .Include(record => record.Student)
            .ThenInclude(student => student!.SchoolClass)
            .Include(record => record.Student)
            .ThenInclude(student => student!.Section)
            .ToListAsync(cancellationToken);

        return records
            .GroupBy(record => record.StudentId)
            .Select(group =>
            {
                var student = group.First().Student!;
                var total = group.Count();
                var present = group.Count(record => record.Status == "Present");
                var absent = group.Count(record => record.Status == "Absent");
                var late = group.Count(record => record.Status == "Late");

                return new StudentAttendanceSummaryDto(
                    student.Id,
                    $"{student.FirstName} {student.LastName}",
                    student.AdmissionNumber,
                    student.SchoolClass!.Name,
                    student.Section!.Name,
                    present,
                    absent,
                    late,
                    total == 0 ? 0 : decimal.Round((present + late * 0.5m) * 100 / total, 1));
            })
            .OrderBy(summary => summary.StudentName)
            .ToList();
    }

    public async Task<IReadOnlyList<ClassAttendanceSummaryDto>> GetClassAttendanceSummaryAsync(
        DateOnly? attendanceDate = null,
        int? classId = null,
        int? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        var records = await GetAttendanceRecordsAsync(cancellationToken);
        var effectiveDate = attendanceDate ?? records.MaxBy(record => record.AttendanceDate)?.AttendanceDate;

        if (effectiveDate is null)
        {
            return [];
        }

        var roster = await ResolveRosterAsync(effectiveDate.Value, classId, sectionId, cancellationToken);

        return records
            .Where(record =>
                record.AttendanceDate == effectiveDate.Value
                && record.ClassName == roster.ClassName
                && record.SectionName == roster.SectionName)
            .GroupBy(record => new { record.ClassName, record.SectionName })
            .Select(group =>
            {
                var total = group.Count();
                var present = group.Count(record => record.Status == "Present");
                var absent = group.Count(record => record.Status == "Absent");
                var late = group.Count(record => record.Status == "Late");

                return new ClassAttendanceSummaryDto(
                    group.Key.ClassName,
                    group.Key.SectionName,
                    total,
                    present,
                    absent,
                    late,
                    total == 0 ? 0 : decimal.Round((present + late * 0.5m) * 100 / total, 1));
            })
            .OrderBy(summary => summary.ClassName)
            .ThenBy(summary => summary.SectionName)
            .ToList();
    }

    public async Task<AttendanceMonthlyReportDto> GetMonthlyReportAsync(DateOnly? referenceDate = null, CancellationToken cancellationToken = default)
    {
        var records = await GetAttendanceRecordsAsync(cancellationToken);
        var summaries = await GetStudentAttendanceSummaryAsync(cancellationToken);
        var effectiveDate = referenceDate ?? records.MaxBy(record => record.AttendanceDate)?.AttendanceDate;

        if (effectiveDate is null)
        {
            return new AttendanceMonthlyReportDto("No data", 0, 0, 0, [], []);
        }

        var monthRecords = records
            .Where(record => record.AttendanceDate.Year == effectiveDate.Value.Year && record.AttendanceDate.Month == effectiveDate.Value.Month)
            .ToList();

        var classSummaries = monthRecords
            .GroupBy(record => new { record.ClassName, record.SectionName })
            .Select(group =>
            {
                var total = group.Count();
                var present = group.Count(record => record.Status == "Present");
                var absent = group.Count(record => record.Status == "Absent");
                var late = group.Count(record => record.Status == "Late");

                return new ClassAttendanceSummaryDto(
                    group.Key.ClassName,
                    group.Key.SectionName,
                    group.Select(record => record.AdmissionNumber).Distinct().Count(),
                    present,
                    absent,
                    late,
                    total == 0 ? 0 : decimal.Round((present + late * 0.5m) * 100 / total, 1));
            })
            .OrderBy(summary => summary.ClassName)
            .ThenBy(summary => summary.SectionName)
            .ToList();

        var workingDays = monthRecords.Select(record => record.AttendanceDate).Distinct().Count();
        var overallAttendance = monthRecords.Count == 0
            ? 0
            : decimal.Round((monthRecords.Count(record => record.Status == "Present") + monthRecords.Count(record => record.Status == "Late") * 0.5m) * 100 / monthRecords.Count, 1);

        return new AttendanceMonthlyReportDto(
            effectiveDate.Value.ToString("MMMM yyyy"),
            workingDays,
            monthRecords.Select(record => record.AdmissionNumber).Distinct().Count(),
            overallAttendance,
            classSummaries,
            summaries.Where(summary => summary.AttendancePercentage < 75).OrderBy(summary => summary.AttendancePercentage).ToList());
    }

    public async Task<AttendanceEntryBoardDto> GetEntryBoardAsync(
        DateOnly? attendanceDate = null,
        int? classId = null,
        int? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        var latestAttendanceDate = attendanceDate ?? await dbContext.AttendanceRecords
            .AsNoTracking()
            .MaxAsync(record => (DateOnly?)record.AttendanceDate, cancellationToken) ?? DateOnly.FromDateTime(DateTime.Now);

        var roster = await ResolveRosterAsync(latestAttendanceDate, classId, sectionId, cancellationToken);

        var leaveRequests = await dbContext.StudentLeaveRequests
            .AsNoTracking()
            .Where(request =>
                request.LeaveDate == latestAttendanceDate
                && request.Student!.SchoolClassId == roster.ClassId
                && request.Student.SectionId == roster.SectionId
                && request.Status == "Approved")
            .ToListAsync(cancellationToken);

        var records = await dbContext.AttendanceRecords
            .AsNoTracking()
            .Where(record =>
                record.AttendanceDate == latestAttendanceDate
                && record.Student!.SchoolClassId == roster.ClassId
                && record.Student.SectionId == roster.SectionId)
            .ToListAsync(cancellationToken);

        var upcomingHolidays = await dbContext.SchoolHolidays
            .AsNoTracking()
            .Where(holiday => holiday.HolidayDate >= latestAttendanceDate)
            .OrderBy(holiday => holiday.HolidayDate)
            .Take(3)
            .Select(holiday => new AttendanceHolidayDto(
                holiday.Id,
                holiday.HolidayDate,
                holiday.Title,
                holiday.Category))
            .ToListAsync(cancellationToken);

        var studentRows = roster.Students.Select(student =>
        {
            var record = records.FirstOrDefault(item => item.StudentId == student.Id);
            var leave = leaveRequests.FirstOrDefault(item => item.StudentId == student.Id);

            return new AttendanceEntryStudentDto(
                student.Id,
                $"{student.FirstName} {student.LastName}",
                student.AdmissionNumber,
                record?.Status ?? "Unmarked",
                leave is not null,
                leave?.LeaveType,
                leave?.Reason ?? record?.Remarks);
        }).ToList();

        return new AttendanceEntryBoardDto(
            latestAttendanceDate,
            roster.ClassId,
            roster.ClassName,
            roster.SectionId,
            roster.SectionName,
            roster.Students.Count,
            studentRows.Count(student => student.Status != "Unmarked"),
            studentRows,
            upcomingHolidays);
    }

    public async Task<AttendanceEntryBoardDto> SaveEntryBoardAsync(AttendanceEntryBoardSaveRequestDto request, CancellationToken cancellationToken = default)
    {
        if (request.Students.Count == 0)
        {
            throw new InvalidOperationException("At least one attendance row is required.");
        }

        var studentIds = request.Students.Select(student => student.StudentId).Distinct().ToList();
        var studentLookup = await dbContext.Students
            .AsNoTracking()
            .Where(student => studentIds.Contains(student.Id))
            .Select(student => new { student.Id, student.SchoolClassId, student.SectionId })
            .ToListAsync(cancellationToken);

        if (studentLookup.Count != studentIds.Count)
        {
            throw new InvalidOperationException("One or more students could not be found.");
        }

        if (studentLookup.Select(student => new { student.SchoolClassId, student.SectionId }).Distinct().Count() != 1)
        {
            throw new InvalidOperationException("Attendance rows must belong to the same class and section.");
        }

        var invalidStatus = request.Students
            .Select(student => student.Status.Trim())
            .FirstOrDefault(status => !SupportedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase));

        if (invalidStatus is not null)
        {
            throw new InvalidOperationException($"Unsupported attendance status '{invalidStatus}'.");
        }

        var attendanceDate = request.AttendanceDate;
        var markedOn = DateTime.Now;

        var existingRecords = await dbContext.AttendanceRecords
            .Where(record => record.AttendanceDate == attendanceDate && studentIds.Contains(record.StudentId))
            .ToListAsync(cancellationToken);

        foreach (var entry in request.Students)
        {
            var status = SupportedStatuses.First(supported =>
                string.Equals(supported, entry.Status.Trim(), StringComparison.OrdinalIgnoreCase));

            var existingRecord = existingRecords.FirstOrDefault(record => record.StudentId == entry.StudentId);

            if (existingRecord is null)
            {
                dbContext.AttendanceRecords.Add(new AttendanceRecord(
                    entry.StudentId,
                    attendanceDate,
                    status,
                    markedOn,
                    entry.Remarks));

                continue;
            }

            existingRecord.Mark(status, markedOn, entry.Remarks);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var roster = studentLookup[0];
        return await GetEntryBoardAsync(attendanceDate, roster.SchoolClassId, roster.SectionId, cancellationToken);
    }

    public async Task<ClassAttendanceRegisterDto> GetClassRegisterAsync(
        DateOnly? referenceDate = null,
        int? classId = null,
        int? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        var records = await GetAttendanceRecordsAsync(cancellationToken);
        var effectiveDate = referenceDate ?? records.MaxBy(record => record.AttendanceDate)?.AttendanceDate;

        if (effectiveDate is null)
        {
            return new ClassAttendanceRegisterDto("No data", 0, string.Empty, 0, string.Empty, [], []);
        }

        var roster = await ResolveRosterAsync(effectiveDate.Value, classId, sectionId, cancellationToken);

        var monthRecords = records
            .Where(record =>
                record.AttendanceDate.Year == effectiveDate.Value.Year
                && record.AttendanceDate.Month == effectiveDate.Value.Month
                && record.ClassName == roster.ClassName
                && record.SectionName == roster.SectionName)
            .OrderBy(record => record.AttendanceDate)
            .ToList();

        var workingDays = monthRecords
            .Select(record => record.AttendanceDate)
            .Distinct()
            .OrderBy(day => day)
            .Select(day => day.ToString("dd"))
            .ToList();

        var rows = roster.Students
            .Select(student =>
            {
                var studentRecords = monthRecords
                    .Where(record => record.AdmissionNumber == student.AdmissionNumber)
                    .ToList();

                var dailyStatus = studentRecords.ToDictionary(record => record.AttendanceDate.ToString("dd"), record => record.Status);

                return new ClassRegisterRowDto(
                    student.Id,
                    $"{student.FirstName} {student.LastName}",
                    student.AdmissionNumber,
                    dailyStatus,
                    studentRecords.Count(record => record.Status == "Present"),
                    studentRecords.Count(record => record.Status == "Absent"),
                    studentRecords.Count(record => record.Status == "Late"));
            })
            .OrderBy(row => row.StudentName)
            .ToList();

        return new ClassAttendanceRegisterDto(
            effectiveDate.Value.ToString("MMMM yyyy"),
            roster.ClassId,
            roster.ClassName,
            roster.SectionId,
            roster.SectionName,
            workingDays,
            rows);
    }

    public async Task<AttendanceDashboardDto> GetDashboardAsync(
        DateOnly? attendanceDate = null,
        int? classId = null,
        int? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        var records = await GetAttendanceRecordsAsync(cancellationToken);
        var summaries = await GetStudentAttendanceSummaryAsync(cancellationToken);
        var effectiveDate = attendanceDate ?? records.MaxBy(record => record.AttendanceDate)?.AttendanceDate;

        if (effectiveDate is null)
        {
            return new AttendanceDashboardDto(DateOnly.MinValue, 0, 0, 0, 0, [], [], []);
        }

        var roster = await ResolveRosterAsync(effectiveDate.Value, classId, sectionId, cancellationToken);
        var classSummaries = await GetClassAttendanceSummaryAsync(effectiveDate, roster.ClassId, roster.SectionId, cancellationToken);
        var todayRecords = records
            .Where(record =>
                record.AttendanceDate == effectiveDate.Value
                && record.ClassName == roster.ClassName
                && record.SectionName == roster.SectionName)
            .ToList();

        return new AttendanceDashboardDto(
            effectiveDate.Value,
            todayRecords.Count,
            todayRecords.Count(record => record.Status == "Present"),
            todayRecords.Count(record => record.Status == "Absent"),
            todayRecords.Count(record => record.Status == "Late"),
            todayRecords,
            summaries.Where(summary => summary.ClassName == roster.ClassName && summary.SectionName == roster.SectionName).ToList(),
            classSummaries);
    }

    public async Task<IReadOnlyList<AttendanceLeaveRequestDto>> GetLeaveRequestsAsync(
        DateOnly? attendanceDate = null,
        int? classId = null,
        int? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        var effectiveDate = attendanceDate ?? await dbContext.StudentLeaveRequests
            .AsNoTracking()
            .MaxAsync(request => (DateOnly?)request.LeaveDate, cancellationToken) ?? DateOnly.FromDateTime(DateTime.Now);

        var roster = await ResolveRosterAsync(effectiveDate, classId, sectionId, cancellationToken);

        return await dbContext.StudentLeaveRequests
            .AsNoTracking()
            .Include(request => request.Student)
            .ThenInclude(student => student!.SchoolClass)
            .Include(request => request.Student)
            .ThenInclude(student => student!.Section)
            .Where(request =>
                request.LeaveDate == effectiveDate
                && request.Student!.SchoolClassId == roster.ClassId
                && request.Student.SectionId == roster.SectionId)
            .OrderBy(request => request.Status)
            .ThenBy(request => request.Student!.AdmissionNumber)
            .Select(request => new AttendanceLeaveRequestDto(
                request.Id,
                request.StudentId,
                $"{request.Student!.FirstName} {request.Student.LastName}",
                request.Student.AdmissionNumber,
                request.Student.SchoolClass!.Name,
                request.Student.Section!.Name,
                request.LeaveDate,
                request.LeaveType,
                request.Reason,
                request.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AttendanceLeaveRequestDto>> UpdateLeaveRequestStatusAsync(
        int leaveRequestId,
        AttendanceLeaveDecisionRequestDto request,
        DateOnly? attendanceDate = null,
        int? classId = null,
        int? sectionId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedStatus = request.Status.Trim();

        if (!SupportedLeaveStatuses.Contains(normalizedStatus, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Unsupported leave status '{request.Status}'.");
        }

        var leaveRequest = await dbContext.StudentLeaveRequests
            .Include(item => item.Student)
            .FirstOrDefaultAsync(item => item.Id == leaveRequestId, cancellationToken);

        if (leaveRequest is null)
        {
            throw new InvalidOperationException("Leave request could not be found.");
        }

        var finalStatus = SupportedLeaveStatuses.First(status =>
            string.Equals(status, normalizedStatus, StringComparison.OrdinalIgnoreCase));

        leaveRequest.UpdateStatus(finalStatus);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await GetLeaveRequestsAsync(attendanceDate ?? leaveRequest.LeaveDate, classId ?? leaveRequest.Student!.SchoolClassId, sectionId ?? leaveRequest.Student!.SectionId, cancellationToken);
    }

    private async Task<RosterContext> ResolveRosterAsync(DateOnly attendanceDate, int? classId, int? sectionId, CancellationToken cancellationToken)
    {
        var students = await dbContext.Students
            .AsNoTracking()
            .Include(student => student.SchoolClass)
            .Include(student => student.Section)
            .Where(student => !classId.HasValue || student.SchoolClassId == classId.Value)
            .Where(student => !sectionId.HasValue || student.SectionId == sectionId.Value)
            .OrderBy(student => student.SchoolClass!.DisplayOrder)
            .ThenBy(student => student.Section!.Name)
            .ThenBy(student => student.AdmissionNumber)
            .ToListAsync(cancellationToken);

        if (students.Count == 0)
        {
            throw new InvalidOperationException("No students found for the selected class and section.");
        }

        var selectedStudents = students;

        if (!classId.HasValue || !sectionId.HasValue)
        {
            var firstGroup = students
                .GroupBy(student => new
                {
                    student.SchoolClassId,
                    ClassName = student.SchoolClass!.Name,
                    student.SectionId,
                    SectionName = student.Section!.Name
                })
                .OrderBy(group => group.Key.ClassName)
                .ThenBy(group => group.Key.SectionName)
                .First();

            selectedStudents = firstGroup.ToList();

            return new RosterContext(
                attendanceDate,
                firstGroup.Key.SchoolClassId,
                firstGroup.Key.ClassName,
                firstGroup.Key.SectionId,
                firstGroup.Key.SectionName,
                selectedStudents);
        }

        var firstStudent = selectedStudents.First();

        return new RosterContext(
            attendanceDate,
            firstStudent.SchoolClassId,
            firstStudent.SchoolClass!.Name,
            firstStudent.SectionId,
            firstStudent.Section!.Name,
            selectedStudents);
    }

    private sealed record RosterContext(
        DateOnly AttendanceDate,
        int ClassId,
        string ClassName,
        int SectionId,
        string SectionName,
        IReadOnlyList<Student> Students);
}
