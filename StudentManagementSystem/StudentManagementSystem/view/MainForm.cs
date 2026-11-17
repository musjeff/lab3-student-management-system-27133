using System.Data;
using StudentTrack.Controllers;
using StudentTrack.Models;

namespace StudentTrack.Views;

public class MainForm : Form
{
    private readonly StudentController _ctrl = new();
    private readonly TextBox txtName = new(), txtAge = new(), txtEmail = new(), txtSearch = new();
    private readonly ComboBox cmbDept = new(), cmbSemester = new();
    private readonly List<TextBox> txtMarks = new();
    private readonly DataGridView grid = new();
    private readonly Label lblTotal = new(), lblAvg = new(), lblTop = new();
    private List<Lookup> _courses = new();
    private List<Student> _current = new();
    private int _selectedId = -1;
    private string _selectedName = "";

    public MainForm()
    {
        Text = $"StudentTrack - Student Management System  (user: {Session.Username})";
        ClientSize = new Size(1240, 700);
        StartPosition = FormStartPosition.CenterScreen;

        // ----- Right side: search + grid -----
        var lblSearch = new Label { Text = "Search (name or ID):", Left = 380, Top = 18, Width = 130 };
        txtSearch.SetBounds(515, 15, 300, 25);
        txtSearch.TextChanged += (s, e) => RefreshGrid();   // FR4.1: live filtering

        grid.SetBounds(380, 50, 840, 590);
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        grid.RowHeadersVisible = false;
        grid.CellClick += Grid_CellClick;

        // ----- Bottom: summary -----
        lblTotal.SetBounds(380, 655, 200, 25);
        lblAvg.SetBounds(590, 655, 220, 25);
        lblTop.SetBounds(820, 655, 400, 25);
        foreach (var l in new[] { lblTotal, lblAvg, lblTop })
            l.Font = new Font("Segoe UI", 10, FontStyle.Bold);

        Controls.AddRange(new Control[] { lblSearch, txtSearch, grid, lblTotal, lblAvg, lblTop });
        Load += MainForm_Load;
    }

    // The left panel is built here because the course list comes from the database
    private void MainForm_Load(object? sender, EventArgs e)
    {
        try
        {
            int y = 15;
            AddRow("Name", txtName, ref y);
            AddRow("Age", txtAge, ref y);
            AddRow("Email", txtEmail, ref y);

            SetupCombo(cmbDept, _ctrl.GetDepartments());
            AddRow("Department", cmbDept, ref y);
            SetupCombo(cmbSemester, _ctrl.GetSemesters());
            AddRow("Semester", cmbSemester, ref y);

            _courses = _ctrl.GetCourses();
            foreach (var c in _courses)
            {
                var t = new TextBox();
                txtMarks.Add(t);
                AddRow(c.Name + " (mark)", t, ref y);
            }

            y += 8;
            var btnAdd = MakeButton("Add", 15, y);
            var btnUpdate = MakeButton("Update", 95, y);
            var btnDelete = MakeButton("Delete", 175, y);
            var btnClear = MakeButton("Clear", 255, y);
            var btnAudit = MakeButton("Audit Log", 15, y + 40);
            btnAudit.Width = 155;
            btnAdd.Click += (s, ev) => DoAdd();
            btnUpdate.Click += (s, ev) => DoUpdate();
            btnDelete.Click += (s, ev) => DoDelete();
            btnClear.Click += (s, ev) => ClearFields();
            btnAudit.Click += (s, ev) => { using var f = new AuditForm(); f.ShowDialog(this); };
            Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete, btnClear, btnAudit });

