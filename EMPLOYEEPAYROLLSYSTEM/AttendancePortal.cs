using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Data.OleDb;

namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    public partial class AttendancePortal : BaseForm
    {
        OleDbConnection myConn;
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        private int cornerRadius = 30;
        private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";

        public AttendancePortal()
        {
            InitializeComponent();
            this.Paint += AttendancePortal_Paint;
            this.MouseDown += AttendancePortal_MouseDown;
            CustomizePanel();
        }
        private void timerAttendance_Tick(object sender, EventArgs e)
        {
            lblDatetime.Text = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss");
        }

        private void btnBcktoLogin_Click(object sender, EventArgs e)
        {
            LoginForm Login = new LoginForm();
            Login.Show();
            this.Hide();
        }

        private void btnScanID_Click(object sender, EventArgs e)
        {
            string empID = txtbEmpID.Text.Trim();
            if (string.IsNullOrEmpty(empID))
            {
                MessageBox.Show("Please enter your Employee ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (OleDbConnection connection = new OleDbConnection(connString))
            {
                try
                {
                    connection.Open();

                    DateTime currentTime = DateTime.Now;
                    string dateToday = currentTime.ToString("yyyy-MM-dd");
                    string status = (currentTime.TimeOfDay <= new TimeSpan(8, 0, 0)) ? "On Time" : "Late";

                    //to get the employee name
                    string getNameQuery = "SELECT EmployeeName FROM Employees WHERE EmployeeID = ?";
                    OleDbCommand getNameCmd = new OleDbCommand(getNameQuery, connection);
                    getNameCmd.Parameters.AddWithValue("?", empID);
                    object result = getNameCmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Employee not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string employeeName = result.ToString();
                    lblEmpName.Text = "Name: " + employeeName;

                    //check if already has TimeIn but no TimeOut today
                    string checkQuery = "SELECT TimeIn FROM Attendance WHERE EmployeeID = ? AND [Date] = ? AND TimeOut IS NULL";
                    OleDbCommand checkCmd = new OleDbCommand(checkQuery, connection);
                    checkCmd.Parameters.AddWithValue("?", empID);
                    checkCmd.Parameters.AddWithValue("?", dateToday);
                    object timeInObj = checkCmd.ExecuteScalar();

                    if (timeInObj != null && DateTime.TryParse(timeInObj.ToString(), out DateTime timeIn))
                    {
                        //TimeIn exists and no TimeOut → update TimeOut and compute hours
                        TimeSpan totalWorked = currentTime - timeIn;
                        double totalHours = Math.Round(totalWorked.TotalHours, 2);

                        TimeSpan fivePM = new TimeSpan(17, 0, 0);
                        double overtimeWorked = 0;

                        if (currentTime.TimeOfDay > fivePM)
                        {
                            overtimeWorked = Math.Round((currentTime.TimeOfDay - fivePM).TotalHours, 2);
                        }
                       

                        string updateQuery = "UPDATE Attendance SET TimeOut = ?, [Status] = ?, TotalHoursWorked = ?, OverTimeWorked = ? WHERE EmployeeID = ? AND [Date] = ? AND TimeOut IS NULL";
                        OleDbCommand updateCmd = new OleDbCommand(updateQuery, connection);
                        updateCmd.Parameters.AddWithValue("?", currentTime.ToString("HH:mm:ss"));
                        updateCmd.Parameters.AddWithValue("?", status);
                        updateCmd.Parameters.AddWithValue("?", totalHours);
                        updateCmd.Parameters.AddWithValue("?", overtimeWorked);
                        updateCmd.Parameters.AddWithValue("?", empID);
                        updateCmd.Parameters.AddWithValue("?", dateToday);
                        updateCmd.ExecuteNonQuery();

                        lblTimeIn.Text = "Time In: " + timeIn.ToString("hh:mm tt");
                        lblTimeOut.Text = "Time Out: " + currentTime.ToString("hh:mm tt");
                        lblStatus.Text = "Status: " + status;
                        lblOvertime.Text = $"Overtime: {overtimeWorked} hrs";
                        MessageBox.Show($"Time Out recorded.\nTotal Hours Worked: {totalHours} hrs", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        //insert new record
                        string insertQuery = "INSERT INTO Attendance (EmployeeID, EmployeeName, [Date], TimeIn, [Status]) VALUES (?, ?, ?, ?, ?)";
                        OleDbCommand insertCmd = new OleDbCommand(insertQuery, connection);
                        insertCmd.Parameters.AddWithValue("?", empID);
                        insertCmd.Parameters.AddWithValue("?", employeeName);
                        insertCmd.Parameters.AddWithValue("?", dateToday);
                        insertCmd.Parameters.AddWithValue("?", currentTime.ToString("HH:mm:ss"));
                        insertCmd.Parameters.AddWithValue("?", status);
                        insertCmd.ExecuteNonQuery();

                        lblTimeIn.Text = "Time In: " + currentTime.ToString("hh:mm tt");
                        lblTimeOut.Text = "Time Out: Not yet timed out";
                        lblStatus.Text = "Status: " + status;
                        MessageBox.Show($"Time In recorded at {currentTime.ToShortTimeString()}.\nStatus: {status}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AttendancePortal_Paint(object sender, PaintEventArgs e)
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

        private void CustomizePanel()
        {
            panelAttendancePortal.Paint += (sender, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectanglePath(panelAttendancePortal.ClientRectangle, 30))
                {
                    panelAttendancePortal.Region = new Region(path);
                    using (SolidBrush brush = new SolidBrush(panelAttendancePortal.BackColor))
                    {
                        e.Graphics.FillPath(brush, path);
                    }
                }
            };
        }
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        private void AttendancePortal_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        private void Timeout_Click(object sender, EventArgs e)
        {

        }
    }

    public class UISyntax : Form
    {

    }
}
