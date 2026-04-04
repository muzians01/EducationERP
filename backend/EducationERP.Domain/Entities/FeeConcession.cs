using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class FeeConcession : BaseEntity
{
    private FeeConcession()
    {
    }

    public FeeConcession(
        int studentFeeId,
        string concessionType,
        decimal amount,
        DateOnly approvedOn,
        string approvedBy,
        string? remarks = null)
    {
        StudentFeeId = studentFeeId;
        ConcessionType = concessionType.Trim();
        Amount = amount;
        ApprovedOn = approvedOn;
        ApprovedBy = approvedBy.Trim();
        Remarks = remarks?.Trim();
    }

    public int StudentFeeId { get; private set; }
    public string ConcessionType { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public DateOnly ApprovedOn { get; private set; }
    public string ApprovedBy { get; private set; } = string.Empty;
    public string? Remarks { get; private set; }

    public StudentFee? StudentFee { get; private set; }
}
