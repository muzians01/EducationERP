namespace EducationERP.Application.ParentPortal;

public sealed record ParentPortalAnnouncementDto(
    string Title,
    string Message,
    DateOnly PublishDate);
