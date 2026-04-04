using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class StudentFeeConfiguration : IEntityTypeConfiguration<StudentFee>
{
    public void Configure(EntityTypeBuilder<StudentFee> builder)
    {
        builder.ToTable("StudentFees");

        builder.HasKey(studentFee => studentFee.Id);

        builder.Property(studentFee => studentFee.Amount)
            .HasPrecision(12, 2);

        builder.Property(studentFee => studentFee.AmountPaid)
            .HasPrecision(12, 2);

        builder.Property(studentFee => studentFee.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(studentFee => studentFee.Student)
            .WithMany(student => student.Fees)
            .HasForeignKey(studentFee => studentFee.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(studentFee => studentFee.FeeStructure)
            .WithMany(feeStructure => feeStructure.StudentFees)
            .HasForeignKey(studentFee => studentFee.FeeStructureId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new StudentFee(1, 1, new DateOnly(2026, 6, 10), 48000m, 24000m, "Partially Paid")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 2, 9, 0, 0, DateTimeKind.Local)
            },
            new StudentFee(1, 2, new DateOnly(2026, 6, 10), 12000m, 0m, "Pending")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 2, 9, 10, 0, DateTimeKind.Local)
            });
    }
}
