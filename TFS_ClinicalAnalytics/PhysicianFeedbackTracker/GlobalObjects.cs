using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;

namespace PhysicianFeedbackTracker
{
    static class GlobalObjects
    {

        //STATIC VARIABLES
        //STATIC VARIABLES
        //STATIC VARIABLES
        private static string _strCurrentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Replace("MS\\", "");
        public static string strCurrentUser
        {
            get { return _strCurrentUser; }
        }



        private static string _strCurrentEmail;
        public static string strCurrentEmail
        {
            get { return _strCurrentEmail; }
            set { _strCurrentEmail = value; }
        }



        //CONNECTION STRINGS
        //CONNECTION STRINGS
        //CONNECTION STRINGS

        private static string _strEnvironment = ConfigurationManager.AppSettings["Environment"];
        public static string strEnvironment
        {
            get { return _strEnvironment; }
        }

        //-----------------------------------------
        //Microsoft SQL Server Non-User ID: iluca_ucs_sql
        //DBSWS0047-IL_UCA Initial Password: LxX&56eb  
        //--------------------------------------------
        //Microsoft SQL Server Non-User ID: iluca_ucs_sql
        //DBSWP0063-IL_UCA Initial Password: YBZ=&Q6u  
        //-----------------------------------------------
        //Microsoft SQL Server Non-User ID: iluca_ucs_sql
        //DBSWD0039-IL_UCA Initial Password: H9:SRDTa  
        //-------------------------------------------------

        private static string _strILUCAUsername = "iluca_ucs_sql";//(_strEnvironment == "Dev" ? "pei2_sql_noprod" : "pei2_sql_prod");
        private static string _strILUCAPassword = (_strEnvironment == "Dev" ? "H9:SRDTa" : "YBZ=&Q6u");
        private static string _strILUCAHostname = (_strEnvironment == "Dev" ? "DBSWD0039" : "DBSWP0063");



        private static string _strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"].Replace("{$ilucadbhostname}", _strILUCAHostname).Replace("{$ilucadbusername}", _strILUCAUsername).Replace("{$ilucadbpassword}", _strILUCAPassword);
        public static string strILUCAConnectionString
        {
            get { return _strILUCAConnectionString; }
        }





        //Microsoft SQL Server Non-User ID: pei_ucs_sql
        //DBSED0112-PEIPortalDB Initial Password: vC+c93jV  
        //------------------------------------------------
        //Microsoft SQL Server Non-User ID: pei_ucs_sql
        //DBSWS0018-PEIPortalDB Initial Password: P3n%aThE  
        //------------------------------------------------
        //Microsoft SQL Server Non-User ID: pei_ucs_sql
        //DBSWP0306-PEIPortalDB Initial Password: Zt=Ct4=D


        //pei2_sql_noprod = EG7dAyH6
        //private static string _strPEIUsername = "pei2_sql_prod";
        //private static string _strPEIPassword = "b5kZ6Wvz";

        private static string _strPEIUsername = "pei_ucs_sql";//(_strEnvironment == "Dev" ? "pei2_sql_noprod" : "pei2_sql_prod");
        private static string _strPEIPassword = (_strEnvironment == "Dev" ? "vC+c93jV" : "Zt=Ct4=D");
        private static string _strPEIHostname = (_strEnvironment == "Dev" ? "DBSED0112" : "DBSWP0306");



        private static string _strPEIConnectionString = ConfigurationManager.AppSettings["PEI_Database"].Replace("{$peidbhostname}", _strPEIHostname).Replace("{$peidbusername}", _strPEIUsername).Replace("{$peidbpassword}", _strPEIPassword);
        public static string strPEIConnectionString
        {
            get { return _strPEIConnectionString; }
        }



        private static string _strExcelConnectionString = ConfigurationManager.AppSettings["ExcelConnectionString"];
        public static string strExcelConnectionString
        {
            get { return _strExcelConnectionString; }
        }

        private static string _strExcelXConnectionString = ConfigurationManager.AppSettings["ExcelXConnectionString"];
        public static string strExcelXConnectionString
        {
            get { return _strExcelXConnectionString; }
        }



        private static string _strUploadDocumentsPath = ConfigurationManager.AppSettings["Tracker_UploadPath"];
        public static string strUploadDocumentsPath
        {
            get { return _strUploadDocumentsPath; }
        }




        //PEI LINKS
        //PEI LINKS
        //PEI LINKS
        private static string _strPEIURLType = (_strEnvironment == "Dev" ? "-dev" : "");

