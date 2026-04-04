using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class SchoolClass : BaseEntity
{
    private SchoolClass()
    {
    }

    public SchoolClass(int campusId, string code, string name, int displayOrder)
    {
        CampusId = campusId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        DisplayOrder = displayOrder;
    }

    public int CampusId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    public Campus? Campus { get; private set; }
    public ICollection<Section> Sections { get; private set; } = [];
    public ICollection<AdmissionApplication> AdmissionApplications { get; private set; } = [];
    public ICollection<Student> Students { get; private set; } = [];
    public ICollection<FeeStructure> FeeStructures { get; private set; } = [];
}
