using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClosedXML.Excel;
using System.Diagnostics;
using System.Configuration;

namespace PhysicianFeedbackTracker
{
    public partial class frmSelectTrackingItem : _BaseClass
    {

        DataGridViewCell ActiveCell = null;
        ContextMenu contextMenu = new ContextMenu();
        public frmSelectTrackingItem()
        {
            InitializeComponent();


            contextMenu.MenuItems.Add(new MenuItem("Copy", CopyClick));
        }

        private void EditTrackingItem_Load(object sender, EventArgs e)
        {

            //if (!ActiveDirectoryFunctions.hasAccess(GlobalObjects.strCurrentUser, "pei2_readonly") && !ActiveDirectoryFunctions.hasAccess(GlobalObjects.strCurrentUser, "pei2_users"))
            //{
            //    MessageBox.Show("Get Out!!!!");
            //    this.Close();

            //    //TAKE THEM SOMEWHERE ELSE
            //    return;
            //}

            //DataTable dtTmp = null;
            //List<string> lstADInfo = null;
            //Hashtable htTmp = GlobalObjects.htGetUserEmailByUserIdSQL(GlobalObjects.strCurrentUser);
            //object objPEIReturn = DBConnection.getMSSQLExecuteScalarSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strGetUserEmailByUserIdSQL, htTmp);

            //if (objPEIReturn == null)
            //{

            //    if (ActiveDirectoryFunctions.strADUserName == null || ActiveDirectoryFunctions.strADPassword == null)
            //    {
            //        dtTmp = DBConnection.getMSSQLDataTable(GlobalObjects.strPEIConnectionString, GlobalObjects.strGetAdUserNamePassword);
            //        if (dtTmp != null)
            //        {
            //            ActiveDirectoryFunctions.strADUserName = dtTmp.Rows[0][0] + "";
            //            ActiveDirectoryFunctions.strADPassword = dtTmp.Rows[0][1] + "";
            //        }
            //    }

            //    lstADInfo = ActiveDirectoryFunctions.GetADInfo(GlobalObjects.strCurrentUser);
            //    htTmp = GlobalObjects.htInsertUpdateUserSQL(GlobalObjects.strCurrentUser, lstADInfo[0], lstADInfo[1],lstADInfo[2]);
            //    DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strInsertUpdateUserSQL, htTmp);

            //    GlobalObjects.strCurrentEmail = lstADInfo[2];
            //}
            //else
            //{
            //    GlobalObjects.strCurrentEmail = objPEIReturn.ToString();
            //}


            GlobalObjects.strCurrentEmail = SharedFunctions.getEmailAddress(GlobalObjects.strCurrentUser);


            //if(GlobalObjects.argumentFilterMPINString != null || GlobalObjects.argumentFilterParentIdString != null)
            //{
            //    clearLinkFilterToolStripMenuItem.Visible = true;
            //}



            DataTable dtTmp = GlobalObjects.getNameValueDataTable("users").Select("filter IN ('QA','Affordability')").CopyToDataTable();
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("Check/Uncheck All", "-9999", dtTmp);
            clbSelectUser.DataSource = dtTmp;
            clbSelectUser.DisplayMember = "name";
            clbSelectUser.ValueMember = "value";
            //SharedWinFormFunctions.checkUncheckCheckBoxList(ref clbSelectUser, false, strValueToCheck:GlobalObjects.strCurrentUser);
            SharedWinFormFunctions.checkUncheckCheckBoxList(ref clbSelectUser, true);


            dtTmp = GlobalObjects.getNameValueDataTable("phase");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--All Projects--", "-9999", dtTmp);
            cmbPhase.DataSource = dtTmp;
            cmbPhase.DisplayMember = "name";
            cmbPhase.ValueMember = "value";
            //cmbPhase.SelectedIndex = 2;

            dtTmp = GlobalObjects.getNameValueDataTable("inquiry_category");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--All Categories--", "-9999", dtTmp);
            cbxInquiryCategory.DataSource = dtTmp;
            cbxInquiryCategory.DisplayMember = "name";
            cbxInquiryCategory.ValueMember = "value";



            dtTmp = GlobalObjects.getNameValueDataTable("inquiry_status");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--All Statuses--", "-9999", dtTmp);
            cbxInquiryStatus.DataSource = dtTmp;
            cbxInquiryStatus.DisplayMember = "name";
            cbxInquiryStatus.ValueMember = "value";



            Dictionary<string, string> dicTrackerStatus = new Dictionary<string, string>();
            dicTrackerStatus.Add("-9999", "Open and Closed");
            dicTrackerStatus.Add("1", "Open Only");
            dicTrackerStatus.Add("0", "Closed Only");
            cbxTrackerStatus.DataSource = new BindingSource(dicTrackerStatus, null);
            cbxTrackerStatus.DisplayMember = "Value";
            cbxTrackerStatus.ValueMember = "Key";


            //QA TOOLS SECTION
            //if(GlobalObjects.getNameValueDataTable("users").Select("filter ='QA' AND  value ='" + GlobalObjects.strCurrentUser + "'").Count() > 0)
            //    detailsToolStripMenuItem.Visible = true;
            //else
            //    detailsToolStripMenuItem.Visible = false;


            detailsToolStripMenuItem.Visible = SharedFunctions.hasAccess(GlobalObjects.strCurrentUser, "QA");




            if (GlobalObjects.strCurrentUser == "cgiorda" || GlobalObjects.strCurrentUser == "mdimar2")
                iLUCAToolStripMenuItem.Enabled = true;




            //UNCOMMNET ME!!!!! .... MAYBE
            //loadDataGridViewOLD();

                //TESTING DATAGRIDVIEW VIRTUAL MODE START
                //TESTING DATAGRIDVIEW VIRTUAL MODE START
                //TESTING DATAGRIDVIEW VIRTUAL MODE START
            dgvTrackingItems.CellValueNeeded += new DataGridViewCellValueEventHandler(dgvTrackingItems_CellValueNeeded);
            dgvTrackingItems.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(dgvTrackingItems_ColumnHeaderMouseClick);



            dgvTrackingItems.VirtualMode = true;
            loadDataGridView();
            //TESTING DATAGRIDVIEW VIRTUAL MODE END
            //TESTING DATAGRIDVIEW VIRTUAL MODE END
            //TESTING DATAGRIDVIEW VIRTUAL MODE END

        }

