namespace EducationERP.Application.Fees;

public sealed record RecordFeePaymentDto(
    int StudentFeeId,
    decimal Amount,
    string PaymentMethod,
    string PaymentReference,
    DateOnly PaidOn);

public sealed record UpdateFeePaymentDto(
    decimal Amount,
    string PaymentMethod,
    string PaymentReference,
    DateOnly PaidOn);

public sealed record CreateFeeConcessionDto(
    int StudentFeeId,
    string ConcessionType,
    decimal Amount,
    string Remarks);

public sealed record ApproveFeeConcessionDto(string ApprovedBy);
