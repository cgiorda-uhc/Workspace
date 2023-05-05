namespace PhysicianFeedbackTracker
{
    partial class frmPX
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
            this.dgvPX = new System.Windows.Forms.DataGridView();
            this.tlpAPRDRG = new System.Windows.Forms.TableLayoutPanel();
            this.tlpAPRDRGButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnUGAPPX = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tssStatus = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPX)).BeginInit();
            this.tlpAPRDRG.SuspendLayout();
            this.tlpAPRDRGButtons.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvPX
            // 
            this.dgvPX.AllowUserToAddRows = false;
            this.dgvPX.AllowUserToDeleteRows = false;
            this.dgvPX.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPX.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvPX.Location = new System.Drawing.Point(3, 53);
            this.dgvPX.Name = "dgvPX";
            this.dgvPX.Size = new System.Drawing.Size(762, 646);
            this.dgvPX.TabIndex = 0;
            this.dgvPX.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPX_CellContentClick);
            this.dgvPX.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPX_CellEndEdit);
            this.dgvPX.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPX_EditingControlShowing);
            this.dgvPX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dgvPX_MouseMove);
            // 
            // tlpAPRDRG
            // 
            this.tlpAPRDRG.ColumnCount = 1;
            this.tlpAPRDRG.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPRDRG.Controls.Add(this.tlpAPRDRGButtons, 0, 0);
            this.tlpAPRDRG.Controls.Add(this.dgvPX, 0, 1);
            this.tlpAPRDRG.Controls.Add(this.statusStrip1, 0, 2);
            this.tlpAPRDRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAPRDRG.Location = new System.Drawing.Point(0, 0);
            this.tlpAPRDRG.Name = "tlpAPRDRG";
            this.tlpAPRDRG.RowCount = 3;
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tlpAPRDRG.Size = new System.Drawing.Size(768, 725);
            this.tlpAPRDRG.TabIndex = 2;
            // 
            // tlpAPRDRGButtons
            // 
            this.tlpAPRDRGButtons.ColumnCount = 4;
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpAPRDRGButtons.Controls.Add(this.btnUGAPPX, 3, 0);
            this.tlpAPRDRGButtons.Controls.Add(this.btnExit, 0, 0);
            this.tlpAPRDRGButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAPRDRGButtons.Location = new System.Drawing.Point(3, 3);
            this.tlpAPRDRGButtons.Name = "tlpAPRDRGButtons";
            this.tlpAPRDRGButtons.RowCount = 1;
            this.tlpAPRDRGButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPRDRGButtons.Size = new System.Drawing.Size(762, 44);
            this.tlpAPRDRGButtons.TabIndex = 2;
            // 
            // btnUGAPPX
            // 
            this.btnUGAPPX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUGAPPX.Location = new System.Drawing.Point(573, 3);
            this.btnUGAPPX.Name = "btnUGAPPX";
            this.btnUGAPPX.Size = new System.Drawing.Size(186, 38);
            this.btnUGAPPX.TabIndex = 7;
            this.btnUGAPPX.Text = "Load UGAP.PX into IL_UCA.PBP_PX";
            this.btnUGAPPX.UseVisualStyleBackColor = true;
            this.btnUGAPPX.Click += new System.EventHandler(this.btnUGAPPX_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExit.Location = new System.Drawing.Point(3, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(184, 38);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Close PX Session";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssProgressBar,
            this.tssStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 703);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(768, 22);
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
            this.tssStatus.Size = new System.Drawing.Size(39, 17);
            this.tssStatus.Text = "Ready";
            // 
            // frmPX
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(768, 725);
            this.Controls.Add(this.tlpAPRDRG);
            this.Name = "frmPX";
            this.Text = "Update PX";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dgvPX)).EndInit();
            this.tlpAPRDRG.ResumeLayout(false);
            this.tlpAPRDRG.PerformLayout();
            this.tlpAPRDRGButtons.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPX;
        private System.Windows.Forms.TableLayoutPanel tlpAPRDRG;
        private System.Windows.Forms.TableLayoutPanel tlpAPRDRGButtons;
        private System.Windows.Forms.Button btnUGAPPX;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar tssProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tssStatus;
    }
}