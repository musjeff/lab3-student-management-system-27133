using StudentTrack.Controllers;

namespace StudentTrack.Views;

public class RegisterForm : Form
{
    private readonly AuthController _auth = new();
    private readonly TextBox txtUser = new();
    private readonly TextBox txtPass = new();
    private readonly TextBox txtConfirm = new();

    public RegisterForm()
    {
        Text = "StudentTrack - Register";
        ClientSize = new Size(360, 210);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var lblU = new Label { Text = "Username", Left = 20, Top = 28, Width = 115 };
        var lblP = new Label { Text = "Password", Left = 20, Top = 68, Width = 115 };
        var lblC = new Label { Text = "Confirm password", Left = 20, Top = 108, Width = 115 };
        txtUser.SetBounds(140, 25, 190, 25);
        txtPass.SetBounds(140, 65, 190, 25);
        txtConfirm.SetBounds(140, 105, 190, 25);
        txtPass.UseSystemPasswordChar = true;
        txtConfirm.UseSystemPasswordChar = true;

        var btn = new Button { Text = "Register", Left = 140, Top = 150, Width = 90 };
        btn.Click += (s, e) => DoRegister();
        AcceptButton = btn;

        Controls.AddRange(new Control[] { lblU, lblP, lblC, txtUser, txtPass, txtConfirm, btn });
    }

    private void DoRegister()
    {
        try
        {
            string? err = _auth.Register(txtUser.Text, txtPass.Text, txtConfirm.Text);
            if (err != null) { MessageBox.Show(err); return; }
            MessageBox.Show("Account created successfully.");   // FR1.4
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Database error: " + ex.Message);
        }
    }
}