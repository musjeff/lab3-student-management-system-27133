using StudentTrack.Data;
using StudentTrack.Models;

namespace StudentTrack.Controllers;

public class AuditController
{
    private readonly AuditRepository _repo = new();
    public List<AuditEntry> GetLog() => _repo.GetAll();
}