using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class SchoolClassConfiguration : IEntityTypeConfiguration<SchoolClass>
{
    public void Configure(EntityTypeBuilder<SchoolClass> builder)
    {
        builder.ToTable("Classes");

        builder.HasKey(schoolClass => schoolClass.Id);

        builder.Property(schoolClass => schoolClass.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(schoolClass => schoolClass.Name)
            .HasMaxLength(60)
            .IsRequired();

        builder.Property(schoolClass => schoolClass.DisplayOrder)
            .IsRequired();

        builder.HasIndex(schoolClass => new { schoolClass.CampusId, schoolClass.Code })
            .IsUnique();

        builder.HasOne(schoolClass => schoolClass.Campus)
            .WithMany(campus => campus.Classes)
            .HasForeignKey(schoolClass => schoolClass.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new SchoolClass(1, "GRADE-1", "Grade 1", 1)
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new SchoolClass(1, "GRADE-2", "Grade 2", 2)
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new SchoolClass(2, "GRADE-1", "Grade 1", 1)
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
