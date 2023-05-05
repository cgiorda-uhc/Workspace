namespace PhysicianFeedbackTracker
{
    partial class frmHome
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
            this.btnAddProviders = new System.Windows.Forms.Button();
            this.btnGetTrackingRecords = new System.Windows.Forms.Button();
            this.btnClearTrackingRecords = new System.Windows.Forms.Button();
            this.btnQAWorkFlow = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnAddProviders
            // 
            this.btnAddProviders.Location = new System.Drawing.Point(569, 24);
            this.btnAddProviders.Name = "btnAddProviders";
            this.btnAddProviders.Size = new System.Drawing.Size(175, 23);
            this.btnAddProviders.TabIndex = 0;
            this.btnAddProviders.Text = "Add Providers to Track";
            this.btnAddProviders.UseVisualStyleBackColor = true;
            this.btnAddProviders.Click += new System.EventHandler(this.btnAddProviders_Click);
            // 
            // btnGetTrackingRecords
            // 
            this.btnGetTrackingRecords.Location = new System.Drawing.Point(761, 24);
            this.btnGetTrackingRecords.Name = "btnGetTrackingRecords";
            this.btnGetTrackingRecords.Size = new System.Drawing.Size(176, 23);
            this.btnGetTrackingRecords.TabIndex = 1;
            this.btnGetTrackingRecords.Text = "Get Tracking Records";
            this.btnGetTrackingRecords.UseVisualStyleBackColor = true;
            // 
            // btnClearTrackingRecords
            // 
            this.btnClearTrackingRecords.Location = new System.Drawing.Point(958, 24);
            this.btnClearTrackingRecords.Name = "btnClearTrackingRecords";
            this.btnClearTrackingRecords.Size = new System.Drawing.Size(211, 23);
            this.btnClearTrackingRecords.TabIndex = 2;
            this.btnClearTrackingRecords.Text = "Clear Tracking Records";
            this.btnClearTrackingRecords.UseVisualStyleBackColor = true;
            // 
            // btnQAWorkFlow
            // 
            this.btnQAWorkFlow.Location = new System.Drawing.Point(24, 24);
            this.btnQAWorkFlow.Name = "btnQAWorkFlow";
            this.btnQAWorkFlow.Size = new System.Drawing.Size(181, 23);
            this.btnQAWorkFlow.TabIndex = 3;
            this.btnQAWorkFlow.Text = "Create QA Work Item";
            this.btnQAWorkFlow.UseVisualStyleBackColor = true;
            this.btnQAWorkFlow.Click += new System.EventHandler(this.btnQAWorkFlow_Click);
            // 
            // frmHome
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1232, 831);
            this.Controls.Add(this.btnQAWorkFlow);
            this.Controls.Add(this.btnClearTrackingRecords);
            this.Controls.Add(this.btnGetTrackingRecords);
            this.Controls.Add(this.btnAddProviders);
            this.Name = "frmHome";
            this.Text = "Physician Feedback Tracker";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnAddProviders;
        private System.Windows.Forms.Button btnGetTrackingRecords;
        private System.Windows.Forms.Button btnClearTrackingRecords;
        private System.Windows.Forms.Button btnQAWorkFlow;
    }
}

