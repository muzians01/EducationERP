namespace EducationERP.Application.Fees;

public sealed record FeesDashboardDto(
    decimal TotalExpectedAmount,
    decimal TotalConcessionAmount,
    decimal NetReceivableAmount,
    decimal TotalCollectedAmount,
    decimal OutstandingAmount,
    int OverdueCount,
    IReadOnlyList<StudentFeeDto> OutstandingFees,
    IReadOnlyList<FeePaymentDto> RecentPayments,
    IReadOnlyList<FeeReceiptDto> RecentReceipts);
