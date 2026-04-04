using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(section => section.Id);

        builder.Property(section => section.Name)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(section => section.Capacity)
            .IsRequired();

        builder.Property(section => section.RoomNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(section => new { section.SchoolClassId, section.Name })
            .IsUnique();

        builder.HasOne(section => section.SchoolClass)
            .WithMany(schoolClass => schoolClass.Sections)
            .HasForeignKey(section => section.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Section(1, "A", 35, "G1-A01")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new Section(1, "B", 35, "G1-B01")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new Section(2, "A", 35, "G2-A01")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new Section(3, "A", 30, "W-G1-A01")
            {
                Id = 4,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
