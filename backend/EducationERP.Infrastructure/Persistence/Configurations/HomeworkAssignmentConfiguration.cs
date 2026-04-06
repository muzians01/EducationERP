using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class HomeworkAssignmentConfiguration : IEntityTypeConfiguration<HomeworkAssignment>
{
    public void Configure(EntityTypeBuilder<HomeworkAssignment> builder)
    {
        builder.ToTable("HomeworkAssignments");

        builder.HasKey(item => item.Id);

        builder.Property(item => item.Title)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(item => item.Instructions)
            .HasMaxLength(400)
            .IsRequired();

        builder.Property(item => item.AssignedBy)
            .HasMaxLength(80)
            .IsRequired();

        builder.HasOne(item => item.AcademicYear)
            .WithMany()
            .HasForeignKey(item => item.AcademicYearId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.SchoolClass)
            .WithMany()
            .HasForeignKey(item => item.SchoolClassId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Section)
            .WithMany()
            .HasForeignKey(item => item.SectionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(item => item.Subject)
            .WithMany()
            .HasForeignKey(item => item.SubjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
            CreateAssignment(1, 1, 2, 1, new DateOnly(2026, 4, 6), new DateOnly(2026, 4, 8), "Reading journal", "Read chapter 4 and write five new vocabulary words.", "Anita Rao"),
            CreateAssignment(2, 1, 2, 2, new DateOnly(2026, 4, 6), new DateOnly(2026, 4, 7), "Number patterns", "Complete workbook page 18 and revise skip counting.", "Rahul Mehta"),
            CreateAssignment(3, 1, 2, 3, new DateOnly(2026, 4, 5), new DateOnly(2026, 4, 9), "Plant observation", "Observe one plant at home and note changes for three days.", "Priya Nair"));
    }

    private static HomeworkAssignment CreateAssignment(int id, int academicYearId, int sectionId, int subjectId, DateOnly assignedOn, DateOnly dueOn, string title, string instructions, string assignedBy)
    {
        return new HomeworkAssignment(
            academicYearId,
            1,
            sectionId,
            subjectId,
            assignedOn,
            dueOn,
            title,
            instructions,
            assignedBy)
        {
            Id = id,
            CreatedAt = new DateTime(2026, 4, 6, 9, 0, 0, DateTimeKind.Local)
        };
    }
}
