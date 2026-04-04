namespace EducationERP.Application.Fees;

public sealed record FeePaymentDto(
    int Id,
    string StudentName,
    string AdmissionNumber,
    string FeeName,
    string PaymentReference,
    DateOnly PaidOn,
    decimal Amount,
    string PaymentMethod,
    string Status);
