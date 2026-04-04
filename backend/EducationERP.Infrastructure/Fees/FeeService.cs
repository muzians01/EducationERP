using EducationERP.Application.Fees;
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
}
