using EducationERP.Application.AcademicStructure;
using EducationERP.Application.Academics;
using EducationERP.Application.Admissions;
using EducationERP.Application.Attendance;
using EducationERP.Application.Campuses;
using EducationERP.Application.Examinations;
using EducationERP.Application.Fees;
using EducationERP.Application.Homework;
using EducationERP.Application.ParentPortal;
using EducationERP.Application.Students;
using EducationERP.Infrastructure;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Education ERP API",
        Version = "v1",
        Description = "Starter API for a school-focused Education ERP."
    });
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Education ERP API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("Frontend");

app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Ok(new
{
    name = "Education ERP API",
    version = "v1",
    modules = new[]
    {
        "Admissions",
        "Students",
        "Attendance",
        "Academics",
        "Fees",
        "Examinations",
        "Transport",
        "HR",
        "Inventory",
        "Parent Portal"
    }
}));

app.MapGet("/api/campuses", async (ICampusService campusService, CancellationToken cancellationToken) =>
{
    var campuses = await campusService.GetCampusesAsync(cancellationToken);
    return Results.Ok(campuses);
})
.WithName("GetCampuses");

app.MapGet("/api/academic-years", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var academicYears = await academicStructureService.GetAcademicYearsAsync(cancellationToken);
    return Results.Ok(academicYears);
})
.WithName("GetAcademicYears");

app.MapGet("/api/classes", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var classes = await academicStructureService.GetClassesAsync(cancellationToken);
    return Results.Ok(classes);
})
.WithName("GetClasses");

app.MapGet("/api/sections", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var sections = await academicStructureService.GetSectionsAsync(cancellationToken);
    return Results.Ok(sections);
})
.WithName("GetSections");

app.MapGet("/api/academic-structure", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var structure = await academicStructureService.GetAcademicStructureAsync(cancellationToken);
    return Results.Ok(structure);
})
.WithName("GetAcademicStructure");

app.MapGet("/api/academics/subjects", async (IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var subjects = await academicsService.GetSubjectsAsync(cancellationToken);
    return Results.Ok(subjects);
})
.WithName("GetSubjects");

app.MapGet("/api/academics/timetable", async (int? classId, int? sectionId, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var timetable = await academicsService.GetTimetableAsync(classId, sectionId, cancellationToken);
    return Results.Ok(timetable);
})
.WithName("GetTimetable");

