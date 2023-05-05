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
using System.Threading;

namespace PhysicianFeedbackTracker
{
    public partial class frmDX : _BaseClass
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
        //private string strSortCol = "DIAG_CD_SYS_ID";
        private string strSortCol = "update_date";
        private string strSortOrder = "ASC";

        string strMessageGlobal = null;
        bool blResizedGrid_GLOBAL = false;
        // DataTable dtDX_GLOBAL = null;
        public frmDX()
        {
            InitializeComponent();



            cbEdit_SelectionChangeCommittedHandler = new EventHandler(cbEdit_SelectionChangeCommitted);

            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null,
                this.dgvDX,
                new object[] { true });

            dgvDX.VirtualMode = true;
            //dgvDX.ReadOnly = true;
            dgvDX.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDX.CellValueNeeded += new DataGridViewCellValueEventHandler(dgvDX_CellValueNeeded);
            dgvDX.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dgvDX_ColumnHeaderMouseClick);
            dgvDX.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dgvDX_RowPostPaint);
            dgvDX.CellValueChanged += new DataGridViewCellEventHandler(dgvDX_CellValueChanged);

            Resetsort();
            LoadData(strSortCol, strSortOrder);


        }



        private void LoadData(string strSortCol, string strSortOrder)
        {
            dgvDX.Rows.Clear();
            dgvDX.Refresh();
            dgvDX.Columns.Clear();
            dgvDX.DataSource = null;

            try
            {

                var lstOptions = new List<string>() { "", "Y" ,"U"};

                DataGridViewComboBoxColumn colCbx = null;
                DataGridViewColumn col = null;
                DataRetriever retriever = new DataRetriever(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCADXSQL(), strSortCol, strSortOrder);
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
                        dgvDX.Columns.Add(colCbx);

                    }
                    else
                    {
                        col = new DataGridViewColumn();
                        col.Name = column.ColumnName;
                        col.HeaderText = column.ColumnName;
                        col.CellTemplate = new DataGridViewTextBoxCell();
                        col.SortMode = DataGridViewColumnSortMode.Programmatic;
                        col.ReadOnly = true;
                        dgvDX.Columns.Add(col);
                    }


                }
                this.dgvDX.RowCount = retriever.RowCount;
                if(!blResizedGrid_GLOBAL)
                {
                    this.dgvDX.AutoResizeColumns();
                    this.dgvDX.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    blResizedGrid_GLOBAL = true;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Load Grid Error", MessageBoxButtons.OK);
                Application.Exit();
            }

            // Adjust the column widths based on the displayed values. 
            this.dgvDX.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
            if (strSortOrder == "ASC")
                this.dgvDX.Columns[strSortCol].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Ascending;
            else
                this.dgvDX.Columns[strSortCol].HeaderCell.SortGlyphDirection = System.Windows.Forms.SortOrder.Descending;

        }
        private void Resetsort()
        {
            //strSortCol = "DIAG_CD_SYS_ID";
            strSortCol = "update_date";
            strSortOrder = "ASC";
        }

        //bool blEdit = false;
        private void dgvDX_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

        }


        bool blErrorGlobal = false;
        private void dgvDX_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            try
            {
                e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
                DateTime temp;
                if (DateTime.TryParse(e.Value.ToString(), out temp))
                {
                    e.Value = temp.ToShortDateString();
                    //e.FormattingApplied = true;
                }

            }
            catch
            {
                blErrorGlobal = true;
            }

        }

        private void dgvDX_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {

            if (blErrorGlobal)
            {
                blErrorGlobal = false;
                return;
            }

            // this routine will add row no to HeaderCell which comes left most
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void dgvDX_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (strSortCol.Trim().ToUpper() != this.dgvDX.Columns[e.ColumnIndex].Name.Trim().ToUpper())
            {
                strSortOrder = "DESC";
            }
            else
            {
                strSortOrder = (this.dgvDX.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection == System.Windows.Forms.SortOrder.Ascending ? "DESC" : "ASC");
            }
            strSortCol = this.dgvDX.Columns[e.ColumnIndex].Name;
            LoadData(strSortCol, strSortOrder);
        }


        private void saveChanges(DataRow dr)
        {
            //MessageBox.Show("Save");

            try
            {
                this.Cursor = Cursors.WaitCursor;

                tssStatus.Text = "Saving your updates to ILUCA.PBP_DX..."; statusStrip1.Refresh();
                string strTickMark = null;
                string strUpdateSQL = GlobalObjects.getILUCADXUpdateSQL();
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


        private void btnUGAPDX_Click(object sender, EventArgs e)
        {

            //DONT ALLOW ANY ACTION WHILE PROGRESS BAR RUNS!!!!!
            if (tssProgressBar.Visible == true)
                return;

            try
            {

                string strTableName = "PBP_DX";
                this.Cursor = Cursors.WaitCursor;

                //PREP
                tssStatus.Text = "Preparing to retreive data from uhcdm001.diagnosis_Code..."; statusStrip1.Refresh();
                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "TRUNCATE TABLE CG_DEV_TMP_PBP_DX_LANDING;");
                //INSERT LANDING
                var vrResultCnt = DBConnection.getTeraDataExecuteScalar(GlobalObjects.strUGAPConnectionString, "SELECT COUNT(*) as total FROM (" + GlobalObjects.getUGAPDXSQL() + ") tmp ");
                strMessageGlobal = "Retrieving {$rowCnt} out of " + String.Format("{0:n0}", vrResultCnt) + " records from uhcdm001.diagnosis_Code";
                tssProgressBar.Visible = true;
                tssProgressBar.Maximum = (int)vrResultCnt;
                tssProgressBar.Visible = true;

                DBConnection.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection.SQLServerBulkImport(GlobalObjects.strUGAPConnectionString, GlobalObjects.strILUCAConnectionString, GlobalObjects.getUGAPDXSQL(), "CG_DEV_TMP_PBP_DX_LANDING", 500);

                tssProgressBar.Value = 0;
                tssProgressBar.Visible = false;
                //UPDATE MAIN
                tssStatus.Text = "Loading new records into ILUCA.PBP_DX..."; statusStrip1.Refresh();
                //DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "INSERT INTO [dbo].[CG_DEV_TMP_PBP_DX] ([DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] ) SELECT [DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] FROM [dbo].[CG_DEV_TMP_PBP_DX_LANDING] o WHERE not exists ( select * from CG_DEV_TMP_PBP_DX i where o.DIAG_CD = i.DIAG_CD);IF OBJECT_ID('tempdb..#DX_to_update') IS NOT NULL DROP TABLE #DX_to_update; CREATE TABLE #DX_to_update ([AHRQ_DIAG_DTL_CATGY_CD] [nvarchar](4) NOT NULL, [Sens] [varchar](5) NULL, [Sens_OB] [varchar](5) NULL); INSERT INTO #DX_to_update(AHRQ_DIAG_DTL_CATGY_CD, Sens, Sens_OB) SELECT distinct a.AHRQ_DIAG_DTL_CATGY_CD , a.Sens, a.Sens_OB as Sens_OB FROM [dbo].[CG_DEV_TMP_PBP_DX] AS a WHERE a.update_date IS NOT NULL; Create NonClustered Index TMP_INDX_DX_to_update On #DX_to_update (AHRQ_DIAG_DTL_CATGY_CD); UPDATE a SET a.Sens = b.Sens,a.Sens_OB = b.Sens_OB, a.update_date = getdate() FROM [dbo].[CG_DEV_TMP_PBP_DX] AS a INNER JOIN #DX_to_update AS b ON a.AHRQ_DIAG_DTL_CATGY_CD=b.AHRQ_DIAG_DTL_CATGY_CD WHERE a.update_date IS NULL; UPDATE CG_DEV_TMP_PBP_DX SET Sens = 'U',Sens_OB = 'U', update_date = getdate() WHERE AHRQ_DIAG_DTL_CATGY_CD = '999' AND update_date IS NULL;");


                DBConnection.ExecuteMSSQL(GlobalObjects.strILUCAConnectionString, "IF OBJECT_ID('tempdb..#DX_new_data') IS NOT NULL DROP TABLE #DX_new_data;CREATE TABLE #DX_new_data ([DIAG_CD_SYS_ID] [int], [AHRQ_DIAG_DTL_CATGY_NM] [nvarchar](100), [CHRNC_FLG_NM] [nvarchar](10), [DIAG_CD] [nvarchar](7), [DIAG_DECM_CD] [nvarchar](8), [DIAG_DESC] [nvarchar](70), [AHRQ_DIAG_DTL_CATGY_CD] [nvarchar](4), [ICD_VER_CD] [nvarchar](1), [AHRQ_Diagnosis_Category] [nvarchar](255), [Sens] [varchar](5), [Sens_OB] [varchar](5), [update_date] [date]); INSERT INTO #DX_new_data ([DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category]) SELECT [DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] FROM [dbo].[CG_DEV_TMP_PBP_DX_LANDING] o WHERE not exists ( select * from "+ strTableName + " i where o.DIAG_CD = i.DIAG_CD);IF OBJECT_ID('tempdb..#DX_to_update') IS NOT NULL DROP TABLE #DX_to_update;CREATE TABLE #DX_to_update ([AHRQ_DIAG_DTL_CATGY_CD] [nvarchar](4) NOT NULL  unique, [AHRQ_Diagnosis_Category] [nvarchar](255) NOT NULL,[Sens] [varchar](5) NULL, [Sens_OB] [varchar](5) NULL); INSERT INTO #DX_to_update(AHRQ_DIAG_DTL_CATGY_CD, AHRQ_Diagnosis_Category, Sens, Sens_OB) SELECT distinct a.AHRQ_DIAG_DTL_CATGY_CD , a.AHRQ_Diagnosis_Category,a.Sens, a.Sens_OB as Sens_OB FROM [dbo].[" + strTableName + "] AS a; Create NonClustered Index TMP_INDX_DX_to_update On #DX_to_update (AHRQ_DIAG_DTL_CATGY_CD); UPDATE a SET a.Sens = b.Sens,a.Sens_OB = b.Sens_OB, a.AHRQ_Diagnosis_Category= b.AHRQ_Diagnosis_Category, a.update_date = getdate() FROM #DX_new_data AS a INNER JOIN #DX_to_update AS b ON a.AHRQ_DIAG_DTL_CATGY_CD=b.AHRQ_DIAG_DTL_CATGY_CD; UPDATE #DX_new_data SET Sens = 'U',Sens_OB = 'U', AHRQ_Diagnosis_Category= 'UNKNOWN DIAGNOSIS', update_date = getdate() WHERE AHRQ_DIAG_DTL_CATGY_CD = '999'; INSERT INTO " + strTableName + " ([DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] ,[Sens] ,[Sens_OB], update_date) SELECT [DIAG_CD_SYS_ID] ,[AHRQ_DIAG_DTL_CATGY_NM] ,[CHRNC_FLG_NM] ,[DIAG_CD] ,[DIAG_DECM_CD] ,[DIAG_DESC] ,[AHRQ_DIAG_DTL_CATGY_CD] ,[ICD_VER_CD] ,[AHRQ_Diagnosis_Category] ,[Sens] ,[Sens_OB], update_date FROM #DX_new_data;");


                //999 = SensU both




                //UPDATE a SET a.Sens = b.Sens,a.Sens_OB = b.Sens_OB, a.update_date = getdate() FROM [dbo].[CG_DEV_TMP_PBP_DX] AS a INNER JOIN (SELECT distinct AHRQ_DIAG_DTL_CATGY_CD , a.Sens,a.Sens_OB, a.update_date FROM [dbo].[CG_DEV_TMP_PBP_DX] AS a WHERE a.update_date IS NOT NULL) AS b ON a.AHRQ_DIAG_DTL_CATGY_CD=b.AHRQ_DIAG_DTL_CATGY_CD WHERE a.update_date IS NULL;

                //REFRESH
                tssStatus.Text = "Refreshing data..."; statusStrip1.Refresh();

                //dtDX_GLOBAL = SharedFunctions.prepDataTableForEditing(GlobalObjects.strILUCAConnectionString, GlobalObjects.getILUCADXSQL());

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
            if(tssProgressBar.Value < tssProgressBar.Maximum)
                tssProgressBar.Value += 1;
            Application.DoEvents();
        }


        private void dgvDX_MouseMove(object sender, MouseEventArgs e)
        {


            if (tssStatus.Text == "Ready")
            {
                dgvDX.ScrollBars = ScrollBars.Both;
                this.Cursor = Cursors.Default;
            }
            else
            {
                dgvDX.ScrollBars = ScrollBars.None;
                this.Cursor = Cursors.WaitCursor;
            }


            Application.DoEvents();
        }



        private void dgvDX_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            string dataValue = dgvDX.Rows[e.RowIndex].Cells["AHRQ_DIAG_DTL_CATGY_CD"].Value.ToString();

            frmDXDetails form = new frmDXDetails(dataValue);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }




        private static DataGridViewComboBoxEditingControl cbEdit;
        private EventHandler cbEdit_SelectionChangeCommittedHandler;

        private void dgvDX_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            cbEdit = e.Control as DataGridViewComboBoxEditingControl;
            if (cbEdit != null)
                cbEdit.SelectionChangeCommitted += cbEdit_SelectionChangeCommittedHandler;
        }

        private  void cbEdit_SelectionChangeCommitted(object sender, EventArgs e)
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

            var row = dgvDX.Rows[cell.RowIndex];
            var changedValue = (string)row.Cells[cell.ColumnIndex].EditedFormattedValue;
            memoryCache.cachePages[0].table.Rows[cell.RowIndex][cell.ColumnIndex] = changedValue;
            saveChanges(memoryCache.cachePages[0].table.Rows[cell.RowIndex]);


            try
            {
                dgvDX.ClearSelection();
                dgvDX.CurrentCell = null;
            }
            catch (Exception)
            {

            }

        }


        private void dgvDX_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (cbEdit != null)
                cbEdit.SelectionChangeCommitted -= cbEdit_SelectionChangeCommitted;


        }

      
    }
}
