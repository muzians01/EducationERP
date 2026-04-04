using EducationERP.Application.Attendance;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeAttendanceService : IAttendanceService
{
    private readonly List<AttendanceEntryStudentDto> _grade1SectionBStudents =
    [
        new AttendanceEntryStudentDto(1, "Ishita Verma", "STU-2026-001", "Absent", true, "Sick Leave", "Fever and rest advised")
    ];

    private readonly List<AttendanceLeaveRequestDto> _leaveRequests =
    [
        new AttendanceLeaveRequestDto(1, 1, "Ishita Verma", "STU-2026-001", "Grade 1", "B", new DateOnly(2026, 4, 3), "Sick Leave", "Fever and rest advised", "Approved")
    ];

    public Task<IReadOnlyList<AttendanceRecordDto>> GetAttendanceRecordsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AttendanceRecordDto> records =
        [
            new AttendanceRecordDto(1, "Ishita Verma", "STU-2026-001", "Grade 1", "B", new DateOnly(2026, 4, 3), "Absent", new DateTime(2026, 4, 3, 8, 30, 0, DateTimeKind.Local), "Parent informed class teacher"),
            new AttendanceRecordDto(2, "Ishita Verma", "STU-2026-001", "Grade 1", "B", new DateOnly(2026, 4, 2), "Late", new DateTime(2026, 4, 2, 8, 20, 0, DateTimeKind.Local), "Arrived after assembly")
        ];

        return Task.FromResult(records);
    }

    public Task<IReadOnlyList<StudentAttendanceSummaryDto>> GetStudentAttendanceSummaryAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<StudentAttendanceSummaryDto> summaries =
        [
            new StudentAttendanceSummaryDto(1, "Ishita Verma", "STU-2026-001", "Grade 1", "B", 1, 1, 1, 50.0m)
        ];

        return Task.FromResult(summaries);
    }

    public Task<IReadOnlyList<ClassAttendanceSummaryDto>> GetClassAttendanceSummaryAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ClassAttendanceSummaryDto> summaries =
        [
            new ClassAttendanceSummaryDto("Grade 1", "B", 1, 0, 1, 0, 0m)
        ];

        return Task.FromResult(summaries);
    }

    public Task<AttendanceMonthlyReportDto> GetMonthlyReportAsync(DateOnly? referenceDate = null, CancellationToken cancellationToken = default)
    {
        var monthDate = referenceDate ?? new DateOnly(2026, 4, 3);

        return Task.FromResult(new AttendanceMonthlyReportDto(
            monthDate.ToString("MMMM yyyy"),
            3,
            1,
            50.0m,
            [
                new ClassAttendanceSummaryDto("Grade 1", "B", 1, 1, 1, 1, 50.0m)
            ],
            [
                new StudentAttendanceSummaryDto(1, "Ishita Verma", "STU-2026-001", "Grade 1", "B", 1, 1, 1, 50.0m)
            ]));
    }

    public Task<AttendanceEntryBoardDto> GetEntryBoardAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AttendanceEntryBoardDto(
            attendanceDate ?? new DateOnly(2026, 4, 3),
            classId ?? 1,
            "Grade 1",
            sectionId ?? 2,
            "B",
            1,
            _grade1SectionBStudents.Count(student => student.Status != "Unmarked"),
            _grade1SectionBStudents,
            [
                new AttendanceHolidayDto(1, new DateOnly(2026, 4, 14), "Ambedkar Jayanti", "National Holiday")
            ]));
    }

    public Task<AttendanceEntryBoardDto> SaveEntryBoardAsync(AttendanceEntryBoardSaveRequestDto request, CancellationToken cancellationToken = default)
    {
        _grade1SectionBStudents.Clear();
        _grade1SectionBStudents.AddRange(request.Students.Select(student =>
            new AttendanceEntryStudentDto(
                student.StudentId,
                "Ishita Verma",
                "STU-2026-001",
                student.Status,
                student.Status == "Absent",
                student.Status == "Absent" ? "Sick Leave" : null,
                student.Remarks)));

        return GetEntryBoardAsync(request.AttendanceDate, 1, 2, cancellationToken);
    }

    public Task<ClassAttendanceRegisterDto> GetClassRegisterAsync(DateOnly? referenceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var monthDate = referenceDate ?? new DateOnly(2026, 4, 3);

        return Task.FromResult(new ClassAttendanceRegisterDto(
            monthDate.ToString("MMMM yyyy"),
            classId ?? 1,
            "Grade 1",
            sectionId ?? 2,
            "B",
            ["01", "02", "03"],
            [
                new ClassRegisterRowDto(
                    1,
                    "Ishita Verma",
                    "STU-2026-001",
                    new Dictionary<string, string>
                    {
                        ["01"] = "Present",
                        ["02"] = "Late",
                        ["03"] = "Absent"
                    },
                    1,
                    1,
                    1)
            ]));
    }

    public Task<AttendanceDashboardDto> GetDashboardAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new AttendanceDashboardDto(
            attendanceDate ?? new DateOnly(2026, 4, 3),
            1,
            0,
            1,
            0,
            [
                new AttendanceRecordDto(1, "Ishita Verma", "STU-2026-001", "Grade 1", "B", new DateOnly(2026, 4, 3), "Absent", new DateTime(2026, 4, 3, 8, 30, 0, DateTimeKind.Local), "Parent informed class teacher")
            ],
            [
                new StudentAttendanceSummaryDto(1, "Ishita Verma", "STU-2026-001", "Grade 1", "B", 1, 1, 1, 50.0m)
            ],
            [
                new ClassAttendanceSummaryDto("Grade 1", "B", 1, 0, 1, 0, 0m)
            ]));
    }

    public Task<IReadOnlyList<AttendanceLeaveRequestDto>> GetLeaveRequestsAsync(DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<AttendanceLeaveRequestDto>>(_leaveRequests);
    }

    public Task<IReadOnlyList<AttendanceLeaveRequestDto>> UpdateLeaveRequestStatusAsync(int leaveRequestId, AttendanceLeaveDecisionRequestDto request, DateOnly? attendanceDate = null, int? classId = null, int? sectionId = null, CancellationToken cancellationToken = default)
    {
        var index = _leaveRequests.FindIndex(item => item.Id == leaveRequestId);

        if (index >= 0)
        {
            _leaveRequests[index] = _leaveRequests[index] with { Status = request.Status };
        }

        return Task.FromResult<IReadOnlyList<AttendanceLeaveRequestDto>>(_leaveRequests);
    }
}
