using System;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using System.Data;

namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    public partial class LoginForm : BaseForm
    {
        OleDbConnection? myConn;
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        private int cornerRadius = 30;
        private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";
        public LoginForm()
        {
            InitializeComponent();
            this.Paint += LoginForm_Paint;
            this.MouseDown += LoginForm_MouseDown;
            CustomizePanel();
        }
        private void LoginForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GetRoundedRectanglePath(this.ClientRectangle, cornerRadius))
            {
                this.Region = new Region(path);
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
        }

        private void CustomizePanel()
        {
            panelLoginAcc.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectanglePath(panelLoginAcc.ClientRectangle, 30))
                {
                    panelLoginAcc.Region = new Region(path);
                    using (SolidBrush brush = new SolidBrush(panelLoginAcc.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };
        }
        private GraphicsPath GetRoundedRectanglePath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void LoginForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }
        private void chckBxLoginShowPass_CheckedChanged(object sender, EventArgs e)
        {
            txtboxloginPassword.PasswordChar = chckBxLoginShowPass.Checked ? '\0' : '*';
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxUsername.Text) || string.IsNullOrEmpty(txtboxloginPassword.Text))
            {
                MessageBox.Show("Please fill all the fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT UserType, [password], EmployeeID, FullName FROM Users WHERE Username = @Username";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", txtbxUsername.Text);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["password"].ToString();
                                string inputPassword = txtboxloginPassword.Text;

                                if (storedPassword == inputPassword)
                                {
                                    string role = reader["UserType"].ToString();
                                    MessageBox.Show("Login Successful", "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    if (role == "admin")
                                    {
                                        adminForm Main = new adminForm();
                                        Main.Show();
                                    }
                                    else if (role == "hr")
                                    {
                                        string empID = reader["EmployeeID"].ToString();
                                        string fullname = reader["FullName"].ToString();
                                        HRForm HR = new HRForm(empID, fullname);
                                        HR.Show();
                                    }
                                    else if (role == "employee")
                                    {
                                        string empID = reader["EmployeeID"].ToString();
                                        string fullName = reader["FullName"].ToString(); // using the fullname from the users table
                                        EmployeeForm Emp = new EmployeeForm(empID, fullName);
                                        Emp.Show();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Unknown role assigned", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        return;
                                    }

                                    this.Hide();
                                }
                                else
                                {
                                    MessageBox.Show("Invalid password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else
                            {
                                MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}\n{ex.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnAttendancePortal_Click(object sender, EventArgs e)
        {
            AttendancePortal attendancePortal = new AttendancePortal();
            attendancePortal.Show();
            this.Hide();
        }
    }
}
