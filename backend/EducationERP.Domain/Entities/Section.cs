using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class Section : BaseEntity
{
    private Section()
    {
    }

    public Section(int schoolClassId, string name, int capacity, string roomNumber)
    {
        SchoolClassId = schoolClassId;
        Name = name.Trim().ToUpperInvariant();
        Capacity = capacity;
        RoomNumber = roomNumber.Trim().ToUpperInvariant();
    }

    public int SchoolClassId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public string RoomNumber { get; private set; } = string.Empty;

    public SchoolClass? SchoolClass { get; private set; }
    public ICollection<AdmissionApplication> AdmissionApplications { get; private set; } = [];
    public ICollection<Student> Students { get; private set; } = [];
}
