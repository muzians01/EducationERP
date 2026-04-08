using EducationERP.Application.Fees;
using EducationERP.Domain.Entities;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Fees;

internal sealed class FeeService(EducationErpDbContext dbContext) : IFeeService
{
    public async Task<IReadOnlyList<FeeStructureDto>> GetFeeStructuresAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.FeeStructures
            .AsNoTracking()
            .Include(feeStructure => feeStructure.Campus)
            .Include(feeStructure => feeStructure.AcademicYear)
            .Include(feeStructure => feeStructure.SchoolClass)
            .OrderBy(feeStructure => feeStructure.Campus!.Name)
            .ThenBy(feeStructure => feeStructure.SchoolClass!.Name)
            .ThenBy(feeStructure => feeStructure.FeeName)
            .Select(feeStructure => new FeeStructureDto(
                feeStructure.Id,
                feeStructure.FeeCode,
                feeStructure.FeeName,
                feeStructure.Campus!.Name,
                feeStructure.AcademicYear!.Name,
                feeStructure.SchoolClass!.Name,
                feeStructure.BillingCycle,
                feeStructure.Amount))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<StudentFeeDto>> GetStudentFeesAsync(CancellationToken cancellationToken = default)
    {
        var studentFees = await dbContext.StudentFees
            .AsNoTracking()
            .Include(studentFee => studentFee.Student)
            .Include(studentFee => studentFee.FeeStructure)
            .Include(studentFee => studentFee.Concessions)
            .OrderBy(studentFee => studentFee.DueOn)
            .ToListAsync(cancellationToken);

        return studentFees
            .Select(studentFee =>
            {
                var concessionAmount = studentFee.Concessions.Sum(concession => concession.Amount);
                return new StudentFeeDto(
                studentFee.Id,
                studentFee.StudentId,
                $"{studentFee.Student!.FirstName} {studentFee.Student.LastName}",
                studentFee.Student.AdmissionNumber,
                studentFee.FeeStructure!.FeeName,
                studentFee.FeeStructure.BillingCycle,
                studentFee.DueOn,
                studentFee.Amount,
                concessionAmount,
                studentFee.AmountPaid,
                studentFee.Amount - concessionAmount - studentFee.AmountPaid,
                studentFee.Status);
            })
            .ToList();
    }

    public async Task<IReadOnlyList<FeePaymentDto>> GetRecentPaymentsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.FeePayments
            .AsNoTracking()
            .Include(payment => payment.Student)
            .Include(payment => payment.StudentFee)
            .ThenInclude(studentFee => studentFee!.FeeStructure)
            .OrderByDescending(payment => payment.PaidOn)
            .Select(payment => new FeePaymentDto(
                payment.Id,
                $"{payment.Student!.FirstName} {payment.Student.LastName}",
                payment.Student.AdmissionNumber,
                payment.StudentFee!.FeeStructure!.FeeName,
                payment.PaymentReference,
                payment.PaidOn,
                payment.Amount,
                payment.PaymentMethod,
                payment.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FeeConcessionDto>> GetConcessionsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.FeeConcessions
            .AsNoTracking()
            .Include(concession => concession.StudentFee)
            .ThenInclude(studentFee => studentFee!.Student)
            .Include(concession => concession.StudentFee)
            .ThenInclude(studentFee => studentFee!.FeeStructure)
            .OrderByDescending(concession => concession.ApprovedOn)
            .Select(concession => new FeeConcessionDto(
                concession.Id,
                $"{concession.StudentFee!.Student!.FirstName} {concession.StudentFee.Student.LastName}",
                concession.StudentFee.Student.AdmissionNumber,
                concession.StudentFee.FeeStructure!.FeeName,
                concession.ConcessionType,
                concession.Amount,
                concession.ApprovedOn,
                concession.ApprovedBy,
                concession.Remarks))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FeeReceiptDto>> GetReceiptsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.FeePayments
            .AsNoTracking()
            .Include(payment => payment.Student)
            .Include(payment => payment.StudentFee)
            .ThenInclude(studentFee => studentFee!.FeeStructure)
            .OrderByDescending(payment => payment.PaidOn)
            .Select(payment => new FeeReceiptDto(
                payment.Id,
                payment.PaymentReference,
                $"{payment.Student!.FirstName} {payment.Student.LastName}",
                payment.Student.AdmissionNumber,
                payment.StudentFee!.FeeStructure!.FeeName,
                payment.PaidOn,
                payment.Amount,
                payment.PaymentMethod,
                payment.Status))
            .ToListAsync(cancellationToken);
    }

    public async Task<FeesDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var studentFees = await GetStudentFeesAsync(cancellationToken);
        var payments = await GetRecentPaymentsAsync(cancellationToken);
        var concessions = await GetConcessionsAsync(cancellationToken);
        var receipts = await GetReceiptsAsync(cancellationToken);

        var totalExpected = studentFees.Sum(studentFee => studentFee.Amount);
        var totalConcession = studentFees.Sum(studentFee => studentFee.ConcessionAmount);
        var netReceivable = totalExpected - totalConcession;
        var totalCollected = payments.Sum(payment => payment.Amount);
        var outstanding = studentFees.Sum(studentFee => studentFee.BalanceAmount);
        var today = DateOnly.FromDateTime(DateTime.Now);

        return new FeesDashboardDto(
            totalExpected,
            totalConcession,
            netReceivable,
            totalCollected,
            outstanding,
            studentFees.Count(studentFee => studentFee.BalanceAmount > 0 && studentFee.DueOn < today),
            studentFees.Where(studentFee => studentFee.BalanceAmount > 0).ToList(),
            payments.Take(5).ToList(),
            receipts.Take(5).ToList());
    }

