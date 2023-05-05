namespace PhysicianFeedbackTracker
{
    partial class frmAddProvders
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddProvders));
            this.cmbPhase = new System.Windows.Forms.ComboBox();
            this.lblPhase = new System.Windows.Forms.Label();
            this.lblEnterProvider = new System.Windows.Forms.Label();
            this.btnSearchProvider = new System.Windows.Forms.Button();
            this.lblTinSearch = new System.Windows.Forms.Label();
            this.lblProviderResults = new System.Windows.Forms.Label();
            this.btnSumbitProviders = new System.Windows.Forms.Button();
            this.lvTinSearch = new System.Windows.Forms.ListView();
            this.lvProviderResults = new System.Windows.Forms.ListView();
            this.grpCheckUncheck = new System.Windows.Forms.GroupBox();
            this.radSelectAll = new System.Windows.Forms.RadioButton();
            this.radDeselectAll = new System.Windows.Forms.RadioButton();
            this.tlpMainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.tlpTINSearch = new System.Windows.Forms.TableLayoutPanel();
            this.tlpSearchResultsOptions = new System.Windows.Forms.TableLayoutPanel();
            this.tlpSearchResults = new System.Windows.Forms.TableLayoutPanel();
            this.tlpSubmitExit = new System.Windows.Forms.TableLayoutPanel();
            this.chkCloneItems = new System.Windows.Forms.CheckBox();
            this.tlpSearchOptionsTop = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cmbProviderSearch = new System.Windows.Forms.ComboBox();
            this.btnProviderDetails = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grpCheckUncheck.SuspendLayout();
            this.tlpMainContainer.SuspendLayout();
            this.tlpTINSearch.SuspendLayout();
            this.tlpSearchResultsOptions.SuspendLayout();
            this.tlpSearchResults.SuspendLayout();
            this.tlpSubmitExit.SuspendLayout();
            this.tlpSearchOptionsTop.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbPhase
            // 
            this.cmbPhase.FormattingEnabled = true;
            this.cmbPhase.Location = new System.Drawing.Point(3, 14);
            this.cmbPhase.Name = "cmbPhase";
            this.cmbPhase.Size = new System.Drawing.Size(177, 21);
            this.cmbPhase.TabIndex = 0;
            this.cmbPhase.SelectedIndexChanged += new System.EventHandler(this.cmbPhase_SelectedIndexChanged);
            this.cmbPhase.SelectedValueChanged += new System.EventHandler(this.cmbPhase_SelectedValueChanged);
            // 
            // lblPhase
            // 
            this.lblPhase.AutoSize = true;
            this.lblPhase.Location = new System.Drawing.Point(3, 0);
            this.lblPhase.Name = "lblPhase";
            this.lblPhase.Size = new System.Drawing.Size(85, 11);
            this.lblPhase.TabIndex = 1;
            this.lblPhase.Text = "Select a Project:";
            // 
            // lblEnterProvider
            // 
            this.lblEnterProvider.AutoSize = true;
            this.lblEnterProvider.Location = new System.Drawing.Point(291, 0);
            this.lblEnterProvider.Name = "lblEnterProvider";
            this.lblEnterProvider.Size = new System.Drawing.Size(163, 11);
            this.lblEnterProvider.TabIndex = 3;
            this.lblEnterProvider.Text = "Enter Provider Name/MPIN/TIN:";
            // 
            // btnSearchProvider
            // 
            this.btnSearchProvider.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSearchProvider.Location = new System.Drawing.Point(634, 14);
            this.btnSearchProvider.Name = "btnSearchProvider";
            this.btnSearchProvider.Size = new System.Drawing.Size(144, 23);
            this.btnSearchProvider.TabIndex = 4;
            this.btnSearchProvider.Text = "Search Providers";
            this.btnSearchProvider.UseVisualStyleBackColor = true;
            this.btnSearchProvider.Click += new System.EventHandler(this.btnSearchProvider_Click);
            // 
            // lblTinSearch
            // 
            this.lblTinSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTinSearch.AutoSize = true;
            this.lblTinSearch.Location = new System.Drawing.Point(3, 4);
            this.lblTinSearch.Name = "lblTinSearch";
            this.lblTinSearch.Size = new System.Drawing.Size(80, 13);
            this.lblTinSearch.TabIndex = 5;
            this.lblTinSearch.Text = "Search By TIN:";
            // 
            // lblProviderResults
            // 
            this.lblProviderResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblProviderResults.AutoSize = true;
            this.lblProviderResults.Location = new System.Drawing.Point(3, 22);
            this.lblProviderResults.Name = "lblProviderResults";
            this.lblProviderResults.Size = new System.Drawing.Size(93, 13);
            this.lblProviderResults.TabIndex = 9;
            this.lblProviderResults.Text = "Select Provider(s):";
            // 
            // btnSumbitProviders
            // 
            this.btnSumbitProviders.Location = new System.Drawing.Point(685, 3);
            this.btnSumbitProviders.Name = "btnSumbitProviders";
            this.btnSumbitProviders.Size = new System.Drawing.Size(75, 23);
            this.btnSumbitProviders.TabIndex = 11;
            this.btnSumbitProviders.Text = "Submit";
            this.btnSumbitProviders.UseVisualStyleBackColor = true;
            this.btnSumbitProviders.Click += new System.EventHandler(this.btnSumbitProviders_Click);
            // 
            // lvTinSearch
            // 
            this.lvTinSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvTinSearch.Location = new System.Drawing.Point(3, 20);
            this.lvTinSearch.Name = "lvTinSearch";
            this.lvTinSearch.Size = new System.Drawing.Size(830, 99);
            this.lvTinSearch.TabIndex = 13;
            this.lvTinSearch.UseCompatibleStateImageBehavior = false;
            this.lvTinSearch.SelectedIndexChanged += new System.EventHandler(this.lvTinSearch_SelectedIndexChanged);
            // 
            // lvProviderResults
            // 
            this.lvProviderResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvProviderResults.Location = new System.Drawing.Point(3, 3);
            this.lvProviderResults.Name = "lvProviderResults";
            this.lvProviderResults.Size = new System.Drawing.Size(830, 235);
            this.lvProviderResults.TabIndex = 14;
            this.lvProviderResults.UseCompatibleStateImageBehavior = false;
            this.lvProviderResults.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvProviderResults_MouseClick);
            // 
            // grpCheckUncheck
            // 
            this.grpCheckUncheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCheckUncheck.Controls.Add(this.radSelectAll);
            this.grpCheckUncheck.Controls.Add(this.radDeselectAll);
            this.grpCheckUncheck.Location = new System.Drawing.Point(654, 3);
            this.grpCheckUncheck.Name = "grpCheckUncheck";
            this.grpCheckUncheck.Size = new System.Drawing.Size(179, 29);
            this.grpCheckUncheck.TabIndex = 15;
            this.grpCheckUncheck.TabStop = false;
            // 
            // radSelectAll
            // 
            this.radSelectAll.AutoSize = true;
            this.radSelectAll.Location = new System.Drawing.Point(6, 10);
            this.radSelectAll.Name = "radSelectAll";
            this.radSelectAll.Size = new System.Drawing.Size(69, 17);
            this.radSelectAll.TabIndex = 1;
            this.radSelectAll.TabStop = true;
            this.radSelectAll.Text = "Select All";
            this.radSelectAll.UseVisualStyleBackColor = true;
            this.radSelectAll.CheckedChanged += new System.EventHandler(this.radSelectAll_CheckedChanged);
            // 
            // radDeselectAll
            // 
            this.radDeselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.radDeselectAll.AutoSize = true;
            this.radDeselectAll.Location = new System.Drawing.Point(92, 10);
            this.radDeselectAll.Name = "radDeselectAll";
            this.radDeselectAll.Size = new System.Drawing.Size(81, 17);
            this.radDeselectAll.TabIndex = 0;
            this.radDeselectAll.TabStop = true;
            this.radDeselectAll.Text = "Deselect All";
            this.radDeselectAll.UseVisualStyleBackColor = true;
            this.radDeselectAll.CheckedChanged += new System.EventHandler(this.radDeselectAll_CheckedChanged);
            // 
            // tlpMainContainer
            // 
            this.tlpMainContainer.ColumnCount = 1;
            this.tlpMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMainContainer.Controls.Add(this.tlpTINSearch, 0, 1);
            this.tlpMainContainer.Controls.Add(this.tlpSearchResultsOptions, 0, 2);
            this.tlpMainContainer.Controls.Add(this.tlpSearchResults, 0, 3);
            this.tlpMainContainer.Controls.Add(this.tlpSubmitExit, 0, 4);
            this.tlpMainContainer.Controls.Add(this.tlpSearchOptionsTop, 0, 0);
            this.tlpMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMainContainer.Location = new System.Drawing.Point(0, 24);
            this.tlpMainContainer.Name = "tlpMainContainer";
            this.tlpMainContainer.RowCount = 5;
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tlpMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tlpMainContainer.Size = new System.Drawing.Size(842, 515);
            this.tlpMainContainer.TabIndex = 16;
            // 
            // tlpTINSearch
            // 
            this.tlpTINSearch.ColumnCount = 1;
            this.tlpTINSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpTINSearch.Controls.Add(this.lblTinSearch, 0, 0);
            this.tlpTINSearch.Controls.Add(this.lvTinSearch, 0, 1);
            this.tlpTINSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpTINSearch.Location = new System.Drawing.Point(3, 64);
            this.tlpTINSearch.Name = "tlpTINSearch";
            this.tlpTINSearch.RowCount = 2;
            this.tlpTINSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 13.98601F));
            this.tlpTINSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 86.01398F));
            this.tlpTINSearch.Size = new System.Drawing.Size(836, 122);
            this.tlpTINSearch.TabIndex = 1;
            // 
            // tlpSearchResultsOptions
            // 
            this.tlpSearchResultsOptions.ColumnCount = 3;
            this.tlpSearchResultsOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.33493F));
            this.tlpSearchResultsOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.79426F));
            this.tlpSearchResultsOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.7512F));
            this.tlpSearchResultsOptions.Controls.Add(this.lblProviderResults, 0, 0);
            this.tlpSearchResultsOptions.Controls.Add(this.grpCheckUncheck, 2, 0);
            this.tlpSearchResultsOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearchResultsOptions.Location = new System.Drawing.Point(3, 192);
            this.tlpSearchResultsOptions.Name = "tlpSearchResultsOptions";
            this.tlpSearchResultsOptions.RowCount = 1;
            this.tlpSearchResultsOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSearchResultsOptions.Size = new System.Drawing.Size(836, 35);
            this.tlpSearchResultsOptions.TabIndex = 2;
            // 
            // tlpSearchResults
            // 
            this.tlpSearchResults.ColumnCount = 1;
            this.tlpSearchResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchResults.Controls.Add(this.lvProviderResults, 0, 0);
            this.tlpSearchResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearchResults.Location = new System.Drawing.Point(3, 233);
            this.tlpSearchResults.Name = "tlpSearchResults";
            this.tlpSearchResults.RowCount = 1;
            this.tlpSearchResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchResults.Size = new System.Drawing.Size(836, 241);
            this.tlpSearchResults.TabIndex = 3;
            // 
            // tlpSubmitExit
            // 
            this.tlpSubmitExit.ColumnCount = 4;
            this.tlpSubmitExit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSubmitExit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 344F));
            this.tlpSubmitExit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSubmitExit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tlpSubmitExit.Controls.Add(this.btnSumbitProviders, 3, 0);
            this.tlpSubmitExit.Controls.Add(this.chkCloneItems, 2, 0);
            this.tlpSubmitExit.Location = new System.Drawing.Point(3, 480);
            this.tlpSubmitExit.Name = "tlpSubmitExit";
            this.tlpSubmitExit.RowCount = 1;
            this.tlpSubmitExit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSubmitExit.Size = new System.Drawing.Size(836, 32);
            this.tlpSubmitExit.TabIndex = 4;
            // 
            // chkCloneItems
            // 
            this.chkCloneItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCloneItems.AutoSize = true;
            this.chkCloneItems.Location = new System.Drawing.Point(516, 7);
            this.chkCloneItems.Name = "chkCloneItems";
            this.chkCloneItems.Size = new System.Drawing.Size(163, 17);
            this.chkCloneItems.TabIndex = 2;
            this.chkCloneItems.Text = "Clone Items";
            this.chkCloneItems.UseVisualStyleBackColor = true;
            // 
            // tlpSearchOptionsTop
            // 
            this.tlpSearchOptionsTop.ColumnCount = 3;
            this.tlpSearchOptionsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchOptionsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSearchOptionsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.tlpSearchOptionsTop.Controls.Add(this.cmbPhase, 0, 1);
            this.tlpSearchOptionsTop.Controls.Add(this.lblPhase, 0, 0);
            this.tlpSearchOptionsTop.Controls.Add(this.lblEnterProvider, 1, 0);
            this.tlpSearchOptionsTop.Controls.Add(this.btnSearchProvider, 2, 1);
            this.tlpSearchOptionsTop.Controls.Add(this.tableLayoutPanel1, 1, 1);
            this.tlpSearchOptionsTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSearchOptionsTop.Location = new System.Drawing.Point(3, 3);
            this.tlpSearchOptionsTop.Name = "tlpSearchOptionsTop";
            this.tlpSearchOptionsTop.RowCount = 2;
            this.tlpSearchOptionsTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpSearchOptionsTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tlpSearchOptionsTop.Size = new System.Drawing.Size(836, 55);
            this.tlpSearchOptionsTop.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.10638F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.89362F));
            this.tableLayoutPanel1.Controls.Add(this.cmbProviderSearch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnProviderDetails, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(291, 14);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(282, 38);
            this.tableLayoutPanel1.TabIndex = 5;
            // 
            // cmbProviderSearch
            // 
            this.cmbProviderSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbProviderSearch.FormattingEnabled = true;
            this.cmbProviderSearch.Location = new System.Drawing.Point(3, 3);
            this.cmbProviderSearch.Name = "cmbProviderSearch";
            this.cmbProviderSearch.Size = new System.Drawing.Size(233, 21);
            this.cmbProviderSearch.TabIndex = 5;
            this.cmbProviderSearch.SelectedIndexChanged += new System.EventHandler(this.cmbProviderSearch_SelectedIndexChanged);
            this.cmbProviderSearch.SelectionChangeCommitted += new System.EventHandler(this.cmbProviderSearch_SelectionChangeCommitted);
            this.cmbProviderSearch.TextChanged += new System.EventHandler(this.cmbProviderSearch_TextChanged);
            this.cmbProviderSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbProviderSearch_KeyDown);
            this.cmbProviderSearch.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbProviderSearch_MouseClick);
            // 
            // btnProviderDetails
            // 
            this.btnProviderDetails.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnProviderDetails.BackgroundImage")));
            this.btnProviderDetails.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnProviderDetails.Location = new System.Drawing.Point(242, 3);
            this.btnProviderDetails.Name = "btnProviderDetails";
            this.btnProviderDetails.Size = new System.Drawing.Size(32, 32);
            this.btnProviderDetails.TabIndex = 6;
            this.btnProviderDetails.UseVisualStyleBackColor = true;
            this.btnProviderDetails.Click += new System.EventHandler(this.btnProviderDetails_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(842, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // frmAddProvders
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(842, 539);
            this.Controls.Add(this.tlpMainContainer);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddProvders";
            this.Text = "Add Provider(s)";
            this.Load += new System.EventHandler(this.AddProvders_Load);
            this.grpCheckUncheck.ResumeLayout(false);
            this.grpCheckUncheck.PerformLayout();
            this.tlpMainContainer.ResumeLayout(false);
            this.tlpTINSearch.ResumeLayout(false);
            this.tlpTINSearch.PerformLayout();
            this.tlpSearchResultsOptions.ResumeLayout(false);
            this.tlpSearchResultsOptions.PerformLayout();
            this.tlpSearchResults.ResumeLayout(false);
            this.tlpSubmitExit.ResumeLayout(false);
            this.tlpSubmitExit.PerformLayout();
            this.tlpSearchOptionsTop.ResumeLayout(false);
            this.tlpSearchOptionsTop.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblPhase;
        private System.Windows.Forms.Label lblEnterProvider;
        private System.Windows.Forms.Button btnSearchProvider;
        private System.Windows.Forms.Label lblTinSearch;
        private System.Windows.Forms.Label lblProviderResults;
        private System.Windows.Forms.Button btnSumbitProviders;
        private System.Windows.Forms.ListView lvTinSearch;
        private System.Windows.Forms.ListView lvProviderResults;
        private System.Windows.Forms.GroupBox grpCheckUncheck;
        private System.Windows.Forms.RadioButton radSelectAll;
        private System.Windows.Forms.RadioButton radDeselectAll;
        private System.Windows.Forms.TableLayoutPanel tlpMainContainer;
        private System.Windows.Forms.TableLayoutPanel tlpSearchOptionsTop;
        private System.Windows.Forms.TableLayoutPanel tlpTINSearch;
        private System.Windows.Forms.TableLayoutPanel tlpSearchResultsOptions;
        private System.Windows.Forms.TableLayoutPanel tlpSearchResults;
        private System.Windows.Forms.TableLayoutPanel tlpSubmitExit;
        private System.Windows.Forms.ComboBox cmbProviderSearch;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox chkCloneItems;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnProviderDetails;
        public System.Windows.Forms.ComboBox cmbPhase;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}