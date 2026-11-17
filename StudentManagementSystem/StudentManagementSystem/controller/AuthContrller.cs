using StudentTrack.Data;

namespace StudentTrack.Controllers;

public class AuthController
{
    private readonly UserRepository _users = new();
    private readonly AuditRepository _audit = new();

    // Returns null if OK, otherwise the error message (FR1.2, FR1.3)
    public string? Register(string username, string password, string confirm)
    {
        username = username.Trim();
        if (username == "") return "Username cannot be empty.";
        if (_users.UsernameExists(username)) return "Username already exists.";
        if (password.Length < 3) return "Password must be at least 3 characters.";
        if (password != confirm) return "Password and confirm password do not match.";

        int id = _users.Insert(username, password);
        _audit.Log(id, "REGISTER", $"New account: {username}");
        return null;
    }

    public bool Login(string username, string password)
    {
        username = username.Trim();
        int? id = _users.CheckLogin(username, password);
        if (id == null) return false;

        Session.UserId = id;
        Session.Username = username;
        _audit.Log(id, "LOGIN", $"{username} logged in");
        return true;
    }
}