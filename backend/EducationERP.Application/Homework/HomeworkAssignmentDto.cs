namespace EducationERP.Application.Homework;

public sealed record HomeworkAssignmentDto(
    int Id,
    int ClassId,
    string ClassName,
    int SectionId,
    string SectionName,
    int SubjectId,
    string SubjectName,
    DateOnly AssignedOn,
    DateOnly DueOn,
    string Title,
    string Instructions,
    string AssignedBy);
