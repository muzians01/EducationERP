namespace EducationERP.Application.Students;

public sealed record StudentDto(
    int Id,
    string AdmissionNumber,
    string StudentName,
    string CampusName,
    string AcademicYearName,
    string ClassName,
    string SectionName,
    string GuardianName,
    DateOnly EnrolledOn,
    string Status);
