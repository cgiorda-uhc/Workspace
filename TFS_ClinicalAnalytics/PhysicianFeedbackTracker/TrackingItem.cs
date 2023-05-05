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

namespace PhysicianFeedbackTracker
{
    public partial class frmTrackingItem : _BaseClass
    {


        Color _clrHightlight = Color.LightPink;
        public string strTrackingChildItemId;
        public DataRow drTrackingChild;
        public string strTrackingItemId;


        public frmTrackingItem()
        {
            InitializeComponent();
            populateInputFields();

        }


        private void populateInputFields()
        {

            DataTable dtTmp = GlobalObjects.getNameValueDataTable("inquiry_category");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cbxInquiryCategory.DataSource = dtTmp;
            cbxInquiryCategory.DisplayMember = "name";
            cbxInquiryCategory.ValueMember = "value";

            dtTmp = GlobalObjects.getNameValueDataTable("inquiry_status");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cbxInquiryStatus.DataSource = dtTmp;
            cbxInquiryStatus.DisplayMember = "name";
            cbxInquiryStatus.ValueMember = "value";


            dtTmp = GlobalObjects.getNameValueDataTable("provider_tone");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cbxProviderTone.DataSource = dtTmp;
            cbxProviderTone.DisplayMember = "name";
            cbxProviderTone.ValueMember = "value";


            dtTmp = GlobalObjects.getNameValueDataTable("member_detail");
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cbxMemberDetailAvailable.DataSource = dtTmp;
            cbxMemberDetailAvailable.DisplayMember = "name";
            cbxMemberDetailAvailable.ValueMember = "value";



            dtTmp = GlobalObjects.getNameValueDataTable("users").Select("filter IN ('QA','Measures','Operations')").CopyToDataTable();
            dtTmp = SharedDataTableFunctions.addToNameValueDatatable("--Select One--", "-9999", dtTmp);
            cbxPCRAssignedTo.DataSource = dtTmp;
            cbxPCRAssignedTo.DisplayMember = "name";
            cbxPCRAssignedTo.ValueMember = "value";


        }

