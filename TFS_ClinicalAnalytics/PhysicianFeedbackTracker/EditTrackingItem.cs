using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


//truncate table dbo.qa_tracker_children;
//truncate table dbo.qa_tracker_children_group;

//truncate table dbo.qa_tracker_parent;
//truncate table dbo.qa_tracker_parent_group;


//PEI....AND Inquiry Status = 'Pending MMD Action'

//Add Inquiry status filter to main



namespace PhysicianFeedbackTracker
{
    public partial class frmEditTrackingItem : _BaseClass
    {
        private string _strTrackingItemId;
        private string _strTrackingChildItemId;

        private bool _blEditMode = false;

        Color _clrHightlight = Color.LightPink;


        private bool? blIsGroupedInitially = false;

        DataGridViewCell ActiveCell = null;
        ContextMenu contextMenu = new ContextMenu();

        public frmEditTrackingItem(string strTrackingItemId)
        {
            // if (!ActiveDirectoryFunctions.hasAccess(GlobalObjects.strCurrentUser, "pei2_readonly") && !ActiveDirectoryFunctions.hasAccess(GlobalObjects.strCurrentUser, "pei2_users"))
            //if (!ActiveDirectoryFunctions.hasAccess(GlobalObjects.strCurrentUser, "pei2_readonly"))
            //{
            //    MessageBox.Show("Get Out!!!!");
            //    this.Close();

            //    //TAKE THEM SOMEWHERE ELSE
            //    return;
            //}


            _strTrackingItemId = strTrackingItemId;
            InitializeComponent();

            contextMenu.MenuItems.Add(new MenuItem("Copy", CopyClick));
            //if (GlobalObjects.inquiryCurrentParentGroupId != null)
            //{
            //    chkIsGrouped.Checked = true;
            //    chkIsGrouped.Text = chkIsGrouped.Text + " = " + GlobalObjects.inquiryCurrentParentGroupName;
            //} 
            //else
            //    chkIsGrouped.Visible = false;

            if (GlobalObjects.inquiryCurrentParentIsGrouped == true)
            {
                chkIsGrouped.Checked = (bool)GlobalObjects.inquiryCurrentParentIsGrouped;
                //chkIsGrouped.Text = chkIsGrouped.Text + " = " + GlobalObjects.inquiryCurrentParentGroupName;
                chkIsGrouped.Text = chkIsGrouped.Text + " = " + GlobalObjects.inquiryCurrentParentGroupName.Replace("_", " ");
                chkIsGrouped.Visible = true;


                blIsGroupedInitially = chkIsGrouped.Checked;
            }
            else
                chkIsGrouped.Visible = false;




            //QA TOOLS SECTION
            //if (GlobalObjects.getNameValueDataTable("users").Select("filter ='QA' AND  value ='" + GlobalObjects.strCurrentUser + "'").Count() > 0)
            //    detailsToolStripMenuItem.Visible = true;
            //else
            //    detailsToolStripMenuItem.Visible = false;

            detailsToolStripMenuItem.Visible = SharedFunctions.hasAccess(GlobalObjects.strCurrentUser, "QA");




            populateMainFields();
            populateGrid();

        }

        string _strURL = null;
        private List<string> _strFilesList;
        private string _strPEIProjectName;
        private string _strParentMPIN;
        private void populateMainFields()
        {

            DataTable dtTmp = GlobalObjects.getNameValueDataTable("user_groups");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Choose Role--", "-9999", dtTmp);
            cbxRequesterRole.DataSource = dtTmp;
            cbxRequesterRole.DisplayMember = "name";
            cbxRequesterRole.ValueMember = "value";


             dtTmp = GlobalObjects.getNameValueDataTable("source_of_inquiry");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cbxSourceOfInquiry.DataSource = dtTmp;
            cbxSourceOfInquiry.DisplayMember = "name";
            cbxSourceOfInquiry.ValueMember = "value";

            DataRow[] drTmpArr = GlobalObjects.dtTrackingParentCache.Select("qa_tracker_parent_id =" + _strTrackingItemId);


            //CHRIS ADDED THIS NEW LOGIC 1192017
            if (drTmpArr.Count() == 0)
            {
                //SELECT FROM ILUCA WHERE ID = _strTrackingItemId;
                Hashtable ht = new Hashtable();
                ht.Add("@qa_tracker_parent_id", _strTrackingItemId);
                drTmpArr = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectTrackerRequestSQL, ht).Select();
            }





