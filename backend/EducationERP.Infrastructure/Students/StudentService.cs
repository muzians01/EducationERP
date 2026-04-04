using EducationERP.Application.Students;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Students;

internal sealed class StudentService(EducationErpDbContext dbContext) : IStudentService
{
    public async Task<IReadOnlyList<StudentDto>> GetStudentsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Students
            .AsNoTracking()
            .Include(student => student.Campus)
            .Include(student => student.AcademicYear)
            .Include(student => student.SchoolClass)
            .Include(student => student.Section)
            .Include(student => student.Guardian)
            .OrderBy(student => student.AdmissionNumber)
            .Select(student => new StudentDto(
                student.Id,
                student.AdmissionNumber,
                $"{student.FirstName} {student.LastName}",
                student.Campus!.Name,
                student.AcademicYear!.Name,
                student.SchoolClass!.Name,
                student.Section!.Name,
                student.Guardian!.FullName,
                student.EnrolledOn,
                student.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentProfileOverviewDto>> GetStudentProfileOverviewAsync(CancellationToken cancellationToken = default)
    {
        var students = await dbContext.Students
            .AsNoTracking()
            .Include(student => student.SchoolClass)
            .Include(student => student.Section)
            .Include(student => student.Guardian)
            .Include(student => student.Documents)
            .OrderBy(student => student.AdmissionNumber)
            .ToListAsync(cancellationToken);

        return students
            .Select(student => new StudentProfileOverviewDto(
                student.Id,
                student.AdmissionNumber,
                $"{student.FirstName} {student.LastName}",
                student.SchoolClass!.Name,
                student.Section!.Name,
                student.Guardian!.FullName,
                student.PrimaryContactNumber,
                $"{student.AddressLine}, {student.City}, {student.State} {student.PostalCode}",
                student.Gender,
                student.BloodGroup,
                student.MedicalNotes,
                CalculateProfileCompletion(student),
                student.Documents.Count(document => document.Status != "Verified")))
            .ToList();
    }

    public async Task<IReadOnlyList<StudentDocumentDto>> GetStudentDocumentsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.StudentDocuments
            .AsNoTracking()
            .Include(document => document.Student)
            .OrderBy(document => document.Student!.AdmissionNumber)
            .ThenBy(document => document.DocumentType)
            .Select(document => new StudentDocumentDto(
                document.Id,
                document.StudentId,
                $"{document.Student!.FirstName} {document.Student.LastName}",
                document.Student.AdmissionNumber,
                document.DocumentType,
                document.Status,
                document.SubmittedOn,
                document.VerifiedOn,
                document.Remarks))
            .ToListAsync(cancellationToken);
    }

    private static int CalculateProfileCompletion(EducationERP.Domain.Entities.Student student)
    {
        var completedFields = 0;
        const int totalFields = 8;

        if (!string.IsNullOrWhiteSpace(student.PrimaryContactNumber)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.AddressLine)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.City)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.State)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.PostalCode)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.Gender)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.BloodGroup)) completedFields++;
        if (!string.IsNullOrWhiteSpace(student.MedicalNotes)) completedFields++;

        return completedFields * 100 / totalFields;
    }
}
