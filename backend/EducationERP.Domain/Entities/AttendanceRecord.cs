using EducationERP.Domain.Common;

namespace EducationERP.Domain.Entities;

public sealed class AttendanceRecord : BaseEntity
{
    private AttendanceRecord()
    {
    }

    public AttendanceRecord(
        int studentId,
        DateOnly attendanceDate,
        string status,
        DateTime markedOn,
        string? remarks = null)
    {
        StudentId = studentId;
        AttendanceDate = attendanceDate;
        Status = status.Trim();
        MarkedOn = markedOn;
        Remarks = remarks?.Trim();
    }

    public int StudentId { get; private set; }
    public DateOnly AttendanceDate { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime MarkedOn { get; private set; }
    public string? Remarks { get; private set; }

    public Student? Student { get; private set; }

    public void Mark(string status, DateTime markedOn, string? remarks = null)
    {
        Status = status.Trim();
        MarkedOn = markedOn;
        Remarks = remarks?.Trim();
        Touch();
    }
}
