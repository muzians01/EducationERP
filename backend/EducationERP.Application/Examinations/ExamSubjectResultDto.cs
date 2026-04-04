namespace EducationERP.Application.Examinations;

public sealed record ExamSubjectResultDto(
    int SubjectId,
    string SubjectName,
    int MaxMarks,
    int PassMarks,
    decimal MarksObtained,
    string Grade,
    string ResultStatus);
