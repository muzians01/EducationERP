using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class TransportVehicle : BaseEntity
{
    private TransportVehicle()
    {
    }

    public TransportVehicle(string vehicleNumber, string vehicleType, int capacity, int assignedRouteId, string status)
    {
        VehicleNumber = vehicleNumber.Trim().ToUpperInvariant();
        VehicleType = vehicleType.Trim();
        Capacity = capacity;
        AssignedRouteId = assignedRouteId;
        Status = status.Trim();
    }

    public string VehicleNumber { get; private set; } = string.Empty;
    public string VehicleType { get; private set; } = string.Empty;
    public int Capacity { get; private set; }
    public int AssignedRouteId { get; private set; }
    public TransportRoute AssignedRoute { get; private set; } = null!;
    public string Status { get; private set; } = string.Empty;

    public void UpdateDetails(string vehicleNumber, string vehicleType, int capacity, int assignedRouteId, string status)
    {
        VehicleNumber = vehicleNumber.Trim().ToUpperInvariant();
        VehicleType = vehicleType.Trim();
        Capacity = capacity;
        AssignedRouteId = assignedRouteId;
        Status = status.Trim();
    }
}
