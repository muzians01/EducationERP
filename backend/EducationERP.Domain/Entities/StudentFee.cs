using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class StudentFee : BaseEntity
{
    private StudentFee()
    {
    }

    public StudentFee(
        int studentId,
        int feeStructureId,
        DateOnly dueOn,
        decimal amount,
        decimal amountPaid,
        string status)
    {
        StudentId = studentId;
        FeeStructureId = feeStructureId;
        DueOn = dueOn;
        Amount = amount;
        AmountPaid = amountPaid;
        Status = status.Trim();
    }

    public int StudentId { get; private set; }
    public int FeeStructureId { get; private set; }
    public DateOnly DueOn { get; private set; }
    public decimal Amount { get; private set; }
    public decimal AmountPaid { get; private set; }
    public string Status { get; private set; } = string.Empty;

    public void UpdateAmountPaid(decimal amountPaid)
    {
        AmountPaid = amountPaid;
    }

    public void UpdateStatus(string status)
    {
        Status = status.Trim();
    }

    public Student? Student { get; private set; }
    public FeeStructure? FeeStructure { get; private set; }
    public ICollection<FeePayment> Payments { get; private set; } = [];
    public ICollection<FeeConcession> Concessions { get; private set; } = [];
}
