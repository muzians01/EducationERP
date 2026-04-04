using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class Student : BaseEntity
{
    private Student()
    {
    }

    public Student(
        int campusId,
        int academicYearId,
        int schoolClassId,
        int sectionId,
        int guardianId,
        int admissionApplicationId,
        string admissionNumber,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string gender,
        DateOnly enrolledOn,
        string status,
        string primaryContactNumber,
        string addressLine,
        string city,
        string state,
        string postalCode,
        string? bloodGroup = null,
        string? medicalNotes = null)
    {
        CampusId = campusId;
        AcademicYearId = academicYearId;
        SchoolClassId = schoolClassId;
        SectionId = sectionId;
        GuardianId = guardianId;
        AdmissionApplicationId = admissionApplicationId;
        AdmissionNumber = admissionNumber.Trim().ToUpperInvariant();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        DateOfBirth = dateOfBirth;
        Gender = gender.Trim();
        EnrolledOn = enrolledOn;
        Status = status.Trim();
        PrimaryContactNumber = primaryContactNumber.Trim();
        AddressLine = addressLine.Trim();
        City = city.Trim();
        State = state.Trim();
        PostalCode = postalCode.Trim();
        BloodGroup = bloodGroup?.Trim();
        MedicalNotes = medicalNotes?.Trim();
    }

    public int CampusId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int SchoolClassId { get; private set; }
    public int SectionId { get; private set; }
    public int GuardianId { get; private set; }
    public int AdmissionApplicationId { get; private set; }
    public string AdmissionNumber { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public DateOnly DateOfBirth { get; private set; }
    public string Gender { get; private set; } = string.Empty;
    public DateOnly EnrolledOn { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public string PrimaryContactNumber { get; private set; } = string.Empty;
    public string AddressLine { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string PostalCode { get; private set; } = string.Empty;
    public string? BloodGroup { get; private set; }
    public string? MedicalNotes { get; private set; }

    public Campus? Campus { get; private set; }
    public AcademicYear? AcademicYear { get; private set; }
    public SchoolClass? SchoolClass { get; private set; }
    public Section? Section { get; private set; }
    public Guardian? Guardian { get; private set; }
    public AdmissionApplication? AdmissionApplication { get; private set; }
    public ICollection<StudentDocument> Documents { get; private set; } = [];
    public ICollection<StudentFee> Fees { get; private set; } = [];
    public ICollection<FeePayment> FeePayments { get; private set; } = [];
    public ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = [];
    public ICollection<StudentLeaveRequest> LeaveRequests { get; private set; } = [];
}
