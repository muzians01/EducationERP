namespace EducationERP.Application.Fees;

public interface IFeeService
{
    Task<IReadOnlyList<FeeStructureDto>> GetFeeStructuresAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentFeeDto>> GetStudentFeesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeePaymentDto>> GetRecentPaymentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeConcessionDto>> GetConcessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeReceiptDto>> GetReceiptsAsync(CancellationToken cancellationToken = default);
    Task<FeesDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
