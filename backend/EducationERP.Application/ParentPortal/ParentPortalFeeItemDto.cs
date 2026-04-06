namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalFeeItemDto(
    string FeeName,
    DateOnly DueOn,
    decimal BalanceAmount,
    string Status);
