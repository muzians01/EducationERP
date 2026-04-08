using EducationERP.Application.Transport;
using EducationERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EducationERP.Infrastructure.Transport;

internal sealed class TransportService(EducationErpDbContext dbContext) : ITransportService
{
    public async Task<IReadOnlyList<TransportRouteDto>> GetRoutesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.TransportRoutes
            .AsNoTracking()
            .Include(route => route.Vehicles)
            .OrderBy(route => route.RouteName)
            .Select(route => MapRoute(route))
            .ToListAsync(cancellationToken);
    }

    public async Task<TransportRouteDto> CreateRouteAsync(CreateTransportRouteDto dto, CancellationToken cancellationToken = default)
    {
        var route = new Domain.Entities.TransportRoute(dto.RouteName, dto.Origin, dto.Destination, dto.Status);
        dbContext.TransportRoutes.Add(route);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TransportRouteDto(route.Id, route.RouteName, route.Origin, route.Destination, 0, 0, route.Status);
    }

    public async Task<TransportRouteDto> UpdateRouteAsync(int routeId, UpdateTransportRouteDto dto, CancellationToken cancellationToken = default)
    {
        var route = await dbContext.TransportRoutes
            .Include(item => item.Vehicles)
            .FirstOrDefaultAsync(item => item.Id == routeId, cancellationToken);

        if (route is null)
        {
            throw new InvalidOperationException("Transport route not found.");
        }

        route.UpdateDetails(dto.RouteName, dto.Origin, dto.Destination, dto.Status);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MapRoute(route);
    }

    public async Task DeleteRouteAsync(int routeId, CancellationToken cancellationToken = default)
    {
        var route = await dbContext.TransportRoutes.FindAsync(new object?[] { routeId }, cancellationToken);
        if (route is null)
        {
            throw new InvalidOperationException("Transport route not found.");
        }

        dbContext.TransportRoutes.Remove(route);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TransportVehicleDto>> GetVehiclesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.TransportVehicles
            .AsNoTracking()
            .Include(vehicle => vehicle.AssignedRoute)
            .OrderBy(vehicle => vehicle.VehicleNumber)
            .Select(vehicle => MapVehicle(vehicle))
            .ToListAsync(cancellationToken);
    }

    public async Task<TransportVehicleDto> CreateVehicleAsync(CreateTransportVehicleDto dto, CancellationToken cancellationToken = default)
    {
        await EnsureRouteExistsAsync(dto.AssignedRouteId, cancellationToken);

        var vehicle = new Domain.Entities.TransportVehicle(dto.VehicleNumber, dto.VehicleType, dto.Capacity, dto.AssignedRouteId, dto.Status);
        dbContext.TransportVehicles.Add(vehicle);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(vehicle).Reference(item => item.AssignedRoute).LoadAsync(cancellationToken);

        return MapVehicle(vehicle);
    }

    public async Task<TransportVehicleDto> UpdateVehicleAsync(int vehicleId, UpdateTransportVehicleDto dto, CancellationToken cancellationToken = default)
    {
        var vehicle = await dbContext.TransportVehicles
            .Include(item => item.AssignedRoute)
            .FirstOrDefaultAsync(item => item.Id == vehicleId, cancellationToken);

        if (vehicle is null)
        {
            throw new InvalidOperationException("Transport vehicle not found.");
        }

        await EnsureRouteExistsAsync(dto.AssignedRouteId, cancellationToken);

        vehicle.UpdateDetails(dto.VehicleNumber, dto.VehicleType, dto.Capacity, dto.AssignedRouteId, dto.Status);
        await dbContext.SaveChangesAsync(cancellationToken);
        await dbContext.Entry(vehicle).Reference(item => item.AssignedRoute).LoadAsync(cancellationToken);

        return MapVehicle(vehicle);
    }

    public async Task DeleteVehicleAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicle = await dbContext.TransportVehicles.FindAsync(new object?[] { vehicleId }, cancellationToken);
        if (vehicle is null)
        {
            throw new InvalidOperationException("Transport vehicle not found.");
        }

        dbContext.TransportVehicles.Remove(vehicle);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TransportDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var routes = await GetRoutesAsync(cancellationToken);
        var vehicles = await GetVehiclesAsync(cancellationToken);
        var totalCapacity = vehicles.Sum(vehicle => vehicle.Capacity);
        var onRouteCount = vehicles.Count(vehicle => vehicle.Status == "On Route");

        var capacityUtilization = totalCapacity == 0
            ? 0
            : Math.Min(100, (int)Math.Round(onRouteCount / (double)vehicles.Count * 100));

        return new TransportDashboardDto(
            TotalRoutes: routes.Count,
            TotalVehicles: vehicles.Count,
            ActiveTrips: routes.Sum(route => route.ActiveTrips),
            CapacityUtilizationPercentage: capacityUtilization,
            Routes: routes,
            Vehicles: vehicles);
    }

    private async Task EnsureRouteExistsAsync(int routeId, CancellationToken cancellationToken)
    {
        var exists = await dbContext.TransportRoutes
            .AsNoTracking()
            .AnyAsync(route => route.Id == routeId, cancellationToken);

        if (!exists)
        {
            throw new InvalidOperationException("Assigned transport route not found.");
        }
    }

    private static TransportRouteDto MapRoute(Domain.Entities.TransportRoute route) =>
        new(
            route.Id,
            route.RouteName,
            route.Origin,
            route.Destination,
            route.Vehicles.Count,
            route.Vehicles.Count(vehicle => vehicle.Status == "On Route"),
            route.Status);

    private static TransportVehicleDto MapVehicle(Domain.Entities.TransportVehicle vehicle) =>
        new(
            vehicle.Id,
            vehicle.VehicleNumber,
            vehicle.VehicleType,
            vehicle.Capacity,
            vehicle.AssignedRoute.RouteName,
            vehicle.Status);
}
