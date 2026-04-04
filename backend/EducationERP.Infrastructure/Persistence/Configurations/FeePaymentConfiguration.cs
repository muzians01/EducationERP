using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class FeePaymentConfiguration : IEntityTypeConfiguration<FeePayment>
{
    public void Configure(EntityTypeBuilder<FeePayment> builder)
    {
        builder.ToTable("FeePayments");

        builder.HasKey(payment => payment.Id);

        builder.Property(payment => payment.PaymentReference)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(payment => payment.Amount)
            .HasPrecision(12, 2);

        builder.Property(payment => payment.PaymentMethod)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(payment => payment.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(payment => payment.PaymentReference)
            .IsUnique();

        builder.HasOne(payment => payment.Student)
            .WithMany(student => student.FeePayments)
            .HasForeignKey(payment => payment.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(payment => payment.StudentFee)
            .WithMany(studentFee => studentFee.Payments)
            .HasForeignKey(payment => payment.StudentFeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new FeePayment(1, 1, "RCPT-2026-001", new DateOnly(2026, 4, 3), 24000m, "UPI", "Posted")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 3, 11, 0, 0, DateTimeKind.Local)
            });
    }
}
