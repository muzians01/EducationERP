using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");

        builder.HasKey(subject => subject.Id);

        builder.Property(subject => subject.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(subject => subject.Name)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(subject => subject.Category)
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(subject => new { subject.CampusId, subject.Code })
            .IsUnique();

        builder.HasOne(subject => subject.Campus)
            .WithMany()
            .HasForeignKey(subject => subject.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Subject(1, "ENG", "English", "Language", 6)
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 4, 8, 0, 0, DateTimeKind.Local)
            },
            new Subject(1, "MAT", "Mathematics", "Core", 7)
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 4, 8, 0, 0, DateTimeKind.Local)
            },
            new Subject(1, "SCI", "Science", "Core", 5)
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 4, 4, 8, 0, 0, DateTimeKind.Local)
            },
            new Subject(1, "SST", "Social Studies", "Humanities", 4)
            {
                Id = 4,
                CreatedAt = new DateTime(2026, 4, 4, 8, 0, 0, DateTimeKind.Local)
            },
            new Subject(1, "ART", "Art & Craft", "Activity", 2)
            {
                Id = 5,
                CreatedAt = new DateTime(2026, 4, 4, 8, 0, 0, DateTimeKind.Local)
            });
    }
}
