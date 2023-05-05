using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmExcelParser : _BaseClass
    {
        public frmExcelParser()
        {
            InitializeComponent();

           this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(frmExcelParser_DragEnter);
            this.DragDrop += new DragEventHandler(frmExcelParser_DragDrop);


            txtExcelFilePath.Text = @"C:\Work\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\PhysicianFeedbackTracker\PR_Load_Sample.xlsx";
        }



        private void frmExcelParser_DragEnter(object sender, DragEventArgs e)
        {

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void frmExcelParser_DragDrop(object sender, DragEventArgs e)
        {
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if(FileList.Count() > 0)
            {
                //StringBuilder sb = new StringBuilder();
                //foreach (string File in FileList)
                //    sb.Append( " " + File);
                //txtExcelFilePath.Text = sb.ToString();

                string strFileName = FileList[0];
                if(strFileName.EndsWith(".xls") || strFileName.EndsWith(".xlsx"))
                    txtExcelFilePath.Text = strFileName;

            }

        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                txtExcelFilePath.Text = openFileDialog1.FileName;

        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtExcelFilePath_TextChanged(object sender, EventArgs e)
        {
            if (txtExcelFilePath.Text.EndsWith(".xls") || txtExcelFilePath.Text.EndsWith(".xlsx"))
                btnParseExcelFile.Enabled = true;
            else
                btnParseExcelFile.Enabled = false;
        }

        private void btnParseExcelFile_Click(object sender, EventArgs e)
        {
            DataTable dtMain = SharedDataTableFunctions.getDataTableFromExcel(txtExcelFilePath.Text, "Validate_Data");
            DataTable dtRules = SharedDataTableFunctions.getDataTableFromExcel(txtExcelFilePath.Text, "Validate_Data_Rules");
            DataTable dtActions = SharedDataTableFunctions.getDataTableFromExcel(txtExcelFilePath.Text, "Actions");

            DataSet dsFinal = DataValidation.getValidatedData(dtMain, dtRules, dtActions, ref txtParseExcelResults);


            txtParseExcelResults.AppendText("Populating Validation Results..." + Environment.NewLine);

            ClosedXMLExcelFunctions.addValidationFeedbackSheet(txtExcelFilePath.Text, dsFinal.Tables["dtOverviewFinal"], dsFinal.Tables["dtMainFinal"], "Validate_Data_Results");
            //ClosedXMLExcelFunctions.addValidationFeedbackSheet(txtExcelFilePath.Text, dsFinal.Tables["dtOverviewFinal"], dtMain, "Validate_Data_Results");


            txtParseExcelResults.AppendText("Validation Complete!" + Environment.NewLine);
            txtParseExcelResults.AppendText("Opening Excel..." + Environment.NewLine);

            Process.Start(txtExcelFilePath.Text);

        }
    }
}
