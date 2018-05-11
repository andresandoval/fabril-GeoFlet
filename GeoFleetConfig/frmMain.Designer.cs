namespace GeoFleetConfig {
    partial class frmMain {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.trayNotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLog = new System.Windows.Forms.LinkLabel();
            this.label11 = new System.Windows.Forms.Label();
            this.txtEnterpriceName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtVehicleImei = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkVehicleInactive = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbxVehicleMin = new System.Windows.Forms.ComboBox();
            this.cbxVehicleDay = new System.Windows.Forms.ComboBox();
            this.cbxVehicleHour = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dateHistoryStartSyncDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.chkHistoryInactive = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.cbxHistoryMin = new System.Windows.Forms.ComboBox();
            this.cbxHistoryDay = new System.Windows.Forms.ComboBox();
            this.cbxHistoryHour = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblServiceStatus = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // trayNotifyIcon
            // 
            this.trayNotifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.trayNotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayNotifyIcon.Icon")));
            this.trayNotifyIcon.Text = "##";
            this.trayNotifyIcon.DoubleClick += new System.EventHandler(this.trayNotifyIcon_DoubleClick);
            this.trayNotifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.trayNotifyIcon_MouseMove);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.linkLog);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.txtEnterpriceName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtToken);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(519, 104);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // linkLog
            // 
            this.linkLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLog.AutoSize = true;
            this.linkLog.Location = new System.Drawing.Point(82, 80);
            this.linkLog.Name = "linkLog";
            this.linkLog.Size = new System.Drawing.Size(54, 15);
            this.linkLog.TabIndex = 3;
            this.linkLog.TabStop = true;
            this.linkLog.Text = "Ninguno";
            this.linkLog.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLog_LinkClicked);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(35, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 15);
            this.label11.TabIndex = 4;
            this.label11.Text = " Log:";
            // 
            // txtEnterpriceName
            // 
            this.txtEnterpriceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEnterpriceName.Location = new System.Drawing.Point(83, 50);
            this.txtEnterpriceName.Name = "txtEnterpriceName";
            this.txtEnterpriceName.Size = new System.Drawing.Size(410, 21);
            this.txtEnterpriceName.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Empresa:";
            // 
            // txtToken
            // 
            this.txtToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToken.Location = new System.Drawing.Point(83, 23);
            this.txtToken.Name = "txtToken";
            this.txtToken.Size = new System.Drawing.Size(410, 21);
            this.txtToken.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Token:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtVehicleImei);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.chkVehicleInactive);
            this.groupBox2.Controls.Add(this.groupBox3);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox2.Location = new System.Drawing.Point(16, 124);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(519, 143);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vehiculos";
            // 
            // txtVehicleImei
            // 
            this.txtVehicleImei.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtVehicleImei.Location = new System.Drawing.Point(83, 86);
            this.txtVehicleImei.Multiline = true;
            this.txtVehicleImei.Name = "txtVehicleImei";
            this.txtVehicleImei.Size = new System.Drawing.Size(410, 44);
            this.txtVehicleImei.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "IMEI:";
            // 
            // chkVehicleInactive
            // 
            this.chkVehicleInactive.AutoSize = true;
            this.chkVehicleInactive.Location = new System.Drawing.Point(426, 41);
            this.chkVehicleInactive.Name = "chkVehicleInactive";
            this.chkVehicleInactive.Size = new System.Drawing.Size(67, 19);
            this.chkVehicleInactive.TabIndex = 7;
            this.chkVehicleInactive.Text = "Inactivo";
            this.chkVehicleInactive.UseVisualStyleBackColor = true;
            this.chkVehicleInactive.CheckedChanged += new System.EventHandler(this.chkVehicleInactive_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbxVehicleMin);
            this.groupBox3.Controls.Add(this.cbxVehicleDay);
            this.groupBox3.Controls.Add(this.cbxVehicleHour);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(385, 54);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Intervalo de sincronizacion:";
            // 
            // cbxVehicleMin
            // 
            this.cbxVehicleMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVehicleMin.FormattingEnabled = true;
            this.cbxVehicleMin.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.cbxVehicleMin.Location = new System.Drawing.Point(303, 21);
            this.cbxVehicleMin.Name = "cbxVehicleMin";
            this.cbxVehicleMin.Size = new System.Drawing.Size(63, 23);
            this.cbxVehicleMin.TabIndex = 6;
            // 
            // cbxVehicleDay
            // 
            this.cbxVehicleDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVehicleDay.FormattingEnabled = true;
            this.cbxVehicleDay.Location = new System.Drawing.Point(59, 21);
            this.cbxVehicleDay.Name = "cbxVehicleDay";
            this.cbxVehicleDay.Size = new System.Drawing.Size(63, 23);
            this.cbxVehicleDay.TabIndex = 4;
            // 
            // cbxVehicleHour
            // 
            this.cbxVehicleHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxVehicleHour.FormattingEnabled = true;
            this.cbxVehicleHour.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.cbxVehicleHour.Location = new System.Drawing.Point(187, 21);
            this.cbxVehicleHour.Name = "cbxVehicleHour";
            this.cbxVehicleHour.Size = new System.Drawing.Size(63, 23);
            this.cbxVehicleHour.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Dias:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(261, 25);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 15);
            this.label6.TabIndex = 9;
            this.label6.Text = "Min:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(133, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "Horas:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dateHistoryStartSyncDate);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.chkHistoryInactive);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.groupBox4.Location = new System.Drawing.Point(16, 275);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(519, 143);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Historial";
            // 
            // dateHistoryStartSyncDate
            // 
            this.dateHistoryStartSyncDate.Location = new System.Drawing.Point(111, 101);
            this.dateHistoryStartSyncDate.Name = "dateHistoryStartSyncDate";
            this.dateHistoryStartSyncDate.Size = new System.Drawing.Size(286, 21);
            this.dateHistoryStartSyncDate.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(25, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 15);
            this.label7.TabIndex = 5;
            this.label7.Text = "Fecha inicial:";
            // 
            // chkHistoryInactive
            // 
            this.chkHistoryInactive.AutoSize = true;
            this.chkHistoryInactive.Location = new System.Drawing.Point(426, 38);
            this.chkHistoryInactive.Name = "chkHistoryInactive";
            this.chkHistoryInactive.Size = new System.Drawing.Size(67, 19);
            this.chkHistoryInactive.TabIndex = 12;
            this.chkHistoryInactive.Text = "Inactivo";
            this.chkHistoryInactive.UseVisualStyleBackColor = true;
            this.chkHistoryInactive.CheckedChanged += new System.EventHandler(this.chkHistoryInactive_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.cbxHistoryMin);
            this.groupBox5.Controls.Add(this.cbxHistoryDay);
            this.groupBox5.Controls.Add(this.cbxHistoryHour);
            this.groupBox5.Controls.Add(this.label8);
            this.groupBox5.Controls.Add(this.label9);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Location = new System.Drawing.Point(12, 20);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(385, 54);
            this.groupBox5.TabIndex = 3;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Intervalo de sincronizacion:";
            // 
            // cbxHistoryMin
            // 
            this.cbxHistoryMin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHistoryMin.FormattingEnabled = true;
            this.cbxHistoryMin.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23",
            "24",
            "25",
            "26",
            "27",
            "28",
            "29",
            "30",
            "31",
            "32",
            "33",
            "34",
            "35",
            "36",
            "37",
            "38",
            "39",
            "40",
            "41",
            "42",
            "43",
            "44",
            "45",
            "46",
            "47",
            "48",
            "49",
            "50",
            "51",
            "52",
            "53",
            "54",
            "55",
            "56",
            "57",
            "58",
            "59"});
            this.cbxHistoryMin.Location = new System.Drawing.Point(303, 21);
            this.cbxHistoryMin.Name = "cbxHistoryMin";
            this.cbxHistoryMin.Size = new System.Drawing.Size(63, 23);
            this.cbxHistoryMin.TabIndex = 11;
            // 
            // cbxHistoryDay
            // 
            this.cbxHistoryDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHistoryDay.FormattingEnabled = true;
            this.cbxHistoryDay.Location = new System.Drawing.Point(59, 21);
            this.cbxHistoryDay.Name = "cbxHistoryDay";
            this.cbxHistoryDay.Size = new System.Drawing.Size(63, 23);
            this.cbxHistoryDay.TabIndex = 9;
            // 
            // cbxHistoryHour
            // 
            this.cbxHistoryHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxHistoryHour.FormattingEnabled = true;
            this.cbxHistoryHour.Items.AddRange(new object[] {
            "0",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12",
            "13",
            "14",
            "15",
            "16",
            "17",
            "18",
            "19",
            "20",
            "21",
            "22",
            "23"});
            this.cbxHistoryHour.Location = new System.Drawing.Point(187, 21);
            this.cbxHistoryHour.Name = "cbxHistoryHour";
            this.cbxHistoryHour.Size = new System.Drawing.Size(63, 23);
            this.cbxHistoryHour.TabIndex = 10;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 15);
            this.label8.TabIndex = 4;
            this.label8.Text = "Dias:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(261, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 15);
            this.label9.TabIndex = 9;
            this.label9.Text = "Min:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(133, 25);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(43, 15);
            this.label10.TabIndex = 6;
            this.label10.Text = "Horas:";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem1.Text = "Aplicar";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem2.Text = "Aplicar y cerrar";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnApply);
            this.panel1.Controls.Add(this.lblServiceStatus);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 38);
            this.panel1.TabIndex = 4;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Location = new System.Drawing.Point(391, 6);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 27);
            this.btnApply.TabIndex = 14;
            this.btnApply.Text = "&Aplicar";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // lblServiceStatus
            // 
            this.lblServiceStatus.AutoSize = true;
            this.lblServiceStatus.Location = new System.Drawing.Point(6, 12);
            this.lblServiceStatus.Name = "lblServiceStatus";
            this.lblServiceStatus.Size = new System.Drawing.Size(0, 15);
            this.lblServiceStatus.TabIndex = 0;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(471, 6);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 27);
            this.btnClose.TabIndex = 15;
            this.btnClose.Text = "&Cerrar";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 467);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "Interfaz de configuración";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayNotifyIcon;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtEnterpriceName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbxVehicleMin;
        private System.Windows.Forms.ComboBox cbxVehicleHour;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbxVehicleDay;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtVehicleImei;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkVehicleInactive;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkHistoryInactive;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cbxHistoryMin;
        private System.Windows.Forms.ComboBox cbxHistoryDay;
        private System.Windows.Forms.ComboBox cbxHistoryHour;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.DateTimePicker dateHistoryStartSyncDate;
        private System.Windows.Forms.LinkLabel linkLog;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Label lblServiceStatus;
        private System.Windows.Forms.Button btnClose;

    }
}

