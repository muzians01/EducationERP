namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalExamResultDto(
    string ExamTermName,
    string SubjectName,
    decimal MarksObtained,
    int MaxMarks,
    string Grade,
    string ResultStatus);