app.MapGet("/api/academics/dashboard", async (int? classId, int? sectionId, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var dashboard = await academicsService.GetDashboardAsync(classId, sectionId, cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetAcademicsDashboard");

app.MapGet("/api/examinations/terms", async (IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var terms = await examinationsService.GetExamTermsAsync(cancellationToken);
    return Results.Ok(terms);
})
.WithName("GetExamTerms");

app.MapGet("/api/examinations/schedule", async (int? examTermId, int? classId, int? sectionId, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var schedule = await examinationsService.GetExamScheduleAsync(examTermId, classId, sectionId, cancellationToken);
    return Results.Ok(schedule);
})
.WithName("GetExamSchedule");

app.MapGet("/api/examinations/dashboard", async (int? examTermId, int? classId, int? sectionId, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var dashboard = await examinationsService.GetDashboardAsync(examTermId, classId, sectionId, cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetExaminationsDashboard");

app.MapGet("/api/homework/assignments", async (int? classId, int? sectionId, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var assignments = await homeworkService.GetAssignmentsAsync(classId, sectionId, cancellationToken);
    return Results.Ok(assignments);
})
.WithName("GetHomeworkAssignments");

app.MapGet("/api/homework/progress", async (int? classId, int? sectionId, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var progress = await homeworkService.GetProgressAsync(classId, sectionId, cancellationToken);
    return Results.Ok(progress);
})
.WithName("GetHomeworkProgress");

app.MapGet("/api/homework/dashboard", async (int? classId, int? sectionId, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var dashboard = await homeworkService.GetDashboardAsync(classId, sectionId, cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetHomeworkDashboard");

app.MapGet("/api/parent-portal/dashboard", async (int? studentId, IParentPortalService parentPortalService, CancellationToken cancellationToken) =>
{
    var dashboard = await parentPortalService.GetDashboardAsync(studentId, cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetParentPortalDashboard");

app.MapGet("/api/admissions/applications", async (IAdmissionsService admissionsService, CancellationToken cancellationToken) =>
{
    var applications = await admissionsService.GetApplicationsAsync(cancellationToken);
    return Results.Ok(applications);
})
.WithName("GetAdmissionApplications");

app.MapGet("/api/admissions/guardians", async (IAdmissionsService admissionsService, CancellationToken cancellationToken) =>
{
    var guardians = await admissionsService.GetGuardiansAsync(cancellationToken);
    return Results.Ok(guardians);
})
.WithName("GetAdmissionGuardians");

app.MapGet("/api/admissions/dashboard", async (IAdmissionsService admissionsService, CancellationToken cancellationToken) =>
{
    var dashboard = await admissionsService.GetDashboardAsync(cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetAdmissionsDashboard");

app.MapGet("/api/students", async (IStudentService studentService, CancellationToken cancellationToken) =>
{
    var students = await studentService.GetStudentsAsync(cancellationToken);
    return Results.Ok(students);
})
.WithName("GetStudents");

app.MapGet("/api/students/profile-overview", async (IStudentService studentService, CancellationToken cancellationToken) =>
{
    var profiles = await studentService.GetStudentProfileOverviewAsync(cancellationToken);
    return Results.Ok(profiles);
})
.WithName("GetStudentProfileOverview");

app.MapGet("/api/students/documents", async (IStudentService studentService, CancellationToken cancellationToken) =>
{
    var documents = await studentService.GetStudentDocumentsAsync(cancellationToken);
    return Results.Ok(documents);
})
.WithName("GetStudentDocuments");

app.MapGet("/api/fees/structures", async (IFeeService feeService, CancellationToken cancellationToken) =>
{
    var feeStructures = await feeService.GetFeeStructuresAsync(cancellationToken);
    return Results.Ok(feeStructures);
})
.WithName("GetFeeStructures");

app.MapGet("/api/fees/student-balances", async (IFeeService feeService, CancellationToken cancellationToken) =>
{
    var studentFees = await feeService.GetStudentFeesAsync(cancellationToken);
    return Results.Ok(studentFees);
})
.WithName("GetStudentFees");

app.MapGet("/api/fees/payments", async (IFeeService feeService, CancellationToken cancellationToken) =>
{
    var payments = await feeService.GetRecentPaymentsAsync(cancellationToken);
    return Results.Ok(payments);
})
.WithName("GetFeePayments");

app.MapGet("/api/fees/concessions", async (IFeeService feeService, CancellationToken cancellationToken) =>
{
    var concessions = await feeService.GetConcessionsAsync(cancellationToken);
    return Results.Ok(concessions);
})
.WithName("GetFeeConcessions");

app.MapGet("/api/fees/receipts", async (IFeeService feeService, CancellationToken cancellationToken) =>
{
    var receipts = await feeService.GetReceiptsAsync(cancellationToken);
    return Results.Ok(receipts);
})
.WithName("GetFeeReceipts");

app.MapGet("/api/fees/dashboard", async (IFeeService feeService, CancellationToken cancellationToken) =>
{
    var dashboard = await feeService.GetDashboardAsync(cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetFeesDashboard");

app.MapGet("/api/attendance/records", async (IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var records = await attendanceService.GetAttendanceRecordsAsync(cancellationToken);
    return Results.Ok(records);
})
.WithName("GetAttendanceRecords");

app.MapGet("/api/attendance/student-summary", async (IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var summaries = await attendanceService.GetStudentAttendanceSummaryAsync(cancellationToken);
    return Results.Ok(summaries);
})
.WithName("GetStudentAttendanceSummary");

app.MapGet("/api/attendance/class-summary", async (DateOnly? attendanceDate, int? classId, int? sectionId, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var summaries = await attendanceService.GetClassAttendanceSummaryAsync(attendanceDate, classId, sectionId, cancellationToken);
    return Results.Ok(summaries);
})
.WithName("GetClassAttendanceSummary");

app.MapGet("/api/attendance/monthly-report", async (DateOnly? referenceDate, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var report = await attendanceService.GetMonthlyReportAsync(referenceDate, cancellationToken);
    return Results.Ok(report);
})
.WithName("GetAttendanceMonthlyReport");

app.MapGet("/api/attendance/entry-board", async (DateOnly? attendanceDate, int? classId, int? sectionId, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var board = await attendanceService.GetEntryBoardAsync(attendanceDate, classId, sectionId, cancellationToken);
    return Results.Ok(board);
})
.WithName("GetAttendanceEntryBoard");

app.MapPost("/api/attendance/entry-board", async (AttendanceEntryBoardSaveRequestDto request, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    try
    {
        var board = await attendanceService.SaveEntryBoardAsync(request, cancellationToken);
        return Results.Ok(board);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["attendance"] = [exception.Message]
        });
    }
})
.WithName("SaveAttendanceEntryBoard");

app.MapGet("/api/attendance/class-register", async (DateOnly? referenceDate, int? classId, int? sectionId, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var register = await attendanceService.GetClassRegisterAsync(referenceDate, classId, sectionId, cancellationToken);
    return Results.Ok(register);
})
.WithName("GetAttendanceClassRegister");

app.MapGet("/api/attendance/dashboard", async (DateOnly? attendanceDate, int? classId, int? sectionId, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var dashboard = await attendanceService.GetDashboardAsync(attendanceDate, classId, sectionId, cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetAttendanceDashboard");

app.MapGet("/api/attendance/leave-requests", async (DateOnly? attendanceDate, int? classId, int? sectionId, IAttendanceService attendanceService, CancellationToken cancellationToken) =>
{
    var requests = await attendanceService.GetLeaveRequestsAsync(attendanceDate, classId, sectionId, cancellationToken);
    return Results.Ok(requests);
})
.WithName("GetAttendanceLeaveRequests");

app.MapPost("/api/attendance/leave-requests/{leaveRequestId:int}/status", async (
    int leaveRequestId,
    AttendanceLeaveDecisionRequestDto request,
    DateOnly? attendanceDate,
    int? classId,
    int? sectionId,
    IAttendanceService attendanceService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var requests = await attendanceService.UpdateLeaveRequestStatusAsync(leaveRequestId, request, attendanceDate, classId, sectionId, cancellationToken);
        return Results.Ok(requests);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["leaveRequest"] = [exception.Message]
        });
    }
})
.WithName("UpdateAttendanceLeaveRequestStatus");

app.MapPost("/api/setup/database/migrate", async (EducationErpDbContext dbContext, CancellationToken cancellationToken) =>
{
    await dbContext.Database.MigrateAsync(cancellationToken);
    return Results.Accepted("/health", new { status = "Database migrated" });
})
.WithName("MigrateDatabase");

app.Run();

public partial class Program;