        public void populateInputFieldsWithData()
        {

            //clearInputFields();

            //DataRow[] dr = _dtTrackingChildGlobal.Select("qa_tracker_child_id = " + _strTrackingChildItemId);


            if (drTrackingChild["Inquiry Category"] != DBNull.Value)
            {
                cbxInquiryCategory.Text = drTrackingChild["Inquiry Category"].ToString();
                grpInquiryCategory.BackColor = _clrHightlight;
            }
            else
            {
                grpInquiryCategory.BackColor = this.BackColor;
            }


            if (drTrackingChild["Inquiry Status"] != DBNull.Value)
            {
                cbxInquiryStatus.Text = drTrackingChild["Inquiry Status"].ToString();
                grpInquiryStatus.BackColor = _clrHightlight;
            }
            else
            {
                grpInquiryStatus.BackColor = this.BackColor;
            }


            if (drTrackingChild["Member Detail"] != DBNull.Value)
            {
                cbxMemberDetailAvailable.Text = drTrackingChild["Member Detail"].ToString();
                grpMemberDetailAvailable.BackColor = _clrHightlight;
            }
            else
            {
                grpMemberDetailAvailable.BackColor = this.BackColor;
            }


            if (drTrackingChild["Date Resolved"] != DBNull.Value)
            {
                dtpDateResolved.Value = DateTime.Parse(drTrackingChild["Date Resolved"].ToString());
                dtpDateResolved.Checked = true;
                grpDateResolved.BackColor = _clrHightlight;
            }
            else
            {
                grpDateResolved.BackColor = this.BackColor;
            }



            if (drTrackingChild["Attestation Required"] != DBNull.Value)
            {
                if (drTrackingChild["Attestation Required"].ToString().Equals("Yes"))
                    radAttestationRequiredYes.Checked = true;
                else
                    radAttestationRequiredNo.Checked = true;

                grpAttestationRequired.BackColor = _clrHightlight;
            }
            else
            {
                grpAttestationRequired.BackColor = this.BackColor;
            }




            if (drTrackingChild["Date Attestation Sent"] != DBNull.Value)
            {
                dtpDateAttestationSent.Value = DateTime.Parse(drTrackingChild["Date Attestation Sent"].ToString());
                dtpDateAttestationSent.Checked = true;

                grpDateAttestationSent.BackColor = _clrHightlight;
            }
            else
            {
                grpDateAttestationSent.BackColor = this.BackColor;
            }



            if (drTrackingChild["Date Attestation Received"] != DBNull.Value)
            {
                dtpDateAttestationReceived.Value = DateTime.Parse(drTrackingChild["Date Attestation Received"].ToString());
                dtpDateAttestationReceived.Checked = true;

                grpDateAttestationReceived.BackColor = _clrHightlight;
            }
            else
            {
                grpDateAttestationReceived.BackColor = this.BackColor;
            }



            if (drTrackingChild["Provider Tone Description"] != DBNull.Value)
            {
                cbxProviderTone.Text = drTrackingChild["Provider Tone Description"].ToString();

                grpProviderTone.BackColor = _clrHightlight;
            }
            else
            {
                grpProviderTone.BackColor = this.BackColor;
            }


            if (drTrackingChild["Date Member Detail Available"] != DBNull.Value)
            {
                dtpDateMemberDetailAvailable.Value = DateTime.Parse(drTrackingChild["Date Member Detail Available"].ToString());
                dtpDateMemberDetailAvailable.Checked = true;

                grpDateMemberDetailAvailable.BackColor = _clrHightlight;
            }
            else
            {
                grpDateMemberDetailAvailable.BackColor = this.BackColor;
            }



            if (drTrackingChild["Date Ready for Analytics"] != DBNull.Value)
            {
                dtpDateReadyForAnaytics.Value = DateTime.Parse(drTrackingChild["Date Ready for Analytics"].ToString());
                dtpDateReadyForAnaytics.Checked = true;

                grpDateReadyForAnaytics.BackColor = _clrHightlight;
            }
            else
            {
                grpDateReadyForAnaytics.BackColor = this.BackColor;
            }



            if (drTrackingChild["Date Resolved by Analytics"] != DBNull.Value)
            {
                dtpDateResolvedByAnalytics.Value = DateTime.Parse(drTrackingChild["Date Resolved by Analytics"].ToString());
                dtpDateResolvedByAnalytics.Checked = true;

                grpDateResolvedByAnalytics.BackColor = _clrHightlight;
            }
            else
            {
                grpDateResolvedByAnalytics.BackColor = this.BackColor;
            }



            if (drTrackingChild["Notes"] != DBNull.Value)
            {
                txtNotes.Text = drTrackingChild["Notes"].ToString();

                grpComments.BackColor = _clrHightlight;
            }
            else
            {
                grpComments.BackColor = this.BackColor;
            }




            if (drTrackingChild["PCR Completed By"] != DBNull.Value)
            {
                cbxPCRAssignedTo.Text = drTrackingChild["PCR Completed By"].ToString();
                grpPCRAssignedTo.BackColor = _clrHightlight;
            }
            else
            {
                grpPCRAssignedTo.BackColor = this.BackColor;
            }



            if (drTrackingChild["Analytic Notes"] != DBNull.Value)
            {
                txtNotesFromAnalytics.Text = drTrackingChild["Analytic Notes"].ToString();

                grpNotesFromAnalytics.BackColor = _clrHightlight;
            }
            else
            {
                grpNotesFromAnalytics.BackColor = this.BackColor;
            }

            if (drTrackingChild["Reason for Exclusion"] != DBNull.Value)
            {
                txtReasonForExclusion.Text = drTrackingChild["Reason for Exclusion"].ToString();

                grpReasonForExclusion.BackColor = _clrHightlight;
            }
            else
            {
                grpReasonForExclusion.BackColor = this.BackColor;
            }


            if (drTrackingChild["Exclude from Practice Mailing"] != DBNull.Value)
            {
                chkExcludeFromPracticeMailing.Checked = (drTrackingChild["Exclude from Practice Mailing"] + "" == "Yes" ? true : false);

                if (chkExcludeFromPracticeMailing.Checked)
                    grpExcludeFromPracticeMailing.BackColor = _clrHightlight;
                else
                    grpExcludeFromPracticeMailing.BackColor = this.BackColor;
            }
            else
            {
                grpExcludeFromPracticeMailing.BackColor = this.BackColor;
            }


            if (drTrackingChild["Exclude from MPIN Mailing"] != DBNull.Value)
            {
                chkExcludeFromMPINMailing.Checked = (drTrackingChild["Exclude from MPIN Mailing"] + "" == "Yes" ? true : false);

                if (chkExcludeFromMPINMailing.Checked)
                    grpExcludeFromMPINMailing.BackColor = _clrHightlight;
                else
                    grpExcludeFromMPINMailing.BackColor = this.BackColor;

            }
            else
            {
                grpExcludeFromMPINMailing.BackColor = this.BackColor;
            }


            if (drTrackingChild["Date Inquiry Received"] != DBNull.Value)
            {
                dtpDateInquiryReceived.Value = DateTime.Parse(drTrackingChild["Date Inquiry Received"].ToString());
                dtpDateInquiryReceived.Checked = true;

                grpDateInquiryReceived.BackColor = _clrHightlight;
            }
            else
            {
                grpDateInquiryReceived.BackColor = this.BackColor;
            }



            //if (drTrackingChild["is_grouped"] != DBNull.Value)
            //{
            //    GlobalObjects.inquiryCurrentParentIsGrouped = (drTrackingChild["is_grouped"] + "" == "1" ? true : false);
            //}

                

        }