        //TESTING DATAGRIDVIEW VIRTUAL MODE START
        //TESTING DATAGRIDVIEW VIRTUAL MODE START
        //TESTING DATAGRIDVIEW VIRTUAL MODE START
        //const int PAGE_SIZE = 20;
        int _intColDifference = 0;
        public void loadDataGridView(bool blResetTable = true)
        {
            this.Cursor = Cursors.WaitCursor;


            if(blResetTable)
            {
                GlobalObjects.dtTrackingParentCache = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectTrackerRequestSQL, getParameterList());
                dgvTrackingItems.RowCount = 0;
                dgvTrackingItems.Rows.Clear();
                SharedWinFormFunctions.removeSortGlyphsFromDataGridView(ref dgvTrackingItems);
            }

            if(dgvTrackingItems.Columns.Count == 0)
            {
                DataGridViewColumn col = null;
                for (int cnt = 0; cnt < GlobalObjects.dtTrackingParentCache.Columns.Count; cnt++)
                {
                    col = new DataGridViewColumn();
                    col.Name = GlobalObjects.dtTrackingParentCache.Columns[cnt].ColumnName;
                    col.HeaderText = GlobalObjects.dtTrackingParentCache.Columns[cnt].ColumnName;
                    col.CellTemplate = new DataGridViewTextBoxCell();
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                    dgvTrackingItems.Columns.Add(col);
                }
                SharedWinFormFunctions.hideColumnsInDataGridView(ref dgvTrackingItems, GlobalObjects.strTrackerRequestHideArr);

                _intColDifference = (GlobalObjects.strTrackerRequestHideArr.Count() > 0 ? -3 :0);
                SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvTrackingItems, "Delete", Color.Red, Color.White, 0, false);
                SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvTrackingItems, "New Inquiry", Color.Blue, Color.White, 1, false);
                SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvTrackingItems, "History", Color.Green, Color.White, 2, false);

            }

            dgvTrackingItems.RowCount = GlobalObjects.dtTrackingParentCache.Rows.Count;
            dgvTrackingItems.AutoResizeColumns();

            this.Cursor = Cursors.Default;
        }

        private void dgvTrackingItems_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            int rowIndex = e.RowIndex;

            if (rowIndex >= GlobalObjects.dtTrackingParentCache.Rows.Count)
                return;

            e.Value = GlobalObjects.dtTrackingParentCache.Rows[rowIndex][e.ColumnIndex + _intColDifference];

        }

        private void dgvTrackingItems_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (String.IsNullOrEmpty(dgvTrackingItems.Columns[e.ColumnIndex].HeaderText))
                return;


            GlobalObjects.dtTrackingParentCache = SharedWinFormFunctions.handeDataGridViewSorting(ref this.dgvTrackingItems, GlobalObjects.dtTrackingParentCache, e.ColumnIndex);
            this.dgvTrackingItems.Refresh();

        }

        //TESTING DATAGRIDVIEW VIRTUAL MODE END
        //TESTING DATAGRIDVIEW VIRTUAL MODE END
        //TESTING DATAGRIDVIEW VIRTUAL MODE END


        frmAddProvders _frmAddProvders;
        private void btnAddProviders_Click(object sender, EventArgs e)
        {

        }


        public void loadDataGridViewOLD()
        {
            this.Cursor = Cursors.WaitCursor;

           

            GlobalObjects.dtTrackingParentCache = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString,GlobalObjects.strSelectTrackerRequestSQL, getParameterList());

            dgvTrackingItems.DataSource = GlobalObjects.dtTrackingParentCache;
            SharedWinFormFunctions.hideColumnsInDataGridView(ref dgvTrackingItems, GlobalObjects.strTrackerRequestHideArr);
            SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvTrackingItems, "Delete", Color.Red, Color.White, 0, false);
            SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvTrackingItems, "New Inquiry", Color.Blue, Color.White, 1, false);
            SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvTrackingItems, "History", Color.Green, Color.White, 2, false);
            dgvTrackingItems.AutoResizeColumns();

            this.Cursor = Cursors.Default;
        }



        private Hashtable getParameterList()
        {
            string strPhase = cmbPhase.SelectedValue.ToString();
            string userIdsCSV = SharedWinFormFunctions.checkBoxListCheckedToCSV(clbSelectUser);
            string strStartDate = (dtpStartDate.Checked == true ? dtpStartDate.Value.ToShortDateString() : null);
            string strEndDate = (dtpEndDate.Checked == true ? dtpEndDate.Value.ToShortDateString() : null);
            string strProviderSearch = (!String.IsNullOrEmpty(txtProviderSearch.Text) ? txtProviderSearch.Text.ToFullTextSearch() : null);
            string strTrackerStatus = (!cbxTrackerStatus.SelectedValue.ToString().Equals("-9999") ? cbxTrackerStatus.SelectedValue.ToString() : null);

            string strInquiryCategory = (!cbxInquiryCategory.SelectedValue.ToString().Equals("-9999") ? cbxInquiryCategory.SelectedValue.ToString() : null);
            
            string strInquiryStatus = (!cbxInquiryStatus.SelectedValue.ToString().Equals("-9999") ? cbxInquiryStatus.SelectedValue.ToString() : null);

            if (GlobalObjects.argumentFilterMPINString != null || GlobalObjects.argumentFilterParentIdString != null)
            {
                strPhase = null;
                userIdsCSV = null;
                strStartDate = null;
                strEndDate = null;
                strProviderSearch = null;
                strTrackerStatus = null;
                strInquiryCategory = null;
                strInquiryStatus = null;

            }

            Hashtable htTmp = GlobalObjects.getSelectTrackerRequestSQL(strPhase, userIdsCSV, strStartDate, strEndDate, strProviderSearch, strTrackerStatus, strInquiryCategory, strInquiryStatus, GlobalObjects.argumentFilterMPINString, GlobalObjects.argumentFilterParentIdString);

            return htTmp;
        }


        frmEditTrackingItem _frmEditTrackingItem;
        private void dgvTrackingItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;


            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)//IGNORE BUTTON ROWS
                return;






            string trackingItemId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_id"].Value.ToString();

            //GlobalObjects.inquiryParentGroupList = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectParentGroupSQL, GlobalObjects.htSelectParentGroupSQL(trackingItemId.ToString())).AsEnumerable().Select(r => r.Field<int>("qa_tracker_parent_id")).ToList();

            //GlobalObjects.inquiryCurrentParentGroupId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_group_id"].Value.ToString();

            //if (string.IsNullOrEmpty(GlobalObjects.inquiryCurrentParentGroupId))
            //    GlobalObjects.inquiryCurrentParentGroupId = null;


            //if (GlobalObjects.inquiryCurrentParentGroupId != null)
            //{
            //    object isGrouped = senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value;
            //    GlobalObjects.inquiryCurrentParentIsGrouped = (isGrouped != DBNull.Value ? (bool?)bool.Parse(isGrouped.ToString()) : null);
            //}

            //if (GlobalObjects.inquiryCurrentParentIsGrouped != true)
            //    GlobalObjects.inquiryParentGroupList = new List<int> { int.Parse(trackingItemId.ToString()) };

            //GlobalObjects.inquiryCurrentParentGroupName = senderGrid.Rows[e.RowIndex].Cells["tracker_parent_group_name"].Value.ToString();

            //return trackingItemId.ToString();

            //string trackingItemId = populateGlobalParentGrouping(senderGrid, e);

            GlobalObjects.clearChildGroups();

            GlobalObjects.populateParentGroups(trackingItemId, senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_group_id"].Value.ToString(), senderGrid.Rows[e.RowIndex].Cells["tracker_parent_group_name"].Value.ToString(), senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value.ToString());



            _frmEditTrackingItem = new frmEditTrackingItem(trackingItemId);
            if(!_frmEditTrackingItem.IsDisposed)
                _frmEditTrackingItem.ShowDialog(this);

        }


        private void dgvTrackingItems_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;


            var senderGrid = (DataGridView)sender;
            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)//IGNORE BUTTON ROWS
            {
                string  trackingItemId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_id"].Value.ToString();
                if (senderGrid.Columns[e.ColumnIndex].Name == "Delete")
                {
                    var confirmResult = MessageBox.Show("Delete selected item from the tracker?", "Confirm Delete!", MessageBoxButtons.YesNo); //ALWAYS CONFIRM FIRST
                    if (confirmResult == DialogResult.Yes)
                    {
                        DataTable dtTmp = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getSelectTrackerChildRequestSQL(trackingItemId.ToString()));
                        if (dtTmp.Rows.Count > 0)//CONFIRM AGAIN IF CHILD RECORDS EXIST
                        {
                            confirmResult = MessageBox.Show("This Inquiry has child records. Are you still sure you want to delete these items to the tracker?", "Confirm Delete!", MessageBoxButtons.YesNo);
                            if (confirmResult == DialogResult.No)
                            {
                                return;
                            }
                        }

                        Hashtable htTmp = GlobalObjects.htDeleteTrackerItemSQL(trackingItemId.ToString(), null);
                        DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strDeleteTrackerItemSQL, htTmp);
                        loadDataGridView();

                    }
                    else
                    {
                        return;
                    }
                }
                else if (senderGrid.Columns[e.ColumnIndex].Name == "New Inquiry")
                {
                    //GlobalObjects.clearChildGroups();
                    
                    //populateGlobalParentGrouping(senderGrid, e);
                    GlobalObjects.populateParentGroups(trackingItemId, senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_group_id"].Value.ToString(), senderGrid.Rows[e.RowIndex].Cells["tracker_parent_group_name"].Value.ToString(), senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value.ToString());



                    //MessageBox.Show("INSERT!!!!");

                    string strMPIN = senderGrid.Rows[e.RowIndex].Cells["Provider MPIN"].Value + "";
                    string strProviderName = senderGrid.Rows[e.RowIndex].Cells["Provider Name"].Value + "";
                    string strProjectName = senderGrid.Rows[e.RowIndex].Cells["Project Name"].Value + "";

                    openEditTrackingItemWindow(trackingItemId, strMPIN, strProviderName, strProjectName);
                }
                else if (senderGrid.Columns[e.ColumnIndex].Name == "History")
                {
                    string strTIN = senderGrid.Rows[e.RowIndex].Cells["Provider TIN"].Value.ToString();
                    string strMPIN = senderGrid.Rows[e.RowIndex].Cells["Provider MPIN"].Value.ToString();

                    var form = new frmDetails(strTIN, strMPIN);
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.ShowDialog(this); // if you need non-modal window
                }



            }
            else
                return;
        }

        private void dgvTrackingItems_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvTrackingItems.Rows.Count > 0)
                dgvTrackingItems.Rows[0].Selected = false;
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            loadDataGridView(true);
        }

       private bool _blChecking = false;
        private void clbSelectUser_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_blChecking)
                return;

            if ( ((CheckedListBox)sender).SelectedIndex == 0)
            {
                _blChecking = true;
                for (int i = 0; i < clbSelectUser.Items.Count; i++)
                {
                    clbSelectUser.SetItemChecked(i, (e.NewValue.ToString().Equals("Checked") ? true : false));
                }
                _blChecking = false;

            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exportToExcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files|*.xlsx",
                Title = "Save an Excel File"
            };

            DialogResult result = saveFileDialog.ShowDialog();
            if (result != DialogResult.OK)
                return;

            this.Cursor = Cursors.WaitCursor;
            string strFileName = saveFileDialog.FileName;

            XLWorkbook wb = new XLWorkbook();
            // IXLWorksheet

            DataTable dt = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectTrackerFullRequestSQL, getParameterList());

            wb.Worksheets.Add(dt, "WorksheetName");
            wb.SaveAs(strFileName, true);
            Process.Start(strFileName);

            //MemoryStream fs = new MemoryStream();
            //wb.SaveAs(fs);
            // fs.Position = 0;
            //SpreadsheetDocument doc = SpreadsheetDocument.Open(fs,false);
            //var doc = SpreadSheetDocument
            this.Cursor = Cursors.Default;
        }

        private void addProvidersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _frmAddProvders = new frmAddProvders();
            _frmAddProvders.ShowDialog(this);
        }



        //private string populateGlobalParentGrouping(DataGridView senderGrid, DataGridViewCellEventArgs e)
        //{
        //    GlobalObjects.clearChildGroups();
        //    GlobalObjects.clearParentGroups();

        //    var trackingItemId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_id"].Value;

        //    GlobalObjects.inquiryParentGroupList = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectParentGroupSQL, GlobalObjects.htSelectParentGroupSQL(trackingItemId.ToString())).AsEnumerable().Select(r => r.Field<int>("qa_tracker_parent_id")).ToList();

        //    GlobalObjects.inquiryCurrentParentGroupId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_parent_group_id"].Value.ToString();

        //    if (string.IsNullOrEmpty(GlobalObjects.inquiryCurrentParentGroupId))
        //        GlobalObjects.inquiryCurrentParentGroupId = null;


        //    if (GlobalObjects.inquiryCurrentParentGroupId != null)
        //    {
        //        object isGrouped = senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value;
        //        GlobalObjects.inquiryCurrentParentIsGrouped = (isGrouped != DBNull.Value ? (bool?)bool.Parse(isGrouped.ToString()) : null);
        //    }

        //    if(GlobalObjects.inquiryCurrentParentIsGrouped != true)
        //        GlobalObjects.inquiryParentGroupList =  new List<int> { int.Parse(trackingItemId.ToString()) };

        //    GlobalObjects.inquiryCurrentParentGroupName = senderGrid.Rows[e.RowIndex].Cells["tracker_parent_group_name"].Value.ToString();

        //    return trackingItemId.ToString();
        //}


        private void openEditTrackingItemWindow(string trackingItemId, string strMPIN, string strProviderName, string strProjectName)
        {

            var form = new frmTrackingItem();
            form.btnInsertTrackingChild.Text = "Add Tracking Item";
            form.strTrackingChildItemId = null;
            form.strTrackingItemId = trackingItemId; 

            form.lblMPINDisplay.Text = strMPIN;
            form.lblProviderNameDisplay.Text = strProviderName;
            form.lblProjectNameDisplay.Text = strProjectName;
            //form.lblProviderSpecialtyDisplay.Text = this.lblProviderSpecialtyDisplay.Text;

            form.Text = "Add Tracking Item";


            //form.Parent = this.form;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need non-modal window
        }



        private void clearFilters()
        {
           
            SharedWinFormFunctions.checkUncheckCheckBoxList(ref clbSelectUser, true);
            cmbPhase.SelectedIndex = 0;
            cbxInquiryCategory.SelectedIndex = 0;
            cbxInquiryStatus.SelectedIndex = 0;
            cbxTrackerStatus.SelectedIndex = 0;
            txtProviderSearch.Text = "";
            dtpStartDate.Value = DateTime.Today;
            dtpStartDate.Checked = false;
            dtpEndDate.Value = DateTime.Today;
            dtpEndDate.Checked = false;
            loadDataGridView();

        }



        private void btnClearFilters_Click(object sender, EventArgs e)
        {
            clearFilters();
        }

        //private void clearLinkFilterToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    GlobalObjects.argumentFilterMPINString = null;
        //    GlobalObjects.argumentFilterParentIdString = null;
        //   clearLinkFilterToolStripMenuItem.Visible = false;
        //    loadDataGridView(true);

        //}

        private void memberDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmPatientDetailGenerator();
            form.Owner = ParentForm; //TEST 1172017
            form.strMPIN = null;
            form.strProject = null;
            //form.Parent = this.form;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need no
        }

        private void excelParserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmExcelParser();
            form.Owner = ParentForm; //TEST 1172017
            //form.Parent = this.form;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need no
        }





        private void CopyClick(object sender, EventArgs e)
        {
            if (ActiveCell != null && ActiveCell.Value != null)
                Clipboard.SetText(ActiveCell.Value.ToString());
        }

        private void dgvTrackingItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Windows.Forms.DataGridView.HitTestInfo hittestinfo = dgvTrackingItems.HitTest(e.X, e.Y);

                if (hittestinfo != null && hittestinfo.Type == DataGridViewHitTestType.Cell)
                {
                    ActiveCell = dgvTrackingItems[hittestinfo.ColumnIndex, hittestinfo.RowIndex];
                    ActiveCell.Selected = true;
                    contextMenu.Show(dgvTrackingItems, new Point(e.X, e.Y));
                }

            }
        }

        private void qACompanionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmQACompanion();
            form.Owner = ParentForm; 
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need no
        }

        private void aPRDRGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmAPRDRG();
            form.Owner = ParentForm; //TEST 1172017
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need no
        }

        private void dXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
               
                var form = new frmDX();
                form.Owner = ParentForm; //TEST 1172017
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this); // if you need no
            }
            catch
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }


        }

        private void pXToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.Cursor = Cursors.WaitCursor;
            try
            {
                var form = new frmPX();
                form.Owner = ParentForm; //TEST 1172017
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this); // if you need no
            }
            catch
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}
