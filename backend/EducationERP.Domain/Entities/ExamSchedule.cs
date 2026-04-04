using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class ExamSchedule : BaseEntity
{
    private ExamSchedule()
    {
    }

    public ExamSchedule(
        int examTermId,
        int schoolClassId,
        int sectionId,
        int subjectId,
        DateOnly examDate,
        TimeOnly startTime,
        int durationMinutes,
        int maxMarks,
        int passMarks)
    {
        ExamTermId = examTermId;
        SchoolClassId = schoolClassId;
        SectionId = sectionId;
        SubjectId = subjectId;
        ExamDate = examDate;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        MaxMarks = maxMarks;
        PassMarks = passMarks;
    }

    public int ExamTermId { get; private set; }
    public int SchoolClassId { get; private set; }
    public int SectionId { get; private set; }
    public int SubjectId { get; private set; }
    public DateOnly ExamDate { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public int DurationMinutes { get; private set; }
    public int MaxMarks { get; private set; }
    public int PassMarks { get; private set; }

    public ExamTerm? ExamTerm { get; private set; }
    public SchoolClass? SchoolClass { get; private set; }
    public Section? Section { get; private set; }
    public Subject? Subject { get; private set; }
    public ICollection<StudentExamScore> StudentExamScores { get; private set; } = [];
}