        private static string _strPEIOpenEngagementURL = ConfigurationManager.AppSettings["PEI_OpenEngagement"];
        public static string strPEIOpenEngagementURL
        {
            get { return _strPEIOpenEngagementURL.Replace("{$prod-dev}", _strPEIURLType); }
        }

        private static string _strPEIClosedEngagementURL = ConfigurationManager.AppSettings["PEI_ClosedEngagement"];
        public static string strPEIClosedEngagementURL
        {
            get { return _strPEIClosedEngagementURL.Replace("{$prod-dev}", _strPEIURLType); }
        }

        private static string _strPEIDocumentsPath = ConfigurationManager.AppSettings["PEI_DocumentsPath"];
        public static string strPEIDocumentsPath
        {
            get { return _strPEIDocumentsPath; }
        }
        //SQL COMMANDS
        //SQL COMMANDS
        //SQL COMMANDS
        private static string _strGetNameValueSQL = "exec sp_cg_QATrackerTool_select_NameValuePairs '{@nameValue}';";
        public static string getNameValueSQL(string strNameValue)
        {
            return _strGetNameValueSQL.Replace("{@nameValue}", strNameValue);
        }

        private static string _strGetProviderSearchSQL = "sp_cg_QATrackerTool_select_provider";
        public static string strGetProviderSearchSQL
        {
            get { return _strGetProviderSearchSQL; }
        }
        public static Hashtable htProviderSearchSQL(string strSearchValue, string strPhaseId, string strRequestType)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@searchQuery", strSearchValue);
            ht.Add("@phase_id", strPhaseId);
            ht.Add("@requestType", strRequestType);

            return ht;

        }

        private static string[] _strGetProviderSearchExcludeArr = { "type", "SetNum" };
        public static string[] strGetProviderSearchExcludeArr
        {
            get { return _strGetProviderSearchExcludeArr; }
        }


        private static string _strBulkInsertProvidersToTrackerSQL = "exec sp_cg_QATrackerTool_execute_insert_update_delete 'DECLARE @OutputTbl TABLE (ID INT); INSERT INTO qa_tracker_parent (mpin,phase_id,inserted_by_nt_id) OUTPUT INSERTED.qa_tracker_parent_id INTO @OutputTbl(ID) {@sql}; SELECT * FROM @OutputTbl'";

        public static string getBulkInsertProvidersToTrackerSQL(string strSQL)
        {
            return _strBulkInsertProvidersToTrackerSQL.Replace("{@sql}", strSQL.TrimEnd('U', 'N', 'I', 'O', 'N', ' '));
        }




        private static string _strBulkUpdateProvidersGroupSQL = "exec sp_cg_QATrackerTool_execute_insert_update_delete 'DECLARE @qa_tracker_parent_group_id  INT; INSERT INTO qa_tracker_parent_group (tracker_parent_group_name) VALUES (''{$name}'');  SET @qa_tracker_parent_group_id = SCOPE_IDENTITY();   UPDATE qa_tracker_parent SET qa_tracker_parent_group_id =  @qa_tracker_parent_group_id,is_grouped = 1 WHERE   qa_tracker_parent_id in ({@sql}); SELECT @qa_tracker_parent_group_id;'";

        public static string getBulkUpdateProvidersGroupSQL(string strSQL, string strName)
        {
            return _strBulkUpdateProvidersGroupSQL.Replace("{@sql}", strSQL).Replace("{$name}", strName);
        }





        //private static string _strBulkInsertProvidersGroupSQL = "exec sp_cg_QATrackerTool_execute_insert_update_delete 'DECLARE @qa_tracker_parent_group_id  INT; INSERT INTO qa_tracker_parent_group (tracker_parent_group_name) VALUES (''Name'');  SET @qa_tracker_parent_group_id = SCOPE_IDENTITY();   INSERT INTO qa_tracker_parent_grouping (qa_tracker_parent_group_id,qa_tracker_parent_id)      {@sql};'";

        //public static string getBulkInsertProvidersGroupSQL(string strSQL)
        //{
        //    return _strBulkInsertProvidersGroupSQL.Replace("{@sql}", strSQL.TrimEnd('U', 'N', 'I', 'O', 'N', ' '));
        //}


        private static string _strInsertUpdateChildToTrackerSQL = "sp_cg_QATrackerTool_insert_update_tracker_child";
        public static string strInsertUpdateChildToTrackerSQL
        {
            get { return _strInsertUpdateChildToTrackerSQL; }
        }


