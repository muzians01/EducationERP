using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class TransportRoute : BaseEntity
{
    private TransportRoute()
    {
    }

    public TransportRoute(string routeName, string origin, string destination, string status)
    {
        RouteName = routeName.Trim();
        Origin = origin.Trim();
        Destination = destination.Trim();
        Status = status.Trim();
        Vehicles = new List<TransportVehicle>();
    }

    public string RouteName { get; private set; } = string.Empty;
    public string Origin { get; private set; } = string.Empty;
    public string Destination { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public ICollection<TransportVehicle> Vehicles { get; private set; } = []; 

    public void UpdateDetails(string routeName, string origin, string destination, string status)
    {
        RouteName = routeName.Trim();
        Origin = origin.Trim();
        Destination = destination.Trim();
        Status = status.Trim();
    }
}
