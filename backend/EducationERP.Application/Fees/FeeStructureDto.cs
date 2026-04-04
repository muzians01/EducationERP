namespace EducationERP.Application.Fees;

public sealed record FeeStructureDto(
    int Id,
    string FeeCode,
    string FeeName,
    string CampusName,
    string AcademicYearName,
    string ClassName,
    string BillingCycle,
    decimal Amount);
