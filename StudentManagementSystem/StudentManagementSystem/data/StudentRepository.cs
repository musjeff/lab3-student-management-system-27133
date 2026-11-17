
using MySqlConnector;
using StudentTrack.Models;

namespace StudentTrack.Data;

public class StudentRepository
{
    // FR4: search by name or ID. GPA comes from the vw_StudentGPA view.
    public List<Student> GetAll(string search = "")
    {
        var list = new List<Student>();
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();

        string sql = @"SELECT s.StudentId, s.Name, s.Age, s.Email, s.DepartmentId,
                              d.DepartmentName, g.GPA
                       FROM Students s
                       JOIN Departments d ON d.DepartmentId = s.DepartmentId
                       LEFT JOIN vw_StudentGPA g ON g.StudentId = s.StudentId";
        bool hasSearch = !string.IsNullOrWhiteSpace(search);
        if (hasSearch)
            sql += " WHERE s.Name LIKE @s OR CAST(s.StudentId AS CHAR) LIKE @s";
        sql += " ORDER BY s.StudentId";

        using var cmd = new MySqlCommand(sql, conn);
        if (hasSearch) cmd.Parameters.AddWithValue("@s", "%" + search + "%");

        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            list.Add(new Student
            {
                StudentId = Convert.ToInt32(r["StudentId"]),
                Name = Convert.ToString(r["Name"]) ?? "",
                Age = Convert.ToInt32(r["Age"]),
                Email = Convert.ToString(r["Email"]) ?? "",
                DepartmentId = Convert.ToInt32(r["DepartmentId"]),
                DepartmentName = Convert.ToString(r["DepartmentName"]) ?? "",
                Gpa = r["GPA"] is DBNull ? 0 : Convert.ToDouble(r["GPA"])
            });
        }
        return list;
    }

    // Marks of one student (CourseId -> Mark) and the semester they belong to
    public (Dictionary<int, double> Marks, int SemesterId) GetMarks(int studentId)
    {
        var marks = new Dictionary<int, double>();
        int semesterId = 0;
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT CourseId, SemesterId, Mark FROM Enrollments WHERE StudentId=@id", conn);
        cmd.Parameters.AddWithValue("@id", studentId);
        using var r = cmd.ExecuteReader();
        while (r.Read())
        {
            marks[Convert.ToInt32(r["CourseId"])] = Convert.ToDouble(r["Mark"]);
            semesterId = Convert.ToInt32(r["SemesterId"]);
        }
        return (marks, semesterId);
    }

    // 1 student row + 9 enrollment rows, in one transaction
    public int Insert(Student s, Dictionary<int, double> marks, int semesterId)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        using var cmd = new MySqlCommand(
            "INSERT INTO Students(Name,Age,Email,DepartmentId) VALUES(@n,@a,@e,@d)", conn, tx);
        cmd.Parameters.AddWithValue("@n", s.Name);
        cmd.Parameters.AddWithValue("@a", s.Age);
        cmd.Parameters.AddWithValue("@e", s.Email);
        cmd.Parameters.AddWithValue("@d", s.DepartmentId);
        cmd.ExecuteNonQuery();
        int id = (int)cmd.LastInsertedId;

        InsertMarks(conn, tx, id, marks, semesterId);
        tx.Commit();
        return id;
    }

    public void Update(Student s, Dictionary<int, double> marks, int semesterId)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var tx = conn.BeginTransaction();

        using (var cmd = new MySqlCommand(
            "UPDATE Students SET Name=@n, Age=@a, Email=@e, DepartmentId=@d WHERE StudentId=@id",
            conn, tx))
        {
            cmd.Parameters.AddWithValue("@n", s.Name);
            cmd.Parameters.AddWithValue("@a", s.Age);
            cmd.Parameters.AddWithValue("@e", s.Email);
            cmd.Parameters.AddWithValue("@d", s.DepartmentId);
            cmd.Parameters.AddWithValue("@id", s.StudentId);
            cmd.ExecuteNonQuery();
        }

        using (var del = new MySqlCommand("DELETE FROM Enrollments WHERE StudentId=@id", conn, tx))
        {
            del.Parameters.AddWithValue("@id", s.StudentId);
            del.ExecuteNonQuery();
        }

        InsertMarks(conn, tx, s.StudentId, marks, semesterId);
        tx.Commit();
    }

    // Enrollments are deleted automatically (ON DELETE CASCADE)
    public void Delete(int id)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand("DELETE FROM Students WHERE StudentId=@id", conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    private static void InsertMarks(MySqlConnection conn, MySqlTransaction tx,
                                    int studentId, Dictionary<int, double> marks, int semesterId)
    {
        foreach (var kv in marks)
        {
            using var cmd = new MySqlCommand(
                "INSERT INTO Enrollments(StudentId,CourseId,SemesterId,Mark) VALUES(@s,@c,@sem,@m)",
                conn, tx);
            cmd.Parameters.AddWithValue("@s", studentId);
            cmd.Parameters.AddWithValue("@c", kv.Key);
            cmd.Parameters.AddWithValue("@sem", semesterId);
            cmd.Parameters.AddWithValue("@m", kv.Value);
            cmd.ExecuteNonQuery();
        }
    }
}