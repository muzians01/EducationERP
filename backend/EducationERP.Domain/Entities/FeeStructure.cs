using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class FeeStructure : BaseEntity
{
    private FeeStructure()
    {
    }

    public FeeStructure(
        int campusId,
        int academicYearId,
        int schoolClassId,
        string feeCode,
        string feeName,
        decimal amount,
        string billingCycle)
    {
        CampusId = campusId;
        AcademicYearId = academicYearId;
        SchoolClassId = schoolClassId;
        FeeCode = feeCode.Trim().ToUpperInvariant();
        FeeName = feeName.Trim();
        Amount = amount;
        BillingCycle = billingCycle.Trim();
    }

    public int CampusId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int SchoolClassId { get; private set; }
    public string FeeCode { get; private set; } = string.Empty;
    public string FeeName { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string BillingCycle { get; private set; } = string.Empty;

    public Campus? Campus { get; private set; }
    public AcademicYear? AcademicYear { get; private set; }
    public SchoolClass? SchoolClass { get; private set; }
    public ICollection<StudentFee> StudentFees { get; private set; } = [];
}
