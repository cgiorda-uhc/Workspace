using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Teradata.Client.Provider;
using System.Reflection;
using System.Drawing;


namespace PhysicianFeedbackTracker
{
    public partial class frmAPRDRG : _BaseClass
    {

        private const int CP_NOCLOSE_BUTTON = 0x200;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }




        private Cache memoryCache = null;
        private string strSortCol = "APR_DRG_SYS_ID";
        private string strSortOrder = "ASC";

        string strMessageGlobal = null;
        bool blResizedGrid_GLOBAL = false;
        // DataTable dtAPRDRG_GLOBAL = null;
        public frmAPRDRG()
        {
            InitializeComponent();


            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                this.dgvAPRDRG,
                new object[] { true });

            dgvAPRDRG.VirtualMode = true;
            //dgvAPRDRG.ReadOnly = true;
            dgvAPRDRG.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAPRDRG.CellValueNeeded += new DataGridViewCellValueEventHandler(dgvAPRDRG_CellValueNeeded);
            dgvAPRDRG.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dgvAPRDRG_ColumnHeaderMouseClick);
            dgvAPRDRG.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgvAPRDRG_RowPostPaint);
            dgvAPRDRG.CellValueChanged += new DataGridViewCellEventHandler(dgvAPRDRG_CellValueChanged);

            Resetsort();
            LoadData(strSortCol, strSortOrder);


        }



        private void LoadData(string strSortCol, string strSortOrder)
        {
            dgvAPRDRG.Rows.Clear();
            dgvAPRDRG.Refresh();
            dgvAPRDRG.Columns.Clear();
            dgvAPRDRG.DataSource = null;

            try
            {

                var lstOptions = new List<string>() { "", "Y", "U" };

                DataGridViewComboBoxColumn colCbx = null;
                DataGridViewColumn col = null;
                DataRetriever retriever = new DataRetriever(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAAPRDRGSQL(), strSortCol, strSortOrder);
                memoryCache = new Cache(retriever, 16);

                foreach (DataColumn column in retriever.Columns)
                {



                    if (column.ColumnName == "Sens" || column.ColumnName == "Sens_OB")
                    {
                        colCbx = new DataGridViewComboBoxColumn();
                        colCbx.DataSource = lstOptions;
                        colCbx.Name = column.ColumnName;
                        colCbx.HeaderText = column.ColumnName;
                        colCbx.ReadOnly = false;
                        dgvAPRDRG.Columns.Add(colCbx);

                    }
                    else
                    {
                        col = new DataGridViewColumn();
                        col.Name = column.ColumnName;
                        col.HeaderText = column.ColumnName;
                        col.CellTemplate = new DataGridViewTextBoxCell();
                        col.SortMode = DataGridViewColumnSortMode.Programmatic;
                        col.ReadOnly = true;
                        if (column.ColumnName == "APR_DRG_DESC_lc")
                            col.ReadOnly = false;
               



                            dgvAPRDRG.Columns.Add(col);
                    }


                }
                this.dgvAPRDRG.RowCount = retriever.RowCount;
                if (!blResizedGrid_GLOBAL)
                {
                    this.dgvAPRDRG.AutoResizeColumns();
                    this.dgvAPRDRG.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    blResizedGrid_GLOBAL = true;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Load Grid Error", MessageBoxButtons.OK);
                Application.Exit();
            }

            // Adjust the column widths based on the displayed values. 
            this.dgvAPRDRG.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            if (strSortOrder == "ASC")
                this.dgvAPRDRG.Columns[strSortCol].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
            else
                this.dgvAPRDRG.Columns[strSortCol].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;

        }
        private void Resetsort()
        {
            strSortCol = "APR_DRG_SYS_ID";
            strSortOrder = "ASC";
        }

        //bool blEdit = false;
        private void dgvAPRDRG_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            //DONT ALLOW ANY ACTION WHILE PROGRESS BAR RUNS!!!!!
            if (tssProgressBar.Visible == true)
                return;


            // blEdit = true;
            ////do your checks to see RowIndex is not -1 and other good stuffs
            var row = dgvAPRDRG.Rows[e.RowIndex];
            var changedValue = (string)row.Cells[e.ColumnIndex].EditedFormattedValue;
            memoryCache.cachePages[0].table.Rows[e.RowIndex][e.ColumnIndex] = changedValue;
            saveChanges(memoryCache.cachePages[0].table.Rows[e.RowIndex]);
        }



        private void dgvAPRDRG_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
            //this.Cursor = Cursors.Default;

        }

        private void dgvAPRDRG_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            // this routine will add row no to HeaderCell which comes left most
            var grid = sender as DataGridView;
            var rowIAPRDRG = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIAPRDRG, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dgvAPRDRG_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (strSortCol.Trim().ToUpper() != this.dgvAPRDRG.Columns[e.ColumnIndex].Name.Trim().ToUpper())
            {
                strSortOrder = "ASC";
            }
            else
            {
                strSortOrder = (this.dgvAPRDRG.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending ? "DESC" : "ASC");
            }
            strSortCol = this.dgvAPRDRG.Columns[e.ColumnIndex].Name;
            LoadData(strSortCol, strSortOrder);
        }


        private void saveChanges(DataRow dr)
        {

            try
            {
                tssStatus.Text = "Saving your updates to ILUCA.PBP_APRDRG..."; statusStrip1.Refresh();
                this.Cursor = Cursors.WaitCursor;
                string strTickMark = null;
                string strUpdateSQL = GlobalObjects.getILUCAAPRDRGUpdateSQL();
                foreach (DataColumn dc in dr.Table.Columns)
                {

                    if (dc.DataType == System.Type.GetType("System.String") || dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.Date"))
                        strTickMark = "'";
                    else
                        strTickMark = "";

                    strUpdateSQL = strUpdateSQL.Replace("{$" + dc.ColumnName + "}", (dr[dc.ColumnName] != DBNull.Value && dr[dc.ColumnName] + "" != "" ? strTickMark + dr[dc.ColumnName].ToString().Replace("'", "''") + strTickMark : "NULL"));
                }


                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, strUpdateSQL);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK);
            }
            finally
            {

                tssStatus.Text = "Ready"; statusStrip1.Refresh();
            }
        }


        private void btnUGAPAPRDRG_Click(object sender, EventArgs e)
        {

            //DONT ALLOW ANY ACTION WHILE PROGRESS BAR RUNS!!!!!
            if (tssProgressBar.Visible == true)
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                //PREP
                tssStatus.Text = "Preparing to retreive data from uhcdm001.diagnosis_Code..."; statusStrip1.Refresh();
                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "TRUNCATE TABLE CG_DEV_TMP_PBP_APRDRG_LANDING;");
                //INSERT LANDING
                var vrResultCnt = DBConnection.getTeraDataExecuteScalar(GlobalObjects.strUGAPConnectionString, "SELECT COUNT(*) as total FROM (" + GlobalObjects.getUGAPAPRDRGSQL() + ") tmp ");
                strMessageGlobal = "Retrieving {$rowCnt} out of " + String.Format("{0:n0}", vrResultCnt) + " records from uhcdm001.APR_DRG_BASE_CLASS";
                tssProgressBar.Visible = true;
                tssProgressBar.Maximum = (int)vrResultCnt;
                tssProgressBar.Visible = true;

                DBConnection.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection.SQLServerBulkImport(GlobalObjects.strUGAPConnectionString, GlobalObjects.strILUCAConnectionString, GlobalObjects.getUGAPAPRDRGSQL(), "CG_DEV_TMP_PBP_APRDRG_LANDING", 500);

                tssProgressBar.Value = 0;
                tssProgressBar.Visible = false;
                //UPDATE MAIN
                tssStatus.Text = "Loading new records into ILUCA.PBP_APRDRG..."; statusStrip1.Refresh();
                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "INSERT INTO [dbo].[CG_DEV_TMP_PBP_APRDRG] ([APR_DRG_SYS_ID] ,[APR_DRG_CD] ,[APR_DRG_DESC] ,[DSES_ST_NM] ,[APR_DRG_DESC_lc],[LOAD_DT],[UPDT_DT]) SELECT [APR_DRG_SYS_ID] ,[APR_DRG_CD] ,[APR_DRG_DESC] ,[DSES_ST_NM] ,[APR_DRG_DESC] as APR_DRG_DESC_lc ,[LOAD_DT] ,[UPDT_DT] FROM [dbo].[CG_DEV_TMP_PBP_APRDRG_LANDING] o WHERE not exists ( select * from CG_DEV_TMP_PBP_APRDRG i where o.APR_DRG_SYS_ID = i.APR_DRG_SYS_ID);");

                //REFRESH
                tssStatus.Text = "Refreshing data..."; statusStrip1.Refresh();

                //dtAPRDRG_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAAPRDRGSQL());

                Resetsort();
                LoadData(strSortCol, strSortOrder);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK);
            }
            finally
            {
                tssStatus.Text = "Ready"; statusStrip1.Refresh();
                tssProgressBar.Visible = false;
            }


        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //DONT ALLOW ANY ACTION WHILE PROGRESS BAR RUNS!!!!!
            if (tssProgressBar.Visible == true)
                return;

            Close();
        }

        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            tssStatus.Text = strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)); statusStrip1.Refresh();
            if (tssProgressBar.Value < tssProgressBar.Maximum)
                tssProgressBar.Value += 1;
            Application.DoEvents();
        }

        private void dgvAPRDRG_MouseHover(object sender, EventArgs e)
        {
            if (tssStatus.Text == "Ready")
            {
                dgvAPRDRG.ScrollBars = ScrollBars.Both;
                this.Cursor = Cursors.Default;
            }
            else
            {
                dgvAPRDRG.ScrollBars = ScrollBars.None;
                this.Cursor = Cursors.WaitCursor;
            }


            Application.DoEvents();
        }










        //string strMessageGlobal = null;
        //bool blSomenthingTouched_GLOBAL = false;
        //DataTable dtAPRDRG_GLOBAL = null;
        //public frmAPRDRG()
        //{
        //    InitializeComponent();
        //    dtAPRDRG_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAAPRDRGSQL());
        //    dgvAPRDRG.CellValueNeeded += new DataGridViewCellValueEventHandler(dgvAPRDRG_CellValueNeeded);
        //    dgvAPRDRG.CellEndEdit += dgvAPRDRG_CellEndEdit;
        //    dgvAPRDRG.CellValueChanged += dgvAPRDRG_CellValueChanged;
        //    dgvAPRDRG.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dgvAPRDRG_ColumnHeaderMouseClick);
        //    dgvAPRDRG.VirtualMode = true;
        //    populateDetails();
        //}








        //private void populateDetails()
        //{
        //    blSomenthingTouched_GLOBAL = false;
        //    // SharedWinFormFunctions.removeSortGlyphsFromDataGridView(ref dgvTrackingItems);
        //    dgvAPRDRG.Rows.Clear();
        //    if (dgvAPRDRG.Columns.Count == 0)
        //    {
        //        var lstOptions = new List<string>() { "", "Y", "U" };

        //        DataGridViewComboBoxColumn colCbx = null;
        //        DataGridViewColumn col = null;
        //        for (int cnt = 0; cnt < dtAPRDRG_GLOBAL.Columns.Count; cnt++)
        //        {

        //            if (dtAPRDRG_GLOBAL.Columns[cnt].ColumnName == "Sens" || dtAPRDRG_GLOBAL.Columns[cnt].ColumnName == "Sens_OB")
        //            {
        //                colCbx = new DataGridViewComboBoxColumn();
        //                colCbx.DataSource = lstOptions;
        //                colCbx.Name = dtAPRDRG_GLOBAL.Columns[cnt].ColumnName;
        //                colCbx.HeaderText = dtAPRDRG_GLOBAL.Columns[cnt].ColumnName;
        //                colCbx.SortMode = DataGridViewColumnSortMode.Programmatic;
        //                //colCbx.CellTemplate = new DataGridViewTextBoxCell();
        //                dgvAPRDRG.Columns.Add(colCbx);
        //            }
        //            else
        //            {
        //                col = new DataGridViewColumn();
        //                col.Name = dtAPRDRG_GLOBAL.Columns[cnt].ColumnName;
        //                col.HeaderText = dtAPRDRG_GLOBAL.Columns[cnt].ColumnName;
        //                col.CellTemplate = new DataGridViewTextBoxCell();
        //                col.SortMode = DataGridViewColumnSortMode.Programmatic;
        //                if (dtAPRDRG_GLOBAL.Columns[cnt].ColumnName != "APR_DRG_DESC_lc")
        //                    col.ReadOnly = true;

        //                if (dtAPRDRG_GLOBAL.Columns[cnt].ColumnName == "IsUpdated")
        //                    col.Visible = false;

        //                dgvAPRDRG.Columns.Add(col);
        //            }

        //        }
        //    }

        //    dgvAPRDRG.RowCount = dtAPRDRG_GLOBAL.Rows.Count;
        //    dgvAPRDRG.AutoResizeColumns();
        //    dgvAPRDRG.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        //    dgvAPRDRG.CurrentCell = null;
        //    dgvAPRDRG.ClearSelection();

        //    foreach (DataGridViewColumn column in dgvAPRDRG.Columns)
        //    {
        //        dgvAPRDRG.Columns[column.Name].SortMode = DataGridViewColumnSortMode.Automatic;
        //    }


        //}

        ////bool blEdit = false;
        //private void dgvAPRDRG_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //   // blEdit = true;
        //    ////do your checks to see RowIndex is not -1 and other good stuffs
        //    var row = dgvAPRDRG.Rows[e.RowIndex];
        //    var changedValue = (string)row.Cells[e.ColumnIndex].EditedFormattedValue;
        //    dtAPRDRG_GLOBAL.Rows[e.RowIndex][e.ColumnIndex] = changedValue;
        //    dtAPRDRG_GLOBAL.Rows[e.RowIndex]["IsUpdated"] = true;
        //    blSomenthingTouched_GLOBAL = true;
        //    //row.Cells[e.ColumnIndex].Value = changedValue;
        //    //MessageBox.Show("CellValueChanged = " + changedValue);
        //}


        //private void dgvAPRDRG_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        //{

        //}

        //private void dgvAPRDRG_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        //{

        //    int rowIndex = e.RowIndex;

        //    if (rowIndex >= dtAPRDRG_GLOBAL.Rows.Count)
        //        return;

        //    e.Value = dtAPRDRG_GLOBAL.Rows[rowIndex][e.ColumnIndex];

        //}


        //private void dgvAPRDRG_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        //{
        //    if (String.IsNullOrEmpty(dgvAPRDRG.Columns[e.ColumnIndex].HeaderText))
        //        return;


        //    dtAPRDRG_GLOBAL = SharedWinFormFunctions.handeDataGridViewSorting(ref this.dgvAPRDRG, dtAPRDRG_GLOBAL, e.ColumnIndex);
        //    this.dgvAPRDRG.Refresh();

        //}


        //private void frmAPRDRG_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    if (blSomenthingTouched_GLOBAL)
        //    {
        //        var window = MessageBox.Show("Changes will be lost. Close your APRDRG Session anyway?", "Are you sure?", MessageBoxButtons.YesNo);
        //        e.Cancel = (window == DialogResult.No);
        //    }



        //}

        //private void btnSubmitChanges_Click(object sender, EventArgs e)
        //{
        //    if (!blSomenthingTouched_GLOBAL)
        //        return;

        //    var dr_continue = MessageBox.Show("Do you want to save your changes?", "Are you sure?", MessageBoxButtons.YesNo);
        //    if (dr_continue == DialogResult.No)
        //        return;

        //    try
        //    {
        //        this.Cursor = Cursors.WaitCursor;
        //        tssStatus.Text = "Saving your updates to ILUCA.PBP_APRDRG..."; statusStrip1.Refresh();
        //        int intCnt = 0;
        //        //FIND ALL UPDATE RECORDS IN GRID
        //        DataRow[] drArr = dtAPRDRG_GLOBAL.Select("IsUpdated");
        //        StringBuilder sbUpdateSQL = new StringBuilder();
        //        string strTickMark = "";
        //        string strUpdateSQLTmp = GlobalObjects.getILUCAAPRDRGUpdateSQL();
        //        foreach (DataRow dr in drArr)
        //        {
        //            foreach (DataColumn dc in dr.Table.Columns)
        //            {

        //                if (dc.DataType == System.Type.GetType("System.String") || dc.DataType == System.Type.GetType("System.DateTime") || dc.DataType == System.Type.GetType("System.Date"))
        //                    strTickMark = "'";
        //                else
        //                    strTickMark = "NULL";

        //                strUpdateSQLTmp = strUpdateSQLTmp.Replace("{$" + dc.ColumnName + "}", (dr[dc.ColumnName] != DBNull.Value && dr[dc.ColumnName] + ""  != "" ? strTickMark + dr[dc.ColumnName].ToString().Replace("'", "''") + strTickMark : "NULL"));
        //            }
        //            //APPEND UPDATE STATEMENTS
        //            sbUpdateSQL.Append(strUpdateSQLTmp);
        //            strUpdateSQLTmp = GlobalObjects.getILUCAAPRDRGUpdateSQL();
        //            intCnt++;
        //        }
        //        //EXECUTE UPDATE STATEMENTS
        //        DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, sbUpdateSQL.ToString());
        //        //REFRESH
        //        tssStatus.Text = "Refreshing data..."; statusStrip1.Refresh();
        //        dtAPRDRG_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAAPRDRGSQL());
        //        populateDetails();

        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK);
        //    }
        //    finally
        //    {

        //        this.Cursor = Cursors.Default;
        //        tssStatus.Text = "Ready"; statusStrip1.Refresh();
        //    }
        //}

        //private void btnRefresh_Click(object sender, EventArgs e)
        //{
        //    if(blSomenthingTouched_GLOBAL)
        //    {
        //        var dr_continue = MessageBox.Show("Changes will be lost. Refresh this data anyway?", "Are you sure?", MessageBoxButtons.YesNo);
        //        if (dr_continue == DialogResult.No)
        //            return;
        //    }

        //    //REFRESH
        //    tssStatus.Text = "Refreshing data..."; statusStrip1.Refresh();
        //    dtAPRDRG_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAAPRDRGSQL());
        //    populateDetails();
        //    tssStatus.Text = "Ready"; statusStrip1.Refresh();
        //}

        //private void btnUGAPAPRDRG_Click(object sender, EventArgs e)
        //{
        //    if (blSomenthingTouched_GLOBAL)
        //    {
        //        var dr_continue = MessageBox.Show("Changes will be lost. Refresh this data with UGAP anyway?", "Are you sure?", MessageBoxButtons.YesNo);
        //        if (dr_continue == DialogResult.No)
        //            return;
        //    }


        //    try
        //    {
        //        this.Cursor = Cursors.WaitCursor;
        //        //PREP
        //        tssStatus.Text = "Preparing to retreive data from hcdm001.APR_DRG_BASE_CLASS..."; statusStrip1.Refresh();
        //        DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "TRUNCATE TABLE CG_DEV_TMP_PBP_APRDRG_LANDING;");
        //        //INSERT LANDING
        //        var vrResultCnt = DBConnection.getTeraDataExecuteScalar(GlobalObjects.strUGAPConnectionString, "SELECT COUNT(*) as total FROM (" + GlobalObjects.getUGAPAPRDRGSQL() + ") tmp ");
        //        strMessageGlobal = "Retrieving {$rowCnt} out of " + vrResultCnt + " records from hcdm001.APR_DRG_BASE_CLASS";
        //        tssProgressBar.Maximum = (int)vrResultCnt;
        //        tssProgressBar.Visible = true;
        //        SQLServerBulkImport(GlobalObjects.strUGAPConnectionString, GlobalObjects.strILUCAConnectionString, GlobalObjects.getUGAPAPRDRGSQL(), "CG_DEV_TMP_PBP_APRDRG_LANDING");
        //        tssProgressBar.Value = 0;
        //        tssProgressBar.Visible = false;
        //        //UPDATE MAIN
        //        tssStatus.Text = "Loading new records into ILUCA.PBP_APRDRG..."; statusStrip1.Refresh();
        //        DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "INSERT INTO [dbo].[CG_DEV_TMP_PBP_APRDRG] ([APR_DRG_SYS_ID] ,[APR_DRG_CD] ,[APR_DRG_DESC] ,[DSES_ST_NM] ,[APR_DRG_DESC_lc],[LOAD_DT],[UPDT_DT]) SELECT [APR_DRG_SYS_ID] ,[APR_DRG_CD] ,[APR_DRG_DESC] ,[DSES_ST_NM] ,[APR_DRG_DESC] as APR_DRG_DESC_lc ,[LOAD_DT] ,[UPDT_DT] FROM [dbo].[CG_DEV_TMP_PBP_APRDRG_LANDING] o WHERE not exists ( select * from CG_DEV_TMP_PBP_APRDRG i where o.APR_DRG_SYS_ID = i.APR_DRG_SYS_ID);");

        //        //REFRESH
        //        tssStatus.Text = "Refreshing data..."; statusStrip1.Refresh();
        //        dtAPRDRG_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAAPRDRGSQL());
        //        populateDetails();

        //    }
        //    catch(Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Error!", MessageBoxButtons.OK);
        //    }
        //    finally
        //    {
        //        this.Cursor = Cursors.Default;
        //        tssStatus.Text = "Ready"; statusStrip1.Refresh();
        //    }




        //}

        //private void btnExit_Click(object sender, EventArgs e)
        //{
        //    Close();
        //}





        //// ALL PURSPOSE MS SQL BULK DATA IMPORTER
        //private void SQLServerBulkImport(string strSourcenConnectionString, string strDestinationConnectionString, string strSQL, string strTableName)
        //{

        //    // GET THE SOURCE DATA
        //    using (TdConnection sourceConnection = new TdConnection(strSourcenConnectionString))
        //    {
        //        TdCommand myCommand =
        //            new TdCommand(strSQL, sourceConnection);
        //        sourceConnection.Open();
        //        TdDataReader reader = myCommand.ExecuteReader();

        //        // OPEN THE DESTINATION DATA
        //        using (SqlConnection destinationConnection =
        //                    new SqlConnection(strDestinationConnectionString))
        //        {
        //            // OPEN THE CONNECTION
        //            destinationConnection.Open();

        //            using (SqlBulkCopy bulkCopy =
        //            new SqlBulkCopy(destinationConnection.ConnectionString))
        //            {
        //                bulkCopy.BatchSize = 500;
        //                bulkCopy.NotifyAfter = 1;
        //                bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
        //                bulkCopy.DestinationTableName = strTableName;
        //                bulkCopy.WriteToServer(reader);
        //            }
        //        }
        //        reader.Close();
        //    }
        //}


        //private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        //{
        //    tssStatus.Text = strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)); statusStrip1.Refresh();
        //    //if (e.RowsCopied % 100 == 0)
        //        tssProgressBar.PerformStep();
        //}












    }
}
