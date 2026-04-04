using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class FeeConcessionConfiguration : IEntityTypeConfiguration<FeeConcession>
{
    public void Configure(EntityTypeBuilder<FeeConcession> builder)
    {
        builder.ToTable("FeeConcessions");

        builder.HasKey(concession => concession.Id);

        builder.Property(concession => concession.ConcessionType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(concession => concession.Amount)
            .HasPrecision(12, 2);

        builder.Property(concession => concession.ApprovedBy)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(concession => concession.Remarks)
            .HasMaxLength(200);

        builder.HasOne(concession => concession.StudentFee)
            .WithMany(studentFee => studentFee.Concessions)
            .HasForeignKey(concession => concession.StudentFeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new FeeConcession(1, "Sibling Concession", 6000m, new DateOnly(2026, 4, 4), "Bursar", "Approved for sibling admission")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 4, 10, 0, 0, DateTimeKind.Local)
            });
    }
}
