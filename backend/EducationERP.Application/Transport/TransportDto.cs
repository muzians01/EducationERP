namespace EducationERP.Application.Transport;

public sealed record TransportRouteDto(
    int Id,
    string RouteName,
    string Origin,
    string Destination,
    int AssignedVehicles,
    int ActiveTrips,
    string Status);

public sealed record TransportVehicleDto(
    int Id,
    string VehicleNumber,
    string VehicleType,
    int Capacity,
    string AssignedRoute,
    string Status);

public sealed record TransportDashboardDto(
    int TotalRoutes,
    int TotalVehicles,
    int ActiveTrips,
    int CapacityUtilizationPercentage,
    IReadOnlyList<TransportRouteDto> Routes,
    IReadOnlyList<TransportVehicleDto> Vehicles);

public sealed record CreateTransportRouteDto(
    string RouteName,
    string Origin,
    string Destination,
    string Status);

public sealed record UpdateTransportRouteDto(
    string RouteName,
    string Origin,
    string Destination,
    string Status);

public sealed record CreateTransportVehicleDto(
    string VehicleNumber,
    string VehicleType,
    int Capacity,
    int AssignedRouteId,
    string Status);

public sealed record UpdateTransportVehicleDto(
    string VehicleNumber,
    string VehicleType,
    int Capacity,
    int AssignedRouteId,
    string Status);
