namespace PhysicianFeedbackTracker
{
    partial class frmAPRDRG
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
            this.dgvAPRDRG = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tssStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlpAPRDRG.SuspendLayout();
            this.tlpAPRDRGButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAPRDRG)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpAPRDRG
            // 
            this.tlpAPRDRG.ColumnCount = 1;
            this.tlpAPRDRG.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpAPRDRG.Controls.Add(this.tlpAPRDRGButtons, 0, 0);
            this.tlpAPRDRG.Controls.Add(this.dgvAPRDRG, 0, 1);
            this.tlpAPRDRG.Controls.Add(this.statusStrip1, 0, 2);
            this.tlpAPRDRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpAPRDRG.Location = new System.Drawing.Point(0, 0);
            this.tlpAPRDRG.Name = "tlpAPRDRG";
            this.tlpAPRDRG.RowCount = 3;
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7F));
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tlpAPRDRG.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3F));
            this.tlpAPRDRG.Size = new System.Drawing.Size(953, 735);
            this.tlpAPRDRG.TabIndex = 2;
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
            this.tlpAPRDRGButtons.Size = new System.Drawing.Size(947, 45);
            this.tlpAPRDRGButtons.TabIndex = 2;
            // 
            // btnUGAPAPRDRG
            // 
            this.btnUGAPAPRDRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnUGAPAPRDRG.Location = new System.Drawing.Point(711, 3);
            this.btnUGAPAPRDRG.Name = "btnUGAPAPRDRG";
            this.btnUGAPAPRDRG.Size = new System.Drawing.Size(233, 39);
            this.btnUGAPAPRDRG.TabIndex = 7;
            this.btnUGAPAPRDRG.Text = "Load UGAP.APRDRG into IL_UCA.PBP_APRDRG";
            this.btnUGAPAPRDRG.UseVisualStyleBackColor = true;
            this.btnUGAPAPRDRG.Click += new System.EventHandler(this.btnUGAPAPRDRG_Click);
            // 
            // btnExit
            // 
            this.btnExit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExit.Location = new System.Drawing.Point(3, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(230, 39);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Close APRDRG Session";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // dgvAPRDRG
            // 
            this.dgvAPRDRG.AllowUserToAddRows = false;
            this.dgvAPRDRG.AllowUserToDeleteRows = false;
            this.dgvAPRDRG.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvAPRDRG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAPRDRG.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvAPRDRG.Location = new System.Drawing.Point(3, 54);
            this.dgvAPRDRG.Name = "dgvAPRDRG";
            this.dgvAPRDRG.Size = new System.Drawing.Size(947, 655);
            this.dgvAPRDRG.TabIndex = 0;
            this.dgvAPRDRG.MouseHover += new System.EventHandler(this.dgvAPRDRG_MouseHover);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssProgressBar,
            this.tssStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 713);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(953, 22);
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
            // frmAPRDRG
            // 
            this.ClientSize = new System.Drawing.Size(953, 735);
            this.Controls.Add(this.tlpAPRDRG);
            this.Name = "frmAPRDRG";
            this.Text = "Update APRDRG";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.tlpAPRDRG.ResumeLayout(false);
            this.tlpAPRDRG.PerformLayout();
            this.tlpAPRDRGButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAPRDRG)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpAPRDRG;
        private System.Windows.Forms.TableLayoutPanel tlpAPRDRGButtons;
        private System.Windows.Forms.Button btnUGAPAPRDRG;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.DataGridView dgvAPRDRG;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar tssProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tssStatus;
    }
}