        public static Hashtable htInsertUpdateChildToTrackerSQL(string str_qa_tracker_parent_id, string str_qa_tracker_child_id, string str_date_inquiry_received,  string str_inquiry_category_id, string str_inquiry_status_id, string str_member_detail_available_id, string str_date_resolved, string str_attestation_required, string str_date_attestation_sent, string str_date_attestation_received, string str_provider_tone_id, string str_date_member_detail_available, string str_resolved_analytics_date, string str_ready_analytics_date, string str_notes, string str_exclude_practice_mailing, string str_exclude_mpin_mailing, string str_reason_for_exclusion, string str_analytic_note, string str_assigned_pcr, string str_inserted_by_nt_id, string str_updated_by_nt_id, string str_parent_group_id, string str_child_group_id, string str_child_group_name, string str_is_grouped)
        {


           



            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_parent_id", str_qa_tracker_parent_id);
            ht.Add("@qa_tracker_child_id", str_qa_tracker_child_id);
            ht.Add("@inquiry_category_id", str_inquiry_category_id);
            ht.Add("@inquiry_status_id", str_inquiry_status_id);
            ht.Add("@member_detail_id", str_member_detail_available_id);
            ht.Add("@date_resolved", str_date_resolved);
            ht.Add("@attestation_required", str_attestation_required);
            ht.Add("@date_attestation_sent", str_date_attestation_sent);
            ht.Add("@date_attestation_received", str_date_attestation_received);
            ht.Add("@provider_tone_id", str_provider_tone_id);
            ht.Add("@date_member_detail_available", str_date_member_detail_available);
            ht.Add("@date_resolved_by_analytics", str_resolved_analytics_date);
            ht.Add("@date_ready_for_analytics", str_ready_analytics_date);
            ht.Add("@notes", str_notes);
            ht.Add("@inserted_by_nt_id", str_inserted_by_nt_id);
            ht.Add("@updated_by_nt_id", str_updated_by_nt_id);
            ht.Add("@date_inquiry_received", str_date_inquiry_received);


            ht.Add("@exclude_practice_mailing", str_exclude_practice_mailing);
            ht.Add("@exclude_mpin_mailing", str_exclude_mpin_mailing);
            ht.Add("@reason_for_exclusion", str_reason_for_exclusion);
            ht.Add("@analytic_notes", str_analytic_note);
            ht.Add("@assigned_pcr_nt_id", str_assigned_pcr);


            ht.Add("@qa_tracker_parent_group_id", str_parent_group_id);
            ht.Add("@qa_tracker_child_group_id", str_child_group_id);
            ht.Add("@tracker_child_group_name", str_child_group_name);
            ht.Add("@is_grouped", str_is_grouped);



            return ht;
        }



        private static string _strUpdateParentRequestSQL = "sp_cg_QATrackerTool_update_parent_request";
        public static string strUpdateParentRequestSQL
        {
            get { return _strUpdateParentRequestSQL; }
        }

