namespace EducationERP.Application.Fees;

public sealed record FeeConcessionDto(
    int Id,
    string StudentName,
    string AdmissionNumber,
    string FeeName,
    string ConcessionType,
    decimal Amount,
    DateOnly ApprovedOn,
    string ApprovedBy,
    string? Remarks);
