namespace PhysicianFeedbackTracker
{
    partial class frmComplianceReporting_Filters
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
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.tlpFilterOptions = new System.Windows.Forms.TableLayoutPanel();
            this.btnClearAllFilters = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblCurrentFilter = new System.Windows.Forms.Label();
            this.clbCurrentFilters = new System.Windows.Forms.CheckedListBox();
            this.tlpMain.SuspendLayout();
            this.tlpFilterOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpMain
            // 
            this.tlpMain.ColumnCount = 1;
            this.tlpMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpMain.Controls.Add(this.tlpFilterOptions, 0, 0);
            this.tlpMain.Controls.Add(this.clbCurrentFilters, 0, 1);
            this.tlpMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpMain.Location = new System.Drawing.Point(0, 0);
            this.tlpMain.Name = "tlpMain";
            this.tlpMain.RowCount = 3;
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.888889F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.88889F));
            this.tlpMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 0F));
            this.tlpMain.Size = new System.Drawing.Size(800, 450);
            this.tlpMain.TabIndex = 0;
            // 
            // tlpFilterOptions
            // 
            this.tlpFilterOptions.ColumnCount = 3;
            this.tlpFilterOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.49874F));
            this.tlpFilterOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.26952F));
            this.tlpFilterOptions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.35768F));
            this.tlpFilterOptions.Controls.Add(this.btnClearAllFilters, 1, 0);
            this.tlpFilterOptions.Controls.Add(this.btnClose, 2, 0);
            this.tlpFilterOptions.Controls.Add(this.lblCurrentFilter, 0, 0);
            this.tlpFilterOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpFilterOptions.Location = new System.Drawing.Point(3, 3);
            this.tlpFilterOptions.Name = "tlpFilterOptions";
            this.tlpFilterOptions.RowCount = 1;
            this.tlpFilterOptions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpFilterOptions.Size = new System.Drawing.Size(794, 34);
            this.tlpFilterOptions.TabIndex = 17;
            // 
            // btnClearAllFilters
            // 
            this.btnClearAllFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClearAllFilters.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearAllFilters.Location = new System.Drawing.Point(530, 3);
            this.btnClearAllFilters.Name = "btnClearAllFilters";
            this.btnClearAllFilters.Size = new System.Drawing.Size(146, 28);
            this.btnClearAllFilters.TabIndex = 3;
            this.btnClearAllFilters.Text = "Clear Filters";
            this.btnClearAllFilters.UseVisualStyleBackColor = true;
            this.btnClearAllFilters.Click += new System.EventHandler(this.btnClearAllFilters_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(682, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(109, 28);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblCurrentFilter
            // 
            this.lblCurrentFilter.AutoSize = true;
            this.lblCurrentFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentFilter.Location = new System.Drawing.Point(3, 0);
            this.lblCurrentFilter.Name = "lblCurrentFilter";
            this.lblCurrentFilter.Size = new System.Drawing.Size(521, 34);
            this.lblCurrentFilter.TabIndex = 1;
            // 
            // clbCurrentFilters
            // 
            this.clbCurrentFilters.CheckOnClick = true;
            this.clbCurrentFilters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbCurrentFilters.FormattingEnabled = true;
            this.clbCurrentFilters.Location = new System.Drawing.Point(3, 43);
            this.clbCurrentFilters.Name = "clbCurrentFilters";
            this.clbCurrentFilters.Size = new System.Drawing.Size(794, 403);
            this.clbCurrentFilters.TabIndex = 18;
            this.clbCurrentFilters.SelectedIndexChanged += new System.EventHandler(this.clbCurrentFilters_SelectedIndexChanged);
            // 
            // frmComplianceReporting_Filters
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.tlpMain);
            this.Name = "frmComplianceReporting_Filters";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Selected Filters";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmComplianceReporting_Filters_FormClosing);
            this.tlpMain.ResumeLayout(false);
            this.tlpFilterOptions.ResumeLayout(false);
            this.tlpFilterOptions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.TableLayoutPanel tlpFilterOptions;
        private System.Windows.Forms.Button btnClearAllFilters;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblCurrentFilter;
        private System.Windows.Forms.CheckedListBox clbCurrentFilters;
    }
}