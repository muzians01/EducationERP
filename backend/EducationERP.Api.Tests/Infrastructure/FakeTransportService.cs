using EducationERP.Application.Transport;

namespace EducationERP.Api.Tests.Infrastructure;

internal sealed class FakeTransportService : ITransportService
{
    private readonly List<TransportRouteDto> _routes =
    [
        new TransportRouteDto(1, "North Campus Shuttle", "Main Campus", "North Campus", 1, 1, "Active"),
        new TransportRouteDto(2, "East Campus Express", "Main Campus", "East Campus", 1, 1, "Active"),
        new TransportRouteDto(3, "City School Link", "Downtown", "Main Campus", 1, 0, "Inactive")
    ];

    private readonly List<TransportVehicleDto> _vehicles =
    [
        new TransportVehicleDto(1, "ERP-101", "Bus", 50, "North Campus Shuttle", "On Route"),
        new TransportVehicleDto(2, "ERP-102", "Van", 18, "East Campus Express", "On Route"),
        new TransportVehicleDto(3, "ERP-103", "Bus", 52, "City School Link", "Idle")
    ];

    public Task<IReadOnlyList<TransportRouteDto>> GetRoutesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TransportRouteDto>>(_routes.OrderBy(route => route.RouteName).ToList());

    public Task<TransportRouteDto> CreateRouteAsync(CreateTransportRouteDto dto, CancellationToken cancellationToken = default)
    {
        var nextId = _routes.Max(route => route.Id) + 1;
        var route = new TransportRouteDto(nextId, dto.RouteName, dto.Origin, dto.Destination, 0, 0, dto.Status);
        _routes.Add(route);
        return Task.FromResult(route);
    }

    public Task<TransportRouteDto> UpdateRouteAsync(int routeId, UpdateTransportRouteDto dto, CancellationToken cancellationToken = default)
    {
        var index = _routes.FindIndex(route => route.Id == routeId);
        if (index < 0)
        {
            throw new InvalidOperationException("Transport route not found.");
        }

        var current = _routes[index];
        var assignedVehicles = _vehicles.Count(vehicle => vehicle.AssignedRoute == current.RouteName);
        var activeTrips = _vehicles.Count(vehicle => vehicle.AssignedRoute == current.RouteName && vehicle.Status == "On Route");
        var updated = new TransportRouteDto(routeId, dto.RouteName, dto.Origin, dto.Destination, assignedVehicles, activeTrips, dto.Status);

        _routes[index] = updated;

        for (var i = 0; i < _vehicles.Count; i++)
        {
            if (_vehicles[i].AssignedRoute == current.RouteName)
            {
                _vehicles[i] = _vehicles[i] with { AssignedRoute = dto.RouteName };
            }
        }

        return Task.FromResult(updated);
    }

    public Task DeleteRouteAsync(int routeId, CancellationToken cancellationToken = default)
    {
        var route = _routes.FirstOrDefault(item => item.Id == routeId)
            ?? throw new InvalidOperationException("Transport route not found.");

        _routes.RemoveAll(item => item.Id == routeId);
        _vehicles.RemoveAll(vehicle => vehicle.AssignedRoute == route.RouteName);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TransportVehicleDto>> GetVehiclesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TransportVehicleDto>>(_vehicles.OrderBy(vehicle => vehicle.VehicleNumber).ToList());

    public Task<TransportVehicleDto> CreateVehicleAsync(CreateTransportVehicleDto dto, CancellationToken cancellationToken = default)
    {
        var route = _routes.FirstOrDefault(item => item.Id == dto.AssignedRouteId)
            ?? throw new InvalidOperationException("Assigned transport route not found.");

        var nextId = _vehicles.Max(vehicle => vehicle.Id) + 1;
        var vehicle = new TransportVehicleDto(nextId, dto.VehicleNumber.ToUpperInvariant(), dto.VehicleType, dto.Capacity, route.RouteName, dto.Status);
        _vehicles.Add(vehicle);
        SyncRouteCounts();
        return Task.FromResult(vehicle);
    }

    public Task<TransportVehicleDto> UpdateVehicleAsync(int vehicleId, UpdateTransportVehicleDto dto, CancellationToken cancellationToken = default)
    {
        var index = _vehicles.FindIndex(vehicle => vehicle.Id == vehicleId);
        if (index < 0)
        {
            throw new InvalidOperationException("Transport vehicle not found.");
        }

        var route = _routes.FirstOrDefault(item => item.Id == dto.AssignedRouteId)
            ?? throw new InvalidOperationException("Assigned transport route not found.");

        var updated = new TransportVehicleDto(vehicleId, dto.VehicleNumber.ToUpperInvariant(), dto.VehicleType, dto.Capacity, route.RouteName, dto.Status);
        _vehicles[index] = updated;
        SyncRouteCounts();
        return Task.FromResult(updated);
    }

    public Task DeleteVehicleAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        _vehicles.RemoveAll(vehicle => vehicle.Id == vehicleId);
        SyncRouteCounts();
        return Task.CompletedTask;
    }

    public Task<TransportDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        SyncRouteCounts();
        var routes = _routes.OrderBy(route => route.RouteName).ToList();
        var vehicles = _vehicles.OrderBy(vehicle => vehicle.VehicleNumber).ToList();
        var activeTrips = routes.Sum(route => route.ActiveTrips);
        var onRouteVehicles = vehicles.Count(vehicle => vehicle.Status == "On Route");
        var capacityUtilization = vehicles.Count == 0 ? 0 : (int)Math.Round(onRouteVehicles / (double)vehicles.Count * 100);

        return Task.FromResult(new TransportDashboardDto(routes.Count, vehicles.Count, activeTrips, capacityUtilization, routes, vehicles));
    }

    private void SyncRouteCounts()
    {
        for (var i = 0; i < _routes.Count; i++)
        {
            var route = _routes[i];
            var assignedVehicles = _vehicles.Count(vehicle => vehicle.AssignedRoute == route.RouteName);
            var activeTrips = _vehicles.Count(vehicle => vehicle.AssignedRoute == route.RouteName && vehicle.Status == "On Route");
            _routes[i] = route with { AssignedVehicles = assignedVehicles, ActiveTrips = activeTrips };
        }
    }
}
