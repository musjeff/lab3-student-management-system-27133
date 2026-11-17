using StudentTrack.Data;
using StudentTrack.Models;

namespace StudentTrack.Controllers;

public record Summary(int Total, string AverageGpa, string TopStudent);

public class StudentController
{
    private readonly StudentRepository _students = new();
    private readonly LookupRepository _lookups = new();
    private readonly AuditRepository _audit = new();
    private List<Lookup>? _courses;

    public List<Lookup> GetDepartments() => _lookups.GetDepartments();
    public List<Lookup> GetSemesters() => _lookups.GetSemesters();
    public List<Lookup> GetCourses() => _courses ??= _lookups.GetCourses();

    public List<Student> GetStudents(string search) => _students.GetAll(search);

    public (Dictionary<int, double> Marks, int SemesterId) GetMarks(int studentId)
        => _students.GetMarks(studentId);

    // Returns null if OK, otherwise the error message (FR2.6)
    public string? Add(string name, string age, string email, int deptId, int semId, string[] markTexts)
    {
        string? err = Validate(name, age, email, markTexts, out Student? s, out var marks);
        if (err != null) return err;

        s!.DepartmentId = deptId;
        int id = _students.Insert(s, marks!, semId);
        _audit.Log(Session.UserId, "ADD_STUDENT", $"Added {s.Name} (ID {id})");
        return null;
    }

    public string? Update(int id, string name, string age, string email, int deptId, int semId, string[] markTexts)
    {
        if (id < 0) return "Please select a student first.";
        string? err = Validate(name, age, email, markTexts, out Student? s, out var marks);
        if (err != null) return err;

        s!.StudentId = id;
        s.DepartmentId = deptId;
        _students.Update(s, marks!, semId);
        _audit.Log(Session.UserId, "UPDATE_STUDENT", $"Updated {s.Name} (ID {id})");
        return null;
    }

    public void Delete(int id, string name)
    {
        _students.Delete(id);
        _audit.Log(Session.UserId, "DELETE_STUDENT", $"Deleted {name} (ID {id})");
    }

    // FR5.3 and FR5.5
    public Summary GetSummary(List<Student> students)
    {
        if (students.Count == 0) return new Summary(0, "N/A", "N/A");
        double avg = Math.Round(students.Average(s => s.Gpa), 2);
        var top = students.OrderByDescending(s => s.Gpa).First();
        return new Summary(students.Count, avg.ToString("0.00"), $"{top.Name} ({top.Gpa:0.00})");
    }

    // FR2.2 to FR2.5
    private string? Validate(string name, string ageText, string email, string[] markTexts,
                             out Student? student, out Dictionary<int, double>? marks)
    {
        student = null;
        marks = null;

        if (string.IsNullOrWhiteSpace(name)) return "Name cannot be empty.";
        if (!int.TryParse(ageText, out int age) || age < 5 || age > 100)
            return "Age must be a whole number between 5 and 100.";
        if (!email.Contains('@') || !email.Contains('.'))
            return "Email must contain '@' and '.'.";

        var courses = GetCourses();
        var result = new Dictionary<int, double>();
        for (int i = 0; i < courses.Count; i++)
        {
            if (!double.TryParse(markTexts[i], out double m) || m < 0 || m > 100)
                return $"{courses[i].Name} mark must be a number between 0 and 100.";
            result[courses[i].Id] = m;
        }

        student = new Student { Name = name.Trim(), Age = age, Email = email.Trim() };
        marks = result;
        return null;
    }
}