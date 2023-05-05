namespace PhysicianFeedbackTracker
{
    partial class VBC_Bundled
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
            this.tlpMainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.tlpSearchOptionsTop = new System.Windows.Forms.TableLayoutPanel();
            this.grpTins = new System.Windows.Forms.GroupBox();
            this.btnMatchTin = new System.Windows.Forms.Button();
            this.txtTIN = new System.Windows.Forms.TextBox();
            this.grpRptType = new System.Windows.Forms.GroupBox();
            this.btnSearchProvider = new System.Windows.Forms.Button();
            this.cmbBundleType = new System.Windows.Forms.ComboBox();
            this.lblBundleType = new System.Windows.Forms.Label();
            this.cmbReportingPeriod = new System.Windows.Forms.ComboBox();
            this.lblReportDate = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.complaintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpMainContainer.SuspendLayout();
            this.tlpSearchOptionsTop.SuspendLayout();
            this.grpTins.SuspendLayout();
            this.grpRptType.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMainContainer
            // 
            this.tlpMainContainer.ColumnCount = 1;
            this.tlpMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainContainer.Controls.Add(this.txtStatus, 0, 1);
            this.tlpMainContainer.Controls.Add(this.tlpSearchOptionsTop, 0, 0);
            this.tlpMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainContainer.Location = new System.Drawing.Point(0, 0);
            this.tlpMainContainer.Margin = new System.Windows.Forms.Padding(4);
            this.tlpMainContainer.Name = "tlpMainContainer";
            this.tlpMainContainer.RowCount = 2;
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.26715F));
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 61.73285F));
            this.tlpMainContainer.Size = new System.Drawing.Size(1077, 554);
            this.tlpMainContainer.TabIndex = 17;
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(4, 216);
            this.txtStatus.Margin = new System.Windows.Forms.Padding(4);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(1069, 334);
            this.txtStatus.TabIndex = 3;
            // 
            // tlpSearchOptionsTop
            // 
            this.tlpSearchOptionsTop.ColumnCount = 2;
            this.tlpSearchOptionsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 59.49485F));
            this.tlpSearchOptionsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40.50515F));
            this.tlpSearchOptionsTop.Controls.Add(this.grpTins, 0, 1);
            this.tlpSearchOptionsTop.Controls.Add(this.grpRptType, 0, 1);
            this.tlpSearchOptionsTop.Controls.Add(this.menuStrip1, 0, 0);
            this.tlpSearchOptionsTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearchOptionsTop.Location = new System.Drawing.Point(4, 4);
            this.tlpSearchOptionsTop.Margin = new System.Windows.Forms.Padding(4);
            this.tlpSearchOptionsTop.Name = "tlpSearchOptionsTop";
            this.tlpSearchOptionsTop.RowCount = 2;
            this.tlpSearchOptionsTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tlpSearchOptionsTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 156F));
            this.tlpSearchOptionsTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 209F));
            this.tlpSearchOptionsTop.Size = new System.Drawing.Size(1069, 204);
            this.tlpSearchOptionsTop.TabIndex = 0;
            // 
            // grpTins
            // 
            this.grpTins.Controls.Add(this.btnMatchTin);
            this.grpTins.Controls.Add(this.txtTIN);
            this.grpTins.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTins.Location = new System.Drawing.Point(4, 29);
            this.grpTins.Margin = new System.Windows.Forms.Padding(4);
            this.grpTins.Name = "grpTins";
            this.grpTins.Padding = new System.Windows.Forms.Padding(4);
            this.grpTins.Size = new System.Drawing.Size(627, 171);
            this.grpTins.TabIndex = 17;
            this.grpTins.TabStop = false;
            this.grpTins.Text = "Enter TIN(s)";
            // 
            // btnMatchTin
            // 
            this.btnMatchTin.Location = new System.Drawing.Point(510, 134);
            this.btnMatchTin.Name = "btnMatchTin";
            this.btnMatchTin.Size = new System.Drawing.Size(100, 30);
            this.btnMatchTin.TabIndex = 1;
            this.btnMatchTin.Text = "Get Filters";
            this.btnMatchTin.UseVisualStyleBackColor = true;
            this.btnMatchTin.Click += new System.EventHandler(this.btnMatchTin_Click);
            // 
            // txtTIN
            // 
            this.txtTIN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTIN.Location = new System.Drawing.Point(4, 19);
            this.txtTIN.Margin = new System.Windows.Forms.Padding(4);
            this.txtTIN.Multiline = true;
            this.txtTIN.Name = "txtTIN";
            this.txtTIN.Size = new System.Drawing.Size(619, 108);
            this.txtTIN.TabIndex = 0;
            this.txtTIN.Enter += new System.EventHandler(this.txtTIN_Enter);
            // 
            // grpRptType
            // 
            this.grpRptType.Controls.Add(this.btnSearchProvider);
            this.grpRptType.Controls.Add(this.cmbBundleType);
            this.grpRptType.Controls.Add(this.lblBundleType);
            this.grpRptType.Controls.Add(this.cmbReportingPeriod);
            this.grpRptType.Controls.Add(this.lblReportDate);
            this.grpRptType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpRptType.Location = new System.Drawing.Point(639, 29);
            this.grpRptType.Margin = new System.Windows.Forms.Padding(4);
            this.grpRptType.Name = "grpRptType";
            this.grpRptType.Padding = new System.Windows.Forms.Padding(4);
            this.grpRptType.Size = new System.Drawing.Size(426, 171);
            this.grpRptType.TabIndex = 16;
            this.grpRptType.TabStop = false;
            this.grpRptType.Text = "Choose Filters";
            // 
            // btnSearchProvider
            // 
            this.btnSearchProvider.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchProvider.Location = new System.Drawing.Point(53, 126);
            this.btnSearchProvider.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearchProvider.Name = "btnSearchProvider";
            this.btnSearchProvider.Size = new System.Drawing.Size(213, 28);
            this.btnSearchProvider.TabIndex = 4;
            this.btnSearchProvider.Text = "Generate PDF";
            this.btnSearchProvider.UseVisualStyleBackColor = true;
            this.btnSearchProvider.Click += new System.EventHandler(this.btnSearchProvider_Click);
            // 
            // cmbBundleType
            // 
            this.cmbBundleType.FormattingEnabled = true;
            this.cmbBundleType.Location = new System.Drawing.Point(135, 34);
            this.cmbBundleType.Margin = new System.Windows.Forms.Padding(4);
            this.cmbBundleType.Name = "cmbBundleType";
            this.cmbBundleType.Size = new System.Drawing.Size(211, 24);
            this.cmbBundleType.TabIndex = 6;
            this.cmbBundleType.SelectedIndexChanged += new System.EventHandler(this.cmbBundleType_SelectedIndexChanged);
            // 
            // lblBundleType
            // 
            this.lblBundleType.AutoSize = true;
            this.lblBundleType.Location = new System.Drawing.Point(7, 37);
            this.lblBundleType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblBundleType.Name = "lblBundleType";
            this.lblBundleType.Size = new System.Drawing.Size(88, 17);
            this.lblBundleType.TabIndex = 5;
            this.lblBundleType.Text = "BundleType:";
            // 
            // cmbReportingPeriod
            // 
            this.cmbReportingPeriod.FormattingEnabled = true;
            this.cmbReportingPeriod.Location = new System.Drawing.Point(135, 77);
            this.cmbReportingPeriod.Margin = new System.Windows.Forms.Padding(4);
            this.cmbReportingPeriod.Name = "cmbReportingPeriod";
            this.cmbReportingPeriod.Size = new System.Drawing.Size(131, 24);
            this.cmbReportingPeriod.TabIndex = 3;
            this.cmbReportingPeriod.SelectedIndexChanged += new System.EventHandler(this.cmbReportingPeriod_SelectedIndexChanged);
            // 
            // lblReportDate
            // 
            this.lblReportDate.AutoSize = true;
            this.lblReportDate.Location = new System.Drawing.Point(8, 77);
            this.lblReportDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReportDate.Name = "lblReportDate";
            this.lblReportDate.Size = new System.Drawing.Size(119, 17);
            this.lblReportDate.TabIndex = 2;
            this.lblReportDate.Text = "Reporting Period:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.complaintsToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(635, 25);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(50, 21);
            this.filesToolStripMenuItem.Text = "Files";
            this.filesToolStripMenuItem.Click += new System.EventHandler(this.filesToolStripMenuItem_Click);
            // 
            // complaintsToolStripMenuItem
            // 
            this.complaintsToolStripMenuItem.Name = "complaintsToolStripMenuItem";
            this.complaintsToolStripMenuItem.Size = new System.Drawing.Size(96, 21);
            this.complaintsToolStripMenuItem.Text = "Complaints";
            this.complaintsToolStripMenuItem.Click += new System.EventHandler(this.complaintsToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(45, 21);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // VBC_Bundled
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 554);
            this.Controls.Add(this.tlpMainContainer);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VBC_Bundled";
            this.Text = "VBC_Bundled";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VBC_Bundled_FormClosing);
            this.Load += new System.EventHandler(this.VBC_Bundled_Load);
            this.tlpMainContainer.ResumeLayout(false);
            this.tlpMainContainer.PerformLayout();
            this.tlpSearchOptionsTop.ResumeLayout(false);
            this.tlpSearchOptionsTop.PerformLayout();
            this.grpTins.ResumeLayout(false);
            this.grpTins.PerformLayout();
            this.grpRptType.ResumeLayout(false);
            this.grpRptType.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMainContainer;
        private System.Windows.Forms.TableLayoutPanel tlpSearchOptionsTop;
        private System.Windows.Forms.Button btnSearchProvider;
        private System.Windows.Forms.ComboBox cmbBundleType;
        private System.Windows.Forms.Label lblBundleType;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.GroupBox grpTins;
        private System.Windows.Forms.TextBox txtTIN;
        private System.Windows.Forms.GroupBox grpRptType;
        private System.Windows.Forms.ComboBox cmbReportingPeriod;
        private System.Windows.Forms.Label lblReportDate;
        private System.Windows.Forms.Button btnMatchTin;
        private System.Windows.Forms.ToolStripMenuItem complaintsToolStripMenuItem;
        public System.Windows.Forms.TextBox txtStatus;
    }
}