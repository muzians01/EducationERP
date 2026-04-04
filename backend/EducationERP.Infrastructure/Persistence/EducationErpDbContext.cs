using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Persistence;

public sealed class EducationErpDbContext(DbContextOptions<EducationErpDbContext> options) : DbContext(options)
{
    public DbSet<Campus> Campuses => Set<Campus>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TimetablePeriod> TimetablePeriods => Set<TimetablePeriod>();
    public DbSet<ExamTerm> ExamTerms => Set<ExamTerm>();
    public DbSet<ExamSchedule> ExamSchedules => Set<ExamSchedule>();
    public DbSet<StudentExamScore> StudentExamScores => Set<StudentExamScore>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<AdmissionApplication> AdmissionApplications => Set<AdmissionApplication>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentDocument> StudentDocuments => Set<StudentDocument>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<StudentFee> StudentFees => Set<StudentFee>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<FeeConcession> FeeConcessions => Set<FeeConcession>();
    public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
    public DbSet<SchoolHoliday> SchoolHolidays => Set<SchoolHoliday>();
    public DbSet<StudentLeaveRequest> StudentLeaveRequests => Set<StudentLeaveRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EducationErpDbContext).Assembly);
    }
}
