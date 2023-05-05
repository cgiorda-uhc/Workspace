namespace PhysicianFeedbackTracker
{
    partial class frmSelectTrackingItem
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
            this.tlpEditTrackingMain = new System.Windows.Forms.TableLayoutPanel();
            this.dgvTrackingItems = new System.Windows.Forms.DataGridView();
            this.tlpFilters = new System.Windows.Forms.TableLayoutPanel();
            this.grpSelectUser = new System.Windows.Forms.GroupBox();
            this.clbSelectUser = new System.Windows.Forms.CheckedListBox();
            this.grpSelectProject = new System.Windows.Forms.GroupBox();
            this.cmbPhase = new System.Windows.Forms.ComboBox();
            this.tlpDateFilter = new System.Windows.Forms.TableLayoutPanel();
            this.grpEndDate = new System.Windows.Forms.GroupBox();
            this.dtpEndDate = new System.Windows.Forms.DateTimePicker();
            this.grpStartDate = new System.Windows.Forms.GroupBox();
            this.dtpStartDate = new System.Windows.Forms.DateTimePicker();
            this.tlpSearchButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnClearFilters = new System.Windows.Forms.Button();
            this.tlpProviderName = new System.Windows.Forms.TableLayoutPanel();
            this.grpProviderSearch = new System.Windows.Forms.GroupBox();
            this.txtProviderSearch = new System.Windows.Forms.TextBox();
            this.grpTrackerStatus = new System.Windows.Forms.GroupBox();
            this.cbxTrackerStatus = new System.Windows.Forms.ComboBox();
            this.tlpInquiryCategory = new System.Windows.Forms.TableLayoutPanel();
            this.grpInquiryCategory = new System.Windows.Forms.GroupBox();
            this.cbxInquiryCategory = new System.Windows.Forms.ComboBox();
            this.grpInquiryStatus = new System.Windows.Forms.GroupBox();
            this.cbxInquiryStatus = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.addProvidersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToExcelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memberDetailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.excelParserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.qACompanionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iLUCAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aPRDRGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpEditTrackingMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrackingItems)).BeginInit();
            this.tlpFilters.SuspendLayout();
            this.grpSelectUser.SuspendLayout();
            this.grpSelectProject.SuspendLayout();
            this.tlpDateFilter.SuspendLayout();
            this.grpEndDate.SuspendLayout();
            this.grpStartDate.SuspendLayout();
            this.tlpSearchButtons.SuspendLayout();
            this.tlpProviderName.SuspendLayout();
            this.grpProviderSearch.SuspendLayout();
            this.grpTrackerStatus.SuspendLayout();
            this.tlpInquiryCategory.SuspendLayout();
            this.grpInquiryCategory.SuspendLayout();
            this.grpInquiryStatus.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpEditTrackingMain
            // 
            this.tlpEditTrackingMain.ColumnCount = 1;
            this.tlpEditTrackingMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpEditTrackingMain.Controls.Add(this.dgvTrackingItems, 0, 1);
            this.tlpEditTrackingMain.Controls.Add(this.tlpFilters, 0, 0);
            this.tlpEditTrackingMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEditTrackingMain.Location = new System.Drawing.Point(0, 24);
            this.tlpEditTrackingMain.Name = "tlpEditTrackingMain";
            this.tlpEditTrackingMain.RowCount = 3;
            this.tlpEditTrackingMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.tlpEditTrackingMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83F));
            this.tlpEditTrackingMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2F));
            this.tlpEditTrackingMain.Size = new System.Drawing.Size(1221, 754);
            this.tlpEditTrackingMain.TabIndex = 1;
            // 
            // dgvTrackingItems
            // 
            this.dgvTrackingItems.AllowUserToAddRows = false;
            this.dgvTrackingItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTrackingItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTrackingItems.Location = new System.Drawing.Point(3, 116);
            this.dgvTrackingItems.Name = "dgvTrackingItems";
            this.dgvTrackingItems.ReadOnly = true;
            this.dgvTrackingItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTrackingItems.Size = new System.Drawing.Size(1215, 619);
            this.dgvTrackingItems.TabIndex = 0;
            this.dgvTrackingItems.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTrackingItems_CellClick);
            this.dgvTrackingItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTrackingItems_CellDoubleClick);
            this.dgvTrackingItems.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgvTrackingItems_DataBindingComplete);
            this.dgvTrackingItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dgvTrackingItems_MouseDown);
            // 
            // tlpFilters
            // 
            this.tlpFilters.ColumnCount = 6;
            this.tlpFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.26337F));
            this.tlpFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.98765F));
            this.tlpFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpFilters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tlpFilters.Controls.Add(this.grpSelectUser, 1, 0);
            this.tlpFilters.Controls.Add(this.grpSelectProject, 0, 0);
            this.tlpFilters.Controls.Add(this.tlpDateFilter, 2, 0);
            this.tlpFilters.Controls.Add(this.tlpSearchButtons, 5, 0);
            this.tlpFilters.Controls.Add(this.tlpProviderName, 3, 0);
            this.tlpFilters.Controls.Add(this.tlpInquiryCategory, 4, 0);
            this.tlpFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFilters.Location = new System.Drawing.Point(3, 3);
            this.tlpFilters.Name = "tlpFilters";
            this.tlpFilters.RowCount = 1;
            this.tlpFilters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpFilters.Size = new System.Drawing.Size(1215, 107);
            this.tlpFilters.TabIndex = 1;
            // 
            // grpSelectUser
            // 
            this.grpSelectUser.Controls.Add(this.clbSelectUser);
            this.grpSelectUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSelectUser.Location = new System.Drawing.Point(205, 3);
            this.grpSelectUser.Name = "grpSelectUser";
            this.grpSelectUser.Size = new System.Drawing.Size(196, 101);
            this.grpSelectUser.TabIndex = 15;
            this.grpSelectUser.TabStop = false;
            this.grpSelectUser.Text = "Select User";
            // 
            // clbSelectUser
            // 
            this.clbSelectUser.CheckOnClick = true;
            this.clbSelectUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbSelectUser.FormattingEnabled = true;
            this.clbSelectUser.Location = new System.Drawing.Point(3, 16);
            this.clbSelectUser.Name = "clbSelectUser";
            this.clbSelectUser.Size = new System.Drawing.Size(190, 82);
            this.clbSelectUser.TabIndex = 10;
            this.clbSelectUser.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbSelectUser_ItemCheck);
            // 
            // grpSelectProject
            // 
            this.grpSelectProject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSelectProject.Controls.Add(this.cmbPhase);
            this.grpSelectProject.Location = new System.Drawing.Point(3, 3);
            this.grpSelectProject.Name = "grpSelectProject";
            this.grpSelectProject.Size = new System.Drawing.Size(196, 101);
            this.grpSelectProject.TabIndex = 9;
            this.grpSelectProject.TabStop = false;
            this.grpSelectProject.Text = "Select Project";
            // 
            // cmbPhase
            // 
            this.cmbPhase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPhase.FormattingEnabled = true;
            this.cmbPhase.Location = new System.Drawing.Point(13, 41);
            this.cmbPhase.Name = "cmbPhase";
            this.cmbPhase.Size = new System.Drawing.Size(177, 21);
            this.cmbPhase.TabIndex = 4;
            // 
            // tlpDateFilter
            // 
            this.tlpDateFilter.ColumnCount = 1;
            this.tlpDateFilter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpDateFilter.Controls.Add(this.grpEndDate, 0, 1);
            this.tlpDateFilter.Controls.Add(this.grpStartDate, 0, 0);
            this.tlpDateFilter.Location = new System.Drawing.Point(407, 3);
            this.tlpDateFilter.Name = "tlpDateFilter";
            this.tlpDateFilter.RowCount = 2;
            this.tlpDateFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpDateFilter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpDateFilter.Size = new System.Drawing.Size(143, 100);
            this.tlpDateFilter.TabIndex = 11;
            // 
            // grpEndDate
            // 
            this.grpEndDate.Controls.Add(this.dtpEndDate);
            this.grpEndDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpEndDate.Location = new System.Drawing.Point(3, 53);
            this.grpEndDate.Name = "grpEndDate";
            this.grpEndDate.Size = new System.Drawing.Size(137, 44);
            this.grpEndDate.TabIndex = 1;
            this.grpEndDate.TabStop = false;
            this.grpEndDate.Text = "End Date";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpEndDate.Checked = false;
            this.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpEndDate.Location = new System.Drawing.Point(6, 17);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.ShowCheckBox = true;
            this.dtpEndDate.Size = new System.Drawing.Size(111, 20);
            this.dtpEndDate.TabIndex = 1;
            // 
            // grpStartDate
            // 
            this.grpStartDate.Controls.Add(this.dtpStartDate);
            this.grpStartDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpStartDate.Location = new System.Drawing.Point(3, 3);
            this.grpStartDate.Name = "grpStartDate";
            this.grpStartDate.Size = new System.Drawing.Size(137, 44);
            this.grpStartDate.TabIndex = 0;
            this.grpStartDate.TabStop = false;
            this.grpStartDate.Text = "Start Date";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.dtpStartDate.Checked = false;
            this.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpStartDate.Location = new System.Drawing.Point(6, 17);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.ShowCheckBox = true;
            this.dtpStartDate.Size = new System.Drawing.Size(111, 20);
            this.dtpStartDate.TabIndex = 0;
            // 
            // tlpSearchButtons
            // 
            this.tlpSearchButtons.ColumnCount = 1;
            this.tlpSearchButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchButtons.Controls.Add(this.btnSearch, 0, 0);
            this.tlpSearchButtons.Controls.Add(this.btnClearFilters, 0, 1);
            this.tlpSearchButtons.Location = new System.Drawing.Point(1013, 3);
            this.tlpSearchButtons.Name = "tlpSearchButtons";
            this.tlpSearchButtons.RowCount = 2;
            this.tlpSearchButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchButtons.Size = new System.Drawing.Size(199, 100);
            this.tlpSearchButtons.TabIndex = 12;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(65, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(131, 23);
            this.btnSearch.TabIndex = 7;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClearFilters
            // 
            this.btnClearFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearFilters.Location = new System.Drawing.Point(65, 53);
            this.btnClearFilters.Name = "btnClearFilters";
            this.btnClearFilters.Size = new System.Drawing.Size(131, 23);
            this.btnClearFilters.TabIndex = 8;
            this.btnClearFilters.Text = "Clear Filters";
            this.btnClearFilters.UseVisualStyleBackColor = true;
            this.btnClearFilters.Click += new System.EventHandler(this.btnClearFilters_Click);
            // 
            // tlpProviderName
            // 
            this.tlpProviderName.ColumnCount = 1;
            this.tlpProviderName.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProviderName.Controls.Add(this.grpProviderSearch, 0, 0);
            this.tlpProviderName.Controls.Add(this.grpTrackerStatus, 0, 1);
            this.tlpProviderName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpProviderName.Location = new System.Drawing.Point(556, 3);
            this.tlpProviderName.Name = "tlpProviderName";
            this.tlpProviderName.RowCount = 2;
            this.tlpProviderName.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProviderName.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpProviderName.Size = new System.Drawing.Size(249, 101);
            this.tlpProviderName.TabIndex = 13;
            // 
            // grpProviderSearch
            // 
            this.grpProviderSearch.Controls.Add(this.txtProviderSearch);
            this.grpProviderSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProviderSearch.Location = new System.Drawing.Point(3, 3);
            this.grpProviderSearch.Name = "grpProviderSearch";
            this.grpProviderSearch.Size = new System.Drawing.Size(243, 44);
            this.grpProviderSearch.TabIndex = 0;
            this.grpProviderSearch.TabStop = false;
            this.grpProviderSearch.Text = "Provider Name/MPIN/TIN";
            // 
            // txtProviderSearch
            // 
            this.txtProviderSearch.Location = new System.Drawing.Point(7, 17);
            this.txtProviderSearch.Name = "txtProviderSearch";
            this.txtProviderSearch.Size = new System.Drawing.Size(230, 20);
            this.txtProviderSearch.TabIndex = 0;
            // 
            // grpTrackerStatus
            // 
            this.grpTrackerStatus.Controls.Add(this.cbxTrackerStatus);
            this.grpTrackerStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTrackerStatus.Location = new System.Drawing.Point(3, 53);
            this.grpTrackerStatus.Name = "grpTrackerStatus";
            this.grpTrackerStatus.Size = new System.Drawing.Size(243, 45);
            this.grpTrackerStatus.TabIndex = 1;
            this.grpTrackerStatus.TabStop = false;
            this.grpTrackerStatus.Text = "Tracker Status";
            // 
            // cbxTrackerStatus
            // 
            this.cbxTrackerStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxTrackerStatus.FormattingEnabled = true;
            this.cbxTrackerStatus.Location = new System.Drawing.Point(7, 19);
            this.cbxTrackerStatus.Name = "cbxTrackerStatus";
            this.cbxTrackerStatus.Size = new System.Drawing.Size(230, 21);
            this.cbxTrackerStatus.TabIndex = 0;
            // 
            // tlpInquiryCategory
            // 
            this.tlpInquiryCategory.ColumnCount = 1;
            this.tlpInquiryCategory.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpInquiryCategory.Controls.Add(this.grpInquiryCategory, 0, 0);
            this.tlpInquiryCategory.Controls.Add(this.grpInquiryStatus, 0, 1);
            this.tlpInquiryCategory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInquiryCategory.Location = new System.Drawing.Point(811, 3);
            this.tlpInquiryCategory.Name = "tlpInquiryCategory";
            this.tlpInquiryCategory.RowCount = 2;
            this.tlpInquiryCategory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpInquiryCategory.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpInquiryCategory.Size = new System.Drawing.Size(196, 101);
            this.tlpInquiryCategory.TabIndex = 16;
            // 
            // grpInquiryCategory
            // 
            this.grpInquiryCategory.Controls.Add(this.cbxInquiryCategory);
            this.grpInquiryCategory.Location = new System.Drawing.Point(3, 3);
            this.grpInquiryCategory.Name = "grpInquiryCategory";
            this.grpInquiryCategory.Size = new System.Drawing.Size(190, 44);
            this.grpInquiryCategory.TabIndex = 0;
            this.grpInquiryCategory.TabStop = false;
            this.grpInquiryCategory.Text = "Inquiry Category";
            // 
            // cbxInquiryCategory
            // 
            this.cbxInquiryCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxInquiryCategory.FormattingEnabled = true;
            this.cbxInquiryCategory.Location = new System.Drawing.Point(7, 17);
            this.cbxInquiryCategory.Name = "cbxInquiryCategory";
            this.cbxInquiryCategory.Size = new System.Drawing.Size(177, 21);
            this.cbxInquiryCategory.TabIndex = 0;
            // 
            // grpInquiryStatus
            // 
            this.grpInquiryStatus.Controls.Add(this.cbxInquiryStatus);
            this.grpInquiryStatus.Location = new System.Drawing.Point(3, 53);
            this.grpInquiryStatus.Name = "grpInquiryStatus";
            this.grpInquiryStatus.Size = new System.Drawing.Size(190, 45);
            this.grpInquiryStatus.TabIndex = 1;
            this.grpInquiryStatus.TabStop = false;
            this.grpInquiryStatus.Text = "Inquiry Status";
            // 
            // cbxInquiryStatus
            // 
            this.cbxInquiryStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxInquiryStatus.FormattingEnabled = true;
            this.cbxInquiryStatus.Location = new System.Drawing.Point(7, 19);
            this.cbxInquiryStatus.Name = "cbxInquiryStatus";
            this.cbxInquiryStatus.Size = new System.Drawing.Size(177, 21);
            this.cbxInquiryStatus.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addProvidersToolStripMenuItem,
            this.exportToExcelToolStripMenuItem,
            this.detailsToolStripMenuItem,
            this.iLUCAToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1221, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // addProvidersToolStripMenuItem
            // 
            this.addProvidersToolStripMenuItem.Name = "addProvidersToolStripMenuItem";
            this.addProvidersToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.addProvidersToolStripMenuItem.Text = "Add Provider(s)";
            this.addProvidersToolStripMenuItem.Click += new System.EventHandler(this.addProvidersToolStripMenuItem_Click);
            // 
            // exportToExcelToolStripMenuItem
            // 
            this.exportToExcelToolStripMenuItem.Name = "exportToExcelToolStripMenuItem";
            this.exportToExcelToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.exportToExcelToolStripMenuItem.Text = "Export to Excel";
            this.exportToExcelToolStripMenuItem.Click += new System.EventHandler(this.exportToExcelToolStripMenuItem_Click);
            // 
            // detailsToolStripMenuItem
            // 
            this.detailsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.memberDetailsToolStripMenuItem,
            this.excelParserToolStripMenuItem,
            this.qACompanionToolStripMenuItem});
            this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
            this.detailsToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.detailsToolStripMenuItem.Text = "QA Tools";
            // 
            // memberDetailsToolStripMenuItem
            // 
            this.memberDetailsToolStripMenuItem.Name = "memberDetailsToolStripMenuItem";
            this.memberDetailsToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.memberDetailsToolStripMenuItem.Text = "Member Details";
            this.memberDetailsToolStripMenuItem.Click += new System.EventHandler(this.memberDetailsToolStripMenuItem_Click);
            // 
            // excelParserToolStripMenuItem
            // 
            this.excelParserToolStripMenuItem.Enabled = false;
            this.excelParserToolStripMenuItem.Name = "excelParserToolStripMenuItem";
            this.excelParserToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.excelParserToolStripMenuItem.Text = "Excel Parser";
            this.excelParserToolStripMenuItem.Click += new System.EventHandler(this.excelParserToolStripMenuItem_Click);
            // 
            // qACompanionToolStripMenuItem
            // 
            this.qACompanionToolStripMenuItem.Enabled = false;
            this.qACompanionToolStripMenuItem.Name = "qACompanionToolStripMenuItem";
            this.qACompanionToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.qACompanionToolStripMenuItem.Text = "QA Companion";
            this.qACompanionToolStripMenuItem.Click += new System.EventHandler(this.qACompanionToolStripMenuItem_Click);
            // 
            // iLUCAToolStripMenuItem
            // 
            this.iLUCAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aPRDRGToolStripMenuItem,
            this.dXToolStripMenuItem,
            this.pXToolStripMenuItem});
            this.iLUCAToolStripMenuItem.Name = "iLUCAToolStripMenuItem";
            this.iLUCAToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.iLUCAToolStripMenuItem.Text = "ILUCA";
            // 
            // aPRDRGToolStripMenuItem
            // 
            this.aPRDRGToolStripMenuItem.Enabled = false;
            this.aPRDRGToolStripMenuItem.Name = "aPRDRGToolStripMenuItem";
            this.aPRDRGToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.aPRDRGToolStripMenuItem.Text = "APRDRG";
            this.aPRDRGToolStripMenuItem.Click += new System.EventHandler(this.aPRDRGToolStripMenuItem_Click);
            // 
            // dXToolStripMenuItem
            // 
            this.dXToolStripMenuItem.Enabled = false;
            this.dXToolStripMenuItem.Name = "dXToolStripMenuItem";
            this.dXToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.dXToolStripMenuItem.Text = "DX";
            this.dXToolStripMenuItem.Click += new System.EventHandler(this.dXToolStripMenuItem_Click);
            // 
            // pXToolStripMenuItem
            // 
            this.pXToolStripMenuItem.Name = "pXToolStripMenuItem";
            this.pXToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.pXToolStripMenuItem.Text = "PX";
            this.pXToolStripMenuItem.Click += new System.EventHandler(this.pXToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(101, 20);
            this.exitToolStripMenuItem.Text = "Exit Application";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // frmSelectTrackingItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1221, 778);
            this.Controls.Add(this.tlpEditTrackingMain);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmSelectTrackingItem";
            this.Text = "Select Tracking Items";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.EditTrackingItem_Load);
            this.tlpEditTrackingMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTrackingItems)).EndInit();
            this.tlpFilters.ResumeLayout(false);
            this.grpSelectUser.ResumeLayout(false);
            this.grpSelectProject.ResumeLayout(false);
            this.tlpDateFilter.ResumeLayout(false);
            this.grpEndDate.ResumeLayout(false);
            this.grpStartDate.ResumeLayout(false);
            this.tlpSearchButtons.ResumeLayout(false);
            this.tlpProviderName.ResumeLayout(false);
            this.grpProviderSearch.ResumeLayout(false);
            this.grpProviderSearch.PerformLayout();
            this.grpTrackerStatus.ResumeLayout(false);
            this.tlpInquiryCategory.ResumeLayout(false);
            this.grpInquiryCategory.ResumeLayout(false);
            this.grpInquiryStatus.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTrackingItems;
        private System.Windows.Forms.TableLayoutPanel tlpEditTrackingMain;
        private System.Windows.Forms.TableLayoutPanel tlpFilters;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox grpSelectProject;
        private System.Windows.Forms.ComboBox cmbPhase;
        private System.Windows.Forms.TableLayoutPanel tlpDateFilter;
        private System.Windows.Forms.GroupBox grpEndDate;
        private System.Windows.Forms.GroupBox grpStartDate;
        private System.Windows.Forms.DateTimePicker dtpEndDate;
        private System.Windows.Forms.DateTimePicker dtpStartDate;
        private System.Windows.Forms.TableLayoutPanel tlpSearchButtons;
        private System.Windows.Forms.TableLayoutPanel tlpProviderName;
        private System.Windows.Forms.GroupBox grpProviderSearch;
        private System.Windows.Forms.TextBox txtProviderSearch;
        private System.Windows.Forms.GroupBox grpSelectUser;
        private System.Windows.Forms.CheckedListBox clbSelectUser;
        private System.Windows.Forms.GroupBox grpTrackerStatus;
        private System.Windows.Forms.ComboBox cbxTrackerStatus;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem addProvidersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToExcelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tlpInquiryCategory;
        private System.Windows.Forms.GroupBox grpInquiryCategory;
        private System.Windows.Forms.ComboBox cbxInquiryCategory;
        private System.Windows.Forms.GroupBox grpInquiryStatus;
        private System.Windows.Forms.ComboBox cbxInquiryStatus;
        private System.Windows.Forms.Button btnClearFilters;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem memberDetailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem excelParserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem qACompanionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iLUCAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aPRDRGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pXToolStripMenuItem;
    }
}