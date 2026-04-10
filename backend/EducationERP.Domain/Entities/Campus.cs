using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class Campus : BaseEntity
{
    private Campus()
    {
    }

    public Campus(int institutionId, string code, string name, string city, string state, string country, string boardAffiliation)
    {
        InstitutionId = institutionId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        City = city.Trim();
        State = state.Trim();
        Country = country.Trim();
        BoardAffiliation = boardAffiliation.Trim();
    }

    public void UpdateDetails(int institutionId, string code, string name, string city, string state, string country, string boardAffiliation)
    {
        InstitutionId = institutionId;
        Code = code.Trim().ToUpperInvariant();
        Name = name.Trim();
        City = city.Trim();
        State = state.Trim();
        Country = country.Trim();
        BoardAffiliation = boardAffiliation.Trim();
        Touch();
    }

    public int InstitutionId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string State { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string BoardAffiliation { get; private set; } = string.Empty;
    public Institution? Institution { get; private set; }
    public ICollection<AcademicYear> AcademicYears { get; private set; } = [];
    public ICollection<SchoolClass> Classes { get; private set; } = [];
    public ICollection<Guardian> Guardians { get; private set; } = [];
    public ICollection<AdmissionApplication> AdmissionApplications { get; private set; } = [];
    public ICollection<Student> Students { get; private set; } = [];
    public ICollection<FeeStructure> FeeStructures { get; private set; } = [];
    public ICollection<SchoolHoliday> Holidays { get; private set; } = [];
}
