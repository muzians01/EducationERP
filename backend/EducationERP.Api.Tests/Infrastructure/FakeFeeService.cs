using EducationERP.Application.Fees;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeFeeService : IFeeService
{
    private readonly List<StudentFeeDto> _studentFees =
    [
        new StudentFeeDto(1, 1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "Quarterly", new DateOnly(2026, 6, 10), 48000m, 6000m, 24000m, 18000m, "Partially Paid"),
        new StudentFeeDto(2, 1, "Ishita Verma", "STU-2026-001", "Transport Fee", "Quarterly", new DateOnly(2026, 6, 10), 12000m, 0m, 0m, 12000m, "Pending")
    ];

    private readonly List<FeePaymentDto> _payments =
    [
        new FeePaymentDto(1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "RCPT-2026-001", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
    ];

    private readonly List<FeeConcessionDto> _concessions =
    [
        new FeeConcessionDto(1, "Ishita Verma", "STU-2026-001", "Tuition Fee", "Sibling Concession", 6000m, new DateOnly(2026, 4, 4), "Bursar", "Approved for sibling admission")
    ];

    private readonly List<FeeReceiptDto> _receipts =
    [
        new FeeReceiptDto(1, "RCPT-2026-001", "Ishita Verma", "STU-2026-001", "Tuition Fee", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
    ];

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
        => Task.FromResult<IReadOnlyList<StudentFeeDto>>(_studentFees.ToList());

    public Task<IReadOnlyList<FeePaymentDto>> GetRecentPaymentsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<FeePaymentDto>>(_payments.OrderByDescending(payment => payment.PaidOn).ToList());

    public Task<IReadOnlyList<FeeConcessionDto>> GetConcessionsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<FeeConcessionDto>>(_concessions.ToList());

    public Task<IReadOnlyList<FeeReceiptDto>> GetReceiptsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<FeeReceiptDto>>(_receipts.OrderByDescending(receipt => receipt.PaidOn).ToList());

    public Task<FeesDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var totalExpected = _studentFees.Sum(fee => fee.Amount);
        var totalConcessions = _studentFees.Sum(fee => fee.ConcessionAmount);
        var totalCollected = _payments.Sum(payment => payment.Amount);
        var outstanding = _studentFees.Sum(fee => fee.BalanceAmount);

        return Task.FromResult(new FeesDashboardDto(
            totalExpected,
            totalConcessions,
            totalExpected - totalConcessions,
            totalCollected,
            outstanding,
            _studentFees.Count(fee => fee.Status == "Overdue"),
            _studentFees.Where(fee => fee.BalanceAmount > 0).ToList(),
            _payments.OrderByDescending(payment => payment.PaidOn).ToList(),
            _receipts.OrderByDescending(receipt => receipt.PaidOn).ToList()));
    }

    public Task<int> RecordPaymentAsync(RecordFeePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var feeIndex = _studentFees.FindIndex(fee => fee.Id == dto.StudentFeeId);
        if (feeIndex < 0)
        {
            throw new InvalidOperationException("Student fee not found.");
        }

        var fee = _studentFees[feeIndex];
        var paymentId = _payments.Max(payment => payment.Id) + 1;
        var receiptNumber = $"RCPT-2026-{paymentId:000}";

        _payments.Add(new FeePaymentDto(paymentId, fee.StudentName, fee.AdmissionNumber, fee.FeeName, dto.PaymentReference, dto.PaidOn, dto.Amount, dto.PaymentMethod, "Posted"));
        _receipts.Add(new FeeReceiptDto(paymentId, receiptNumber, fee.StudentName, fee.AdmissionNumber, fee.FeeName, dto.PaidOn, dto.Amount, dto.PaymentMethod, "Posted"));

        var paidAmount = fee.AmountPaid + dto.Amount;
        var balance = Math.Max(0m, fee.Amount - fee.ConcessionAmount - paidAmount);
        var status = balance == 0 ? "Paid" : "Partially Paid";
        _studentFees[feeIndex] = fee with { AmountPaid = paidAmount, BalanceAmount = balance, Status = status };

        return Task.FromResult(paymentId);
    }

    public Task UpdatePaymentAsync(int paymentId, UpdateFeePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var paymentIndex = _payments.FindIndex(payment => payment.Id == paymentId);
        if (paymentIndex < 0)
        {
            throw new InvalidOperationException("Fee payment not found.");
        }

        var current = _payments[paymentIndex];
        var feeIndex = _studentFees.FindIndex(fee =>
            fee.StudentName == current.StudentName &&
            fee.AdmissionNumber == current.AdmissionNumber &&
            fee.FeeName == current.FeeName);

        _payments[paymentIndex] = current with
        {
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            PaymentReference = dto.PaymentReference,
            PaidOn = dto.PaidOn
        };

        if (feeIndex >= 0)
        {
            var fee = _studentFees[feeIndex];
            var revisedPaidAmount = Math.Max(0m, fee.AmountPaid - current.Amount + dto.Amount);
            var revisedBalance = Math.Max(0m, fee.Amount - fee.ConcessionAmount - revisedPaidAmount);
            var revisedStatus = revisedBalance == 0 ? "Paid" : revisedPaidAmount > 0 ? "Partially Paid" : "Pending";

            _studentFees[feeIndex] = fee with
            {
                AmountPaid = revisedPaidAmount,
                BalanceAmount = revisedBalance,
                Status = revisedStatus
            };
        }

        var receiptIndex = _receipts.FindIndex(receipt => receipt.Id == paymentId);
        if (receiptIndex >= 0)
        {
            var receipt = _receipts[receiptIndex];
            _receipts[receiptIndex] = receipt with
            {
                Amount = dto.Amount,
                PaymentMethod = dto.PaymentMethod,
                PaidOn = dto.PaidOn
            };
        }

        return Task.CompletedTask;
    }

    public Task<int> CreateConcessionAsync(CreateFeeConcessionDto dto, CancellationToken cancellationToken = default)
    {
        var feeIndex = _studentFees.FindIndex(fee => fee.Id == dto.StudentFeeId);
        if (feeIndex < 0)
        {
            throw new InvalidOperationException("Student fee not found.");
        }

        var fee = _studentFees[feeIndex];
        var concessionId = _concessions.Max(concession => concession.Id) + 1;
        _concessions.Add(new FeeConcessionDto(
            concessionId,
            fee.StudentName,
            fee.AdmissionNumber,
            fee.FeeName,
            dto.ConcessionType,
            dto.Amount,
            new DateOnly(2026, 4, 8),
            "Pending Approval",
            dto.Remarks));

        _studentFees[feeIndex] = fee with
        {
            ConcessionAmount = fee.ConcessionAmount + dto.Amount,
            BalanceAmount = Math.Max(0m, fee.Amount - (fee.ConcessionAmount + dto.Amount) - fee.AmountPaid),
            Status = fee.AmountPaid >= fee.Amount - (fee.ConcessionAmount + dto.Amount) ? "Paid" : fee.AmountPaid > 0 ? "Partially Paid" : "Pending"
        };

        return Task.FromResult(concessionId);
    }

    public Task ApproveConcessionAsync(int concessionId, string approvedBy, CancellationToken cancellationToken = default)
    {
        var index = _concessions.FindIndex(concession => concession.Id == concessionId);
        if (index < 0)
        {
            throw new InvalidOperationException("Fee concession not found.");
        }

        var concession = _concessions[index];
        _concessions[index] = concession with { ApprovedBy = approvedBy };
        return Task.CompletedTask;
    }

    public Task<FeeReceiptDto> GenerateReceiptAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var receipt = _receipts.FirstOrDefault(item => item.Id == paymentId)
            ?? throw new InvalidOperationException("Fee receipt not found.");

        return Task.FromResult(receipt);
    }
}
