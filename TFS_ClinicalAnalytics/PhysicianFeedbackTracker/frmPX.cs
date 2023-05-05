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
    public partial class frmPX : _BaseClass
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
        private string strSortCol = "AHRQ_PROC_DTL_CATGY_CD";
        private string strSortOrder = "ASC";

        string strMessageGlobal = null;
        bool blResizedGrid_GLOBAL = false;
        // DataTable dtPX_GLOBAL = null;
        public frmPX()
        {
            InitializeComponent();


            cbEdit_SelectionChangeCommittedHandler = new EventHandler(cbEdit_SelectionChangeCommitted);

            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                this.dgvPX,
                new object[] { true });

            dgvPX.VirtualMode = true;
            //dgvPX.ReadOnly = true;
            dgvPX.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPX.CellValueNeeded += new DataGridViewCellValueEventHandler(dgvPX_CellValueNeeded);
            dgvPX.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dgvPX_ColumnHeaderMouseClick);
            dgvPX.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgvPX_RowPostPaint);
            dgvPX.CellValueChanged += new DataGridViewCellEventHandler(dgvPX_CellValueChanged);

            Resetsort();
            LoadData(strSortCol, strSortOrder);


        }



        private void LoadData(string strSortCol, string strSortOrder)
        {
            dgvPX.Rows.Clear();
            dgvPX.Refresh();
            dgvPX.Columns.Clear();
            dgvPX.DataSource = null;

            try
            {

                var lstOptions = new List<string>() { "", "Y", "U" };

                DataGridViewButtonColumn colBtn = null;
                DataGridViewComboBoxColumn colCbx = null;
                DataGridViewColumn col = null;
                DataRetriever retriever = new DataRetriever(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAPXSQL(), strSortCol, strSortOrder);
                memoryCache = new Cache(retriever, 10000);

                foreach (DataColumn column in retriever.Columns)
                {



                    if (column.ColumnName == "Sens" || column.ColumnName == "Sens_OB")
                    {
                        colCbx = new DataGridViewComboBoxColumn();
                        colCbx.DataSource = lstOptions;
                        colCbx.Name = column.ColumnName;
                        colCbx.HeaderText = column.ColumnName;
                        colCbx.ReadOnly = false;
                        dgvPX.Columns.Add(colCbx);

                    }
                    else if (column.ColumnName == "Action")
                    {
                        colBtn = new DataGridViewButtonColumn();
                        colBtn.Text = "Resolve";
                        colBtn.Name = column.ColumnName;
                        colBtn.HeaderText = column.ColumnName;
                        colBtn.ReadOnly = false;
                        dgvPX.Columns.Add(colBtn);

                    }
                    else
                    {
                        col = new DataGridViewColumn();
                        col.Name = column.ColumnName;
                        col.HeaderText = column.ColumnName;
                        col.CellTemplate = new DataGridViewTextBoxCell();
                        col.SortMode = DataGridViewColumnSortMode.Programmatic;
                        col.ReadOnly = true;

                        dgvPX.Columns.Add(col);
                    }


                }
                this.dgvPX.RowCount = retriever.RowCount;
                if (!blResizedGrid_GLOBAL)
                {
                    this.dgvPX.AutoResizeColumns();
                    this.dgvPX.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    blResizedGrid_GLOBAL = true;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Load Grid Error", MessageBoxButtons.OK);
                Application.Exit();
            }

            // Adjust the column widths based on the displayed values. 
            this.dgvPX.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            if (strSortOrder == "ASC")
                this.dgvPX.Columns[strSortCol].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
            else
                this.dgvPX.Columns[strSortCol].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;

        }
        private void Resetsort()
        {
            strSortCol = "AHRQ_PROC_DTL_CATGY_CD";
            strSortOrder = "ASC";
        }

        //bool blEdit = false;
        private void dgvPX_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            ////DONT ALLOW ANY ACTION WHILE PROGRESS BAR RUNS!!!!!
            //if (tssProgressBar.Visible == true)
            //    return;


            //// blEdit = true;
            //////do your checks to see RowIndex is not -1 and other good stuffs
            //var row = dgvPX.Rows[e.RowIndex];
            //var changedValue = (string)row.Cells[e.ColumnIndex].EditedFormattedValue;
            //memoryCache.cachePages[0].table.Rows[e.RowIndex][e.ColumnIndex] = changedValue;
            //saveChanges(memoryCache.cachePages[0].table.Rows[e.RowIndex]);
        }


        bool blErrorGlobal = false;
        private void dgvPX_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            try
            {

                e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
            }
            catch
            {
                blErrorGlobal = true;
            }


        }

        private void dgvPX_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (blErrorGlobal)
            {
                blErrorGlobal = false;
                return;
            }


            // this routine will add row no to HeaderCell which comes left most
            var grid = sender as DataGridView;
            var rowIPX = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIPX, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dgvPX_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (strSortCol.Trim().ToUpper() != this.dgvPX.Columns[e.ColumnIndex].Name.Trim().ToUpper())
            {
                strSortOrder = "ASC";
            }
            else
            {
                strSortOrder = (this.dgvPX.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending ? "DESC" : "ASC");
            }
            strSortCol = this.dgvPX.Columns[e.ColumnIndex].Name;
            LoadData(strSortCol, strSortOrder);
        }


        private void saveChanges(DataRow dr)
        {

            try
            {
                this.Cursor = Cursors.WaitCursor;
                tssStatus.Text = "Saving your updates to ILUCA.PBP_PX..."; statusStrip1.Refresh();
                string strTickMark = null;
                string strUpdateSQL = GlobalObjects.getILUCAPXUpdateSQL();
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

        string strUGAPUserName;
        string strUGAPPassword;

        private void btnUGAPPX_Click(object sender, EventArgs e)
        {

            //DONT ALLOW ANY ACTION WHILE PROGRESS BAR RUNS!!!!!
            if (tssProgressBar.Visible == true)
                return;

            try
            {

                string UGAPConnectionString = GlobalObjects.strUGAPConnectionString;

                if(GlobalObjects.strCurrentUser != "cgiorda")
                {
                    if (strUGAPPassword == null || strUGAPUserName == null)
                    {
                        frmUserNamePassword form = new frmUserNamePassword();
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.strDBName = "UGAP";
                        form.ShowDialog();

                        strUGAPUserName = form.strUserName;
                        strUGAPPassword = form.strPassword;
                    }

                    UGAPConnectionString = GlobalObjects.strUGAPConnectionStringAddCredentials.Replace("{$ugapdbusername}", strUGAPUserName).Replace("{$ugapdbpassword}", strUGAPPassword);
                }


                string strTableName = "PBP_LabPath_Procs_MAD";
                this.Cursor = Cursors.WaitCursor;

                //PREP
                tssStatus.Text = "Preparing to retreive data from uhcdm001.PROCEDURE_CODE..."; statusStrip1.Refresh();


                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + "_UGAP_LANDING;");
                //INSERT LANDING
                var vrResultCnt = DBConnection.getTeraDataExecuteScalar(UGAPConnectionString, "SELECT COUNT(*) as total FROM (" + GlobalObjects.getUGAPPXSQL() + ") tmp ");
                strMessageGlobal = "Retrieving {$rowCnt} out of " + String.Format("{0:n0}", vrResultCnt) + " records from uhcdm001.PROCEDURE_CODE";
                tssProgressBar.Visible = true;
                tssProgressBar.Maximum = (int)vrResultCnt;
                tssProgressBar.Visible = true;

                DBConnection.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection.SQLServerBulkImport(UGAPConnectionString, GlobalObjects.strILUCAConnectionString, GlobalObjects.getUGAPPXSQL(), strTableName + "_UGAP_LANDING", 500);

                tssProgressBar.Value = 0;
                tssProgressBar.Visible = false;
                //UPDATE MAIN
                tssStatus.Text = "Loading new records into ILUCA.PBP_PX..."; statusStrip1.Refresh();

               

               // string strSQLTmp = "truncate table " + strTableName + "; INSERT INTO [dbo].[" + strTableName + "] ([AHRQ_PROC_DTL_CATGY_CD] ,[PROC_CD] ,[PROC_DESC] ,[AHRQ_PROC_DTL_CATGY_DESC] ,[Proc_Categ_LC] ,[Sensitive] ,[Proc_Categ] ,[Sens] ,[Sens_OB] ,[PROC_TYP_CD] ,[insert_date] ,[update_date]) SELECT LTRIM(RTRIM([AHRQ_PROC_DTL_CATGY_CD])) as AHRQ_PROC_DTL_CATGY_CD ,LTRIM(RTRIM([PROC_CD])) as PROC_CD ,[PROC_DESC] ,[AHRQ_PROC_DTL_CATGY_DESC] ,[Proc_Categ_LC] ,[Sensitive] ,[Proc_Categ] ,[Sens] ,[Sens_OB] ,[PROC_TYP_CD], '01/01/2019', '02/01/2019' FROM [dbo].[PBP_LabPath_Procs_MAD];UPDATE a SET a.SENS_COND_IND = b.SENS_COND_IND,a.SENS_COND_CATGY = b.SENS_COND_CATGY FROM " + strTableName + " AS a INNER JOIN " + strTableName + "_LANDING AS b ON a.PROC_CD = b.PROC_CD; UPDATE [IL_UCA].[dbo].[CG_DEV_TMP_PBP_LabPath_Procs] SET [AHRQ_PROC_DTL_CATGY_CD] = LTRIM(RTRIM([AHRQ_PROC_DTL_CATGY_CD])), [PROC_CD]= LTRIM(RTRIM([PROC_CD])); "; strSQLTmp + 


                string strSQL = "IF OBJECT_ID('tempdb..#PX_landing_new') IS NOT NULL DROP TABLE #PX_landing_new; CREATE TABLE #PX_landing_new ( [AHRQ_PROC_DTL_CATGY_CD] [char](10) NULL, [PROC_CD] [char](7) NULL, [PROC_DESC] [varchar](70) NULL, [AHRQ_PROC_DTL_CATGY_DESC] [varchar](75) NULL, [PROC_TYP_CD] [char](5) NULL, [Proc_Categ] [nvarchar](75) NULL, [Sens] [varchar](1) NULL, [Sens_OB] [varchar](1) NULL, SENS_COND_IND [char](1) NULL, SENS_COND_CATGY [varchar](30) NULL, [update_date] [datetime] NULL); INSERT INTO #PX_landing_new ([AHRQ_PROC_DTL_CATGY_CD] ,[PROC_CD] ,[PROC_DESC] ,[AHRQ_PROC_DTL_CATGY_DESC] ,[PROC_TYP_CD], SENS_COND_IND, SENS_COND_CATGY) SELECT o.AHRQ_PROC_DTL_CATGY_CD ,o.PROC_CD ,o.PROC_DESC ,o.AHRQ_PROC_DTL_CATGY_DESC ,o.PROC_TYP_CD, o.SENS_COND_IND, o.SENS_COND_CATGY FROM [dbo].[" + strTableName + "_UGAP_LANDING] o left join [dbo].[" + strTableName + "] as i on o.PROC_CD = i.PROC_CD WHERE i.PROC_CD is null; update o set o.proc_categ=t.proc_categ, o.sens=t.sens, o.sens_ob=t.sens_ob, o.update_date = getdate() from #PX_landing_new as o inner join (SELECT distinct AHRQ_PROC_DTL_CATGY_CD,Proc_Categ,sens,sens_ob from [dbo].[" + strTableName + "] WHERE AHRQ_PROC_DTL_CATGY_CD in ( SELECT AHRQ_PROC_DTL_CATGY_CD FROM ( select distinct AHRQ_PROC_DTL_CATGY_CD,Proc_Categ,sens,sens_ob from [dbo].[" + strTableName + "] ) tmp group by AHRQ_PROC_DTL_CATGY_CD having count(*)=1 )) as t on t.AHRQ_PROC_DTL_CATGY_CD=o.AHRQ_PROC_DTL_CATGY_CD INSERT INTO [dbo].[" + strTableName + "] ([AHRQ_PROC_DTL_CATGY_CD] ,[PROC_CD] ,[PROC_DESC] ,[AHRQ_PROC_DTL_CATGY_DESC] ,[PROC_TYP_CD], SENS_COND_IND, SENS_COND_CATGY ,[Proc_Categ] ,[Sens] ,[Sens_OB] ,[update_date]) SELECT [AHRQ_PROC_DTL_CATGY_CD], [PROC_CD], [PROC_DESC], [AHRQ_PROC_DTL_CATGY_DESC], [PROC_TYP_CD], SENS_COND_IND, SENS_COND_CATGY, [Proc_Categ], [Sens], [Sens_OB], [update_date] FROM #PX_landing_new;";


                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, strSQL);


                //DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "INSERT INTO [dbo].[CG_DEV_TMP_PBP_PX] ([DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] ) SELECT [DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] FROM [dbo].[CG_DEV_TMP_PBP_PX_LANDING] o WHERE not exists ( select * from CG_DEV_TMP_PBP_PX i where o.DIAG_CD_SYS_ID = i.DIAG_CD_SYS_ID);");

                //REFRESH
                tssStatus.Text = "Refreshing data..."; statusStrip1.Refresh();

                //dtPX_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCAPXSQL());

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

    



        private static DataGridViewComboBoxEditingControl cbEdit;
        private EventHandler cbEdit_SelectionChangeCommittedHandler;

        private void dgvPX_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            cbEdit = e.Control as DataGridViewComboBoxEditingControl;
            if (cbEdit != null)
                cbEdit.SelectionChangeCommitted += cbEdit_SelectionChangeCommittedHandler;
        }

        private void cbEdit_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (tssProgressBar.Visible == true)
                return;

            DataGridView dgv = cbEdit.EditingControlDataGridView;
            DataGridViewCell cell = dgv.CurrentCell;

            if (cell.RowIndex >= memoryCache.cachePages[0].table.Rows.Count)
                return;


            cell.Value = cbEdit.SelectedItem;
            dgv.EndEdit();

            //cell.Value.ToString

            var row = dgvPX.Rows[cell.RowIndex];
            var changedValue = (string)row.Cells[cell.ColumnIndex].EditedFormattedValue;
            memoryCache.cachePages[0].table.Rows[cell.RowIndex][cell.ColumnIndex] = changedValue;
            saveChanges(memoryCache.cachePages[0].table.Rows[cell.RowIndex]);


            try
            {
                dgvPX.ClearSelection();
                dgvPX.CurrentCell = null;
            }
            catch (Exception)
            {

            }

        }


        private void dgvPX_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (cbEdit != null)
                cbEdit.SelectionChangeCommitted -= cbEdit_SelectionChangeCommitted;
        }

        private void dgvPX_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string dataValue = dgvPX.Rows[e.RowIndex].Cells["AHRQ_DIAG_DTL_CATGY_CD"].Value.ToString();

            frmDXDetails form = new frmDXDetails(dataValue);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        private void dgvPX_MouseMove(object sender, MouseEventArgs e)
        {
            if (tssStatus.Text == "Ready")
            {
                dgvPX.ScrollBars = ScrollBars.Both;
                this.Cursor = Cursors.Default;
            }
            else
            {
                dgvPX.ScrollBars = ScrollBars.None;
                this.Cursor = Cursors.WaitCursor;
            }


            Application.DoEvents();
        }

        private void dgvPX_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            


            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                //TODO - Button Clicked - Execute Code Here

                string catCode = dgvPX.Rows[e.RowIndex].Cells["AHRQ_PROC_DTL_CATGY_CD"].Value.ToString();
                string procCode = dgvPX.Rows[e.RowIndex].Cells["PROC_CD"].Value.ToString();
                string catDesc = dgvPX.Rows[e.RowIndex].Cells["AHRQ_PROC_DTL_CATGY_DESC"].Value.ToString();
                string procDesc = dgvPX.Rows[e.RowIndex].Cells["PROC_DESC"].Value.ToString();

                frmPXDetails form = new frmPXDetails(catCode, procCode, catDesc,procDesc);
                form.StartPosition = FormStartPosition.CenterParent;
                var result = form.ShowDialog();


                if (result == DialogResult.OK)
                {
                    dgvPX.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkGreen;
                    dgvPX.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }
               else  if (result == DialogResult.None)
                {
                    MessageBox.Show(form.errorMessage, "Error");

                    dgvPX.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.DarkRed;
                    dgvPX.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.White;
                }
                else if (result == DialogResult.Cancel)
                {
                    //CANCELLED
                }
            }
        }
    }
}