        private void btnInsertTrackingChild_Click(object sender, EventArgs e)
        {
            if (!insertTrackingChild())
                return;

            // GlobalObjects.clearChildGroups();
            this.Close();

           
            //changeMode(false);

        }


        private void btnInsertAndNewTrackingChild_Click(object sender, EventArgs e)
        {
            if (!insertTrackingChild())
                return;

            GlobalObjects.clearChildGroups();

            strTrackingChildItemId = null;
            clearInputFields();

        }



        private bool insertTrackingChild()
        {

            if (!allowInsert())
            {
                MessageBox.Show("All fields are empty!", "Input Error", MessageBoxButtons.OK);
                return false;
            }

            string str_qa_tracker_parent_id ;
            string str_qa_tracker_child_id;
            string str_inquiry_category_id;
            string str_inquiry_status_id;
            string str_member_detail_available_id;
            string str_date_resolved;
            string str_attestation_required;
            string str_date_attestation_sent;
            string str_date_attestation_received;
            string str_provider_tone_id;
            string str_date_member_detail_available;
            string str_resolved_analytics_date;
            string str_ready_analytics_date;
            string str_notes;
            string str_inserted_by_nt_id;
            string str_updated_by_nt_id;

            string str_exclude_practice_mailing;
            string str_exclude_mpin_mailing;
            string str_reason_for_exclusion;
            string str_analytic_note;
            string str_assigned_pcr;
            string str_date_inquiry_received;


            string str_parent_group_id = null;
            string str_child_group_id;
            string str_group_name = null;
            string str_is_grouped;

            object obj_qa_tracker_child_id = null;
            DataTable dtResults = null;
            Hashtable ht = null; 
            bool blHasChildren = false;


            if (strTrackingChildItemId != null) //UPDATE
            {
                GlobalObjects.inquiryChildGroupList = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectChildGroupSQL, GlobalObjects.htSelectChildGroupSQL(strTrackingChildItemId)).AsEnumerable().Select(r => r.Field<int>("qa_tracker_child_id")).ToList();

                if (GlobalObjects.inquiryChildGroupList.Count < 1 || chkIsGroup.Checked == false)
                    GlobalObjects.inquiryChildGroupList = new List<int> { int.Parse(strTrackingChildItemId) };




                foreach (int intId in GlobalObjects.inquiryChildGroupList)
                {


                    //        GlobalObjects.inquiryChildGroupList
                    //GlobalObjects.inquiryCurrenChildGroupId

                    str_parent_group_id = GlobalObjects.inquiryCurrentParentGroupId;
                    

                    if (GlobalObjects.inquiryCurrenChildGroupId == null)
                        str_child_group_id = (obj_qa_tracker_child_id == null ? null : obj_qa_tracker_child_id.ToString());
                    else
                        str_child_group_id = GlobalObjects.inquiryCurrenChildGroupId;

                    //str_is_grouped = (GlobalObjects.inquiryCurrentParentGroupId == null ? "0" : "1");

                    str_is_grouped = null;
                    if (GlobalObjects.inquiryCurrentChildIsGrouped != null)
                    str_is_grouped = (GlobalObjects.inquiryCurrentChildIsGrouped == false ? "0" : "1");

                    if (chkIsGroup.Checked == false)
                        str_is_grouped = "0";




                    

                    str_qa_tracker_parent_id = intId.ToString();

                    str_qa_tracker_child_id = intId.ToString();



                    str_inquiry_category_id = (cbxInquiryCategory.SelectedValue.ToString().Equals("-9999") ? null : cbxInquiryCategory.SelectedValue.ToString());
                    str_inquiry_status_id = (cbxInquiryStatus.SelectedValue.ToString().Equals("-9999") ? null : cbxInquiryStatus.SelectedValue.ToString());
                    str_member_detail_available_id = (cbxMemberDetailAvailable.SelectedValue.ToString().Equals("-9999") ? null : cbxMemberDetailAvailable.SelectedValue.ToString());
                    str_date_resolved = (dtpDateResolved.Checked == false ? null : dtpDateResolved.Value.ToShortDateString());
                    str_attestation_required = (radAttestationRequiredYes.Checked ? "1" : (radAttestationRequiredNo.Checked ? "0" : null));
                    str_date_attestation_sent = (dtpDateAttestationSent.Checked == false ? null : dtpDateAttestationSent.Value.ToShortDateString());
                    str_date_attestation_received = (dtpDateAttestationReceived.Checked == false ? null : dtpDateAttestationReceived.Value.ToShortDateString());
                    str_provider_tone_id = (cbxProviderTone.SelectedValue.ToString().Equals("-9999") ? null : cbxProviderTone.SelectedValue.ToString());
                    str_date_member_detail_available = (dtpDateMemberDetailAvailable.Checked == false ? null : dtpDateMemberDetailAvailable.Value.ToShortDateString());
                    str_resolved_analytics_date = (dtpDateResolvedByAnalytics.Checked == false ? null : dtpDateResolvedByAnalytics.Value.ToShortDateString());
                    str_ready_analytics_date = (dtpDateReadyForAnaytics.Checked == false ? null : dtpDateReadyForAnaytics.Value.ToShortDateString());
                    str_notes = (string.IsNullOrEmpty(txtNotes.Text) ? null : txtNotes.Text.Trim());
                    str_inserted_by_nt_id = GlobalObjects.strCurrentUser;
                    str_updated_by_nt_id = GlobalObjects.strCurrentUser;

                    str_exclude_practice_mailing = (chkExcludeFromPracticeMailing.Checked ? "1" : "0");
                    str_exclude_mpin_mailing = (chkExcludeFromMPINMailing.Checked ? "1" : "0");
                    str_reason_for_exclusion = (string.IsNullOrEmpty(txtReasonForExclusion.Text) ? null : txtReasonForExclusion.Text.Trim());
                    str_analytic_note = (string.IsNullOrEmpty(txtNotesFromAnalytics.Text) ? null : txtNotesFromAnalytics.Text.Trim());
                    str_assigned_pcr = (cbxPCRAssignedTo.SelectedValue.ToString().Equals("-9999") ? null : cbxPCRAssignedTo.SelectedValue.ToString());


                    str_date_inquiry_received = (dtpDateInquiryReceived.Checked == false ? null : dtpDateInquiryReceived.Value.ToShortDateString());

                    ht = GlobalObjects.htInsertUpdateChildToTrackerSQL(str_qa_tracker_parent_id, str_qa_tracker_child_id, str_date_inquiry_received, str_inquiry_category_id, str_inquiry_status_id, str_member_detail_available_id, str_date_resolved, str_attestation_required, str_date_attestation_sent, str_date_attestation_received, str_provider_tone_id, str_date_member_detail_available, str_resolved_analytics_date, str_ready_analytics_date, str_notes, str_exclude_practice_mailing, str_exclude_mpin_mailing, str_reason_for_exclusion, str_analytic_note, str_assigned_pcr, str_inserted_by_nt_id, str_updated_by_nt_id, str_parent_group_id, str_child_group_id, str_group_name, str_is_grouped);


                    dtResults =  DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strInsertUpdateChildToTrackerSQL, ht);

                    obj_qa_tracker_child_id = dtResults.Rows[0][0];

                    GlobalObjects.inquiryCurrenChildGroupId = (obj_qa_tracker_child_id != null ? obj_qa_tracker_child_id.ToString() : null);

                }


            }
            else //INSERT  
            {
                StringBuilder sbName = new StringBuilder();
                sbName.Append(lblProjectNameDisplay.Text.Trim().Replace(" ", "_") + "_");


                if (GlobalObjects.inquiryParentGroupList == null)
                    GlobalObjects.inquiryParentGroupList = new List<int> { int.Parse(strTrackingItemId) };

                foreach (int intId in GlobalObjects.inquiryParentGroupList)
                {

                    str_qa_tracker_child_id = null;


                    //        GlobalObjects.inquiryChildGroupList
                    //GlobalObjects.inquiryCurrenChildGroupId


                    //strGroupId
                    str_parent_group_id = GlobalObjects.inquiryCurrentParentGroupId;


                    if (GlobalObjects.inquiryCurrenChildGroupId == null)
                        str_child_group_id = (obj_qa_tracker_child_id == null ? null : obj_qa_tracker_child_id.ToString());
                    else
                        str_child_group_id = GlobalObjects.inquiryCurrenChildGroupId;

                    //str_is_grouped = (GlobalObjects.inquiryCurrentParentGroupId == null ? "0" : "1");


                    str_is_grouped = (GlobalObjects.inquiryCurrentParentIsGrouped == null ? null : (GlobalObjects.inquiryCurrentParentIsGrouped == false ? "0" : "1"));

                    //str_is_grouped = null;
                    //if (GlobalObjects.inquiryCurrentChildIsGrouped != null)
                    //    str_is_grouped = (GlobalObjects.inquiryCurrentChildIsGrouped == false ? "0" : "1");



                    str_qa_tracker_parent_id = intId.ToString();


                    str_inquiry_category_id = (cbxInquiryCategory.SelectedValue.ToString().Equals("-9999") ? null : cbxInquiryCategory.SelectedValue.ToString());
                    str_inquiry_status_id = (cbxInquiryStatus.SelectedValue.ToString().Equals("-9999") ? null : cbxInquiryStatus.SelectedValue.ToString());
                    str_member_detail_available_id = (cbxMemberDetailAvailable.SelectedValue.ToString().Equals("-9999") ? null : cbxMemberDetailAvailable.SelectedValue.ToString());
                    str_date_resolved = (dtpDateResolved.Checked == false ? null : dtpDateResolved.Value.ToShortDateString());
                    str_attestation_required = (radAttestationRequiredYes.Checked ? "1" : (radAttestationRequiredNo.Checked ? "0" : null));
                    str_date_attestation_sent = (dtpDateAttestationSent.Checked == false ? null : dtpDateAttestationSent.Value.ToShortDateString());
                    str_date_attestation_received = (dtpDateAttestationReceived.Checked == false ? null : dtpDateAttestationReceived.Value.ToShortDateString());
                    str_provider_tone_id = (cbxProviderTone.SelectedValue.ToString().Equals("-9999") ? null : cbxProviderTone.SelectedValue.ToString());
                    str_date_member_detail_available = (dtpDateMemberDetailAvailable.Checked == false ? null : dtpDateMemberDetailAvailable.Value.ToShortDateString());
                    str_resolved_analytics_date = (dtpDateResolvedByAnalytics.Checked == false ? null : dtpDateResolvedByAnalytics.Value.ToShortDateString());
                    str_ready_analytics_date = (dtpDateReadyForAnaytics.Checked == false ? null : dtpDateReadyForAnaytics.Value.ToShortDateString());
                    str_notes = (string.IsNullOrEmpty(txtNotes.Text) ? null : txtNotes.Text.Trim());
                    str_inserted_by_nt_id = GlobalObjects.strCurrentUser;
                    str_updated_by_nt_id = GlobalObjects.strCurrentUser;

                    str_exclude_practice_mailing = (chkExcludeFromPracticeMailing.Checked ? "1" : "0");
                    str_exclude_mpin_mailing = (chkExcludeFromMPINMailing.Checked ? "1" : "0");
                    str_reason_for_exclusion = (string.IsNullOrEmpty(txtReasonForExclusion.Text) ? null : txtReasonForExclusion.Text.Trim());
                    str_analytic_note = (string.IsNullOrEmpty(txtNotesFromAnalytics.Text) ? null : txtNotesFromAnalytics.Text.Trim());
                    str_assigned_pcr = (cbxPCRAssignedTo.SelectedValue.ToString().Equals("-9999") ? null : cbxPCRAssignedTo.SelectedValue.ToString());


                    str_date_inquiry_received = (dtpDateInquiryReceived.Checked == false ? null : dtpDateInquiryReceived.Value.ToShortDateString());

                    ht = GlobalObjects.htInsertUpdateChildToTrackerSQL(str_qa_tracker_parent_id, str_qa_tracker_child_id, str_date_inquiry_received, str_inquiry_category_id, str_inquiry_status_id, str_member_detail_available_id, str_date_resolved, str_attestation_required, str_date_attestation_sent, str_date_attestation_received, str_provider_tone_id, str_date_member_detail_available, str_resolved_analytics_date, str_ready_analytics_date, str_notes, str_exclude_practice_mailing, str_exclude_mpin_mailing, str_reason_for_exclusion, str_analytic_note, str_assigned_pcr, str_inserted_by_nt_id, str_updated_by_nt_id, str_parent_group_id, str_child_group_id, str_group_name, str_is_grouped);

                    dtResults = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strInsertUpdateChildToTrackerSQL, ht);

                    obj_qa_tracker_child_id = dtResults.Rows[0][0];


                    if(str_parent_group_id != null)
                    {
                        GlobalObjects.inquiryCurrenChildGroupId = (obj_qa_tracker_child_id != null ? obj_qa_tracker_child_id.ToString() : null);
                        sbName.Append(dtResults.Rows[0][1] + "_");
                    }



                }

