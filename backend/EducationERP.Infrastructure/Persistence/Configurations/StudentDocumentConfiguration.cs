using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class StudentDocumentConfiguration : IEntityTypeConfiguration<StudentDocument>
{
    public void Configure(EntityTypeBuilder<StudentDocument> builder)
    {
        builder.ToTable("StudentDocuments");

        builder.HasKey(document => document.Id);

        builder.Property(document => document.DocumentType)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(document => document.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(document => document.Remarks)
            .HasMaxLength(200);

        builder.HasOne(document => document.Student)
            .WithMany(student => student.Documents)
            .HasForeignKey(document => document.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new StudentDocument(1, "Birth Certificate", "Verified", new DateOnly(2026, 4, 1), new DateOnly(2026, 4, 1), "Original verified at front desk")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Local)
            },
            new StudentDocument(1, "Transfer Certificate", "Pending", new DateOnly(2026, 4, 1), null, "Awaiting previous school submission")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 1, 9, 30, 0, DateTimeKind.Local)
            },
            new StudentDocument(1, "Immunization Record", "Verified", new DateOnly(2026, 4, 2), new DateOnly(2026, 4, 2), "Uploaded by guardian and reviewed")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 4, 2, 10, 15, 0, DateTimeKind.Local)
            });
    }
}
