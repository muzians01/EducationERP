using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class AdmissionApplication : BaseEntity
{
    private AdmissionApplication()
    {
    }

    public AdmissionApplication(
        int campusId,
        int academicYearId,
        int schoolClassId,
        int sectionId,
        int guardianId,
        string applicationNumber,
        string studentFirstName,
        string studentLastName,
        DateOnly dateOfBirth,
        string gender,
        string status,
        DateOnly appliedOn,
        decimal registrationFee)
    {
        CampusId = campusId;
        AcademicYearId = academicYearId;
        SchoolClassId = schoolClassId;
        SectionId = sectionId;
        GuardianId = guardianId;
        ApplicationNumber = applicationNumber.Trim().ToUpperInvariant();
        StudentFirstName = studentFirstName.Trim();
        StudentLastName = studentLastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender.Trim();
        Status = status.Trim();
        AppliedOn = appliedOn;
        RegistrationFee = registrationFee;
    }

    public int CampusId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int SchoolClassId { get; private set; }
    public int SectionId { get; private set; }
    public int GuardianId { get; private set; }
    public string ApplicationNumber { get; private set; } = string.Empty;
    public string StudentFirstName { get; private set; } = string.Empty;
    public string StudentLastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateOnly AppliedOn { get; private set; }
    public decimal RegistrationFee { get; private set; }

    public Campus? Campus { get; private set; }
    public AcademicYear? AcademicYear { get; private set; }
    public SchoolClass? SchoolClass { get; private set; }
    public Section? Section { get; private set; }
    public Guardian? Guardian { get; private set; }
    public ICollection<Student> Students { get; private set; } = [];
}