                if (str_parent_group_id != null)
                {
                    str_group_name = "ChildGroup_" + sbName.ToString().TrimEnd('_');
                    //WHERE  = str_child_group_id 
                    GlobalObjects.inquiryCurrentChildGroupName = str_group_name;


                    ht = GlobalObjects.htUpdateChildGroupSQL(GlobalObjects.inquiryCurrenChildGroupId, str_group_name);
                    DBConnection.getMSSQLExecuteSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strUpdateChildGroupSQL, ht);

                }





            }



            return true;
        }


        private bool allowInsert()
        {
            bool blAllowInsert = true;

            if (strTrackingChildItemId != null)
                return blAllowInsert;


            if (cbxInquiryCategory.SelectedIndex == 0 && cbxInquiryStatus.SelectedIndex == 0 && cbxMemberDetailAvailable.SelectedIndex == 0 && dtpDateResolved.Checked == false && radAttestationRequiredYes.Checked == false && radAttestationRequiredNo.Checked == false && dtpDateAttestationSent.Checked == false && dtpDateAttestationReceived.Checked == false && cbxProviderTone.SelectedIndex == 0 && dtpDateReadyForAnaytics.Checked == false && dtpDateResolvedByAnalytics.Checked == false && dtpDateMemberDetailAvailable.Checked == false && txtNotes.Text == "" && txtNotesFromAnalytics.Text == "" && txtReasonForExclusion.Text == "" && chkExcludeFromPracticeMailing.Checked == false && chkExcludeFromMPINMailing.Checked == false && cbxPCRAssignedTo.SelectedIndex == 0 && dtpDateInquiryReceived.Checked == false)
            {
                blAllowInsert = false;
            }



            return blAllowInsert;
        }



        private void clearInputFields()
        {
            cbxInquiryCategory.SelectedIndex = 0;
            cbxInquiryStatus.SelectedIndex = 0;
            cbxMemberDetailAvailable.SelectedIndex = 0;
            dtpDateResolved.Value = DateTime.Today;
            dtpDateResolved.Checked = false;
            radAttestationRequiredYes.Checked = false;
            radAttestationRequiredNo.Checked = false;
            dtpDateAttestationSent.Value = DateTime.Today;
            dtpDateAttestationSent.Checked = false;
            dtpDateAttestationReceived.Value = DateTime.Today;
            dtpDateAttestationReceived.Checked = false;
            cbxProviderTone.SelectedIndex = 0;
            dtpDateReadyForAnaytics.Value = DateTime.Today;
            dtpDateReadyForAnaytics.Checked = false;


            dtpDateMemberDetailAvailable.Value = DateTime.Today;
            dtpDateMemberDetailAvailable.Checked = false;

            dtpDateResolvedByAnalytics.Value = DateTime.Today;
            dtpDateResolvedByAnalytics.Checked = false;



            cbxPCRAssignedTo.SelectedIndex = 0;
            txtNotesFromAnalytics.Text = "";
            txtReasonForExclusion.Text = "";
            chkExcludeFromMPINMailing.Checked = false;
            chkExcludeFromPracticeMailing.Checked = false;


            dtpDateInquiryReceived.Value = DateTime.Today;
            dtpDateInquiryReceived.Checked = false;


            txtNotes.Text = "";

            grpInquiryCategory.BackColor = this.BackColor;
            grpInquiryStatus.BackColor = this.BackColor;
            grpDateResolved.BackColor = this.BackColor;
            grpAttestationRequired.BackColor = this.BackColor;
            grpDateAttestationSent.BackColor = this.BackColor;
            grpDateAttestationReceived.BackColor = this.BackColor;
            grpProviderTone.BackColor = this.BackColor;
            grpDateReadyForAnaytics.BackColor = this.BackColor;
            grpDateResolvedByAnalytics.BackColor = this.BackColor;
            grpDateMemberDetailAvailable.BackColor = this.BackColor;
            grpComments.BackColor = this.BackColor;
            grpMemberDetailAvailable.BackColor = this.BackColor;
            grpDateInquiryReceived.BackColor = this.BackColor;

            grpPCRAssignedTo.BackColor = this.BackColor;
            grpNotesFromAnalytics.BackColor = this.BackColor;
            grpReasonForExclusion.BackColor = this.BackColor;
            grpExcludeFromPracticeMailing.BackColor = this.BackColor;
            grpExcludeFromPracticeMailing.BackColor = this.BackColor;


        }



  


        private void closeProcess()
        {
            GlobalObjects.clearChildGroups();


            Form frmEditTrackingItem = Application.OpenForms["frmEditTrackingItem"];
            if (frmEditTrackingItem != null)
                ((frmEditTrackingItem)frmEditTrackingItem).populateGrid();

            
        }

        private void cbxInquiryCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxInquiryCategory.SelectedIndex == 0)
                grpInquiryCategory.BackColor = this.BackColor;
            else
            {
                grpInquiryCategory.BackColor = _clrHightlight;
            }

        }

        private void cbxInquiryStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxInquiryStatus.SelectedIndex == 0)
                grpInquiryStatus.BackColor = this.BackColor;
            else
            {
                grpInquiryStatus.BackColor = _clrHightlight;
            }

            if(cbxInquiryStatus.Text == "Analytics Response Completed" && dtpDateResolvedByAnalytics.Checked == false)
            {
                dtpDateResolvedByAnalytics.Value = DateTime.Now;
                dtpDateResolvedByAnalytics.Checked = true;
            }
            else if (cbxInquiryStatus.Text == "Pending Analytics Response" && dtpDateReadyForAnaytics.Checked == false)
            {
                dtpDateReadyForAnaytics.Value = DateTime.Now;
                dtpDateReadyForAnaytics.Checked = true;
            }
        }


        private void cbxMemberDetailAvailable_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxMemberDetailAvailable.SelectedIndex == 0)
                grpMemberDetailAvailable.BackColor = this.BackColor;
            else
            {
                grpMemberDetailAvailable.BackColor = _clrHightlight;
            }

        }

        private void dtpDateResolved_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateResolved.Checked == false)
                grpDateResolved.BackColor = this.BackColor;
            else
            {
                grpDateResolved.BackColor = _clrHightlight;
            }

        }


        private void radAttestationRequiredYes_CheckedChanged(object sender, EventArgs e)
        {
            grpAttestationRequired.BackColor = _clrHightlight;
        }

        private void radAttestationRequiredNo_CheckedChanged(object sender, EventArgs e)
        {
            grpAttestationRequired.BackColor = _clrHightlight;
        }

        private void dtpDateAttestationSent_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateAttestationSent.Checked == false)
                grpDateAttestationSent.BackColor = this.BackColor;
            else
            {
                grpDateAttestationSent.BackColor = _clrHightlight;
            }

        }

        private void dtpDateAttestationReceived_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateAttestationReceived.Checked == false)
                grpDateAttestationReceived.BackColor = this.BackColor;
            else
            {
                grpDateAttestationReceived.BackColor = _clrHightlight;
            }

        }

        private void cbxProviderTone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxProviderTone.SelectedIndex == 0)
                grpProviderTone.BackColor = this.BackColor;
            else
            {
                grpProviderTone.BackColor = _clrHightlight;
            }

        }



        private void dtpDateMemberDetailAvailable_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateMemberDetailAvailable.Checked == false)
                grpDateMemberDetailAvailable.BackColor = this.BackColor;
            else
            {
                grpDateMemberDetailAvailable.BackColor = _clrHightlight;
            }

        }


        private void dtpDateReadyForAnaytics_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateReadyForAnaytics.Checked == false)
                grpDateReadyForAnaytics.BackColor = this.BackColor;
            else
            {
                grpDateReadyForAnaytics.BackColor = _clrHightlight;
            }

        }

        private void dtpDateResolvedByAnalytics_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateResolvedByAnalytics.Checked == false)
                grpDateResolvedByAnalytics.BackColor = this.BackColor;
            else
            {
                grpDateResolvedByAnalytics.BackColor = _clrHightlight;
            }

        }




        private void txtNotes_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNotes.Text))
                grpComments.BackColor = this.BackColor;
            else
            {
                grpComments.BackColor = _clrHightlight;
            }

        }



        private void chkExcludeFromMPINMailing_CheckedChanged(object sender, EventArgs e)
        {
            if (chkExcludeFromMPINMailing.Checked == false)
                grpExcludeFromMPINMailing.BackColor = this.BackColor;
            else
            {
                grpExcludeFromMPINMailing.BackColor = _clrHightlight;
            }

        }

        private void chkExcludeFromPracticeMailing_CheckedChanged(object sender, EventArgs e)
        {
            if (chkExcludeFromPracticeMailing.Checked == false)
                grpExcludeFromPracticeMailing.BackColor = this.BackColor;
            else
            {
                grpExcludeFromPracticeMailing.BackColor = _clrHightlight;
            }

        }

        private void txtReasonForExclusion_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtReasonForExclusion.Text))
                grpReasonForExclusion.BackColor = this.BackColor;
            else
            {
                grpReasonForExclusion.BackColor = _clrHightlight;
            }

        }

        private void txtNotesFromAnalytics_TextChanged(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtNotesFromAnalytics.Text))
                grpNotesFromAnalytics.BackColor = this.BackColor;
            else
            {
                grpNotesFromAnalytics.BackColor = _clrHightlight;
            }

        }

        private void cbxPCRAssignedTo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxPCRAssignedTo.SelectedIndex == 0)
                grpPCRAssignedTo.BackColor = this.BackColor;
            else
            {
                grpPCRAssignedTo.BackColor = _clrHightlight;
            }

        }



        private void dtpDateInquiryReceived_ValueChanged(object sender, EventArgs e)
        {
            if (dtpDateInquiryReceived.Checked == false)
                grpDateInquiryReceived.BackColor = this.BackColor;
            else
            {
                grpDateInquiryReceived.BackColor = _clrHightlight;
            }

        }

        private void grpGeneralInquiryInfo_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            SharedWinFormFunctions.DrawGroupBox(box, e.Graphics, Color.Red, Color.Blue);
        }

        private void grpMemberDetail_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            SharedWinFormFunctions.DrawGroupBox(box, e.Graphics, Color.Red, Color.Blue);
        }

        private void grpAnalyticsAction_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            SharedWinFormFunctions.DrawGroupBox(box, e.Graphics, Color.Red, Color.Blue);
        }

        private void grpMiscellaneous_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            SharedWinFormFunctions.DrawGroupBox(box, e.Graphics, Color.Red, Color.Blue);
        }

        private void grpNotes_Paint(object sender, PaintEventArgs e)
        {
            GroupBox box = sender as GroupBox;
            SharedWinFormFunctions.DrawGroupBox(box, e.Graphics, Color.Red, Color.Blue);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            //closeThisWindow();
        }

        private void frmTrackingItem_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeProcess();
        }







        //private void setTableScrolling()
        //{
        //    int intWidth = 0;
        //    int intHeight = 0;

        //    intHeight = tlpAddEditInquiry.Height;
        //    intWidth = tlpAddEditInquiry.Width;

        //    tlpAddEditInquiry.Dock = DockStyle.Top;
        //    tlpAddEditInquiry.AutoSize = true;

        //    pnlAddEditInquiry.AutoScroll = true;
        //}


        //private void tlpAddEditInquiry_MouseEnter(object sender, EventArgs e)
        //{
        //    pnlAddEditInquiry.Focus();
        //}













        //private void clearInputFields()
        //{
        //    cbxInquiryCategory.SelectedIndex = 0;
        //    cbxInquiryStatus.SelectedIndex = 0;
        //    cbxMemberDetailAvailable.SelectedIndex = 0;
        //    dtpDateResolved.Value = DateTime.Today;
        //    dtpDateResolved.Checked = false;
        //    radAttestationRequiredYes.Checked = false;
        //    radAttestationRequiredNo.Checked = false;
        //    dtpDateAttestationSent.Value = DateTime.Today;
        //    dtpDateAttestationSent.Checked = false;
        //    dtpDateAttestationReceived.Value = DateTime.Today;
        //    dtpDateAttestationReceived.Checked = false;
        //    cbxProviderTone.SelectedIndex = 0;
        //    dtpDateReadyForAnaytics.Value = DateTime.Today;
        //    dtpDateReadyForAnaytics.Checked = false;


        //    dtpDateMemberDetailAvailable.Value = DateTime.Today;
        //    dtpDateMemberDetailAvailable.Checked = false;

        //    dtpDateResolvedByAnalytics.Value = DateTime.Today;
        //    dtpDateResolvedByAnalytics.Checked = false;



        //    cbxPCRAssignedTo.SelectedIndex = 0;
        //    txtNotesFromAnalytics.Text = "";
        //    txtReasonForExclusion.Text = "";
        //    chkExcludeFromMPINMailing.Checked = false;
        //    chkExcludeFromPracticeMailing.Checked = false;


        //    dtpDateInquiryReceived.Value = DateTime.Today;
        //    dtpDateInquiryReceived.Checked = false;


        //    txtNotes.Text = "";

        //    grpSourceOfInquiry.BackColor = this.BackColor;
        //    grpInquiryCategory.BackColor = this.BackColor;
        //    grpInquiryStatus.BackColor = this.BackColor;
        //    grpDateResolved.BackColor = this.BackColor;
        //    grpAttestationRequired.BackColor = this.BackColor;
        //    grpDateAttestationSent.BackColor = this.BackColor;
        //    grpDateAttestationReceived.BackColor = this.BackColor;
        //    grpProviderTone.BackColor = this.BackColor;
        //    grpDateReadyForAnaytics.BackColor = this.BackColor;
        //    grpDateResolvedByAnalytics.BackColor = this.BackColor;
        //    grpDateMemberDetailAvailable.BackColor = this.BackColor;
        //    grpComments.BackColor = this.BackColor;
        //    grpMemberDetailAvailable.BackColor = this.BackColor;
        //    grpDateInquiryReceived.BackColor = this.BackColor;

        //    grpPCRAssignedTo.BackColor = this.BackColor;
        //    grpNotesFromAnalytics.BackColor = this.BackColor;
        //    grpReasonForExclusion.BackColor = this.BackColor;
        //    grpExcludeFromPracticeMailing.BackColor = this.BackColor;
        //    grpExcludeFromMPINMailing.BackColor = this.BackColor;


        //}












    }
}
