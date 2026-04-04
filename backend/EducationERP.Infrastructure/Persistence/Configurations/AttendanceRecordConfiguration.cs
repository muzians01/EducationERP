using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        builder.ToTable("AttendanceRecords");

        builder.HasKey(record => record.Id);

        builder.Property(record => record.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(record => record.Remarks)
            .HasMaxLength(160);

        builder.HasIndex(record => new { record.StudentId, record.AttendanceDate })
            .IsUnique();

        builder.HasOne(record => record.Student)
            .WithMany(student => student.AttendanceRecords)
            .HasForeignKey(record => record.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new AttendanceRecord(1, new DateOnly(2026, 4, 1), "Present", new DateTime(2026, 4, 1, 8, 5, 0, DateTimeKind.Local))
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 1, 8, 5, 0, DateTimeKind.Local)
            },
            new AttendanceRecord(1, new DateOnly(2026, 4, 2), "Late", new DateTime(2026, 4, 2, 8, 20, 0, DateTimeKind.Local), "Arrived after assembly")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 2, 8, 20, 0, DateTimeKind.Local)
            },
            new AttendanceRecord(1, new DateOnly(2026, 4, 3), "Absent", new DateTime(2026, 4, 3, 8, 30, 0, DateTimeKind.Local), "Parent informed class teacher")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 4, 3, 8, 30, 0, DateTimeKind.Local)
            });
    }
}
