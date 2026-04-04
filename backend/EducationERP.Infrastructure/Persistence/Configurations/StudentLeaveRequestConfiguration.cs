using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class StudentLeaveRequestConfiguration : IEntityTypeConfiguration<StudentLeaveRequest>
{
    public void Configure(EntityTypeBuilder<StudentLeaveRequest> builder)
    {
        builder.ToTable("StudentLeaveRequests");

        builder.HasKey(request => request.Id);

        builder.Property(request => request.LeaveType)
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(request => request.Reason)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(request => request.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(request => new { request.StudentId, request.LeaveDate })
            .IsUnique();

        builder.HasOne(request => request.Student)
            .WithMany(student => student.LeaveRequests)
            .HasForeignKey(request => request.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new StudentLeaveRequest(1, new DateOnly(2026, 4, 3), "Sick Leave", "Fever and rest advised", "Approved")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 3, 7, 30, 0, DateTimeKind.Local)
            });
    }
}
