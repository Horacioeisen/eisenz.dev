using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    public partial class adminForm : BaseForm
    {
        OleDbConnection? myConn;
        OleDbDataAdapter? myDataAdapter;
        OleDbCommand? myCmd;
        DataSet? myDataSet;
        int indexRow;

        public adminForm()
        {
            InitializeComponent();
            timer1CurrentTimeDashboard.Start();
            ShowPanel(panelDashboard);


        }
        private void ShowPanel(Panel panelToShow)
        {
            panelDashboard.Visible = false;

            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }
        private void LoadDashboardData()
        {
            string connString = ("Provider=Microsoft.ACE.OLEDB.12.0;Data Source= C:\\Users\\User\\Document\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb");
            using (OleDbConnection conn = new OleDbConnection(connString))
            {
                try
                {
                    conn.Open();

                    // Get total employees
                    string queryEmployees = "SELECT COUNT(*) FROM Employees";
                    OleDbCommand cmdEmployees = new OleDbCommand(queryEmployees, conn);
                    int totalEmployees = Convert.ToInt32(cmdEmployees.ExecuteScalar());

                    // Get total HR users
                    string queryHR = "SELECT COUNT(*) FROM Users WHERE Role = 'HR'";
                    OleDbCommand cmdHR = new OleDbCommand(queryHR, conn);
                    int totalHR = Convert.ToInt32(cmdHR.ExecuteScalar());

                    // Get total Admin users
                    string queryAdmin = "SELECT COUNT(*) FROM Users WHERE Role = 'Admin'";
                    OleDbCommand cmdAdmin = new OleDbCommand(queryAdmin, conn);
                    int totalAdmins = Convert.ToInt32(cmdAdmin.ExecuteScalar());

                    // Update UI labels
                    lblTotalEmployees.Text = totalEmployees.ToString();
                    lblHRUsers.Text = totalHR.ToString();
                    lblAdminUsers.Text = totalAdmins.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading dashboard data: " + ex.Message);
                }
            }
        }
        private void timer1CurrentTimeDashboard_Tick(object sender, EventArgs e)
        {
            labelTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }


        private void btnDashboard_Click(object sender, EventArgs e)
        {
            ShowPanel(panelDashboard);
        }

        private void btnDatabase_Click(object sender, EventArgs e)
        {
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
        }

        private void btnLogs_Click(object sender, EventArgs e)
        {

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
    }
}
