using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmQACompanion : _BaseClass
    {
        public frmQACompanion()
        {
            InitializeComponent();
        }

        private void frmQACompanion_Load(object sender, EventArgs e)
        {
            DataTable dtTmp = GlobalObjects.getNameValueDataTable("phase");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--All Projects--", "-9999", dtTmp);
            cmbPhase.DataSource = dtTmp;
            cmbPhase.DisplayMember = "name";
            cmbPhase.ValueMember = "value";
        }

        private void cmbPhase_SelectedIndexChanged(object sender, EventArgs e)
        {

            //ComboBox senderComboBox = (ComboBox)sender;

            //if(senderComboBox.SelectedIndex != 0)
            //{
            //    MessageBox.Show("Suffix = " + senderComboBox.SelectedValue.ToString());

            //}
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        CancellationTokenSource source = null;
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if(source != null)
            {
                txtStatus.AppendText("-----------------------------------------------------------------------" + Environment.NewLine);
                txtStatus.AppendText("Cancel Requested...." + Environment.NewLine);
                source.Cancel();
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            string strPhase = cmbPhase.SelectedValue.ToString();
            if (strPhase == "-9999")
            {
                MessageBox.Show("Choose a Project First!");
                return;
            }



            // Add measures
            //fix mpins
            //Add sampling SQL To Excel
            //Fix Sampling querie: PR_MPIN in (Select distinct MPIN from PBP_Profile_px_Ph32 Where Measure_ID = 38 and signif = 'Yes')
            //Select* from dbo.PBP_dim_Measures






            txtStatus.Text = "";

            Hashtable ht = GlobalObjects.htMeasureQuerySQL(strPhase);
            DataTable dt = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strMeasureQuerySQL, ht);

            //DEFAULTS
            Int16 intSampleSize = 10;
            string strMPINQuery = " Measure_id in ({$measure_id}) ";

            //DEFAULTS OVERRRIDE
            if (!string.IsNullOrEmpty(txtChooseMpin.Text))
            {
                intSampleSize = (Int16)txtChooseMpin.Text.Split(',').Count();
                strMPINQuery = " MPIN in ("+ txtChooseMpin.Text + ") ";

            }
            else if (txtSampleSize.Text.IsNumeric())
            {
                intSampleSize = Int16.Parse(txtSampleSize.Text);
            }
                


            

            StringBuilder sbSQLSamplingOuter = new StringBuilder();
            sbSQLSamplingOuter.Append("Select SUBSTRING( ");
            sbSQLSamplingOuter.Append("( ");
            sbSQLSamplingOuter.Append("SELECT TOP "+ intSampleSize + " ',' + CAST(CAST(MPIN as BIGINT) as Varchar) ");
            sbSQLSamplingOuter.Append("FROM ");
            sbSQLSamplingOuter.Append("( ");
            sbSQLSamplingOuter.Append(" SELECT distinct t.MPIN FROM ( select MPIN, Measure_id FROM PBP_Profile_px_Ph{$suffix} WHERE signif = 'Yes' UNION select MPIN, Measure_id FROM PBP_Profile_Ph{$suffix} WHERE signif = 'Yes' ) t WHERE "+ strMPINQuery  + " ");
            sbSQLSamplingOuter.Append(") tmp ");
            sbSQLSamplingOuter.Append("ORDER BY NEWID() ");
            sbSQLSamplingOuter.Append("FOR XML PATH('') ");
            sbSQLSamplingOuter.Append("), 2 , 9999) As MPIN ");


            //ClosedXMLExcelFunctions.addQAResultsLoader(dt, sbSQLSamplingOuter.ToString(), strPhase, ref txtParseExcelResults);

            try
            {
                await TryTask(dt, sbSQLSamplingOuter.ToString(), strPhase);
                txtStatus.AppendText("Process Complete!!!");
            }
            catch(Exception)
            {
                txtStatus.AppendText("Process Cancelled");
            }
          
        }

        private async Task TryTask(DataTable dt, string strSamplingOuter, string strPhase)
        {


            source = new CancellationTokenSource();
            //source.CancelAfter(TimeSpan.FromSeconds(1));
            //Task task = Task.Run(() => ClosedXMLExcelFunctions.addQAResultsLoader(dt, strSamplingOuter, strPhase, ref txtStatus, source.Token), source.Token);


            string strFile = await Task.Run(() => ClosedXMLExcelFunctions.addQAResultsLoader(dt, strSamplingOuter, strPhase, ref txtStatus, source.Token), source.Token);

            if(strFile != null)
            {

                string strPath = Environment.ExpandEnvironmentVariables(GlobalObjects.strQACompanion_Reports_Path);
                //string strNewFile = "QA_Companion_Results_" +  DateTime.Now.Month  + DateTime.Now.Day + DateTime.Now.Year + ".xlsx";

                //if (File.Exists(strPath + "\\" + strNewFile))
                //    File.Delete(strPath + "\\" + strNewFile);

                //if (!Directory.Exists(strPath))
                //Directory.CreateDirectory(strPath);

                //File.Copy(strFile, strPath + "//" + strNewFile);


                //File.Delete(strFile);

                //Process.Start(strPath);

                Process.Start(strPath);

            }
            else
            {
                txtStatus.AppendText(Environment.NewLine + Environment.NewLine + "NO VALID MEASURES TO DISPLAY!!!" + Environment.NewLine + Environment.NewLine);
            }


            // (A canceled task will raise an exception when awaited).
            //string val = await task;
        }

      
    }
}
