using StudentTrack.Controllers;

namespace StudentTrack.Views;

public class AuditForm : Form
{
    public AuditForm()
    {
        Text = "Audit Log";
        ClientSize = new Size(800, 450);
        StartPosition = FormStartPosition.CenterParent;

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };
        Controls.Add(grid);

        try { grid.DataSource = new AuditController().GetLog(); }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }
}