namespace EducationERP.Application.Examinations;

public sealed record ExamTermDto(
    int Id,
    string Name,
    string ExamType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status);
