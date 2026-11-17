namespace StudentTrack.Models;

public class Student
{
    public int StudentId { get; set; }
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string Email { get; set; } = "";
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = "";
    public double Gpa { get; set; }   // comes from the vw_StudentGPA view, never stored
}