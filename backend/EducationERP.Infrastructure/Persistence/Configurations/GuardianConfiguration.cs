using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class GuardianConfiguration : IEntityTypeConfiguration<Guardian>
{
    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.ToTable("Guardians");

        builder.HasKey(guardian => guardian.Id);

        builder.Property(guardian => guardian.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(guardian => guardian.Relationship)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(guardian => guardian.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(guardian => guardian.Email)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(guardian => guardian.Occupation)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasIndex(guardian => guardian.Email);

        builder.HasOne(guardian => guardian.Campus)
            .WithMany(campus => campus.Guardians)
            .HasForeignKey(guardian => guardian.CampusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            new Guardian(1, "Ananya Sharma", "Mother", "9876500001", "ananya.sharma@example.com", "Architect")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)
            },
            new Guardian(1, "Rahul Verma", "Father", "9876500002", "rahul.verma@example.com", "Business Owner")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)
            },
            new Guardian(2, "Meera Nair", "Mother", "9876500003", "meera.nair@example.com", "Doctor")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 1, 2, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
