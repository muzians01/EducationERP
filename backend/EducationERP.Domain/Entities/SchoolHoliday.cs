using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class SchoolHoliday : BaseEntity
{
    private SchoolHoliday()
    {
    }

    public SchoolHoliday(
        int campusId,
        DateOnly holidayDate,
        string title,
        string category)
    {
        CampusId = campusId;
        HolidayDate = holidayDate;
        Title = title.Trim();
        Category = category.Trim();
    }

    public int CampusId { get; private set; }
    public DateOnly HolidayDate { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Category { get; private set; } = string.Empty;

    public Campus? Campus { get; private set; }
}
