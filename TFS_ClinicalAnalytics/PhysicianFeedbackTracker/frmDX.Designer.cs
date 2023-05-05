namespace PhysicianFeedbackTracker
{
    partial class frmDX
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
            this.tlpAPRDRG = new System.Windows.Forms.TableLayoutPanel();
            this.tlpAPRDRGButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnUGAPAPRDRG = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.dgvDX = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tssStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlpAPRDRG.SuspendLayout();
            this.tlpAPRDRGButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDX)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpAPRDRG
            // 
            this.tlpAPRDRG.ColumnCount = 1;
            this.tlpAPRDRG.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPRDRG.Controls.Add(this.tlpAPRDRGButtons, 0, 0);
            this.tlpAPRDRG.Controls.Add(this.dgvDX, 0, 1);
            this.tlpAPRDRG.Controls.Add(this.statusStrip1, 0, 2);
            this.tlpAPRDRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAPRDRG.Location = new System.Drawing.Point(0, 0);
            this.tlpAPRDRG.Name = "tlpAPRDRG";
            this.tlpAPRDRG.RowCount = 3;
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tlpAPRDRG.Size = new System.Drawing.Size(1006, 751);
            this.tlpAPRDRG.TabIndex = 1;
            // 
            // tlpAPRDRGButtons
            // 
            this.tlpAPRDRGButtons.ColumnCount = 4;
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.Controls.Add(this.btnUGAPAPRDRG, 3, 0);
            this.tlpAPRDRGButtons.Controls.Add(this.btnExit, 0, 0);
            this.tlpAPRDRGButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAPRDRGButtons.Location = new System.Drawing.Point(3, 3);
            this.tlpAPRDRGButtons.Name = "tlpAPRDRGButtons";
            this.tlpAPRDRGButtons.RowCount = 1;
            this.tlpAPRDRGButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPRDRGButtons.Size = new System.Drawing.Size(1000, 46);
            this.tlpAPRDRGButtons.TabIndex = 2;
            // 
            // btnUGAPAPRDRG
            // 
            this.btnUGAPAPRDRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUGAPAPRDRG.Location = new System.Drawing.Point(753, 3);
            this.btnUGAPAPRDRG.Name = "btnUGAPAPRDRG";
            this.btnUGAPAPRDRG.Size = new System.Drawing.Size(244, 40);
            this.btnUGAPAPRDRG.TabIndex = 7;
            this.btnUGAPAPRDRG.Text = "Load UGAP.DX into IL_UCA.PBP_DX";
            this.btnUGAPAPRDRG.UseVisualStyleBackColor = true;
            this.btnUGAPAPRDRG.Click += new System.EventHandler(this.btnUGAPDX_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExit.Location = new System.Drawing.Point(3, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(244, 40);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Close DX Session";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // dgvDX
            // 
            this.dgvDX.AllowUserToAddRows = false;
            this.dgvDX.AllowUserToDeleteRows = false;
            this.dgvDX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDX.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvDX.Location = new System.Drawing.Point(3, 55);
            this.dgvDX.Name = "dgvDX";
            this.dgvDX.Size = new System.Drawing.Size(1000, 669);
            this.dgvDX.TabIndex = 0;
            this.dgvDX.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDX_CellContentDoubleClick);
            this.dgvDX.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDX_CellEndEdit);
            this.dgvDX.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvDX_EditingControlShowing);
            this.dgvDX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvDX_MouseMove);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssProgressBar,
            this.tssStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 729);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1006, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssProgressBar
            // 
            this.tssProgressBar.Name = "tssProgressBar";
            this.tssProgressBar.Size = new System.Drawing.Size(100, 16);
            this.tssProgressBar.Visible = false;
            // 
            // tssStatus
            // 
            this.tssStatus.Name = "tssStatus";
            this.tssStatus.Size = new System.Drawing.Size(38, 17);
            this.tssStatus.Text = "Ready";
            // 
            // frmDX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 751);
            this.Controls.Add(this.tlpAPRDRG);
            this.Name = "frmDX";
            this.Text = "Update DX";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tlpAPRDRG.ResumeLayout(false);
            this.tlpAPRDRG.PerformLayout();
            this.tlpAPRDRGButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDX)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpAPRDRG;
        private System.Windows.Forms.TableLayoutPanel tlpAPRDRGButtons;
        private System.Windows.Forms.Button btnUGAPAPRDRG;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DataGridView dgvDX;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssStatus;
        private System.Windows.Forms.ToolStripProgressBar tssProgressBar;
    }
}