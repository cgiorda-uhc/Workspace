using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XmlConfiguration;

namespace PhysicianFeedbackTrackerDataLoader
{
    class PhysicianFeedbackTrackerDataLoader
    {
        static void Main(string[] args)
        {
            generateProviderScripts();
        }


        private static void generateProviderScripts()
        {
            // string strConnectionString = ConfigurationManager.AppSettings["PEILocal"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strPEIConnectionString = ConfigurationManager.AppSettings["PEIProd"];
            string strScriptPath = ConfigurationManager.AppSettings["ScriptPath"];

            int intTotalRows,intCnt = 0;

            StringBuilder sbSQLScript = new StringBuilder();
            StringBuilder sbSQLMMDIn = new StringBuilder();

            DataTable dtMain = null;



            StreamWriter swProviderMasterScript = new StreamWriter(strScriptPath + "provider_tracker_demographics_updatemaster.sql", false);

            StreamWriter swTracker_UsersScript = new StreamWriter(strScriptPath + "qa_tracker_users_updatemaster.sql", false);


            string str_phase_id, str_tin, str_mpin, str_physician_first_name, str_physician_last_name, str_physician_full_name, str_physician_street, str_physician_city, str_physician_state, str_physician_zip_code,
str_physician_specialty, str_physician_market, str_physician_market_number, str_physician_mmd, str_practice_mpin, str_practice_name, str_practice_street, str_practice_city, str_practice_state,
str_practice_zip_code, str_practice_market, str_practice_market_number, str_practice_mmd, str_pei_project;


            string strMMDTmp = null;

            string strSQL = "SELECT 1 as [phase_id] ,'PCP Cohort 1' as phase_description,'Performance Reports 151117' as pei_project ,d.UHN_TIN as tin ,o.mpin as phys_id ,o.NDB_Specialty as phys_specialty ,d.PTIGroupID as practice_id ,t.CorpOwnerName as practice_name ,FirstName as phys_FirstName ,LastName as phys_LastName ,d.Street as phys_Street ,d.City as phys_City ,d.State as phys_State ,d.zipcd as phys_zipcd ,d.MarketNbr as phys_MarketNbr ,d.MKT_RLLP_NM as phys_Market ,m.assigned_username as phys_mmd_username ,t.Street as pract_Street ,t.City as pract_City ,t.State as pract_State ,t.zipcd as pract_zipcd ,t.MarketNbr as pract_MarketNbr ,t.MKT_RLLP_NM as pract_Market ,m1.assigned_username as pract_mmd_username from dbo.PBP_outl_ph1 as o inner join dbo.PBP_outl_demogr_ph1 as d on o.mpin=d.mpin inner join dbo.PBP_outl_TIN_addr_ph1 as t on t.TaxID=d.UHN_TIN inner join dbo.MMD_Assignment_master as m on convert(decimal(10,0),m.market_nbr)=convert(decimal(10,0),d.MarketNbr) inner join dbo.MMD_Assignment_master as m1 on convert(decimal(10,0),m1.market_nbr)=convert(decimal(10,0),t.MarketNbr) where o.exclude in(0,4,7,8,9)UNION SELECT 2 as [phase_id] ,'OBGYN Cohort 1' as phase_description,'PR OB 82016' as pei_project , d.TaxID as tin ,o.mpin as phys_id ,o.NDB_Specialty as phys_specialty ,t.PTIGroupID as practice_id ,t.Name as practice_name ,FirstName as phys_FirstName ,LastName as phys_LastName ,d.Street as phys_Street ,d.City as phys_City ,d.State as phys_State ,d.zipcd as phys_zipcd ,d.MarketNbr as phys_MarketNbr ,d.MKT_RLLP_NM as phys_Market ,m.assigned_username as phys_mmd_username ,t.Street as pract_Street ,t.City as pract_City ,t.State as pract_State ,t.zipcd as pract_zipcd ,t.MarketNbr as pract_MarketNbr ,t.MKT_RLLP_NM as pract_Market ,m1.assigned_username as pract_mmd_username from dbo.PBP_outl_ph2 as o inner join dbo.PBP_outl_demogr_ph2 as d on o.mpin=d.mpin inner join dbo.PBP_outl_TIN_addr_ph2 as t on t.TaxID=d.taxid inner join dbo.MMD_Assignment_master as m on convert(decimal(10,0),m.market_nbr)=convert(decimal(10,0),d.MarketNbr) inner join dbo.MMD_Assignment_master as m1 on convert(decimal(10,0),m1.market_nbr)=convert(decimal(10,0),t.MarketNbr) where o.exclude in(0,5,10)UNION SELECT 3 as [phase_id] ,'Specialties Cohort 1' as phase_description ,'PR Spec Oct2016' as pei_project ,d.TaxID as tin ,o.mpin as phys_id ,o.NDB_Specialty as phys_specialty ,t.PTIGroupID as practice_id ,t.Name as practice_name ,FirstName as phys_FirstName ,LastName as phys_LastName ,d.Street as phys_Street ,d.City as phys_City ,d.State as phys_State ,d.zipcd as phys_zipcd ,d.MarketNbr as phys_MarketNbr ,d.MKT_RLLP_NM as phys_Market ,m.assigned_username as phys_mmd_username ,t.Street as pract_Street ,t.City as pract_City ,t.State as pract_State ,t.zipcd as pract_zipcd ,t.MarketNbr as pract_MarketNbr ,t.MKT_RLLP_NM as pract_Market ,m1.assigned_username as pract_mmd_username from dbo.PBP_outl_ph3 as o inner join dbo.PBP_outl_demogr_ph3 as d on o.mpin=d.mpin inner join dbo.PBP_outl_TIN_addr_ph3 as t on t.TaxID=d.taxid inner join dbo.MMD_Assignment_master as m on convert(decimal(10,0),m.market_nbr)=convert(decimal(10,0),d.MarketNbr) inner join dbo.MMD_Assignment_master as m1 on convert(decimal(10,0),m1.market_nbr)=convert(decimal(10,0),t.MarketNbr) where o.exclude in(0,5)UNION SELECT 12 as [phase_id] ,'PCP Cohort 2' as phase_description,'NONE_YET' as pei_project ,d.TaxID as tin ,o.mpin as phys_id ,o.NDB_Specialty as phys_specialty ,t.MPIN as practice_id ,t.Practice_Name as practice_name ,FirstName as phys_FirstName ,LastName as phys_LastName ,d.Street as phys_Street ,d.City as phys_City ,d.State as phys_State ,d.zipcd as phys_zipcd ,d.MarketNbr as phys_MarketNbr ,d.MKT_RLLP_NM as phys_Market ,m.assigned_username as phys_mmd_username ,t.Street as pract_Street ,t.City as pract_City ,t.State as pract_State ,t.zipcd as pract_zipcd ,t.MarketNbr as pract_MarketNbr ,t.MKT_RLLP_NM as pract_Market ,m1.assigned_username as pract_mmd_username from dbo.PBP_outl_ph12 as o inner join dbo.PBP_outl_demogr_ph12 as d on o.mpin=d.mpin inner join dbo.PBP_outl_PTI_addr_Ph12 as t on t.mpin=d.PTIGroupID_upd inner join dbo.MMD_Assignment_master as m on convert(decimal(10,0),m.market_nbr)=convert(decimal(10,0),d.MarketNbr) inner join dbo.MMD_Assignment_master as m1 on convert(decimal(10,0),m1.market_nbr)=convert(decimal(10,0),t.MarketNbr) where o.exclude in(0,5)";

            dtMain = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, strSQL);


