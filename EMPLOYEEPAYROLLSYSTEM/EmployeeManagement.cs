using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;

namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    public partial class EmployeeManagement : Form
    {
        OleDbConnection myConn;
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        private string _currentEmployeeId;
        private bool _personalDataSaved = false;
        private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";
        public EmployeeManagement()
        {
            InitializeComponent();
        }
        public EmployeeManagement(string empID, string employeeName, string department, string position, string experiencelevel, decimal monthlySalary, decimal hourlyRate)
        {
            InitializeComponent();

            tabControlEmployeeManagement.SelectedTab = tabVIEWORUPDATEEMPLOYEE;
            txtBxEmpID.Text = empID;
            txtBxEmpName.Text = employeeName;
            txtBxDepartment.Text = department;
            txtBxPosition.Text = position;
            txtBExperienceLevel.Text = experiencelevel;
            txtBxMonthlySalary.Text = monthlySalary.ToString("0.00");
            txtBHourlyRate.Text = hourlyRate.ToString("0.00");
        }
        private void LoadPositions()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT PositionID, Department, Positions, ExperienceLevel, MonthlySalary, HourlyRate FROM Positions";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvPositions.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ADD_Click(object sender, EventArgs e)
        {

        }

        private void btnPositions_Click(object sender, EventArgs e)
        {
            LoadPositions();
        }

        private void dgvPositions_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPositions.Rows[e.RowIndex];

                txtBDepartment.Text = row.Cells["Department"].Value.ToString();
                txtBPosition.Text = row.Cells["Positions"].Value.ToString();
                txtBxExperienceLevel.Text = row.Cells["ExperienceLevel"].Value.ToString();
                txtBMonthlySalary.Text = row.Cells["MonthlySalary"].Value.ToString();
                txtBHourlyRate.Text = row.Cells["HourlyRate"].Value.ToString();
            }
        }

        private void VIEWORUPDATEEMPLOYEE_Click(object sender, EventArgs e)
        {

        }

        private void btnAddEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBEmployeeID.Text) ||
        string.IsNullOrWhiteSpace(txtBEmployeeFullName.Text) ||
        string.IsNullOrWhiteSpace(txtBDepartment.Text) ||
        string.IsNullOrWhiteSpace(txtBxExperienceLevel.Text) ||
        string.IsNullOrWhiteSpace(txtBPosition.Text) ||
        string.IsNullOrWhiteSpace(txtBMonthlySalary.Text) ||
        string.IsNullOrWhiteSpace(txtBHourlyRate.Text) ||
        string.IsNullOrWhiteSpace(txtBUsername.Text) ||
        string.IsNullOrWhiteSpace(txtBPassword.Text) ||
        cmbxUserType.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill in all fields before adding an employee.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeeQuery = "UPDATE Employees SET [HireDate] = ?, [Department] = ?, [Position] = ?, [ExperienceLevel] = ? , [MonthlySalary] = ?, [HourlyRate] = ? WHERE [EmployeeID] = ?";

            string userQuery = "INSERT INTO Users ([Username], [Password], [FullName], [UserType], [CreatedAt], [EmployeeID]) " +
                               "VALUES (@Username, @Password, @FullName, @UserType, @CreatedAt, @EmployeeID)";

            string payrollQuery = "INSERT INTO Payroll ([EmployeeID], [EmployeeName], [MonthlySalary], [HourlyRate]) " +
                                 "VALUES (@PayrollEmployeeID, @PayrollEmployeeName, @PayrollMonthlySalary, @PayrollHourlyRate)";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    using (OleDbCommand cmdEmp = new OleDbCommand(employeeQuery, conn))
                    {
                        cmdEmp.Parameters.Add("@HireDate", OleDbType.Date).Value = dtpHireDate.Value;
                        cmdEmp.Parameters.Add("@Department", OleDbType.VarChar).Value = txtBDepartment.Text.Trim();
                        cmdEmp.Parameters.Add("@Position", OleDbType.VarChar).Value = txtBPosition.Text.Trim();
                        cmdEmp.Parameters.Add("@ExperienceLevel", OleDbType.VarChar).Value = txtBxExperienceLevel.Text.Trim();

                        if (decimal.TryParse(txtBMonthlySalary.Text.Trim(), out decimal baseSalary))
                        {
                            cmdEmp.Parameters.AddWithValue("@MonthlySalary", baseSalary);
                        }
                        else
                        {
                            MessageBox.Show("Invalid hourly rate. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (decimal.TryParse(txtBHourlyRate.Text.Trim(), out decimal hourlyRate))
                        {
                            cmdEmp.Parameters.AddWithValue("@HourlyRate", hourlyRate);
                        }
                        else
                        {
                            MessageBox.Show("Invalid salary amount. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        cmdEmp.Parameters.Add("@EmployeeID", OleDbType.VarChar).Value = txtBEmployeeID.Text.Trim();

                        int empRows = cmdEmp.ExecuteNonQuery();

                        if (empRows > 0)
                        {
                            using (OleDbCommand cmdUser = new OleDbCommand(userQuery, conn))
                            {
                                cmdUser.Parameters.Add("@Username", OleDbType.VarChar).Value = txtBUsername.Text.Trim();
                                cmdUser.Parameters.Add("@Password", OleDbType.VarChar).Value = txtBPassword.Text.Trim();
                                cmdUser.Parameters.Add("@FullName", OleDbType.VarChar).Value = txtBEmployeeFullName.Text.Trim();
                                cmdUser.Parameters.Add("@UserType", OleDbType.VarChar).Value = cmbxUserType.SelectedItem.ToString();
                                cmdUser.Parameters.Add("@CreatedAt", OleDbType.Date).Value = DateTime.Now;
                                cmdUser.Parameters.Add("@EmployeeID", OleDbType.VarChar).Value = txtBEmployeeID.Text.Trim();

                                int userRows = cmdUser.ExecuteNonQuery();

                                if (userRows > 0)
                                {
                                    // Insert into Payroll table
                                    using (OleDbCommand cmdPayroll = new OleDbCommand(payrollQuery, conn))
                                    {
                                        cmdPayroll.Parameters.Add("@PayrollEmployeeID", OleDbType.VarChar).Value = txtBEmployeeID.Text.Trim();
                                        cmdPayroll.Parameters.Add("@PayrollEmployeeName", OleDbType.VarChar).Value = txtBEmployeeFullName.Text.Trim();
                                        cmdPayroll.Parameters.Add("@PayrollMonthlySalary", OleDbType.Decimal).Value = baseSalary;
                                        cmdPayroll.Parameters.Add("@PayrollHourlyRate", OleDbType.Decimal).Value = hourlyRate;

                                        int payrollRows = cmdPayroll.ExecuteNonQuery();

                                        if (payrollRows > 0)
                                        {
                                            MessageBox.Show("Employee, user account, and payroll information added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                            txtBEmployeeID.Clear();
                                            txtBEmployeeFullName.Clear();
                                            txtBDepartment.Clear();
                                            txtBxExperienceLevel.Clear();
                                            txtBPosition.Clear();
                                            txtBMonthlySalary.Clear();
                                            txtBHourlyRate.Clear();
                                            dtpHireDate.Value = DateTime.Now;
                                            txtBUsername.Clear();
                                            txtBPassword.Clear();
                                            cmbxUserType.SelectedIndex = -1;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Failed to add payroll information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Failed to create user account.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Failed to add employee. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void btnPosition2_Click(object sender, EventArgs e)
        {
            LoadPositions();
            dgvPositions2.DataSource = dgvPositions.DataSource;

        }

        private void btnUpdateEmployee_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBxEmpID.Text) ||
        string.IsNullOrWhiteSpace(txtBxEmpName.Text) ||
        string.IsNullOrWhiteSpace(txtBxDepartment.Text) ||
        string.IsNullOrWhiteSpace(txtBExperienceLevel.Text) ||
        string.IsNullOrWhiteSpace(txtBxPosition.Text) ||
        string.IsNullOrWhiteSpace(txtBxMonthlySalary.Text))
            {
                MessageBox.Show("Please fill in all fields before updating the employee.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string updateEmpQuery = "UPDATE Employees SET [EmployeeName] = ?, [Department] = ?, [Position] = ?, [ExperienceLevel] = ?, [MonthlySalary] = ?, [HourlyRate] = ? WHERE [EmployeeID] = ?";
            string checkPayrollQuery = "SELECT COUNT(*) FROM Payroll WHERE EmployeeID = ?";
            string insertPayrollQuery = "INSERT INTO Payroll ([EmployeeID], [EmployeeName], [MonthlySalary], [HourlyRate]) VALUES (?, ?, ?, ?)";
            string updatePayrollQuery = "UPDATE Payroll SET [EmployeeName] = ?, [MonthlySalary] = ?, [HourlyRate] = ? WHERE [EmployeeID] = ?";

            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Update Employee
                    using (OleDbCommand cmdEmp = new OleDbCommand(updateEmpQuery, conn))
                    {
                        cmdEmp.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpName.Text.Trim();
                        cmdEmp.Parameters.Add("?", OleDbType.VarChar).Value = txtBxDepartment.Text.Trim();
                        cmdEmp.Parameters.Add("?", OleDbType.VarChar).Value = txtBxPosition.Text.Trim();
                        cmdEmp.Parameters.Add("?", OleDbType.VarChar).Value = txtBExperienceLevel.Text.Trim();

                        if (!decimal.TryParse(txtBxMonthlySalary.Text.Trim(), out decimal monthlySalary))
                        {
                            MessageBox.Show("Invalid monthly salary. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        cmdEmp.Parameters.Add("?", OleDbType.Currency).Value = monthlySalary;

                        if (!decimal.TryParse(txtBxHourlyRate.Text.Trim(), out decimal hourlyRate))
                        {
                            MessageBox.Show("Invalid hourly rate. Please enter a valid number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        cmdEmp.Parameters.Add("?", OleDbType.Currency).Value = hourlyRate;

                        cmdEmp.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpID.Text.Trim();

                        int rowsAffected = cmdEmp.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            // Check if Payroll record exists
                            using (OleDbCommand cmdCheck = new OleDbCommand(checkPayrollQuery, conn))
                            {
                                cmdCheck.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpID.Text.Trim();
                                int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                                if (count > 0)
                                {
                                    // Update existing Payroll
                                    using (OleDbCommand cmdUpdatePayroll = new OleDbCommand(updatePayrollQuery, conn))
                                    {
                                        cmdUpdatePayroll.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpName.Text.Trim();
                                        cmdUpdatePayroll.Parameters.Add("?", OleDbType.Currency).Value = monthlySalary;
                                        cmdUpdatePayroll.Parameters.Add("?", OleDbType.Currency).Value = hourlyRate;
                                        cmdUpdatePayroll.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpID.Text.Trim();

                                        cmdUpdatePayroll.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    // Insert new Payroll
                                    using (OleDbCommand cmdInsertPayroll = new OleDbCommand(insertPayrollQuery, conn))
                                    {
                                        cmdInsertPayroll.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpID.Text.Trim();
                                        cmdInsertPayroll.Parameters.Add("?", OleDbType.VarChar).Value = txtBxEmpName.Text.Trim();
                                        cmdInsertPayroll.Parameters.Add("?", OleDbType.Currency).Value = monthlySalary;
                                        cmdInsertPayroll.Parameters.Add("?", OleDbType.Currency).Value = hourlyRate;

                                        cmdInsertPayroll.ExecuteNonQuery();
                                    }
                                }

                                MessageBox.Show("Employee and payroll information updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("No records updated. Check if the Employee ID exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvPositions2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvPositions2.Rows[e.RowIndex];

                txtBxDepartment.Text = row.Cells["Department"].Value.ToString();
                txtBxPosition.Text = row.Cells["Positions"].Value.ToString();
                txtBExperienceLevel.Text = row.Cells["ExperienceLevel"].Value.ToString();
                txtBxMonthlySalary.Text = row.Cells["MonthlySalary"].Value.ToString();
                txtBxHourlyRate.Text = row.Cells["HourlyRate"].Value.ToString();
            }
        }

        private void tabPPersonalInfo_Click(object sender, EventArgs e)
        {

        }

        private void btnSavePersonalData_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtBEmpID.Text))
                {
                    MessageBox.Show("Employee ID is required and cannot be empty.",
                                   "Validation Error",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Warning);
                    txtBEmpID.Focus();
                    return;
                }
                // Validate required fields
                if  (string.IsNullOrWhiteSpace(txtBLastName.Text) ||
                    string.IsNullOrWhiteSpace(txtBFirstName.Text) ||
                    string.IsNullOrWhiteSpace(txtBAddress.Text) ||
                    string.IsNullOrWhiteSpace(txtBPhoneNum.Text) ||
                    string.IsNullOrWhiteSpace(txtBEmail.Text) ||
                    string.IsNullOrWhiteSpace(txtBEmergencyContact.Text) ||
                    string.IsNullOrWhiteSpace(txtBSSSNum.Text) ||
                    string.IsNullOrWhiteSpace(txtBPagIbigNum.Text) ||
                    string.IsNullOrWhiteSpace(txtBPhilHealthNum.Text))
                {
                    MessageBox.Show("Please fill in all fields before adding an employee.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate number formats
                if (txtBSSSNum.Text.Length != 10 || !txtBSSSNum.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Please enter a valid 10-digit SSS number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtBPagIbigNum.Text.Length != 12 || !txtBPagIbigNum.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Please enter a valid 12-digit Pag-IBIG number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtBPhilHealthNum.Text.Length != 12 || !txtBPhilHealthNum.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Please enter a valid 12-digit PhilHealth number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    OleDbTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        // Insert into Employees table
                        string employeeQuery = "INSERT INTO Employees ([EmployeeID], [EmployeeName]) VALUES (?, ?)";
                        using (OleDbCommand empCmd = new OleDbCommand(employeeQuery, conn, transaction))
                        {
                            empCmd.Parameters.AddWithValue("@EmployeeID", txtBEmpID.Text.Trim());

                            string fullName = $"{txtBLastName.Text.Trim()}, {txtBFirstName.Text.Trim()} {txtBMiddleName.Text.Trim()}";
                            empCmd.Parameters.AddWithValue("@EmployeeName", fullName);
                            empCmd.ExecuteNonQuery();
                        }

                        // Insert into PersonalData table
                        string personalQuery = "INSERT INTO PersonalData ([EmployeeID], [LastName], [FirstName], [MiddleName], [DateOfBirth], [Gender], [Address], [PhoneNumber], [Email], [EmergencyContact], [SSSNumber], [PagIbigNumber], [PhilHealthNumber]) " +
                                               "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                        using (OleDbCommand cmd = new OleDbCommand(personalQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@EmployeeID", txtBEmpID.Text.Trim());
                            cmd.Parameters.AddWithValue("@LastName", txtBLastName.Text.Trim());
                            cmd.Parameters.AddWithValue("@FirstName", txtBFirstName.Text.Trim());
                            cmd.Parameters.AddWithValue("@MiddleName", txtBMiddleName.Text.Trim());
                            cmd.Parameters.AddWithValue("@DateOfBirth", datetpDateofBirth.Value.Date);
                            cmd.Parameters.AddWithValue("@Gender", rbMale.Checked ? "Male" : "Female");
                            cmd.Parameters.AddWithValue("@Address", txtBAddress.Text.Trim());
                            cmd.Parameters.AddWithValue("@PhoneNumber", txtBPhoneNum.Text.Trim());
                            cmd.Parameters.AddWithValue("@Email", txtBEmail.Text.Trim());
                            cmd.Parameters.AddWithValue("@EmergencyContact", txtBEmergencyContact.Text.Trim());
                            cmd.Parameters.AddWithValue("@SSSNumber", txtBSSSNum.Text.Trim());
                            cmd.Parameters.AddWithValue("@PagIbigNumber", txtBPagIbigNum.Text.Trim());
                            cmd.Parameters.AddWithValue("@PhilHealthNumber", txtBPhilHealthNum.Text.Trim());

                            cmd.ExecuteNonQuery();
                        }

                        // Commit transaction
                        transaction.Commit();
                        _currentEmployeeId = txtBEmpID.Text.Trim();
                        _personalDataSaved = true;
                        txtBEmployeeID.Text = _currentEmployeeId;
                        txtBEmployeeFullName.Text = $"{txtBLastName.Text}, {txtBFirstName.Text} {txtBMiddleName.Text}";
                        MessageBox.Show("Personal data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ClearFields();
                        tabControlEmployeeManagement.SelectedTab = tabPAddEmployee;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            txtBEmpID.Clear();
            txtBLastName.Clear();
            txtBFirstName.Clear();
            txtBMiddleName.Clear();
            txtBAddress.Clear();
            txtBPhoneNum.Clear();
            txtBEmail.Clear();
            txtBEmergencyContact.Clear();
            txtBSSSNum.Clear();
            txtBPagIbigNum.Clear();
            txtBPhilHealthNum.Clear();
            datetpDateofBirth.Value = DateTime.Now;
            rbMale.Checked = false;
            rbFemale.Checked = false;
        }

        private void btnGenerateEmpID_Click(object sender, EventArgs e)
        {
            string newEmpID = GenerateUniqueEmpID();
            txtBEmpID.Text = newEmpID;
        }

        private string GenerateUniqueEmpID()
        {
            string empID;
            do
            {
                empID = GenerateRandomEmpID();
            }
            while (IsEmpIDExists(empID));

            return empID;
        }
        private string GenerateRandomEmpID()
        {
            Random random = new Random();

            string letters = "";
            for (int i = 0; i < 3; i++)
            {
                letters += (char)random.Next('A', 'Z' + 1);
            }
            int numbers = random.Next(0, 10000);
            string numberPart = numbers.ToString("D4"); // Format to 4 digits
            string empID = letters + numberPart;

            return empID;
        }
        private bool IsEmpIDExists(string empID)
        {
            bool exists = false;
            string query = "SELECT COUNT(*) FROM Employees WHERE EmployeeID = @EmpID";

            using (OleDbConnection conn = new OleDbConnection(connString))
            using (OleDbCommand cmd = new OleDbCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@EmpID", empID);
                conn.Open();
                int count = (int)cmd.ExecuteScalar();
                exists = count > 0;
            }

            return exists;
        }

        private void txtBEmpID_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class EmpIDGenerator : EmployeeManagement
    {
        
    }

}




