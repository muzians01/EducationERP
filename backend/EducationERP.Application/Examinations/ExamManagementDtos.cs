namespace EducationERP.Application.Examinations;

public sealed record CreateExamTermDto(
    int CampusId,
    int AcademicYearId,
    string Name,
    string ExamType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status);

public sealed record UpdateExamTermDto(
    int CampusId,
    int AcademicYearId,
    string Name,
    string ExamType,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status);

public sealed record CreateExamScheduleDto(
    int ExamTermId,
    int ClassId,
    int SectionId,
    int SubjectId,
    DateOnly ExamDate,
    TimeOnly StartTime,
    int DurationMinutes,
    int MaxMarks,
    int PassMarks);

public sealed record UpdateExamScheduleDto(
    int ExamTermId,
    int ClassId,
    int SectionId,
    int SubjectId,
    DateOnly ExamDate,
    TimeOnly StartTime,
    int DurationMinutes,
    int MaxMarks,
    int PassMarks);
