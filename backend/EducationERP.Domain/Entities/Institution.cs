using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class Institution : BaseEntity
{
    private Institution()
    {
    }

    public Institution(string code, string name, string city, string state, string country)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        City = city.Trim();
        State = state.Trim();
        Country = country.Trim();
    }

    public void UpdateDetails(string code, string name, string city, string state, string country)
    {
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        City = city.Trim();
        State = state.Trim();
        Country = country.Trim();
        Touch();
    }

    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;

    public ICollection<Campus> Campuses { get; private set; } = [];
}
