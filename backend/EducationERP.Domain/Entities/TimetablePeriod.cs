using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class TimetablePeriod : BaseEntity
{
    private TimetablePeriod()
    {
    }

    public TimetablePeriod(
        int academicYearId,
        int schoolClassId,
        int sectionId,
        int subjectId,
        DayOfWeek dayOfWeek,
        int periodNumber,
        TimeOnly startTime,
        TimeOnly endTime,
        string teacherName,
        string roomNumber)
    {
        AcademicYearId = academicYearId;
        SchoolClassId = schoolClassId;
        SectionId = sectionId;
        SubjectId = subjectId;
        DayOfWeek = dayOfWeek;
        PeriodNumber = periodNumber;
        StartTime = startTime;
        EndTime = endTime;
        TeacherName = teacherName.Trim();
        RoomNumber = roomNumber.Trim().ToUpperInvariant();
    }

    public int AcademicYearId { get; private set; }
    public int SchoolClassId { get; private set; }
    public int SectionId { get; private set; }
    public int SubjectId { get; private set; }
    public DayOfWeek DayOfWeek { get; private set; }
    public int PeriodNumber { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public string TeacherName { get; private set; } = string.Empty;
    public string RoomNumber { get; private set; } = string.Empty;

    public AcademicYear? AcademicYear { get; private set; }
    public SchoolClass? SchoolClass { get; private set; }
    public Section? Section { get; private set; }
    public Subject? Subject { get; private set; }

    public void UpdatePeriod(
        int academicYearId,
        int schoolClassId,
        int sectionId,
        int subjectId,
        DayOfWeek dayOfWeek,
        int periodNumber,
        TimeOnly startTime,
        TimeOnly endTime,
        string teacherName,
        string roomNumber)
    {
        AcademicYearId = academicYearId;
        SchoolClassId = schoolClassId;
        SectionId = sectionId;
        SubjectId = subjectId;
        DayOfWeek = dayOfWeek;
        PeriodNumber = periodNumber;
        StartTime = startTime;
        EndTime = endTime;
        TeacherName = teacherName.Trim();
        RoomNumber = roomNumber.Trim().ToUpperInvariant();
    }
}
