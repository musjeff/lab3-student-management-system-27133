using StudentTrack.Controllers;

namespace StudentTrack.Views;

public class LoginForm : Form
{
    private readonly AuthController _auth = new();
    private readonly TextBox txtUser = new();
    private readonly TextBox txtPass = new();

    public LoginForm()
    {
        Text = "StudentTrack - Login";
        ClientSize = new Size(340, 220);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var title = new Label
        {
            Text = "StudentTrack Login",
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Left = 20,
            Top = 15,
            Width = 300,
            Height = 30
        };
        var lblU = new Label { Text = "Username", Left = 20, Top = 63, Width = 80 };
        var lblP = new Label { Text = "Password", Left = 20, Top = 98, Width = 80 };
        txtUser.SetBounds(110, 60, 200, 25);
        txtPass.SetBounds(110, 95, 200, 25);
        txtPass.UseSystemPasswordChar = true;

        var btnLogin = new Button { Text = "Login", Left = 110, Top = 130, Width = 90 };
        var link = new LinkLabel { Text = "Create an account", Left = 110, Top = 170, Width = 150 };

        btnLogin.Click += (s, e) => DoLogin();
        link.Click += (s, e) => { using var f = new RegisterForm(); f.ShowDialog(); };

        AcceptButton = btnLogin;
        Controls.AddRange(new Control[] { title, lblU, lblP, txtUser, txtPass, btnLogin, link });
    }

    private void DoLogin()
    {
        try
        {
            if (_auth.Login(txtUser.Text, txtPass.Text))
            {
                DialogResult = DialogResult.OK;   // closes this form, Program opens MainForm
                Close();
            }
            else
                MessageBox.Show("Invalid username or password");   // FR1.7
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database error: " + ex.Message);
        }
    }
}