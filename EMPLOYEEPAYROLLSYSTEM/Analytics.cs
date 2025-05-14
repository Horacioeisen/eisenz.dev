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
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;

namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    public partial class Analytics : Form
    {
        OleDbConnection myConn;
        OleDbDataAdapter? da;
        OleDbCommand? cmd;
        DataSet? ds;
        int indexRow;
        private Chart attendanceChart; //get weekly totalhoursworked
        private Chart attendanceStatsChart; //get total counts of presen&lates
        private Chart employeesDeptChart; //get total employees in each department
        private const string connString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\User\\Documents\\RisTechCompany Database\\EmployeePayrollManagementDatabase.accdb";
        public Analytics()
        {
            InitializeComponent();
            timerDate.Start();
            InitializeChart();
            LoadChartData();
        }

        private void InitializeChart()
        {
            InitializeAttendanceChart();
            InitializeAttendanceStatsChart();
            InitializeEmployeesDeptChart();
        }

        private void InitializeEmployeesDeptChart()
        {
            employeesDeptChart = new Chart
            {
                Name = "chartEmployeeDept",
                Size = new Size(1096, 642),
                Location = new Point(20, 20), // Adjust as needed to avoid overlap
                BackColor = Color.WhiteSmoke
            };

            ChartArea chartArea = new ChartArea("DeptChartArea");
            chartArea.BackColor = Color.White;
            chartArea.Area3DStyle.Enable3D = true;
            employeesDeptChart.ChartAreas.Add(chartArea);

            Series deptSeries = new Series("Employees per Department")
            {
                ChartType = SeriesChartType.Pie,
                IsValueShownAsLabel = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                LabelForeColor = Color.Black,
                Label = "#PERCENT\n#VALX",
                Palette = ChartColorPalette.BrightPastel
            };
            deptSeries["PieLabelStyle"] = "Outside";
            deptSeries["PieDrawingStyle"] = "Concave";
            employeesDeptChart.Series.Add(deptSeries);

            Title chartTitle = new Title("Employees per Department")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            employeesDeptChart.Titles.Add(chartTitle);

            // Optional: Add legend
            employeesDeptChart.Legends.Add(new Legend("DeptLegend"));

            tabPEmployees.Controls.Add(employeesDeptChart);
            employeesDeptChart.BringToFront();
        }
        private void InitializeAttendanceStatsChart() //initialize the attendance stats chart (present&lates)
        {
            attendanceStatsChart = new Chart //employee attendance stats (present&lates)
            {
                Name = "chartAttendanceStats",
                Size = new Size(667, 460),
                Location = new Point(792, 158),
                BackColor = Color.WhiteSmoke
            };

            ChartArea chartArea = new ChartArea("StatsChartArea")  //employee attendance stats (present&lates)
            {
                AxisX = {
            Title = "Employee",
            Interval = 1,
            MajorGrid = { Enabled = false },
            LabelStyle = { Angle = -45 },
            LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont |
                               LabelAutoFitStyles.StaggeredLabels
        },
                AxisY = {
            Title = "Count",
            MajorGrid = { LineDashStyle = ChartDashStyle.Dot },
            Minimum = 0
        }
            };
            attendanceStatsChart.ChartAreas.Add(chartArea);

            // Series for Present days
            Series presentSeries = new Series("Presents")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.SeaGreen,
                IsValueShownAsLabel = true,
                Label = "#VALY (Present)",
                Font = new Font("Arial", 8, FontStyle.Bold),
                LabelForeColor = Color.Black
            };
            attendanceStatsChart.Series.Add(presentSeries);

            // Series for Late arrivals
            Series lateSeries = new Series("Lates")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.Orange,
                IsValueShownAsLabel = true,
                Label = "#VALY (Late)",
                Font = new Font("Arial", 8, FontStyle.Bold),
                LabelForeColor = Color.Black
            };
            attendanceStatsChart.Series.Add(lateSeries);

            Title chartTitle = new Title("Employee Attendance Statistics (Monthly)")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            attendanceStatsChart.Titles.Add(chartTitle);

            tabPAttendance.Controls.Add(attendanceStatsChart);
            attendanceStatsChart.BringToFront();
        }

        private void InitializeAttendanceChart() //initialize the attendance chart
        {
            attendanceChart = new Chart //employee monthly hours worked
            {
                Name = "chartAttendance",
                Size = new Size(667, 460),
                Location = new Point(20, 158),
                BackColor = Color.WhiteSmoke
            };

            ChartArea chartArea = new ChartArea("MainChartArea")
            {
                AxisX = {
            Title = "Employee",
            Interval = 1,
            MajorGrid = { Enabled = false },
            LabelStyle = { Angle = -45 },
            LabelAutoFitStyle = LabelAutoFitStyles.DecreaseFont |
                                 LabelAutoFitStyles.StaggeredLabels
        },
                AxisY = {
            Title = "Hours Worked (Monthly)",
            MajorGrid = { LineDashStyle = ChartDashStyle.Dot },
            Minimum = 0
        }
            };
            attendanceChart.ChartAreas.Add(chartArea);

            Series series = new Series("Hours Worked")
            {
                ChartType = SeriesChartType.Column,
                Color = Color.SteelBlue,
                IsValueShownAsLabel = true,
                LabelFormat = "N1",
                Font = new Font("Arial", 8, FontStyle.Bold),
                LabelBackColor = Color.Transparent
            };
            attendanceChart.Series.Add(series);

            Title chartTitle = new Title("Employee Monthly Hours Worked")
            {
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            attendanceChart.Titles.Add(chartTitle);

            tabPAttendance.Controls.Add(attendanceChart);
            attendanceChart.BringToFront();

        }

        private void LoadAttendanceStatsChartData()
        {
            try
            {
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                string query = @"
                    SELECT 
                        EmployeeName,
                        SUM(IIF([Status] = 'Present', 1, 0)) AS PresentCount,
                        SUM(IIF([Status] = 'Late', 1, 0)) AS LateCount
                    FROM 
                        Attendance 
                    WHERE 
                        [Date] BETWEEN ? AND ?
                    GROUP BY 
                        EmployeeName
                    ORDER BY 
                        EmployeeName";

                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("?", startDate);
                    cmd.Parameters.AddWithValue("?", endDate);

                    conn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        attendanceStatsChart.Series["Presents"].Points.Clear();
                        attendanceStatsChart.Series["Lates"].Points.Clear();

                        while (reader.Read())
                        {
                            string empName2 = reader["EmployeeName"].ToString();
                            int presents = Convert.ToInt32(reader["PresentCount"]);
                            int lates = Convert.ToInt32(reader["LateCount"]);

                            attendanceStatsChart.Series["Presents"].Points.AddXY(empName2, presents);
                            attendanceStatsChart.Series["Lates"].Points.AddXY(empName2, lates);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading attendance stats: {ex.Message}",
                                "Chart Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LoadAttendanceChartData()
        {
            try
            {
                string query = @"
            SELECT 
                EmployeeName,
                SUM(TotalHoursWorked) AS TotalHours
            FROM 
                Attendance
            WHERE 
                Format([Date], 'yyyy-mm') = Format(Date(), 'yyyy-mm')
            GROUP BY 
                EmployeeName";

                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    Console.WriteLine("Executing: " + cmd.CommandText);
                    conn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        attendanceChart.Series[0].Points.Clear();
                        attendanceChart.Series[0].LegendText = "Monthly Hours";

                        while (reader.Read())
                        {
                            string empName = reader["EmployeeName"].ToString();
                            double hours = Convert.ToDouble(reader["TotalHours"]);
                            attendanceChart.Series[0].Points.AddXY(empName, hours);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading monthly attendance chart: {ex.Message}",
                                "Chart Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LoadEmployeeDeptChartData()
        {
            try
            {
                string query = @"
                            SELECT Department, COUNT(*) AS TotalEmployees
                            FROM Employees
                            GROUP BY Department";

                using (OleDbConnection conn = new OleDbConnection(connString))
                using (OleDbCommand cmd = new OleDbCommand(query, conn))
                {
                    conn.Open();
                    using (OleDbDataReader reader = cmd.ExecuteReader())
                    {
                        Series series = employeesDeptChart.Series["Employees per Department"];
                        series.Points.Clear();

                        int totalEmployees = 0;

                        while (reader.Read())
                        {
                            string department = reader["Department"].ToString();
                            int count = Convert.ToInt32(reader["TotalEmployees"]);
                            totalEmployees += count;
                            series.Points.AddXY(department, count);
                        }

                        // Update or add chart title
                        if (employeesDeptChart.Titles.Count > 0)
                        {
                            employeesDeptChart.Titles[0].Text = $"Employees per Department (Total Employees: {totalEmployees})";
                        }
                        else
                        {
                            employeesDeptChart.Titles.Add(new Title(
                                $"Employees per Department (Total Employees: {totalEmployees})",
                                Docking.Top,
                                new Font("Segoe UI", 12, FontStyle.Bold),
                                Color.Black));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee department chart: {ex.Message}",
                                "Chart Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void LoadChartData()
        {
            if (ds != null) ds.Clear();
            try
            {
                LoadAttendanceChartData();
                LoadAttendanceStatsChartData();
                LoadEmployeeDeptChartData();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading chart data: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void timerDate_Tick(object sender, EventArgs e)
        {
            lblDate.Text = DateTime.Now.ToString("dddd, yyyy-MM-dd HH:mm:ss");
        }

        private void Analytics_Load(object sender, EventArgs e)
        {

        }

        private void btnRefreshDepartmentChart_Click(object sender, EventArgs e)
        {
            LoadEmployeeDeptChartData();
        }
    }
}
