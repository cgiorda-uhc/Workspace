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
    public partial class frmAddEngagementToPEI : _BaseClass
    {
        public frmAddEngagementToPEI()
        {
            InitializeComponent();


            DataTable dtTmp = GlobalObjects.getNameValueDataTable("users").Select("filter IN ('MMD')").CopyToDataTable();
            cmbMMDAssignment.DataSource = dtTmp;
            cmbMMDAssignment.DisplayMember = "name";
            cmbMMDAssignment.ValueMember = "value";
            

        }

        private DataTable _dtFullInquiryList;
        //private DataTable _dtInquiry;
        private string _strMPIN;
        public void populateFields(string strCSVChildIds)
        {
           // _strMPIN = strMPIN;
           // _dtInquiry = dtInquiry;
           // lblKeyTopic.Text = strKeyTopic;
            //lblProviderName.Text = strProviderName;
            //cmbMMDAssignment.Text = strMMD;


            Hashtable ht = new Hashtable();
            ht.Add("@qa_tracker_child_id", strCSVChildIds);
            _dtFullInquiryList = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectTrackerFullRequestSQL, ht);

            if (_dtFullInquiryList.Rows.Count <= 0)
                this.Close();


            _strMPIN = _dtFullInquiryList.Rows[0]["Physician MPIN"].ToString();
            lblKeyTopic.Text = _dtFullInquiryList.Rows[0]["PEI Project Name"].ToString();
            lblProviderName.Text = _dtFullInquiryList.Rows[0]["Physician Name"].ToString();
            cmbMMDAssignment.Text = _dtFullInquiryList.Rows[0]["Physician MMD"].ToString();



        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddEngagement_Click(object sender, EventArgs e)
        {
            bool blPassed = false;
            object objPEIReturn;
            string strProviderId, strKeyTopicId, strNotes, strChildIdCSV, strMMDEmail, strCCEmail;

            string strURL;

            try
            {

                this.Cursor = Cursors.WaitCursor;

                tssAddEngagementToPEI.Text = "Retreiving Email Address...";
                //SEND CONFIRMATION EMAIL TO MMD
                strMMDEmail  = SharedFunctions.getEmailAddress(cmbMMDAssignment.SelectedValue.ToString());
                
                if(String.IsNullOrEmpty(strMMDEmail))
                {
                    MessageBox.Show("Cannot find email address for the selected MMD. Please have them update their UHC email address via Self Service.");
                    return;

                }


                //ht = GlobalObjects.htGetUserEmailByUserIdSQL(GlobalObjects.strCurrentUser);
                //objPEIReturn = DBConnection.getMSSQLExecuteScalarSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strGetUserEmailByUserIdSQL, ht);
                //if (objPEIReturn == null)
                //{
                //    MessageBox.Show("Your email was not found");
                //    return;
                //}
                //strCCEmail = objPEIReturn.ToString();
                strCCEmail = GlobalObjects.strCurrentEmail;


                if (String.IsNullOrEmpty(strCCEmail))
                {
                    MessageBox.Show("Cannot find your email address. Please update your UHC email address via Self Service.");
                    return;

                }



                tssAddEngagementToPEI.Text = "Verifying in PEI...";
                ////VERIFY TOPIC IS IN PEI
                //objPEIReturn = DBConnection.getMSSQLExecuteScalar(GlobalObjects.strPEIConnectionString, GlobalObjects.getPEIKeyTopicCheckSQL(lblKeyTopic.Text.Replace("'","''")));
                //if(objPEIReturn == null)
                //{
                //    MessageBox.Show("KeyTopic not found in PEI");
                //    return;
                //}
                //strKeyTopicId = objPEIReturn.ToString();

                ////VERIFY PROVIDER IS IN PEI
                //objPEIReturn = DBConnection.getMSSQLExecuteScalar(GlobalObjects.strPEIConnectionString, GlobalObjects.getPEIProviderCheckSQL(_strMPIN));
                //if (objPEIReturn == null)
                //{
                //    MessageBox.Show("Provider not found in PEI");
                //    return;
                //}
                //strProviderId = objPEIReturn.ToString();


                DataTable dt = null;
                if(GlobalObjects.inquiryCurrentParentIsGrouped == true)
                {
                    dt = DBConnection.getMSSQLDataTable(GlobalObjects.strPEIConnectionString, GlobalObjects.getPEIGetTopicProviderGroupSQL(lblKeyTopic.Text.Replace("'", "''"), _strMPIN));
                }
                else
                {
                    dt = DBConnection.getMSSQLDataTable(GlobalObjects.strPEIConnectionString, GlobalObjects.getPEIGetTopicProviderIndividualSQL(lblKeyTopic.Text.Replace("'", "''"), _strMPIN));
                }

                strKeyTopicId = dt.Rows[0]["key_topic_id"].ToString();
                strProviderId = dt.Rows[0]["org_prov_master_id"].ToString();



                tssAddEngagementToPEI.Text = "Adding Engagement to PEI...";

                //ADD ENGAGEMENT TO PEI
                strNotes = SharedDataTableFunctions.getConcatenatedListFromDatatable(_dtFullInquiryList, "Notes", Environment.NewLine, 2);
                //strNotes = SharedDataTableFunctions.getConcatenatedListFromDatatable(_dtInquiry, "Notes", Environment.NewLine, 2);
               objPEIReturn = DBConnection.getMSSQLExecuteScalar(GlobalObjects.strPEIConnectionString, GlobalObjects.getPEIInsertEngagementSQL(strProviderId, cmbMMDAssignment.SelectedValue.ToString(), GlobalObjects.strCurrentUser, strKeyTopicId, strNotes));
                if (objPEIReturn == null)
                {
                    MessageBox.Show("Engagement not added to PEI");
                    return;
                }
                else
                {
                   // if (dt.Rows[0]["is_open"].ToString().Equals("Closed"))
                       // strURL = GlobalObjects.strPEIClosedEngagementURL.Replace("{$eid}", objPEIReturn.ToString());
                   // else
                        strURL = GlobalObjects.strPEIOpenEngagementURL.Replace("{$eid}", objPEIReturn.ToString());

                }

                //ADD ADD TO PEI FLAG IN IL_UCA
                strChildIdCSV = SharedDataTableFunctions.getConcatenatedListFromDatatable(_dtFullInquiryList, "Provider InquiryId", ",", 1);
                //strChildIdCSV = SharedDataTableFunctions.getConcatenatedListFromDatatable(_dtInquiry, "qa_tracker_child_id", ",", 1);



                //OVERRIDE GROUP SENARIOS
                bool blIsGroup = false;
                if (GlobalObjects.inquiryCurrentParentIsGrouped== true)
                {
                    strChildIdCSV = String.Join(",", GlobalObjects.inquiryParentGroupList.Select(x => x.ToString()).ToArray());
                    blIsGroup = true;
                }






                objPEIReturn = DBConnection.getMSSQLExecuteScalar(GlobalObjects.strILUCAConnectionString, GlobalObjects.getBulkUpdateChildTrackerSQL(" pei_engagement_id = " + objPEIReturn + ", entered_in_pei = 1, date_entered_in_pei = getDate() ", strChildIdCSV, blIsGroup));
                if (objPEIReturn == null)
                {
                    MessageBox.Show("Engagement confirmation not added to Tracking Tool");
                    return;
                }
                if (int.Parse(objPEIReturn.ToString()) < 1)
                {
                    MessageBox.Show("Engagement confirmation not added to Tracking Tool");
                    return;
                }

               List<int> l =  GlobalObjects.inquiryChildGroupList;
                l = GlobalObjects.inquiryParentGroupList;


                tssAddEngagementToPEI.Text = "Sending Email to MMD...";
                //string strEmailMessage = getEmailNotes(_strMPIN, lblProviderName.Text, lblKeyTopic.Text, strURL, strNotes);
                string strEmailMessage = getEmailNotes(strURL);
                //SEND EMAIL
                if (GlobalObjects.strEnvironment == "Dev")
                {
                    blPassed = OutlookHelper.sendEmail(strCCEmail, strCCEmail, "An engagement has been added to your PEI work queue", strEmailMessage);
                }
                else
                {
                    blPassed = OutlookHelper.sendEmail(strMMDEmail, strCCEmail, "An engagement has been added to your PEI work queue", strEmailMessage);
                }

                if (!blPassed)
                {
                    MessageBox.Show("MMD email failed");
                    return;
                }


                //ADD MMD NOTIFIED FLAG IN IL_UCA
                objPEIReturn = DBConnection.getMSSQLExecuteScalar(GlobalObjects.strILUCAConnectionString, GlobalObjects.getBulkUpdateChildTrackerSQL(" mmd_notified = 1, date_mmd_notified = getDate() ", strChildIdCSV, blIsGroup));
                if (objPEIReturn == null)
                {
                    MessageBox.Show("MMD email confirmation not added to Tracking Tool");
                    return;
                }
                if (int.Parse(objPEIReturn.ToString()) < 1)
                {
                    MessageBox.Show("MMD email confirmation not added to Tracking Tool");
                    return;
                }

           
                MessageBox.Show("Engagement successfully created. MMD was notified. ");

            }
            catch (Exception ex)
            {

            }
            finally
            {
                tssAddEngagementToPEI.Text = "Ready";

                this.Cursor = Cursors.Default;

                Form frmEditTrackingItem = Application.OpenForms["frmEditTrackingItem"];
                if (frmEditTrackingItem != null)
                    ((frmEditTrackingItem)frmEditTrackingItem).populateGrid();
                this.Close();

            }




        }



        //private string getEmailNotes(string strMPIN, string strProviderName, string strProjectName, string strURL, string strNotes)
        //{
        //    StringBuilder sbNotes = new StringBuilder();

        //    sbNotes.Append("MPIN: " + strMPIN + Environment.NewLine + Environment.NewLine);
        //    sbNotes.Append("Provider Name: " + strProviderName + Environment.NewLine + Environment.NewLine);
        //    sbNotes.Append("Key Topic: " + strProjectName + Environment.NewLine + Environment.NewLine);
        //    sbNotes.Append("URL: " + strURL + Environment.NewLine + Environment.NewLine);
        //    //sbNotes.Append(Environment.NewLine + Environment.NewLine);
        //    sbNotes.Append("Notes: " + strNotes);

        //    return sbNotes.ToString();

        //}


        private string getEmailNotes(string strURL)
        {
            StringBuilder sbNotes = new StringBuilder();





            sbNotes.Append("MPIN: " + _dtFullInquiryList.Rows[0]["Physician MPIN"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Provider Name: " + _dtFullInquiryList.Rows[0]["Physician Name"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Provider Specialty: " + _dtFullInquiryList.Rows[0]["Physician Specialty"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Requester Name: " + _dtFullInquiryList.Rows[0]["Requester Name"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Requester Email: " + _dtFullInquiryList.Rows[0]["Requester Email"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Requester Phone: " + _dtFullInquiryList.Rows[0]["Requester Phone"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Requester Date: " + _dtFullInquiryList.Rows[0]["Requester Date"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Requester Role: " + _dtFullInquiryList.Rows[0]["Requester Role"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Source of Inquiry: " + _dtFullInquiryList.Rows[0]["Source of Inquiry"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Key Topic: " + _dtFullInquiryList.Rows[0]["PEI Project Name"] + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("URL: " + strURL + Environment.NewLine + Environment.NewLine);
            //sbNotes.Append(Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Notes from Affordability: " + SharedDataTableFunctions.getConcatenatedListFromDatatable(_dtFullInquiryList, "Notes", Environment.NewLine, 2) + Environment.NewLine + Environment.NewLine + Environment.NewLine);
            sbNotes.Append("Notes from Analytics: " + SharedDataTableFunctions.getConcatenatedListFromDatatable(_dtFullInquiryList, "Analytic Notes", Environment.NewLine, 2));






            return sbNotes.ToString();

        }




    }
}
