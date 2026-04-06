using System.Net;
using System.Net.Http.Json;
using EducationERP.Application.Admissions;
using EducationERP.Application.AcademicStructure;
using EducationERP.Application.Academics;
using EducationERP.Application.Attendance;
using EducationERP.Api.Tests.Infrastructure;
using EducationERP.Application.Campuses;
using EducationERP.Application.Examinations;
using EducationERP.Application.Fees;
using EducationERP.Application.Homework;
using EducationERP.Application.ParentPortal;
using EducationERP.Application.Students;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace EducationERP.Api.Tests.Endpoints;

public sealed class ApiEndpointsTests(EducationErpApiFactory factory) : IClassFixture<EducationErpApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient(new WebApplicationFactoryClientOptions
    {
        BaseAddress = new Uri("https://localhost")
    });

    [Fact]
    public async Task RootEndpoint_ReturnsApiMetadata()
    {
        var response = await _client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<RootResponse>();

        Assert.NotNull(payload);
        Assert.Equal("Education ERP API", payload.Name);
        Assert.Contains("Admissions", payload.Modules);
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsHealthyStatusCode()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CampusesEndpoint_ReturnsConfiguredCampuses()
    {
        var campuses = await _client.GetFromJsonAsync<List<CampusDto>>("/api/campuses");

        Assert.NotNull(campuses);
        Assert.Single(campuses);
        Assert.Equal("Test Campus", campuses[0].Name);
    }

    [Fact]
    public async Task AcademicStructureEndpoint_ReturnsAcademicMasterData()
    {
        var structure = await _client.GetFromJsonAsync<AcademicStructureDto>("/api/academic-structure");

        Assert.NotNull(structure);
        Assert.Single(structure.AcademicYears);
        Assert.Single(structure.Classes);
        Assert.Single(structure.Sections);
        Assert.Equal("2026-2027", structure.AcademicYears[0].Name);
        Assert.Equal("Grade 1", structure.Classes[0].Name);
        Assert.Equal("A", structure.Sections[0].Name);
    }

    [Fact]
    public async Task AcademicsDashboardEndpoint_ReturnsSubjectAndTimetableData()
    {
        var dashboard = await _client.GetFromJsonAsync<AcademicsDashboardDto>("/api/academics/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal("Grade 1", dashboard.SelectedClassName);
        Assert.Equal("B", dashboard.SelectedSectionName);
        Assert.Equal(3, dashboard.SubjectCount);
        Assert.Single(dashboard.WeeklyTimetable);
    }

    [Fact]
    public async Task ExaminationsDashboardEndpoint_ReturnsScheduleAndReportCardData()
    {
        var dashboard = await _client.GetFromJsonAsync<ExaminationsDashboardDto>("/api/examinations/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal("Term 1 Assessment", dashboard.SelectedExamTermName);
        Assert.Equal("Grade 1", dashboard.SelectedClassName);
        Assert.Equal("B", dashboard.SelectedSectionName);
        Assert.Equal(2, dashboard.Schedule.Count);
        Assert.Single(dashboard.ReportCards);
    }

    [Fact]
    public async Task ParentPortalDashboardEndpoint_ReturnsStudentFacingSnapshot()
    {
        var dashboard = await _client.GetFromJsonAsync<ParentPortalDashboardDto>("/api/parent-portal/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal("Ishita Verma", dashboard.StudentName);
        Assert.Equal("Anjali Verma", dashboard.GuardianName);
        Assert.Single(dashboard.UpcomingHomework);
        Assert.Single(dashboard.OutstandingFeeItems);
        Assert.Single(dashboard.ExamResults);
    }

    [Fact]
    public async Task HomeworkDashboardEndpoint_ReturnsAssignmentsAndProgress()
    {
        var dashboard = await _client.GetFromJsonAsync<HomeworkDashboardDto>("/api/homework/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal("Grade 1", dashboard.SelectedClassName);
        Assert.Equal("B", dashboard.SelectedSectionName);
        Assert.Single(dashboard.Assignments);
        Assert.Single(dashboard.Progress);
    }

    [Fact]
    public async Task AdmissionsDashboardEndpoint_ReturnsAdmissionsData()
    {
        var dashboard = await _client.GetFromJsonAsync<AdmissionsDashboardDto>("/api/admissions/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal(1, dashboard.TotalApplications);
        Assert.Equal(1, dashboard.NewApplications);
        Assert.Single(dashboard.RecentApplications);
        Assert.Single(dashboard.Guardians);
        Assert.Equal("Aarav Sharma", dashboard.RecentApplications[0].StudentName);
    }

    [Fact]
    public async Task StudentsEndpoint_ReturnsEnrolledStudents()
    {
        var students = await _client.GetFromJsonAsync<List<StudentDto>>("/api/students");

        Assert.NotNull(students);
        Assert.Single(students);
        Assert.Equal("STU-2026-001", students[0].AdmissionNumber);
        Assert.Equal("Active", students[0].Status);
    }

    [Fact]
    public async Task StudentProfileOverviewEndpoint_ReturnsProfileCompletionData()
    {
        var profiles = await _client.GetFromJsonAsync<List<StudentProfileOverviewDto>>("/api/students/profile-overview");

        Assert.NotNull(profiles);
        Assert.Single(profiles);
        Assert.Equal(100, profiles[0].ProfileCompletionPercentage);
        Assert.Equal(1, profiles[0].PendingDocumentCount);
        Assert.Equal("B+", profiles[0].BloodGroup);
    }

    [Fact]
    public async Task StudentDocumentsEndpoint_ReturnsDocumentTrackingData()
    {
        var documents = await _client.GetFromJsonAsync<List<StudentDocumentDto>>("/api/students/documents");

        Assert.NotNull(documents);
        Assert.Equal(2, documents.Count);
        Assert.Contains(documents, document => document.Status == "Pending");
        Assert.Contains(documents, document => document.DocumentType == "Birth Certificate");
    }

    [Fact]
    public async Task FeesDashboardEndpoint_ReturnsFeeSummary()
    {
        var dashboard = await _client.GetFromJsonAsync<FeesDashboardDto>("/api/fees/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal(60000m, dashboard.TotalExpectedAmount);
        Assert.Equal(6000m, dashboard.TotalConcessionAmount);
        Assert.Equal(24000m, dashboard.TotalCollectedAmount);
        Assert.Equal(30000m, dashboard.OutstandingAmount);
        Assert.Equal(2, dashboard.OutstandingFees.Count);
    }

    [Fact]
    public async Task FeeStructuresEndpoint_ReturnsConfiguredFeeStructures()
    {
        var feeStructures = await _client.GetFromJsonAsync<List<FeeStructureDto>>("/api/fees/structures");

        Assert.NotNull(feeStructures);
        Assert.Equal(2, feeStructures.Count);
        Assert.Contains(feeStructures, feeStructure => feeStructure.FeeCode == "TUITION");
    }

    [Fact]
    public async Task AttendanceDashboardEndpoint_ReturnsAttendanceSummary()
    {
        var dashboard = await _client.GetFromJsonAsync<AttendanceDashboardDto>("/api/attendance/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal(1, dashboard.TotalStudentsMarked);
        Assert.Equal(1, dashboard.AbsentCount);
        Assert.Single(dashboard.TodayRecords);
        Assert.Single(dashboard.StudentSummaries);
    }

    [Fact]
    public async Task AttendanceRecordsEndpoint_ReturnsAttendanceRecords()
    {
        var records = await _client.GetFromJsonAsync<List<AttendanceRecordDto>>("/api/attendance/records");

        Assert.NotNull(records);
        Assert.Equal(2, records.Count);
        Assert.Contains(records, record => record.Status == "Absent");
    }

    [Fact]
    public async Task AttendanceClassSummaryEndpoint_ReturnsClassRollup()
    {
        var summaries = await _client.GetFromJsonAsync<List<ClassAttendanceSummaryDto>>("/api/attendance/class-summary");

        Assert.NotNull(summaries);
        Assert.Single(summaries);
        Assert.Equal("Grade 1", summaries[0].ClassName);
        Assert.Equal("B", summaries[0].SectionName);
    }

    [Fact]
    public async Task AttendanceMonthlyReportEndpoint_ReturnsMonthlyReport()
    {
        var report = await _client.GetFromJsonAsync<AttendanceMonthlyReportDto>("/api/attendance/monthly-report");

        Assert.NotNull(report);
        Assert.Equal("April 2026", report.MonthLabel);
        Assert.Equal(3, report.WorkingDays);
        Assert.Single(report.ClassSummaries);
        Assert.Single(report.StudentsNeedingAttention);
    }

    [Fact]
    public async Task AttendanceEntryBoardEndpoint_ReturnsEntryBoard()
    {
        var board = await _client.GetFromJsonAsync<AttendanceEntryBoardDto>("/api/attendance/entry-board");

        Assert.NotNull(board);
        Assert.Equal(1, board.ClassId);
        Assert.Equal("Grade 1", board.ClassName);
        Assert.Equal(2, board.SectionId);
        Assert.Equal("B", board.SectionName);
        Assert.Single(board.Students);
        Assert.Single(board.UpcomingHolidays);
    }

    [Fact]
    public async Task AttendanceClassRegisterEndpoint_ReturnsRegisterView()
    {
        var register = await _client.GetFromJsonAsync<ClassAttendanceRegisterDto>("/api/attendance/class-register");

        Assert.NotNull(register);
        Assert.Equal("April 2026", register.MonthLabel);
        Assert.Equal(1, register.ClassId);
        Assert.Equal(3, register.WorkingDayLabels.Count);
        Assert.Single(register.Rows);
    }

    [Fact]
    public async Task AttendanceEntryBoardSaveEndpoint_PersistsMarkedStatuses()
    {
        var response = await _client.PostAsJsonAsync("/api/attendance/entry-board", new AttendanceEntryBoardSaveRequestDto(
            new DateOnly(2026, 4, 3),
            [
                new AttendanceEntrySaveItemDto(1, "Present", "Joined morning assembly")
            ]));

        response.EnsureSuccessStatusCode();

        var board = await response.Content.ReadFromJsonAsync<AttendanceEntryBoardDto>();

        Assert.NotNull(board);
        Assert.Equal(1, board.StudentsMarked);
        Assert.Single(board.Students);
        Assert.Equal("Present", board.Students[0].Status);
        Assert.Equal("Joined morning assembly", board.Students[0].Remarks);
    }

    [Fact]
    public async Task AttendanceEntryBoardEndpoint_RespectsSelectedDate()
    {
        var board = await _client.GetFromJsonAsync<AttendanceEntryBoardDto>("/api/attendance/entry-board?attendanceDate=2026-04-02");

        Assert.NotNull(board);
        Assert.Equal(new DateOnly(2026, 4, 2), board.AttendanceDate);
    }

    [Fact]
    public async Task AttendanceLeaveRequestsEndpoint_ReturnsLeaveDeskRows()
    {
        var requests = await _client.GetFromJsonAsync<List<AttendanceLeaveRequestDto>>("/api/attendance/leave-requests");

        Assert.NotNull(requests);
        Assert.Single(requests);
        Assert.Equal("Approved", requests[0].Status);
    }

    [Fact]
    public async Task AttendanceLeaveDecisionEndpoint_UpdatesLeaveStatus()
    {
        var response = await _client.PostAsJsonAsync("/api/attendance/leave-requests/1/status", new AttendanceLeaveDecisionRequestDto("Rejected"));

        response.EnsureSuccessStatusCode();

        var requests = await response.Content.ReadFromJsonAsync<List<AttendanceLeaveRequestDto>>();

        Assert.NotNull(requests);
        Assert.Single(requests);
        Assert.Equal("Rejected", requests[0].Status);
    }

    private sealed record RootResponse(string Name, string Version, string[] Modules);
}
