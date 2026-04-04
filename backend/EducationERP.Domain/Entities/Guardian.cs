using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class Guardian : BaseEntity
{
    private Guardian()
    {
    }

    public Guardian(int campusId, string fullName, string relationship, string phoneNumber, string email, string occupation)
    {
        CampusId = campusId;
        FullName = fullName.Trim();
        Relationship = relationship.Trim();
        PhoneNumber = phoneNumber.Trim();
        Email = email.Trim().ToLowerInvariant();
        Occupation = occupation.Trim();
    }

    public int CampusId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string Relationship { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string Occupation { get; private set; } = string.Empty;

    public Campus? Campus { get; private set; }
    public ICollection<AdmissionApplication> AdmissionApplications { get; private set; } = [];
    public ICollection<Student> Students { get; private set; } = [];
}