        public static Hashtable htUpdateParentRequestSQL(string str_qa_tracker_parent_id, string str_requester_name, string str_requester_email, string str_requester_phone, string str_requester_date, string str_requester_role, string str_source_of_inquiry_id, string str_updated_by_nt_id, string strIsGrouped, string str_tracker_parent_group_id, string str_qa_tracker_parent_id_original)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_parent_id", str_qa_tracker_parent_id);
            ht.Add("@requester_name", str_requester_name);
            ht.Add("@requester_email", str_requester_email);
            ht.Add("@requester_phone", str_requester_phone);
            ht.Add("@requester_date", str_requester_date);
            ht.Add("@user_group_access_id", str_requester_role);
            ht.Add("@source_of_inquiry_id", str_source_of_inquiry_id);
            ht.Add("@updated_by_nt_id", str_updated_by_nt_id);
            ht.Add("@is_grouped", strIsGrouped);
            ht.Add("@qa_tracker_parent_group_id", str_tracker_parent_group_id);
            ht.Add("@qa_tracker_parent_id_original", str_qa_tracker_parent_id_original);
            return ht;
        }


        private static string _strDeleteTrackerItemSQL = "sp_cg_QATrackerTool_delete_tracker_item";
        public static string strDeleteTrackerItemSQL
        {
            get { return _strDeleteTrackerItemSQL; }
        }


        public static Hashtable htDeleteTrackerItemSQL(string strTrackingParentId, string strTrackingChildId = null)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_parent_id", strTrackingParentId);
            ht.Add("@qa_tracker_child_id", strTrackingChildId);

            return ht;
        }


        

        private static string[] _strTrackerRequestHideArr = { "qa_tracker_parent_id", "pei_project_name", "qa_tracker_parent_group_id", "is_grouped", "tracker_parent_group_name"};
        public static string[] strTrackerRequestHideArr
        {
            get { return _strTrackerRequestHideArr; }
        }


        private static string _strSelectTrackerRequestSQL = "sp_cg_QATrackerTool_select_parent_request";
        public static string strSelectTrackerRequestSQL
        {
            get { return _strSelectTrackerRequestSQL; }
        }

        public static Hashtable getSelectTrackerRequestSQL(string strPhaseId, string strInsertedByNTIdCSV, string strStartDate, string strEndDate, string strProviderSearch, string strTrackerStatus, string strInquiryCategory, string strInquiryStatus, string strMPINList, string strParentIdList)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@phase_id", strPhaseId);
            ht.Add("@inserted_by_nt_ids", strInsertedByNTIdCSV);
            ht.Add("@startDate", strStartDate);
            ht.Add("@endDate", strEndDate);
            ht.Add("@providerSearch", strProviderSearch);
            ht.Add("@isOpen", strTrackerStatus);
            ht.Add("@inquiry_category_id", strInquiryCategory);
            ht.Add("@inquiry_status_id", strInquiryStatus);

            ht.Add("@qa_tracker_parent_id", strParentIdList);
            ht.Add("@qa_mpin", strMPINList);

            return ht;
        }


        private static string[] _strTrackerChildRequestHideArr = { "qa_tracker_child_id", "qa_tracker_parent_id", "qa_tracker_child_group_id", "is_grouped", "tracker_child_group_name" };
        public static string[] strTrackerChildRequestHideArr
        {
            get { return _strTrackerChildRequestHideArr; }
        }


        private static string _strSelectTrackerChildRequestSQL = "exec sp_cg_QATrackerTool_select_child_request '{@qa_tracker_parent_id}'";

        public static string getSelectTrackerChildRequestSQL(string strTrackerParentId)
        {
            return _strSelectTrackerChildRequestSQL.Replace("{@qa_tracker_parent_id}", strTrackerParentId);
        }



        private static string _strSelectDuplicateCheckSQL = "exec sp_cg_QATrackerTool_execute_duplicate_check '{@sqlFilter}'";

        public static string getSelectDuplicateCheckSQL(string strFilterSQL)
        {
            return _strSelectDuplicateCheckSQL.Replace("{@sqlFilter}", strFilterSQL);
        }







        private static string _strSelectUnionSQL = "SELECT {@columns} UNION ";

        public static string getSelectUnionSQL(string strColumnsCSV)
        {
            return _strSelectUnionSQL.Replace("{@columns}", strColumnsCSV);
        }



        private static string _strSelectTrackerFullRequestSQL = "sp_cg_QATrackerTool_select_request_complete";
        public static string strSelectTrackerFullRequestSQL
        {
            get { return _strSelectTrackerFullRequestSQL; }
        }


        private static string _strProviderSmartSearchSQL = "sp_cg_QATrackerTool_provider_smart_search";
        public static string strProviderSmartSearchSQL
        {
            get { return _strProviderSmartSearchSQL; }
        }

        public static Hashtable htProviderSmartSearchSQL(string strPhaseId, string strProviderSearch)
        {

            Hashtable ht = new Hashtable();
            ht.Add("@phase_id", strPhaseId);
            ht.Add("@providerSearch", strProviderSearch);

            return ht;
        }



        private static string _strBulkUpdateChildTrackerIndividualSQL = "exec sp_cg_QATrackerTool_execute_insert_update_delete 'UPDATE qa_tracker_children SET {@columnSet} WHERE qa_tracker_child_id in ({@whereSet})';";

        private static string _strBulkUpdateChildTrackerGroupSQL = "exec sp_cg_QATrackerTool_execute_insert_update_delete 'UPDATE qa_tracker_children SET {@columnSet} WHERE qa_tracker_child_id in (select qa_tracker_child_id from qa_tracker_children WHERE qa_tracker_child_id in (SELECT qa_tracker_child_id FROM qa_tracker_children WHERE qa_tracker_parent_id in ({@whereSet}) AND inquiry_category_id = (SELECT inquiry_category_id FROM qa_tracker_inquiry_category WHERE inquiry_category_description = ''MMD Follow-up Request'')  AND inquiry_status_id = (SELECT inquiry_status_id FROM qa_tracker_inquiry_status WHERE inquiry_status_description = ''Pending MMD Action'')))';";

        //private static string _strBulkUpdateChildTrackerGroupSQL = "exec sp_cg_QATrackerTool_execute_insert_update_delete 'UPDATE qa_tracker_children SET {@columnSet} WHERE qa_tracker_child_id in (select qa_tracker_child_id from qa_tracker_children WHERE qa_tracker_child_id in (SELECT qa_tracker_child_id FROM qa_tracker_children WHERE qa_tracker_parent_id in ({@whereSet}) AND inquiry_category_id = (SELECT inquiry_category_id FROM qa_tracker_inquiry_category WHERE inquiry_category_description = ''MMD Follow-up Request'') AND ISNULL(entered_in_pei,0) = 0 AND inquiry_status_id = (SELECT inquiry_status_id FROM qa_tracker_inquiry_status WHERE inquiry_status_description = ''Pending MMD Action'')))';";

        public static string getBulkUpdateChildTrackerSQL(string strColumnSet, string strWhereSet, bool isGroup)
        {
            if(isGroup == false)
                return _strBulkUpdateChildTrackerIndividualSQL.Replace("{@columnSet}", strColumnSet).Replace("{@whereSet}", strWhereSet);
            else
                return _strBulkUpdateChildTrackerGroupSQL.Replace("{@columnSet}", strColumnSet).Replace("{@whereSet}", strWhereSet);

        }




        private static string _strGetUserEmailByUserIdSQL = "sp_cg_QATrackerTool_get_email_by_userid";
        public static string strGetUserEmailByUserIdSQL
        {
            get { return _strGetUserEmailByUserIdSQL; }
        }

        public static Hashtable htGetUserEmailByUserIdSQL(string strUserId)
        {

            Hashtable ht = new Hashtable();
            ht.Add("@user_nt_id", strUserId);
            return ht;
        }






        private static string _strInsertUpdateUserSQL = "sp_cg_QATrackerTool_insert_update_user";
        public static string strInsertUpdateUserSQL
        {
            get { return _strInsertUpdateUserSQL; }
        }
        public static Hashtable htInsertUpdateUserSQL(string strUserId, string strFirstName, string strLastName,  string strEmail)
        {

            Hashtable ht = new Hashtable();
            ht.Add("@user_nt_id", strUserId);
            ht.Add("@user_firstname", strFirstName);
            ht.Add("@user_lastname", strLastName);
            ht.Add("@user_email", strEmail);
            return ht;
        }






        private static string _strSelectParentGroupSQL = "sp_cg_QATrackerTool_select_parent_group";
        public static string strSelectParentGroupSQL
        {
            get { return _strSelectParentGroupSQL; }
        }


        public static Hashtable htSelectParentGroupSQL(string strTrackingParentId)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_parent_id", strTrackingParentId);

            return ht;
        }



        private static string _strSelectChildGroupSQL = "sp_cg_QATrackerTool_select_child_group";
        public static string strSelectChildGroupSQL
        {
            get { return _strSelectChildGroupSQL; }
        }


        public static Hashtable htSelectChildGroupSQL(string strTrackingChildId)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_child_id", strTrackingChildId);

            return ht;
        }





        private static string _strUpdateChildGroupSQL = "sp_cg_QATrackerTool_update_child_group_name";
        public static string strUpdateChildGroupSQL
        {
            get { return _strUpdateChildGroupSQL; }
        }


        public static Hashtable htUpdateChildGroupSQL(string strChildGroupId, string strTrackingChildGroupName = null)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_child_group_id", strChildGroupId);
            ht.Add("@tracker_child_group_name", strTrackingChildGroupName);

            return ht;
        }



        private static string _strUpdateParentGroupSQL = "sp_cg_QATrackerTool_update_parent_group";
        public static string strUpdateParentGroupSQL
        {
            get { return _strUpdateParentGroupSQL; }
        }


        public static Hashtable htUpdateParentGroupSQL(string strTrackerParentId, string strTrackingParentGroupId, bool isGrouped)
        {

            Hashtable ht = new Hashtable();

            ht.Add("@qa_tracker_parent_id", strTrackerParentId);
            ht.Add("@qa_tracker_parent_group_id", strTrackingParentGroupId);
            ht.Add("@isGrouped", isGrouped);

            return ht;
        }




        private static string _strGetProviderDetailsSearchSQL = "sp_cg_QATrackerTool_select_provider_details";
        public static string strGetProviderDetailsSearchSQL
        {
            get { return _strGetProviderDetailsSearchSQL; }
        }
        public static Hashtable htProviderDetailsSearchSQL(string strSearchValue)
        {
            Hashtable ht = new Hashtable();

            ht.Add("@searchQuery", strSearchValue);

            return ht;

        }





        //PEI SQL
        //PEI SQL
        //PEI SQL
        private static string _strGetAdUserNamePassword = "Select nt_nexun, nt_nexpw from PEI2_nt_nexpw";
        public static string strGetAdUserNamePassword
        {
            get { return _strGetAdUserNamePassword; }
        }



        private static string _strPEIEngagementSQL = "Select top 1 engagement_id, is_open from dbo.vw_PEI2_Engagement_FullSet where key_topic_description = '{@key_topic_description}' and mpin = {@mpin};";

        public static string getPEIEngagementSQL(string strKeyTopicDescription, string strMPIN)
        {
            return _strPEIEngagementSQL.Replace("{@key_topic_description}", strKeyTopicDescription).Replace("{@mpin}", strMPIN);
        }



        //private static string _strPEIKeyTopicCheckSQL = "select TOP 1 key_topic_id from PEI2_key_topic where is_archived = 0 and key_topic_description = '{@key_topic_description}';";

        //public static string getPEIKeyTopicCheckSQL(string strKeyTopicDescription)
        //{
        //    return _strPEIKeyTopicCheckSQL.Replace("{@key_topic_description}", strKeyTopicDescription);
        //}


        //private static string _strPEIProviderCheckSQL = "select TOP 1 org_prov_master_id from PEI2_org_prov_master where mpin = {@mpin};";

        //public static string getPEIProviderCheckSQL(string strMPIN)
        //{
        //    return _strPEIProviderCheckSQL.Replace("{@mpin}", strMPIN);
        //}





        private static string _strPEIGetTopicProviderIndividualSQL = "SELECT v.org_prov_master_id, v.key_topic_id FROM vw_PEI2_Engagement_FullSet v WHERE v.mpin = {@mpin} and v.key_topic_description = '{@key_topic_description}' ;";

        public static string getPEIGetTopicProviderIndividualSQL(string strKeyTopicDescription, string strMPIN)
        {
            return _strPEIGetTopicProviderIndividualSQL.Replace("{@key_topic_description}", strKeyTopicDescription).Replace("{@mpin}", strMPIN); ;
        }


        private static string _strPEIGetTopicProviderGroupSQL = "SELECT org_prov_master_id_p as org_prov_master_id, key_topic_id FROM PEI2_org_prov_grp_sys where org_prov_master_id_c = (select TOP 1 org_prov_master_id from PEI2_org_prov_master where mpin = {@mpin}) AND key_topic_id in (select key_topic_id from PEI2_key_topic WHERE key_topic_description = '{@key_topic_description}' ) ;";

        public static string getPEIGetTopicProviderGroupSQL(string strKeyTopicDescription, string strMPIN)
        {
            return _strPEIGetTopicProviderGroupSQL.Replace("{@key_topic_description}", strKeyTopicDescription).Replace("{@mpin}", strMPIN); ;
        }









        //private static string _strPEIInsertEngagementSQL = " DECLARE @RowCount INTEGER; DECLARE @iEngagementId INT;    INSERT INTO PEI2_engagement ( org_prov_master_id, Cohort, assigned_username,priority, is_open, is_archived, is_abandoned, is_live,  start_date, engagement_cohort_id ,insert_date, insert_data_source, project_manager_username, has_letter, is_project, gap_count, additional_notes)  VALUES ({@provider_id}, 'P2P', '{@assigned_username}', 1, 1, 0, 0, 1, GETDATE(),(SELECT engagement_cohort_id from dbo.PEI2_engagement_cohort where cohort_description = 'P2P'),getDate(), 'QA_TRACKER', '{@current_user}', 0, 1,0, '{@notes}');    SET @iEngagementId = SCOPE_IDENTITY();   INSERT INTO PEI2_engagement_key_topic (key_topic_id,engagement_id, audience_response, insert_date )  VALUES ({@key_topic_id},@iEngagementId ,'No Response', getDate()); SET @RowCount = @@ROWCOUNT; SELECT @RowCount; ";


        private static string _strPEIInsertEngagementSQL = " DECLARE @RowCount INTEGER; DECLARE @iEngagementId INT;    INSERT INTO PEI2_engagement ( org_prov_master_id, Cohort, assigned_username,priority, is_open, is_archived, is_abandoned, is_live,  start_date, engagement_cohort_id ,insert_date, insert_data_source, project_manager_username, has_letter, is_project, gap_count, additional_notes)  VALUES ({@provider_id}, 'P2P', '{@assigned_username}', 1, 1, 0, 0, 1, GETDATE(),(SELECT engagement_cohort_id from dbo.PEI2_engagement_cohort where cohort_description = 'P2P'),getDate(), 'QA_TRACKER', '{@current_user}', 0, 1,0, '{@notes}');    SET @iEngagementId = SCOPE_IDENTITY();   INSERT INTO PEI2_engagement_key_topic (key_topic_id,engagement_id, audience_response, insert_date )  VALUES ({@key_topic_id},@iEngagementId ,'No Response', getDate());  SELECT @iEngagementId; ";



        public static string getPEIInsertEngagementSQL(string str_provider_id, string str_assigned_username, string str_current_user, string str_key_topic_id, string str_notes)
        {
            return _strPEIInsertEngagementSQL.Replace("{@provider_id}", str_provider_id).Replace("{@assigned_username}", str_assigned_username).Replace("{@current_user}", str_current_user).Replace("{@key_topic_id}", str_key_topic_id).Replace("{@notes}", str_notes.Replace("'", "''"));
        }



        //public static string getPEIMMDAssignmentsSQL(DataTable dt)
        //{
        //    string strSQLTemplate = "Select key_topic_description, mpin, assigned_username  from dbo.vw_PEI2_Engagement_FullSet where key_topic_description in ({$keyTopicDescription}) and mpin in ({$mpinList})";
        //    StringBuilder sbSQLFinal = new StringBuilder();
        //    StringBuilder sbListTmp = new StringBuilder();


        //    return sbSQLFinal.ToString();

        //}






        //DATATABLE CACHING
        //DATATABLE CACHING
        //DATATABLE CACHING
        private static DataTable _dtNameValueCache;
        public static DataTable getNameValueDataTable(string strCategory)
        {

            if (_dtNameValueCache == null)//FIRST TIME
            {
                _dtNameValueCache = DBConnection.getMSSQLDataTable(_strILUCAConnectionString, getNameValueSQL(strCategory));
                return _dtNameValueCache.Copy();
            }
                

            DataTable dtTmp;
            DataRow[] drTmp = _dtNameValueCache.Select("category = '" + strCategory + "'");
            if (drTmp.Length != 0)
            {
                dtTmp = drTmp.CopyToDataTable();
            }
            else  //NOT IN CACHE
            {
                dtTmp = DBConnection.getMSSQLDataTable(_strILUCAConnectionString, getNameValueSQL(strCategory));
                //ADD TO CACHE
                if (_dtNameValueCache == null)
                    _dtNameValueCache = new DataTable();

                _dtNameValueCache.Merge(dtTmp,true, MissingSchemaAction.Ignore);
            }
            return dtTmp;
            
        }


        private static DataTable _dtTrackingParentCache;
        public static DataTable dtTrackingParentCache
        {
            get { return _dtTrackingParentCache; }
            set { _dtTrackingParentCache = value; }
        }




        private static string _argumentFilterParentIdString;
        public static string argumentFilterParentIdString
        {
            get { return _argumentFilterParentIdString; }
            set { _argumentFilterParentIdString = value; }
        }


        private static string _argumentFilterMPINString;
        public static string argumentFilterMPINString
        {
            get { return _argumentFilterMPINString; }
            set { _argumentFilterMPINString = value; }
        }







        private static List<int> _inquiryParentGroupList;
        public static List<int> inquiryParentGroupList
        {
            get { return _inquiryParentGroupList; }
            set { _inquiryParentGroupList = value; }
        }

        private static string _inquiryCurrentParentGroupId;
        public static string  inquiryCurrentParentGroupId
        {
            get { return _inquiryCurrentParentGroupId; }
            set { _inquiryCurrentParentGroupId = value; }
        }


        private static string _inquiryCurrentParentGroupName;
        public static string inquiryCurrentParentGroupName
        {
            get { return _inquiryCurrentParentGroupName; }
            set { _inquiryCurrentParentGroupName = value; }
        }

        private static bool? _inquiryCurrentParentIsGrouped;
        public static bool? inquiryCurrentParentIsGrouped
        {
            get { return _inquiryCurrentParentIsGrouped; }
            set { _inquiryCurrentParentIsGrouped = value; }
        }


        private static List<int> _inquiryChildGroupList;
        public static List<int> inquiryChildGroupList
        {
            get { return _inquiryChildGroupList; }
            set { _inquiryChildGroupList = value; }
        }

        private static string _inquiryCurrentChildGroupId;
        public static string inquiryCurrenChildGroupId
        {
            get { return _inquiryCurrentChildGroupId; }
            set { _inquiryCurrentChildGroupId = value; }
        }

        private static string _inquiryCurrentChildGroupName;
        public static string inquiryCurrentChildGroupName
        {
            get { return _inquiryCurrentChildGroupName; }
            set { _inquiryCurrentChildGroupName = value; }
        }

        private static bool? _inquiryCurrentChildIsGrouped;
        public static bool? inquiryCurrentChildIsGrouped
        {
            get { return _inquiryCurrentChildIsGrouped; }
            set { _inquiryCurrentChildIsGrouped = value; }
        }




        public static void clearParentGroups()
        {
            _inquiryParentGroupList = null;
            _inquiryCurrentParentGroupId = null;
            _inquiryCurrentParentGroupName = null;
            _inquiryCurrentParentIsGrouped = null;

        }
        public static void clearChildGroups()
        {
            _inquiryChildGroupList = null;
            _inquiryCurrentChildGroupId = null;
            _inquiryCurrentChildGroupName = null;
            _inquiryCurrentChildIsGrouped = null;

        }



        public static void populateParentGroups(string parentId, string parentGroupId, string parentGroupName, string isGrouped)
        {
            clearParentGroups();
            clearChildGroups();

            _inquiryCurrentParentGroupId = parentGroupId;
            _inquiryCurrentParentGroupName = parentGroupName;
            _inquiryCurrentParentIsGrouped = bool.Parse((String.IsNullOrEmpty(isGrouped) ? "false" : isGrouped));


            if(parentId != null)
            {
                _inquiryParentGroupList = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectParentGroupSQL, GlobalObjects.htSelectParentGroupSQL(parentId)).AsEnumerable().Select(r => r.Field<int>("qa_tracker_parent_id")).ToList();

                if (_inquiryParentGroupList.Count < 1 || inquiryCurrentParentIsGrouped == false)
                    _inquiryParentGroupList = new List<int> { int.Parse(parentId) };
            }


        }
        public static void populateChildGroups(string childId, string childGroupId, string childGroupName, string isGrouped)
        {
            clearChildGroups();

            _inquiryCurrentChildGroupId = childGroupId;
            _inquiryCurrentChildGroupName = childGroupName;
            _inquiryCurrentChildIsGrouped = bool.Parse(isGrouped);

            _inquiryChildGroupList = DBConnection.getMSSQLDataTableSP(GlobalObjects.strILUCAConnectionString, GlobalObjects.strSelectChildGroupSQL, GlobalObjects.htSelectChildGroupSQL(childId)).AsEnumerable().Select(r => r.Field<int>("qa_tracker_child_id")).ToList();

            if (_inquiryChildGroupList.Count < 1 || inquiryCurrentChildIsGrouped == false)
                _inquiryChildGroupList = new List<int> { int.Parse(childId) };


        }




        //OBJECT CACHING
        //OBJECT CACHING
        //OBJECT CACHING
        private static List<UserAccess> _userAccessList;
        public static UserAccess getUserAccess(string strUsername, string strGroupName)
        {
            UserAccess userAccessTmp= null;

            if (_userAccessList == null)
            {
                _userAccessList = new List<UserAccess>();
            }
           else
            {
                userAccessTmp = _userAccessList.Find(r => r.ntGroup == strGroupName);
            }

            if(userAccessTmp == null)
            {
                userAccessTmp = new UserAccess();
                userAccessTmp.ntGroup = strGroupName;
                userAccessTmp.blHasPermission = ActiveDirectoryFunctions.isUserInGroups(strUsername, strGroupName);
                _userAccessList.Add(userAccessTmp);
            }

            return userAccessTmp;

        }


    }


    public class UserAccess
    {
        public string ntGroup;
        public bool blHasPermission;
    }


}
