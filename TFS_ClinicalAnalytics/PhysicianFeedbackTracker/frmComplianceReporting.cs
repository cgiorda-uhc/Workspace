using ClosedXML.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    public partial class frmComplianceReporting : Form
    {
        //CERT LINK
        //https://certificateservices.optum.com/vedadmin/
        //https://certificateservices.optum.com/vedadmin/
        //https://certificateservices.optum.com/vedadmin/
        //https://certificateservices.optum.com/vedadmin/
        //https://certificateservices.optum.com/vedadmin/


        DataTable dtProviderFiltersGLOBAL = null;
        DataTable dtMeasureFiltersGLOBAL = null;
        DataTable dtFacilityTypeFiltersGLOBAL = null;


        frmComplianceReportingColumns frmColumnsGlobal;
        frmComplianceReporting_Filters frmFiltersGlobal;

        //FOR PASTE OVERRIDE TINS TO COMBO START
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        //FOR PASTE OVERRIDE TINS TO COMBO END

        public frmComplianceReporting()
        {
            try
            {
                InitializeComponent();

                //FOR PASTE OVERRIDE TINS TO COMBO START
                IntPtr lhWnd = FindWindowEx(cmbProvider.Handle, IntPtr.Zero, "EDIT", null);
                CleanProviderPaste p = new CleanProviderPaste();
                p.AssignHandle(lhWnd);
                //FOR PASTE OVERRIDE TINS TO COMBO END


                //TESTING SECURE PW BUG!!!!
                //GlobalObjects.strILUCAConnectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";

                //return;
                frmFiltersGlobal = new frmComplianceReporting_Filters();
                frmFiltersGlobal.VisibleChanged += new System.EventHandler(this.filterForm_Visibility);
                frmColumnsGlobal = new frmComplianceReportingColumns();
                frmColumnsGlobal.VisibleChanged += new System.EventHandler(this.filterForm_Visibility);

                //POPULATE DEFAULT FILTERS (ALL)
                getFilterMaster();

                btnQuarterFilters.Click += new System.EventHandler(this.btnFilters_Click);
                btnMeasureFilters.Click += new System.EventHandler(this.btnFilters_Click);
                btnSpecialtyFilters.Click += new System.EventHandler(this.btnFilters_Click);
                btnSurgicalFilters.Click += new System.EventHandler(this.btnFilters_Click);
                btnProviderFilters.Click += new System.EventHandler(this.btnFilters_Click);
                btnFacilityTypeFilters.Click += new System.EventHandler(this.btnFilters_Click);
                btnMarketFilters.Click += new System.EventHandler(this.btnFilters_Click);

                //DISBALE KEY PRESSES
                cmbMeasure.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_KeyPressDisable);
                cmbSpecialty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_KeyPressDisable);
                cmbQuarter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_KeyPressDisable);
                cmbSurgical.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_KeyPressDisable);
                cmbMarket.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_KeyPressDisable);

                radFacility.CheckedChanged += new System.EventHandler(this.radProvider_CheckedChanged);
                radPractice.CheckedChanged += new System.EventHandler(this.radProvider_CheckedChanged);


                //PROVIDER FILTERS
                refreshFacilityTypeFilters();
                refreshProviderFilters();
                //INDEPENDENT FILTERS
                refreshSampleSizeFilters();
                refreshConfidenceFilters();
                refreshMeasureFilters();

                //INTER-RELATED FILTERS
                refreshQuarterFilters();
                refreshSurgicalFilters();
                refreshSpecialtyFilters();
                refreshMarketFilters();
                refreshLOBFilters();


                cleanFacilityTypeFilters(false);
                cleanProviderFilters(false);

                cleanMeasureFilters(false);

                cleanQuarterFilters(false);
                cleanSurgicalFilters(false);
                cleanSpecialtyFilters(false);
                cleanMarketFilters(false);


                foreach (var cb in Controls.OfType<ComboBox>())
                {
                    cb.Resize += (sender, e) => {
                        if (!cb.Focused)
                            cb.SelectionLength = 0;
                    };
                }


                if (frmColumnsGlobal == null)
                    frmColumnsGlobal = new frmComplianceReportingColumns();



                //var headerColor = Color.Black;
                //var success = Color.Green;
                //var failure = Color.Red;
                //var message = Color.DarkGray;
                //var normal = txtStatus.Font;
                //var noBold = new Font(normal, FontStyle.Regular);
                //var bold = new Font(normal, FontStyle.Bold);

                //grpMeasureFilters.BackColor = Color.DarkBlue ;
                //grpProviderFilters.BackColor = Color.DarkGreen;
                //grpAdditionalFilters.BackColor = Color.DarkMagenta;
                //btnGenerateReport.BackColor = Color.DarkMagenta;
                //btnGetFilters.BackColor =  Color.DarkOrange;
                addDefaultReadyStatus();


            }
            catch (Exception ex)
            {
                cleanupError(ex.Message);
            }


        }
 
        #region CLEAN DROPDOWNLIST FILTERS
        private void cleanQuarterFilters(bool blShowFilters)
        {
            btnQuarterFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnQuarterFilters.Visible = blShowFilters;
            if (blShowFilters)
            {

                tlpQuarterFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 95);
                tlpQuarterFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 5);
            }
            else
            {
                tlpQuarterFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpQuarterFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
        }

        private void cleanFacilityTypeFilters(bool blShowFilters)
        {
            btnFacilityTypeFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnFacilityTypeFilters.Visible = blShowFilters;
            if (blShowFilters)
            {

                tlpFacilityTypeFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 93);
                tlpFacilityTypeFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 7);
            }
            else
            {
                tlpFacilityTypeFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpFacilityTypeFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
        }

        private void cleanProviderFilters(bool blShowFilters)
        {
            btnProviderFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnProviderFilters.Visible = blShowFilters;
            if (blShowFilters)
            {

                tlpProviderFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 95);
                tlpProviderFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 5);
            }
            else
            {
                tlpProviderFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpProviderFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
            //SUPRESS COMBOX DROP 3172021
            //cmbProvider.DataSource = null;
            //cmbProvider.Items.Clear();
            //cmbProvider.ResetText();
            //cmbProvider.DroppedDown = false;
            //cmbProvider.SelectedIndex = -1;
            //cmbProvider.Text = "";
            //cmbProvider.SelectionStart = 0;
            //cmbProvider.SelectionLength = 0;


           
        }

        private void cleanSurgicalFilters(bool blShowFilters)
        {
            btnSurgicalFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnSurgicalFilters.Visible = blShowFilters;
            if (blShowFilters)
            {

                tlpSurgicalFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 95);
                tlpSurgicalFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 5);
            }
            else
            {
                tlpSurgicalFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpSurgicalFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
        }

        private void cleanMeasureFilters(bool blShowFilters)
        {
            btnMeasureFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnMeasureFilters.Visible = blShowFilters;
            if (blShowFilters)
            {

                tlpMeasureFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 95);
                tlpMeasureFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 5);
            }
            else
            {
                tlpMeasureFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpMeasureFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
        }



        private void cleanMarketFilters(bool blShowFilters)
        {
            btnMarketFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnMarketFilters.Visible = blShowFilters;
            if (blShowFilters)
            {

                tlpMarketFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 95);
                tlpMarketFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 5);
            }
            else
            {
                tlpMarketFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpMarketFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
        }

        private void cleanSpecialtyFilters(bool blShowFilters)
        {
            btnSpecialtyFilters.BackgroundImageLayout = ImageLayout.Stretch;
            btnSpecialtyFilters.Visible = blShowFilters;
            if (blShowFilters)
            {
                tlpSpecialtyFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 95);
                tlpSpecialtyFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 5);
            }
            else
            {
                tlpSpecialtyFilter.ColumnStyles[0] = new ColumnStyle(SizeType.Percent, 100);
                tlpSpecialtyFilter.ColumnStyles[1] = new ColumnStyle(SizeType.Percent, 0);
            }
        }
        private void clearAllFilters()
        {
            if (frmFiltersGlobal != null)
            {
                frmFiltersGlobal.dicMarketFiltersGlobal.Clear();
                frmFiltersGlobal.dicQuarterFiltersGlobal.Clear();
                frmFiltersGlobal.dicSurgicalFiltersGlobal.Clear();
                frmFiltersGlobal.dicSpecialtyFiltersGlobal.Clear();
            }
            //cmbQuarter.SelectedIndex = -1;
            //cmbLOB.SelectedIndex = -1;
            cmbQuarter.SelectedIndex = 0;
            cmbLOB.SelectedIndex = 0;
        }
        #endregion

        #region REFRESH DROPDOWNLIST FILTERS
        private void refreshFacilityTypeFilters()
        {
            if (dtFacilityTypeFiltersGLOBAL == null)
            {
                string strSQL = "select distinct CAST(fac_type as varchar(25)) as fac_type,  CAST(fac_type as varchar(25)) as fac_type_val from dbo.compl_app WHERE fac_type is NOT NULL  order by fac_type";
                dtFacilityTypeFiltersGLOBAL = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL);
                DataRow dataRow = dtFacilityTypeFiltersGLOBAL.NewRow();
                dataRow["fac_type_val"] = -9999;
                dataRow["fac_type"] = "~All Facility Types~";
                dtFacilityTypeFiltersGLOBAL.Rows.InsertAt(dataRow, 0);
            }

            BindingSource bSource = new BindingSource();
            if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
            {
                //DataTable dtTmp = dtFacilityTypeFiltersGLOBAL.Select("fac_type_val not in (" + string.Join(",", frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Keys) + ")").CopyToDataTable();

                var csv = String.Join(",", Array.ConvertAll(frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Keys.ToArray(), z => "'" + z + "'"));
                DataTable dtTmp = dtFacilityTypeFiltersGLOBAL.Select("fac_type_val not in (" + csv + ")").CopyToDataTable();


                DataRow dr = dtTmp.Select("fac_type_val='-9999'").FirstOrDefault();
                if (dr != null)
                    dr["fac_type"] = "(FILTERED)";

                dtTmp.DefaultView.Sort = "fac_type";
                dtTmp = dtTmp.DefaultView.ToTable();
                bSource.DataSource = dtTmp;
            }
            else
                bSource.DataSource = dtFacilityTypeFiltersGLOBAL;

            cmbFacilityType.ValueMember = "fac_type_val";
            cmbFacilityType.DisplayMember = "fac_type";
            cmbFacilityType.DataSource = bSource;

        }

        private void refreshQuarterFilters()
        {
            DataView viewTmp;

            if (dtLiveFilters_GLOBAL == null)
                viewTmp = new DataView(dtCurrentFilters_GLOBAL);
            else
                viewTmp = new DataView(dtLiveFilters_GLOBAL);


            DataTable dtQuarterFilters = viewTmp.ToTable(true, "lst_run_qrt", "lst_run_qrt_val");
            dtQuarterFilters.DefaultView.Sort = "lst_run_qrt DESC";
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dtQuarterFilters;

            cmbQuarter.ValueMember = "lst_run_qrt_val";
            cmbQuarter.DisplayMember = "lst_run_qrt";
            cmbQuarter.DataSource = bSource;

        }

        private void refreshLOBFilters()
        {

            if (dtLiveFilters_GLOBAL == null)
                updateFilters();

            DataView viewTmp = new DataView(dtLiveFilters_GLOBAL);
            DataTable dtLOBFilters = viewTmp.ToTable(true, "lob_desc", "lob_id");

            DataRow dataRow = dtLOBFilters.NewRow();
            dataRow["lob_id"] = 4;
            dataRow["lob_desc"] = "~All LOBs~";
            dtLOBFilters.Rows.InsertAt(dataRow, 0);

            dtLOBFilters.DefaultView.Sort = "lob_desc ASC";
            BindingSource bSource = new BindingSource();
            bSource.DataSource = dtLOBFilters;

            cmbLOB.ValueMember = "lob_id";
            cmbLOB.DisplayMember = "lob_desc";
            cmbLOB.DataSource = bSource;


        }

        private void refreshConfidenceFilters()
        {


            // Bind combobox to dictionary
            Dictionary<string, string> dicConfidenceValues = new Dictionary<string, string>();
            dicConfidenceValues.Add("3.841", "95% (Recommended)");
            dicConfidenceValues.Add("2.706", "98% (PD Users)");
            dicConfidenceValues.Add("5.412", "90% (Least Conservative)");
            cmbConfidence.DataSource = new BindingSource(dicConfidenceValues, null);
            cmbConfidence.DisplayMember = "Value";
            cmbConfidence.ValueMember = "Key";


        }
        private void refreshSampleSizeFilters()
        {
            // Bind combobox to dictionary
            Dictionary<string, string> dicSampleSizeValues = new Dictionary<string, string>();
            dicSampleSizeValues.Add("20", "20");
            dicSampleSizeValues.Add("10", "10");
            dicSampleSizeValues.Add("5", "5");
            cmbSampleSize.DataSource = new BindingSource(dicSampleSizeValues, null);
            cmbSampleSize.DisplayMember = "Value";
            cmbSampleSize.ValueMember = "Key";

        }

        private void refreshProviderFilters()
        {
            //select distinct claim_tin, Pract_Name into [dbo].compl_aggr_prac_provider_cache from dbo.compl_app ;
            if (dtProviderFiltersGLOBAL == null)
            {
                //string strSQL = "select distinct claim_tin, Pract_Name from dbo.compl_app order by Pract_Name";
                //string strSQL = "select  [Pract_Name],[claim_tin], [claim_tin_num] from [dbo].compl_aggr_prac_provcache order by [Pract_Name]";
                string strSQL = "select  provider_name,tin, tin_num, provider_type, fac_type  from dbo.compl_app_provcache order by provider_name";
                dtProviderFiltersGLOBAL = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL);
            }
        }

        private void refreshSurgicalFilters()
        {

            BindingSource bSource = new BindingSource();
            DataView viewTmp;
            DataTable dtSurgicalFilters = null;
            DataRow dataRow;
            DataRow[] dataRowArrTmp;

            //CURRENT DROPDOWN CANT LIMIT ITSELF!!! 
            //REMOVE DIRECT FROM DROPDOWN
            DataTable dtDirect = null;
            if (strCurrentFilterGLOBAL == "Surgical")
            {
                if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
                {
                    bSource = (BindingSource)cmbSurgical.DataSource; // Se convierte el DataSource 
                    dtDirect = (DataTable)bSource.DataSource;
                }
                strCurrentFilterGLOBAL = null;
            }


            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
            {
                if(dtDirect != null)
                    dataRowArrTmp = dtDirect.Select("surg_id not in (" + string.Join(",", frmFiltersGlobal.dicSurgicalFiltersGlobal.Keys) + ") and surg_id <> -9999");
                else
                    dataRowArrTmp = dtCurrentFilters_GLOBAL.Select("surg_id not in (" + string.Join(",", frmFiltersGlobal.dicSurgicalFiltersGlobal.Keys) + ")");
                //dataRowArrTmp = dtLiveFilters_GLOBAL.Select("surg_id not in (" + string.Join(",", frmFiltersGlobal.dicSurgicalFiltersGlobal.Keys) + ")");

                DataTable dtTmp;
                if (dataRowArrTmp.Count() > 0)
                    dtTmp = dataRowArrTmp.CopyToDataTable();
                else
                    dtTmp = dtCurrentFilters_GLOBAL.Clone();

                viewTmp = new DataView(dtTmp);
                dtSurgicalFilters = viewTmp.ToTable(true, "surg_desc", "surg_id");
                dataRow = dtSurgicalFilters.NewRow();
                dataRow["surg_id"] = -9999;
                dataRow["surg_desc"] = "(FILTERED)";
                dtSurgicalFilters.Rows.InsertAt(dataRow, 0);
                dtSurgicalFilters.DefaultView.Sort = "surg_desc ASC";
                dtSurgicalFilters = dtSurgicalFilters.DefaultView.ToTable();

            }
            else
            {
                if (dtLiveFilters_GLOBAL == null)
                    updateFilters();

                viewTmp = new DataView(dtLiveFilters_GLOBAL);
                dtSurgicalFilters = viewTmp.ToTable(true, "surg_desc", "surg_id");
                dataRow = dtSurgicalFilters.NewRow();
                dataRow["surg_id"] = -9999;
                dataRow["surg_desc"] = "~All Surgeries~";
                dtSurgicalFilters.Rows.InsertAt(dataRow, 0);
                dtSurgicalFilters.DefaultView.Sort = "surg_desc ASC";
            }

            if (dtSurgicalFilters != null)
            {
                bSource.DataSource = dtSurgicalFilters;
                cmbSurgical.ValueMember = "surg_id";
                cmbSurgical.DisplayMember = "surg_desc";
                cmbSurgical.DataSource = bSource;
            }

        }

        private void refreshSpecialtyFilters()
        {
            BindingSource bSource = new BindingSource();
            DataView viewTmp;
            DataTable dtSpecialtyFilters = null;
            DataRow dataRow;
            DataRow[] dataRowArrTmp;

            //CURRENT DROPDOWN CANT LIMIT ITSELF!!! 
            //REMOVE DIRECT FROM DROPDOWN
            DataTable dtDirect = null;
            if (strCurrentFilterGLOBAL == "Specialty")
            {
                if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
                {
                    bSource = (BindingSource)cmbSpecialty.DataSource; // Se convierte el DataSource 
                    dtDirect = (DataTable)bSource.DataSource;
                }
                strCurrentFilterGLOBAL = null;
            }

            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
            {
                if (dtDirect != null)
                    dataRowArrTmp = dtDirect.Select("Spec_id not in (" + string.Join(",", frmFiltersGlobal.dicSpecialtyFiltersGlobal.Keys) + ") and Spec_id <> -9999");
                else
                    dataRowArrTmp = dtCurrentFilters_GLOBAL.Select("Spec_id not in (" + string.Join(",", frmFiltersGlobal.dicSpecialtyFiltersGlobal.Keys) + ")");


                DataTable dtTmp;
                if (dataRowArrTmp.Count() > 0)
                    dtTmp = dataRowArrTmp.CopyToDataTable();
                else
                    dtTmp = dtCurrentFilters_GLOBAL.Clone();
                viewTmp = new DataView(dtTmp);
                dtSpecialtyFilters = viewTmp.ToTable(true, "Spec_Desc", "Spec_id");
                dataRow = dtSpecialtyFilters.NewRow();
                dataRow["Spec_id"] = -9999;
                dataRow["Spec_Desc"] = "(FILTERED)";
                dtSpecialtyFilters.Rows.InsertAt(dataRow, 0);
                dtSpecialtyFilters.DefaultView.Sort = "Spec_Desc ASC";
                dtSpecialtyFilters = dtSpecialtyFilters.DefaultView.ToTable();

            }
            else
            {
                if (dtLiveFilters_GLOBAL == null)
                    updateFilters();

                viewTmp = new DataView(dtLiveFilters_GLOBAL);
                dtSpecialtyFilters = viewTmp.ToTable(true, "Spec_Desc", "Spec_id");
                dataRow = dtSpecialtyFilters.NewRow();
                dataRow["Spec_id"] = -9999;
                dataRow["Spec_Desc"] = "~All Specialties~";
                dtSpecialtyFilters.Rows.InsertAt(dataRow, 0);
                dtSpecialtyFilters.DefaultView.Sort = "Spec_Desc ASC";
            }

            if(dtSpecialtyFilters != null)
            {
                bSource.DataSource = dtSpecialtyFilters;
                cmbSpecialty.ValueMember = "Spec_id";
                cmbSpecialty.DisplayMember = "Spec_Desc";
                cmbSpecialty.DataSource = bSource;
            }


        }

        private void refreshMeasureFilters()
        {
            if (dtMeasureFiltersGLOBAL == null)
            {
                string strSQL = "select distinct Measure_ID,Measure_desc from dbo.PBP_dim_Measures WHERE Measure_ID BETWEEN 324 and 327 order by Measure_desc";
                dtMeasureFiltersGLOBAL = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL);
                DataRow dataRow = dtMeasureFiltersGLOBAL.NewRow();
                dataRow["Measure_ID"] = -9999;
                dataRow["Measure_desc"] = "~All Measures~";
                dtMeasureFiltersGLOBAL.Rows.InsertAt(dataRow, 0);

                dataRow = dtMeasureFiltersGLOBAL.NewRow();
                dataRow["Measure_ID"] = 9999;
                dataRow["Measure_desc"] = "Composite Measures";
                dtMeasureFiltersGLOBAL.Rows.InsertAt(dataRow, dtMeasureFiltersGLOBAL.Rows.Count);
            }


            BindingSource bSource = new BindingSource();
            if (frmFiltersGlobal.dicMeasureFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
            {
                DataTable dtTmp = dtMeasureFiltersGLOBAL.Select("Measure_ID not in (" + string.Join(",", frmFiltersGlobal.dicMeasureFiltersGlobal.Keys) + ")").CopyToDataTable();
                DataRow dr = dtTmp.Select("Measure_ID='-9999'").FirstOrDefault();
                if (dr != null)
                    dr["Measure_desc"] = "(FILTERED)";

                dtTmp.DefaultView.Sort = "Measure_desc ASC";
                dtTmp = dtTmp.DefaultView.ToTable();
                bSource.DataSource = dtTmp;
            }
            else
                bSource.DataSource = dtMeasureFiltersGLOBAL;


            cmbMeasure.ValueMember = "Measure_ID";
            cmbMeasure.DisplayMember = "Measure_desc";
            cmbMeasure.DataSource = bSource;
        }

        private void refreshMarketFilters()
        {
            BindingSource bSource = new BindingSource();
            DataView viewTmp;
            DataTable dtMarketFilters = null;
            DataRow dataRow;
            DataRow[] dataRowArrTmp;


            //CURRENT DROPDOWN CANT LIMIT ITSELF!!! 
            //REMOVE DIRECT FROM DROPDOWN
            DataTable dtDirect = null;
            if (strCurrentFilterGLOBAL == "Market")
            {
                if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
                {
                    bSource = (BindingSource)cmbMarket.DataSource; // Se convierte el DataSource 
                    dtDirect = (DataTable)bSource.DataSource;
                }
                strCurrentFilterGLOBAL = null;
            }




            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
            {

                if (dtDirect != null)
                    dataRowArrTmp = dtDirect.Select("Market_Nbr not in (" + string.Join(",", frmFiltersGlobal.dicMarketFiltersGlobal.Keys) + ") and Market_Nbr <> -9999");
                else
                    dataRowArrTmp = dtCurrentFilters_GLOBAL.Select("Market_Nbr not in (" + string.Join(",", frmFiltersGlobal.dicMarketFiltersGlobal.Keys) + ")");

                DataTable dtTmp;
                if (dataRowArrTmp.Count() > 0)
                    dtTmp = dataRowArrTmp.CopyToDataTable();
                else
                    dtTmp = dtCurrentFilters_GLOBAL.Clone();

                viewTmp = new DataView(dtTmp);
                dtMarketFilters = viewTmp.ToTable(true, "Market_Name", "Market_Nbr");
                dataRow = dtMarketFilters.NewRow();
                dataRow["Market_Nbr"] = -9999;
                dataRow["Market_Name"] = "(FILTERED)";
                dtMarketFilters.Rows.InsertAt(dataRow, 0);
                dtMarketFilters.DefaultView.Sort = "Market_Nbr ASC";
                dtMarketFilters = dtMarketFilters.DefaultView.ToTable();

            }
            else
            {
                if (dtLiveFilters_GLOBAL == null)
                    updateFilters();

                viewTmp = new DataView(dtLiveFilters_GLOBAL);
                dtMarketFilters = viewTmp.ToTable(true, "Market_Name", "Market_Nbr");
                dataRow = dtMarketFilters.NewRow();
                dataRow["Market_Nbr"] = -9999;
                dataRow["Market_Name"] = "~All Markets~";
                dtMarketFilters.Rows.InsertAt(dataRow, 0);
                dtMarketFilters.DefaultView.Sort = "Market_Nbr ASC";
            }

            if (dtMarketFilters != null)
            {
                bSource.DataSource = dtMarketFilters;
                cmbMarket.ValueMember = "Market_Nbr";
                cmbMarket.DisplayMember = "Market_Name";
                cmbMarket.DataSource = bSource;
            }

            
        }
        #endregion

        #region EXCEL REPORT GENERATION

        System.Diagnostics.Stopwatch timerGLOBAL = null;

        int iCntStatusGLOBAL = 0;
        string strSQLGLOBAL = null;
        bool blFirstRowGLOBAL = false;
        bool blCancelReportGLOBAL = false;
        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            try
            {
                txtStatus.Clear();
                iCntStatusGLOBAL = 1;

                Button btn = sender as Button;
                if (btn.Text == "Generate Report")
                {
                    blFirstRowGLOBAL = true;
                    timerGLOBAL = System.Diagnostics.Stopwatch.StartNew();
                    txtStatus.AppendLog("**************************************Status********************************************" + Environment.NewLine, statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("Requesting data from the server..." + Environment.NewLine, statusDefaultColorGlobal, normalFontGlobal);


                    //alStatusGlobal.Clear();
                    //alStatusGlobal.Add(new Status("Status", "Requesting data from the server..."));
                    //sbStatusGlobal.Append("Requesting data from the server..." + Environment.NewLine);
                    //alStatusGlobal.Add(new Status("Ready...", Color.Black, txtStatus.Font));
                    //populateStatus();




                    blCancelReportGLOBAL = false;
                    btn.Text = "Cancel";
                }
                else
                {
                    txtStatus.AppendLog("Sending cancellation to the server..." + Environment.NewLine, statusDefaultColorGlobal, normalFontGlobal);

                    //alStatusGlobal.Clear();
                    //alStatusGlobal.Add(new Status("Status", "Sending cancellation to the server..."));
                    //sbStatusGlobal.Append("Sending cancellation to the server..." + Environment.NewLine);
                    //populateStatus();


                    blCancelReportGLOBAL = true;
                    return;
                }

                //GLOBAL SO NO DELEGATING LATER
                strConfidenceGLOBAL = cmbConfidence.Text;
                strSampleSizeGLOBAL = cmbSampleSize.Text;
                strLOBGLOBAL = cmbLOB.Text;
                strQuarterGLOBAL = cmbQuarter.Text;

                strSQLGLOBAL = getFinalSQL();
                this.UseWaitCursor = true;

                SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder(GlobalObjects.strILUCAConnectionString)
                {
                    ConnectTimeout = 4000,
                    AsynchronousProcessing = true
                };
                SqlConnection conn = new SqlConnection(connectionBuilder.ConnectionString);
                SqlCommand cmd = new SqlCommand(strSQLGLOBAL, conn);
                //try
                //{
                conn.Open();
                //The actual T-SQL execution happens in a separate work thread.  
                cmd.BeginExecuteReader(new AsyncCallback(ReportCallbackFunction), cmd);

            }
            catch (Exception ex)
            {
                cleanupError(ex.Message);
            }
        }
        
        private delegate void DispatchHandler();
        private void ReportCallbackFunction(IAsyncResult asyncResult)
        {

            string strFinalPath = null;
            string strStatusTmp = null;
            //try
            //{
            //un-box the AsynState back to the SqlCommand
            SqlCommand cmd = (SqlCommand)asyncResult.AsyncState;
            SqlDataReader reader = cmd.EndExecuteReader(asyncResult);
            int intCurrentRow = 1;
            int i2 = 0; //SECOND SHEET COLUMN COUNTER
            XLColor xlColorCurrent = null;
            string[] strRGBArr = null;
            string[] strColumnNameArr = null;
            string strVerbiage = null;
            string strColumnName = null;
            string strCurrentMeasure = null;
            string strPreviousMeasure = null;

            DataRow drMeasureValues = null;
            
            XLWorkbook workbook = null;
            IXLWorksheet worksheet = null;

            IXLWorksheet worksheet2 = null;

            bool blSecondMeasureSheet = false;

            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Average Allowed Per Episode (Additional Sheet)") != -1)
                blSecondMeasureSheet = true;

            string currentSheet = "Output";

            while (reader.Read())
            {

                //CHECK FOR CANCEL BEFORE ANYTHING ELSE
                if (blCancelReportGLOBAL)
                {
                    blCancelReportGLOBAL = false;
                    cmd.Cancel();
                    cmd.Connection.Close();
                    finishReports(-9999, null);
                    return;
                }

                if (blFirstRowGLOBAL)
                {
                    DispatchHandler handler5 = delegate ()
                    {
                        //EXCEL NOTIFICATION FOR TIMING FOR USERS SAKE
                        //txtStatus.AppendText("Loading Excel libraries..." + Environment.NewLine);
                        txtStatus.AppendLog("Loading Excel libraries..." + Environment.NewLine, statusDefaultColorGlobal, normalFontGlobal);
                        //alStatusGlobal.Clear();
                        //alStatusGlobal.Add(new Status("Status", "Loading Excel libraries..."));
                        //populateStatus();
                        //sbStatusGlobal.Append("Loading Excel libraries..." + Environment.NewLine);
                        //populateHandlerStatus();
                    };
                    this.BeginInvoke(handler5);

                    //START EXCEL TEMPLATE
                    //workbook = new XLWorkbook(GlobalObjects.strComplianceReportingTemplate_Path);
                    workbook = new XLWorkbook();
                    worksheet = workbook.Worksheets.Add("Output");

                    //2021
                    if (blSecondMeasureSheet)
                        worksheet2 = workbook.Worksheets.Add("Output2");


                    //ADD COLUMN NAMES TO EXCEL
                    for (int i = 0; i < reader.FieldCount; i++)
                    {

                        strColumnNameArr = reader.GetName(i).ToString().Split('~');
                        strColumnName = strColumnNameArr[0];
                        if (strColumnNameArr.Length > 1)
                        {
                            strCurrentMeasure = strColumnNameArr[1];

                            if (strPreviousMeasure != strCurrentMeasure)
                            {
                                //HEAVY MEASURE ROWSPAN HERE!!!!?
                                drMeasureValues = dtMeasureSQLGLOBAL.Select("MeasureId = " + strCurrentMeasure).FirstOrDefault();
                                strVerbiage = drMeasureValues["Verbiage"].ToString();
                                strRGBArr = drMeasureValues["ColorArray"].ToString().Split(',');
                                currentSheet = drMeasureValues["ExcelSheet"].ToString();
                                xlColorCurrent = XLColor.FromArgb(int.Parse(strRGBArr[0]), int.Parse(strRGBArr[1]), int.Parse(strRGBArr[2]));
                                strPreviousMeasure = strCurrentMeasure;
                            }
                        }
                        else
                        {
                            strCurrentMeasure = null;
                            strPreviousMeasure = null;
                            xlColorCurrent = XLColor.NoColor;
                        }


                        string strCurrentRange2 = null;
                        string strCurrentRange = null;
                        //NOT A MEASURE COLUMN, CHANGE ROWSPAN
                        if (strCurrentMeasure == null)
                        {
                            //worksheet.Range("B2:D3").Row(1).Merge();
                            strCurrentRange = worksheet.Range(worksheet.Cell(1, i + 1), worksheet.Cell(2, i + 1)).RangeAddress.ToString();
                            worksheet.Range(strCurrentRange).Merge();

                            if (blSecondMeasureSheet)
                            {
                                strCurrentRange2 = worksheet2.Range(worksheet2.Cell(1, i2 + 1), worksheet2.Cell(2, i2 + 1)).RangeAddress.ToString();
                                worksheet2.Range(strCurrentRange2).Merge();
                            }

                        }
                        else
                        {
                            if (currentSheet == "Output")
                            {
                                if (strColumnName == "Adv Events")
                                {
                                    strCurrentRange = worksheet.Range(worksheet.Cell(1, i + 1), worksheet.Cell(1, i + intMeasureColCntGLOBAL)).RangeAddress.ToString();
                                    worksheet.Range(strCurrentRange).Merge();
                                    worksheet.Range(strCurrentRange).Value = strVerbiage;
                                    worksheet.Range(strCurrentRange).Style.Alignment.WrapText = true;
                                    worksheet.Range(strCurrentRange).Style.Font.Bold = true;
                                    worksheet.Range(strCurrentRange).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    worksheet.Range(strCurrentRange).Style.Fill.BackgroundColor = xlColorCurrent;

                                    worksheet.Range(strCurrentRange).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    worksheet.Range(strCurrentRange).Style.Border.BottomBorderColor = XLColor.Black;
                                    worksheet.Range(strCurrentRange).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    worksheet.Range(strCurrentRange).Style.Border.LeftBorderColor = XLColor.Black;
                                    worksheet.Range(strCurrentRange).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    worksheet.Range(strCurrentRange).Style.Border.TopBorderColor = XLColor.Black;
                                    worksheet.Range(strCurrentRange).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    worksheet.Range(strCurrentRange).Style.Border.RightBorderColor = XLColor.Black;
                                }

                                strCurrentRange = worksheet.Range(worksheet.Cell(2, i + 1), worksheet.Cell(2, i + 1)).RangeAddress.ToString();
                                worksheet.Columns(worksheet.Cell(2, i + 1).WorksheetColumn().ColumnLetter()).Width = 7.5;
                            }
                            else if (currentSheet == "Output2")
                            {
                                if (strColumnName == "Total")
                                {
                                    strCurrentRange2 = worksheet2.Range(worksheet2.Cell(1, i2 + 1), worksheet2.Cell(1, i2 + 7)).RangeAddress.ToString();
                                    worksheet2.Range(strCurrentRange2).Merge();
                                    worksheet2.Range(strCurrentRange2).Value = strVerbiage;
                                    worksheet2.Range(strCurrentRange2).Style.Alignment.WrapText = true;
                                    worksheet2.Range(strCurrentRange2).Style.Font.Bold = true;
                                    worksheet2.Range(strCurrentRange2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                    worksheet2.Range(strCurrentRange2).Style.Fill.BackgroundColor = xlColorCurrent;

                                    worksheet2.Range(strCurrentRange2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                    worksheet2.Range(strCurrentRange2).Style.Border.BottomBorderColor = XLColor.Black;
                                    worksheet2.Range(strCurrentRange2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                    worksheet2.Range(strCurrentRange2).Style.Border.LeftBorderColor = XLColor.Black;
                                    worksheet2.Range(strCurrentRange2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                    worksheet2.Range(strCurrentRange2).Style.Border.TopBorderColor = XLColor.Black;
                                    worksheet2.Range(strCurrentRange2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                    worksheet2.Range(strCurrentRange2).Style.Border.RightBorderColor = XLColor.Black;
                                }

                                strCurrentRange2 = worksheet2.Range(worksheet2.Cell(2, i2 + 1), worksheet2.Cell(2, i2 + 1)).RangeAddress.ToString();
                                worksheet2.Columns(worksheet2.Cell(2, i2 + 1).WorksheetColumn().ColumnLetter()).Width = 7.5;

                            }
                        }


                        if (currentSheet == "Output")
                        {
                            //ADD COLUMN NAMES TO ROW 1 OF SHEET
                            worksheet.Range(strCurrentRange).Value = strColumnName;

                            if (strCurrentMeasure != null)
                                worksheet.Range(strCurrentRange).Style.Alignment.WrapText = true;

                            worksheet.Range(strCurrentRange).Style.Font.Bold = true;
                            worksheet.Range(strCurrentRange).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            worksheet.Range(strCurrentRange).Style.Fill.BackgroundColor = xlColorCurrent;

                            worksheet.Range(strCurrentRange).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(strCurrentRange).Style.Border.BottomBorderColor = XLColor.Black;
                            worksheet.Range(strCurrentRange).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(strCurrentRange).Style.Border.LeftBorderColor = XLColor.Black;
                            worksheet.Range(strCurrentRange).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(strCurrentRange).Style.Border.TopBorderColor = XLColor.Black;
                            worksheet.Range(strCurrentRange).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                            worksheet.Range(strCurrentRange).Style.Border.RightBorderColor = XLColor.Black;

                        }

                        //if (currentSheet == "Output2")
                        //if (strCurrentMeasure == "8888")
                        if (blSecondMeasureSheet)
                        {

                            if (strCurrentRange2 == null)
                                strCurrentRange2 = worksheet2.Range(worksheet2.Cell(1, i2 + 1), worksheet2.Cell(2, i2 + 1)).RangeAddress.ToString();


                            //ADD COLUMN NAMES TO ROW 1 OF SHEET
                            //if (strCurrentMeasure == null || (strColumnName == "Total" || strColumnName == "Mgt" || strColumnName == "Surg" || strColumnName == "Facl" || strColumnName == "Inp" || strColumnName == "Otp" || strColumnName == "Pharm"))
                            if (strCurrentMeasure == null || strCurrentMeasure == "8888")
                                worksheet2.Range(strCurrentRange2).Value = strColumnName;

                            //if (strCurrentMeasure != null)
                            if (strCurrentMeasure == "8888")
                                worksheet2.Range(strCurrentRange2).Style.Alignment.WrapText = true;

                            //if (strCurrentMeasure == null || (strColumnName == "Total" || strColumnName == "Mgt" || strColumnName == "Surg" || strColumnName == "Facl" || strColumnName == "Inp" || strColumnName == "Otp" || strColumnName == "Pharm"))
                            if (strCurrentMeasure == null || strCurrentMeasure == "8888")
                            {
                                worksheet2.Range(strCurrentRange2).Style.Font.Bold = true;
                                worksheet2.Range(strCurrentRange2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                                worksheet2.Range(strCurrentRange2).Style.Fill.BackgroundColor = xlColorCurrent;

                                worksheet2.Range(strCurrentRange2).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                                worksheet2.Range(strCurrentRange2).Style.Border.BottomBorderColor = XLColor.Black;
                                worksheet2.Range(strCurrentRange2).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                                worksheet2.Range(strCurrentRange2).Style.Border.LeftBorderColor = XLColor.Black;
                                worksheet2.Range(strCurrentRange2).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                                worksheet2.Range(strCurrentRange2).Style.Border.TopBorderColor = XLColor.Black;
                                worksheet2.Range(strCurrentRange2).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                                worksheet2.Range(strCurrentRange2).Style.Border.RightBorderColor = XLColor.Black;

                                i2++;
                            }


                        }

                    }



                    //strStatusTmp = "Duration: {$time},  Rows Loaded: {$rows}";

                    DispatchHandler handler6 = delegate ()
                    {
                        //txtStatus.AppendText("Duration: {$time},  Rows Loaded: {$rows}" + Environment.NewLine);
                        txtStatus.AppendLog("Duration: {$time},  Rows Loaded: {$rows}" + Environment.NewLine, statusDefaultColorGlobal, normalFontGlobal);
                        strStatusTmp = txtStatus.Text;
                    };
                    this.BeginInvoke(handler6);

                    //sbStatusGlobal.Append("{$time_rows}" + Environment.NewLine);


                    worksheet.Row(1).Height = 30;
                    worksheet.Row(2).Height = 43.2;

                    if (blSecondMeasureSheet)
                    {
                        worksheet2.Row(1).Height = 30;
                        worksheet2.Row(2).Height = 43.2;
                    }

                    blFirstRowGLOBAL = false;
                    //i2++;
                }//if blFirstRowGLOBAL 

                i2 = 0;
                for (int i = 0; i < reader.FieldCount; i++)
                {

                    strColumnNameArr = reader.GetName(i).ToString().Split('~');
                    strColumnName = strColumnNameArr[0];
                    if (strColumnNameArr.Length > 1)
                    {
                        strCurrentMeasure = strColumnNameArr[1];

                        if (strPreviousMeasure != strCurrentMeasure)
                        {
                            //HEAVY MEASURE ROWSPAN HERE!!!!?
                            drMeasureValues = dtMeasureSQLGLOBAL.Select("MeasureId = " + strCurrentMeasure).FirstOrDefault();
                            strRGBArr = drMeasureValues["ColorArray"].ToString().Split(',');
                            xlColorCurrent = XLColor.FromArgb(int.Parse(strRGBArr[0]), int.Parse(strRGBArr[1]), int.Parse(strRGBArr[2]));
                            currentSheet = drMeasureValues["ExcelSheet"].ToString();
                            strPreviousMeasure = strCurrentMeasure;
                        }
                    }
                    else
                    {
                        strCurrentMeasure = null;
                        strPreviousMeasure = null;
                        xlColorCurrent = XLColor.NoColor;
                    }


                    var rowCnt = intCurrentRow + 2;
                    if (strColumnName != "Total" && strColumnName != "Mgt" && strColumnName != "Surg" && strColumnName != "Facl" && strColumnName != "Inp" && strColumnName != "Otp" && strColumnName != "Pharm")
                    {

                        //ADD COLUMN NAMES TO ROW 1 OF SHEET
                        worksheet.Cell(rowCnt, i + 1).Value = reader.GetValue(i).ToString();

                        if (strColumnName == "# of Surg" || strColumnName == "Adv Events" || strColumnName == "MPIN Count")
                        {
                            worksheet.Cell(rowCnt, i + 1).Style.NumberFormat.Format = "#,##0";
                            worksheet.Cell(rowCnt, i + 1).DataType = XLCellValues.Number;
                        }
                        else if (strColumnName == "Facility Rate" || strColumnName == "Practice Rate" || strColumnName == "Exp Rate" || strColumnName == "Adj Rate")
                        {
                            worksheet.Cell(rowCnt, i + 1).Style.NumberFormat.Format = "0.0%";
                            worksheet.Cell(rowCnt, i + 1).DataType = XLCellValues.Number;
                        }
                        else if (strColumnName == "Exp Adv Events" || strColumnName == "Chi Sq" || strColumnName == "OE")
                        {
                            worksheet.Cell(rowCnt, i + 1).Style.NumberFormat.Format = "#,##0.00";
                            worksheet.Cell(rowCnt, i + 1).DataType = XLCellValues.Number;
                        }
                        else if (strColumnName == "Avg Cost" || strColumnName == "Avg Total Allowed")
                        {
                            //worksheet.Cell(rowCnt, i + 1).Style.NumberFormat.Format = "$#,##0.00";
                            worksheet.Cell(rowCnt, i + 1).Style.NumberFormat.Format = "$#,##0";
                            worksheet.Cell(rowCnt, i + 1).DataType = XLCellValues.Number;
                            worksheet.Column(i + 1).Width = 12;
                        }
                        else
                        {
                            worksheet.Cell(rowCnt, i + 1).DataType = XLCellValues.Text;
                        }

                        if (strColumnName == "Stat Sign")
                        {
                            worksheet.Cell(rowCnt, i + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }

                        worksheet.Cell(rowCnt, i + 1).Style.Fill.BackgroundColor = xlColorCurrent;

                        worksheet.Cell(rowCnt, i + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.BottomBorderColor = XLColor.Black;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.LeftBorderColor = XLColor.Black;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.TopBorderColor = XLColor.Black;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet.Cell(rowCnt, i + 1).Style.Border.RightBorderColor = XLColor.Black;
                    }



                    if (currentSheet == "Output2")
                    {
                        worksheet2.Cell(rowCnt, i2 + 1).Value = reader.GetValue(i).ToString();

                        if (strColumnName == "# of Surg" || strColumnName == "Adv Events" || strColumnName == "MPIN Count")
                        {
                            worksheet2.Cell(rowCnt, i2 + 1).Style.NumberFormat.Format = "#,##0";
                            worksheet2.Cell(rowCnt, i2 + 1).DataType = XLCellValues.Number;
                        }
                        else if (strColumnName == "Facility Rate" || strColumnName == "Practice Rate" || strColumnName == "Exp Rate" || strColumnName == "Adj Rate")
                        {
                            worksheet2.Cell(rowCnt, i2 + 1).Style.NumberFormat.Format = "0.0%";
                            worksheet2.Cell(rowCnt, i2 + 1).DataType = XLCellValues.Number;
                        }
                        else if (strColumnName == "Exp Adv Events" || strColumnName == "Chi Sq" || strColumnName == "OE")
                        {
                            worksheet2.Cell(rowCnt, i2 + 1).Style.NumberFormat.Format = "#,##0.00";
                            worksheet2.Cell(rowCnt, i2 + 1).DataType = XLCellValues.Number;
                        }
                        else if (strColumnName == "Avg Cost" || strColumnName == "Avg Total Allowed")
                        {
                            //worksheet2.Cell(rowCnt, i2 + 1).Style.NumberFormat.Format = "$#,##0.00";
                            worksheet2.Cell(rowCnt, i2 + 1).Style.NumberFormat.Format = "$#,##0";
                            worksheet2.Cell(rowCnt, i2 + 1).DataType = XLCellValues.Number;
                            worksheet2.Column(i2 + 1).Width = 12;
                        }
                        else if (strColumnName == "Total" || strColumnName == "Mgt" || strColumnName == "Surg" || strColumnName == "Facl" || strColumnName == "Inp" || strColumnName == "Otp" || strColumnName == "Pharm")
                        {
                            worksheet2.Cell(rowCnt, i2 + 1).Style.NumberFormat.Format = "$#,##0";
                            worksheet2.Cell(rowCnt, i2 + 1).DataType = XLCellValues.Number;
                            worksheet2.Column(i2 + 1).Width = 12;
                        }
                        else
                        {
                            worksheet2.Cell(rowCnt, i2 + 1).DataType = XLCellValues.Text;
                        }

                        if (strColumnName == "Stat Sign")
                        {
                            worksheet2.Cell(rowCnt, i2 + 1).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                        }

                        worksheet2.Cell(rowCnt, i2 + 1).Style.Fill.BackgroundColor = xlColorCurrent;

                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.BottomBorderColor = XLColor.Black;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.LeftBorderColor = XLColor.Black;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.TopBorderColor = XLColor.Black;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        worksheet2.Cell(rowCnt, i2 + 1).Style.Border.RightBorderColor = XLColor.Black;

                        i2++;

                    }

                }

                var elapsed = timerGLOBAL.Elapsed;
                if (!reader.IsClosed)
                {
                    DispatchHandler handler7 = delegate ()
                    {
                        txtStatus.Text = strStatusTmp.Replace("{$time}", elapsed.ToString("mm':'ss':'fff")).Replace("{$rows}", (iCntStatusGLOBAL/2).ToString());
                        iCntStatusGLOBAL++;
                    };
                    this.BeginInvoke(handler7);

                    iCntStatusGLOBAL++;

                    //if (strStatusTmp != null)
                    //    alStatusGlobal.RemoveAt(alStatusGlobal.Count -1);

                    //strStatusTmpGlobal = "Duration: "+ elapsed.ToString("mm':'ss':'fff") + ",  Rows Loaded: "+ iCntStatusGLOBAL.ToString() ;

                    //alStatusGlobal.Add(new Status("Status", strStatusTmp));
                    //populateStatus();
                    //populateHandlerStatus(sbStatusGlobal.ToString().Replace("{$time_rows}", strStatusTmpGlobal));
                    //sbStatusGlobal.Append("{$time_rows}" + Environment.NewLine);


                }

                intCurrentRow++;

            }

            //LOOP ALL NON MEASURE COLUMNS
            if (worksheet != null)
            {
                for (int i = 1; i <= intTotalExcelColumnsNMGLOBAL; i++)
                {
                    ////RESIZE COLUMNS
                    ////CLOSEDXML BUG RESIZE INT COL WITH HEADER DOESNT AUTOSIZE SO MANUAL
                    if (worksheet.Cell(1, i).Value.ToString().Equals("# of Surg"))
                    {
                        worksheet.Column(i).Width = 8.33;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("Facility Type"))
                    {
                        worksheet.Column(i).Width = 12;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("Specialty"))
                    {
                        worksheet.Column(i).Width = 11;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("PD status"))
                    {
                        worksheet.Column(i).Width = 13;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("Avg Total Allowed"))
                    {
                        worksheet.Column(i).Width = 13;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("Market Number"))
                    {
                        worksheet.Column(i).Width = 15;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("Market Name"))
                    {
                        worksheet.Column(i).Width = 25;
                    }
                    else if (worksheet.Cell(1, i).Value.ToString().Equals("MPIN Count"))
                    {
                        worksheet.Column(i).Width = 12;
                    }
                    else //AUTO
                        worksheet.Column(i).AdjustToContents();
                }
            }
            else
                iCntStatusGLOBAL = 0;


            if (worksheet2 != null)
            {
                for (int i = 1; i <= intTotalExcelColumnsNMGLOBAL; i++)
                {
                    ////RESIZE COLUMNS
                    ////CLOSEDXML BUG RESIZE INT COL WITH HEADER DOESNT AUTOSIZE SO MANUAL
                    if (worksheet2.Cell(1, i).Value.ToString().Equals("# of Surg"))
                    {
                        worksheet2.Column(i).Width = 8.33;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("Facility Type"))
                    {
                        worksheet2.Column(i).Width = 12;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("Specialty"))
                    {
                        worksheet2.Column(i).Width = 11;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("PD status"))
                    {
                        worksheet2.Column(i).Width = 13;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("Avg Total Allowed"))
                    {
                        worksheet2.Column(i).Width = 15;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("Market Number"))
                    {
                        worksheet2.Column(i).Width = 15;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("Market Name"))
                    {
                        worksheet2.Column(i).Width = 25;
                    }
                    else if (worksheet2.Cell(1, i).Value.ToString().Equals("MPIN Count"))
                    {
                        worksheet2.Column(i).Width = 12;
                    }
                    else //AUTO
                        worksheet2.Column(i).AdjustToContents();
                }
            }

            if (cmd.Connection.State.Equals(ConnectionState.Open))
            {
                if (workbook != null)
                {
                    addParametersToWorksheet(ref workbook);
                    //worksheet.Columns().AdjustToContents();
                    worksheet = workbook.Worksheets.Add("SQL");
                    worksheet.Cell(1, 1).Value = strSQLGLOBAL;
                    worksheet.Columns().AdjustToContents();

                    var path = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\compliance_reports\\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    strFinalPath = path + "\\compliance_rpt_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xlsx";
                    workbook.SaveAs(strFinalPath, true);
                }
                cmd.Connection.Close();
                finishReports(iCntStatusGLOBAL, strFinalPath);
            }


        }

        string strConfidenceGLOBAL = null, strSampleSizeGLOBAL = null, strLOBGLOBAL = null, strQuarterGLOBAL = null;
        private void addParametersToWorksheet(ref XLWorkbook workbook)
        {

            int intColCnt = 1;
            int intRowCnt = 0;
            IXLWorksheet worksheet = workbook.Worksheets.Add("Parameters");
            XLColor color = XLColor.LightSteelBlue;
            populateWorksheetCell(ref worksheet, 1, 1, "Confidence Interval", color, true, true, true);
            populateWorksheetCell(ref worksheet, 2, 1, strConfidenceGLOBAL, XLColor.NoColor, false, false, false);
            intColCnt++;
            populateWorksheetCell(ref worksheet, 1, intColCnt, "Min Sample Size", color, true, true, true);
            populateWorksheetCell(ref worksheet, 2, intColCnt, strSampleSizeGLOBAL, XLColor.NoColor, false, false, false);

            intColCnt++;
            populateWorksheetCell(ref worksheet, 1, intColCnt, "Quarter", color, true, true, true);
            populateWorksheetCell(ref worksheet, 2, intColCnt, strQuarterGLOBAL, XLColor.NoColor, false, false, false);

            //if (strLOBGLOBAL != "~All LOBs~")
            //{
                intColCnt++;
                populateWorksheetCell(ref worksheet, 1, intColCnt, "LOB", color, true, true, true);
                populateWorksheetCell(ref worksheet, 2, intColCnt, strLOBGLOBAL, XLColor.NoColor, false, false, false);
            //}


            //if (frmFiltersGlobal.dicQuarterFiltersGlobal.Count() > 0)
            //{
            //    intColCnt++;

            //    populateWorksheetCell(ref worksheet, 1, intColCnt, "Quarter", color, true, true, true);
            //    intRowCnt = 2;
            //    foreach (var item in frmFiltersGlobal.dicQuarterFiltersGlobal)
            //    {
            //        populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value, XLColor.NoColor, false, false, false);
            //        intRowCnt++;
            //    }

            //}

            if (frmFiltersGlobal.dicProviderFiltersGlobal.Count() > 0)
            {
                intColCnt++;

                populateWorksheetCell(ref worksheet, 1, intColCnt, "Providers", color, true, true, true);
                intRowCnt = 2;
                foreach (var item in frmFiltersGlobal.dicProviderFiltersGlobal)
                {
                    populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value + " - " + item.Key, XLColor.NoColor, false, false, false);
                    intRowCnt++;
                }
                //}
            }


            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() > 0)
            {
                intColCnt++;
                populateWorksheetCell(ref worksheet, 1, intColCnt, "Surgeries", color, true, true, true);
                intRowCnt = 2;
                foreach (var item in frmFiltersGlobal.dicSurgicalFiltersGlobal)
                {
                    populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value, XLColor.NoColor, false, false, false);
                    intRowCnt++;
                }
                //}
            }

            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() > 0)
            {
                intColCnt++;
                populateWorksheetCell(ref worksheet, 1, intColCnt, "Specialties", color, true, true, true);
                intRowCnt = 2;
                foreach (var item in frmFiltersGlobal.dicSpecialtyFiltersGlobal)
                {
                    populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value, XLColor.NoColor, false, false, false);
                    intRowCnt++;
                }
                // }
            }


            if (frmFiltersGlobal.dicMeasureFiltersGlobal.Count() > 0)
            {
                intColCnt++;
                populateWorksheetCell(ref worksheet, 1, intColCnt, "Measures", color, true, true, true);
                intRowCnt = 2;
                foreach (var item in frmFiltersGlobal.dicMeasureFiltersGlobal)
                {
                    populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value, XLColor.NoColor, false, false, false);
                    intRowCnt++;
                }
                //}
            }


            if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count() > 0 && radFacility.Checked)
            {
                intColCnt++;
                populateWorksheetCell(ref worksheet, 1, intColCnt, "Facility Type", color, true, true, true);
                intRowCnt = 2;
                foreach (var item in frmFiltersGlobal.dicFacilityTypeFiltersGlobal)
                {
                    populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value, XLColor.NoColor, false, false, false);
                    intRowCnt++;
                }
                //}
            }


            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() > 0)
            {
                intColCnt++;
                populateWorksheetCell(ref worksheet, 1, intColCnt, "Markets", color, true, true, true);
                intRowCnt = 2;
                foreach (var item in frmFiltersGlobal.dicMarketFiltersGlobal)
                {
                    populateWorksheetCell(ref worksheet, intRowCnt, intColCnt, item.Value, XLColor.NoColor, false, false, false);
                    intRowCnt++;
                }
                // }
            }
            worksheet.Columns().AdjustToContents();

        }

        private void populateWorksheetCell(ref IXLWorksheet worksheet, int row, int col, string strColumnValue, XLColor color, bool blBold, bool blForceCenter, bool blWrap)
        {
            //ADD COLUMN NAMES TO ROW 1 OF SHEET
            worksheet.Cell(row, col).Value = strColumnValue;
            worksheet.Cell(row, col).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(row, col).Style.Border.LeftBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(row, col).Style.Border.TopBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(row, col).Style.Border.RightBorder = XLBorderStyleValues.Thin;
            worksheet.Cell(row, col).Style.Font.Bold = blBold;
            worksheet.Cell(row, col).Style.Fill.BackgroundColor = color;
            worksheet.Cell(row, col).Style.Alignment.WrapText = blWrap;
            if (blForceCenter)
                worksheet.Cell(row, col).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        }

        private void finishReports(int rowCnt, string strFinalPath)
        {
            if (strFinalPath != null && rowCnt > 0)
            {
                DispatchHandler handler2 = delegate ()
                {
                    txtStatus.AppendText("Report generation completed!" + Environment.NewLine);
                    txtStatus.AppendText("Opening report..." + Environment.NewLine);
                };
                this.BeginInvoke(handler2);

                //alStatusGlobal.Add(new Status("Status", "Report generation completed!"));
                //alStatusGlobal.Add(new Status("Status", "Opening report..."));
                //populateStatus();



                //Create a new process info structure.
                ProcessStartInfo pInfo = new ProcessStartInfo();
                //Set the file name member of the process info structure.
                pInfo.FileName = strFinalPath;
                //Start the process.
                Process p = Process.Start(pInfo);
                //Wait for the window to finish loading.
                p.WaitForInputIdle();
                //Wait for the process to


                //sbStatusGlobal.Append("Report generation completed!" + Environment.NewLine);
                //sbStatusGlobal.Append("Opening report..." + Environment.NewLine);
                ////alStatusGlobal.Add(new Status("Status", "Report generation completed!"));
                ////alStatusGlobal.Add(new Status("Status", "Opening report..."));
                ////alStatusGlobal.Add(new Status("Success", "Report cancelled"));
                //populateHandlerStatus(sbStatusGlobal.ToString().Replace("{$time_rows}", strStatusTmpGlobal));


            }




            DispatchHandler handler3 = delegate ()
            {
                //if (rowCnt == -9999)
                //    txtStatus.AppendText("Report cancelled" + Environment.NewLine);
                //else if (rowCnt <= 0)
                //    txtStatus.AppendText("NO RESULTS FOUND!!! Modify your filters and try again." + Environment.NewLine);

                //txtStatus.AppendText("~----------------------------------------------------------------------------------------------------------------~" + Environment.NewLine);
                //txtStatus.AppendText("Ready" + Environment.NewLine);
                if (rowCnt == -9999)
                {
                    txtStatus.AppendLog("Report cancelled", statusDefaultColorGlobal,boldFontGlobal);


                    //sbStatusGlobal.Remove(0, sbStatusGlobal.Length);
                    //sbStatusGlobal.Append("Report cancelled");
                    //alStatusGlobal.Add(new Status("Success", "Report cancelled"));
                    //populateHandlerStatus();
                }

                else if (rowCnt <= 0)
                {
                    //sbStatusGlobal.Remove(0, sbStatusGlobal.Length);
                    //sbStatusGlobal.Append("NO RESULTS FOUND!!! Modify your filters and try again.");
                    ////alStatusGlobal.Add(new Status("Instruction", "NO RESULTS FOUND!!! Modify your filters and try again."));
                    //populateHandlerStatus();

                    txtStatus.AppendLog("NO RESULTS FOUND!!! Modify your filters and try again.", statusDefaultColorGlobal, boldFontGlobal);
                }

                populateStatus(false);


                btnGenerateReport.Text = "Generate Report";
            };
            this.BeginInvoke(handler3);

          

            //DispatchHandler handler1 = delegate ()
            //{
            //    btnGenerateReport.Text = "Generate Report";
            //};
            //this.BeginInvoke(handler1);
                //btnGenerateReport.Text = "Generate Report";
            // populateStatus();

            //populateHandlerStatus();



            this.UseWaitCursor = false;

        }

        #endregion  

        #region DYNAMIC SQL FOR FINAL REPORT
        Int16 intMeasureColCntGLOBAL;
        int intTotalExcelColumnsNMGLOBAL;
        DataTable dtMeasureSQLGLOBAL;
        private string getFinalSQL()
        {
            StringBuilder sbFinalSQL = new StringBuilder();
            StringBuilder sbInnerMeasureColumnFinalSQL = new StringBuilder();
            StringBuilder sbOuterMeasureColumnFinalSQL = new StringBuilder();
            StringBuilder sbOuterMeasureFilterFinalSQL = new StringBuilder();


            StringBuilder sbOuterSQLColumns = new StringBuilder();
            StringBuilder sbInnerSQLColumns = new StringBuilder();


            string strFacilityPractice = "Practice";


            //GET COLUMN COUNT TO HANDLE CLOSEDXML RESIZE BUG :(
            //CONSTANT COLUMNS
            if (!radFacility.Checked || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Practice (TIN and Name)") != -1)
            {
                sbOuterSQLColumns.Append("main.claim_tin as TIN,main.Pract_Name as [Practice Name],");
                sbInnerSQLColumns.Append("a.claim_tin,a.Pract_Name,");
                intTotalExcelColumnsNMGLOBAL += 2;
            }

            if (radFacility.Checked || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Facility (TIN, Name and Type)") != -1)
            {
                sbOuterSQLColumns.Append("main.facl_TIN as TIN,main.Fac_TIN_Name as [Facility Name],fac_type as [Facility Type],");
                sbInnerSQLColumns.Append("a.facl_TIN,a.Fac_TIN_Name,a.fac_type,");
                intTotalExcelColumnsNMGLOBAL += 3;
                strFacilityPractice = "Facility";
            }


            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("# of Surg") != -1)
            {
                sbOuterSQLColumns.Append("main.denom as [# of Surg],");
                intTotalExcelColumnsNMGLOBAL += 1;
            }


            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Avg Total Allowed") != -1)
            {
                sbOuterSQLColumns.Append("main.avg_epi as [Avg Total Allowed],");
                intTotalExcelColumnsNMGLOBAL += 1;
            }

            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("PD Status (# of Surg > 0 AND Provider = 'Practice')") != -1)
            {
                sbOuterSQLColumns.Append("main.pd_status as [PD status],main.mpins_cnt as [MPIN Count],");
                sbInnerSQLColumns.Append("a.pd_status,");
                intTotalExcelColumnsNMGLOBAL += 1;
                intTotalExcelColumnsNMGLOBAL += 1;
            }


            //DYNAMIC MEASURE SQL START
            //DYNAMIC MEASURE SQL START
            //DYNAMIC MEASURE SQL START
            string strMeasureOuterSQLColumns = null;
            string strMeasure2OuterSQLColumns = null;
            string strMeasureOuterSQLFilters = null;
            StringBuilder sbMeasureInnerSQLFrom = new StringBuilder();
            StringBuilder sbMearsureStatisticsFrom = new StringBuilder();


            string strConfidence = cmbConfidence.SelectedValue.ToString();
            string strSampleSize = cmbSampleSize.SelectedValue.ToString();

            intMeasureColCntGLOBAL = 9;
            //IF DETAILS NEEDED
            strMeasureOuterSQLColumns = "main.num{$MeasureId}  as [Adv Events~{$MeasureId}], main.rate{$MeasureId} as [" + strFacilityPractice + " Rate~{$MeasureId}], main.expt{$MeasureId} as [Exp Adv Events~{$MeasureId}], main.exp_rate{$MeasureId}  as [Exp Rate~{$MeasureId}],main.OE{$MeasureId} as [OE~{$MeasureId}], main.OE{$MeasureId}*v.avg_rate{$MeasureId} as [Adj Rate~{$MeasureId}], main.ChiSq{$MeasureId} as [Chi Sq~{$MeasureId}], case when denom<" + strSampleSize + " and main.OE{$MeasureId}>1.05 and main.ChiSq{$MeasureId}>" + strConfidence + " then 'High' when denom<" + strSampleSize + " and main.OE{$MeasureId}<1.05 and main.ChiSq{$MeasureId}>" + strConfidence + " then 'Low' when denom>=" + strSampleSize + " and main.OE{$MeasureId}>1.05 and main.ChiSq{$MeasureId}>" + strConfidence + " then 'Stat High' when denom>=" + strSampleSize + " and main.OE{$MeasureId}<1.05 and main.ChiSq{$MeasureId}>" + strConfidence + " then 'Stat Low' when main.ChiSq{$MeasureId}<" + strConfidence + " then 'Avg' end as [Stat Sign~{$MeasureId}], main.avg_cost{$MeasureId} as [Avg Cost~{$MeasureId}],";
            //strMeasureOuterSQLFilters = " (main.measure_id{$MeasureId} = {$MeasureId} AND main.expt{$MeasureId} > 0) OR";

            //2021
            strMeasure2OuterSQLColumns = "main.avg_epi{$MeasureId} as [Total~{$MeasureId}], main.avg_mgt{$MeasureId} as [Mgt~{$MeasureId}], main.avg_surg{$MeasureId} as [Surg~{$MeasureId}], main.avg_facl{$MeasureId} as [Facl~{$MeasureId}], main.avg_ip{$MeasureId} as [Inp~{$MeasureId}], main.avg_outp{$MeasureId} as [Otp~{$MeasureId}], main.avg_rx{$MeasureId} as [Pharm~{$MeasureId}],";




            strMeasureOuterSQLFilters = " (main.expt{$MeasureId} > 0) OR";
            sbMeasureInnerSQLFrom.Append("from dbo.compl_app as a ");

            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() > 0 || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Specialty") != -1)
            {
                sbMeasureInnerSQLFrom.Append("inner join dbo.PBP_dim_Spec as sp on sp.Spec_id = a.Spec_id ");
                if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() == 1 || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Specialty") != -1)
                {
                    intTotalExcelColumnsNMGLOBAL++;
                    sbOuterSQLColumns.Append("main.Spec_Desc as Specialty,");
                    sbInnerSQLColumns.Append("sp.Spec_Desc,");
                }
            }

            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() > 0 || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Surgery") != -1)
            {
                sbMeasureInnerSQLFrom.Append("inner join dbo.dim_peg_spec as sur on sur.surg_id = a.surg_id ");
                if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() == 1 || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Surgery") != -1)
                {
                    intTotalExcelColumnsNMGLOBAL++;
                    sbOuterSQLColumns.Append("main.surg_desc as Surgery,");
                    sbInnerSQLColumns.Append("sur.surg_desc,");
                }
            }

            if (!cmbLOB.SelectedValue.ToString().Equals("4") || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Line of Business") != -1)
            {
                intTotalExcelColumnsNMGLOBAL++;
                sbMeasureInnerSQLFrom.Append("inner join dbo.dim_lob as lob on lob.lob_id = a.lob_id ");
                sbOuterSQLColumns.Append("main.lob_desc as LOB,");
                sbInnerSQLColumns.Append("lob.lob_desc,");

            }


            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() > 0 || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Market Name") != -1)
            {
                if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() == 1 || frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Market Name") != -1)
                {
                    intTotalExcelColumnsNMGLOBAL++;
                    intTotalExcelColumnsNMGLOBAL++;
                    //sbOuterSQLColumns.Append("main.UNET_MKT_NBR as [Market Number], main.UNET_MKT_NM as [Market Name],");
                    //sbInnerSQLColumns.Append("a.UNET_MKT_NBR,a.UNET_MKT_NM,");
                    sbOuterSQLColumns.Append("main.UNET_MKT_NM as [Market Name],");
                    sbInnerSQLColumns.Append("a.UNET_MKT_NM,");
                }
            }

            sbMeasureInnerSQLFrom.Append("where " + (!radFacility.Checked ? "a.Pract_name" : "a.Fac_TIN_Name") + " is not null");

            if (dtMeasureSQLGLOBAL == null)
            {
                dtMeasureSQLGLOBAL = new DataTable();

                //DYNAMIC MEASURE SECTION ADD MORE FOR EXCEL LIKE COLOR AND HEADERS!!!!!!!
                dtMeasureSQLGLOBAL.Columns.Add("OrderId", typeof(int));
                dtMeasureSQLGLOBAL.Columns.Add("MeasureId", typeof(int));
                dtMeasureSQLGLOBAL.Columns.Add("InnerSQL", typeof(string));
                dtMeasureSQLGLOBAL.Columns.Add("ColorArray", typeof(string));
                dtMeasureSQLGLOBAL.Columns.Add("Verbiage", typeof(string));
                dtMeasureSQLGLOBAL.Columns.Add("ExcelSheet", typeof(string));

                dtMeasureSQLGLOBAL.Rows.Add(1, 327, "sum(a.com_adm) as num327, sum(a.com_adm)/count(*) as rate327, sum(a.exp_com_adm) as expt327, sum(a.exp_com_adm)/count(*) as exp_rate327, case when sum(a.exp_com_adm)=0 then 0 else power((sum(a.com_adm)-sum(a.exp_com_adm)),2)/sum(a.exp_com_adm) end + power((sum(a.com_adm)-sum(a.exp_com_adm)),2)/(count(*)-sum(a.exp_com_adm)) as ChiSq327, case when sum(a.exp_com_adm)=0 then 0 else sum(a.com_adm)/sum(a.exp_com_adm) end as OE327, sum(case when a.com_adm<>0 then a.TOT_PEG_ALLW_AMT else 0 end) as tot_cost327, case when sum(a.com_adm)=0 then 0 else sum(case when a.com_adm=0 then 0 else a.TOT_PEG_ALLW_AMT end)/sum(a.com_adm) end as avg_cost327,", "252,228,214", "30-Day Post-Procedural Inp Adm with Compl DXs", "Output");

                dtMeasureSQLGLOBAL.Rows.Add(2, 325, "sum(a.adm) as num325, sum(a.adm)/count(*) as rate325, sum(a.exp_adm) as expt325, sum(a.exp_adm)/count(*) as exp_rate325, case when sum(a.exp_adm)=0 then 0 else power((sum(a.adm)-sum(a.exp_adm)),2)/sum(a.exp_adm) end + case when count(*)=sum(a.exp_adm) then 0 else power((sum(a.adm)-sum(a.exp_adm)),2)/(count(*)-sum(a.exp_adm)) end as ChiSq325, case when sum(a.exp_adm)=0 then 0 else sum(a.adm)/sum(a.exp_adm) end as OE325, sum(case when a.adm<>0 then a.TOT_PEG_ALLW_AMT else 0 end) as tot_cost325, case when sum(a.adm)=0 then 0 else sum(case when a.adm=0 then 0 else a.TOT_PEG_ALLW_AMT end)/sum(a.adm) end as avg_cost325,", "221,235,247", "30-Day Post-Procedural Inp Adm w/o Compl DXs", "Output");

                dtMeasureSQLGLOBAL.Rows.Add(3, 326, "sum(a.ed) as num326, sum(a.ed)/count(*) as rate326, sum(a.exp_ed) as expt326, sum(a.exp_ed)/count(*) as exp_rate326, case when sum(a.exp_ed)=0 then 0 else power((sum(a.ed)-sum(a.exp_ed)),2)/sum(a.exp_ed) end + case when count(*)=sum(a.exp_ed) then 0 else power((sum(a.ed)-sum(a.exp_ed)),2)/(count(*)-sum(a.exp_ed)) end as ChiSq326, case when sum(a.exp_ed)=0 then 0 else sum(a.ed)/sum(a.exp_ed) end as OE326, sum(case when a.ed<>0 then a.TOT_PEG_ALLW_AMT else 0 end) as tot_cost326, case when sum(a.ed)=0 then 0 else sum(case when a.ed=0 then 0 else a.TOT_PEG_ALLW_AMT end)/sum(a.ed) end as avg_cost326,", "255, 242,204", "30-Day Post-Procedural ED Visit", "Output");

                dtMeasureSQLGLOBAL.Rows.Add(4, 324, "sum(a.com) as num324, sum(a.com)/count(*) as rate324, sum(a.exp_com) as expt324, sum(a.exp_com)/count(*) as exp_rate324, case when sum(a.exp_com)=0 then 0 else power((sum(a.com)-sum(a.exp_com)),2)/sum(a.exp_com) end + case when count(*)=sum(a.exp_com) then 0 else power((sum(a.com)-sum(a.exp_com)),2)/(count(*)-sum(a.exp_com)) end as ChiSq324, case when sum(a.exp_com)=0 then 0 else sum(a.com)/sum(a.exp_com) end as OE324, sum(case when a.com<>0 then a.TOT_PEG_ALLW_AMT else 0 end) as tot_cost324, case when sum(a.com)=0 then 0 else sum(case when a.com=0 then 0 else a.TOT_PEG_ALLW_AMT end)/sum(a.com) end as avg_cost324,", "226,239,218", "30-Day Post-Procedural Complication Rate", "Output");

                dtMeasureSQLGLOBAL.Rows.Add(5, 9999, "sum(0.75*(a.com_adm+a.adm)+0.15*a.ed+0.1*a.com) as num9999, sum(0.75*(a.com_adm+a.adm)+0.15*a.ed+0.1*a.com)/count(*) as rate9999, sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com) as expt9999, sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com)/count(*) as exp_rate9999, case when sum(a.com_adm+a.adm+a.ed+a.com)=0 then 0 else power(sum(0.75*(a.com_adm+a.adm)+0.15*a.ed+0.1*a.com)-sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com),2)/sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com) end + case when count(*)=sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com) then 0 else power(sum(0.75*(a.com_adm+a.adm)+0.15*a.ed+0.1*a.com)-sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com),2)/(count(*)-sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com)) end as ChiSq9999, case when sum(a.com_adm+a.adm+a.ed+a.com)=0 then 0 else sum(0.75*(a.com_adm+a.adm)+0.15*a.ed+0.1*a.com)/sum(0.75*(a.exp_com_adm+a.exp_adm)+0.15*a.exp_ed+0.1*a.exp_com) end as OE9999, sum(case when a.com_adm<>0 then a.TOT_PEG_ALLW_AMT else 0 end + case when a.adm<>0 then a.TOT_PEG_ALLW_AMT else 0 end + case when a.ed<>0 then a.TOT_PEG_ALLW_AMT else 0 end + case when a.com<>0 then a.TOT_PEG_ALLW_AMT else 0 end) as tot_cost9999, case when sum(a.com_adm+a.adm+a.ed+a.com)=0 then 0 else sum(case when a.com_adm=0 then 0 else a.TOT_PEG_ALLW_AMT end + case when a.adm=0 then 0 else a.TOT_PEG_ALLW_AMT end + case when a.ed=0 then 0 else a.TOT_PEG_ALLW_AMT end + case when a.com=0 then 0 else a.TOT_PEG_ALLW_AMT end) /sum(a.com_adm+a.adm+a.ed+a.com) end as avg_cost9999,", "226,226,200", "30-Day Composite", "Output");

                dtMeasureSQLGLOBAL.Rows.Add(6, 8888, "sum(a.TOT_PEG_ALLW_AMT)/count(*) as avg_epi8888, sum(a.MGT_ALLW_AMT)/count(*) as avg_mgt8888, sum(a.SURG_ALLW_AMT)/count(*) as avg_surg8888, sum(a.FACL_ALLW_AMT)/count(*) as avg_facl8888, sum(a.IPTNT_ALLW_AMT)/count(*) as avg_ip8888, sum(a.OPTNT_ALLW_AMT)/count(*) as avg_outp8888, sum(a.PHRM_ALLW_AMT)/count(*) as avg_rx8888,", "255,242,204", "Average Allow Amount per Episode", "Output2");

            }

            DataRow[] drMeasures;
            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Measures") != -1)
            {

                if (frmFiltersGlobal.dicMeasureFiltersGlobal.Count() <= 0)
                    drMeasures = dtMeasureSQLGLOBAL.Select("ExcelSheet='Output'");
                else
                    drMeasures = dtMeasureSQLGLOBAL.Select("MeasureId in (" + string.Join(",", frmFiltersGlobal.dicMeasureFiltersGlobal.Keys) + ")");

                foreach (DataRow dr in drMeasures)
                {
                    sbOuterMeasureColumnFinalSQL.Append(strMeasureOuterSQLColumns.Replace("{$MeasureId}", dr["MeasureId"].ToString()));
                    sbOuterMeasureFilterFinalSQL.Append(strMeasureOuterSQLFilters.Replace("{$MeasureId}", dr["MeasureId"].ToString()));
                    sbInnerMeasureColumnFinalSQL.Append(dr["InnerSQL"].ToString());

                    switch (dr["MeasureId"].ToString())
                    {
                        case "327":
                            sbMearsureStatisticsFrom.Append("sum(com_adm)/ count(*) as avg_rate327,");
                            break;
                        case "325":
                            sbMearsureStatisticsFrom.Append("sum(adm) / count(*) as avg_rate325,");
                            break;
                        case "326":
                            sbMearsureStatisticsFrom.Append("sum(a.ed) / count(*) as avg_rate326,");
                            break;
                        case "324":
                            sbMearsureStatisticsFrom.Append("sum(com) / count(*) as avg_rate324,");
                            break;
                        case "9999":
                            sbMearsureStatisticsFrom.Append("sum(0.75 * (a.com_adm + a.adm) + 0.15 * a.ed + 0.1 * a.com) / count(*) as avg_rate9999,");
                            break;
                        default:
                            break;
                    }

                }

            }

            //2021
            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("Average Allowed Per Episode (Additional Sheet)") != -1)
            {
                drMeasures = dtMeasureSQLGLOBAL.Select("ExcelSheet='Output2'");
                foreach (DataRow dr in drMeasures)
                {
                    sbOuterMeasureColumnFinalSQL.Append(strMeasure2OuterSQLColumns.Replace("{$MeasureId}", dr["MeasureId"].ToString()));
                    sbInnerMeasureColumnFinalSQL.Append(dr["InnerSQL"].ToString());
                }
            }


            //DYNAMIC MEASURE SQL END
            //DYNAMIC MEASURE SQL END
            //DYNAMIC MEASURE SQL END



            //DYNAMIC FILTER SQL START
            //DYNAMIC FILTER SQL START
            //DYNAMIC FILTER SQL START
            //CREATE INDEX indx_compl_app_lst_run_qrt_all ON compl_app(lst_run_qrt, surg_id, claim_tin_num, Spec_id);
            //CREATE INDEX indx_compl_app_claim_tin ON compl_app(claim_tin_num);
            //CREATE INDEX indx_compl_app_Spec_id ON compl_app(Spec_id);
            //CREATE INDEX indx_compl_app_surg_id ON compl_app(surg_id);
            StringBuilder sbInnerSQLWhere = new StringBuilder();
            StringBuilder sbInnerStatisticsSQLWhere = new StringBuilder();
            //if (frmFiltersGlobal.dicQuarterFiltersGlobal.Count() > 0)
            //    sbInnerSQLWhere.Append(" AND a.lst_run_qrt in (" + string.Join(",", frmFiltersGlobal.dicQuarterFiltersGlobal.Keys) + ")");
            sbInnerSQLWhere.Append(" AND a.lst_run_qrt = '"+ cmbQuarter.SelectedValue +"'");
            sbInnerStatisticsSQLWhere.Append(" AND a.lst_run_qrt = '" + cmbQuarter.SelectedValue + "'");

            if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count() > 0)
            {
                var csv = String.Join(",", Array.ConvertAll(frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Keys.ToArray(), z => "'" + z + "'"));
                sbInnerSQLWhere.Append(" AND a.fac_type in (" + csv + ")");
                sbInnerStatisticsSQLWhere.Append(" AND a.fac_type in (" + csv + ")");
            }


            if (frmFiltersGlobal.dicProviderFiltersGlobal.Count() > 0)
            {
                if (!radFacility.Checked)
                    sbInnerSQLWhere.Append(" AND a.claim_tin_num  in (" + string.Join(",", frmFiltersGlobal.dicProviderFiltersGlobal.Keys) + ")");
                else
                    sbInnerSQLWhere.Append(" AND a.facl_TIN  in (" + string.Join(",", frmFiltersGlobal.dicProviderFiltersGlobal.Keys) + ")");
            }


            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() > 0)
            {
                sbInnerSQLWhere.Append(" AND a.surg_id in (" + string.Join(",", frmFiltersGlobal.dicSurgicalFiltersGlobal.Keys) + ")");
                sbInnerStatisticsSQLWhere.Append(" AND a.surg_id in (" + string.Join(",", frmFiltersGlobal.dicSurgicalFiltersGlobal.Keys) + ")");
            }
                

            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() > 0)
            {
                sbInnerSQLWhere.Append(" AND a.Spec_id in (" + string.Join(",", frmFiltersGlobal.dicSpecialtyFiltersGlobal.Keys) + ")");
                sbInnerStatisticsSQLWhere.Append(" AND a.Spec_id in (" + string.Join(",", frmFiltersGlobal.dicSpecialtyFiltersGlobal.Keys) + ")");
            }
                


            if (!cmbLOB.SelectedValue.ToString().Equals("4"))
            {
                sbInnerSQLWhere.Append(" AND a.lob_id = " + cmbLOB.SelectedValue.ToString());
                sbInnerStatisticsSQLWhere.Append(" AND a.lob_id = " + cmbLOB.SelectedValue.ToString());
            }
                


            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() > 0)
            {
                var csv = String.Join(",", Array.ConvertAll(frmFiltersGlobal.dicMarketFiltersGlobal.Values.ToArray(), z => "'" + z + "'"));
                sbInnerSQLWhere.Append(" AND a.UNET_MKT_NM in (" + csv + ")");
                sbInnerStatisticsSQLWhere.Append(" AND a.UNET_MKT_NM in (" + csv + ")");
            }
      

            //DYNAMIC FILTER SQL END
            //DYNAMIC FILTER SQL END
            //DYNAMIC FILTER SQL END


            sbFinalSQL.Append("SELECT " + sbOuterSQLColumns.ToString().TrimEnd(','));

            if (sbOuterMeasureFilterFinalSQL.Length > 0)
                sbFinalSQL.Append("," + sbOuterMeasureColumnFinalSQL.ToString().TrimEnd(',') + " ");

            sbFinalSQL.Append("FROM ( ");
            sbFinalSQL.Append("SELECT ");
            sbFinalSQL.Append(sbInnerSQLColumns.ToString().TrimEnd(',') + ",count(*) as denom,sum(a.TOT_PEG_ALLW_AMT)/count(*) as avg_epi ");

            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("PD Status (# of Surg > 0 AND Provider = 'Practice')") != -1)
                sbFinalSQL.Append(",count(distinct PROV_mpin) as mpins_cnt  ");

            if (sbOuterMeasureFilterFinalSQL.Length > 0)
                sbFinalSQL.Append("," + sbInnerMeasureColumnFinalSQL.ToString().TrimEnd(',') + " ");

            sbFinalSQL.Append(sbMeasureInnerSQLFrom.ToString());
            sbFinalSQL.Append(sbInnerSQLWhere.ToString() + " ");
            sbFinalSQL.Append("GROUP BY " + sbInnerSQLColumns.ToString().TrimEnd(','));
            sbFinalSQL.Append(") main ");


            sbFinalSQL.Append("inner join ( select ");
            sbFinalSQL.Append(sbMearsureStatisticsFrom.ToString().TrimEnd(',') + " ");
            sbFinalSQL.Append("FROM dbo.compl_app as a ");
            sbFinalSQL.Append("WHERE 1=1 "+ sbInnerStatisticsSQLWhere.ToString() +") as v on 1 = 1 ");





            sbFinalSQL.Append("WHERE 1=1 ");

            if (sbOuterMeasureFilterFinalSQL.Length > 0)
                sbFinalSQL.Append(" AND " + sbOuterMeasureFilterFinalSQL.ToString().TrimEnd('O', 'R'));

            if (frmColumnsGlobal.clbColumns.CheckedItems.IndexOf("PD Status (# of Surg > 0 AND Provider = 'Practice')") != -1)
                sbFinalSQL.Append(" AND main.avg_epi > 0 ");

 
            if (!radFacility.Checked)
                sbFinalSQL.Append("ORDER BY TIN, [# of Surg] DESC  ");
            else
                sbFinalSQL.Append("ORDER BY [# of Surg] DESC  ");


            return sbFinalSQL.ToString();
        }
        #endregion

        #region PROVIDER SMART SEARCH START
        bool blDontRunHandler;
        bool blStopSmartSearch = false;
        private void cmbProvider_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int intTotalResults = 30;
                int intTotalDifference = 0;
                DataTable dtFinalResults = null;
                string strFilterColumn = null;
                string strFilterAgainstExistingFilters = null;

                if (blStopSmartSearch == true)
                    return;

                string strSearchString = cmbProvider.Text.Replace("\"", "").Replace("[", "").Replace("]", "").Replace(",", "").Replace("-", "").Replace("'", "");

                //if(strSearchString == "sunnt")
                //{
                //    string ss = "";
                //}

                if (strSearchString.Length > 0)
                {

                    //2021 CHECK FOR TIN CSV AND THEN NUMERIC ONLY ARRAY, IF SO BATCH MATCH AND DISPLAY!!!!????!!???
                    //EXIT SMART SEARCH FUNCTIONALITY
                    bool blCSVProviders = false;
                    string[] strArr = cmbProvider.Text.Split(',');
                    if (strArr.Length > 1)
                    {
                        foreach (string s in strArr)
                        {
                            if (s.Trim().IsNumeric())
                                blCSVProviders = true;
                            else
                            {
                                blCSVProviders = false;
                                break;
                            }
                        }
                    }

                    if (blCSVProviders)
                    {
                        StringBuilder sbMissing = new StringBuilder();
                        foreach (string s in strArr)
                        {
                            //MAGIC HERE!!!!!
                            //CHECK PROVIDER VS FACILITY 
                            //COMPARE TO ARRAY
                            //ADD TO SEARCH 
                            //NOTIFIY MISSING MATCHES
                            //DONE!!!

                            DataRow dr = dtProviderFiltersGLOBAL.Select("tin = '" + s.Trim() + "'").FirstOrDefault();
                            if (dr != null)
                            {
                                if (!frmFiltersGlobal.dicProviderFiltersGlobal.ContainsKey(dr["tin"].ToString().TrimStart(new Char[] { '0' })))
                                {
                                    frmFiltersGlobal.dicProviderFiltersGlobal.Add(dr["tin"].ToString().TrimStart(new Char[] { '0' }), dr["provider_name"].ToString());
                                    //MessageBox.Show("{" + dr["Pract_Name"].ToString() + "} added to Provider Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    //txtStatus.AppendText("{" + dr["provider_name"].ToString() + "} added to provider filters list" + Environment.NewLine);
                                    btnGetFilters.ForeColor = Color.White;
                                    btnGetFilters.BackColor = alertColorGlobal;
                                    alStatusGlobal.Clear();
                                    alStatusGlobal.Add(new Status("Success","{" + dr["provider_name"].ToString() + "} has been added to provider filters list."));
                                    alStatusGlobal.Add(new Status("Instruction", "Click the 'Refresh Filters' button to genertate your report.", btnGetFilters.BackColor));
                                    populateStatus();


                                    grpAdditionalFilters.Enabled = false;
                                }

                            }
                            else
                            {

                                sbMissing.Append(s + Environment.NewLine);
                                //txtStatus.AppendText("TIN " + s + " was not found in database " + Environment.NewLine);



                                alStatusGlobal.Clear();
                                ///alStatusGlobal.Add(new Status("{" + dr["provider_name"].ToString() + "} has been added to provider filters list.", defaultColor, txtStatus.Font));
                                alStatusGlobal.Add(new Status("Instruction", "TIN " + s + " was not found in database"));
                                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());
                                //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
                                //populateStatus();


                                //FOLLOWING NOT FOUND

                            }

                        }

                        cleanProviderFilters(true);
                        refreshProviderFilters();
                        cmbProvider.DataSource = null;
                        cmbProvider.Items.Clear();

                        cmbProvider.Text = "";
                        cmbProvider.DroppedDown = false;


                        if (sbMissing.Length > 0)
                        {
                            MessageBox.Show("The following TINs could not be found in the database:" + Environment.NewLine + sbMissing.ToString());

                        }

                        return;
                    }


                    blStopSmartSearch = true;

                    var text = strSearchString;

                    if (text.IsNumeric())
                        strFilterColumn = "tin";
                    else
                        strFilterColumn = "provider_name";

                    if (frmFiltersGlobal.dicProviderFiltersGlobal.Count() > 0)
                    {
                        strFilterAgainstExistingFilters = " AND tin_num not in (" + string.Join(", ", frmFiltersGlobal.dicProviderFiltersGlobal.Keys) + ")";
                    }


                    try
                    {
                        var providerFilter = "provider_type = 'Practice'";

                        if (radFacility.Checked)
                        {
                            providerFilter = "provider_type = 'Facility'";
                            if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count() > 0)
                            {
                                var csv = String.Join(",", Array.ConvertAll(frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Keys.ToArray(), z => "'" + z + "'"));
                                providerFilter += " AND fac_type in (" + csv + ")";
                            }

                        }

                        DataTable dtFinalProv = dtProviderFiltersGLOBAL.Select(providerFilter).CopyToDataTable();


                        DataRow[] drWild1 = dtFinalProv.Select(strFilterColumn + " LIKE '" + text + "%'" + strFilterAgainstExistingFilters);
                        if (drWild1.Count() > 0)
                            dtFinalResults = drWild1.CopyToDataTable();
                        if (dtFinalResults == null)
                        {
                            dtFinalResults = new DataTable();
                            intTotalDifference = intTotalResults;
                        }
                        else
                            intTotalDifference = intTotalResults - dtFinalResults.Rows.Count;


                        if (intTotalDifference > 0)
                        {
                            DataRow[] drWild2 = dtFinalProv.Select(strFilterColumn + " LIKE '%" + text + "%'" + strFilterAgainstExistingFilters);
                            int intTotalTake = (intTotalDifference > drWild2.Count() ? drWild2.Count() : intTotalDifference);
                            if (drWild2.Count() > 0)
                                dtFinalResults.Merge(drWild2.CopyToDataTable().AsEnumerable().Take(intTotalTake).CopyToDataTable());
                        }
                    }
                    catch (Exception exDELETEME)
                    {
                        string s = exDELETEME.ToString();
                    }


                    if (dtFinalResults.Rows.Count > 0)
                    {

                        var dict = new Dictionary<string, string>();
                        foreach (DataRow row in dtFinalResults.Rows)
                        {
                            if (dict.ContainsKey(row["tin"].ToString()))
                                continue;
                            dict.Add(row["tin"].ToString(), row["provider_name"] + " - " + row["tin"]);

                        }

                        BindingSource bSource = new BindingSource();
                        bSource.DataSource = dict;

                        cmbProvider.ValueMember = "Key";
                        cmbProvider.DisplayMember = "Value";
                        cmbProvider.DataSource = bSource;


                        cmbProvider.DroppedDown = true;
                        Cursor.Current = Cursors.Default;
                        cmbProvider.Text = text;



                        cmbProvider.SelectionStart = cmbProvider.Text.Length; // add some logic if length is 0
                        cmbProvider.SelectionLength = 0;
                    }
                    else
                    {
                        cmbProvider.DroppedDown = false;
                    }

                }
            }
            catch (Exception ex)
            {
                cleanupError(ex.Message);
            }
        }

        private void cmbProvider_MouseClick(object sender, MouseEventArgs e)
        {
            if (blDontRunHandler)
                return;

            blStopSmartSearch = true;
        }

        private void cmbProvider_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (blDontRunHandler)
                return;
          
            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.Items.Count <= 0)
            {
                //cmbProvider.DataSource = null;
                //cmbProvider.Items.Clear();
                //cmbProvider.ResetText();
                //cmbProvider.DroppedDown = false;
                //cmbProvider.SelectedIndex = -1;
                //cmbProvider.Text = "";
                //cmbProvider.SelectionStart = 0;
                //cmbProvider.SelectionLength = 0;
                return;
            }
            
            cmbProvider.Text = senderComboBox.SelectedItem.ToString();
            blStopSmartSearch = true;

            addProviderToFilters();
        }

        private void cmbProvider_KeyDown(object sender, KeyEventArgs e)
        {
            if (blDontRunHandler)
                return;

            if (e.KeyValue == (int)Keys.Enter || e.KeyValue == (int)Keys.Up || e.KeyValue == (int)Keys.Down)
            {
                blStopSmartSearch = true;

                if (e.KeyValue == (int)Keys.Enter)
                    addProviderToFilters();
            }
            else
                blStopSmartSearch = false;
        }

        private void cmbProvider_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blDontRunHandler)
                return;

            ////blStopSmartSearch = true;
            //ComboBox senderComboBox = (ComboBox)sender;
            //MessageBox.Show("SelectedIndexChanged Sender = " + senderComboBox.SelectedItem.ToString());
            ////if (senderComboBox.SelectedIndex > 0)
            ////    addProviderToFilters();

            //MessageBox.Show("SelectedIndexChanged cmbProvider = " + cmbProvider.SelectedItem.ToString());
        }

        private void cmbProvider_DropDownClosed(object sender, EventArgs e)
        {
            if (blDontRunHandler)
                return;

            //ComboBox senderComboBox = (ComboBox)sender;

            //MessageBox.Show(cmbProvider.SelectedItem.ToString());

            //MessageBox.Show("SelectedIndexChanged Sender = " + senderComboBox.SelectedItem.ToString());
            //if (senderComboBox.SelectedIndex > 0)
            //    addProviderToFilters();

            //COMMENTED 3172021 DONT REMOVE BAD TEXT!!!!!
            //COMMENTED 3172021 DONT REMOVE BAD TEXT!!!!!
            //COMMENTED 3172021 DONT REMOVE BAD TEXT!!!!!
            //if (cmbProvider.Text.Length > 0)
            //{
            //    //MessageBox.Show("OOPS!!!!");
            //    cmbProvider.Text = "";
            //}

        }
    
        private void cmbProvider_Click(object sender, EventArgs e)
        {

            try
            {

                //NEVER CALLED ADDED 3172021 TO TEST COMBO SUPRESSION
                //GONNA TRY IN RERESH AREA INSTEAD
                blDontRunHandler = false;
                ComboBox senderComboBox = (ComboBox)sender;
                if (senderComboBox.Items.Count <= 0)
                {

                    cmbProvider.Items.Clear();
                    cmbProvider.SelectedIndex = -1;

                    cmbProvider.Focus();

                    SendKeys.Send("{esc}");





                    //alStatusGlobal.Clear();
                    //alStatusGlobal.Add(new Status("Instruction", "You must type the name of the " + (radFacility.Checked? "Facility" : "Practice") + ".", btnGetFilters.BackColor));
                    //populateStatus();

                    // cmbProvider = new ComboBox();
                    //cmbProvider.DataSource = null;
                    //cmbProvider.Items.Clear();
                    //cmbProvider.ResetText();
                    //cmbProvider.SelectedIndex = -1;
                    //cmbProvider.Text = "";
                    //cmbProvider.SelectionStart = 0;
                    //cmbProvider.SelectionLength = 0;

                }
            }
            catch (Exception ex)
            {
                cleanupError(ex.Message);
            }
        }
        private void addProviderToFilters()
        {
            try
            {
                if (String.IsNullOrEmpty(cmbProvider.Text))
                    return;


                //JUST IN CASE CHECK/GET DATATABLE AGAIN...
                refreshProviderFilters();

                //ADDED 3172021 TO FIX INVALID SELECTION RESULTING IN EMPTY COMBOS
                //ADDED 3172021 TO FIX INVALID SELECTION RESULTING IN EMPTY COMBOS
                //ADDED 3172021 TO FIX INVALID SELECTION RESULTING IN EMPTY COMBOS
                if (cmbProvider.SelectedValue == null)
                    return;

                string strSearch = cmbProvider.SelectedValue.ToString();


                DataRow dr = dtProviderFiltersGLOBAL.Select("tin = '" + strSearch + "'").FirstOrDefault();
                if (dr != null)
                {
                    if (!frmFiltersGlobal.dicProviderFiltersGlobal.ContainsKey(dr["tin"].ToString().TrimStart(new Char[] { '0' })))
                    {
                        frmFiltersGlobal.dicProviderFiltersGlobal.Add(dr["tin"].ToString().TrimStart(new Char[] { '0' }), dr["provider_name"].ToString());
                        //MessageBox.Show("{" + dr["Pract_Name"].ToString() + "} added to Provider Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // txtStatus.AppendText("{" + dr["provider_name"].ToString() + "} added to provider filters list" + Environment.NewLine);

                        //  txtStatus.AppendText("Additonal filters must be refreshed" + Environment.NewLine);
                        btnGetFilters.ForeColor = Color.White;
                        btnGetFilters.BackColor = alertColorGlobal;

                        alStatusGlobal.Clear();
                        alStatusGlobal.Add(new Status("Success", "{" + dr["provider_name"].ToString() + "} has been added to provider filters list."));
                        alStatusGlobal.Add(new Status("Instruction", "Click the 'Refresh Filters' button to genertate your report.", btnGetFilters.BackColor));
                        populateStatus();

                        grpAdditionalFilters.Enabled = false;
                    }

                    cleanProviderFilters(true);
                    refreshProviderFilters();
                    cmbProvider.DataSource = null;
                    cmbProvider.Items.Clear();

                }
            }
            catch (Exception ex)
            {
                cleanupError(ex.Message);
            }

        }

        #endregion  

        #region FILTER FORM
        //ALL FITERS CLICK
        private void btnFilters_Click(object sender, EventArgs e)
        {

            Button btn = sender as Button;

            frmFiltersGlobal.populateFilters(btn.Name.Replace("btn", "").Replace("Filters", ""));
            frmFiltersGlobal.ShowDialog();
        }

        //WHEN FITER FORM BECOMES INVISIBLE LETS REFERSH APPROPRIATE FILTER LIST ON MAIN FORM
        private void filterForm_Visibility(object sender, EventArgs e)
        {
            //IF FILTERS BECAME INVISIBLE THEN LETS SEE WHAT CHANGED
            if (!frmFiltersGlobal.Visible)
            {
                if (frmFiltersGlobal.blResetGLOBAL)
                {
                    switch (frmFiltersGlobal.strCurrentFilterGLOBAL)
                    {
                        case "Quarter":
                            refreshQuarterFilters();
                            if (frmFiltersGlobal.dicQuarterFiltersGlobal.Count <= 0)
                                cleanQuarterFilters(false);
                            break;
                        case "Measure":
                            refreshMeasureFilters();
                            if (frmFiltersGlobal.dicMeasureFiltersGlobal.Count <= 0)
                                cleanMeasureFilters(false);
                            break;
                        case "Market":
                            updateFilters(frmFiltersGlobal.strCurrentFilterGLOBAL);
                            refreshMarketFilters();
                            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count <= 0)
                                cleanMarketFilters(false);
                            break;
                        case "Surgical":
                            updateFilters(frmFiltersGlobal.strCurrentFilterGLOBAL);
                            refreshSurgicalFilters();
                            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count <= 0)
                                cleanSurgicalFilters(false);
                            break;
                        case "Specialty":
                            updateFilters(frmFiltersGlobal.strCurrentFilterGLOBAL);
                            refreshSpecialtyFilters();
                            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count <= 0)
                                cleanSpecialtyFilters(false);
                            break;
                        case "Provider":
                            refreshProviderFilters();
                            //txtStatus.AppendText("Additonal filters must be refreshed" + Environment.NewLine);
                            btnGetFilters.ForeColor = Color.White;
                            btnGetFilters.BackColor = alertColorGlobal;
                            alStatusGlobal.Clear();
                            alStatusGlobal.Add(new Status("Instruction", "Click the 'Refresh Filters' button to genertate your report.", btnGetFilters.BackColor));
                            populateStatus();
                            grpAdditionalFilters.Enabled = false;
                            if (frmFiltersGlobal.dicProviderFiltersGlobal.Count <= 0)
                                cleanProviderFilters(false);
                            break;
                        case "FacilityType":
                            refreshFacilityTypeFilters();
                            if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count <= 0)
                                cleanFacilityTypeFilters(false);
                            break;
                    }

                    frmFiltersGlobal.blResetGLOBAL = false;

                    alStatusGlobal.Clear();
                    ///alStatusGlobal.Add(new Status("{" + dr["provider_name"].ToString() + "} has been added to provider filters list.", defaultColor, txtStatus.Font));
                    alStatusGlobal.Add(new Status("Success", "Filters have been updated"));
                    addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());

                }
                else if(!frmColumnsGlobal.Visible)
                {

                    alStatusGlobal.Clear();
                    ///alStatusGlobal.Add(new Status("{" + dr["provider_name"].ToString() + "} has been added to provider filters list.", defaultColor, txtStatus.Font));
                    alStatusGlobal.Add(new Status("Success", "Columns have been updated"));
                    addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());


                    //addDefaultReadyStatus();
                }

                //ADDED 3172021 IF COLUMN SELECT THAT CONFLICTS WITH FACILITY OR PRACTICE RESET!!!!
                //ADDED 3172021 IF COLUMN SELECT THAT CONFLICTS WITH FACILITY OR PRACTICE RESET!!!!
                //ADDED 3172021 IF COLUMN SELECT THAT CONFLICTS WITH FACILITY OR PRACTICE RESET!!!!
                //if (!frmColumnsGlobal.Visible)
                //{
                //    if (frmColumnsGlobal.blChangeToPracticeGLOBAL == true)
                //    {
                //        radPractice.Checked = true;
                //        grpFacilityType.Enabled = false;
                //        grpProviders.Text = "Practice:";
                //        frmFiltersGlobal.dicProviderFiltersGlobal.Clear();
                //        frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Clear();
                //        refreshFacilityTypeFilters();
                //        grpAdditionalFilters.Enabled = false;
                //    }

                //}
                

                this.Show();
            }
        }
        #endregion  

        #region SELECTED INDEX CHANGED FILTERS
        private void cmbLOB_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //ComboBox senderComboBox = (ComboBox)sender;
            //if (senderComboBox.SelectedIndex > 0)
            // {
            updateFilters("LOB");
            //refreshLOBFilters();
            //  }
            alStatusGlobal.Clear();
            //alStatusGlobal.Add(new Status("LOB changed to " + cmbLOB.Text, defaultColor, boldFont));
            //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
            addDefaultReadyStatus();
            //populateStatus();
        }


        private void cmbQuarter_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateFilters();
            alStatusGlobal.Clear();
            //alStatusGlobal.Add(new Status("Quarter changed to " + cmbQuarter.Text, defaultColor, boldFont));
            //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
            addDefaultReadyStatus();
            //populateStatus();
        }


        private void cmbConfidence_SelectionChangeCommitted(object sender, EventArgs e)
        {
            alStatusGlobal.Clear();
            //alStatusGlobal.Add(new Status("Confidence changed to " + cmbConfidence.Text, defaultColor, boldFont));
            //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
            addDefaultReadyStatus();
            //populateStatus();
        }

        private void cmbSampleSize_SelectionChangeCommitted(object sender, EventArgs e)
        {

            alStatusGlobal.Clear();
            //alStatusGlobal.Add(new Status("SampleSize changed to " + cmbSampleSize.Text, defaultColor, boldFont));
            //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
            addDefaultReadyStatus();
            //populateStatus();

        }



        private void cmbSurgical_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (frmFiltersGlobal == null)
                frmFiltersGlobal = new frmComplianceReporting_Filters();


            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex > 0)
            {

                frmFiltersGlobal.dicSurgicalFiltersGlobal.Add(senderComboBox.SelectedValue.ToString(), senderComboBox.Text);
                //MessageBox.Show("{" + senderComboBox.Text + "} added to Surgical Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtStatus.AppendText("{" + senderComboBox.Text + "} added to surgical filters list" + Environment.NewLine);


                alStatusGlobal.Clear();
                alStatusGlobal.Add(new Status("Success", "{" + senderComboBox.Text + "} added to surgical filters list"));
                //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());
                //populateStatus();

                updateFilters("Surgical");
                refreshSurgicalFilters();
                cleanSurgicalFilters(true);
            }
        }

        private void cmbSpecialty_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (frmFiltersGlobal == null)
                frmFiltersGlobal = new frmComplianceReporting_Filters();

            string specValue = null;
            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex > 0)
            {
                specValue = senderComboBox.SelectedValue.ToString();

                frmFiltersGlobal.dicSpecialtyFiltersGlobal.Add(specValue, senderComboBox.Text);
                //MessageBox.Show("{" + senderComboBox.Text + "} added to Specialty Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtStatus.AppendText("{" + senderComboBox.Text + "} added to specialty filters list" + Environment.NewLine);


                alStatusGlobal.Clear();
                alStatusGlobal.Add(new Status("Success", "{" + senderComboBox.Text + "} added to specialty filters list"));
                //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());
                //populateStatus();

                updateFilters("Specialty");
                refreshSpecialtyFilters();
                cleanSpecialtyFilters(true);

            }
        }

        private void cmbMarket_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (frmFiltersGlobal == null)
                frmFiltersGlobal = new frmComplianceReporting_Filters();

            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex > 0)
            {
                frmFiltersGlobal.dicMarketFiltersGlobal.Add(senderComboBox.SelectedValue.ToString(), senderComboBox.Text);
                //MessageBox.Show("{" + senderComboBox.Text + "} added to Measure Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtStatus.AppendText("{" + senderComboBox.Text + "} added to market filters list" + Environment.NewLine);

                alStatusGlobal.Clear();
                alStatusGlobal.Add(new Status("Success", "{" + senderComboBox.Text + "} added to market filters list"));
                //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());
                //populateStatus();

                updateFilters("Market");
                refreshMarketFilters();
                cleanMarketFilters(true);

            }
        }

        private void cmbMeasure_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (frmFiltersGlobal == null)
                frmFiltersGlobal = new frmComplianceReporting_Filters();

            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex > 0)
            {
                frmFiltersGlobal.dicMeasureFiltersGlobal.Add(senderComboBox.SelectedValue.ToString(), senderComboBox.Text);
                //MessageBox.Show("{" + senderComboBox.Text + "} added to Measure Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtStatus.AppendText("{" + senderComboBox.Text + "} added to measure filters list" + Environment.NewLine);

                alStatusGlobal.Clear();
                alStatusGlobal.Add(new Status("Success", "{" + senderComboBox.Text + "} added to measure filters list"));
                //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
                ///populateStatus();
                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());

                refreshMeasureFilters();
                cleanMeasureFilters(true);

            }
        }

        private void cmbFacilityType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (frmFiltersGlobal == null)
                frmFiltersGlobal = new frmComplianceReporting_Filters();

            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex > 0)
            {
                frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Add(senderComboBox.SelectedValue.ToString(), senderComboBox.Text);
                // MessageBox.Show("{" + senderComboBox.Text + "} added to Quarter Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //txtStatus.AppendText("{" + senderComboBox.Text + "} added to facility type filters list" + Environment.NewLine);


                alStatusGlobal.Clear();
                alStatusGlobal.Add(new Status("Success", "{" + senderComboBox.Text + "} added to facility type filters list"));
                //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());
                //populateStatus();
                refreshFacilityTypeFilters();
                cleanFacilityTypeFilters(true);
            }
        }




        //private void cmbQuarter_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (frmFiltersGlobal == null)
        //        frmFiltersGlobal = new frmComplianceReporting_Filters();

        //    ComboBox senderComboBox = (ComboBox)sender;
        //    if (senderComboBox.SelectedIndex > 0)
        //    {
        //        frmFiltersGlobal.dicQuarterFiltersGlobal.Add(senderComboBox.SelectedValue.ToString(), senderComboBox.Text);
        //       // MessageBox.Show("{" + senderComboBox.Text + "} added to Quarter Filters List", "Filter Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        txtStatus.AppendText("{" + senderComboBox.Text + "} added to Quarter Filters List" + Environment.NewLine);
        //        refreshQuarterFilters();
        //        cleanQuarterFilters(true);
        //    }
        //}

        #endregion

        #region MENU STRIP FUNCTIONS
        private void chooseColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmColumnsGlobal == null)
                frmColumnsGlobal = new frmComplianceReportingColumns();

            //frmColumnsGlobal.populateColumns(radPractice.Checked);
            frmColumnsGlobal.ShowDialog();
        }

        private void defaultsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmColumnsGlobal == null)
                frmColumnsGlobal = new frmComplianceReportingColumns();

            frmColumnsGlobal.setDefaults();

            alStatusGlobal.Clear();
            alStatusGlobal.Add(new Status("Success", "Columns Reset"));
            //alStatusGlobal.Add(new Status("Ready...", defaultColor, txtStatus.Font));
            addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Do you want to reset all filers?", "Clear All Filters", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (dr == DialogResult.Yes)
            {
                radFacility.Checked = false;
                radPractice.Checked = true;

                frmFiltersGlobal.dicQuarterFiltersGlobal.Clear();
                frmFiltersGlobal.dicProviderFiltersGlobal.Clear();
                frmFiltersGlobal.dicSurgicalFiltersGlobal.Clear();
                frmFiltersGlobal.dicSpecialtyFiltersGlobal.Clear();
                frmFiltersGlobal.dicMeasureFiltersGlobal.Clear();
                frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Clear();
                frmFiltersGlobal.dicMarketFiltersGlobal.Clear();

                getFilterMaster();

                refreshQuarterFilters();
                refreshFacilityTypeFilters();
                refreshProviderFilters();
                refreshSpecialtyFilters();
                refreshSurgicalFilters();
                refreshMeasureFilters();
                refreshMarketFilters();

                cleanQuarterFilters(false);
                cleanFacilityTypeFilters(false);
                cleanProviderFilters(false);
                cleanSpecialtyFilters(false);
                cleanSurgicalFilters(false);
                cleanMeasureFilters(false);
                cleanMarketFilters(false);

                cmbLOB.SelectedIndex = 0;

                grpAdditionalFilters.Enabled = true;


                alStatusGlobal.Clear();
                ///alStatusGlobal.Add(new Status("{" + dr["provider_name"].ToString() + "} has been added to provider filters list.", defaultColor, txtStatus.Font));
                alStatusGlobal.Add(new Status("Success", "All filters cleared"));
                addDefaultReadyStatus((ArrayList)alStatusGlobal.Clone());

            }
        }

        private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var path = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents") + "\\compliance_reports\\";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Process.Start(path);

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion MENU STRIP FUNCTIONS

        #region DYNAMIC FILTERS

        DataTable dtAllFiltersMaster_GLOBAL; //ALL FILTER CACHE
        DataTable dtCurrentFilters_GLOBAL; //ALL FILTER WITH PROV CACHE
        DataTable dtLiveFilters_GLOBAL; //ACTIVE FILTERS
        private void getFilterMaster()
        {
            dtLiveFilters_GLOBAL = null; //RESET LIVE FILTERS


            string strSQL = "SELECT distinct f.lst_run_qrt ,f.lst_run_qrt_val ,f.surg_id ,f.surg_desc ,f.Spec_id ,f.Spec_Desc, f.lob_id, f.lob_desc, f.Market_Nbr ,f.Market_Name FROM [IL_UCA].[dbo].[compl_app_filtercache] f ";
            if (dtAllFiltersMaster_GLOBAL == null)
            {
                dtAllFiltersMaster_GLOBAL = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, strSQL );
            }


            if (frmFiltersGlobal.dicProviderFiltersGlobal.Count() > 0)
            {
                StringBuilder sbSQL = new StringBuilder();
                sbSQL.Append(strSQL);
                sbSQL.Append(" INNER JOIN dbo.compl_app a ON a.lst_run_qrt = f.lst_run_qrt AND a.surg_id = f.surg_id AND a.Spec_id = f.Spec_id AND a.UNET_MKT_NM = f.Market_Name AND a.lob_id = f.lob_id");
                if (radFacility.Checked)
                {
                    sbSQL.Append(" WHERE a.facl_TIN  in (" + string.Join(",", frmFiltersGlobal.dicProviderFiltersGlobal.Keys) + ")");

                    if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count() > 0)
                    {
                        var csv = String.Join(",", Array.ConvertAll(frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Keys.ToArray(), z => "'" + z + "'"));
                        sbSQL.Append(" AND a.fac_type in (" + csv + ")");
                    }
                }
                else
                {
                    sbSQL.Append(" WHERE a.claim_tin_num  in (" + string.Join(",", frmFiltersGlobal.dicProviderFiltersGlobal.Keys) + ")");
                }

                dtCurrentFilters_GLOBAL = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, sbSQL.ToString());
            }
            else
            {
                dtCurrentFilters_GLOBAL = dtAllFiltersMaster_GLOBAL;
            }
                

        }


        private void btnGetFilters_Click(object sender, EventArgs e)
        {
            clearAllFilters();

            getFilterMaster();

            refreshQuarterFilters();
            refreshSurgicalFilters();
            refreshSpecialtyFilters();
            refreshMarketFilters();
            refreshLOBFilters();

            cleanQuarterFilters(false);
            cleanSurgicalFilters(false);
            cleanSpecialtyFilters(false);
            cleanMarketFilters(false);

            //txtStatus.AppendText("Additonal filters have been refreshed" + Environment.NewLine);
            addDefaultReadyStatus();

            btnGetFilters.ForeColor = Color.Black;
            btnGetFilters.BackColor = btnGenerateReport.BackColor;

            grpAdditionalFilters.Enabled = true;
        }

        string strCurrentFilterGLOBAL = null;
        private void updateFilters(string strCurrentFilter = "None")
        {

            if(dtCurrentFilters_GLOBAL.Rows.Count == 0)
            {
                getFilterMaster();
            }


            StringBuilder sbFilters = new StringBuilder();

            if(cmbQuarter.SelectedValue != null)
            sbFilters.Append("lst_run_qrt_val = '" + cmbQuarter.SelectedValue + "' AND ");

            if (cmbLOB.SelectedIndex > 0 )
                sbFilters.Append("lob_id = '" + cmbLOB.SelectedValue + "' AND ");

            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count() > 0 )
                sbFilters.Append("Market_Nbr in (" + string.Join(",", frmFiltersGlobal.dicMarketFiltersGlobal.Keys) + ") AND ");

            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count() > 0 )//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
                sbFilters.Append("Spec_id in (" + string.Join(",", frmFiltersGlobal.dicSpecialtyFiltersGlobal.Keys) + ") AND ");


            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count() > 0)//IF ALREADY FILTERED, LETS LIMIT THOSE ITEMS FROM DISPLAY...
                sbFilters.Append("surg_id in (" + string.Join(",", frmFiltersGlobal.dicSurgicalFiltersGlobal.Keys) + ") AND ");


            dtLiveFilters_GLOBAL = dtCurrentFilters_GLOBAL.Select(sbFilters.ToString().Trim().TrimEnd('A','N', 'D')).CopyToDataTable();

            ///????????????????????????????????????????????????????????????????
            ///????????????????????????????????????????????????????????????????
            ///????????????????????????????????????????????????????????????????
            if (strCurrentFilter != "LOB")
                refreshLOBFilters();

            if (strCurrentFilter != "Market")
                refreshMarketFilters();

            if (strCurrentFilter != "Specialty")
                refreshSpecialtyFilters();

            if (strCurrentFilter != "Surgical")
                refreshSurgicalFilters();


            strCurrentFilterGLOBAL = strCurrentFilter;

        }
        #endregion

        #region ERROR HANDLING
        private void cleanupError(string strMessage)
        {
            try
            {
                DispatchHandler handler4 = delegate ()
                {
                    //txtStatus.Text = "";
                    //txtStatus.AppendText("ERROR:" + Environment.NewLine);
                    //txtStatus.AppendText(strMessage + Environment.NewLine);

                    //alStatusGlobal.Clear();
                    //alStatusGlobal.Add(new Status("Error", "ERROR"));
                    //alStatusGlobal.Add(new Status("Error", strMessage));
                    //populateStatus();

               
                    txtStatus.Clear();
                    txtStatus.AppendLog("ERROR" + Environment.NewLine, Color.DarkRed, new Font(txtStatus.Font, FontStyle.Bold));
                    txtStatus.AppendLog(strMessage, Color.DarkRed, new Font(txtStatus.Font, FontStyle.Bold));

                    //CLEANUP REPORTING VARIABLES JUST IN CASE!!!!!
                    this.UseWaitCursor = false;
                    btnGenerateReport.Text = "Generate Report";
                };
                this.BeginInvoke(handler4);
            }
            catch (InvalidOperationException)
            {
                //txtStatus.Text = "";
                //txtStatus.AppendText("ERROR:" + Environment.NewLine);
                //txtStatus.AppendText(strMessage + Environment.NewLine);


                //alStatusGlobal.Clear();
                //alStatusGlobal.Add(new Status("Error", "ERROR"));
                //alStatusGlobal.Add(new Status("Error", strMessage));
                //populateStatus();

                txtStatus.Clear();
                txtStatus.AppendLog("ERROR" + Environment.NewLine, Color.DarkRed, new Font(txtStatus.Font, FontStyle.Bold));
                txtStatus.AppendLog(strMessage, Color.DarkRed, new Font(txtStatus.Font, FontStyle.Bold));

                //CLEANUP REPORTING VARIABLES JUST IN CASE!!!!!
                this.UseWaitCursor = false;
                btnGenerateReport.Text = "Generate Report";
            }


            blFirstRowGLOBAL = false;
            blCancelReportGLOBAL = false;
            blStopSmartSearch = false;
        }


        #endregion

        #region RANDOM EVENTS  

        private void cmb_KeyPressDisable(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }


        private void radProvider_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton; 
            if (rb != null)
            {
                if (!rb.Checked)
                    return;

                if (rb.Name == "radFacility")
                {
                    grpFacilityType.Enabled = true;
                    grpProviders.Text = "Facility:";
                    cmbMarket.Enabled = false;
                }
                else
                {
                    grpFacilityType.Enabled = false;
                    grpProviders.Text = "Practice:";
                    cmbMarket.Enabled = true;

                }
                frmColumnsGlobal.populateColumns(radPractice.Checked, false);
                frmFiltersGlobal.dicProviderFiltersGlobal.Clear();
                frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Clear();

                cmbProvider.DataSource = null;
                cmbProvider.Items.Clear();
                cmbProvider.ResetText();
                cmbProvider.SelectedIndex = -1;
                cmbProvider.Text = "";
                //cmbProvider.SelectionStart = 0;
                //cmbProvider.SelectionLength = 0;


                refreshFacilityTypeFilters();
                cleanProviderFilters(false);
                cleanFacilityTypeFilters(false);
                grpAdditionalFilters.Enabled = false;

                btnGetFilters.ForeColor = Color.White;
                btnGetFilters.BackColor = alertColorGlobal;

                populateStatus();
            }
        }

        //VISUAL STUDIO BUG MUST MANUALLY DESELECT ALL COMBOS :(
        private void frmComplianceReporting_Shown(object sender, EventArgs e)
        {
            cmbSampleSize.SelectionLength = 0;
            cmbConfidence.SelectionLength = 0;
            cmbSurgical.SelectionLength = 0;
            cmbSpecialty.SelectionLength = 0;
            cmbMeasure.SelectionLength = 0;
            cmbMarket.SelectionLength = 0;
        }

        private void frmComplianceReporting_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.WindowState = FormWindowState.Normal;
            this.Focus(); this.Show();
        }

        #endregion



        #region SQL SETUP SCRIPTS
        /****** TEST TINS
         
            
            1.INSERT/SELECT INNA_NEW_COMP_QUARTER
            /*
             *  DELETE FROM [IL_UCA].[dbo].[compl_app] where lst_run_qrt = 'XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX'
             * 
             * INSERT INTO [dbo].[compl_app] ([PEG_EPSD_NBR] ,[surg_id] ,[INDV_SYS_ID] ,[lob_id] ,[PROV_MPIN] ,[Prov_Name] ,[UNET_MKT_NBR] ,[UNET_MKT_NM] ,[TOT_PEG_ALLW_AMT] ,[MGT_ALLW_AMT] ,[SURG_ALLW_AMT] ,[PHRM_ALLW_AMT] ,[FACL_ALLW_AMT] ,[IPTNT_ALLW_AMT] ,[OPTNT_ALLW_AMT] ,[PEG_ANCH_DT] ,[PEG_ANCH_YR] ,[SVRTY_LVL] ,[SVRTY_SCOR] ,[PROC_CD] ,[PROC_DESC] ,[DIAG1] ,[DX1] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[QLTY_PEG_ANCH_CATGY_SYS_ID] ,[ETG_DESC] ,[claim_tin] ,[AMA_PL_OF_SRVC_CD] ,[AMA_PL_OF_SRVC_DESC] ,[HCCC_CD] ,[HCCC_DESC] ,[SRVC_LOC] ,[Svrty] ,[SEPSIS] ,[PNEUM] ,[UTI] ,[CLINFC] ,[PE] ,[AIREMB] ,[DVT] ,[MI] ,[ARF] ,[PULM] ,[FB] ,[HEM] ,[IA] ,[PERF] ,[UlC] ,[CVA] ,[SSI] ,[TRANS] ,[WOUND] ,[CMPLCTN_IND] ,[adm_idx] ,[adm_QLTY_PEG_EPSD_NBR] ,[admits_num] ,[min_anch_adm_diff] ,[Total_adm_allw] ,[adm_match_type] ,[ed_idx] ,[ed_QLTY_PEG_EPSD_NBR] ,[eds_num] ,[min_anch_ed_diff] ,[ed_match_type] ,[com_adm] ,[adm] ,[ed] ,[com] ,[Spec_id] ,[fac_type] ,[facl_TIN] ,[Fac_TIN_Name] ,[Pract_Name] ,[claim_tin_num] ,[exp_com_adm] ,[exp_adm] ,[exp_ed] ,[exp_com] ,[lst_run_qrt] ,[pd_status]) SELECT [PEG_EPSD_NBR] ,[surg_id] ,[INDV_SYS_ID] ,[lob_id] ,[PROV_MPIN] ,[Prov_Name] ,[UNET_MKT_NBR] ,[UNET_MKT_NM] ,[TOT_PEG_ALLW_AMT] ,[MGT_ALLW_AMT] ,[SURG_ALLW_AMT] ,[FACL_ALLW_AMT] ,[PHRM_ALLW_AMT] ,[IPTNT_ALLW_AMT] ,[OPTNT_ALLW_AMT] ,[PEG_ANCH_DT] ,[PEG_ANCH_YR] ,[SVRTY_LVL] ,[SVRTY_SCOR] ,[PROC_CD] ,[PROC_DESC] ,[DIAG1] ,[DX1] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[QLTY_PEG_ANCH_CATGY_SYS_ID] ,[ETG_DESC] ,[claim_tin] ,[AMA_PL_OF_SRVC_CD] ,[AMA_PL_OF_SRVC_DESC] ,[HCCC_CD] ,[HCCC_DESC] ,[SRVC_LOC] ,[Svrty] ,[SEPSIS] ,[PNEUM] ,[UTI] ,[CLINFC] ,[PE] ,[AIREMB] ,[DVT] ,[MI] ,[ARF] ,[PULM] ,[FB] ,[HEM] ,[IA] ,[PERF] ,[UlC] ,[CVA] ,[SSI] ,[TRANS] ,[WOUND] ,[CMPLCTN_IND] ,[adm_idx] ,[adm_QLTY_PEG_EPSD_NBR] ,[admits_num] ,[min_anch_adm_diff] ,[Total_adm_allw] ,[adm_match_type] ,[ed_idx] ,[ed_QLTY_PEG_EPSD_NBR] ,[eds_num] ,[min_anch_ed_diff] ,[ed_match_type] ,[com_adm] ,[adm] ,[ed] ,[com] ,[Spec_id] ,[fac_type] ,[facl_TIN] ,[Fac_TIN_Name] ,[Pract_Name] ,[claim_tin_num] ,[exp_com_adm] ,[exp_adm] ,[exp_ed] ,[exp_com] ,[lst_run_qrt] ,[pd_status] FROM [IL_UCA].[dbo].[compl_app_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX]
             * /

            2. AFTER INSERT/SELECT INNA_NEW_COMP_QUARTER EACH QUARTER:
            --to create table with 1 mktname pet practice tin using the most frequent market name in the last run quarter data

            select m.* into #upd_mkt
            from
                   (select claim_tin_num,UNET_MKT_NM,
                   row_number() over(partition by claim_tin_num order by count(*) desc) as mkt_order
                   from dbo.compl_app group by claim_tin_num,UNET_MKT_NM) as m
            inner join
                   (select claim_tin_num,lst_run_qrt,
                   row_number() over(partition by claim_tin_num order by lst_run_qrt desc) as qtr_seq
                   from dbo.compl_app group by claim_tin_num,lst_run_qrt) as q
            on m.claim_tin_num=q.claim_tin_num
            where mkt_order=1 and qtr_seq=1

           update a
           set a.UNET_MKT_NM=b.UNET_MKT_NM 
           --select count(*)
           from dbo.compl_app as a--5,184,726
           left join #upd_mkt as b on a.claim_tin_num=b.claim_tin_num

            
           3. TRUNCATE TABLE compl_app_provcache;
            INSERT INTO 
            [dbo].[compl_app_provcache]
            ([tin],[tin_num],[provider_name],[provider_type],[fac_type])
            SELECT [claim_tin] ,[claim_tin_num],[Pract_Name], 'Practice', NULL FROM [dbo].[compl_app]
            WHERE [Pract_Name] is not null and [Pract_Name] <> ''
            UNION
            SELECT CAST([facl_TIN] as varchar(20)) ,[facl_TIN],[Fac_TIN_Name], 'Facility', fac_type FROM [dbo].[compl_app]
            WHERE [Fac_TIN_Name] is not null and [Fac_TIN_Name] <> '';


            
            4. TRUNCATE TABLE dbo.compl_app_filtercache;
            INSERT INTO dbo.compl_app_filtercache (lst_run_qrt, lst_run_qrt_val,surg_id, surg_desc, Spec_id, Spec_Desc, lob_id, lob_desc, Market_Nbr, Market_Name)
            select CAST(lst_run_qrt as varchar(23)) as lst_run_qrt, CAST(lst_run_qrt as varchar(23)) as lst_run_qrt_val,sr.surg_id, sr.surg_desc, sp.Spec_id, sp.Spec_Desc, a.lob_id, CASE WHEN a.lob_id = 1 THEN 'COMMERCIAL' ELSE CASE WHEN a.lob_id = 2 THEN 'MEDICARE' ELSE 'MEDICARE' END END as lob_desc, DENSE_RANK() OVER ( ORDER BY LTRIM(RTRIM(UNET_MKT_NM))) as Market_Nbr, LTRIM(RTRIM(a.UNET_MKT_NM)) as Market_Name from dbo.compl_app a INNER JOIN dbo.dim_peg_spec sr ON sr.surg_id = a.surg_id INNER JOIN dbo.PBP_dim_Spec sp ON sp.Spec_id = a.Spec_id WHERE  a.UNET_MKT_NM IS NOT NULL GROUP BY a.lob_id, a.lst_run_qrt,sr.surg_id, sr.surg_desc, sp.Spec_id, sp.Spec_Desc, a.UNET_MKT_NM


      






         
             SELECT[claim_tin],[Pract_Name], cnt FROM( SELECT [claim_tin], [Pract_Name], count(*) cnt FROM[IL_UCA].[dbo].[compl_app] where [Pract_Name] is not null group by [claim_tin], [Pract_Name] ) tmp order by cnt desc

             claim_tin	Pract_Name	cnt
            453023019	NORTH SHORE LIJ MEDICAL	5837
            391678306	AURORA MEDICAL GROUP	5582
            135562308	NEW YORK UNIVERSITY	4859
            542129332	VITALMD GROUP HOLDINGS	3796
            340714585	CLEVELAND CLINIC FOUNDATION	3510
            611661781	FACULTY PRACTICE ASSOCIATES	3507
            260609255	FLORIDA WOMAN CARE	3416
            752613493	TEXAS HEALTH PHYSICIANS GROUP	3365
            391595302	AURORA ADVANCED HEALTHCARE	3346
            941156581	PALO ALTO MEDICAL FOUNDATION	2696
            300520570	TMH PHYSICIAN ASSOCIATES	2682
         ******/
        #endregion

        #region POPULATE STATUS FUNCTIONS

        ArrayList alStatusGlobal;

        //Color defaultColorGlobal = Color.Black;
        Color alertColorGlobal = Color.DarkMagenta;
        //Color successColorGlobal = Color.DarkGreen;
        //Font boldFont;

        private void populateStatus(bool blStatus = true)
        {
            if (alStatusGlobal == null)
                alStatusGlobal = new ArrayList();

            //if (blReportRunningGlobal)
            //{
            //    DispatchHandler handler0123 = delegate ()
            //    {
            //        if (blReportRunningGlobal)
            //        {
            //            statusUpdate();
            //        }

            //    };
            //  this.BeginInvoke(handler0123);
            //}
            //else
            //    statusUpdate();

            statusUpdate(blStatus);


            //try
            //{
            //    DispatchHandler handler0123 = delegate ()
            //    {
            //        statusUpdate();
            //        return;
            //    };
            //    this.BeginInvoke(handler0123);
            //}
            //catch (InvalidOperationException)
            //{
            //    statusUpdate();
            //}


        }

        Color headerColorGlobal = Color.Black;
        Color statusDefaultColorGlobal = Color.Black;
        Font normalFontGlobal = null;


        Font boldFontGlobal = null;
        private void statusUpdate(bool blStatus)
        {
            normalFontGlobal = txtStatus.Font;
            boldFontGlobal = new Font(normalFontGlobal, FontStyle.Bold);

            Color currentColor = Color.Empty;
            Font currentFont = null;

            if(blStatus)
            {
                txtStatus.Clear();
                //**************************************Status****************************************
                //**************************************Status****************************************
                //**************************************Status****************************************
                txtStatus.AppendLog("**************************************Status********************************************", headerColorGlobal, normalFontGlobal);
                var varStatus = from Status s in alStatusGlobal select s;
                foreach (Status st in varStatus.ToList())
                {
                    switch (st.strType)
                    {
                        case "Status":
                            currentColor = statusDefaultColorGlobal;
                            currentFont = normalFontGlobal;
                            break;
                        case "Success":
                            currentColor = Color.DarkGreen;
                            currentFont = boldFontGlobal;
                            break;
                        case "Instruction":
                            currentColor = Color.DarkMagenta;
                            currentFont = boldFontGlobal;
                            break;
                        case "Error":
                            currentColor = Color.DarkRed;
                            currentFont = boldFontGlobal;
                            break;
                        default:
                            currentColor = statusDefaultColorGlobal;
                            currentFont = normalFontGlobal;
                            break;
                    }

                    if (st.fntColorOverride != null)
                        currentColor = (Color)st.fntColorOverride;

                    if (st.fntStyleOverride != null)
                        currentFont = st.fntStyleOverride;

                    txtStatus.AppendLog(Environment.NewLine + st.strMessage, currentColor, currentFont);
                }
            }
            



            //**************************************Parameters***************************************
            //**************************************Parameters***************************************
            //**************************************Parameters***************************************
            txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "**************************************Parameters***************************************", headerColorGlobal, normalFontGlobal);
            bool blHasMeasures = false;
            txtStatus.AppendLog(Environment.NewLine + "Columns:" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
            foreach (Object cb in frmColumnsGlobal.clbColumns.CheckedItems)
            {
                if (cb.ToString().Equals("Measures"))
                    blHasMeasures = true;
                txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                txtStatus.AppendLog(cb.ToString(), statusDefaultColorGlobal, normalFontGlobal);
                txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);
            }

            if (blHasMeasures)
            {
                txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Measures:" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
                if (frmFiltersGlobal.dicMeasureFiltersGlobal.Count > 0)
                {
                    foreach (var item in frmFiltersGlobal.dicMeasureFiltersGlobal)
                    {

                        txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                        txtStatus.AppendLog(item.Value, statusDefaultColorGlobal, normalFontGlobal);
                        txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                        txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);
                    }
                }
                else
                {
                    txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog("~All Measures~", statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                }


                //txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Selected Confidence Interval:" + Environment.NewLine, blackColor, bold);
                //txtStatus.AppendLog("[", blackColor, bold);
                //txtStatus.AppendLog(cmbConfidence.Text, blackColor, normal);
                //txtStatus.AppendLog("]", blackColor, bold);

                //txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Selected Min Sample:" + Environment.NewLine, blackColor, bold);
                //txtStatus.AppendLog("[", blackColor, bold);
                //txtStatus.AppendLog( cmbSampleSize.SelectedValue.ToString(), blackColor, normal);
                //txtStatus.AppendLog("]", blackColor, bold);
            }



            txtStatus.AppendLog(Environment.NewLine + Environment.NewLine +  (radFacility.Checked ? "Facilities" : "Practices") + ":" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
            if (frmFiltersGlobal.dicProviderFiltersGlobal.Count > 0)
            {
                foreach (var item in frmFiltersGlobal.dicProviderFiltersGlobal)
                {
                    txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(item.Value + " - " + item.Key, statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);

                }
            }
            else
            {
                txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                txtStatus.AppendLog("~All " + (radFacility.Checked ? "Facilities" : "Practices") + "~", statusDefaultColorGlobal, normalFontGlobal);
                txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
            }

            if (radFacility.Checked)
            {
                txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Facility Types:" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
                if (frmFiltersGlobal.dicFacilityTypeFiltersGlobal.Count > 0)
                {
                    foreach (var item in frmFiltersGlobal.dicFacilityTypeFiltersGlobal)
                    {
                        txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                        txtStatus.AppendLog(item.Value, statusDefaultColorGlobal, normalFontGlobal);
                        txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                        txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);
                    }
                }
                else
                {
                    txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog("~All Facility Types~", statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                }
            }


            //txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Selected Quarter:" + Environment.NewLine, blackColor, bold);
            //txtStatus.AppendLog("[", blackColor, bold);
            ////txtStatus.AppendLog(cmbQuarter.SelectedValue.ToString(), blackColor, normal);
            //txtStatus.AppendLog(cmbQuarter.Text, blackColor, normal);
            //txtStatus.AppendLog("]", blackColor, bold);

            //txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Selected LOB:" + Environment.NewLine, blackColor, bold);
            //txtStatus.AppendLog("[", blackColor, bold);
            //txtStatus.AppendLog( cmbLOB.Text, blackColor, normal);
            //txtStatus.AppendLog("]", blackColor, bold);



            txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Specialties:" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
            if (frmFiltersGlobal.dicSpecialtyFiltersGlobal.Count > 0)
            {
                foreach (var item in frmFiltersGlobal.dicSpecialtyFiltersGlobal)
                {
                    txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(item.Value, statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);
                }
            }
            else
            {
                txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                txtStatus.AppendLog("~All Specialties~", statusDefaultColorGlobal, normalFontGlobal);
                txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
            }

            txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Surgeries:" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
            if (frmFiltersGlobal.dicSurgicalFiltersGlobal.Count > 0)
            {
                foreach (var item in frmFiltersGlobal.dicSurgicalFiltersGlobal)
                {
                    txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(item.Value, statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);
                }
            }
            else
            {
                txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                txtStatus.AppendLog("~All Surgeries~", statusDefaultColorGlobal, normalFontGlobal);
                txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
            }

            txtStatus.AppendLog(Environment.NewLine + Environment.NewLine + "Markets:" + Environment.NewLine, statusDefaultColorGlobal, boldFontGlobal);
            if (frmFiltersGlobal.dicMarketFiltersGlobal.Count > 0)
            {
                foreach (var item in frmFiltersGlobal.dicMarketFiltersGlobal)
                {
                    txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(item.Value, statusDefaultColorGlobal, normalFontGlobal);
                    txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
                    txtStatus.AppendLog(" ", statusDefaultColorGlobal, normalFontGlobal);
                }
            }
            else
            {
                txtStatus.AppendLog("[", statusDefaultColorGlobal, boldFontGlobal);
                txtStatus.AppendLog("~All Markets~", statusDefaultColorGlobal, normalFontGlobal);
                txtStatus.AppendLog("]", statusDefaultColorGlobal, boldFontGlobal);
            }

            //SCROLL TO TOP FOR CONSTANT STATUS INSTRUCTIONS
            txtStatus.SelectionStart = 0;
            txtStatus.SelectionLength = txtStatus.Text.Length;
            txtStatus.DeselectAll();
            txtStatus.ScrollToCaret();

        }



        private void addDefaultReadyStatus(ArrayList alStart = null)
        {
            if(alStatusGlobal == null)
                alStatusGlobal = new ArrayList();

            alStatusGlobal.Clear();
            if (alStart != null)
            {
                alStatusGlobal = alStart;
            }
           

            //alStatusGlobal.Add(new Status("Choose any Measure Options, Provider or additional filters and then click 'Generate Report' button", Color.Black, boldFont));
            alStatusGlobal.Add(new Status("Status", "Ready..."));
            populateStatus();
        }

        #endregion

    }

    #region
    public struct Status
    {
        public Status(string sType, string sMessage, Color? fColorOverride = null, Font fStyleOverride = null)
        {
            strType = sType;
            strMessage = sMessage;
            fntColorOverride = fColorOverride;
            fntStyleOverride = fStyleOverride;
        }

        public string strType { get; }
        public string strMessage { get; }
        public Color? fntColorOverride { get; }
        public Font fntStyleOverride { get; }

    }


    public class CleanProviderPaste : NativeWindow
    {
        protected override void WndProc(ref Message m)
        {

            if (m.Msg == (0x0302))
            {
                string srtCurrentClipboad = Clipboard.GetText();
                string srtNewClipboad = "";
                if (srtCurrentClipboad.Contains(","))
                {
                    srtNewClipboad = srtCurrentClipboad.Replace("\r", "").Replace("\n", "").Trim().TrimEnd(',');
                }
                else if(srtCurrentClipboad.Contains('\r') && srtCurrentClipboad.Contains('\n'))
                {
                    srtNewClipboad = srtCurrentClipboad.Replace("\r", ",").Replace("\n", "").Trim().TrimEnd(',');
                }
                else if (srtCurrentClipboad.Contains('\r'))
                {
                    srtNewClipboad = srtCurrentClipboad.Replace("\r", ",").Trim().TrimEnd(',');
                }
                else if (srtCurrentClipboad.Contains('\n'))
                {
                    srtNewClipboad = srtCurrentClipboad.Replace("\n", ",").Trim().TrimEnd(',');
                }
                else
                {
                    srtNewClipboad = srtCurrentClipboad;
                }

                Clipboard.SetText(srtNewClipboad);
                //return;
            }
            base.WndProc(ref m);

        }

    }




    #endregion

}
