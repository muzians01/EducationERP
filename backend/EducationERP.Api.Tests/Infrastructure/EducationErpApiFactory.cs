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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EducationERP.Api.Tests.Infrastructure;

public sealed class EducationErpApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ICampusService>();
            services.RemoveAll<IAcademicStructureService>();
            services.RemoveAll<IAcademicsService>();
            services.RemoveAll<IAdmissionsService>();
            services.RemoveAll<IStudentService>();
            services.RemoveAll<IFeeService>();
            services.RemoveAll<IExaminationsService>();
            services.RemoveAll<IHomeworkService>();
            services.RemoveAll<IAttendanceService>();
            services.RemoveAll<IParentPortalService>();
            services.AddScoped<ICampusService, FakeCampusService>();
            services.AddScoped<IAcademicStructureService, FakeAcademicStructureService>();
            services.AddScoped<IAcademicsService, FakeAcademicsService>();
            services.AddScoped<IAdmissionsService, FakeAdmissionsService>();
            services.AddScoped<IStudentService, FakeStudentService>();
            services.AddScoped<IFeeService, FakeFeeService>();
            services.AddScoped<IExaminationsService, FakeExaminationsService>();
            services.AddScoped<IHomeworkService, FakeHomeworkService>();
            services.AddScoped<IAttendanceService, FakeAttendanceService>();
            services.AddScoped<IParentPortalService, FakeParentPortalService>();
        });
    }
}
