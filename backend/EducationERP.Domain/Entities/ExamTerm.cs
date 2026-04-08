using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class ExamTerm : BaseEntity
{
    private ExamTerm()
    {
    }

    public ExamTerm(int campusId, int academicYearId, string name, string examType, DateOnly startDate, DateOnly endDate, string status)
    {
        CampusId = campusId;
        AcademicYearId = academicYearId;
        Name = name.Trim();
        ExamType = examType.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = status.Trim();
    }

    public int CampusId { get; private set; }
    public int AcademicYearId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ExamType { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public Campus? Campus { get; private set; }
    public AcademicYear? AcademicYear { get; private set; }
    public ICollection<ExamSchedule> ExamSchedules { get; private set; } = [];

    public void UpdateDetails(int campusId, int academicYearId, string name, string examType, DateOnly startDate, DateOnly endDate, string status)
    {
        CampusId = campusId;
        AcademicYearId = academicYearId;
        Name = name.Trim();
        ExamType = examType.Trim();
        StartDate = startDate;
        EndDate = endDate;
        Status = status.Trim();
    }
}