            foreach (DataRow dr in drTmpArr)
            {
                _strPEIProjectName = dr["pei_project_name"].ToString();
                _strParentMPIN = dr["practice_mpin"].ToString();

                lblMPINDisplay.Text = dr["Provider MPIN"].ToString();
                lblTINDisplay.Text = dr["Provider TIN"].ToString();
                lblProviderNameDisplay.Text = dr["Provider Name"].ToString();
                lblProviderCityDisplay.Text = dr["Provider City"].ToString();
                lblProviderStateDisplay.Text = dr["Provider State"].ToString();
                lblProviderSpecialtyDisplay.Text = dr["Provider Specialty"].ToString();
                lblDateLoggedDisplay.Text = DateTime.Parse(dr["Added Date"].ToString()).ToShortDateString();
                lblProjectNameDisplay.Text = dr["Project Name"].ToString();
                lblMarketDisplay.Text = dr["Provider Market"].ToString();
                lblGroupPracticeNameDisplay.Text = dr["Practice Name"].ToString();
                lblOriginatorNameDisplay.Text = dr["Added By"].ToString();
                lblMMDDisplay.Text = dr["Provider MMD"].ToString();
                
                if (dr["Requester Name"] != DBNull.Value)
                {
                    txtRequesterName.Text = dr["Requester Name"].ToString();
                }

                if (dr["Requester Email"] != DBNull.Value)
                {
                    txtRequesterEmail.Text = dr["Requester Email"].ToString();
                }


                if (dr["Requester Phone"] != DBNull.Value)
                {
                    txtRequesterPhone.Text = dr["Requester Phone"].ToString();
                }


                if (dr["Requester Date"] != DBNull.Value)
                {
                    dtpRequesterDate.Value = DateTime.Parse(dr["Requester Date"].ToString());
                    dtpRequesterDate.Checked = true;
                }


                if (dr["Requester Role"] != DBNull.Value)
                {
                    cbxRequesterRole.Text = dr["Requester Role"].ToString();
                }

                if (dr["Source of Inquiry"] != DBNull.Value)
                {
                    cbxSourceOfInquiry.Text = dr["Source of Inquiry"].ToString();
                }



            }

        }

        private DataTable _dtTrackingChildGlobal;
        public void populateGrid()
        {
            _dtTrackingChildGlobal = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getSelectTrackerChildRequestSQL(_strTrackingItemId));
            dgvInquiryListing.DataSource = _dtTrackingChildGlobal;
            SharedWinFormFunctions.hideColumnsInDataGridView(ref dgvInquiryListing, GlobalObjects.strTrackerChildRequestHideArr);
            SharedWinFormFunctions.addButtonColumnToDataGridView(ref dgvInquiryListing, "Delete", Color.Red, Color.White, 0, false);
            dgvInquiryListing.AutoResizeColumns();
            dgvInquiryListing.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvInquiryListing.DefaultCellStyle.WrapMode = DataGridViewTriState.True;


            dgvInquiryListing.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvInquiryListing.Columns["Notes"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dgvInquiryListing.Columns["Analytic Notes"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dgvInquiryListing.ClearSelection();

        }





        private void dgvInquiryListing_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_blEditMode == true)
                if (!continueAction("You have not saved your update. Continue anyway?"))
                    return;

            _blEditMode = false;


            if (e.RowIndex == -1)
                return;
       
            var senderGrid = (DataGridView)sender;
            var trackingItemId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_child_id"].Value;



            if (e.ColumnIndex != -1)
            {
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)//IGNORE BUTTON ROWS
                {
                    var confirmResult = MessageBox.Show("Delete selected item from the tracker?", "Confirm Delete!", MessageBoxButtons.YesNo); //ALWAYS CONFIRM FIRST
                    if (confirmResult == DialogResult.Yes)
                    {
                        Hashtable htTmp = GlobalObjects.htDeleteTrackerItemSQL(_strTrackingItemId, trackingItemId.ToString());
                        DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strDeleteTrackerItemSQL, htTmp);
                        _strTrackingChildItemId = null;

                        populateGrid();

                    }
                  
