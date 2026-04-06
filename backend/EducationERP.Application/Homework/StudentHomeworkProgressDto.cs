namespace EducationERP.Application.Homework;

public sealed record StudentHomeworkProgressDto(
    int HomeworkAssignmentId,
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string HomeworkTitle,
    DateOnly DueOn,
    string Status,
    DateOnly? SubmittedOn,
    string? Remarks);
