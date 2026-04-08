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
using EducationERP.Application.Transport;
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
    public async Task CampusCreationEndpoint_CreatesCampus()
    {
        var response = await _client.PostAsJsonAsync("/api/campuses", new CreateCampusDto(
            "NORTH",
            "North Campus",
            "Mysuru",
            "Karnataka",
            "India",
            "ICSE"));

        response.EnsureSuccessStatusCode();

        var campus = await response.Content.ReadFromJsonAsync<CampusDto>();

        Assert.NotNull(campus);
        Assert.Equal("NORTH", campus.Code);
        Assert.Equal("North Campus", campus.Name);
    }

    [Fact]
    public async Task AcademicYearCreationEndpoint_CreatesAcademicYear()
    {
        var response = await _client.PostAsJsonAsync("/api/academic-structure/academic-years", new CreateAcademicYearDto(
            1,
            "2027-2028",
            new DateOnly(2027, 6, 1),
            new DateOnly(2028, 3, 31),
            false));

        response.EnsureSuccessStatusCode();

        var academicYear = await response.Content.ReadFromJsonAsync<AcademicYearDto>();

        Assert.NotNull(academicYear);
        Assert.Equal("2027-2028", academicYear.Name);
        Assert.False(academicYear.IsActive);
    }

    [Fact]
    public async Task SchoolClassCreationEndpoint_CreatesClass()
    {
        var response = await _client.PostAsJsonAsync("/api/academic-structure/classes", new CreateSchoolClassDto(
            1,
            "GRADE-2",
            "Grade 2",
            2));

        response.EnsureSuccessStatusCode();

        var schoolClass = await response.Content.ReadFromJsonAsync<SchoolClassDto>();

        Assert.NotNull(schoolClass);
        Assert.Equal("GRADE-2", schoolClass.Code);
        Assert.Equal(2, schoolClass.DisplayOrder);
    }

    [Fact]
    public async Task SectionUpdateEndpoint_UpdatesSection()
    {
        var response = await _client.PutAsJsonAsync("/api/academic-structure/sections/1", new UpdateSectionDto(
            1,
            "B",
            40,
            "G1-B01"));

        response.EnsureSuccessStatusCode();

        var section = await response.Content.ReadFromJsonAsync<SectionDto>();

        Assert.NotNull(section);
        Assert.Equal("B", section.Name);
        Assert.Equal(40, section.Capacity);
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
    public async Task SubjectCreationEndpoint_CreatesSubject()
    {
        var response = await _client.PostAsJsonAsync("/api/academics/subjects", new CreateSubjectDto(
            1,
            "SST",
            "Social Studies",
            "Core",
            4));

        response.EnsureSuccessStatusCode();

        var subject = await response.Content.ReadFromJsonAsync<SubjectDto>();

        Assert.NotNull(subject);
        Assert.Equal("SST", subject.Code);
        Assert.Equal("Social Studies", subject.Name);
    }

    [Fact]
    public async Task SubjectUpdateEndpoint_UpdatesSubject()
    {
        var response = await _client.PutAsJsonAsync("/api/academics/subjects/1", new UpdateSubjectDto(
            "ENG",
            "English Language",
            "Language",
            6));

        response.EnsureSuccessStatusCode();

        var subject = await response.Content.ReadFromJsonAsync<SubjectDto>();

        Assert.NotNull(subject);
        Assert.Equal("English Language", subject.Name);
    }

    [Fact]
    public async Task TimetableCreationEndpoint_CreatesTimetableSlot()
    {
        var response = await _client.PostAsJsonAsync("/api/academics/timetable", new CreateTimetablePeriodDto(
            1,
            1,
            2,
            1,
            "Tuesday",
            3,
            new TimeOnly(10, 0),
            new TimeOnly(10, 40),
            "Anita Rao",
            "G1-B01"));

        response.EnsureSuccessStatusCode();

        var period = await response.Content.ReadFromJsonAsync<TimetablePeriodDto>();

        Assert.NotNull(period);
        Assert.Equal("Tuesday", period.DayOfWeek);
        Assert.Equal(3, period.PeriodNumber);
    }

    [Fact]
    public async Task TimetableUpdateEndpoint_UpdatesTimetableSlot()
    {
        var response = await _client.PutAsJsonAsync("/api/academics/timetable/1", new UpdateTimetablePeriodDto(
            1,
            1,
            2,
            2,
            "Monday",
            1,
            new TimeOnly(8, 45),
            new TimeOnly(9, 25),
            "Rahul Mehta",
            "G1-B02"));

        response.EnsureSuccessStatusCode();

        var period = await response.Content.ReadFromJsonAsync<TimetablePeriodDto>();

        Assert.NotNull(period);
        Assert.Equal("Rahul Mehta", period.TeacherName);
        Assert.Equal("G1-B02", period.RoomNumber);
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
    public async Task ExamTermCreationEndpoint_CreatesExamTerm()
    {
        var response = await _client.PostAsJsonAsync("/api/examinations/terms", new CreateExamTermDto(
            1,
            1,
            "Mid Term",
            "Scholastic",
            new DateOnly(2026, 11, 10),
            new DateOnly(2026, 11, 15),
            "Draft"));

        response.EnsureSuccessStatusCode();

        var term = await response.Content.ReadFromJsonAsync<ExamTermDto>();

        Assert.NotNull(term);
        Assert.Equal("Mid Term", term.Name);
        Assert.Equal("Draft", term.Status);
    }

    [Fact]
    public async Task ExamScheduleCreationEndpoint_CreatesSchedule()
    {
        var response = await _client.PostAsJsonAsync("/api/examinations/schedule", new CreateExamScheduleDto(
            1,
            1,
            2,
            1,
            new DateOnly(2026, 9, 16),
            new TimeOnly(11, 0),
            60,
            80,
            28));

        response.EnsureSuccessStatusCode();

        var schedule = await response.Content.ReadFromJsonAsync<ExamScheduleDto>();

        Assert.NotNull(schedule);
        Assert.Equal(new DateOnly(2026, 9, 16), schedule.ExamDate);
        Assert.Equal(80, schedule.MaxMarks);
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
    public async Task TransportDashboardEndpoint_ReturnsTransportSnapshot()
    {
        var dashboard = await _client.GetFromJsonAsync<TransportDashboardDto>("/api/transport/dashboard");

        Assert.NotNull(dashboard);
        Assert.Equal(3, dashboard.TotalRoutes);
        Assert.Equal(3, dashboard.TotalVehicles);
        Assert.Equal(2, dashboard.ActiveTrips);
        Assert.Equal(67, dashboard.CapacityUtilizationPercentage);
    }

    [Fact]
    public async Task TransportRouteCreationEndpoint_CreatesRoute()
    {
        var response = await _client.PostAsJsonAsync("/api/transport/routes", new CreateTransportRouteDto(
            "South Loop",
            "Main Campus",
            "South Campus",
            "Active"));

        response.EnsureSuccessStatusCode();

        var route = await response.Content.ReadFromJsonAsync<TransportRouteDto>();

        Assert.NotNull(route);
        Assert.Equal("South Loop", route.RouteName);
        Assert.Equal("Main Campus", route.Origin);
    }

    [Fact]
    public async Task TransportVehicleCreationEndpoint_CreatesVehicle()
    {
        var response = await _client.PostAsJsonAsync("/api/transport/vehicles", new CreateTransportVehicleDto(
            "erp-110",
            "Mini Bus",
            24,
            1,
            "Idle"));

        response.EnsureSuccessStatusCode();

        var vehicle = await response.Content.ReadFromJsonAsync<TransportVehicleDto>();

        Assert.NotNull(vehicle);
        Assert.Equal("ERP-110", vehicle.VehicleNumber);
        Assert.Equal("North Campus Shuttle", vehicle.AssignedRoute);
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
    public async Task HomeworkAssignmentCreationEndpoint_CreatesAssignment()
    {
        var response = await _client.PostAsJsonAsync("/api/homework/assignments", new CreateHomeworkAssignmentDto(
            1,
            1,
            2,
            1,
            new DateOnly(2026, 4, 8),
            new DateOnly(2026, 4, 10),
            "Grammar worksheet",
            "Complete pages 12 and 13.",
            "Anita Rao"));

        response.EnsureSuccessStatusCode();

        var assignment = await response.Content.ReadFromJsonAsync<HomeworkAssignmentDto>();

        Assert.NotNull(assignment);
        Assert.Equal("Grammar worksheet", assignment.Title);
    }

    [Fact]
    public async Task HomeworkSubmissionUpdateEndpoint_UpdatesProgress()
    {
        var response = await _client.PutAsJsonAsync("/api/homework/progress", new UpdateHomeworkSubmissionDto(
            1,
            1,
            "Submitted",
            new DateOnly(2026, 4, 8),
            "Submitted before evening"));

        response.EnsureSuccessStatusCode();

        var progress = await response.Content.ReadFromJsonAsync<StudentHomeworkProgressDto>();

        Assert.NotNull(progress);
        Assert.Equal("Submitted", progress.Status);
        Assert.Equal(new DateOnly(2026, 4, 8), progress.SubmittedOn);
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
    public async Task AdmissionApplicationCreationEndpoint_CreatesApplication()
    {
        var response = await _client.PostAsJsonAsync("/api/admissions/applications", new CreateAdmissionApplicationDto(
            1,
            1,
            1,
            1,
            1,
            "Vihaan",
            "Kapoor",
            new DateOnly(2020, 2, 5),
            "Male",
            1500m));

        response.EnsureSuccessStatusCode();

        var applicationId = await response.Content.ReadFromJsonAsync<int>();

        Assert.True(applicationId > 0);
    }

    [Fact]
    public async Task AdmissionApplicationStatusEndpoint_UpdatesStatus()
    {
        var response = await _client.PutAsJsonAsync("/api/admissions/applications/1/status", new UpdateAdmissionApplicationStatusDto("Approved"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
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
    public async Task FeePaymentEndpoint_RecordsPayment()
    {
        var response = await _client.PostAsJsonAsync("/api/fees/payments", new RecordFeePaymentDto(
            2,
            4000m,
            "UPI",
            "UTR-4000",
            new DateOnly(2026, 4, 8)));

        response.EnsureSuccessStatusCode();

        var paymentId = await response.Content.ReadFromJsonAsync<int>();

        Assert.True(paymentId > 0);
    }

    [Fact]
    public async Task FeeReceiptEndpoint_ReturnsReceipt()
    {
        var receipt = await _client.GetFromJsonAsync<FeeReceiptDto>("/api/fees/receipts/1");

        Assert.NotNull(receipt);
        Assert.Equal("RCPT-2026-001", receipt.ReceiptNumber);
        Assert.Equal("Ishita Verma", receipt.StudentName);
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
    public async Task AttendanceLeaveRequestCreationEndpoint_CreatesLeaveRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/attendance/leave-requests", new CreateAttendanceLeaveRequestDto(
            1,
            new DateOnly(2026, 4, 4),
            "Personal Leave",
            "Family event"));

        response.EnsureSuccessStatusCode();

        var request = await response.Content.ReadFromJsonAsync<AttendanceLeaveRequestDto>();

        Assert.NotNull(request);
        Assert.Equal("Pending", request.Status);
        Assert.Equal("Personal Leave", request.LeaveType);
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
