namespace PhysicianFeedbackTracker
{
    partial class frmQACompanion
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
            this.tlpQACompanionMain = new System.Windows.Forms.TableLayoutPanel();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.tlpQACompanionTop = new System.Windows.Forms.TableLayoutPanel();
            this.grpSelectProject = new System.Windows.Forms.GroupBox();
            this.cmbPhase = new System.Windows.Forms.ComboBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.grpSampling = new System.Windows.Forms.GroupBox();
            this.txtSampleSize = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpChooseMpin = new System.Windows.Forms.GroupBox();
            this.txtChooseMpin = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpButtons = new System.Windows.Forms.TableLayoutPanel();
            this.tlpQACompanionMain.SuspendLayout();
            this.tlpQACompanionTop.SuspendLayout();
            this.grpSelectProject.SuspendLayout();
            this.grpSampling.SuspendLayout();
            this.grpChooseMpin.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tlpButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpQACompanionMain
            // 
            this.tlpQACompanionMain.ColumnCount = 1;
            this.tlpQACompanionMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpQACompanionMain.Controls.Add(this.txtStatus, 0, 2);
            this.tlpQACompanionMain.Controls.Add(this.tlpQACompanionTop, 0, 1);
            this.tlpQACompanionMain.Controls.Add(this.menuStrip1, 0, 0);
            this.tlpQACompanionMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpQACompanionMain.Location = new System.Drawing.Point(0, 0);
            this.tlpQACompanionMain.Name = "tlpQACompanionMain";
            this.tlpQACompanionMain.RowCount = 4;
            this.tlpQACompanionMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tlpQACompanionMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpQACompanionMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tlpQACompanionMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5F));
            this.tlpQACompanionMain.Size = new System.Drawing.Size(978, 827);
            this.tlpQACompanionMain.TabIndex = 0;
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(3, 126);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStatus.Size = new System.Drawing.Size(972, 655);
            this.txtStatus.TabIndex = 4;
            // 
            // tlpQACompanionTop
            // 
            this.tlpQACompanionTop.ColumnCount = 5;
            this.tlpQACompanionTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.1341F));
            this.tlpQACompanionTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34.8659F));
            this.tlpQACompanionTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 420F));
            this.tlpQACompanionTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.tlpQACompanionTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 175F));
            this.tlpQACompanionTop.Controls.Add(this.grpSelectProject, 0, 0);
            this.tlpQACompanionTop.Controls.Add(this.grpSampling, 1, 0);
            this.tlpQACompanionTop.Controls.Add(this.grpChooseMpin, 2, 0);
            this.tlpQACompanionTop.Controls.Add(this.tlpButtons, 4, 0);
            this.tlpQACompanionTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpQACompanionTop.Location = new System.Drawing.Point(3, 27);
            this.tlpQACompanionTop.Name = "tlpQACompanionTop";
            this.tlpQACompanionTop.RowCount = 1;
            this.tlpQACompanionTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpQACompanionTop.Size = new System.Drawing.Size(972, 93);
            this.tlpQACompanionTop.TabIndex = 3;
            // 
            // grpSelectProject
            // 
            this.grpSelectProject.Controls.Add(this.cmbPhase);
            this.grpSelectProject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelectProject.Location = new System.Drawing.Point(3, 3);
            this.grpSelectProject.Name = "grpSelectProject";
            this.grpSelectProject.Size = new System.Drawing.Size(164, 87);
            this.grpSelectProject.TabIndex = 10;
            this.grpSelectProject.TabStop = false;
            this.grpSelectProject.Text = "Select Project";
            // 
            // cmbPhase
            // 
            this.cmbPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPhase.FormattingEnabled = true;
            this.cmbPhase.Location = new System.Drawing.Point(13, 35);
            this.cmbPhase.Name = "cmbPhase";
            this.cmbPhase.Size = new System.Drawing.Size(145, 21);
            this.cmbPhase.TabIndex = 4;
            // 
            // btnRun
            // 
            this.btnRun.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRun.Location = new System.Drawing.Point(3, 17);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(163, 23);
            this.btnRun.TabIndex = 11;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // grpSampling
            // 
            this.grpSampling.Controls.Add(this.txtSampleSize);
            this.grpSampling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSampling.Location = new System.Drawing.Point(173, 3);
            this.grpSampling.Name = "grpSampling";
            this.grpSampling.Size = new System.Drawing.Size(85, 87);
            this.grpSampling.TabIndex = 13;
            this.grpSampling.TabStop = false;
            this.grpSampling.Text = "Sample Size";
            // 
            // txtSampleSize
            // 
            this.txtSampleSize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSampleSize.Location = new System.Drawing.Point(6, 35);
            this.txtSampleSize.Name = "txtSampleSize";
            this.txtSampleSize.Size = new System.Drawing.Size(73, 20);
            this.txtSampleSize.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(3, 61);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(163, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpChooseMpin
            // 
            this.grpChooseMpin.Controls.Add(this.txtChooseMpin);
            this.grpChooseMpin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpChooseMpin.Location = new System.Drawing.Point(264, 3);
            this.grpChooseMpin.Name = "grpChooseMpin";
            this.grpChooseMpin.Size = new System.Drawing.Size(414, 87);
            this.grpChooseMpin.TabIndex = 14;
            this.grpChooseMpin.TabStop = false;
            this.grpChooseMpin.Text = "Choose MPINS";
            // 
            // txtChooseMpin
            // 
            this.txtChooseMpin.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtChooseMpin.Location = new System.Drawing.Point(3, 16);
            this.txtChooseMpin.Multiline = true;
            this.txtChooseMpin.Name = "txtChooseMpin";
            this.txtChooseMpin.Size = new System.Drawing.Size(408, 68);
            this.txtChooseMpin.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(978, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // tlpButtons
            // 
            this.tlpButtons.ColumnCount = 1;
            this.tlpButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButtons.Controls.Add(this.btnRun, 0, 0);
            this.tlpButtons.Controls.Add(this.btnCancel, 0, 1);
            this.tlpButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpButtons.Location = new System.Drawing.Point(800, 3);
            this.tlpButtons.Name = "tlpButtons";
            this.tlpButtons.RowCount = 2;
            this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpButtons.Size = new System.Drawing.Size(169, 87);
            this.tlpButtons.TabIndex = 15;
            // 
            // frmQACompanion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 827);
            this.Controls.Add(this.tlpQACompanionMain);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmQACompanion";
            this.Text = "frmQACompanion";
            this.Load += new System.EventHandler(this.frmQACompanion_Load);
            this.tlpQACompanionMain.ResumeLayout(false);
            this.tlpQACompanionMain.PerformLayout();
            this.tlpQACompanionTop.ResumeLayout(false);
            this.grpSelectProject.ResumeLayout(false);
            this.grpSampling.ResumeLayout(false);
            this.grpSampling.PerformLayout();
            this.grpChooseMpin.ResumeLayout(false);
            this.grpChooseMpin.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tlpButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpQACompanionMain;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpQACompanionTop;
        private System.Windows.Forms.GroupBox grpSelectProject;
        private System.Windows.Forms.ComboBox cmbPhase;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpSampling;
        private System.Windows.Forms.TextBox txtSampleSize;
        private System.Windows.Forms.GroupBox grpChooseMpin;
        private System.Windows.Forms.TextBox txtChooseMpin;
        private System.Windows.Forms.TableLayoutPanel tlpButtons;
    }
}