namespace EducationERP.Application.Homework;

public sealed record HomeworkDashboardDto(
    int SelectedClassId,
    string SelectedClassName,
    int SelectedSectionId,
    string SelectedSectionName,
    int ActiveAssignments,
    int PendingSubmissions,
    IReadOnlyList<HomeworkAssignmentDto> Assignments,
    IReadOnlyList<StudentHomeworkProgressDto> Progress);
