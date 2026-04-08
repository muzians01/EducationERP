using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class AcademicYear : BaseEntity
{
    private AcademicYear()
    {
    }

    public AcademicYear(int campusId, string name, DateOnly startDate, DateOnly endDate, bool isActive)
    {
        CampusId = campusId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }

    public void UpdateDetails(int campusId, string name, DateOnly startDate, DateOnly endDate, bool isActive)
    {
        CampusId = campusId;
        Name = name.Trim();
        StartDate = startDate;
        EndDate = endDate;
        IsActive = isActive;
    }

    public int CampusId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public bool IsActive { get; private set; }

    public Campus? Campus { get; private set; }
    public ICollection<AdmissionApplication> AdmissionApplications { get; private set; } = [];
    public ICollection<Student> Students { get; private set; } = [];
    public ICollection<FeeStructure> FeeStructures { get; private set; } = [];
}
