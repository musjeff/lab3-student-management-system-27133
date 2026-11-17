
using MySqlConnector;

namespace StudentTrack.Data;

public static class DatabaseHelper
{
    // XAMPP default: user root, empty password.
    // If your root has a password, put it after Password=
    private const string ConnStr =
        "Server=localhost;Port=3306;Database=studenttracking;User ID=root;Password=;";

    public static MySqlConnection GetConnection() => new MySqlConnection(ConnStr);

    public static void TestConnection()
    {
        using var c = GetConnection();
        c.Open();
    }
}