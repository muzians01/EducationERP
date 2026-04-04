using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class SchoolHolidayConfiguration : IEntityTypeConfiguration<SchoolHoliday>
{
    public void Configure(EntityTypeBuilder<SchoolHoliday> builder)
    {
        builder.ToTable("SchoolHolidays");

        builder.HasKey(holiday => holiday.Id);

        builder.Property(holiday => holiday.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(holiday => holiday.Category)
            .HasMaxLength(40)
            .IsRequired();

        builder.HasIndex(holiday => new { holiday.CampusId, holiday.HolidayDate, holiday.Title })
            .IsUnique();

        builder.HasOne(holiday => holiday.Campus)
            .WithMany(campus => campus.Holidays)
            .HasForeignKey(holiday => holiday.CampusId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new SchoolHoliday(1, new DateOnly(2026, 4, 14), "Ambedkar Jayanti", "National Holiday")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 5, 9, 0, 0, DateTimeKind.Local)
            },
            new SchoolHoliday(1, new DateOnly(2026, 4, 18), "PTM Preparation Day", "School Event")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 5, 9, 10, 0, DateTimeKind.Local)
            });
    }
}
