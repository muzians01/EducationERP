namespace EducationERP.Application.Examinations;

public sealed record ExamScheduleDto(
    int Id,
    int ExamTermId,
    string ExamTermName,
    int ClassId,
    string ClassName,
    int SectionId,
    string SectionName,
    int SubjectId,
    string SubjectName,
    DateOnly ExamDate,
    TimeOnly StartTime,
    int DurationMinutes,
    int MaxMarks,
    int PassMarks);
