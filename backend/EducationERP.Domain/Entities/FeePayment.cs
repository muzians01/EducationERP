using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class FeePayment : BaseEntity
{
    private FeePayment()
    {
    }

    public FeePayment(
        int studentId,
        int studentFeeId,
        string paymentReference,
        DateOnly paidOn,
        decimal amount,
        string paymentMethod,
        string status)
    {
        StudentId = studentId;
        StudentFeeId = studentFeeId;
        PaymentReference = paymentReference.Trim().ToUpperInvariant();
        PaidOn = paidOn;
        Amount = amount;
        PaymentMethod = paymentMethod.Trim();
        Status = status.Trim();
    }

    public int StudentId { get; private set; }
    public int StudentFeeId { get; private set; }
    public string PaymentReference { get; private set; } = string.Empty;
    public DateOnly PaidOn { get; private set; }
    public decimal Amount { get; private set; }
    public string PaymentMethod { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public void UpdatePayment(decimal amount, string paymentMethod, string paymentReference, DateOnly paidOn)
    {
        Amount = amount;
        PaymentMethod = paymentMethod.Trim();
        PaymentReference = paymentReference.Trim().ToUpperInvariant();
        PaidOn = paidOn;
    }

    public Student? Student { get; private set; }
    public StudentFee? StudentFee { get; private set; }
}