    public async Task<int> RecordPaymentAsync(RecordFeePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var studentFee = await dbContext.StudentFees
            .Include(sf => sf.Concessions)
            .FirstOrDefaultAsync(sf => sf.Id == dto.StudentFeeId, cancellationToken);

        if (studentFee is null)
        {
            throw new InvalidOperationException("Student fee not found.");
        }

        var paymentReference = string.IsNullOrWhiteSpace(dto.PaymentReference)
            ? await GeneratePaymentReferenceAsync(cancellationToken)
            : dto.PaymentReference.Trim().ToUpperInvariant();

        var payment = new FeePayment(
            studentFee.StudentId,
            dto.StudentFeeId,
            paymentReference,
            dto.PaidOn,
            dto.Amount,
            dto.PaymentMethod,
            "Posted");

        dbContext.FeePayments.Add(payment);

        studentFee.UpdateAmountPaid(studentFee.AmountPaid + dto.Amount);
        UpdateStudentFeeStatus(studentFee);

        await dbContext.SaveChangesAsync(cancellationToken);

        return payment.Id;
    }

    public async Task UpdatePaymentAsync(int paymentId, UpdateFeePaymentDto dto, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.FeePayments
            .Include(p => p.StudentFee)
            .ThenInclude(studentFee => studentFee!.Concessions)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException("Payment not found.");
        }

        var oldAmount = payment.Amount;
        payment.UpdatePayment(dto.Amount, dto.PaymentMethod, dto.PaymentReference, dto.PaidOn);

        payment.StudentFee!.UpdateAmountPaid(payment.StudentFee.AmountPaid - oldAmount + dto.Amount);
        UpdateStudentFeeStatus(payment.StudentFee);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CreateConcessionAsync(CreateFeeConcessionDto dto, CancellationToken cancellationToken = default)
    {
        var studentFee = await dbContext.StudentFees
            .Include(item => item.Concessions)
            .FirstOrDefaultAsync(item => item.Id == dto.StudentFeeId, cancellationToken);

        if (studentFee is null)
        {
            throw new InvalidOperationException("Student fee not found.");
        }

        var concession = new FeeConcession(
            dto.StudentFeeId,
            dto.ConcessionType,
            dto.Amount,
            DateOnly.FromDateTime(DateTime.Now),
            "System", // TODO: Get from current user
            dto.Remarks);

        dbContext.FeeConcessions.Add(concession);
        studentFee.Concessions.Add(concession);
        UpdateStudentFeeStatus(studentFee);
        await dbContext.SaveChangesAsync(cancellationToken);

        return concession.Id;
    }

    public async Task ApproveConcessionAsync(int concessionId, string approvedBy, CancellationToken cancellationToken = default)
    {
        var concession = await dbContext.FeeConcessions
            .FirstOrDefaultAsync(c => c.Id == concessionId, cancellationToken);

        if (concession is null)
        {
            throw new InvalidOperationException("Concession not found.");
        }

        concession.UpdateApprovedBy(approvedBy);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<FeeReceiptDto> GenerateReceiptAsync(int paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await dbContext.FeePayments
            .AsNoTracking()
            .Include(p => p.Student)
            .Include(p => p.StudentFee)
            .ThenInclude(sf => sf!.FeeStructure)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

        if (payment is null)
        {
            throw new InvalidOperationException("Payment not found.");
        }

        return new FeeReceiptDto(
            payment.Id,
            payment.PaymentReference,
            $"{payment.Student!.FirstName} {payment.Student.LastName}",
            payment.Student.AdmissionNumber,
            payment.StudentFee!.FeeStructure!.FeeName,
            payment.PaidOn,
            payment.Amount,
            payment.PaymentMethod,
            payment.Status);
    }

    private async Task<string> GeneratePaymentReferenceAsync(CancellationToken cancellationToken)
    {
        var lastPayment = await dbContext.FeePayments
            .OrderByDescending(p => p.Id)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = lastPayment is null ? 1 : int.Parse(lastPayment.PaymentReference[3..]) + 1;
        return $"PAY{nextNumber:D6}";
    }

    private static void UpdateStudentFeeStatus(StudentFee studentFee)
    {
        var netPayable = studentFee.Amount - studentFee.Concessions.Sum(c => c.Amount);

        if (studentFee.AmountPaid >= netPayable)
        {
            studentFee.UpdateStatus("Paid");
            return;
        }

        studentFee.UpdateStatus(studentFee.AmountPaid > 0 ? "Partially Paid" : "Pending");
    }
}