            RefreshGrid();
        }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }

    private static void SetupCombo(ComboBox box, List<Lookup> items)
    {
        box.DropDownStyle = ComboBoxStyle.DropDownList;
        box.DisplayMember = "Name";
        box.ValueMember = "Id";
        box.DataSource = items;
    }

    private void AddRow(string label, Control box, ref int y)
    {
        Controls.Add(new Label { Text = label, Left = 15, Top = y + 3, Width = 175 });
        box.SetBounds(195, y, 165, 25);
        Controls.Add(box);
        y += 33;
    }

    private static Button MakeButton(string text, int x, int y)
        => new Button { Text = text, Left = x, Top = y, Width = 75, Height = 28 };

    private string[] MarkTexts() => txtMarks.Select(t => t.Text.Trim()).ToArray();
    private static int SelectedId(ComboBox box) => ((Lookup)box.SelectedItem!).Id;

    // FR2
    private void DoAdd()
    {
        try
        {
            string? err = _ctrl.Add(txtName.Text, txtAge.Text.Trim(), txtEmail.Text,
                                    SelectedId(cmbDept), SelectedId(cmbSemester), MarkTexts());
            if (err != null) { MessageBox.Show(err); return; }
            ClearFields();
            RefreshGrid();
        }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }

    // FR3.2 and FR3.3
    private void DoUpdate()
    {
        if (_selectedId < 0) { MessageBox.Show("Please select a student first."); return; }
        try
        {
            string? err = _ctrl.Update(_selectedId, txtName.Text, txtAge.Text.Trim(), txtEmail.Text,
                                       SelectedId(cmbDept), SelectedId(cmbSemester), MarkTexts());
            if (err != null) { MessageBox.Show(err); return; }
            ClearFields();
            RefreshGrid();
        }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }

    // FR3.4 and FR3.5
    private void DoDelete()
    {
        if (_selectedId < 0) { MessageBox.Show("Please select a student first."); return; }
        var answer = MessageBox.Show("Are you sure you want to delete this student?",
                                     "Confirm delete", MessageBoxButtons.YesNo);
        if (answer != DialogResult.Yes) return;
        try
        {
            _ctrl.Delete(_selectedId, _selectedName);
            ClearFields();
            RefreshGrid();
        }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }

    // FR3.1
    private void Grid_CellClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        try
        {
            int id = Convert.ToInt32(grid.Rows[e.RowIndex].Cells["ID"].Value);
            var s = _current.FirstOrDefault(x => x.StudentId == id);
            if (s == null) return;

            _selectedId = s.StudentId;
            _selectedName = s.Name;
            txtName.Text = s.Name;
            txtAge.Text = s.Age.ToString();
            txtEmail.Text = s.Email;
            cmbDept.SelectedValue = s.DepartmentId;

            var (marks, semesterId) = _ctrl.GetMarks(id);
            if (semesterId > 0) cmbSemester.SelectedValue = semesterId;
            for (int i = 0; i < _courses.Count; i++)
                txtMarks[i].Text = marks.TryGetValue(_courses[i].Id, out double m) ? m.ToString() : "";
        }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }

    private void ClearFields()
    {
        _selectedId = -1;
        _selectedName = "";
        txtName.Clear(); txtAge.Clear(); txtEmail.Clear();
        foreach (var t in txtMarks) t.Clear();
        if (cmbDept.Items.Count > 0) cmbDept.SelectedIndex = 0;
        if (cmbSemester.Items.Count > 0) cmbSemester.SelectedIndex = 0;
    }

    // FR2.7, FR3.6, FR4, FR5
    private void RefreshGrid()
    {
        try
        {
            _current = _ctrl.GetStudents(txtSearch.Text.Trim());

            var table = new DataTable();
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name");
            table.Columns.Add("Age", typeof(int));
            table.Columns.Add("Email");
            table.Columns.Add("Department");
            table.Columns.Add("GPA", typeof(double));
            foreach (var s in _current)
                table.Rows.Add(s.StudentId, s.Name, s.Age, s.Email, s.DepartmentName, s.Gpa);
            grid.DataSource = table;

            var sum = _ctrl.GetSummary(_current);
            lblTotal.Text = "Total students: " + sum.Total;
            lblAvg.Text = "Class average GPA: " + sum.AverageGpa;
            lblTop.Text = "Top student: " + sum.TopStudent;
        }
        catch (Exception ex) { MessageBox.Show("Database error: " + ex.Message); }
    }
}