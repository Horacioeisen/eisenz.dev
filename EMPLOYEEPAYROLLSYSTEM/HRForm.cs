using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using Vonage;
using Vonage.Messaging;
using Vonage.Request;
using Vonage.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.IO.Abstractions;
using System.Net;
using System.Net.Mail;
using MimeKit;
using MailKit.Net.Smtp;



namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    public partial class HRForm : BaseForm
    {
        OleDbConnection myConn;
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        private Chart attendanceChart; //get weekly totalhoursworked
        private Chart attendanceStatsChart; //get total counts of present&lates
        private bool chartInitialized = false;
        private string loggedInUsername;
        private string employeeName;
        private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";

        public HRForm(string empID, string fullname)
        {
            InitializeComponent();
            loggedInUsername = fullname;
            employeeName = empID;
            myConn = new OleDbConnection(connString);
            LoadDashboardData();
            timerRealTimeCount.Start();
            timer1CurrentTime.Start();
            lblHRName.Text = $"Goodmorning, {fullname}!";
            ShowPanel(panelDashboard);
            this.Controls.Add(panelAttendance);
            this.Controls.Add(panelEmployees);
            this.Controls.Add(panelSalary);
            this.Controls.Add(panelLeaves);
            this.Controls.Add(panelSettings);

        }

        private void ShowPanel(Panel panelToShow)
        {
            panelDashboard.Visible = false;
            panelAttendance.Visible = false;
            panelEmployees.Visible = false;
            panelSalary.Visible = false;
            panelSettings.Visible = false;
            panelLeaves.Visible = false;

            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }
        private void LoadDashboardData()
        {
            lblTotEmployees.Text = GetCount("SELECT COUNT(*) FROM Employees").ToString();
            lblPresentToday.Text = GetCount($"SELECT COUNT(*) FROM Attendance WHERE Date = #{DateTime.Now.ToShortDateString()}# AND TimeIn < #08:00:00 AM#").ToString();
            lblPresentToday.Text = GetCount($"SELECT COUNT(*) FROM Attendance WHERE Status = 'On Time'").ToString();
            //lblLates.Text = GetCount($"SELECT COUNT(*) FROM Attendance WHERE Status = 'Late'").ToString();
            lblLates.Text = GetCount($"SELECT COUNT(*) FROM Attendance WHERE Date = #{DateTime.Now.ToShortDateString()}# AND TimeIn > #08:00:00 AM#").ToString();
            lblOnLeave.Text = GetCount("SELECT COUNT(*) FROM Leaves WHERE Status = 'Approved'").ToString();


        }
        private void LoadAttendance()
        {
            string query = "SELECT AttendanceID, EmployeeID, EmployeeName, Date, TimeIn, TimeOut, Status, TotalHoursWorked, OverTimeWorked FROM Attendance";
            LoadData(query, dgvAttendanceHistory);
        }

        private void LoadPersonalData()
        {
            string query = "SELECT * FROM PersonalData";
            LoadData(query, dgvEmployees);
        }

        private void LoadEmployees()
        {
            string query = "SELECT * FROM Employees";

            DataTable dt = _databaseService.ExecuteQuery(query);

            dgvEmployees.DataSource = null;
            dgvEmployees.Rows.Clear();
            dgvEmployees.DataSource = dt;
            dgvEmployees.Refresh();
        }

        private void LoadPayRoll()
        {
            string query = "SELECT EmployeeID, EmployeeName, TotalMonthlyHoursWorked,MonthlySalary, HourlyRate, OverTimeWorked, OverTimeRate, GrossSalary, SSS, PhilHealth, PagIbig, TotalDeductions, Bonuses, NetPay, PayDate, PayslipLink FROM Payroll"; 
            LoadData(query, dgvPayroll);
        }
        private bool SearchAttendance(string keyword)
        {
            string query = "SELECT a.AttendanceID, a.EmployeeID, e.EmployeeName, a.Date, a.TimeIn, a.TimeOut, a.Status, a.TotalHoursWorked, a.OverTimeWorked " +
                      "FROM Attendance a " +
                      "LEFT JOIN Employees e ON a.EmployeeID = e.EmployeeID " +
                      "WHERE a.EmployeeID LIKE ? OR e.EmployeeName LIKE ?";

            List<OleDbParameter> parameters = new List<OleDbParameter>
        {
            new OleDbParameter("?", "%" + keyword.Trim() + "%"),
            new OleDbParameter("?", "%" + keyword.Trim() + "%")
        };

            DataTable dt = _databaseService.ExecuteQuery(query, parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvAttendanceHistory.DataSource = dt;
                return true;
            }
            else
            {
                dgvAttendanceHistory.DataSource = null;
                return false;
            }
        }
        private bool SearchPersonalData(string keyword)
        {
            try
            {
                string query = "SELECT a.PersonalDataID, a.EmployeeID, a.LastName, a.FirstName, a.MiddleName, a.DateOfBirth, a.Gender, a.Address, a.PhoneNumber, a.Email, a.EmergencyContact, a.SSSNumber, a.PagIbigNumber, a.PhilHealthNumber " +
                      "FROM PersonalData a " +
                      "LEFT JOIN Employees e ON a.EmployeeID = e.EmployeeID " +
                      "WHERE a.EmployeeID LIKE ? OR e.EmployeeName LIKE ?";

                List<OleDbParameter> parameters = new List<OleDbParameter>
        {
            new OleDbParameter("?", "%" + txtboxSearchEmployee.Text.Trim() + "%"),
            new OleDbParameter("?", "%" + txtboxSearchEmployee.Text.Trim() + "%")
        };

                DataTable dt = _databaseService.ExecuteQuery(query, parameters);

                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvEmployees.DataSource = dt;
                    return true;
                }

                dgvEmployees.DataSource = null;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching personal data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool SearchLeaves(string keyword)
        {
            string query = "SELECT * FROM Leaves WHERE EmployeeID LIKE ? OR LeaveType LIKE ?";
            List<OleDbParameter> parameters = new List<OleDbParameter>
        {
            new OleDbParameter("?", "%" + keyword.Trim() + "%"),
            new OleDbParameter("?", "%" + keyword.Trim() + "%")
        };
            DataTable dt = _databaseService.ExecuteQuery(query, parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvLeavesRequests.DataSource = dt;
                return true;
            }
            else
            {
                dgvLeavesRequests.DataSource = null;
                return false;
            }
        }

        private bool SearchPayroll(string keyword)
        {
            string query = "SELECT * FROM Payroll WHERE EmployeeID LIKE ?";
            List <OleDbParameter> parameters = new List<OleDbParameter>
        {
            new OleDbParameter("?", "%" + keyword.Trim() + "%"),
            new OleDbParameter("?", "%" + keyword.Trim() + "%")
        };
            DataTable dt = _databaseService.ExecuteQuery(query, parameters);
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvPayroll.DataSource = dt; 
                return true;
            }
            else
            {
                dgvPayroll.DataSource = null;
                return false;
            }

        }
        private void timer1CurrentTime_Tick(object sender, EventArgs e)
        {
            labelTime.Text = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss");
            lblCurrentDate.Text = DateTime.Now.ToString("dddd, yyyy-MM-dd");
        }
        private void btnDashboard_Click(object sender, EventArgs e) => ShowPanel(panelDashboard);

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            ShowPanel(panelAttendance);
        }
        private void btnEmployees_Click(object sender, EventArgs e) => ShowPanel(panelEmployees);
        private void btnSalaries_Click(object sender, EventArgs e) => ShowPanel(panelSalary);
        private void btnLeaves_Click(object sender, EventArgs e) => ShowPanel(panelLeaves);

        private void btnSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = !panelSettings.Visible;
            panelSettings.BringToFront();
        }

        private void btnLogoutHR_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                LoginForm Login = new LoginForm();
                Login.Show();
                this.Hide();
            }

        }

        private void picBCloseSettings_Click(object sender, EventArgs e)
        {
            panelSettings.Visible = false;
        }
        private void btnConnectionTest_Click(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection myConn = new OleDbConnection(connString))
                {
                    myConn.Open();
                    MessageBox.Show("Connection Successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void timerRealTimeCount_Tick(object sender, EventArgs e)
        {
            LoadDashboardData();
        }

        private void btnSearchAttendancee_Click(object sender, EventArgs e)
        {
            string keyword = txtboxSearchAttendance.Text.Trim();
            MessageBox.Show("Searching for: [" + keyword + "]", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!string.IsNullOrEmpty(keyword))
            {
                bool recordsFound = SearchAttendance(keyword);
                if (!recordsFound)
                {
                    MessageBox.Show("No records found for the given Employee ID or Name.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAttendance();
                }
            }
            else
            {
                MessageBox.Show("Please enter an Employee ID or Name.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLoadAttendance_Click(object sender, EventArgs e)
        {
            LoadAttendance();
        }

        private void btnLoadEmployees_Click(object sender, EventArgs e)
        {
            LoadEmployees();
        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            EmployeeManagement employeeManagement = new EmployeeManagement();
            employeeManagement.ShowDialog();
        }

        private void btnVieworUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvEmployees.SelectedRows[0];

                if (selectedRow.Cells["EmployeeID"].Value == null || selectedRow.Cells["EmployeeID"].Value == DBNull.Value || string.IsNullOrWhiteSpace(selectedRow.Cells["EmployeeID"].Value.ToString()))
                {
                    MessageBox.Show("Selected row is empty. Please select a valid employee.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string employeeID = selectedRow.Cells["EmployeeID"].Value.ToString();
                string employeeName = selectedRow.Cells["EmployeeName"].Value.ToString();
                string department = selectedRow.Cells["Department"].Value.ToString();
                string position = selectedRow.Cells["Position"].Value.ToString();
                string experiencelevel = selectedRow.Cells["ExperienceLevel"].Value.ToString();
                decimal monthlySalary = Convert.ToDecimal(selectedRow.Cells["MonthlySalary"].Value);
                decimal hourlyRate = Convert.ToDecimal(selectedRow.Cells["HourlyRate"].Value); // assuming HourlyRate exists in the DataGridView

                EmployeeManagement employeeManagement = new EmployeeManagement(employeeID, employeeName, department, position, experiencelevel, monthlySalary, hourlyRate);
                employeeManagement.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select an employee to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void DeleteEmployee(string employeeID)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    using (OleDbTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            Console.WriteLine($"Starting deletion for employee ID: {employeeID}");

                            string[] deleteQueries = {
                        "DELETE FROM Users WHERE EmployeeID = ?",
                        "DELETE FROM Payroll WHERE EmployeeID = ?",
                        "DELETE FROM Leaves WHERE EmployeeID = ?",
                        "DELETE FROM Attendance WHERE EmployeeID = ?",
                        "DELETE FROM PersonalData WHERE EmployeeID = ?"
                    };

                            foreach (string query in deleteQueries)
                            {
                                using (OleDbCommand cmd = new OleDbCommand(query, conn, transaction))
                                {
                                    cmd.Parameters.Add(new OleDbParameter("@EmployeeID", OleDbType.VarChar) { Value = employeeID });
                                    int rowsAffected = cmd.ExecuteNonQuery();
                                    Console.WriteLine($"{query} - Rows affected: {rowsAffected}");
                                }
                            }

                            string deleteEmployeeQuery = "DELETE FROM Employees WHERE EmployeeID = ?";
                            using (OleDbCommand cmdEmployee = new OleDbCommand(deleteEmployeeQuery, conn, transaction))
                            {
                                cmdEmployee.Parameters.Add(new OleDbParameter("@EmployeeID", OleDbType.VarChar) { Value = employeeID });
                                int employeeRows = cmdEmployee.ExecuteNonQuery();

                                if (employeeRows == 0)
                                {
                                    MessageBox.Show("Employee not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    transaction.Rollback();
                                    return;
                                }
                                Console.WriteLine($"Employee deleted - Rows affected: {employeeRows}");
                            }

                            transaction.Commit();
                            MessageBox.Show("Employee's data deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadEmployees();
                        }
                        catch (OleDbException dbEx)
                        {
                            Console.WriteLine($"Database error during deletion: {dbEx.ToString()}");
                            transaction.Rollback();

                            if (dbEx.ErrorCode == -2147467259) // Common error code for referential integrity
                            {
                                MessageBox.Show("Cannot delete employee because related records still exist in other tables.",
                                    "Referential Integrity Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show($"Database error deleting employee: {dbEx.Message}",
                                    "Error",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"General error during deletion: {ex.ToString()}");
                            transaction.Rollback();
                            MessageBox.Show($"Error deleting employee: {ex.Message}",
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database connection error: {ex.ToString()}");
                MessageBox.Show($"Database connection error: {ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvEmployees.SelectedRows[0];

                var employeeIDCellValue = selectedRow.Cells["EmployeeID"].Value;

                if (employeeIDCellValue == null || employeeIDCellValue == DBNull.Value)
                {
                    MessageBox.Show("Invalid employee selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string employeeID = employeeIDCellValue.ToString();

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to permanently delete this employee and ALL their related data?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DeleteEmployee(employeeID);
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ClearPersonalDataView()
        {
            dgvEmployees.DataSource = null;
            dgvEmployees.Rows.Clear();
            txtboxSearchEmployee.Text = string.Empty;
        }

        private void btnSearchPersonalData_Click(object sender, EventArgs e)
        {
            string keyword = txtboxSearchEmployee.Text.Trim();
            MessageBox.Show("Searching for: [" + keyword + "]", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!string.IsNullOrEmpty(keyword))
            {
                bool recordsFound = SearchPersonalData(keyword);
                if (!recordsFound)
                {
                    MessageBox.Show("No records found for the given Employee ID or Name.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {
                MessageBox.Show("Please enter an Employee ID or Name.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void panelEmployees_Paint(object sender, PaintEventArgs e)
        {

        }

        private void HRForm_Load(object sender, EventArgs e)
        {

        }

        private void btnLoadRequest_Click(object sender, EventArgs e)
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    string query = "SELECT EmployeeID, LeaveType, StartDate, EndDate, Reason, Status FROM Leaves";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);

                        dgvLeavesRequests.DataSource = dt;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading leave requests: " + ex.Message);
            }
        }

        private void btnApproveLeave_Click(object sender, EventArgs e)
        {
            if (dgvLeavesRequests.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvLeavesRequests.SelectedRows[0];

                string employeeID = selectedRow.Cells["EmployeeID"].Value.ToString();
                string leaveType = selectedRow.Cells["LeaveType"].Value.ToString();
                DateTime startDate = Convert.ToDateTime(selectedRow.Cells["StartDate"].Value);
                DateTime endDate = Convert.ToDateTime(selectedRow.Cells["EndDate"].Value);
                string currentStatus = selectedRow.Cells["Status"].Value.ToString();


                if (currentStatus == "Approved" || currentStatus == "Rejected")
                {
                    MessageBox.Show("This leave request has already been " + currentStatus.ToLower() + ".");
                    return;
                }

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        conn.Open();

                        string query = "UPDATE Leaves SET Status = 'Approved' " +
                                       "WHERE EmployeeID = ? AND LeaveType = ? AND StartDate = ? AND EndDate = ?";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("?", employeeID);
                            cmd.Parameters.AddWithValue("?", leaveType);
                            cmd.Parameters.AddWithValue("?", startDate);
                            cmd.Parameters.AddWithValue("?", endDate);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Leave request approved.");
                                btnLoadRequest_Click(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("No matching record found to approve.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error approving leave request: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a leave request to approve.");
            }
        }

        private void btnRejectLeave_Click(object sender, EventArgs e)
        {
            if (dgvLeavesRequests.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvLeavesRequests.SelectedRows[0];

                string employeeID = selectedRow.Cells["EmployeeID"].Value.ToString();
                string leaveType = selectedRow.Cells["LeaveType"].Value.ToString();
                DateTime startDate = Convert.ToDateTime(selectedRow.Cells["StartDate"].Value);
                DateTime endDate = Convert.ToDateTime(selectedRow.Cells["EndDate"].Value);
                string currentStatus = selectedRow.Cells["Status"].Value.ToString();

                if (currentStatus == "Approved" || currentStatus == "Rejected")
                {
                    MessageBox.Show("This leave request has already been " + currentStatus.ToLower() + ".");
                    return;
                }

                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        conn.Open();

                        string query = "UPDATE Leaves SET Status = 'Rejected' " +
                                       "WHERE EmployeeID = ? AND LeaveType = ? AND StartDate = ? AND EndDate = ?";

                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("?", employeeID);
                            cmd.Parameters.AddWithValue("?", leaveType);
                            cmd.Parameters.AddWithValue("?", startDate);
                            cmd.Parameters.AddWithValue("?", endDate);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Leave request approved.");
                                btnLoadRequest_Click(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("No matching record found to approve.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error approving leave request: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a leave request to approve.");
            }
        }

        private void btnSearchLeave_Click(object sender, EventArgs e)
        {
            string keyword = txtbSearchLeaves.Text.Trim();
            MessageBox.Show("Searching for: [" + keyword + "]", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!string.IsNullOrEmpty(keyword))
            {
                bool recordsFound = SearchLeaves(keyword);
                if (!recordsFound)
                {
                    MessageBox.Show("No records found for the given Employee ID or Name.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            else
            {
                MessageBox.Show("Please enter an Employee ID or Name.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnAnalytics_Click(object sender, EventArgs e)
        {
            Analytics analyticsform = new Analytics();
            analyticsform.ShowDialog();
        }

        private async void btnGeneratePayslip_Click(object sender, EventArgs e)
        {
            if (dgvPayroll.CurrentRow == null)
            {
                MessageBox.Show("Please select an employee.", "Warning",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeID = dgvPayroll.CurrentRow.Cells["EmployeeID"].Value?.ToString();
            string employeeName = dgvPayroll.CurrentRow.Cells["EmployeeName"].Value?.ToString();

            if (string.IsNullOrEmpty(employeeID))
            {
                MessageBox.Show("Selected employee ID is invalid.", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                PayslipSender payslipSender = new PayslipSender();

                string pdfPath = payslipSender.GeneratePayslipPdf(employeeID);

                decimal grossSalary = Convert.ToDecimal(dgvPayroll.CurrentRow.Cells["GrossSalary"].Value);
                decimal totalDeductions = Convert.ToDecimal(dgvPayroll.CurrentRow.Cells["TotalDeductions"].Value);
                decimal bonuses = Convert.ToDecimal(dgvPayroll.CurrentRow.Cells["Bonuses"].Value);
                decimal netPay = Convert.ToDecimal(dgvPayroll.CurrentRow.Cells["NetPay"].Value);

                string employeeEmail = payslipSender.GetEmployeeEmail(employeeID);

                await payslipSender.GeneratePayslipAndSendSms(employeeID);

                if (!string.IsNullOrEmpty(employeeEmail))
                {
                    payslipSender.SendPayrollEmail(
                        employeeEmail,
                        employeeName,
                        grossSalary,
                        totalDeductions,
                        bonuses,
                        netPay,
                        DateTime.Now,
                        pdfPath);

                    MessageBox.Show($"Payslip generated and notifications sent successfully for {employeeName} (ID: {employeeID})\n" +
                                  $"• Email sent to {employeeEmail}",
                                  "Success",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Payslip generated and SMS sent successfully for {employeeName} (ID: {employeeID})\n" +
                                  "(No email address on file)",
                                  "Success",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating payslip for {employeeName}: {ex.Message}",
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }



        }


        private void btnLoadPayroll_Click(object sender, EventArgs e)
        {
            LoadPayRoll();
        }

        private async void btnCalculatePayroll_Click(object sender, EventArgs e)
        {
            if (dgvPayroll.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dgvPayroll.SelectedRows[0];
                string employeeID = selectedRow.Cells["EmployeeID"].Value?.ToString();
                string employeeName = selectedRow.Cells["EmployeeName"].Value?.ToString();

                string email = GetEmployeeEmail(employeeID);

                SaveManualEntriesToDatabase(employeeID);
                CalculatePayrollForEmployee(employeeID);
                LoadPayRoll();

                // Add this line to generate the PDF
                var payslipSender = new PayslipSender();
                string pdfPath = payslipSender.GeneratePayslipPdf(employeeID);
                MessageBox.Show($"Payslip generated at: {pdfPath}");

                
            }
            else
            {
                MessageBox.Show("Please select an employee from the list.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void CalculatePayrollForEmployee(string employeeID)
        {
            string query = @"
                            UPDATE Payroll
                            SET 
                                GrossSalary = (TotalMonthlyHoursWorked * HourlyRate) + (OvertimeWorked * OvertimeRate) + Bonuses,
                                TotalDeductions = SSS + PhilHealth + PagIbig,
                                NetPay = ((TotalMonthlyHoursWorked * HourlyRate) + (OvertimeWorked * OvertimeRate) + Bonuses) - (SSS + PhilHealth + PagIbig)
                            WHERE EmployeeID = ?";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", employeeID);

                    try
                    {
                        conn.Open();
                        int affectedRows = cmd.ExecuteNonQuery();
                        MessageBox.Show($"Payroll calculated for Employee ID: {employeeID}.", "Payroll Calculation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error calculating payroll: " + ex.Message);
                    }
                }
            }
        }
        private void SaveManualEntriesToDatabase(string employeeID)
        {
            DataGridViewRow selectedRow = dgvPayroll.SelectedRows[0];

            string query = @"
                    UPDATE Payroll 
                    SET 
                        TotalMonthlyHoursWorked = ?,
                        OverTimeWorked = ?,
                        SSS = ?,
                        PhilHealth = ?,
                        PagIbig = ?,
                        Bonuses = ?
                    WHERE EmployeeID = ?";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", selectedRow.Cells["TotalMonthlyHoursWorked"].Value);
                    cmd.Parameters.AddWithValue("?", selectedRow.Cells["OverTimeWorked"].Value);
                    cmd.Parameters.AddWithValue("?", selectedRow.Cells["SSS"].Value);
                    cmd.Parameters.AddWithValue("?", selectedRow.Cells["PhilHealth"].Value);
                    cmd.Parameters.AddWithValue("?", selectedRow.Cells["PagIbig"].Value);
                    cmd.Parameters.AddWithValue("?", selectedRow.Cells["Bonuses"].Value ?? 0); // Handle null
                    cmd.Parameters.AddWithValue("?", employeeID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private string GetEmployeeEmail(string employeeID)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = "SELECT Email FROM PersonalData WHERE EmployeeID = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", employeeID);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        private void btnSearchEmployeeSalary_Click(object sender, EventArgs e)
        {
            string keyword = txtbSearchEmployeeSalary.Text.Trim();
            MessageBox.Show("Searching for: [" + keyword + "]", "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (!string.IsNullOrEmpty(keyword))
            {
                bool recordsFound = SearchPayroll(keyword);
                if (!recordsFound)
                {
                    MessageBox.Show("No records found for the given Employee ID or Name.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please enter an Employee ID or Name.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    public class DatabaseService
        {
            private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";
            public string ConnectionString
            {
                get { return connString; }
            }
            public DataTable ExecuteQuery(string query, List<OleDbParameter> parameters = null)
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        conn.Open();
                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            if (parameters != null)
                            {
                                cmd.Parameters.AddRange(parameters.ToArray());
                            }
                            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }
        public class BaseForm : Form
        {
            protected DatabaseService _databaseService = new DatabaseService();

            protected void LoadData(string query, DataGridView dgv)
            {
                DataTable dt = _databaseService.ExecuteQuery(query);
                if (dt != null)
                {
                    dgv.DataSource = dt;
                }
            }

            protected int GetCount(string query)
            {
                try
                {
                    using (OleDbConnection conn = new OleDbConnection(_databaseService.ConnectionString))
                    {
                        conn.Open();
                        using (OleDbCommand cmd = new OleDbCommand(query, conn))
                        {
                            return Convert.ToInt32(cmd.ExecuteScalar());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    return 0;
                }
            }
        }
            public class PayslipSender
            {
                private string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";

                public async Task GeneratePayslipAndSendSms(string employeeID)
                {
                    string payslipPath = GeneratePayslipPdf(employeeID);

                    string phoneNumber = GetPhoneNumber(employeeID);

                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        await SendSmsWithPayslipLink(phoneNumber, payslipPath);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("Phone number not found.");
                    }

                }

        public string GetEmployeeEmail(string employeeID)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = "SELECT Email FROM PersonalData WHERE EmployeeID = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", employeeID);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        private string GetPhoneNumber(string employeeID)
        {
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                conn.Open();
                string query = "SELECT PhoneNumber FROM PersonalData WHERE EmployeeID = ?";
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", employeeID);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }
        

        private async Task SendSmsWithPayslipLink(string phoneNumber, string filePath)
                {
                    // This is a placeholder - you could upload the file somewhere and get a real link.
                    string payslipLink = "http://example.com/view-payslip";

                    var credentials = Credentials.FromApiKeyAndSecret("b3aa672a", "vpKOWqKMQFrL0Yuc");
                    var client = new VonageClient(credentials);

                    var message = new SendSmsRequest
                    {
                        To = phoneNumber,
                        From = "Ris Tech",
                        Text = $"Your payslip has been generated. View it here: {payslipLink}"
                    };

                    var response = await client.SmsClient.SendAnSmsAsync(message);
                    if (response.Messages[0].Status != "0")
                    {
                        System.Windows.Forms.MessageBox.Show("Failed to send SMS: " + response.Messages[0].ErrorText);
                    }
                }

        public string GeneratePayslipPdf(string employeeID)
        {
            string payslipFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "EmployeePayslips");
            Directory.CreateDirectory(payslipFolder);

            string filePath = Path.Combine(payslipFolder, $"Payslip_{employeeID}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            string fileName = $"Payslip_{employeeID}_{DateTime.Now:yyyyMMddHHmmss}.pdf";


            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();

                    string payslipQuery = "SELECT * FROM Payroll WHERE EmployeeID = ?";
                    using (OleDbCommand payslipCmd = new OleDbCommand(payslipQuery, conn))
                    {
                        payslipCmd.Parameters.AddWithValue("?", employeeID);

                        using (OleDbDataReader reader = payslipCmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string position = "";
                                string employeeName = reader["EmployeeName"].ToString();

                                string empQuery = "SELECT Position FROM Employees WHERE EmployeeID = ?";
                                using (OleDbCommand empCmd = new OleDbCommand(empQuery, conn))
                                {
                                    empCmd.Parameters.AddWithValue("?", employeeID);
                                    object result = empCmd.ExecuteScalar();
                                    if (result != null)
                                        position = result.ToString();
                                }

                                using (Document doc = new Document(PageSize.A4, 40, 40, 40, 40))
                                {
                                    using (PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create)))
                                    {
                                        doc.Open();

                                        Paragraph title = new Paragraph("EMPLOYEE PAYSLIP",
                                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18f, BaseColor.BLUE));
                                        title.Alignment = Element.ALIGN_CENTER;
                                        title.SpacingAfter = 20f;
                                        doc.Add(title);

                                        Paragraph empInfoHeader = new Paragraph("Employee Information",
                                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f));
                                        empInfoHeader.SpacingAfter = 10f;
                                        doc.Add(empInfoHeader);

                                        PdfPTable infoTable = new PdfPTable(2);
                                        infoTable.WidthPercentage = 80;
                                        infoTable.SetWidths(new float[] { 30, 70 });

                                        AddPdfTableRow(infoTable, "Employee ID:", employeeID);
                                        AddPdfTableRow(infoTable, "Employee Name:", employeeName);
                                        AddPdfTableRow(infoTable, "Position:", position);
                                        AddPdfTableRow(infoTable, "Pay Date:", DateTime.Now.ToString("dd/MM/yyyy"));
                                        doc.Add(infoTable);

                                        doc.Add(new Paragraph(" "));

                                        // Earnings
                                        Paragraph earningsHeader = new Paragraph("Earnings",
                                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f));
                                        earningsHeader.SpacingAfter = 10f;
                                        doc.Add(earningsHeader);

                                        PdfPTable earningsTable = new PdfPTable(2);
                                        earningsTable.WidthPercentage = 80;
                                        earningsTable.SetWidths(new float[] { 70, 30 });

                                        AddPdfTableRow(earningsTable, "Base Salary:", FormatCurrency(reader["MonthlySalary"]));
                                        AddPdfTableRow(earningsTable, "Hourly Rate:", FormatCurrency(reader["HourlyRate"]));
                                        AddPdfTableRow(earningsTable, "Total Monthly Hours Worked:", reader["TotalMonthlyHoursWorked"].ToString());
                                        AddPdfTableRow(earningsTable, "Gross Salary:", FormatCurrency(reader["GrossSalary"]));
                                        AddPdfTableRow(earningsTable, "Bonuses:", FormatCurrency(reader["Bonuses"]));
                                        doc.Add(earningsTable);

                                        doc.Add(new Paragraph(" "));

                                        // Deductions
                                        Paragraph deductionsHeader = new Paragraph("Deductions",
                                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f));
                                        deductionsHeader.SpacingAfter = 10f;
                                        doc.Add(deductionsHeader);

                                        PdfPTable deductionsTable = new PdfPTable(2);
                                        deductionsTable.WidthPercentage = 80;
                                        deductionsTable.SetWidths(new float[] { 70, 30 });

                                        decimal sss = Convert.ToDecimal(reader["SSS"]);
                                        decimal philhealth = Convert.ToDecimal(reader["PhilHealth"]);
                                        decimal pagibig = Convert.ToDecimal(reader["PagIbig"]);
                                        decimal totalDeductions = Convert.ToDecimal(reader["TotalDeductions"]);
                                        decimal otherDeductions = totalDeductions - (sss + philhealth + pagibig);

                                        AddPdfTableRow(deductionsTable, "SSS:", FormatCurrency(sss));
                                        AddPdfTableRow(deductionsTable, "PhilHealth:", FormatCurrency(philhealth));
                                        AddPdfTableRow(deductionsTable, "Pag-IBIG:", FormatCurrency(pagibig));
                                        AddPdfTableRow(deductionsTable, "Other Deductions:", FormatCurrency(otherDeductions));

                                        // Net Pay
                                        PdfPCell netLabel = new PdfPCell(new Phrase("NET PAY:",
                                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f)));
                                        netLabel.Border = PdfPCell.NO_BORDER;
                                        deductionsTable.AddCell(netLabel);

                                        PdfPCell netValue = new PdfPCell(new Phrase(FormatCurrency(reader["NetPay"]),
                                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12f, BaseColor.GREEN)));
                                        netValue.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        netValue.Border = PdfPCell.NO_BORDER;
                                        deductionsTable.AddCell(netValue);

                                        doc.Add(deductionsTable);

                                        // Footer
                                        doc.Add(new Paragraph(" "));
                                        Paragraph footer = new Paragraph("This is a computer-generated payslip.",
                                            FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8f));
                                        footer.Alignment = Element.ALIGN_CENTER;
                                        doc.Add(footer);

                                        doc.Close();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error generating payslip: " + ex.Message);
            }
            UpdatePayslipLinkInDatabase(employeeID, fileName);
            return filePath;
        }

        private void AddPdfTableRow(PdfPTable table, string label, string value)
        {
            PdfPCell cell1 = new PdfPCell(new Phrase(label));
            PdfPCell cell2 = new PdfPCell(new Phrase(value));
            cell1.Border = PdfPCell.NO_BORDER;
            cell2.Border = PdfPCell.NO_BORDER;
            table.AddCell(cell1);
            table.AddCell(cell2);
        }

        private string FormatCurrency(object value)
        {
            if (value == null || value == DBNull.Value)
                return "₱0.00";

            if (decimal.TryParse(value.ToString(), out decimal amount))
                return string.Format(CultureInfo.GetCultureInfo("en-PH"), "₱{0:N2}", amount);

            return "₱0.00";
        }
        private void UpdatePayslipLinkInDatabase(string employeeID, string fileName)
        {
            try
            {
                // Use parameterized query with proper placeholders
                string query = "UPDATE Payroll SET PayslipLink = @fileName, PayDate = @currentDate WHERE EmployeeID = @employeeID";

                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    // Add parameters with proper types
                    cmd.Parameters.Add("@fileName", OleDbType.VarChar).Value = fileName;
                    cmd.Parameters.Add("@currentDate", OleDbType.Date).Value = DateTime.Now;
                    cmd.Parameters.Add("@employeeID", OleDbType.VarChar).Value = employeeID;

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        MessageBox.Show("Warning: No records were updated. Please verify the EmployeeID exists.",
                                      "Update Warning",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Warning);
                    }
                }
            }
            catch (OleDbException dbEx)
            {
                MessageBox.Show($"Database error updating payslip link:\n{dbEx.Message}",
                              "Database Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected error updating payslip link:\n{ex.Message}",
                              "Error",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }


        public void SendPayrollEmail(
    string email,
    string employeeName,
    decimal grossSalary,
    decimal totalDeductions,
    decimal bonuses,
    decimal netPay,
    DateTime payDate,
    string payslipLink)
        {
            try
            {
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress("Payroll Department", "your-email@example.com"));
                emailMessage.To.Add(new MailboxAddress(employeeName, email));
                emailMessage.Subject = "Payslip Notification";

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = $"Dear {employeeName},\n\n" +
                    "We are pleased to inform you that your salary for this month has been processed. Below are the details:\n\n" +
                    $"Gross Salary: ₱{grossSalary:N2}\n" +
                    $"Total Deductions (SSS, PhilHealth, Pag-IBIG, etc.): ₱{totalDeductions:N2}\n" +
                    $"Bonuses: ₱{bonuses:N2}\n" +
                    $"Net Pay: ₱{netPay:N2}\n" +
                    $"Pay Date: {payDate.ToString("MMMM dd, yyyy hh:mm tt")}\n\n" +
                    "Thank you for your hard work and dedication.\n\n" +
                    "Best regards,\n" +
                    "Payroll Department"
                };

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("horacelimbaga@gmail.com", "xamy fsqu cxlv luml");
                    client.Send(emailMessage);
                    client.Disconnect(true);
                }

                MessageBox.Show("Payroll email sent successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error sending payroll email: " + ex.Message);
            }
        }


    }
}
