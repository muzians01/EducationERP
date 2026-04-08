namespace EducationERP.Application.Homework;

public sealed record CreateHomeworkAssignmentDto(
    int AcademicYearId,
    int ClassId,
    int SectionId,
    int SubjectId,
    DateOnly AssignedOn,
    DateOnly DueOn,
    string Title,
    string Instructions,
    string AssignedBy);

public sealed record UpdateHomeworkAssignmentDto(
    int AcademicYearId,
    int ClassId,
    int SectionId,
    int SubjectId,
    DateOnly AssignedOn,
    DateOnly DueOn,
    string Title,
    string Instructions,
    string AssignedBy);

public sealed record UpdateHomeworkSubmissionDto(
    int HomeworkAssignmentId,
    int StudentId,
    string Status,
    DateOnly? SubmittedOn,
    string? Remarks);
