namespace EMPLOYEE_PAYROLL_MANAGEMENT_SYSTEM
{
    partial class AttendancePortal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttendancePortal));
            nightControlBox1 = new ReaLTaiizor.Controls.NightControlBox();
            panelAttendancePortal = new Panel();
            lblTimeOut = new Label();
            lblTimeIn = new Label();
            lblStatus = new Label();
            lblEmpName = new Label();
            lblDatetime = new Label();
            panel1 = new Panel();
            btnBcktoLogin = new Button();
            lblEmployeeID = new Label();
            txtbEmpID = new TextBox();
            panelScanID = new Panel();
            btnScanID = new Button();
            lblAttendancePortal = new Label();
            timerAttendance = new System.Windows.Forms.Timer(components);
            lblOvertime = new Label();
            panelAttendancePortal.SuspendLayout();
            panel1.SuspendLayout();
            panelScanID.SuspendLayout();
            SuspendLayout();
            // 
            // nightControlBox1
            // 
            nightControlBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            nightControlBox1.BackColor = Color.Transparent;
            nightControlBox1.CloseHoverColor = Color.FromArgb(199, 80, 80);
            nightControlBox1.CloseHoverForeColor = Color.White;
            nightControlBox1.DefaultLocation = true;
            nightControlBox1.DisableMaximizeColor = Color.FromArgb(105, 105, 105);
            nightControlBox1.DisableMinimizeColor = Color.FromArgb(105, 105, 105);
            nightControlBox1.EnableCloseColor = Color.FromArgb(160, 160, 160);
            nightControlBox1.EnableMaximizeButton = true;
            nightControlBox1.EnableMaximizeColor = Color.FromArgb(160, 160, 160);
            nightControlBox1.EnableMinimizeButton = true;
            nightControlBox1.EnableMinimizeColor = Color.FromArgb(160, 160, 160);
            nightControlBox1.Location = new Point(928, 0);
            nightControlBox1.MaximizeHoverColor = Color.FromArgb(15, 255, 255, 255);
            nightControlBox1.MaximizeHoverForeColor = Color.White;
            nightControlBox1.MinimizeHoverColor = Color.FromArgb(15, 255, 255, 255);
            nightControlBox1.MinimizeHoverForeColor = Color.White;
            nightControlBox1.Name = "nightControlBox1";
            nightControlBox1.Size = new Size(139, 31);
            nightControlBox1.TabIndex = 0;
            // 
            // panelAttendancePortal
            // 
            panelAttendancePortal.Anchor = AnchorStyles.None;
            panelAttendancePortal.BackColor = Color.FromArgb(17, 34, 64);
            panelAttendancePortal.Controls.Add(lblOvertime);
            panelAttendancePortal.Controls.Add(lblTimeOut);
            panelAttendancePortal.Controls.Add(lblTimeIn);
            panelAttendancePortal.Controls.Add(lblStatus);
            panelAttendancePortal.Controls.Add(lblEmpName);
            panelAttendancePortal.Controls.Add(lblDatetime);
            panelAttendancePortal.Controls.Add(panel1);
            panelAttendancePortal.Controls.Add(lblEmployeeID);
            panelAttendancePortal.Controls.Add(txtbEmpID);
            panelAttendancePortal.Controls.Add(panelScanID);
            panelAttendancePortal.Location = new Point(114, 103);
            panelAttendancePortal.Name = "panelAttendancePortal";
            panelAttendancePortal.Size = new Size(849, 583);
            panelAttendancePortal.TabIndex = 21;
            // 
            // lblTimeOut
            // 
            lblTimeOut.AutoSize = true;
            lblTimeOut.BackColor = Color.Transparent;
            lblTimeOut.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimeOut.ForeColor = Color.White;
            lblTimeOut.Location = new Point(81, 373);
            lblTimeOut.Name = "lblTimeOut";
            lblTimeOut.Size = new Size(140, 32);
            lblTimeOut.TabIndex = 34;
            lblTimeOut.Text = "TimeOut:";
            // 
            // lblTimeIn
            // 
            lblTimeIn.AutoSize = true;
            lblTimeIn.BackColor = Color.Transparent;
            lblTimeIn.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTimeIn.ForeColor = Color.White;
            lblTimeIn.Location = new Point(104, 326);
            lblTimeIn.Name = "lblTimeIn";
            lblTimeIn.Size = new Size(117, 32);
            lblTimeIn.TabIndex = 33;
            lblTimeIn.Text = "TimeIn:";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(110, 278);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(111, 32);
            lblStatus.TabIndex = 30;
            lblStatus.Text = "Status:";
            // 
            // lblEmpName
            // 
            lblEmpName.AutoSize = true;
            lblEmpName.BackColor = Color.Transparent;
            lblEmpName.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEmpName.ForeColor = Color.White;
            lblEmpName.Location = new Point(118, 228);
            lblEmpName.Name = "lblEmpName";
            lblEmpName.Size = new Size(103, 32);
            lblEmpName.TabIndex = 29;
            lblEmpName.Text = "Name:";
            // 
            // lblDatetime
            // 
            lblDatetime.AutoSize = true;
            lblDatetime.BackColor = Color.Transparent;
            lblDatetime.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDatetime.ForeColor = Color.White;
            lblDatetime.Location = new Point(28, 24);
            lblDatetime.Name = "lblDatetime";
            lblDatetime.Size = new Size(31, 32);
            lblDatetime.TabIndex = 28;
            lblDatetime.Text = "0";
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(17, 34, 64);
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(btnBcktoLogin);
            panel1.Font = new Font("Microsoft Sans Serif", 8.25F);
            panel1.Location = new Point(188, 487);
            panel1.Name = "panel1";
            panel1.Size = new Size(499, 56);
            panel1.TabIndex = 27;
            // 
            // btnBcktoLogin
            // 
            btnBcktoLogin.BackColor = Color.Gold;
            btnBcktoLogin.Cursor = Cursors.Hand;
            btnBcktoLogin.FlatAppearance.BorderSize = 0;
            btnBcktoLogin.FlatStyle = FlatStyle.Flat;
            btnBcktoLogin.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBcktoLogin.ForeColor = Color.Black;
            btnBcktoLogin.Image = (Image)resources.GetObject("btnBcktoLogin.Image");
            btnBcktoLogin.ImageAlign = ContentAlignment.MiddleLeft;
            btnBcktoLogin.Location = new Point(-14, -21);
            btnBcktoLogin.Name = "btnBcktoLogin";
            btnBcktoLogin.Padding = new Padding(25, 0, 0, 0);
            btnBcktoLogin.Size = new Size(570, 92);
            btnBcktoLogin.TabIndex = 13;
            btnBcktoLogin.Text = "                      BACK TO LOGIN";
            btnBcktoLogin.TextAlign = ContentAlignment.MiddleLeft;
            btnBcktoLogin.UseVisualStyleBackColor = false;
            btnBcktoLogin.Click += btnBcktoLogin_Click;
            // 
            // lblEmployeeID
            // 
            lblEmployeeID.AutoSize = true;
            lblEmployeeID.BackColor = Color.Transparent;
            lblEmployeeID.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEmployeeID.ForeColor = Color.White;
            lblEmployeeID.Location = new Point(48, 135);
            lblEmployeeID.Name = "lblEmployeeID";
            lblEmployeeID.Size = new Size(180, 32);
            lblEmployeeID.TabIndex = 23;
            lblEmployeeID.Text = "EmployeeID";
            // 
            // txtbEmpID
            // 
            txtbEmpID.Font = new Font("Segoe UI", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtbEmpID.Location = new Point(234, 122);
            txtbEmpID.Multiline = true;
            txtbEmpID.Name = "txtbEmpID";
            txtbEmpID.Size = new Size(334, 56);
            txtbEmpID.TabIndex = 20;
            // 
            // panelScanID
            // 
            panelScanID.BackColor = Color.FromArgb(17, 34, 64);
            panelScanID.BorderStyle = BorderStyle.Fixed3D;
            panelScanID.Controls.Add(btnScanID);
            panelScanID.Font = new Font("Microsoft Sans Serif", 8.25F);
            panelScanID.Location = new Point(588, 122);
            panelScanID.Name = "panelScanID";
            panelScanID.Size = new Size(206, 56);
            panelScanID.TabIndex = 19;
            // 
            // btnScanID
            // 
            btnScanID.BackColor = Color.Gold;
            btnScanID.Cursor = Cursors.Hand;
            btnScanID.FlatAppearance.BorderSize = 0;
            btnScanID.FlatStyle = FlatStyle.Flat;
            btnScanID.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnScanID.ForeColor = Color.Black;
            btnScanID.ImageAlign = ContentAlignment.MiddleLeft;
            btnScanID.Location = new Point(-19, -19);
            btnScanID.Name = "btnScanID";
            btnScanID.Padding = new Padding(25, 0, 0, 0);
            btnScanID.Size = new Size(254, 92);
            btnScanID.TabIndex = 13;
            btnScanID.Text = "        SCAN";
            btnScanID.TextAlign = ContentAlignment.MiddleLeft;
            btnScanID.UseVisualStyleBackColor = false;
            btnScanID.Click += btnScanID_Click;
            // 
            // lblAttendancePortal
            // 
            lblAttendancePortal.AutoSize = true;
            lblAttendancePortal.BackColor = Color.Transparent;
            lblAttendancePortal.Font = new Font("Arial Rounded MT Bold", 19.8000011F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAttendancePortal.ForeColor = Color.White;
            lblAttendancePortal.Location = new Point(376, 40);
            lblAttendancePortal.Name = "lblAttendancePortal";
            lblAttendancePortal.Size = new Size(317, 39);
            lblAttendancePortal.TabIndex = 22;
            lblAttendancePortal.Text = "Attendance Portal";
            // 
            // timerAttendance
            // 
            timerAttendance.Enabled = true;
            timerAttendance.Interval = 1000;
            timerAttendance.Tick += timerAttendance_Tick;
            // 
            // lblOvertime
            // 
            lblOvertime.AutoSize = true;
            lblOvertime.BackColor = Color.Transparent;
            lblOvertime.Font = new Font("Arial Rounded MT Bold", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblOvertime.ForeColor = Color.White;
            lblOvertime.Location = new Point(72, 415);
            lblOvertime.Name = "lblOvertime";
            lblOvertime.Size = new Size(357, 32);
            lblOvertime.TabIndex = 35;
            lblOvertime.Text = "Overtime: Not calculated";
            // 
            // AttendancePortal
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(10, 25, 47);
            ClientSize = new Size(1079, 716);
            Controls.Add(lblAttendancePortal);
            Controls.Add(panelAttendancePortal);
            Controls.Add(nightControlBox1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "AttendancePortal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AttendancePortal";
            Paint += AttendancePortal_Paint;
            panelAttendancePortal.ResumeLayout(false);
            panelAttendancePortal.PerformLayout();
            panel1.ResumeLayout(false);
            panelScanID.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ReaLTaiizor.Controls.NightControlBox nightControlBox1;
        private Panel panelAttendancePortal;
        private Label lblAttendancePortal;
        private Panel panelScanID;
        private Button btnScanID;
        private TextBox txtbEmpID;
        private Label lblEmployeeID;
        private Panel panel1;
        private Button btnBcktoLogin;
        private Label lblTimeScan;
        private Label lblstat;
        private System.Windows.Forms.Timer timerAttendance;
        private Label lblDatetime;
        private Label lblStatus;
        private Label lblEmpName;
        private Label lblTimeOut;
        private Label lblTimeIn;
        private Label lblOvertime;
    }
}