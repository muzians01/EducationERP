using EducationERP.Application.Fees;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeFeeService : IFeeService
{
    public Task<IReadOnlyList<FeeStructureDto>> GetFeeStructuresAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FeeStructureDto> feeStructures =
        [
            new FeeStructureDto(1, "TUITION", "Tuition Fee", "Test Campus", "2026-2027", "Grade 1", "Quarterly", 48000m),
            new FeeStructureDto(2, "TRANSPORT", "Transport Fee", "Test Campus", "2026-2027", "Grade 1", "Quarterly", 12000m)
        ];

        return Task.FromResult(feeStructures);
    }

    public Task<IReadOnlyList<StudentFeeDto>> GetStudentFeesAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<StudentFeeDto> studentFees =
        [
            new StudentFeeDto(1, 1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "Quarterly", new DateOnly(2026, 6, 10), 48000m, 6000m, 24000m, 18000m, "Partially Paid"),
            new StudentFeeDto(2, 1, "Ishita Verma", "STU-2026-001", "Transport Fee", "Quarterly", new DateOnly(2026, 6, 10), 12000m, 0m, 0m, 12000m, "Pending")
        ];

        return Task.FromResult(studentFees);
    }

    public Task<IReadOnlyList<FeePaymentDto>> GetRecentPaymentsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FeePaymentDto> payments =
        [
            new FeePaymentDto(1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "RCPT-2026-001", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
        ];

        return Task.FromResult(payments);
    }

    public Task<IReadOnlyList<FeeConcessionDto>> GetConcessionsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FeeConcessionDto> concessions =
        [
            new FeeConcessionDto(1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "Sibling Concession", 6000m, new DateOnly(2026, 4, 4), "Bursar", "Approved for sibling admission")
        ];

        return Task.FromResult(concessions);
    }

    public Task<IReadOnlyList<FeeReceiptDto>> GetReceiptsAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FeeReceiptDto> receipts =
        [
            new FeeReceiptDto(1, "RCPT-2026-001", "Ishita Verma", "STU-2026-001", "Tuition Fee", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
        ];

        return Task.FromResult(receipts);
    }

    public Task<FeesDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new FeesDashboardDto(
            60000m,
            6000m,
            54000m,
            24000m,
            30000m,
            0,
            [
                new StudentFeeDto(1, 1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "Quarterly", new DateOnly(2026, 6, 10), 48000m, 6000m, 24000m, 18000m, "Partially Paid"),
                new StudentFeeDto(2, 1, "Ishita Verma", "STU-2026-001", "Transport Fee", "Quarterly", new DateOnly(2026, 6, 10), 12000m, 0m, 0m, 12000m, "Pending")
            ],
            [
                new FeePaymentDto(1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "RCPT-2026-001", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
            ],
            [
                new FeeReceiptDto(1, "RCPT-2026-001", "Ishita Verma", "STU-2026-001", "Tuition Fee", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
            ]));
    }
}