            if (dtMain.Rows.Count > 0)
            {
                Console.WriteLine("INDIVIDUAL PROVIDER MASTER START");


                intTotalRows = dtMain.Rows.Count;
                intCnt = 0;
                foreach (DataRow dr in dtMain.Rows)
                {

                    str_phase_id = (string.IsNullOrEmpty(dr["phase_id"].ToString()) ? "NULL" : "'" + dr["phase_id"].ToString().Trim().Replace("'", "''") + "'");
                    str_tin = (string.IsNullOrEmpty(dr["tin"].ToString()) ? "NULL" : dr["tin"].ToString().Trim());
                    str_mpin = (string.IsNullOrEmpty(dr["phys_id"].ToString()) ? "NULL" : dr["phys_id"].ToString().Trim());
                    str_physician_first_name = (string.IsNullOrEmpty(dr["phys_FirstName"].ToString()) ? "NULL" :  dr["phys_FirstName"].ToString().Trim().Replace("'", "''"));
                    str_physician_last_name = (string.IsNullOrEmpty(dr["phys_LastName"].ToString()) ? "NULL" :  dr["phys_LastName"].ToString().Trim().Replace("'", "''"));
                    str_physician_street = (string.IsNullOrEmpty(dr["phys_Street"].ToString()) ? "NULL" : "'" + dr["phys_Street"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_city = (string.IsNullOrEmpty(dr["phys_City"].ToString()) ? "NULL" : "'" + dr["phys_City"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_state = (string.IsNullOrEmpty(dr["phys_State"].ToString()) ? "NULL" : "'" + dr["phys_State"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_zip_code = (string.IsNullOrEmpty(dr["phys_zipcd"].ToString()) ? "NULL" : "'" + dr["phys_zipcd"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_specialty = (string.IsNullOrEmpty(dr["phys_specialty"].ToString()) ? "NULL" : "'" + dr["phys_specialty"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_market = (string.IsNullOrEmpty(dr["phys_Market"].ToString()) ? "NULL" : "'" + dr["phys_Market"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_market_number = (string.IsNullOrEmpty(dr["phys_MarketNbr"].ToString()) ? "NULL" : "'" + dr["phys_MarketNbr"].ToString().Trim().Replace("'", "''") + "'");
                    str_physician_mmd = (string.IsNullOrEmpty(dr["phys_mmd_username"].ToString()) ? "NULL" : "'" + dr["phys_mmd_username"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_mpin = (string.IsNullOrEmpty(dr["practice_id"].ToString()) ? "NULL" : dr["practice_id"].ToString().Trim());
                    str_practice_name = (string.IsNullOrEmpty(dr["practice_name"].ToString()) ? "NULL" : "'" + dr["practice_name"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_street = (string.IsNullOrEmpty(dr["pract_Street"].ToString()) ? "NULL" : "'" + dr["pract_Street"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_city = (string.IsNullOrEmpty(dr["pract_City"].ToString()) ? "NULL" : "'" + dr["pract_City"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_state = (string.IsNullOrEmpty(dr["pract_State"].ToString()) ? "NULL" : "'" + dr["pract_State"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_zip_code = (string.IsNullOrEmpty(dr["pract_zipcd"].ToString()) ? "NULL" : "'" + dr["pract_zipcd"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_market = (string.IsNullOrEmpty(dr["pract_Market"].ToString()) ? "NULL" : "'" + dr["pract_Market"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_market_number = (string.IsNullOrEmpty(dr["pract_MarketNbr"].ToString()) ? "NULL" : "'" + dr["pract_MarketNbr"].ToString().Trim().Replace("'", "''") + "'");
                    str_practice_mmd = (string.IsNullOrEmpty(dr["pract_mmd_username"].ToString()) ? "NULL" : "'" + dr["pract_mmd_username"].ToString().Trim().Replace("'", "''") + "'");
                    str_pei_project = dr["pei_project"].ToString();

                    str_physician_full_name = ((str_physician_first_name != "NULL" ? str_physician_first_name.Trim() : "") + " " + str_physician_last_name.Trim()).Trim();
                    if (str_physician_full_name != "NULL") str_physician_full_name = "'" + str_physician_full_name + "'";
                    if (str_physician_first_name != "NULL") str_physician_first_name = "'" + str_physician_first_name + "'";
                    if (str_physician_last_name != "NULL") str_physician_last_name = "'" + str_physician_last_name + "'";


                    strMMDTmp = (string)DBConnection64.getMSSQLExecuteScalar(strPEIConnectionString, "Select TOP 1 assigned_username from dbo.vw_PEI2_Engagement where mpin = " + str_mpin + " and key_topic_description = '" + str_pei_project + "'");
                    if (strMMDTmp != null)
                        str_physician_mmd = "'" + strMMDTmp + "'";
                    strMMDTmp = null;

                    strMMDTmp = (string)DBConnection64.getMSSQLExecuteScalar(strPEIConnectionString, "Select TOP 1 assigned_username from dbo.vw_PEI2_Engagement where mpin = " + str_practice_mpin + " and key_topic_description = '" + str_pei_project + "'");
                    if (strMMDTmp != null)
                        str_practice_mmd = "'" + strMMDTmp + "'";
                    strMMDTmp = null;


                    if (!sbSQLMMDIn.ToString().Contains(str_physician_mmd + ","))
                        sbSQLMMDIn.Append(str_physician_mmd + ",");

                    if (!sbSQLMMDIn.ToString().Contains(str_practice_mmd + ","))
                        sbSQLMMDIn.Append(str_practice_mmd + ",");

                    sbSQLScript.Append("IF NOT EXISTS(SELECT 1 FROM qa_tracker_provider_demographics WHERE mpin = " + str_mpin + " AND phase_id = "+ str_phase_id + ") ");
                    sbSQLScript.Append("INSERT INTO qa_tracker_provider_demographics ");
                    sbSQLScript.Append("(phase_id,tin,mpin,physician_first_name,physician_last_name,physician_full_name,physician_street,physician_city,physician_state,physician_zip_code,physician_specialty,physician_market,physician_market_number,physician_mmd, physician_mmd_iluca,practice_mpin,practice_name,practice_street,practice_city,practice_state,practice_zip_code,practice_market,practice_market_number,practice_mmd, practice_mmd_iluca) ");
                    sbSQLScript.Append("VALUES ");
                    sbSQLScript.Append("(" + str_phase_id + "," + str_tin + ", " + str_mpin + ", " + str_physician_first_name + ", " + str_physician_last_name + ", " + str_physician_full_name + ", " + str_physician_street + ", " + str_physician_city + " , " + str_physician_state + " , " + str_physician_zip_code + " , " + str_physician_specialty + " , " + str_physician_market + ", " + str_physician_market_number + " , " + str_physician_mmd + ", " + str_physician_mmd + ", " + str_practice_mpin + " , " + str_practice_name + ", " + str_practice_street + ", " + str_practice_city + ", " + str_practice_state + ", " + str_practice_zip_code + ", " + str_practice_market + " , " + str_practice_market_number + ", " + str_practice_mmd + ", " + str_practice_mmd +  ") ");
                    sbSQLScript.Append(";");


                    swProviderMasterScript.WriteLine(sbSQLScript.ToString());
                    sbSQLScript.Remove(0, sbSQLScript.Length);

                    intCnt++;
                    Console.WriteLine("Row " + intCnt + " of " + intTotalRows + " : INDIVIDUAL PROVIDER MASTER INSERT, PHASE = " + str_phase_id + " AND MPIN = " + str_mpin );

                    if (intCnt % 50 == 0)
                    {

                        swProviderMasterScript.WriteLine(" GO ");
                        swProviderMasterScript.Flush();
                    }

                    //if (intCnt == 70)
                    //    break;

                }

                swProviderMasterScript.Flush();
                swProviderMasterScript.Close();
            }


            strSQL = "SELECT username, first_name,last_name, email, phone FROM PEI2_user_list where is_archived = 0 and username in ("+ sbSQLMMDIn.ToString().TrimEnd(',') + ")";
            dtMain = DBConnection64.getMSSQLDataTable(strPEIConnectionString, strSQL);
            sbSQLScript.Remove(0, sbSQLScript.Length);


            string str_username ;
            string str_first_name;
            string str_last_name;
            string str_full_name;
            string str_email;
            string str_phone;



            foreach (DataRow dr in dtMain.Rows)
            {

                str_username = (string.IsNullOrEmpty(dr["username"].ToString()) ? "NULL" : "'" + dr["username"].ToString().Trim().Replace("'", "''") + "'");
                str_first_name = (string.IsNullOrEmpty(dr["first_name"].ToString()) ? "NULL" : dr["first_name"].ToString().Trim().Replace("'", "''"));
                str_last_name = (string.IsNullOrEmpty(dr["last_name"].ToString()) ? "NULL" : dr["last_name"].ToString().Trim().Replace("'", "''"));
                str_email = (string.IsNullOrEmpty(dr["email"].ToString()) ? "NULL" : "'" + dr["email"].ToString().Trim().Replace("'", "''") + "'");
                str_phone = (string.IsNullOrEmpty(dr["phone"].ToString()) ? "NULL" : "'" + dr["phone"].ToString().Trim().Replace("'", "''") + "'");



                str_full_name = ((str_first_name != "NULL" ? str_first_name.Trim() : "") + " " + str_last_name.Trim()).Trim();
                if (str_full_name != "NULL") str_full_name = "'" + str_full_name + "'";
                if (str_first_name != "NULL") str_first_name = "'" + str_first_name + "'";
                if (str_last_name != "NULL") str_last_name = "'" + str_last_name + "'";



                sbSQLScript.Append("IF NOT EXISTS(SELECT 1 FROM dbo.qa_tracker_users WHERE user_nt_id = " + str_username + ") ");
                sbSQLScript.Append("INSERT INTO dbo.qa_tracker_users ");
                sbSQLScript.Append("(user_nt_id,user_fullname,user_email,user_phone,user_firstname,user_lastname,user_group_access_id) ");
                sbSQLScript.Append("VALUES ");
                sbSQLScript.Append("(" + str_username + "," + str_full_name + ", " + str_email + ", " + str_phone + ", " + str_first_name + ", " + str_last_name + ", (SELECT user_group_access_id FROM dbo.qa_tracker_user_groups WHERE user_group_access_name = 'MMD'));");
                sbSQLScript.Append(" ELSE ");
                sbSQLScript.Append("UPDATE dbo.qa_tracker_users SET user_fullname = "+ str_full_name + ", user_email = "+ str_email + ", user_phone = "+ str_phone + ", user_firstname = "+ str_first_name + ", user_lastname = "+ str_last_name + ", user_group_access_id = (SELECT user_group_access_id FROM dbo.qa_tracker_user_groups WHERE user_group_access_name = 'MMD') WHERE  user_nt_id = " + str_username + "; ");
                sbSQLScript.Append(";");


                swTracker_UsersScript.WriteLine(sbSQLScript.ToString());
                sbSQLScript.Remove(0, sbSQLScript.Length);
            }


            swTracker_UsersScript.Flush();
            swTracker_UsersScript.Close();

        }

    }
}
