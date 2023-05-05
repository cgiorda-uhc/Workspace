using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class VBC_Bundled : Form
    {
        public VBC_Bundled()
        {
            InitializeComponent();
        }

        DataTable dtReportTypeDatesGLOBAL;
        private void VBC_Bundled_Load(object sender, EventArgs e)
        {

            //refreshReportingPeriod();
            cmbReportingPeriod.Enabled = false;
            cmbBundleType.Enabled = false;
            btnSearchProvider.Enabled = false;

            //CREATE INDEX indx_BP_REPORTING_PERIOD ON BP_DATA(REPORTING_PERIOD);
            //CREATE INDEX indx_BP_BUNDLE_TYPE ON BP_DATA(BUNDLE_TYPE);
            //CREATE INDEX indx_BP_Taxid ON BP_DATA(Taxid);

            //txtTIN.Text = "10211494,10211501,10211503,10211534,10211551,10211797,10212435,10215911";

        }



        DataTable dtFilterOptionsGlobal = null;

        private void btnMatchTin_Click(object sender, EventArgs e)
        {
            clearBundlType();

            dtFilterOptionsGlobal = null;
            if (!String.IsNullOrEmpty(txtTIN.Text))
            {


                try
                {
                    dtFilterOptionsGlobal = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getVBCReportTypeSQL().Replace("{$tin}", txtTIN.Text));
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("No TINs matched DB, try again...");
                    //return;
                    dtFilterOptionsGlobal = new DataTable();
                }


                if (dtFilterOptionsGlobal.Rows.Count <= 0)
                    dtFilterOptionsGlobal = null;
            }

            if (dtFilterOptionsGlobal != null)
            {
                refreshBundlType();
            }
            else
            {
                MessageBox.Show("No TINs matched DB, try again...");
            }
        }

        private void txtTIN_Enter(object sender, EventArgs e)
        {
            clearBundlType();
        }

        private void cmbBundleType_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshReportingPeriod();
        }

        private bool isContractGLOBAL;
        private void cmbReportingPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            isContractGLOBAL = false;
            //DataRow[] drResults = dtFilterOptionsGlobal.Select("BUNDLE_TYPE = '" + cmbBundleType.Text + "' AND REPORTING_PERIOD = '" + cmbReportingPeriod.Text + "'");
            DataRow[] drResults = dtFilterOptionsGlobal.Select("REPORTING_PERIOD = '" + cmbReportingPeriod.Text + "'");
            if(drResults.Count() > 0)
            {
                isContractGLOBAL = (drResults[0]["REPORT_TYPE"].ToString() == "Contract" ? true : false);
            }
                

        }



        private void clearBundlType()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("BUNDLE_TYPE");
            cmbBundleType.DataSource = new BindingSource(dt, null);
            cmbBundleType.Enabled = false;
            clearReportingPeriod();
        }


        private void clearReportingPeriod()
        {
            isContractGLOBAL = false;


            DataTable dt = new DataTable();
            dt.Columns.Add("REPORTING_PERIOD");
            cmbReportingPeriod.DataSource = new BindingSource(dt, null);
            cmbReportingPeriod.Enabled = false;
            btnSearchProvider.Enabled = false;
        }

        private void refreshBundlType()
        {
            clearBundlType();

            DataView view = new DataView(dtFilterOptionsGlobal);
            DataTable distinctValues = view.ToTable(true, "BUNDLE_TYPE");

            DataRow dataRow = distinctValues.NewRow();
            dataRow[0] = "--All--";
            distinctValues.Rows.InsertAt(dataRow, 0);
            //distinctValues = SharedDataTableFunctions.addToNameValueDatatable("--All Types--", "-9999", distinctValues);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = distinctValues;

            cmbBundleType.ValueMember = "BUNDLE_TYPE";
            cmbBundleType.DisplayMember = "BUNDLE_TYPE";
            cmbBundleType.DataSource = bSource;

            cmbBundleType.Enabled = true;

            refreshReportingPeriod();
        }


        private void refreshReportingPeriod()
        {
            clearReportingPeriod();

            DataTable dtTmp = null;
            if(cmbBundleType.Text == "--All--")
            {
                dtTmp = dtFilterOptionsGlobal;
            }
            else
            {
                DataRow[] drResults = dtFilterOptionsGlobal.Select("BUNDLE_TYPE = '" + cmbBundleType.Text + "'");
                dtTmp = drResults.CopyToDataTable();
            }


            DataView view = new DataView(dtTmp);
            DataTable distinctValues = view.ToTable(true, "REPORTING_PERIOD");

            DataRow dataRow = distinctValues.NewRow();
            dataRow[0] = "--All--";
            distinctValues.Rows.InsertAt(dataRow, 0);
            //distinctValues = SharedDataTableFunctions.addToNameValueDatatable("--All Types--", "-9999", distinctValues);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = distinctValues;

            cmbReportingPeriod.ValueMember = "REPORTING_PERIOD";
            cmbReportingPeriod.DisplayMember = "REPORTING_PERIOD";
            cmbReportingPeriod.DataSource = bSource;

            cmbReportingPeriod.Enabled = true;
            btnSearchProvider.Enabled = true;

        }






        private void btnSearchProvider_Click(object sender, EventArgs e)
        {

            if (String.IsNullOrEmpty(txtTIN.Text))
            {
                MessageBox.Show("You must enter a Tin to search...");
                return;
            }

            searchProvider();

        }


        private void searchProvider()
        {
            txtStatus.Text = "";
            exitToolStripMenuItem.Enabled = false;
            this.Cursor = Cursors.WaitCursor;


            bool hasData = false;

            int intExcelRowCnt = 0;



            string strReportTypetColumns = null;

            string strPDFPath = null;

            string strReportingPeriod = cmbReportingPeriod.Text;


            string strTin = txtTIN.Text;



            string strBundleType = cmbBundleType.SelectedValue.ToString();


            try
            {


                if (!isContractGLOBAL)
                {
                    strReportTypetColumns = "CONVERT(VARCHAR(6),ROUND(t1.NATIONAL_RATE * 100,1)) + '%'  as NATIONAL_RATE";
                }
                else
                {
                    strReportTypetColumns = "CONVERT(VARCHAR(6),ROUND(t1.NATIONAL_RATE * 100,1)) + '%'  as BASELINE_CONTRACT_RATE";
                }

                float[] pointColumnWidths;



                string strPDFFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\vbc_files" ;
                if (!Directory.Exists(strPDFFolder))
                    Directory.CreateDirectory(strPDFFolder);


                 strPDFPath = strPDFFolder + "\\vbc_bundled_"+ DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".pdf";
                if (File.Exists(strPDFPath))
                    File.Delete(strPDFPath);

                PDF_Helper.colorTableHeaderBackground = new iText.Kernel.Colors.DeviceRgb(0,38,119);//.ColorConstants.BLUE;
                PDF_Helper.colorTableHeaderForeground = iText.Kernel.Colors.ColorConstants.WHITE;
                PDF_Helper.initializePDF(strPDFPath);


                PDF_Helper.addImage(GlobalObjects.strUHCLogoPath);


                //CREATE INDEX indx_BP_DATA_REPORT_TYPE ON BP_DATA(REPORT_TYPE);
                //CREATE INDEX indx_BP_DATA_TAXID ON BP_DATA(TAXID);
                //CREATE INDEX indx_BP_DATA_BUNDLE_TYPE ON BP_DATA(BUNDLE_TYPE);
                //CREATE INDEX indx_BP_DATA_REPORT_DATE ON BP_DATA(REPORT_DATE);
                //CREATE INDEX indx_BP_DATA_MEASURE_BEGIN_DATE ON BP_DATA(MEASURE_BEGIN_DATE);
                //CREATE INDEX indx_BP_DATA_MEASURE_END_DATE ON BP_DATA(MEASURE_END_DATE);

                //S1 SHEET START///////////////
                //S1 SHEET START///////////////
                //S1 SHEET START///////////////

                //string strSQL = "SELECT Distinct t1.Taxid, t1.GROUP_NAME, t1.Reporting_Period, t1.BUNDLE_TYPE, CONVERT(VARCHAR(10), t1.REPORT_DATE, 101) as REPORT_DATE FROM dbo.BP_DATA t1 WHERE ('-9999' = '" + strBundleType + "' OR t1.BUNDLE_TYPE = '" + strBundleType + "' ) AND t1.REPORTING_PERIOD = '" + strReportingPeriod + "' AND t1.Taxid in (" + strTin + ") AND Convert(DateTime, Convert(VarChar, t1.MEASURE_BEGIN_DATE, 101)) >= '" + strStartDate + "' AND Convert(DateTime, Convert(VarChar, t1.MEASURE_END_DATE, 101)) <= '" + strEndDate + "'";

                string strSQL = "SELECT Distinct t1.Taxid, t1.GROUP_NAME, t1.Reporting_Period, t1.BUNDLE_TYPE, CONVERT(VARCHAR(10), t1.REPORT_DATE, 101) as REPORT_DATE FROM dbo.BP_DATA t1 WHERE ('--All--' = '" + strBundleType + "' OR t1.BUNDLE_TYPE = '" + strBundleType + "' ) AND ('--All--' = '" + strReportingPeriod + "' OR t1.REPORTING_PERIOD = '" + strReportingPeriod + "') AND t1.Taxid in (" + strTin + ") ";

                txtStatus.AppendText("Getting SUMMARY data from DB..." + Environment.NewLine);
                DataTable dt = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL);
                if (dt.Rows.Count > 0)
                {

                    txtStatus.AppendText("Adding data to table..." + Environment.NewLine);

                    pointColumnWidths = new float[] { 1F, 6F, 1F, 2F, 1F };
                    PDF_Helper.addTable(dt, pointColumnWidths);
                    PDF_Helper.addBlankLine();




                    hasData = true;
                    intExcelRowCnt += dt.Rows.Count;
                }
                else
                {
                    txtStatus.AppendText("No data found!" + Environment.NewLine);
                }


                strSQL = "SELECT t1.MEASURE_DESC, t1.Taxid,  CONVERT(VARCHAR(10), t1.MEASURE_BEGIN_DATE, 101) as MEASURE_BEGIN_DATE,  CONVERT(VARCHAR(10), t1.MEASURE_END_DATE, 101) as MEASURE_END_DATE, " + strReportTypetColumns + " FROM dbo.BP_DATA t1  WHERE ('--All--' = '" + strBundleType + "' OR t1.BUNDLE_TYPE = '" + strBundleType + "' ) AND  ('--All--' = '" + strReportingPeriod + "' OR t1.REPORTING_PERIOD = '" + strReportingPeriod + "')  AND t1.Taxid in (" + strTin + ") ORDER BY t1.BUNDLE_TYPE, t1.SORT_ORDER; ";

                //strSQL = "SELECT t1.MEASURE_DESC, t1.Taxid,  CONVERT(VARCHAR(10), t1.MEASURE_BEGIN_DATE, 101) as MEASURE_BEGIN_DATE,  CONVERT(VARCHAR(10), t1.MEASURE_END_DATE, 101) as MEASURE_END_DATE, " + strReportTypetColumns + " FROM dbo.BP_DATA t1  WHERE ('-9999' = '" + strBundleType + "' OR t1.BUNDLE_TYPE = '" + strBundleType + "' ) AND  t1.REPORTING_PERIOD = '" + strReportingPeriod + "'  AND t1.Taxid in (" + strTin + ") AND Convert(DateTime, Convert(VarChar, t1.MEASURE_BEGIN_DATE, 101)) >= '" + strStartDate + "' AND Convert(DateTime, Convert(VarChar, t1.MEASURE_END_DATE, 101)) <= '" + strEndDate + "' ";



                txtStatus.AppendText("Getting MEASURE data from DB..." + Environment.NewLine);
                dt = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL);
                if (dt.Rows.Count > 0)
                {
                    txtStatus.AppendText("Adding data to table..." + Environment.NewLine);


                    pointColumnWidths = new float[] { 7F, 1F, 1F, 1F, 1F };
                    PDF_Helper.addTable(dt, pointColumnWidths);

                    PDF_Helper.addBlankLine();



                    hasData = true;
                    intExcelRowCnt += dt.Rows.Count;
                }
                else
                {
                    txtStatus.AppendText("No data found!" + Environment.NewLine);
                }





               // strSQL = "IF OBJECT_ID('tempdb..#BP_DATA_TMP') IS NOT NULL DROP TABLE #BP_DATA_TMP IF OBJECT_ID('tempdb..#BP_DATA_TOTALS_TMP') IS NOT NULL DROP TABLE #BP_DATA_TOTALS_TMP SELECT t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT AS QUAL_RESULT, t1.Measure_Volume, t1.National_Rate, t1.RESULT INTO #BP_DATA_TMP FROM dbo.BP_DATA t1 WHERE  ('-9999' = '" + strBundleType + "' OR t1.BUNDLE_TYPE = '" + strBundleType + "' ) AND  t1.REPORTING_PERIOD = '" + strReportingPeriod + "'  AND t1.Taxid in (" + strTin + ") AND Convert(DateTime, Convert(VarChar, t1.MEASURE_BEGIN_DATE, 101)) >= '" + strStartDate + "' AND Convert(DateTime, Convert(VarChar, t1.MEASURE_END_DATE, 101)) <= '" + strEndDate + "'; ";




                strSQL = "IF OBJECT_ID('tempdb..#BP_DATA_TMP') IS NOT NULL DROP TABLE #BP_DATA_TMP IF OBJECT_ID('tempdb..#BP_DATA_TOTALS_TMP') IS NOT NULL DROP TABLE #BP_DATA_TOTALS_TMP SELECT t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT AS QUAL_RESULT, t1.Measure_Volume, t1.National_Rate, t1.RESULT,t1.BUNDLE_TYPE, t1.SORT_ORDER INTO #BP_DATA_TMP FROM dbo.BP_DATA t1 WHERE  ('--All--' = '" + strBundleType + "' OR t1.BUNDLE_TYPE = '" + strBundleType + "' ) AND ('--All--' = '" + strReportingPeriod + "' OR  t1.REPORTING_PERIOD = '" + strReportingPeriod + "')  AND t1.Taxid in (" + strTin + "); ";

                strSQL += "WITH t AS ( SELECT MEASURE_DESC, sum(GROUP_DENOMINATOR) as total_events, sum(GROUP_NUMERATOR) as total_yes, SUM(GROUP_NUMERATOR)/SUM(GROUP_DENOMINATOR) as bnchmrk_obsrvd FROM #BP_DATA_TMP GROUP BY MEASURE_DESC ) SELECT t.MEASURE_DESC, t.total_events, t.total_yes, t.bnchmrk_obsrvd, total_events - total_yes as total_no, t.total_events * avg(bnchmrk_obsrvd) as expected_yes, t.total_events - (t.total_events * avg(bnchmrk_obsrvd)) as expected_no, CASE WHEN (t.total_events * avg(bnchmrk_obsrvd)) = 0 OR (t.total_events - (t.total_events * avg(bnchmrk_obsrvd))) = 0 THEN -9999 ELSE ((SQUARE((total_yes - (t.total_events * avg(bnchmrk_obsrvd))))/(t.total_events * avg(bnchmrk_obsrvd))) + (SQUARE(((total_events - total_yes ) - (t.total_events - (t.total_events * avg(bnchmrk_obsrvd)))))/(t.total_events - (t.total_events * avg(bnchmrk_obsrvd))))) END as chisquare_avg, avg(bnchmrk_obsrvd) as bnchmrk_expctd INTO #BP_DATA_TOTALS_TMP FROM t GROUP BY t.MEASURE_DESC,t.total_events, t.total_yes, t.bnchmrk_obsrvd ";

                strSQL += "SELECT tmp.MEASURE_DESC, tmp.TAXID,CONVERT(VARCHAR(6), ROUND(tmp.GROUP_RATE * 100,1)) + '%' as GROUP_RATE, tmp.GROUP_NUMERATOR, tmp.GROUP_DENOMINATOR, tmp.RESULT, tmp.Measure_Volume  FROM ( SELECT t1.MEASURE_DESC as MEASURE_NM_SORT, t1.MEASURE_DESC, t1.TAXID, t1.GROUP_RATE, t1.GROUP_NUMERATOR, t1.GROUP_DENOMINATOR, t1.RESULT As RESULT, t1.Measure_Volume as Measure_Volume, 1 as ROW_NUM, t1.BUNDLE_TYPE, t1.SORT_ORDER FROM #BP_DATA_TMP t1 ";


                if(strTin.Contains(","))
                    strSQL += " UNION ALL select distinct t1.MEASURE_DESC as MEASURE_NM_SORT, 'TOTAL' as MEASURE_DESC, NULL as TAXID, t2.bnchmrk_obsrvd as GROUP_RATE, t2.total_yes as GROUP_NUMERATOR, t2.total_events as GROUP_DENOMINATOR, case when (t2.bnchmrk_obsrvd > t2.bnchmrk_expctd) and (t2.chisquare_avg > 3.841) then 'NOT MET' ELSE 'MET' END as RESULT, CASE WHEN t2.total_events >= 20 THEN 'SUFFICIENT' ELSE 'INSUFFICIENT' END as MEASURE_VOLUME, 2 as ROW_NUM, t1.BUNDLE_TYPE, t1.SORT_ORDER FROM #BP_DATA_TMP t1 INNER JOIN #BP_DATA_TOTALS_TMP t2 ON t1.MEASURE_DESC = t2.MEASURE_DESC";



                //strSQL += ") as tmp ORDER BY tmp.MEASURE_NM_SORT, tmp.ROW_NUM"; 
                strSQL += ") as tmp ORDER BY tmp.BUNDLE_TYPE, tmp.SORT_ORDER, tmp.ROW_NUM";



                txtStatus.AppendText("Getting STATISTICAL_SIGNIFICANCE data from DB..." + Environment.NewLine);
                dt = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL);
                if (dt.Rows.Count > 0)
                {



                    txtStatus.AppendText("Adding data to table..." + Environment.NewLine);

                    pointColumnWidths = new float[] { 6F, 1F, 1F, 1F, 1F, 1F, 1F };
                    PDF_Helper.addTable(dt, pointColumnWidths);


                    hasData = true;
                    intExcelRowCnt += dt.Rows.Count;
                }
                else
                {
                    txtStatus.AppendText("No data found!" + Environment.NewLine);
                }
                //S3 SHEET END///////////////
                //S3 SHEET END///////////////
                //S3 SHEET END///////////////


                //strRange = "H" + (intExcelRowCnt + (intExcelStartRow) + (intExcelGapCnt * 2));


                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                txtStatus.AppendText("Generating Final PDF File for MPIN..."  + Environment.NewLine);
                if (hasData)
                {
                    PDF_Helper.finalizerPDF();
                }
                else
                {
                    txtStatus.AppendText("NO DATA WAS FOUND" + Environment.NewLine);

                    if (File.Exists(strPDFPath))
                        File.Delete(strPDFPath);
                    MessageBox.Show("NO DATA WAS FOUND");

                }

            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was a GENERAL error, see details below" + Environment.NewLine + Environment.NewLine);

                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);

                if (File.Exists(strPDFPath))
                    File.Delete(strPDFPath);
            }
            finally
            {

                txtStatus.AppendText("Completed!" + Environment.NewLine);

                this.Cursor = Cursors.Default;
                exitToolStripMenuItem.Enabled = true;
            }
        }




        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void VBC_Bundled_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (exitToolStripMenuItem.Enabled == false)
            {
                e.Cancel = false;
            }
        }

     

        private void filesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            string strPDFFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\vbc_files";
            if (!Directory.Exists(strPDFFolder))
                Directory.CreateDirectory(strPDFFolder);

            Process.Start(strPDFFolder);
        }

        frmComplianceReporting frmComplaintsGlobal;
        private void complaintsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (frmComplaintsGlobal == null)
            {
                this.Cursor = Cursors.WaitCursor;
                frmComplaintsGlobal = new frmComplianceReporting();
                this.Cursor = Cursors.Default;
            }
                

            //frmColumnsGlobal.populateColumns(radPractice.Checked);
            frmComplaintsGlobal.ShowDialog();

            



        }
    }
}
