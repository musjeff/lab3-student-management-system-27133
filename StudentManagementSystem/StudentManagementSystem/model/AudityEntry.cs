namespace StudentTrack.Models;

public class AuditEntry
{
    public int LogId { get; set; }
    public string Username { get; set; } = "";
    public string Action { get; set; } = "";
    public string Details { get; set; } = "";
    public DateTime LogTime { get; set; }
}