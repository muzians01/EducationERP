using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class HomeworkAssignment : BaseEntity
{
    private HomeworkAssignment()
    {
    }

    public HomeworkAssignment(
        int academicYearId,
        int schoolClassId,
        int sectionId,
        int subjectId,
        DateOnly assignedOn,
        DateOnly dueOn,
        string title,
        string instructions,
        string assignedBy)
    {
        AcademicYearId = academicYearId;
        SchoolClassId = schoolClassId;
        SectionId = sectionId;
        SubjectId = subjectId;
        AssignedOn = assignedOn;
        DueOn = dueOn;
        Title = title.Trim();
        Instructions = instructions.Trim();
        AssignedBy = assignedBy.Trim();
    }

    public int AcademicYearId { get; private set; }
    public int SchoolClassId { get; private set; }
    public int SectionId { get; private set; }
    public int SubjectId { get; private set; }
    public DateOnly AssignedOn { get; private set; }
    public DateOnly DueOn { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Instructions { get; private set; } = string.Empty;
    public string AssignedBy { get; private set; } = string.Empty;

    public AcademicYear? AcademicYear { get; private set; }
    public SchoolClass? SchoolClass { get; private set; }
    public Section? Section { get; private set; }
    public Subject? Subject { get; private set; }
    public ICollection<StudentHomeworkSubmission> StudentSubmissions { get; private set; } = [];
}
