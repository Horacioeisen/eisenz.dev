    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Data.OleDb;
    using System.Windows.Forms.DataVisualization.Charting;
    using Vonage;
    using Vonage.Messaging;
    using Vonage.Request;
using Vonage.Common;
using System.Diagnostics;



    namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
    {
    public partial class EmployeeForm : BaseForm

    {
        OleDbConnection myConn;
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        private Chart attendanceChart;
        private Chart earningsChart;
        private string loggedInUsername;
        private string employeeName;
        private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";
        public EmployeeForm(string empID, string fullname)
        {
            InitializeComponent();
            loggedInUsername = empID;
            employeeName = fullname;
            InitializeChart();
            timerDateToday.Start();
            lblEmpName.Text = $"Goodmorning, {employeeName}!";
            lblStatus.Text = $"{GetEmployeeStatus()}";
            ShowPanel(panelDashboard);
            LoadChartData();
        }
        private void InitializeChart()
        {
            InitializeAttendanceChart();
        }

        private void InitializeAttendanceChart()
        {
            attendanceChart = new Chart
            {
                Name = "chartAttendance",
                Size = new Size(1092, 326),
                Location = new Point(41, 416),
                BackColor = Color.WhiteSmoke,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };


            ChartArea chartArea = new ChartArea("MainChartArea")
            {
                AxisX = {
                        Title = "Day",
                        Interval = 1,
                        MajorGrid = { Enabled = false }
                    },
                AxisY = {
                        Title = "Hours Worked",
                        MajorGrid = { LineDashStyle = ChartDashStyle.Dot }
                    }
            };
            attendanceChart.ChartAreas.Add(chartArea);

            Series series = new Series("Hours Worked")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.SteelBlue,
                IsValueShownAsLabel = true,
                LabelFormat = "N1" // Show 1 decimal place
            };

            attendanceChart.Series.Add(series);

            Title chartTitle = new Title("Weekly Hours Worked")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            attendanceChart.Titles.Add(chartTitle);

            panelDashboard.Controls.Add(attendanceChart);
        }

        private void LoadChartData()
        {
            try
            {
                LoadAttendanceChartData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart data: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadAttendanceChartData()
        {
            try
            {
                if (string.IsNullOrEmpty(loggedInUsername))
                {
                    MessageBox.Show("Employee ID is missing", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime startDate = DateTime.Now.Date.AddDays(-6);
                DateTime endDate = DateTime.Now.Date;

                string query = @"
                        SELECT 
                            Format([Date], 'ddd') AS DayShort,
                            Sum([TotalHoursWorked]) AS Hours
                        FROM 
                            Attendance 
                        WHERE 
                            [EmployeeID] = ? AND [Date] BETWEEN ? AND ?
                        GROUP BY 
                            Format([Date], 'ddd'), 
                            Weekday([Date])
                        ORDER BY 
                            Weekday([Date])";

                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", loggedInUsername);
                    cmd.Parameters.AddWithValue("?", startDate);
                    cmd.Parameters.AddWithValue("?", endDate);

                    conn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        attendanceChart.Series[0].Points.Clear();

                        while (reader.Read())
                        {
                            string day = reader["DayShort"].ToString();
                            double hours = Convert.ToDouble(reader["Hours"]);
                            attendanceChart.Series[0].Points.AddXY(day, hours);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance data: {ex.Message}\n\n" +
                                $"Please verify your database connection and table structure.",
                                "Chart Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }


        private string GetEmployeeStatus()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT TimeIn, TimeOut FROM Attendance 
                               WHERE EmployeeID = ? AND Date = ?";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("?", loggedInUsername);
                        cmd.Parameters.AddWithValue("?", DateTime.Today);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DateTime timeIn = Convert.ToDateTime(reader["TimeIn"]);

                                if (timeIn.TimeOfDay > new TimeSpan(8, 0, 0))
                                {
                                    return "Late";
                                }
                                else
                                {
                                    return "On Time";
                                }
                            }
                            else
                            {
                                return "Not Checked In";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking status: " + ex.Message);
                return "Error";
            }
        }

        private void ShowPanel(Panel panelToShow)
        {
            panelDashboard.Visible = false;
            panelAttendanceHistory.Visible = false;
            panelFileLeave.Visible = false;
            panelPayroll.Visible = false;

            panelToShow.Visible = true;
            panelToShow.BringToFront();

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowPanel(panelDashboard);
        }
        private void LoadAttendanceHistory()
        {
            try
            {
                string query = "SELECT Date, TimeIn, TimeOut, Status, TotalHoursWorked, OverTimeWorked FROM Attendance WHERE EmployeeID = ?";

                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", loggedInUsername); // this is passed from LoginForm
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvAttendanceHistory.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading attendance history: " + ex.Message);
            }
        }
        private void btnAttendanceHistory_Click(object sender, EventArgs e)
        {
            LoadAttendanceHistory();
            ShowPanel(panelAttendanceHistory);
        }

        private void LoadPayslipHistory()
        {
            string employeeID = loggedInUsername; // from login session
            string query = "SELECT PayDate, GrossSalary, NetPay, PayslipLink FROM Payroll WHERE EmployeeID = ?";

            using (OleDbConnection conn = new OleDbConnection(connString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("?", employeeID);
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                dgvPayslips.DataSource = table;
            }
        }
        private void btnPayroll_Click(object sender, EventArgs e)
        {
            LoadPayslipHistory();
            ShowPanel(panelPayroll);
        }
        private void LoadLeavesHistory()
        {
            try
            {
                string query = "SELECT LeaveType, StartDate, EndDate, Reason, Status FROM Leaves WHERE EmployeeID = ?";
                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", loggedInUsername); // This is passed from LoginForm
                    OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvLeavesHistory.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading leaves history: " + ex.Message);
            }
        }
        private void btnFileLeave_Click(object sender, EventArgs e)
        {
            LoadLeavesHistory();
            ShowPanel(panelFileLeave);
        }

        private void btnNotification_Click(object sender, EventArgs e)
        {

        }

        private void btnLogoutEmployee_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                LoginForm Login = new LoginForm();
                Login.Show();
                this.Hide();
            }
        }

        private void btnSubmitLeave_Click(object sender, EventArgs e)
        {
            string leaveType = cmbxLeaveType.SelectedItem?.ToString();
            DateTime startDate = dtpStartDate.Value.Date;
            DateTime endDate = dtpEndDate.Value.Date;
            string reason = txtBReason.Text.Trim();

            if (string.IsNullOrEmpty(leaveType))
            {
                MessageBox.Show("Please select a leave type.");
                return;
            }

            if (endDate < startDate)
            {
                MessageBox.Show("End date cannot be earlier than start date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(reason))
            {
                MessageBox.Show("Please enter a reason for leave.");
                return;
            }

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    string insertQuery = "INSERT INTO Leaves (EmployeeID, LeaveType, StartDate, EndDate, Reason, Status) " +
                                         "VALUES (?, ?, ?, ?, ?, 'Pending')";

                    using (OleDbCommand cmd = new OleDbCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("?", loggedInUsername);      // From login
                        cmd.Parameters.AddWithValue("?", leaveType);
                        cmd.Parameters.AddWithValue("?", startDate);
                        cmd.Parameters.AddWithValue("?", endDate);
                        cmd.Parameters.AddWithValue("?", reason);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Leave request submitted and is pending HR approval.");
                    }
                }

                cmbxLeaveType.SelectedIndex = -1;
                txtBReason.Clear();
                dtpStartDate.Value = DateTime.Now;
                dtpEndDate.Value = DateTime.Now;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error submitting leave request: " + ex.Message);
            }
        }



        private void timerDateToday_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString("dddd, yyyy-MM-dd");
        }

        private void btnRefreshPayslips_Click(object sender, EventArgs e)
        {
            LoadPayslipHistory();
        }

        private void dgvPayslips_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0) return;

            try
            {
                string fileName = dgvPayslips.Rows[e.RowIndex].Cells["PayslipLink"].Value?.ToString();

                if (string.IsNullOrEmpty(fileName))
                {
                    MessageBox.Show("No payslip file linked for this record.");
                    return;
                }

                // Get the payslip folder path (same as generation path)
                string folderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "EmployeePayslips");

                string fullPath = Path.Combine(folderPath, fileName);

                // Verify file exists
                if (!File.Exists(fullPath))
                {
                    MessageBox.Show($"Payslip not found at:\n{fullPath}\n\n" +
                                  $"Please check:\n" +
                                  $"1. The file exists in the folder\n" +
                                  $"2. The filename matches exactly\n" +
                                  $"3. You have permission to access the file",
                                  "File Not Found",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                    return;
                }

                try //open pdf
                {
                    var processInfo = new ProcessStartInfo(fullPath)
                    {
                        UseShellExecute = true 
                    };
                    Process.Start(processInfo);
                }
                catch (Win32Exception winEx)
                {
                    MessageBox.Show($"Failed to open PDF. You may need a PDF reader installed.\n\nError: {winEx.Message}",
                                   "Opening Failed",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error accessing payslip: {ex.Message}",
                               "Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }
    }
}
