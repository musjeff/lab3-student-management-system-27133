
using MySqlConnector;
using StudentTrack.Models;

namespace StudentTrack.Data;

public class AuditRepository
{
    public void Log(int? userId, string action, string details)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(
            "INSERT INTO AuditLog(UserId,Action,Details) VALUES(@u,@a,@d)", conn);
        cmd.Parameters.AddWithValue("@u", userId.HasValue ? userId.Value : DBNull.Value);
        cmd.Parameters.AddWithValue("@a", action);
        cmd.Parameters.AddWithValue("@d", details);
        cmd.ExecuteNonQuery();
    }

    public List<AuditEntry> GetAll()
    {
        var list = new List<AuditEntry>();
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(
            @"SELECT l.LogId, COALESCE(u.Username,'(none)') AS Username,
                     l.Action, COALESCE(l.Details,'') AS Details, l.LogTime
              FROM AuditLog l
              LEFT JOIN Users u ON u.UserId = l.UserId
              ORDER BY l.LogId DESC", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new AuditEntry
            {
                LogId = Convert.ToInt32(r["LogId"]),
                Username = Convert.ToString(r["Username"]) ?? "",
                Action = Convert.ToString(r["Action"]) ?? "",
                Details = Convert.ToString(r["Details"]) ?? "",
                LogTime = Convert.ToDateTime(r["LogTime"])
            });
        }
        return list;
    }
}