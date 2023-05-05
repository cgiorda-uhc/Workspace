namespace PhysicianFeedbackTracker
{
    partial class frmAddEngagementToPEI
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
            this.tlpEngagementToPEI = new System.Windows.Forms.TableLayoutPanel();
            this.tlpPEIEngagementReadOnly = new System.Windows.Forms.TableLayoutPanel();
            this.grpProviderName = new System.Windows.Forms.GroupBox();
            this.lblProviderName = new System.Windows.Forms.Label();
            this.grpKeyTopic = new System.Windows.Forms.GroupBox();
            this.lblKeyTopic = new System.Windows.Forms.Label();
            this.tlpPEIEngagementEditable = new System.Windows.Forms.TableLayoutPanel();
            this.grpEngagementStatus = new System.Windows.Forms.GroupBox();
            this.cmbEngagementStatus = new System.Windows.Forms.ComboBox();
            this.grpMMDAssignment = new System.Windows.Forms.GroupBox();
            this.cmbMMDAssignment = new System.Windows.Forms.ComboBox();
            this.tlpPEIEngagementSubmit = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddEngagement = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssAddEngagementToPEI = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlpEngagementToPEI.SuspendLayout();
            this.tlpPEIEngagementReadOnly.SuspendLayout();
            this.grpProviderName.SuspendLayout();
            this.grpKeyTopic.SuspendLayout();
            this.tlpPEIEngagementEditable.SuspendLayout();
            this.grpEngagementStatus.SuspendLayout();
            this.grpMMDAssignment.SuspendLayout();
            this.tlpPEIEngagementSubmit.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tlpEngagementToPEI
            // 
            this.tlpEngagementToPEI.BackColor = System.Drawing.SystemColors.Control;
            this.tlpEngagementToPEI.ColumnCount = 1;
            this.tlpEngagementToPEI.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpEngagementToPEI.Controls.Add(this.tlpPEIEngagementReadOnly, 0, 0);
            this.tlpEngagementToPEI.Controls.Add(this.tlpPEIEngagementEditable, 0, 1);
            this.tlpEngagementToPEI.Controls.Add(this.tlpPEIEngagementSubmit, 0, 2);
            this.tlpEngagementToPEI.Controls.Add(this.statusStrip1, 0, 3);
            this.tlpEngagementToPEI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpEngagementToPEI.Location = new System.Drawing.Point(0, 0);
            this.tlpEngagementToPEI.Name = "tlpEngagementToPEI";
            this.tlpEngagementToPEI.RowCount = 4;
            this.tlpEngagementToPEI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpEngagementToPEI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tlpEngagementToPEI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tlpEngagementToPEI.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpEngagementToPEI.Size = new System.Drawing.Size(605, 357);
            this.tlpEngagementToPEI.TabIndex = 0;
            // 
            // tlpPEIEngagementReadOnly
            // 
            this.tlpPEIEngagementReadOnly.BackColor = System.Drawing.SystemColors.Control;
            this.tlpPEIEngagementReadOnly.ColumnCount = 2;
            this.tlpPEIEngagementReadOnly.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementReadOnly.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementReadOnly.Controls.Add(this.grpProviderName, 1, 0);
            this.tlpPEIEngagementReadOnly.Controls.Add(this.grpKeyTopic, 0, 0);
            this.tlpPEIEngagementReadOnly.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPEIEngagementReadOnly.Location = new System.Drawing.Point(3, 3);
            this.tlpPEIEngagementReadOnly.Name = "tlpPEIEngagementReadOnly";
            this.tlpPEIEngagementReadOnly.RowCount = 2;
            this.tlpPEIEngagementReadOnly.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementReadOnly.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementReadOnly.Size = new System.Drawing.Size(599, 145);
            this.tlpPEIEngagementReadOnly.TabIndex = 0;
            // 
            // grpProviderName
            // 
            this.grpProviderName.BackColor = System.Drawing.SystemColors.Control;
            this.grpProviderName.Controls.Add(this.lblProviderName);
            this.grpProviderName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpProviderName.Location = new System.Drawing.Point(302, 3);
            this.grpProviderName.Name = "grpProviderName";
            this.grpProviderName.Size = new System.Drawing.Size(294, 66);
            this.grpProviderName.TabIndex = 1;
            this.grpProviderName.TabStop = false;
            this.grpProviderName.Text = "Provider Name";
            // 
            // lblProviderName
            // 
            this.lblProviderName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProviderName.AutoSize = true;
            this.lblProviderName.BackColor = System.Drawing.SystemColors.Control;
            this.lblProviderName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProviderName.Location = new System.Drawing.Point(6, 26);
            this.lblProviderName.Name = "lblProviderName";
            this.lblProviderName.Size = new System.Drawing.Size(90, 13);
            this.lblProviderName.TabIndex = 1;
            this.lblProviderName.Text = "Provider Name";
            // 
            // grpKeyTopic
            // 
            this.grpKeyTopic.BackColor = System.Drawing.SystemColors.Control;
            this.grpKeyTopic.Controls.Add(this.lblKeyTopic);
            this.grpKeyTopic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpKeyTopic.Location = new System.Drawing.Point(3, 3);
            this.grpKeyTopic.Name = "grpKeyTopic";
            this.grpKeyTopic.Size = new System.Drawing.Size(293, 66);
            this.grpKeyTopic.TabIndex = 0;
            this.grpKeyTopic.TabStop = false;
            this.grpKeyTopic.Text = "Key Topic";
            // 
            // lblKeyTopic
            // 
            this.lblKeyTopic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKeyTopic.AutoSize = true;
            this.lblKeyTopic.BackColor = System.Drawing.SystemColors.Control;
            this.lblKeyTopic.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeyTopic.Location = new System.Drawing.Point(7, 26);
            this.lblKeyTopic.Name = "lblKeyTopic";
            this.lblKeyTopic.Size = new System.Drawing.Size(64, 13);
            this.lblKeyTopic.TabIndex = 0;
            this.lblKeyTopic.Text = "Key Topic";
            // 
            // tlpPEIEngagementEditable
            // 
            this.tlpPEIEngagementEditable.BackColor = System.Drawing.SystemColors.Control;
            this.tlpPEIEngagementEditable.ColumnCount = 2;
            this.tlpPEIEngagementEditable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementEditable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementEditable.Controls.Add(this.grpEngagementStatus, 0, 0);
            this.tlpPEIEngagementEditable.Controls.Add(this.grpMMDAssignment, 1, 0);
            this.tlpPEIEngagementEditable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPEIEngagementEditable.Location = new System.Drawing.Point(3, 154);
            this.tlpPEIEngagementEditable.Name = "tlpPEIEngagementEditable";
            this.tlpPEIEngagementEditable.RowCount = 2;
            this.tlpPEIEngagementEditable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementEditable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementEditable.Size = new System.Drawing.Size(599, 145);
            this.tlpPEIEngagementEditable.TabIndex = 1;
            // 
            // grpEngagementStatus
            // 
            this.grpEngagementStatus.BackColor = System.Drawing.SystemColors.Control;
            this.grpEngagementStatus.Controls.Add(this.cmbEngagementStatus);
            this.grpEngagementStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpEngagementStatus.Location = new System.Drawing.Point(3, 3);
            this.grpEngagementStatus.Name = "grpEngagementStatus";
            this.grpEngagementStatus.Size = new System.Drawing.Size(293, 66);
            this.grpEngagementStatus.TabIndex = 3;
            this.grpEngagementStatus.TabStop = false;
            this.grpEngagementStatus.Text = "Engagement Status";
            // 
            // cmbEngagementStatus
            // 
            this.cmbEngagementStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEngagementStatus.FormattingEnabled = true;
            this.cmbEngagementStatus.Items.AddRange(new object[] {
            "Open",
            "Closed"});
            this.cmbEngagementStatus.Location = new System.Drawing.Point(10, 28);
            this.cmbEngagementStatus.Name = "cmbEngagementStatus";
            this.cmbEngagementStatus.Size = new System.Drawing.Size(245, 21);
            this.cmbEngagementStatus.TabIndex = 1;
            this.cmbEngagementStatus.Text = "Open";
            // 
            // grpMMDAssignment
            // 
            this.grpMMDAssignment.BackColor = System.Drawing.SystemColors.Control;
            this.grpMMDAssignment.Controls.Add(this.cmbMMDAssignment);
            this.grpMMDAssignment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpMMDAssignment.Location = new System.Drawing.Point(302, 3);
            this.grpMMDAssignment.Name = "grpMMDAssignment";
            this.grpMMDAssignment.Size = new System.Drawing.Size(294, 66);
            this.grpMMDAssignment.TabIndex = 1;
            this.grpMMDAssignment.TabStop = false;
            this.grpMMDAssignment.Text = "MMD Assignment";
            // 
            // cmbMMDAssignment
            // 
            this.cmbMMDAssignment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbMMDAssignment.FormattingEnabled = true;
            this.cmbMMDAssignment.Location = new System.Drawing.Point(9, 28);
            this.cmbMMDAssignment.Name = "cmbMMDAssignment";
            this.cmbMMDAssignment.Size = new System.Drawing.Size(245, 21);
            this.cmbMMDAssignment.TabIndex = 1;
            // 
            // tlpPEIEngagementSubmit
            // 
            this.tlpPEIEngagementSubmit.BackColor = System.Drawing.SystemColors.Control;
            this.tlpPEIEngagementSubmit.ColumnCount = 2;
            this.tlpPEIEngagementSubmit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementSubmit.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpPEIEngagementSubmit.Controls.Add(this.btnAddEngagement, 1, 0);
            this.tlpPEIEngagementSubmit.Controls.Add(this.btnExit, 0, 0);
            this.tlpPEIEngagementSubmit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpPEIEngagementSubmit.Location = new System.Drawing.Point(3, 305);
            this.tlpPEIEngagementSubmit.Name = "tlpPEIEngagementSubmit";
            this.tlpPEIEngagementSubmit.RowCount = 1;
            this.tlpPEIEngagementSubmit.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tlpPEIEngagementSubmit.Size = new System.Drawing.Size(599, 27);
            this.tlpPEIEngagementSubmit.TabIndex = 2;
            // 
            // btnAddEngagement
            // 
            this.btnAddEngagement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddEngagement.BackColor = System.Drawing.SystemColors.Control;
            this.btnAddEngagement.Location = new System.Drawing.Point(401, 3);
            this.btnAddEngagement.Name = "btnAddEngagement";
            this.btnAddEngagement.Size = new System.Drawing.Size(195, 21);
            this.btnAddEngagement.TabIndex = 0;
            this.btnAddEngagement.Text = "Add Engagement To PEI";
            this.btnAddEngagement.UseVisualStyleBackColor = false;
            this.btnAddEngagement.Click += new System.EventHandler(this.btnAddEngagement_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.Control;
            this.btnExit.Location = new System.Drawing.Point(3, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(113, 21);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.SystemColors.Control;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssAddEngagementToPEI});
            this.statusStrip1.Location = new System.Drawing.Point(0, 335);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(605, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssAddEngagementToPEI
            // 
            this.tssAddEngagementToPEI.BackColor = System.Drawing.SystemColors.Control;
            this.tssAddEngagementToPEI.Name = "tssAddEngagementToPEI";
            this.tssAddEngagementToPEI.Size = new System.Drawing.Size(38, 17);
            this.tssAddEngagementToPEI.Text = "Ready";
            // 
            // frmAddEngagementToPEI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 357);
            this.Controls.Add(this.tlpEngagementToPEI);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddEngagementToPEI";
            this.Text = "Add Engagement To PEI";
            this.tlpEngagementToPEI.ResumeLayout(false);
            this.tlpEngagementToPEI.PerformLayout();
            this.tlpPEIEngagementReadOnly.ResumeLayout(false);
            this.grpProviderName.ResumeLayout(false);
            this.grpProviderName.PerformLayout();
            this.grpKeyTopic.ResumeLayout(false);
            this.grpKeyTopic.PerformLayout();
            this.tlpPEIEngagementEditable.ResumeLayout(false);
            this.grpEngagementStatus.ResumeLayout(false);
            this.grpMMDAssignment.ResumeLayout(false);
            this.tlpPEIEngagementSubmit.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpEngagementToPEI;
        private System.Windows.Forms.TableLayoutPanel tlpPEIEngagementReadOnly;
        private System.Windows.Forms.GroupBox grpProviderName;
        private System.Windows.Forms.Label lblProviderName;
        private System.Windows.Forms.GroupBox grpKeyTopic;
        private System.Windows.Forms.Label lblKeyTopic;
        private System.Windows.Forms.TableLayoutPanel tlpPEIEngagementEditable;
        private System.Windows.Forms.GroupBox grpMMDAssignment;
        private System.Windows.Forms.TableLayoutPanel tlpPEIEngagementSubmit;
        private System.Windows.Forms.Button btnAddEngagement;
        private System.Windows.Forms.GroupBox grpEngagementStatus;
        private System.Windows.Forms.ComboBox cmbEngagementStatus;
        private System.Windows.Forms.ComboBox cmbMMDAssignment;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssAddEngagementToPEI;
    }
}