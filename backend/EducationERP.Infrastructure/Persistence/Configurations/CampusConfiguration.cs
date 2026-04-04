using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class CampusConfiguration : IEntityTypeConfiguration<Campus>
{
    public void Configure(EntityTypeBuilder<Campus> builder)
    {
        builder.ToTable("Campuses");

        builder.HasKey(campus => campus.Id);

        builder.Property(campus => campus.Code)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(campus => campus.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(campus => campus.City)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(campus => campus.State)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(campus => campus.Country)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(campus => campus.BoardAffiliation)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(campus => campus.Code)
            .IsUnique();

        builder.HasData(
            new Campus("HQ", "Greenfield Public School", "Bengaluru", "Karnataka", "India", "CBSE")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new Campus("WEST", "Greenfield West Campus", "Mysuru", "Karnataka", "India", "ICSE")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
