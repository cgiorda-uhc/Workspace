namespace PhysicianFeedbackTracker
{
    partial class frmPatientDetailGenerator
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tlpDetailGeneratorMain = new System.Windows.Forms.TableLayoutPanel();
            this.grpSelectProject = new System.Windows.Forms.GroupBox();
            this.cmbPhase = new System.Windows.Forms.ComboBox();
            this.grpMPINList = new System.Windows.Forms.GroupBox();
            this.txtMPIN = new System.Windows.Forms.TextBox();
            this.chkCleanExcel = new System.Windows.Forms.CheckBox();
            this.tlpOptions = new System.Windows.Forms.TableLayoutPanel();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkGenerateEmail = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.tlpDetailGeneratorMain.SuspendLayout();
            this.grpSelectProject.SuspendLayout();
            this.grpMPINList.SuspendLayout();
            this.tlpOptions.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tlpDetailGeneratorMain, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtStatus, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.89708F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80.10291F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(640, 583);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tlpDetailGeneratorMain
            // 
            this.tlpDetailGeneratorMain.ColumnCount = 2;
            this.tlpDetailGeneratorMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.625F));
            this.tlpDetailGeneratorMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 69.375F));
            this.tlpDetailGeneratorMain.Controls.Add(this.grpSelectProject, 0, 0);
            this.tlpDetailGeneratorMain.Controls.Add(this.grpMPINList, 1, 0);
            this.tlpDetailGeneratorMain.Controls.Add(this.chkCleanExcel, 0, 1);
            this.tlpDetailGeneratorMain.Controls.Add(this.tlpOptions, 1, 1);
            this.tlpDetailGeneratorMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDetailGeneratorMain.Location = new System.Drawing.Point(3, 3);
            this.tlpDetailGeneratorMain.Name = "tlpDetailGeneratorMain";
            this.tlpDetailGeneratorMain.RowCount = 2;
            this.tlpDetailGeneratorMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 69.07217F));
            this.tlpDetailGeneratorMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30.92784F));
            this.tlpDetailGeneratorMain.Size = new System.Drawing.Size(634, 109);
            this.tlpDetailGeneratorMain.TabIndex = 1;
            // 
            // grpSelectProject
            // 
            this.grpSelectProject.Controls.Add(this.cmbPhase);
            this.grpSelectProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelectProject.Location = new System.Drawing.Point(3, 3);
            this.grpSelectProject.Name = "grpSelectProject";
            this.grpSelectProject.Size = new System.Drawing.Size(188, 69);
            this.grpSelectProject.TabIndex = 0;
            this.grpSelectProject.TabStop = false;
            this.grpSelectProject.Text = "Select Project";
            // 
            // cmbPhase
            // 
            this.cmbPhase.FormattingEnabled = true;
            this.cmbPhase.Location = new System.Drawing.Point(6, 19);
            this.cmbPhase.Name = "cmbPhase";
            this.cmbPhase.Size = new System.Drawing.Size(176, 21);
            this.cmbPhase.TabIndex = 5;
            // 
            // grpMPINList
            // 
            this.grpMPINList.Controls.Add(this.txtMPIN);
            this.grpMPINList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMPINList.Location = new System.Drawing.Point(197, 3);
            this.grpMPINList.Name = "grpMPINList";
            this.grpMPINList.Size = new System.Drawing.Size(434, 69);
            this.grpMPINList.TabIndex = 1;
            this.grpMPINList.TabStop = false;
            this.grpMPINList.Text = "Enter MPIN(s)";
            // 
            // txtMPIN
            // 
            this.txtMPIN.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMPIN.Location = new System.Drawing.Point(3, 16);
            this.txtMPIN.Multiline = true;
            this.txtMPIN.Name = "txtMPIN";
            this.txtMPIN.Size = new System.Drawing.Size(428, 50);
            this.txtMPIN.TabIndex = 0;
            // 
            // chkCleanExcel
            // 
            this.chkCleanExcel.AutoSize = true;
            this.chkCleanExcel.Location = new System.Drawing.Point(3, 78);
            this.chkCleanExcel.Name = "chkCleanExcel";
            this.chkCleanExcel.Size = new System.Drawing.Size(131, 17);
            this.chkCleanExcel.TabIndex = 3;
            this.chkCleanExcel.Text = "Clean Excel Instances";
            this.chkCleanExcel.UseVisualStyleBackColor = true;
            // 
            // tlpOptions
            // 
            this.tlpOptions.ColumnCount = 3;
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tlpOptions.Controls.Add(this.btnSubmit, 3, 0);
            this.tlpOptions.Controls.Add(this.chkGenerateEmail, 0, 0);
            this.tlpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpOptions.Location = new System.Drawing.Point(197, 78);
            this.tlpOptions.Name = "tlpOptions";
            this.tlpOptions.RowCount = 1;
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tlpOptions.Size = new System.Drawing.Size(434, 28);
            this.tlpOptions.TabIndex = 4;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSubmit.Location = new System.Drawing.Point(297, 3);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(134, 22);
            this.btnSubmit.TabIndex = 2;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(3, 118);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(634, 462);
            this.txtStatus.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(640, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // chkGenerateEmail
            // 
            this.chkGenerateEmail.AutoSize = true;
            this.chkGenerateEmail.Checked = true;
            this.chkGenerateEmail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGenerateEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkGenerateEmail.Location = new System.Drawing.Point(3, 3);
            this.chkGenerateEmail.Name = "chkGenerateEmail";
            this.chkGenerateEmail.Size = new System.Drawing.Size(141, 22);
            this.chkGenerateEmail.TabIndex = 3;
            this.chkGenerateEmail.Text = "Generate Email";
            this.chkGenerateEmail.UseVisualStyleBackColor = true;
            // 
            // frmPatientDetailGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 607);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPatientDetailGenerator";
            this.Text = "Patient Detail Generator";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPatientDetailGenerator_FormClosing);
            this.Load += new System.EventHandler(this.frmPatientDetailGenerator_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tlpDetailGeneratorMain.ResumeLayout(false);
            this.tlpDetailGeneratorMain.PerformLayout();
            this.grpSelectProject.ResumeLayout(false);
            this.grpMPINList.ResumeLayout(false);
            this.grpMPINList.PerformLayout();
            this.tlpOptions.ResumeLayout(false);
            this.tlpOptions.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tlpDetailGeneratorMain;
        private System.Windows.Forms.GroupBox grpSelectProject;
        private System.Windows.Forms.ComboBox cmbPhase;
        private System.Windows.Forms.GroupBox grpMPINList;
        private System.Windows.Forms.TextBox txtMPIN;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkCleanExcel;
        private System.Windows.Forms.TableLayoutPanel tlpOptions;
        private System.Windows.Forms.CheckBox chkGenerateEmail;
    }
}