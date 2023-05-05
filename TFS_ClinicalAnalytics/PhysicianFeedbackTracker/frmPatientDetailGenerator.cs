using System;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmPatientDetailGenerator : _BaseClass
    {
        public frmPatientDetailGenerator()
        {
            InitializeComponent();
        }

        public string strMPIN = null;
        public string strProject = null;

        private void frmPatientDetailGenerator_Load(object sender, EventArgs e)
        {
            DataTable dtTmp = GlobalObjects.getNameValueDataTable("phase");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--All Projects--", "-9999", dtTmp);
            cmbPhase.DataSource = dtTmp;
            cmbPhase.DisplayMember = "name";
            cmbPhase.ValueMember = "value";


            if (strMPIN != null)
                txtMPIN.Text = strMPIN;

            if (strProject != null)
                cmbPhase.Text = strProject;


        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Generating this report may close any open Excel files. Be sure to save any open Excel files before continuing. Are you sure you want to continue?", "Save Excel Files!!!", MessageBoxButtons.YesNo); //ALWAYS CONFIRM FIRST
            if (confirmResult == DialogResult.No)
                return;

            string strStatus = null;

            try
            {
                exitToolStripMenuItem.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
                strStatus = MemberDetails.generateMemberDetails(cmbPhase.Text, txtMPIN.Text, ref txtStatus);



                if(chkGenerateEmail.Checked)
                {
                    string strEmailAddress = "physician_engagement@uhc.com";
                    //string strEmailAddress = "chris_giordano@uhc.com";
                    string strEmailCC = "";
                    string strSubject = "PCR "+ MemberDetails.strProjectNameGLOBAL + " MPIN " + MemberDetails.strMPINGLOBAL + " " + MemberDetails.strLastNameGLOBAL + " *Confidential-PHI data*";
                    string strBody = "*--SecureDelivery--*";
                    string strAttachmentPath = MemberDetails.strFilePathGLOBAL;


                    OutlookHelper.generateEmail(strEmailAddress, strEmailCC, strSubject, strBody, strAttachmentPath);
                }



                if(chkCleanExcel.Checked)
                {
                    SharedFunctions.killProcess("EXCEL");
                }



            }
            finally
            {
                if (strStatus != null)
                {
                    MessageBox.Show(strStatus);
                }
                this.Cursor = Cursors.Default;
                exitToolStripMenuItem.Enabled = true;
            }

            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPatientDetailGenerator_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(exitToolStripMenuItem.Enabled == false)
            {
                e.Cancel = false;
            }
        }
    }
}
