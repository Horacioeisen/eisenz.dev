namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    partial class Analytics
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Analytics));
            lblAnalytics = new Label();
            pictureBox1 = new PictureBox();
            timerDate = new System.Windows.Forms.Timer(components);
            tabPEmployees = new TabPage();
            panel4 = new Panel();
            panelLoadLeaves = new Panel();
            btnRefreshDepartmentChart = new Button();
            tabPAttendance = new TabPage();
            panel1 = new Panel();
            panel2 = new Panel();
            lblDate = new Label();
            tabctrlAnalytics = new TabControl();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            tabPEmployees.SuspendLayout();
            panelLoadLeaves.SuspendLayout();
            tabPAttendance.SuspendLayout();
            tabctrlAnalytics.SuspendLayout();
            SuspendLayout();
            // 
            // lblAnalytics
            // 
            lblAnalytics.AutoSize = true;
            lblAnalytics.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAnalytics.ForeColor = Color.White;
            lblAnalytics.Location = new Point(728, 17);
            lblAnalytics.Name = "lblAnalytics";
            lblAnalytics.Size = new Size(194, 54);
            lblAnalytics.TabIndex = 5;
            lblAnalytics.Text = "Analytics";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(620, 7);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(102, 73);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // timerDate
            // 
            timerDate.Enabled = true;
            timerDate.Tick += timerDate_Tick;
            // 
            // tabPEmployees
            // 
            tabPEmployees.BackColor = Color.FromArgb(17, 34, 64);
            tabPEmployees.Controls.Add(panelLoadLeaves);
            tabPEmployees.Controls.Add(panel4);
            tabPEmployees.Location = new Point(4, 29);
            tabPEmployees.Name = "tabPEmployees";
            tabPEmployees.Padding = new Padding(3);
            tabPEmployees.Size = new Size(1479, 683);
            tabPEmployees.TabIndex = 1;
            tabPEmployees.Text = "Employees";
            // 
            // panel4
            // 
            panel4.Location = new Point(20, 20);
            panel4.Name = "panel4";
            panel4.Size = new Size(1096, 642);
            panel4.TabIndex = 2;
            // 
            // panelLoadLeaves
            // 
            panelLoadLeaves.BackColor = Color.FromArgb(17, 34, 64);
            panelLoadLeaves.BorderStyle = BorderStyle.Fixed3D;
            panelLoadLeaves.Controls.Add(btnRefreshDepartmentChart);
            panelLoadLeaves.Font = new Font("Microsoft Sans Serif", 8.25F);
            panelLoadLeaves.ForeColor = SystemColors.ControlLightLight;
            panelLoadLeaves.Location = new Point(1172, 36);
            panelLoadLeaves.Name = "panelLoadLeaves";
            panelLoadLeaves.Size = new Size(206, 43);
            panelLoadLeaves.TabIndex = 26;
            // 
            // btnRefreshDepartmentChart
            // 
            btnRefreshDepartmentChart.BackColor = Color.Gold;
            btnRefreshDepartmentChart.Cursor = Cursors.Hand;
            btnRefreshDepartmentChart.FlatAppearance.BorderSize = 0;
            btnRefreshDepartmentChart.FlatStyle = FlatStyle.Flat;
            btnRefreshDepartmentChart.Font = new Font("Segoe UI", 13.2000008F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefreshDepartmentChart.ForeColor = Color.Black;
            btnRefreshDepartmentChart.Image = (Image)resources.GetObject("btnRefreshDepartmentChart.Image");
            btnRefreshDepartmentChart.ImageAlign = ContentAlignment.MiddleLeft;
            btnRefreshDepartmentChart.Location = new Point(-19, -28);
            btnRefreshDepartmentChart.Name = "btnRefreshDepartmentChart";
            btnRefreshDepartmentChart.Padding = new Padding(25, 0, 0, 0);
            btnRefreshDepartmentChart.Size = new Size(246, 97);
            btnRefreshDepartmentChart.TabIndex = 13;
            btnRefreshDepartmentChart.Text = "        Refresh";
            btnRefreshDepartmentChart.TextAlign = ContentAlignment.MiddleLeft;
            btnRefreshDepartmentChart.UseVisualStyleBackColor = false;
            btnRefreshDepartmentChart.Click += btnRefreshDepartmentChart_Click;
            // 
            // tabPAttendance
            // 
            tabPAttendance.BackColor = Color.FromArgb(17, 34, 64);
            tabPAttendance.BackgroundImage = (Image)resources.GetObject("tabPAttendance.BackgroundImage");
            tabPAttendance.Controls.Add(lblDate);
            tabPAttendance.Controls.Add(panel2);
            tabPAttendance.Controls.Add(panel1);
            tabPAttendance.Location = new Point(4, 29);
            tabPAttendance.Name = "tabPAttendance";
            tabPAttendance.Padding = new Padding(3);
            tabPAttendance.Size = new Size(1479, 683);
            tabPAttendance.TabIndex = 0;
            tabPAttendance.Text = "Attendance";
            // 
            // panel1
            // 
            panel1.Location = new Point(20, 158);
            panel1.Name = "panel1";
            panel1.Size = new Size(667, 460);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Location = new Point(792, 158);
            panel2.Name = "panel2";
            panel2.Size = new Size(667, 460);
            panel2.TabIndex = 1;
            // 
            // lblDate
            // 
            lblDate.AutoSize = true;
            lblDate.BackColor = Color.Transparent;
            lblDate.Font = new Font("Segoe UI Semibold", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDate.ForeColor = Color.White;
            lblDate.Location = new Point(20, 102);
            lblDate.Name = "lblDate";
            lblDate.Size = new Size(77, 38);
            lblDate.TabIndex = 6;
            lblDate.Text = "Date";
            // 
            // tabctrlAnalytics
            // 
            tabctrlAnalytics.Controls.Add(tabPAttendance);
            tabctrlAnalytics.Controls.Add(tabPEmployees);
            tabctrlAnalytics.Location = new Point(31, 57);
            tabctrlAnalytics.Name = "tabctrlAnalytics";
            tabctrlAnalytics.SelectedIndex = 0;
            tabctrlAnalytics.Size = new Size(1487, 716);
            tabctrlAnalytics.TabIndex = 0;
            // 
            // Analytics
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 25, 47);
            ClientSize = new Size(1550, 800);
            Controls.Add(pictureBox1);
            Controls.Add(lblAnalytics);
            Controls.Add(tabctrlAnalytics);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "Analytics";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Analytics";
            Load += Analytics_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            tabPEmployees.ResumeLayout(false);
            panelLoadLeaves.ResumeLayout(false);
            tabPAttendance.ResumeLayout(false);
            tabPAttendance.PerformLayout();
            tabctrlAnalytics.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblAnalytics;
        private PictureBox pictureBox1;
        private System.Windows.Forms.Timer timerDate;
        private TabPage tabPEmployees;
        private Panel panelLoadLeaves;
        private Button btnRefreshDepartmentChart;
        private Panel panel4;
        private TabPage tabPAttendance;
        private Label lblDate;
        private Panel panel2;
        private Panel panel1;
        private TabControl tabctrlAnalytics;
    }
}