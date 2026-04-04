namespace EducationERP.Application.Students;

public interface IStudentService
{
    Task<IReadOnlyList<StudentDto>> GetStudentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentProfileOverviewDto>> GetStudentProfileOverviewAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentDocumentDto>> GetStudentDocumentsAsync(CancellationToken cancellationToken = default);
}
