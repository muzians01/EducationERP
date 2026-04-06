using EducationERP.Application.AcademicStructure;
using EducationERP.Application.Academics;
using EducationERP.Application.Admissions;
using EducationERP.Application.Attendance;
using EducationERP.Application.Campuses;
using EducationERP.Application.Fees;
using EducationERP.Application.Examinations;
using EducationERP.Application.Homework;
using EducationERP.Application.Students;
using EducationERP.Application.ParentPortal;
using EducationERP.Infrastructure.AcademicStructure;
using EducationERP.Infrastructure.Academics;
using EducationERP.Infrastructure.Admissions;
using EducationERP.Infrastructure.Attendance;
using EducationERP.Infrastructure.Campuses;
using EducationERP.Infrastructure.Fees;
using EducationERP.Infrastructure.Examinations;
using EducationERP.Infrastructure.Homework;
using EducationERP.Infrastructure.ParentPortal;
using EducationERP.Infrastructure.Persistence;
using EducationERP.Infrastructure.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EducationERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EducationErp")
            ?? throw new InvalidOperationException("Connection string 'EducationErp' was not found.");

        services.AddDbContext<EducationErpDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<ICampusService, CampusService>();
        services.AddScoped<IAcademicStructureService, AcademicStructureService>();
        services.AddScoped<IAcademicsService, AcademicsService>();
        services.AddScoped<IAdmissionsService, AdmissionsService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IFeeService, FeeService>();
        services.AddScoped<IExaminationsService, ExaminationsService>();
        services.AddScoped<IHomeworkService, HomeworkService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IParentPortalService, ParentPortalService>();

        return services;
    }
}
