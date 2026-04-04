namespace EducationERP.Application.Fees;

public sealed record FeeReceiptDto(
    int Id,
    string ReceiptNumber,
    string StudentName,
    string AdmissionNumber,
    string FeeName,
    DateOnly PaidOn,
    decimal Amount,
    string PaymentMethod,
    string Status);
