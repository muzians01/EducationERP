namespace EducationERP.Application.Fees;

public interface IFeeService
{
    Task<IReadOnlyList<FeeStructureDto>> GetFeeStructuresAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StudentFeeDto>> GetStudentFeesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeePaymentDto>> GetRecentPaymentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeConcessionDto>> GetConcessionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeeReceiptDto>> GetReceiptsAsync(CancellationToken cancellationToken = default);
    Task<FeesDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<int> RecordPaymentAsync(RecordFeePaymentDto dto, CancellationToken cancellationToken = default);
    Task UpdatePaymentAsync(int paymentId, UpdateFeePaymentDto dto, CancellationToken cancellationToken = default);
    Task<int> CreateConcessionAsync(CreateFeeConcessionDto dto, CancellationToken cancellationToken = default);
    Task ApproveConcessionAsync(int concessionId, string approvedBy, CancellationToken cancellationToken = default);
    Task<FeeReceiptDto> GenerateReceiptAsync(int paymentId, CancellationToken cancellationToken = default);
}
