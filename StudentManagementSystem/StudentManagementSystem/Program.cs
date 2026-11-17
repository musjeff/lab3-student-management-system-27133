using StudentManagementSystem;
using StudentTrack.Data;
using StudentTrack.Views;

namespace StudentTrack;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try { DatabaseHelper.TestConnection(); }
        catch (Exception ex)
        {
            MessageBox.Show("Cannot reach the database. Is MySQL running in XAMPP?\n\n" + ex.Message);
            return;
        }

        using (var login = new LoginForm())
        {
            if (login.ShowDialog() != DialogResult.OK) return;   // login is the first screen
        }
        Application.Run(new MainForm());
    }
}