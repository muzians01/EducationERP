namespace EducationERP.Application.Examinations;

public sealed record StudentReportCardDto(
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string ClassName,
    string SectionName,
    int TotalMarks,
    decimal MarksObtained,
    decimal Percentage,
    string ResultStatus,
    IReadOnlyList<ExamSubjectResultDto> SubjectResults);
