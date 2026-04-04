namespace EducationERP.Application.Fees;

public sealed record StudentFeeDto(
    int Id,
    int StudentId,
    string StudentName,
    string AdmissionNumber,
    string FeeName,
    string BillingCycle,
    DateOnly DueOn,
    decimal Amount,
    decimal ConcessionAmount,
    decimal AmountPaid,
    decimal BalanceAmount,
    string Status);
