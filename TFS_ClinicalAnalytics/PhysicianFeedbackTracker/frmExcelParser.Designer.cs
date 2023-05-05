namespace PhysicianFeedbackTracker
{
    partial class frmExcelParser
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
            this.tlpExcelParser = new System.Windows.Forms.TableLayoutPanel();
            this.tblExcelSelect = new System.Windows.Forms.TableLayoutPanel();
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.txtExcelFilePath = new System.Windows.Forms.TextBox();
            this.lblExcelFile = new System.Windows.Forms.Label();
            this.txtParseExcelResults = new System.Windows.Forms.TextBox();
            this.btnParseExcelFile = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tlpExcelParser.SuspendLayout();
            this.tblExcelSelect.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpExcelParser
            // 
            this.tlpExcelParser.AllowDrop = true;
            this.tlpExcelParser.ColumnCount = 1;
            this.tlpExcelParser.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpExcelParser.Controls.Add(this.tblExcelSelect, 0, 0);
            this.tlpExcelParser.Controls.Add(this.txtParseExcelResults, 0, 1);
            this.tlpExcelParser.Controls.Add(this.btnParseExcelFile, 0, 2);
            this.tlpExcelParser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpExcelParser.Location = new System.Drawing.Point(0, 24);
            this.tlpExcelParser.Name = "tlpExcelParser";
            this.tlpExcelParser.RowCount = 3;
            this.tlpExcelParser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.170068F));
            this.tlpExcelParser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.82993F));
            this.tlpExcelParser.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.tlpExcelParser.Size = new System.Drawing.Size(863, 711);
            this.tlpExcelParser.TabIndex = 2;
            // 
            // tblExcelSelect
            // 
            this.tblExcelSelect.AllowDrop = true;
            this.tblExcelSelect.ColumnCount = 3;
            this.tblExcelSelect.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.913649F));
            this.tblExcelSelect.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 91.08635F));
            this.tblExcelSelect.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tblExcelSelect.Controls.Add(this.btnChooseFile, 2, 0);
            this.tblExcelSelect.Controls.Add(this.txtExcelFilePath, 1, 0);
            this.tblExcelSelect.Controls.Add(this.lblExcelFile, 0, 0);
            this.tblExcelSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblExcelSelect.Location = new System.Drawing.Point(3, 3);
            this.tblExcelSelect.Name = "tblExcelSelect";
            this.tblExcelSelect.RowCount = 1;
            this.tblExcelSelect.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.552845F));
            this.tblExcelSelect.Size = new System.Drawing.Size(857, 28);
            this.tblExcelSelect.TabIndex = 2;
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChooseFile.Location = new System.Drawing.Point(705, 3);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(149, 22);
            this.btnChooseFile.TabIndex = 0;
            this.btnChooseFile.Text = "Choose Excel File";
            this.btnChooseFile.UseVisualStyleBackColor = true;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // txtExcelFilePath
            // 
            this.txtExcelFilePath.AllowDrop = true;
            this.txtExcelFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExcelFilePath.Location = new System.Drawing.Point(65, 3);
            this.txtExcelFilePath.Name = "txtExcelFilePath";
            this.txtExcelFilePath.Size = new System.Drawing.Size(634, 20);
            this.txtExcelFilePath.TabIndex = 1;
            this.txtExcelFilePath.TextChanged += new System.EventHandler(this.txtExcelFilePath_TextChanged);
            // 
            // lblExcelFile
            // 
            this.lblExcelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblExcelFile.AutoSize = true;
            this.lblExcelFile.Location = new System.Drawing.Point(3, 0);
            this.lblExcelFile.Name = "lblExcelFile";
            this.lblExcelFile.Size = new System.Drawing.Size(56, 13);
            this.lblExcelFile.TabIndex = 2;
            this.lblExcelFile.Text = "Excel File:";
            // 
            // txtParseExcelResults
            // 
            this.txtParseExcelResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtParseExcelResults.Location = new System.Drawing.Point(3, 37);
            this.txtParseExcelResults.Multiline = true;
            this.txtParseExcelResults.Name = "txtParseExcelResults";
            this.txtParseExcelResults.ReadOnly = true;
            this.txtParseExcelResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtParseExcelResults.Size = new System.Drawing.Size(857, 633);
            this.txtParseExcelResults.TabIndex = 3;
            // 
            // btnParseExcelFile
            // 
            this.btnParseExcelFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnParseExcelFile.Enabled = false;
            this.btnParseExcelFile.Location = new System.Drawing.Point(703, 676);
            this.btnParseExcelFile.Name = "btnParseExcelFile";
            this.btnParseExcelFile.Size = new System.Drawing.Size(157, 23);
            this.btnParseExcelFile.TabIndex = 4;
            this.btnParseExcelFile.Text = "Parse Excel File";
            this.btnParseExcelFile.UseVisualStyleBackColor = true;
            this.btnParseExcelFile.Click += new System.EventHandler(this.btnParseExcelFile_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(863, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // frmExcelParser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(863, 735);
            this.Controls.Add(this.tlpExcelParser);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmExcelParser";
            this.Text = "ExcelParser";
            this.tlpExcelParser.ResumeLayout(false);
            this.tlpExcelParser.PerformLayout();
            this.tblExcelSelect.ResumeLayout(false);
            this.tblExcelSelect.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tlpExcelParser;
        private System.Windows.Forms.TableLayoutPanel tblExcelSelect;
        private System.Windows.Forms.Button btnChooseFile;
        private System.Windows.Forms.TextBox txtExcelFilePath;
        private System.Windows.Forms.Label lblExcelFile;
        private System.Windows.Forms.TextBox txtParseExcelResults;
        private System.Windows.Forms.Button btnParseExcelFile;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    }
}