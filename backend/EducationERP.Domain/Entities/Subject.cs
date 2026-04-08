using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class Subject : BaseEntity
{
    private Subject()
    {
    }

    public Subject(int campusId, string code, string name, string category, int weeklyPeriods)
    {
        CampusId = campusId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Category = category.Trim();
        WeeklyPeriods = weeklyPeriods;
    }

    public int CampusId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;
    public int WeeklyPeriods { get; private set; }

    public Campus? Campus { get; private set; }
    public ICollection<TimetablePeriod> TimetablePeriods { get; private set; } = [];
    public ICollection<ExamSchedule> ExamSchedules { get; private set; } = [];

    public void UpdateDetails(string code, string name, string category, int weeklyPeriods)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        Category = category.Trim();
        WeeklyPeriods = weeklyPeriods;
    }
}
