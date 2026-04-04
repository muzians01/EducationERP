namespace EducationERP.Application.Examinations;

public sealed record ExaminationsDashboardDto(
    int SelectedExamTermId,
    string SelectedExamTermName,
    int SelectedClassId,
    string SelectedClassName,
    int SelectedSectionId,
    string SelectedSectionName,
    IReadOnlyList<ExamTermDto> ExamTerms,
    IReadOnlyList<ExamScheduleDto> Schedule,
    IReadOnlyList<StudentReportCardDto> ReportCards);
