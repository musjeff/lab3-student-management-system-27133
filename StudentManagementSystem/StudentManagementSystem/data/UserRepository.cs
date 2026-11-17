
using MySqlConnector;

namespace StudentTrack.Data;

public class UserRepository
{
    public bool UsernameExists(string username)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand("SELECT COUNT(*) FROM Users WHERE Username=@u", conn);
        cmd.Parameters.AddWithValue("@u", username);
        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
    }

    public int Insert(string username, string password)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(
            "INSERT INTO Users(Username,Password) VALUES(@u,@p)", conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        cmd.ExecuteNonQuery();
        return (int)cmd.LastInsertedId;
    }

    // Returns the UserId, or null if the login is wrong
    public int? CheckLogin(string username, string password)
    {
        using var conn = DatabaseHelper.GetConnection();
        conn.Open();
        using var cmd = new MySqlCommand(
            "SELECT UserId FROM Users WHERE Username=@u AND Password=@p", conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);
        object? result = cmd.ExecuteScalar();
        return result == null ? null : Convert.ToInt32(result);
    }
}