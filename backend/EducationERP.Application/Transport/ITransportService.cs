namespace EducationERP.Application.Transport;

public interface ITransportService
{
    Task<IReadOnlyList<TransportRouteDto>> GetRoutesAsync(CancellationToken cancellationToken = default);
    Task<TransportRouteDto> CreateRouteAsync(CreateTransportRouteDto dto, CancellationToken cancellationToken = default);
    Task<TransportRouteDto> UpdateRouteAsync(int routeId, UpdateTransportRouteDto dto, CancellationToken cancellationToken = default);
    Task DeleteRouteAsync(int routeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TransportVehicleDto>> GetVehiclesAsync(CancellationToken cancellationToken = default);
    Task<TransportVehicleDto> CreateVehicleAsync(CreateTransportVehicleDto dto, CancellationToken cancellationToken = default);
    Task<TransportVehicleDto> UpdateVehicleAsync(int vehicleId, UpdateTransportVehicleDto dto, CancellationToken cancellationToken = default);
    Task DeleteVehicleAsync(int vehicleId, CancellationToken cancellationToken = default);
    Task<TransportDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}
