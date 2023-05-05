using System;
using System.Collections;
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
    public partial class frmAddProvders : _BaseClass
    {
        public frmAddProvders()
        {
            InitializeComponent();

        }


        string _strCurrentMPIN;
        string _strCurrentProviderName;
 

        private void AddProvders_Load(object sender, EventArgs e)
        {

            DataTable dtTmp = GlobalObjects.getNameValueDataTable("phase");
            cmbPhase.DataSource = dtTmp;
            cmbPhase.DisplayMember = "name"; 
            cmbPhase.ValueMember = "value";

            cmbPhase.SelectedIndex = 2;


        }

        private void btnSearchProvider_Click(object sender, EventArgs e)
        {
            searchProvider();

        }

        private void lvTinSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                radDeselectAll.Checked = false;
                radSelectAll.Checked = false;

                if (lvTinSearch.SelectedItems.Count == 0)
                return;

                ListViewItem item = lvTinSearch.SelectedItems[0];
                string strTIN = item.Text;
                string strPhase = cmbPhase.SelectedValue.ToString();

                // MessageBox.Show(strTIN);

                Hashtable ht = GlobalObjects.htProviderSearchSQL(strTIN, strPhase, "TIN");
                DataTable dtSearchResults = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strGetProviderSearchSQL, ht);

                DataRow[] dr = dtSearchResults.Select("type='MPIN'");

                if (dr.Count() > 0)
                {
                    _strCurrentMPIN = dr[0]["identifier"].ToString();
                    _strCurrentProviderName = dr[0]["name"].ToString();


                    SharedWinFormFunctions.addDataTableToListView(ref lvProviderResults, dr.CopyToDataTable(), GlobalObjects.strGetProviderSearchExcludeArr, blMultiSelect: true,  blCheckbox: true);
                }
                else
                {
                    // MessageBox.Show("No Results Found", "No Results", MessageBoxButtons.OK);
                    lvProviderResults.Clear();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cursor = Cursors.Default;
            }


        }

        private void lvProviderResults_MouseClick(object sender, MouseEventArgs e)
        {
            var where = lvProviderResults.HitTest(e.Location);
            if (where.Location == ListViewHitTestLocations.Label)
            {
                where.Item.Checked = !where.Item.Checked;
            }
        }

        private void radSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            SharedWinFormFunctions.checkUncheckListView(ref lvProviderResults, true);
        }

        private void radDeselectAll_CheckedChanged(object sender, EventArgs e)
        {
            SharedWinFormFunctions.checkUncheckListView(ref lvProviderResults, false);
        }


        private void btnSumbitProviders_Click(object sender, EventArgs e)
        {

            if(lvProviderResults.Items.Count <=0)
            {
                MessageBox.Show("You must select at least one provider to submit");
                return;
            }



            GlobalObjects.clearParentGroups();
            GlobalObjects.clearChildGroups();

            DataTable dtResults;
            string strGroupId = null;


            //CONFIRMATION SECTION
            DialogResult confirmResult = DialogResult.None;
            DataTable dtDuplicates =  checkForExistingInquiries();
            if(dtDuplicates.Rows.Count > 0)
            {

                using (var form = new frmDuplicateConfirmation())
                {
                    form.dgvDuplicateProviders.DataSource = dtDuplicates;
                    form.dgvDuplicateProviders.AutoResizeColumns();
                    form.dgvDuplicateProviders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    form.StartPosition = FormStartPosition.CenterParent;
                    confirmResult = form.ShowDialog(this);
                }

            }

            if(confirmResult == DialogResult.None)
                confirmResult = MessageBox.Show("Add selected items to the tracker?", "Confirm Inserts!", MessageBoxButtons.YesNo);
            

            if (confirmResult == DialogResult.Yes)
            {
                StringBuilder sb  = new StringBuilder();
                StringBuilder sbName = new StringBuilder();
                string strGroupName = null;

                for (int i = 0; i < lvProviderResults.Items.Count; i++)
                {
                    if (lvProviderResults.Items[i].Checked)
                    {

                        sb.Append(GlobalObjects.getSelectUnionSQL(lvProviderResults.Items[i].Text + ", ''" + cmbPhase.SelectedValue + "'', ''" + GlobalObjects.strCurrentUser + "''"));

                        if(sbName.Length == 0)
                        {
                            sbName.Append("ParentGroup_" + cmbPhase.Text.Trim().Replace(" ", "_") + "_" );
                        }

                        sbName.Append(lvProviderResults.Items[i].SubItems[0].Text + "_");
                    }
                }

                //MessageBox.Show(sb.ToString());

                if (sb.Length == 0)
                {
                    MessageBox.Show("You must select at least one provider", "Warning");
                }
                else
                {
                    List<int> inquiryList = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString, GlobalObjects.getBulkInsertProvidersToTrackerSQL(sb.ToString())).AsEnumerable().Select(r => r.Field<int>("ID")).ToList();
                    int intCnt = inquiryList.Count;


                    string strTrackerId = null;
                    //NO CLONE SO REMOVE THE LOOPING ITEMS
                    if (chkCloneItems.Checked == false)
                    {
                        strTrackerId = inquiryList[0].ToString();
                    }
                    else
                    {

                        strGroupName = sbName.ToString().TrimEnd('_');
                        strGroupId = DBConnection.getMSSQLExecuteScalar(GlobalObjects.strILUCAConnectionString, GlobalObjects.getBulkUpdateProvidersGroupSQL(String.Join(",", inquiryList.Select(x => x.ToString()).ToArray()), strGroupName)).ToString();

                        GlobalObjects.clearChildGroups();
                        GlobalObjects.populateParentGroups(null, strGroupId, strGroupName, "true");
                        GlobalObjects.inquiryParentGroupList = inquiryList;

                    }


                    //GlobalObjects.inquiryCurrentParentIsGrouped = true;
                    //GlobalObjects.inquiryParentGroupList = inquiryList;
                    //GlobalObjects.inquiryCurrentParentGroupId = strGroupId;
                    //GlobalObjects.inquiryCurrentParentGroupName = strGroupName;




                    //CHRIS UPDATES 1092017
                    //openTrackingItemWindow((GlobalObjects.inquiryParentGroupList == null ? strTrackerId : null));
                    openEditTrackingItemWindow((GlobalObjects.inquiryParentGroupList == null ? strTrackerId : GlobalObjects.inquiryParentGroupList[0].ToString()));



                    MessageBox.Show(intCnt + " records inserted", "Success");

                    Form frmSelectTrackingItem = Application.OpenForms["frmSelectTrackingItem"];
                    if(frmSelectTrackingItem != null)
                        ((frmSelectTrackingItem)frmSelectTrackingItem).loadDataGridView();

                }

            }
            else
            {
                // If 'No', do something here.
            }

        }


        private DataTable checkForExistingInquiries()
        {

            DataTable dtDuplicates;
            StringBuilder sbFilter = new StringBuilder();

            for (int i = 0; i < lvProviderResults.Items.Count; i++)
            {

                //r.phase_id = 2 AND r.mpin  =   2522195

                sbFilter.Append("(r.mpin = " + lvProviderResults.Items[i].Text + " AND r.phase_id = " + cmbPhase.SelectedValue + ") OR ");
            }

            dtDuplicates = DBConnection.getMSSQLDataTable(GlobalObjects.strILUCAConnectionString,  GlobalObjects.getSelectDuplicateCheckSQL("(" + sbFilter.ToString().TrimEnd('O','R',' ') + ")") );

            return dtDuplicates;
        }









        private void searchProvider()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                radDeselectAll.Checked = false;
                radSelectAll.Checked = false;

                string strSearch, strPhase;

                strSearch = cmbProviderSearch.Text;
                if(!strSearch.IsNumeric())
                    strSearch = strSearch.ToFullTextSearch();

                strPhase = cmbPhase.SelectedValue.ToString();

                if (String.IsNullOrEmpty(strSearch))
                    return;


                Hashtable ht = GlobalObjects.htProviderSearchSQL(strSearch, strPhase, "Both");
                DataTable dtSearchResults = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strGetProviderSearchSQL, ht);

                DataRow[] dr = dtSearchResults.Select("type='TIN'");

                if (dr.Count() > 0)
                {
                    SharedWinFormFunctions.addDataTableToListView(ref lvTinSearch, dr.CopyToDataTable(), GlobalObjects.strGetProviderSearchExcludeArr);
                }
                else
                {
                    //MessageBox.Show("No Results Found", "No Results", MessageBoxButtons.OK);
                    lvTinSearch.Clear();
                }

                dr = dtSearchResults.Select("type='MPIN'");
                if (dr.Count() > 0)
                {
                    SharedWinFormFunctions.addDataTableToListView(ref lvProviderResults, dr.CopyToDataTable(), GlobalObjects.strGetProviderSearchExcludeArr, blMultiSelect: true, blCheckbox: true);
                }
                else
                {
                    //MessageBox.Show("No Results Found", "No Results", MessageBoxButtons.OK);
                    lvProviderResults.Clear();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Cursor = Cursors.Default;
            }



            //DataTable dtTIN = null;
            //DataTable dtMPIN;
        }

        //SMART SEARCH START
        //SMART SEARCH START
        //SMART SEARCH START
        bool blStopSmartSearch = false;
        private void cmbProviderSearch_TextChanged(object sender, EventArgs e)
        {

            if (blStopSmartSearch == true)
                return;

            string strSearchString = cmbProviderSearch.Text.Replace("\"", "");


            if (strSearchString.Length > 2)
            {

                blStopSmartSearch = true;

                string strPhase = cmbPhase.SelectedValue.ToString();
                Hashtable htTmp = GlobalObjects.htProviderSmartSearchSQL(strPhase, strSearchString.ToFullTextSearch());
                List<string> searchData = DBConnection.getMSSQLToStringListSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strProviderSmartSearchSQL, htTmp, strSearchString);


                var text = strSearchString;

                if (searchData.Count() > 0)
                {
                    cmbProviderSearch.DataSource = searchData;

                    cmbProviderSearch.DroppedDown = true;
                    Cursor.Current = Cursors.Default;
                    cmbProviderSearch.Text = text;


                    cmbProviderSearch.SelectionStart = cmbProviderSearch.Text.Length; // add some logic if length is 0
                    cmbProviderSearch.SelectionLength = 0;
                }
                else
                {
                    cmbProviderSearch.DroppedDown = false;
                }
            }
        }
        private void cmbProviderSearch_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyValue == (int)Keys.Enter || e.KeyValue == (int)Keys.Up || e.KeyValue == (int)Keys.Down)
            {
                blStopSmartSearch = true;

                if (e.KeyValue == (int)Keys.Enter)
                    searchProvider();

            }  
            else
                blStopSmartSearch = false;
        }

        private void cmbProviderSearch_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            cmbProviderSearch.Text = senderComboBox.SelectedItem.ToString();
            blStopSmartSearch = true;
        }

        private void cmbProviderSearch_MouseClick(object sender, MouseEventArgs e)
        {
            
            blStopSmartSearch = true;

        }

        //CHRIS ADDED 4/19/2017 CLICK SEARCH
        private void cmbProviderSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            blStopSmartSearch = true;
            ComboBox senderComboBox = (ComboBox)sender;
            if (senderComboBox.SelectedIndex > 0)
                searchProvider();
        }


        private void openTrackingItemWindow(string strTrackingItemId)
        {

            var form = new frmTrackingItem();
            form.btnInsertTrackingChild.Text = "Add Tracking Item";
            form.strTrackingChildItemId = null;

            form.lblMPINDisplay.Text = _strCurrentMPIN;
            form.lblProviderNameDisplay.Text = _strCurrentProviderName;
            form.lblProjectNameDisplay.Text = this.cmbPhase.Text;
            //form.lblProviderSpecialtyDisplay.Text = this.lblProviderSpecialtyDisplay.Text;

            form.Text = "Add Tracking Item";
            form.strTrackingItemId = strTrackingItemId;

            //form.Parent = this.form;
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this); // if you need non-modal window
        }






        private void openEditTrackingItemWindow(string strTrackingItemId)
        {


            frmEditTrackingItem _frmEditTrackingItem = new frmEditTrackingItem(strTrackingItemId);
            if (!_frmEditTrackingItem.IsDisposed)
                _frmEditTrackingItem.ShowDialog(this);



        }









        private void btnProviderDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                if (string.IsNullOrEmpty(cmbProviderSearch.Text))
                {
                    MessageBox.Show("You must enter a provide first", "Warning");
                    return;
                }

                var form = new frmDetails(cmbProviderSearch.Text.ToFullTextSearch());
                form.StartPosition = FormStartPosition.CenterParent;
                form.ShowDialog(this); // if you need non-modal window
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Cursor = Cursors.Default;
            }

        }

        private void cmbPhase_SelectedIndexChanged(object sender, EventArgs e)
        {
            lvProviderResults.Clear();
            lvTinSearch.Clear();
        }

        private void cmbPhase_SelectedValueChanged(object sender, EventArgs e)
        {
            lvProviderResults.Clear();
            lvTinSearch.Clear();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }






        //SMART SEARCH END
        //SMART SEARCH END
        //SMART SEARCH END
    }
}
