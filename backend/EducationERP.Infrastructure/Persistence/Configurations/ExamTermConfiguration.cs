using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class ExamTermConfiguration : IEntityTypeConfiguration<ExamTerm>
{
    public void Configure(EntityTypeBuilder<ExamTerm> builder)
    {
        builder.ToTable("ExamTerms");

        builder.HasKey(term => term.Id);

        builder.Property(term => term.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(term => term.ExamType)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(term => term.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasOne(term => term.Campus)
            .WithMany()
            .HasForeignKey(term => term.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(term => term.AcademicYear)
            .WithMany()
            .HasForeignKey(term => term.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new ExamTerm(1, 1, "Term 1 Assessment", "Scholastic", new DateOnly(2026, 9, 14), new DateOnly(2026, 9, 18), "Scheduled")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 4, 12, 0, 0, DateTimeKind.Local)
            });
    }
}
