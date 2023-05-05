namespace PhysicianFeedbackTracker
{
    partial class frmQAWorkFlow
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
            this.clbSpecialties = new System.Windows.Forms.CheckedListBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnCreateWorkItem = new System.Windows.Forms.Button();
            this.btnGetSamplingFile = new System.Windows.Forms.Button();
            this.dvSampling = new System.Windows.Forms.DataGridView();
            this.tlpSamplingSelections = new System.Windows.Forms.TableLayoutPanel();
            this.grpQAMeasures = new System.Windows.Forms.GroupBox();
            this.cmbQAMeasures = new System.Windows.Forms.ComboBox();
            this.grpQAType = new System.Windows.Forms.GroupBox();
            this.cmbQAType = new System.Windows.Forms.ComboBox();
            this.grpSpecialtiesToQA = new System.Windows.Forms.GroupBox();
            this.tlpSampling = new System.Windows.Forms.TableLayoutPanel();
            this.tlpSamplingAnalysts = new System.Windows.Forms.TableLayoutPanel();
            this.tlpSamplingDataGrid = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dvSampling)).BeginInit();
            this.tlpSamplingSelections.SuspendLayout();
            this.grpQAMeasures.SuspendLayout();
            this.grpQAType.SuspendLayout();
            this.grpSpecialtiesToQA.SuspendLayout();
            this.tlpSampling.SuspendLayout();
            this.tlpSamplingDataGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // clbSpecialties
            // 
            this.clbSpecialties.CheckOnClick = true;
            this.clbSpecialties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbSpecialties.FormattingEnabled = true;
            this.clbSpecialties.Location = new System.Drawing.Point(3, 16);
            this.clbSpecialties.Name = "clbSpecialties";
            this.clbSpecialties.Size = new System.Drawing.Size(225, 125);
            this.clbSpecialties.TabIndex = 2;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Excel File|*.xls;*.xlsx";
            // 
            // btnCreateWorkItem
            // 
            this.btnCreateWorkItem.Location = new System.Drawing.Point(3, 690);
            this.btnCreateWorkItem.Name = "btnCreateWorkItem";
            this.btnCreateWorkItem.Size = new System.Drawing.Size(165, 23);
            this.btnCreateWorkItem.TabIndex = 3;
            this.btnCreateWorkItem.Text = "Create Work Item";
            this.btnCreateWorkItem.UseVisualStyleBackColor = true;
            // 
            // btnGetSamplingFile
            // 
            this.btnGetSamplingFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetSamplingFile.Location = new System.Drawing.Point(714, 59);
            this.btnGetSamplingFile.Name = "btnGetSamplingFile";
            this.btnGetSamplingFile.Size = new System.Drawing.Size(231, 32);
            this.btnGetSamplingFile.TabIndex = 8;
            this.btnGetSamplingFile.Text = "Choose Sampling File";
            this.btnGetSamplingFile.UseVisualStyleBackColor = true;
            this.btnGetSamplingFile.Click += new System.EventHandler(this.btnGetSamplingFile_Click);
            // 
            // dvSampling
            // 
            this.dvSampling.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvSampling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dvSampling.Location = new System.Drawing.Point(3, 3);
            this.dvSampling.Name = "dvSampling";
            this.dvSampling.Size = new System.Drawing.Size(942, 434);
            this.dvSampling.TabIndex = 9;
            // 
            // tlpSamplingSelections
            // 
            this.tlpSamplingSelections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tlpSamplingSelections.ColumnCount = 4;
            this.tlpSamplingSelections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpSamplingSelections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpSamplingSelections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpSamplingSelections.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tlpSamplingSelections.Controls.Add(this.grpQAMeasures, 1, 0);
            this.tlpSamplingSelections.Controls.Add(this.grpQAType, 0, 0);
            this.tlpSamplingSelections.Controls.Add(this.grpSpecialtiesToQA, 2, 0);
            this.tlpSamplingSelections.Controls.Add(this.btnGetSamplingFile, 3, 0);
            this.tlpSamplingSelections.Location = new System.Drawing.Point(3, 3);
            this.tlpSamplingSelections.Name = "tlpSamplingSelections";
            this.tlpSamplingSelections.RowCount = 1;
            this.tlpSamplingSelections.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSamplingSelections.Size = new System.Drawing.Size(948, 150);
            this.tlpSamplingSelections.TabIndex = 10;
            // 
            // grpQAMeasures
            // 
            this.grpQAMeasures.Controls.Add(this.cmbQAMeasures);
            this.grpQAMeasures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpQAMeasures.Location = new System.Drawing.Point(240, 3);
            this.grpQAMeasures.Name = "grpQAMeasures";
            this.grpQAMeasures.Size = new System.Drawing.Size(231, 144);
            this.grpQAMeasures.TabIndex = 12;
            this.grpQAMeasures.TabStop = false;
            this.grpQAMeasures.Text = "Select Measure to QA";
            // 
            // cmbQAMeasures
            // 
            this.cmbQAMeasures.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbQAMeasures.FormattingEnabled = true;
            this.cmbQAMeasures.Location = new System.Drawing.Point(6, 64);
            this.cmbQAMeasures.Name = "cmbQAMeasures";
            this.cmbQAMeasures.Size = new System.Drawing.Size(219, 21);
            this.cmbQAMeasures.TabIndex = 1;
            this.cmbQAMeasures.SelectedIndexChanged += new System.EventHandler(this.cmbQAMeasures_SelectedIndexChanged);
            // 
            // grpQAType
            // 
            this.grpQAType.Controls.Add(this.cmbQAType);
            this.grpQAType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpQAType.Location = new System.Drawing.Point(3, 3);
            this.grpQAType.Name = "grpQAType";
            this.grpQAType.Size = new System.Drawing.Size(231, 144);
            this.grpQAType.TabIndex = 11;
            this.grpQAType.TabStop = false;
            this.grpQAType.Text = "Select QA Type";
            // 
            // cmbQAType
            // 
            this.cmbQAType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbQAType.FormattingEnabled = true;
            this.cmbQAType.Location = new System.Drawing.Point(6, 63);
            this.cmbQAType.Name = "cmbQAType";
            this.cmbQAType.Size = new System.Drawing.Size(207, 21);
            this.cmbQAType.TabIndex = 0;
            // 
            // grpSpecialtiesToQA
            // 
            this.grpSpecialtiesToQA.Controls.Add(this.clbSpecialties);
            this.grpSpecialtiesToQA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpSpecialtiesToQA.Location = new System.Drawing.Point(477, 3);
            this.grpSpecialtiesToQA.Name = "grpSpecialtiesToQA";
            this.grpSpecialtiesToQA.Size = new System.Drawing.Size(231, 144);
            this.grpSpecialtiesToQA.TabIndex = 13;
            this.grpSpecialtiesToQA.TabStop = false;
            this.grpSpecialtiesToQA.Text = "Select Specialties to QA";
            // 
            // tlpSampling
            // 
            this.tlpSampling.ColumnCount = 1;
            this.tlpSampling.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSampling.Controls.Add(this.tlpSamplingSelections, 0, 0);
            this.tlpSampling.Controls.Add(this.btnCreateWorkItem, 0, 3);
            this.tlpSampling.Controls.Add(this.tlpSamplingAnalysts, 0, 1);
            this.tlpSampling.Controls.Add(this.tlpSamplingDataGrid, 0, 2);
            this.tlpSampling.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSampling.Location = new System.Drawing.Point(0, 0);
            this.tlpSampling.Name = "tlpSampling";
            this.tlpSampling.RowCount = 4;
            this.tlpSampling.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 156F));
            this.tlpSampling.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.00753F));
            this.tlpSampling.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 83.99247F));
            this.tlpSampling.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 69F));
            this.tlpSampling.Size = new System.Drawing.Size(954, 757);
            this.tlpSampling.TabIndex = 11;
            // 
            // tlpSamplingAnalysts
            // 
            this.tlpSamplingAnalysts.ColumnCount = 2;
            this.tlpSamplingAnalysts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSamplingAnalysts.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSamplingAnalysts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSamplingAnalysts.Location = new System.Drawing.Point(3, 159);
            this.tlpSamplingAnalysts.Name = "tlpSamplingAnalysts";
            this.tlpSamplingAnalysts.RowCount = 2;
            this.tlpSamplingAnalysts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSamplingAnalysts.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpSamplingAnalysts.Size = new System.Drawing.Size(948, 79);
            this.tlpSamplingAnalysts.TabIndex = 11;
            // 
            // tlpSamplingDataGrid
            // 
            this.tlpSamplingDataGrid.ColumnCount = 1;
            this.tlpSamplingDataGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSamplingDataGrid.Controls.Add(this.dvSampling, 0, 0);
            this.tlpSamplingDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpSamplingDataGrid.Location = new System.Drawing.Point(3, 244);
            this.tlpSamplingDataGrid.Name = "tlpSamplingDataGrid";
            this.tlpSamplingDataGrid.RowCount = 1;
            this.tlpSamplingDataGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpSamplingDataGrid.Size = new System.Drawing.Size(948, 440);
            this.tlpSamplingDataGrid.TabIndex = 12;
            // 
            // frmQAWorkFlow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(954, 757);
            this.Controls.Add(this.tlpSampling);
            this.Name = "frmQAWorkFlow";
            this.Text = "QA Work Flow";
            this.Load += new System.EventHandler(this.QAWorkFlow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dvSampling)).EndInit();
            this.tlpSamplingSelections.ResumeLayout(false);
            this.grpQAMeasures.ResumeLayout(false);
            this.grpQAType.ResumeLayout(false);
            this.grpSpecialtiesToQA.ResumeLayout(false);
            this.tlpSampling.ResumeLayout(false);
            this.tlpSamplingDataGrid.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.CheckedListBox clbSpecialties;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnCreateWorkItem;
        private System.Windows.Forms.Button btnGetSamplingFile;
        private System.Windows.Forms.DataGridView dvSampling;
        private System.Windows.Forms.TableLayoutPanel tlpSamplingSelections;
        private System.Windows.Forms.TableLayoutPanel tlpSampling;
        private System.Windows.Forms.TableLayoutPanel tlpSamplingAnalysts;
        private System.Windows.Forms.TableLayoutPanel tlpSamplingDataGrid;
        private System.Windows.Forms.GroupBox grpQAMeasures;
        private System.Windows.Forms.ComboBox cmbQAMeasures;
        private System.Windows.Forms.GroupBox grpQAType;
        private System.Windows.Forms.ComboBox cmbQAType;
        private System.Windows.Forms.GroupBox grpSpecialtiesToQA;
    }
}