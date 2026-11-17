namespace StudentTrack.Models;

public class Lookup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public override string ToString() => Name;
}