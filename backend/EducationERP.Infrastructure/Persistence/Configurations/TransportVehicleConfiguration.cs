using EducationERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EducationERP.Infrastructure.Persistence.Configurations;

internal sealed class TransportVehicleConfiguration : IEntityTypeConfiguration<TransportVehicle>
{
    public void Configure(EntityTypeBuilder<TransportVehicle> builder)
    {
        builder.ToTable("TransportVehicles");

        builder.HasKey(vehicle => vehicle.Id);

        builder.Property(vehicle => vehicle.VehicleNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(vehicle => vehicle.VehicleType)
            .HasMaxLength(80)
            .IsRequired();

        builder.Property(vehicle => vehicle.Capacity)
            .IsRequired();

        builder.Property(vehicle => vehicle.Status)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(vehicle => vehicle.AssignedRouteId)
            .IsRequired();

        builder.HasIndex(vehicle => vehicle.VehicleNumber)
            .IsUnique();

        builder.HasData(
            new TransportVehicle("ERP-101", "Bus", 50, 1, "On Route")
            {
                Id = 1,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new TransportVehicle("ERP-102", "Van", 18, 2, "On Route")
            {
                Id = 2,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            },
            new TransportVehicle("ERP-103", "Bus", 52, 3, "Idle")
            {
                Id = 3,
                CreatedAt = new DateTime(2026, 4, 1, 0, 0, 0, DateTimeKind.Local)
            });
    }
}
