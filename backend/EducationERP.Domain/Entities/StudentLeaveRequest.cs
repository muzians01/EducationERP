using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class StudentLeaveRequest : BaseEntity
{
    private StudentLeaveRequest()
    {
    }

    public StudentLeaveRequest(
        int studentId,
        DateOnly leaveDate,
        string leaveType,
        string reason,
        string status)
    {
        StudentId = studentId;
        LeaveDate = leaveDate;
        LeaveType = leaveType.Trim();
        Reason = reason.Trim();
        Status = status.Trim();
    }

    public int StudentId { get; private set; }
    public DateOnly LeaveDate { get; private set; }
    public string LeaveType { get; private set; } = string.Empty;
    public string Reason { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;

    public Student? Student { get; private set; }

    public void UpdateDetails(DateOnly leaveDate, string leaveType, string reason)
    {
        LeaveDate = leaveDate;
        LeaveType = leaveType.Trim();
        Reason = reason.Trim();
        Touch();
    }

    public void UpdateStatus(string status)
    {
        Status = status.Trim();
        Touch();
    }
}