                    return;
                }
            }


            //_strTrackingChildItemId = trackingItemId.ToString();
            //changeMode(true);
            //populateInputFieldsWithData();

        }

        private void dgvInquiryListing_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if(dgvInquiryListing.Rows.Count > 0 )
                dgvInquiryListing.Rows[0].Selected = false;
        }




        //private void changeMode(bool isEdit)
        //{
        //    if(isEdit)
        //    {
        //        //lblStatus.Text = "Update Existing Inquiry # " + _strTrackingChildItemId;
        //        btnInsertTrackingChild.Text = "Update Existing Inquiry";
        //        this.BackColor = Color.LightBlue;
        //    }
        //    else
        //    {
        //        //lblStatus.Text = "Add New Inquiry";
        //        btnInsertTrackingChild.Text = "Add New Inquiry";
        //        this.BackColor = Color.Khaki;
        //    }
        //}



        private bool blRefreshParentTable = false;
        private void btnExit_Click(object sender, EventArgs e)
        {
            exit();
        }


        private void exit()
        {
            //Form frmSelectTrackingItem = Application.OpenForms["frmSelectTrackingItem"];
            //if (frmSelectTrackingItem != null)
            //    ((frmSelectTrackingItem)frmSelectTrackingItem).loadDataGridView(blRefreshParentTable);

            refreshParent(blRefreshParentTable);

            this.Close();
        }


        private void btnUpdateParentRequest_Click(object sender, EventArgs e)
        {

            Int16 intInsertCount = 0;
            //////MOVED FROM  CHK_CLICK 7312017 START
            //////MOVED FROM  CHK_CLICK 7312017 START
            //////MOVED FROM  CHK_CLICK 7312017 START
            GlobalObjects.inquiryCurrentParentIsGrouped = (bool)chkIsGrouped.Checked;

            if (blIsGroupedInitially != GlobalObjects.inquiryCurrentParentIsGrouped)
            {
                //UPDATE CURRENT PARENT GROUPED STATUS
                Hashtable htTmp = GlobalObjects.htUpdateParentGroupSQL(_strTrackingItemId, GlobalObjects.inquiryCurrentParentGroupId, (bool)GlobalObjects.inquiryCurrentParentIsGrouped);
                DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strUpdateParentGroupSQL, htTmp);

                populateGrid();
                blIsGroupedInitially = GlobalObjects.inquiryCurrentParentIsGrouped;
                refreshParent(true);
            }

            if (chkIsGrouped.Checked == false)
            {
                GlobalObjects.clearParentGroups();
                GlobalObjects.clearChildGroups();
            }
            //////MOVED FROM  CHK_CLICK 7312017 END
            //////MOVED FROM  CHK_CLICK 7312017 END
            //////MOVED FROM  CHK_CLICK 7312017 END



            string str_qa_tracker_parent_id = _strTrackingItemId;
            string str_requester_name = (string.IsNullOrEmpty(txtRequesterName.Text) ? null : txtRequesterName.Text.Trim());
            string str_requester_email = (string.IsNullOrEmpty(txtRequesterEmail.Text) ? null : txtRequesterEmail.Text.Trim());
            string str_requester_phone = (string.IsNullOrEmpty(txtRequesterPhone.Text) ? null : txtRequesterPhone.Text.Trim());
            string str_requester_date = (dtpRequesterDate.Checked == false ? null : dtpRequesterDate.Value.ToShortDateString());
            string str_requester_role = (cbxRequesterRole.SelectedValue.ToString().Equals("-9999") ? null : cbxRequesterRole.SelectedValue.ToString());
            string str_source_of_inquiry_id = (cbxSourceOfInquiry.SelectedValue.ToString().Equals("-9999") ? null : cbxSourceOfInquiry.SelectedValue.ToString());
            string str_updated_by_nt_id = GlobalObjects.strCurrentUser;

           // string strIsGrouped  = (chkIsGrouped.Visible == false ? null : chkIsGrouped.Checked.ToString());
            string strIsGrouped = null;
            if (GlobalObjects.inquiryCurrentParentIsGrouped != null)
            {
                strIsGrouped = (GlobalObjects.inquiryCurrentParentIsGrouped == false ? "0" : "1");
            }

            Hashtable ht = null;

            List <int> inquiryItems = GlobalObjects.inquiryParentGroupList;
            //if(!chkIsGrouped.Visible || !chkIsGrouped.Checked)
            //{
            //    inquiryItems = new List<int> { int.Parse(str_qa_tracker_parent_id) };
            //}
            if (GlobalObjects.inquiryCurrentParentIsGrouped != true)
            {
                inquiryItems = new List<int> { int.Parse(str_qa_tracker_parent_id) };
            }


            foreach (int intId in inquiryItems)
            {

                ht = GlobalObjects.htUpdateParentRequestSQL(intId.ToString(), str_requester_name, str_requester_email, str_requester_phone, str_requester_date, str_requester_role, str_source_of_inquiry_id, str_updated_by_nt_id, strIsGrouped, (GlobalObjects.inquiryCurrentParentGroupId == null ? null :GlobalObjects.inquiryCurrentParentGroupId.ToString()), str_qa_tracker_parent_id);

                object objResults = DBConnection.getMSSQLExecuteScalarSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strUpdateParentRequestSQL, ht);
                intInsertCount += 1;


                if (objResults + "" == "1")
                {
                    blRefreshParentTable = true;

                    if(intInsertCount == inquiryItems.Count)
                    {
                        MessageBox.Show("Request Updated!", "Success");
                    }
                    
                }

                //strIsGrouped = null;
            }


            blIsGroupedInitially = chkIsGrouped.Checked;


            //if(strIsGrouped != "1")
            //{
            //    GlobalObjects.clearParentGroups();

            //    if(strIsGrouped == "0")
            //    {

            //    }
            //}




        }



        //SORTING/SELECTION SOLUTION START
        //SORTING/SELECTION SOLUTION START
        //SORTING/SELECTION SOLUTION START
        string strSelectedRow;
        private void dgvInquiryListing_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1 && dgvInquiryListing.SelectedRows.Count > 0)
            {
                strSelectedRow = dgvInquiryListing.SelectedRows[0].Cells["qa_tracker_child_id"].Value.ToString();
            }
        }

        private void dgvInquiryListing_Sorted(object sender, EventArgs e)
        {

            dgvInquiryListing.ClearSelection();
            foreach (DataGridViewRow xRow in dgvInquiryListing.Rows)
            {
                if (xRow.Cells["qa_tracker_child_id"].Value.ToString().Equals(strSelectedRow))
                {
                    dgvInquiryListing.CurrentCell = xRow.Cells[4];
                    //Line Found. No need to loop through the rest.
                    break; // TODO: might not be correct. Was : Exit For
                }
            }

        }
        //SORTING/SELECTION SOLUTION END
        //SORTING/SELECTION SOLUTION END
        //SORTING/SELECTION SOLUTION END


        private void frmEditTrackingItem_Shown(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(getPEILinks));
            t.Start();
        }


        private string _strPEIEngagementLinkError = null;
        private string _strPEIFileLinkError = null;
        private void getPEILinks()
        {
            try
            {
                _strPEIEngagementLinkError = null;
                _strURL = SharedFunctions.getPEIEngagementLink(lblMPINDisplay.Text, _strPEIProjectName);
            }
            catch (Exception ex)
            {
                _strURL = null;
                _strPEIEngagementLinkError = "PEI ENGAGEMENT LINK ERROR: " + ex.ToString();
            }

            try
            {
                _strPEIFileLinkError = null;
                _strFilesList = SharedFunctions.getPEIFileLinks(lblMPINDisplay.Text, lblTINDisplay.Text, _strPEIProjectName, _strParentMPIN);
            }
            catch (Exception ex)
            {
                _strFilesList = new List<string>();
                _strPEIFileLinkError = "PEI FILE LINK ERROR: " + ex.ToString();
            }
            

            AccessControl();
        }



        private void AccessControl()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(AccessControl));
            }
            else
            {

                if (_strURL != null || _strPEIEngagementLinkError != null)
                    viewEngagementToolStripMenuItem.Enabled = true;
                else
                    viewEngagementToolStripMenuItem.Enabled = false;


                if (_strFilesList.Count > 0 || _strPEIFileLinkError != null)
                    viewProfilesToolStripMenuItem.Enabled = true;
                else
                    viewProfilesToolStripMenuItem.Enabled = false;



            }
        }


        private void dgvInquiryListing_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //if (e.ColumnIndex == this.dgvInquiryListing.Columns["Date Resolved"].Index)
            //{
            //    if (e.Value.ToString().Equals(""))
            //    {
            //        this.dgvInquiryListing.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Bold);

            //    }
            //    else
            //    {
            //        this.dgvInquiryListing.Rows[e.RowIndex].DefaultCellStyle.Font = new Font(e.CellStyle.Font, FontStyle.Regular);
            //    }

            //}
        }


        private void dgvInquiryListing_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {


            if (e.RowIndex == -1 || e.ColumnIndex < 1)
                return;
 
            if (this.dgvInquiryListing.Rows[e.RowIndex].Cells["Date Resolved"].Value.ToString().Equals(""))
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);
                using (Pen p = new Pen(Color.Purple,2))
                {
                    Rectangle rect = e.CellBounds;
                    rect.Width -= 2;
                    rect.Height -= 2;
                    e.Graphics.DrawRectangle(p, rect);
                }
                e.Handled = true;

            }

        }

        private void addInquiryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GlobalObjects.clearChildGroups();


            _strTrackingChildItemId = null;
            dgvInquiryListing.ClearSelection();


            openEditTrackingItemWindow(_strTrackingItemId, _strTrackingChildItemId, null, "Create New Tracking Item");

        }

        private void viewEngagementToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if ( _strPEIEngagementLinkError != null)
            {
                MessageBox.Show(_strPEIEngagementLinkError);
            }
            else
            {
                System.Diagnostics.Process.Start(_strURL);
            }

        }

        private void viewProfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_strPEIFileLinkError != null)
            {
                MessageBox.Show(_strPEIFileLinkError);
            }
            else
            {
                var form = new frmLinks();
                form.populateList(_strFilesList);
                form.ShowDialog(this); // if you need non-modal window
            }
        }

        private void addEngagementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow[] drMMDRelated = _dtTrackingChildGlobal.Select("[Inquiry Category] = 'MMD Follow-up Request' AND [Entered in PEI] <> 'Yes' AND [Inquiry Status] = 'Pending MMD Action' ");

            if (drMMDRelated.Count() <= 0)
            {
                MessageBox.Show("No valid Inquiries for PEI!");
                return;
            }

            //PASS TO ALL CHILDREN DATA 10172017
            string strCSVChildIds = string.Join(",", drMMDRelated.AsEnumerable().Select(r => r.Field<int>("qa_tracker_child_id")).ToArray());


            var form = new frmAddEngagementToPEI();
            form.populateFields(strCSVChildIds);
            //form.populateFields(lblMPINDisplay.Text, _strPEIProjectName, lblProviderNameDisplay.Text, lblMMDDisplay.Text, drMMDRelated.CopyToDataTable(), strCSVChildIds);
            form.ShowDialog(this); // if you need non-modal window
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_blEditMode == true)
                if (!continueAction("You have not saved your update. Continue anyway?"))
                    return;

            exit();
        }



         private bool continueAction(string strMessage)
        {
            bool blContinue = false;
            var confirmResult = MessageBox.Show(strMessage, "Continue", MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                blContinue = true;
            }

            return blContinue;
        }

        private void dgvInquiryListing_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

        
            GlobalObjects.clearChildGroups();

            var senderGrid = (DataGridView)sender;
            var trackingItemId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_child_id"].Value;
            var childGroupId = senderGrid.Rows[e.RowIndex].Cells["qa_tracker_child_group_id"].Value;
            var childGroupName = senderGrid.Rows[e.RowIndex].Cells["tracker_child_group_name"].Value;
            object isGrouped = senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value;
            //object isGrouped = (senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value != null ? bool.Parse(senderGrid.Rows[e.RowIndex].Cells["is_grouped"].Value.ToString()) : false);

            if (e.ColumnIndex != -1)
            {
                if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)//IGNORE BUTTON ROWS
                {
                    var confirmResult = MessageBox.Show("Delete selected item from the tracker?", "Confirm Delete!", MessageBoxButtons.YesNo); //ALWAYS CONFIRM FIRST
                    if (confirmResult == DialogResult.Yes)
                    {
                        Hashtable htTmp = GlobalObjects.htDeleteTrackerItemSQL(_strTrackingItemId, trackingItemId.ToString());
                        DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strDeleteTrackerItemSQL, htTmp);
                        _strTrackingChildItemId = null;

                        populateGrid();
                        //changeMode(false);
                        //clearInputFields();

                    }

                    return;
                }
            }


            if(isGrouped != DBNull.Value)
            {
                //GlobalObjects.inquiryCurrenChildGroupId = childGroupId.ToString();
                //GlobalObjects.inquiryCurrentChildGroupName = childGroupName.ToString();
                //GlobalObjects.inquiryCurrentParentIsGrouped = bool.Parse(isGrouped.ToString());

                GlobalObjects.populateChildGroups(trackingItemId.ToString(), childGroupId.ToString(), childGroupName.ToString(), isGrouped.ToString());

            }


            _strTrackingChildItemId = trackingItemId.ToString();


            openEditTrackingItemWindow(_strTrackingItemId, _strTrackingChildItemId, _dtTrackingChildGlobal.Select("qa_tracker_child_id = " + _strTrackingChildItemId)[0],  "Update Tracking Item #" + GlobalObjects.inquiryCurrenChildGroupId);

        }



        private void openEditTrackingItemWindow(string strTrackingItemId, string strTrackingChildItemId, DataRow dr, string strCaption)
        {

            var form = new frmTrackingItem();
            form.btnInsertTrackingChild.Text = "Add Tracking Item";


            //List<int> inquiryList  = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectParentGroupSQL, GlobalObjects.htSelectParentGroupSQL(_strTrackingItemId)).AsEnumerable().Select(r => r.Field<int>("qa_tracker_parent_id")).ToList();


            //if (inquiryList.Count < 1 || chkIsGrouped.Checked == false)
            //    GlobalObjects.inquiryParentGroupList = new List<int> { int.Parse(_strTrackingItemId) };
            //else
            //    GlobalObjects.inquiryParentGroupList = inquiryList;


            form.strTrackingItemId = strTrackingItemId;
            form.strTrackingChildItemId = _strTrackingChildItemId;
            form.drTrackingChild = dr;


            form.lblMPINDisplay.Text = this.lblMPINDisplay.Text;
            form.lblProviderNameDisplay.Text = this.lblProviderNameDisplay.Text;
            form.lblProjectNameDisplay.Text = this.lblProjectNameDisplay.Text;
            form.lblProviderSpecialtyDisplay.Text = this.lblProviderSpecialtyDisplay.Text;


            //if (isGrouped == true && chkIsGrouped.Checked == true)
            //{
            //    form.chkIsGroup.Text = "Is Grouped = " + GlobalObjects.inquiryCurrentChildGroupName;
            //    form.chkIsGroup.Checked = true;
            //}
            //else
            //{
            //    form.chkIsGroup.Visible = false;
            //}


            if (GlobalObjects.inquiryCurrentParentIsGrouped == true && GlobalObjects.inquiryCurrentChildIsGrouped != null)
            {
                form.chkIsGroup.Checked = (bool)GlobalObjects.inquiryCurrentChildIsGrouped;
                form.chkIsGroup.Enabled = (bool)GlobalObjects.inquiryCurrentChildIsGrouped;
                form.chkIsGroup.Text = "Is Grouped = " + GlobalObjects.inquiryCurrentChildGroupName;
               // form.chkIsGroup.Visible = true;
            }
            else
            {
               // form.chkIsGroup.Visible = false;
            }
            





            form.Text = strCaption;
            if (dr != null)
            {
                form.populateInputFieldsWithData();
                form.btnInsertTrackingChild.Text = "Update Tracking Item";
            }

            //form.Parent = this.form;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need non-modal window


            if (chkIsGrouped.Checked == false)
            {
                //GlobalObjects.clearChildGroups();
                GlobalObjects.clearParentGroups();
            }

        }





        private void chkIsGrouped_Click(object sender, EventArgs e)
        {

            //MessageBox.Show(chkIsGrouped.Text);

            //GlobalObjects.inquiryCurrentParentIsGrouped = (bool)chkIsGrouped.Checked;

            //if (blIsGroupedInitially != GlobalObjects.inquiryCurrentParentIsGrouped)
            //{
            //    //UPDATE CURRENT PARENT GROUPED STATUS
            //    Hashtable htTmp = GlobalObjects.htUpdateParentGroupSQL(_strTrackingItemId, GlobalObjects.inquiryCurrentParentGroupId, (bool)GlobalObjects.inquiryCurrentParentIsGrouped);
            //    DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strUpdateParentGroupSQL, htTmp);

            //    populateGrid();
            //    blIsGroupedInitially = GlobalObjects.inquiryCurrentParentIsGrouped;
            //    refreshParent(true);
            //}

            //if(chkIsGrouped.Checked == false)
            //{
            //    GlobalObjects.clearParentGroups();
            //    GlobalObjects.clearChildGroups();
            //}
        }


        private void refreshParent(bool blRefreshDataTable)
        {
            Form frmSelectTrackingItem = Application.OpenForms["frmSelectTrackingItem"];
            if (frmSelectTrackingItem != null)
                ((frmSelectTrackingItem)frmSelectTrackingItem).loadDataGridView(blRefreshDataTable);
        }

        private void historyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(lblTINDisplay.Text))
            {
                return;
            }

            var form = new frmDetails(lblTINDisplay.Text, lblMPINDisplay.Text);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need non-modal window
        }



        private string getPath(bool blWrite = true)
        {
            string strFinalPath = GlobalObjects.strUploadDocumentsPath;


            if (GlobalObjects.inquiryCurrentParentGroupId != null && GlobalObjects.inquiryCurrentParentIsGrouped == true)
            {
                strFinalPath = strFinalPath + "\\grptrk_" + GlobalObjects.inquiryCurrentParentGroupId;
            }
            else
            {
                strFinalPath = strFinalPath + "\\indtrk_" + _strTrackingItemId;
            }

            if (!Directory.Exists(strFinalPath))
            {
                if (blWrite)
                    Directory.CreateDirectory(strFinalPath);
                else
                    return null;
                    
            }

            return strFinalPath;
        }


        private void uploadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                string strFinalPath = getPath(true);
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "All files | *.*"; // file types, that will be allowed to upload
                dialog.Multiselect = true; // allow/deny user to upload more than one file at a time
                if (dialog.ShowDialog() == DialogResult.OK) // if user clicked OK
                {

                    dialog.FileNames.ToList().ForEach(file => {
                        System.IO.File.Copy(file, System.IO.Path.Combine(strFinalPath, System.IO.Path.GetFileName(file)));
                    });
                }


                MessageBox.Show(dialog.FileNames.ToList().Count + " files uploaded");
            }
            catch
            {

            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void viewFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string strPath = getPath(false);


            if (strPath == null)
            {
                MessageBox.Show("No files have been uploaded yet!");
                return;
            }


            var form = new frmUploadedFiles(strPath);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need non-modal window
        }

        private void memberDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new frmPatientDetailGenerator();
            form.Owner = ParentForm; //TEST 1172017
            form.strMPIN = lblMPINDisplay.Text;
            form.strProject = lblProjectNameDisplay.Text;
            //form.Parent = this.form;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need no
        }




        private void CopyClick(object sender, EventArgs e)
        {
            if (ActiveCell != null && ActiveCell.Value != null)
                Clipboard.SetText(ActiveCell.Value.ToString());
        }

        private void dgvInquiryListing_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                System.Windows.Forms.DataGridView.HitTestInfo hittestinfo = dgvInquiryListing.HitTest(e.X, e.Y);

                if (hittestinfo != null && hittestinfo.Type == DataGridViewHitTestType.Cell)
                {
                    ActiveCell = dgvInquiryListing[hittestinfo.ColumnIndex, hittestinfo.RowIndex];
                    ActiveCell.Selected = true;
                    contextMenu.Show(dgvInquiryListing, new Point(e.X, e.Y));
                }

            }
        }
    }
}
