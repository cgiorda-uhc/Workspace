namespace PhysicianFeedbackTracker
{
    partial class frmDuplicateConfirmation
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
            this.tlpDuplicateProvider = new System.Windows.Forms.TableLayoutPanel();
            this.tlpConfirmDuplicates = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddProviderYes = new System.Windows.Forms.Button();
            this.btnAddProviderNo = new System.Windows.Forms.Button();
            this.lblDuplicateMessage = new System.Windows.Forms.Label();
            this.dgvDuplicateProviders = new System.Windows.Forms.DataGridView();
            this.tlpDuplicateProvider.SuspendLayout();
            this.tlpConfirmDuplicates.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateProviders)).BeginInit();
            this.SuspendLayout();
            // 
            // tlpDuplicateProvider
            // 
            this.tlpDuplicateProvider.ColumnCount = 1;
            this.tlpDuplicateProvider.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpDuplicateProvider.Controls.Add(this.tlpConfirmDuplicates, 0, 0);
            this.tlpDuplicateProvider.Controls.Add(this.dgvDuplicateProviders, 0, 1);
            this.tlpDuplicateProvider.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDuplicateProvider.Location = new System.Drawing.Point(0, 0);
            this.tlpDuplicateProvider.Name = "tlpDuplicateProvider";
            this.tlpDuplicateProvider.RowCount = 2;
            this.tlpDuplicateProvider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 8.453609F));
            this.tlpDuplicateProvider.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 91.54639F));
            this.tlpDuplicateProvider.Size = new System.Drawing.Size(1080, 485);
            this.tlpDuplicateProvider.TabIndex = 0;
            // 
            // tlpConfirmDuplicates
            // 
            this.tlpConfirmDuplicates.ColumnCount = 3;
            this.tlpConfirmDuplicates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 827F));
            this.tlpConfirmDuplicates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 46.51163F));
            this.tlpConfirmDuplicates.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 53.48837F));
            this.tlpConfirmDuplicates.Controls.Add(this.btnAddProviderYes, 1, 0);
            this.tlpConfirmDuplicates.Controls.Add(this.btnAddProviderNo, 2, 0);
            this.tlpConfirmDuplicates.Controls.Add(this.lblDuplicateMessage, 0, 0);
            this.tlpConfirmDuplicates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpConfirmDuplicates.Location = new System.Drawing.Point(3, 3);
            this.tlpConfirmDuplicates.Name = "tlpConfirmDuplicates";
            this.tlpConfirmDuplicates.RowCount = 1;
            this.tlpConfirmDuplicates.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpConfirmDuplicates.Size = new System.Drawing.Size(1074, 35);
            this.tlpConfirmDuplicates.TabIndex = 0;
            // 
            // btnAddProviderYes
            // 
            this.btnAddProviderYes.Location = new System.Drawing.Point(830, 3);
            this.btnAddProviderYes.Name = "btnAddProviderYes";
            this.btnAddProviderYes.Size = new System.Drawing.Size(75, 23);
            this.btnAddProviderYes.TabIndex = 0;
            this.btnAddProviderYes.Text = "Yes";
            this.btnAddProviderYes.UseVisualStyleBackColor = true;
            this.btnAddProviderYes.Click += new System.EventHandler(this.btnAddProviderYes_Click);
            // 
            // btnAddProviderNo
            // 
            this.btnAddProviderNo.Location = new System.Drawing.Point(944, 3);
            this.btnAddProviderNo.Name = "btnAddProviderNo";
            this.btnAddProviderNo.Size = new System.Drawing.Size(75, 23);
            this.btnAddProviderNo.TabIndex = 1;
            this.btnAddProviderNo.Text = "No";
            this.btnAddProviderNo.UseVisualStyleBackColor = true;
            this.btnAddProviderNo.Click += new System.EventHandler(this.btnAddProviderNo_Click);
            // 
            // lblDuplicateMessage
            // 
            this.lblDuplicateMessage.AutoSize = true;
            this.lblDuplicateMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDuplicateMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDuplicateMessage.Location = new System.Drawing.Point(3, 0);
            this.lblDuplicateMessage.Name = "lblDuplicateMessage";
            this.lblDuplicateMessage.Size = new System.Drawing.Size(821, 35);
            this.lblDuplicateMessage.TabIndex = 2;
            this.lblDuplicateMessage.Text = "At least one provider you selected has already been added to this project. Are yo" +
    "u sure you want to add them again?";
            // 
            // dgvDuplicateProviders
            // 
            this.dgvDuplicateProviders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDuplicateProviders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDuplicateProviders.Location = new System.Drawing.Point(3, 44);
            this.dgvDuplicateProviders.Name = "dgvDuplicateProviders";
            this.dgvDuplicateProviders.Size = new System.Drawing.Size(1074, 438);
            this.dgvDuplicateProviders.TabIndex = 1;
            // 
            // frmDuplicateConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1080, 485);
            this.ControlBox = false;
            this.Controls.Add(this.tlpDuplicateProvider);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDuplicateConfirmation";
            this.Text = "Duplicate Provider(s) Found";
            this.tlpDuplicateProvider.ResumeLayout(false);
            this.tlpConfirmDuplicates.ResumeLayout(false);
            this.tlpConfirmDuplicates.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDuplicateProviders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpDuplicateProvider;
        private System.Windows.Forms.TableLayoutPanel tlpConfirmDuplicates;
        private System.Windows.Forms.Button btnAddProviderYes;
        private System.Windows.Forms.Button btnAddProviderNo;
        private System.Windows.Forms.Label lblDuplicateMessage;
        public System.Windows.Forms.DataGridView dgvDuplicateProviders;
    }
}