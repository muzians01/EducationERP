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
using EducationERP.Application.Transport;
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

app.MapPost("/api/campuses", async (CreateCampusDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var campus = await academicStructureService.CreateCampusAsync(dto, cancellationToken);
    return Results.Created($"/api/campuses/{campus.Id}", campus);
})
.WithName("CreateCampus");

app.MapPut("/api/campuses/{campusId}", async (int campusId, UpdateCampusDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var campus = await academicStructureService.UpdateCampusAsync(campusId, dto, cancellationToken);
    return Results.Ok(campus);
})
.WithName("UpdateCampus");

app.MapDelete("/api/campuses/{campusId}", async (int campusId, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    await academicStructureService.DeleteCampusAsync(campusId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteCampus");

app.MapGet("/api/academic-years", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var academicYears = await academicStructureService.GetAcademicYearsAsync(cancellationToken);
    return Results.Ok(academicYears);
})
.WithName("GetAcademicYears");

app.MapPost("/api/academic-structure/academic-years", async (CreateAcademicYearDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var academicYear = await academicStructureService.CreateAcademicYearAsync(dto, cancellationToken);
    return Results.Created($"/api/academic-structure/academic-years/{academicYear.Id}", academicYear);
})
.WithName("CreateAcademicYear");

app.MapPut("/api/academic-structure/academic-years/{academicYearId}", async (int academicYearId, UpdateAcademicYearDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var academicYear = await academicStructureService.UpdateAcademicYearAsync(academicYearId, dto, cancellationToken);
    return Results.Ok(academicYear);
})
.WithName("UpdateAcademicYear");

app.MapDelete("/api/academic-structure/academic-years/{academicYearId}", async (int academicYearId, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    await academicStructureService.DeleteAcademicYearAsync(academicYearId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteAcademicYear");

app.MapGet("/api/classes", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var classes = await academicStructureService.GetClassesAsync(cancellationToken);
    return Results.Ok(classes);
})
.WithName("GetClasses");

app.MapPost("/api/academic-structure/classes", async (CreateSchoolClassDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var schoolClass = await academicStructureService.CreateSchoolClassAsync(dto, cancellationToken);
    return Results.Created($"/api/academic-structure/classes/{schoolClass.Id}", schoolClass);
})
.WithName("CreateSchoolClass");

app.MapPut("/api/academic-structure/classes/{schoolClassId}", async (int schoolClassId, UpdateSchoolClassDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var schoolClass = await academicStructureService.UpdateSchoolClassAsync(schoolClassId, dto, cancellationToken);
    return Results.Ok(schoolClass);
})
.WithName("UpdateSchoolClass");

app.MapDelete("/api/academic-structure/classes/{schoolClassId}", async (int schoolClassId, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    await academicStructureService.DeleteSchoolClassAsync(schoolClassId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteSchoolClass");

app.MapGet("/api/sections", async (IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var sections = await academicStructureService.GetSectionsAsync(cancellationToken);
    return Results.Ok(sections);
})
.WithName("GetSections");

app.MapPost("/api/academic-structure/sections", async (CreateSectionDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var section = await academicStructureService.CreateSectionAsync(dto, cancellationToken);
    return Results.Created($"/api/academic-structure/sections/{section.Id}", section);
})
.WithName("CreateSection");

app.MapPut("/api/academic-structure/sections/{sectionId}", async (int sectionId, UpdateSectionDto dto, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    var section = await academicStructureService.UpdateSectionAsync(sectionId, dto, cancellationToken);
    return Results.Ok(section);
})
.WithName("UpdateSection");

app.MapDelete("/api/academic-structure/sections/{sectionId}", async (int sectionId, IAcademicStructureService academicStructureService, CancellationToken cancellationToken) =>
{
    await academicStructureService.DeleteSectionAsync(sectionId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteSection");

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

app.MapPost("/api/academics/subjects", async (CreateSubjectDto dto, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var subject = await academicsService.CreateSubjectAsync(dto, cancellationToken);
    return Results.Created($"/api/academics/subjects/{subject.Id}", subject);
})
.WithName("CreateSubject");

app.MapPut("/api/academics/subjects/{subjectId}", async (int subjectId, UpdateSubjectDto dto, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var subject = await academicsService.UpdateSubjectAsync(subjectId, dto, cancellationToken);
    return Results.Ok(subject);
})
.WithName("UpdateSubject");

app.MapDelete("/api/academics/subjects/{subjectId}", async (int subjectId, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    await academicsService.DeleteSubjectAsync(subjectId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteSubject");

app.MapGet("/api/academics/timetable", async (int? classId, int? sectionId, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var timetable = await academicsService.GetTimetableAsync(classId, sectionId, cancellationToken);
    return Results.Ok(timetable);
})
.WithName("GetTimetable");

app.MapPost("/api/academics/timetable", async (CreateTimetablePeriodDto dto, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var period = await academicsService.CreateTimetablePeriodAsync(dto, cancellationToken);
    return Results.Created($"/api/academics/timetable/{period.Id}", period);
})
.WithName("CreateTimetablePeriod");

app.MapPut("/api/academics/timetable/{timetablePeriodId}", async (int timetablePeriodId, UpdateTimetablePeriodDto dto, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    var period = await academicsService.UpdateTimetablePeriodAsync(timetablePeriodId, dto, cancellationToken);
    return Results.Ok(period);
})
.WithName("UpdateTimetablePeriod");

app.MapDelete("/api/academics/timetable/{timetablePeriodId}", async (int timetablePeriodId, IAcademicsService academicsService, CancellationToken cancellationToken) =>
{
    await academicsService.DeleteTimetablePeriodAsync(timetablePeriodId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteTimetablePeriod");

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

app.MapPost("/api/examinations/terms", async (CreateExamTermDto dto, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var term = await examinationsService.CreateExamTermAsync(dto, cancellationToken);
    return Results.Created($"/api/examinations/terms/{term.Id}", term);
})
.WithName("CreateExamTerm");

app.MapPut("/api/examinations/terms/{examTermId:int}", async (int examTermId, UpdateExamTermDto dto, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var term = await examinationsService.UpdateExamTermAsync(examTermId, dto, cancellationToken);
    return Results.Ok(term);
})
.WithName("UpdateExamTerm");

app.MapDelete("/api/examinations/terms/{examTermId:int}", async (int examTermId, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    await examinationsService.DeleteExamTermAsync(examTermId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteExamTerm");

app.MapGet("/api/examinations/schedule", async (int? examTermId, int? classId, int? sectionId, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var schedule = await examinationsService.GetExamScheduleAsync(examTermId, classId, sectionId, cancellationToken);
    return Results.Ok(schedule);
})
.WithName("GetExamSchedule");

app.MapPost("/api/examinations/schedule", async (CreateExamScheduleDto dto, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var schedule = await examinationsService.CreateExamScheduleAsync(dto, cancellationToken);
    return Results.Created($"/api/examinations/schedule/{schedule.Id}", schedule);
})
.WithName("CreateExamSchedule");

app.MapPut("/api/examinations/schedule/{examScheduleId:int}", async (int examScheduleId, UpdateExamScheduleDto dto, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    var schedule = await examinationsService.UpdateExamScheduleAsync(examScheduleId, dto, cancellationToken);
    return Results.Ok(schedule);
})
.WithName("UpdateExamSchedule");

app.MapDelete("/api/examinations/schedule/{examScheduleId:int}", async (int examScheduleId, IExaminationsService examinationsService, CancellationToken cancellationToken) =>
{
    await examinationsService.DeleteExamScheduleAsync(examScheduleId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteExamSchedule");

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

app.MapPost("/api/homework/assignments", async (CreateHomeworkAssignmentDto dto, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var assignment = await homeworkService.CreateAssignmentAsync(dto, cancellationToken);
    return Results.Created($"/api/homework/assignments/{assignment.Id}", assignment);
})
.WithName("CreateHomeworkAssignment");

app.MapPut("/api/homework/assignments/{homeworkAssignmentId:int}", async (int homeworkAssignmentId, UpdateHomeworkAssignmentDto dto, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var assignment = await homeworkService.UpdateAssignmentAsync(homeworkAssignmentId, dto, cancellationToken);
    return Results.Ok(assignment);
})
.WithName("UpdateHomeworkAssignment");

app.MapDelete("/api/homework/assignments/{homeworkAssignmentId:int}", async (int homeworkAssignmentId, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    await homeworkService.DeleteAssignmentAsync(homeworkAssignmentId, cancellationToken);
    return Results.NoContent();
})
.WithName("DeleteHomeworkAssignment");

app.MapGet("/api/homework/progress", async (int? classId, int? sectionId, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var progress = await homeworkService.GetProgressAsync(classId, sectionId, cancellationToken);
    return Results.Ok(progress);
})
.WithName("GetHomeworkProgress");

app.MapPut("/api/homework/progress", async (UpdateHomeworkSubmissionDto dto, IHomeworkService homeworkService, CancellationToken cancellationToken) =>
{
    var progress = await homeworkService.UpdateSubmissionAsync(dto, cancellationToken);
    return Results.Ok(progress);
})
.WithName("UpdateHomeworkSubmission");

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

app.MapGet("/api/transport/routes", async (ITransportService transportService, CancellationToken cancellationToken) =>
{
    var routes = await transportService.GetRoutesAsync(cancellationToken);
    return Results.Ok(routes);
})
.WithName("GetTransportRoutes");

app.MapPost("/api/transport/routes", async (CreateTransportRouteDto dto, ITransportService transportService, CancellationToken cancellationToken) =>
{
    try
    {
        var route = await transportService.CreateRouteAsync(dto, cancellationToken);
        return Results.Created($"/api/transport/routes/{route.Id}", route);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["route"] = [exception.Message]
        });
    }
})
.WithName("CreateTransportRoute");

app.MapPut("/api/transport/routes/{routeId:int}", async (int routeId, UpdateTransportRouteDto dto, ITransportService transportService, CancellationToken cancellationToken) =>
{
    try
    {
        var route = await transportService.UpdateRouteAsync(routeId, dto, cancellationToken);
        return Results.Ok(route);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["route"] = [exception.Message]
        });
    }
})
.WithName("UpdateTransportRoute");

app.MapDelete("/api/transport/routes/{routeId:int}", async (int routeId, ITransportService transportService, CancellationToken cancellationToken) =>
{
    try
    {
        await transportService.DeleteRouteAsync(routeId, cancellationToken);
        return Results.NoContent();
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["route"] = [exception.Message]
        });
    }
})
.WithName("DeleteTransportRoute");

app.MapGet("/api/transport/vehicles", async (ITransportService transportService, CancellationToken cancellationToken) =>
{
    var vehicles = await transportService.GetVehiclesAsync(cancellationToken);
    return Results.Ok(vehicles);
})
.WithName("GetTransportVehicles");

app.MapPost("/api/transport/vehicles", async (CreateTransportVehicleDto dto, ITransportService transportService, CancellationToken cancellationToken) =>
{
    try
    {
        var vehicle = await transportService.CreateVehicleAsync(dto, cancellationToken);
        return Results.Created($"/api/transport/vehicles/{vehicle.Id}", vehicle);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["vehicle"] = [exception.Message]
        });
    }
})
.WithName("CreateTransportVehicle");

app.MapPut("/api/transport/vehicles/{vehicleId:int}", async (int vehicleId, UpdateTransportVehicleDto dto, ITransportService transportService, CancellationToken cancellationToken) =>
{
    try
    {
        var vehicle = await transportService.UpdateVehicleAsync(vehicleId, dto, cancellationToken);
        return Results.Ok(vehicle);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["vehicle"] = [exception.Message]
        });
    }
})
.WithName("UpdateTransportVehicle");

app.MapDelete("/api/transport/vehicles/{vehicleId:int}", async (int vehicleId, ITransportService transportService, CancellationToken cancellationToken) =>
{
    try
    {
        await transportService.DeleteVehicleAsync(vehicleId, cancellationToken);
        return Results.NoContent();
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["vehicle"] = [exception.Message]
        });
    }
})
.WithName("DeleteTransportVehicle");

app.MapGet("/api/transport/dashboard", async (ITransportService transportService, CancellationToken cancellationToken) =>
{
    var dashboard = await transportService.GetDashboardAsync(cancellationToken);
    return Results.Ok(dashboard);
})
.WithName("GetTransportDashboard");

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

app.MapPost("/api/admissions/applications", async (CreateAdmissionApplicationDto dto, IAdmissionsService admissionsService, CancellationToken cancellationToken) =>
{
    var id = await admissionsService.CreateApplicationAsync(dto, cancellationToken);
    return Results.Created($"/api/admissions/applications/{id}", id);
})
.WithName("CreateAdmissionApplication");

app.MapPut("/api/admissions/applications/{applicationId}/status", async (int applicationId, UpdateAdmissionApplicationStatusDto request, IAdmissionsService admissionsService, CancellationToken cancellationToken) =>
{
    await admissionsService.UpdateApplicationStatusAsync(applicationId, request.Status, cancellationToken);
    return Results.NoContent();
})
.WithName("UpdateAdmissionApplicationStatus");

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

app.MapPost("/api/fees/payments", async (RecordFeePaymentDto dto, IFeeService feeService, CancellationToken cancellationToken) =>
{
    var id = await feeService.RecordPaymentAsync(dto, cancellationToken);
    return Results.Created($"/api/fees/payments/{id}", id);
})
.WithName("RecordFeePayment");

app.MapPut("/api/fees/payments/{paymentId}", async (int paymentId, UpdateFeePaymentDto dto, IFeeService feeService, CancellationToken cancellationToken) =>
{
    await feeService.UpdatePaymentAsync(paymentId, dto, cancellationToken);
    return Results.NoContent();
})
.WithName("UpdateFeePayment");

app.MapPost("/api/fees/concessions", async (CreateFeeConcessionDto dto, IFeeService feeService, CancellationToken cancellationToken) =>
{
    var id = await feeService.CreateConcessionAsync(dto, cancellationToken);
    return Results.Created($"/api/fees/concessions/{id}", id);
})
.WithName("CreateFeeConcession");

app.MapPut("/api/fees/concessions/{concessionId}/approve", async (int concessionId, ApproveFeeConcessionDto request, IFeeService feeService, CancellationToken cancellationToken) =>
{
    await feeService.ApproveConcessionAsync(concessionId, request.ApprovedBy, cancellationToken);
    return Results.NoContent();
})
.WithName("ApproveFeeConcession");

app.MapGet("/api/fees/receipts/{paymentId}", async (int paymentId, IFeeService feeService, CancellationToken cancellationToken) =>
{
    var receipt = await feeService.GenerateReceiptAsync(paymentId, cancellationToken);
    return Results.Ok(receipt);
})
.WithName("GenerateFeeReceipt");

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

app.MapPost("/api/attendance/leave-requests", async (
    CreateAttendanceLeaveRequestDto request,
    IAttendanceService attendanceService,
    CancellationToken cancellationToken) =>
{
    try
    {
        var leaveRequest = await attendanceService.CreateLeaveRequestAsync(request, cancellationToken);
        return Results.Created($"/api/attendance/leave-requests/{leaveRequest.Id}", leaveRequest);
    }
    catch (InvalidOperationException exception)
    {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["leaveRequest"] = [exception.Message]
        });
    }
})
.WithName("CreateAttendanceLeaveRequest");

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
