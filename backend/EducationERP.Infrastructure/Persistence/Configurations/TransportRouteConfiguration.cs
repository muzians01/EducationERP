using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class TransportRouteConfiguration : IEntityTypeConfiguration<TransportRoute>
{
    public void Configure(EntityTypeBuilder<TransportRoute> builder)
    {
        builder.ToTable("TransportRoutes");

        builder.HasKey(route => route.Id);

        builder.Property(route => route.RouteName)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(route => route.Origin)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(route => route.Destination)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(route => route.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasMany(route => route.Vehicles)
            .WithOne(vehicle => vehicle.AssignedRoute)
            .HasForeignKey(vehicle => vehicle.AssignedRouteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new TransportRoute("North Campus Shuttle", "Main Campus", "North Campus", "Active")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new TransportRoute("East Campus Express", "Main Campus", "East Campus", "Active")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new TransportRoute("City School Link", "Downtown", "Main Campus", "Inactive")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
