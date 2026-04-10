using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder.ToTable("Institutions");

        builder.HasKey(institution => institution.Id);

        builder.Property(institution => institution.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(institution => institution.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(institution => institution.City)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(institution => institution.State)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(institution => institution.Country)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasIndex(institution => institution.Code)
            .IsUnique();

        builder.HasData(
            new Institution("GF-EDU", "Greenfield Education Trust", "Bengaluru", "Karnataka", "India")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
