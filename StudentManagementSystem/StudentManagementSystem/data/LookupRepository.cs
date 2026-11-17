
using MySqlConnector;
using StudentTrack.Models;

namespace StudentTrack.Data;

public class LookupRepository
{
    public List<Lookup> GetDepartments() =>
        Query("SELECT DepartmentId, DepartmentName FROM Departments ORDER BY DepartmentName");

    public List<Lookup> GetSemesters() =>
        Query("SELECT SemesterId, SemesterName FROM Semesters ORDER BY SemesterId");

    public List<Lookup> GetCourses() =>
        Query("SELECT CourseId, CourseName FROM Courses ORDER BY CourseId");

    private static List<Lookup> Query(string sql)
    {
        var list = new List<Lookup>();
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(sql, conn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            list.Add(new Lookup { Id = r.GetInt32(0), Name = r.GetString(1) });
        return list;
    }
}