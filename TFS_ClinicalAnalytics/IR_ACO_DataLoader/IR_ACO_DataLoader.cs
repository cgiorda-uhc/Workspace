using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IR_ACO_DataLoader
{
    static class IR_ACO_DataLoader
    {
        static void Main(string[] args)
        {
            //select f.*, d.[date_stamp] FROM [dbo].[ACO_COMM_RAW]  f INNER JOIN[dbo].[ACO_COMM_RAW_DATES] d ON d.aco_comm_id = f.aco_comm_id
            //select f.*, d.[date_stamp] FROM [dbo].[ACO_MR_RAW] f INNER JOIN [dbo].[ACO_MR_RAW_DATES] d ON d.aco_mr_id = f.aco_mr_id
            //select f.*, d.[date_stamp] FROM [dbo].[ACO_CS_RAW] f INNER JOIN [dbo].[ACO_CS_RAW_DATES] d ON d.aco_cs_id = f.aco_cs_id


      //      SELECT[LOB]
      //,[processing_date]
      //,[total_row_cnt]
      //,[total_distinct_prov_cnt]
      //,[total_distinct_mbr_cnt]
      //,[missing_mpin_cnt]  as miss_mpin_cnt_before
      //,[missing_indv_sys_id_cnt] as miss_indv_cnt_before
      //,[found_mpin_cnt] as miss_mpin_cnt_after
      //,[found_indv_sys_id_cnt] as miss_indv_cnt_after
      // FROM[IL_UCA].[dbo].[ACO_Processing_Summary]
      //  WHERE processing_date = '2021-07-10'


            aco_process();
        }

        static string strMessageGlobal = "";

        static void aco_process()
        {
            //SETUP PARAMETERS BEFORE USING IR_DataScrubber
            IR_DataScrubber.lstStrDegree = new List<string>();
            IR_DataScrubber.lstStrDegree.Add("MD");
            IR_DataScrubber.lstStrDegree.Add("DO");
            IR_DataScrubber.lstStrDegree.Add("DNP");
            IR_DataScrubber.lstStrDegree.Add("FNP");
            IR_DataScrubber.lstStrDegree.Add("NP");
            IR_DataScrubber.lstStrDegree.Add("PA");
            IR_DataScrubber.lstStrDegree.Add("RN");

            IR_DataScrubber.lstStrSuffix = new List<string>();
            IR_DataScrubber.lstStrSuffix.Add("SR");
            IR_DataScrubber.lstStrSuffix.Add("JR");
            //IR_DataScrubber.lstStrSuffix.Add("I"); //TOO CLOSE TO MIDDLENAME
            IR_DataScrubber.lstStrSuffix.Add("II");
            IR_DataScrubber.lstStrSuffix.Add("III");
            IR_DataScrubber.lstStrSuffix.Add("IV");
            //IR_DataScrubber.lstStrSuffix.Add("V"); //TOO CLOSE TO MIDDLENAME
            IR_DataScrubber.lstStrSuffix.Add("VI");

            IR_DataScrubber.strPrimarySpecs = "'001', '008', '011', '037', '038', '064', '077', '079', '230', '250', '251', '258', '276', '281', '296', '338', '339', '375', '389', '340'";


            StringBuilder sbBatchUpdatesContainer = new StringBuilder();


            //PLACE APP.CONFIG FILE DATA INTO VARIABLES START
            string strILUCA_ConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strACO_PIAR_ConnectionString = ConfigurationManager.AppSettings["ACO_PIAR_Database"];
            string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
            string strUHN_ConnectionString = ConfigurationManager.AppSettings["UHN_Database"];

            DateTime dtStartTime;
            DateTime dtEndTime;
            TimeSpan tsTimeSpan;
            string strTimeMessage = null;
            string strSQL = null;
            string strDateStamp = null;

            int intResultCnt = 0;

            string strBatchUpdateSQL = null;
            string strMissingSQL = null;
            string strSearchSQL = null;

            bool blRefreshCOMMTable = false;
            bool blCleanCOMM = false;
            bool blRefreshCSTable = false;
            bool blCleanCS = false;
            bool blRefreshMRTable = true;
            bool blCleanMR = true;
            bool blUpdateRaw = true;
            bool blUpdateProcessingSummary = true;

            Console.BufferWidth = 1000;
            Console.WindowWidth = 95;

            int intFilterMax = 500;

            strDateStamp = DateTime.Now.ToShortDateString();

            //PREP TABLE FOR CLEANING PARAMETERS PASSED INTO IR_DataScrubber.processQuerySet(DataTable dtQueries
            DataTable dtQueries = new DataTable();
            dtQueries.Clear();
            dtQueries.Columns.Add("MissingSearchDesc");
            dtQueries.Columns.Add("MissingSQL");
            dtQueries.Columns.Add("MissingConnectionString");
            dtQueries.Columns.Add("SearchSQL");
            dtQueries.Columns.Add("SearchConnectionString");
            dtQueries.Columns.Add("SearchLimit");
            dtQueries.Columns.Add("CachedTableName");
            dtQueries.Columns.Add("CachedInsertConnectionString");
            dtQueries.Columns.Add("BatchUpdateSQL");
            dtQueries.Columns.Add("BatchUpdateConnectionString");
            dtQueries.Columns.Add("Exclude");
            DataRow drQuery = null;

            int intSubstrLNLength = 7;
            int intSubstrFNLength = 4;

            //HELLO!
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("--------------------------------------------------------------------");
            Console.WriteLine("             WELCOME TO INNA RUDI'S ACO DATA LOADER     :)          ");
            Console.WriteLine("--------------------------------------------------------------------");

            //--delete from [dbo].[ACO_Member_Cache] WHERE data_source <> 'SAS_Historical'
            //--delete from [dbo].[ACO_Provider_Cache] WHERE data_source <> 'IL_UCA_History'
            //// DataTable dtt =  DBConnection.TDImportTest(strUGAP_ConnectionString);
            //IR_DataScrubber.strCurrentLOB = "CS";
            //IR_DataScrubber.strCurrentMainTable = "ACO_CS";
            //Console.WriteLine("Step 2.9 ACO_CS.mpin main cleanup...");
            //IR_DataScrubber.processQuerySet(dtQueries, strFilterTagGeneric: "<filter_{$x}_loop>");
            //dtQueries.Clear();


            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_COMM START////////////////////////////////////////////////////////////////////////////

            if (blRefreshCOMMTable)
                {
                    //CLEAR OUT IL_UCA.ACO_COMM TABLE
                    Console.WriteLine("Step 1.1 Truncating table IL_UCA.ACO_COMM... ");
                    DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "TRUNCATE TABLE [dbo].[ACO_COMM]"); //CLEAR OUT LIVE TABLES
                                                                                                             //GET DATA FROM PAIR.ACO_Exec_FullPositiveRegistry_OHP
                    strSQL = "SELECT Distinct CASE WHEN ISNULL(INDV_ID,0) = 0 THEN NULL ELSE convert(int,INDV_ID) END AS indv_sys_id, CASE WHEN ISNULL(MEMFNAME,'') = '' THEN NULL ELSE MEMFNAME END AS MEMFNAME, CASE WHEN ISNULL(MEMLNAME,'') = '' THEN NULL ELSE MEMLNAME END AS MEMLNAME, CASE WHEN ISNULL(BTH_DT,'') = '' THEN NULL ELSE convert(date,BTH_DT) END AS BTH_DT, CASE WHEN ISNULL(MBR_ZIP_CD,'') = '' THEN NULL ELSE MBR_ZIP_CD END AS MBR_ZIP_CD, CASE WHEN ISNULL( NPI,'') = '' THEN NULL ELSE NPI END AS NPI, CASE WHEN ISNULL(mpin,0) = 0 THEN NULL ELSE convert(int,mpin) END AS MPIN, CASE WHEN ISNULL(MKT_NBR,0) = 0 THEN NULL ELSE MKT_NBR END AS MKT_NBR, CASE WHEN ISNULL(TIN,0) = 0 THEN NULL ELSE convert(int,TIN) END AS TIN, CASE WHEN ISNULL(ACO_Name,'') = '' THEN NULL ELSE ACO_Name END AS ACO_Name, 'COMM' as Lob, CASE WHEN ISNULL(PROV_NM,'') = '' THEN NULL ELSE PROV_NM END AS PROV_NM FROM dbo.ACO_Exec_FullPositiveRegistry_UHC WHERE INDV_ID<>-1 AND  TIN NOT LIKE '%-%' UNION ALL SELECT Distinct CASE WHEN ISNULL(INDV_ID,0) = 0 THEN NULL ELSE convert(int,INDV_ID) END AS indv_sys_id, CASE WHEN ISNULL(MEMFNAME,'') = '' THEN NULL ELSE MEMFNAME END AS MEMFNAME, CASE WHEN ISNULL(MEMLNAME,'') = '' THEN NULL ELSE MEMLNAME END AS MEMLNAME, CASE WHEN ISNULL(bth_dt1,'') = '' THEN NULL ELSE convert(date,bth_dt1) END AS BTH_DT, CASE WHEN ISNULL(MBR_ZIP_CD,'') = '' THEN NULL ELSE MBR_ZIP_CD END AS MBR_ZIP_CD, CASE WHEN ISNULL( NPI,'') = '' THEN NULL ELSE NPI END AS NPI, CASE WHEN ISNULL(mpin,0) = 0 THEN NULL ELSE convert(int,mpin) END AS MPIN, CASE WHEN ISNULL(marketnbr,0) = 0 THEN NULL ELSE marketnbr END AS MKT_NBR, CASE WHEN ISNULL(TIN,0) = 0 THEN NULL ELSE convert(int,TIN) END AS TIN, CASE WHEN ISNULL(ACO_Name,'') = '' THEN NULL ELSE ACO_Name END AS ACO_Name, 'COMM' as Lob, CASE WHEN ISNULL(PROV_NM,'') = '' THEN NULL ELSE PROV_NM END AS PROV_NM FROM dbo.ACO_Exec_FullPositiveRegistry_OHP WHERE INDV_ID<>-1 AND  TIN NOT LIKE '%-%'";
                    Console.WriteLine("Step 1.2 Gathering data from PAIR.ACO_Exec_FullPositiveRegistry_OHP...");
                    intResultCnt = (int)DBConnection32.getMSSQLExecuteScalar(strACO_PIAR_ConnectionString, "SELECT COUNT(*) as total FROM (" + strSQL + ") tmp ");
                    Console.WriteLine("Step 1.3 Initializing data transfer from PAIR.ACO_Exec_FullPositiveRegistry_OHP to IL_UCA.ACO_COMM:");
                    strMessageGlobal = "{$rowCnt} out of " + String.Format("{0:n0}", intResultCnt) + " rows inserted...";
                    dtStartTime = DateTime.Now;
                    //TRANSFER PAIR.ACO_Exec_FullPositiveRegistry_OHP DATA TO ILUCA.ACO_COMM
                    SQLServerBulkImport(strACO_PIAR_ConnectionString, strILUCA_ConnectionString, strSQL, "ACO_COMM"); //BULK DATA LOAD
                    dtEndTime = DateTime.Now;
                    tsTimeSpan = dtEndTime.Subtract(dtStartTime);
                    strTimeMessage = (tsTimeSpan.Hours == 0 ? "" : tsTimeSpan.Hours + "hr:") + (tsTimeSpan.Minutes == 0 ? "" : tsTimeSpan.Minutes + "min:") + (tsTimeSpan.Seconds == 0 ? "" : tsTimeSpan.Seconds + "sec");
                    Console.Write("\r" + strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", intResultCnt)).Replace("...", ""));
                    Console.WriteLine("");
                    Console.WriteLine("Transfer completed in:  " + strTimeMessage.TrimEnd(':'));


                    if(blUpdateRaw)
                    {
                        //TRANSFER ANY UNIQUE NEW ILUCA.ACO_COMM TO IL_UCA.ACO_COMM_RAW BEFORE CLEANING
                        Console.WriteLine("Step 1.4 Transfer ILUCA.ACO_COMM to IL_UCA.ACO_COMM_RAW:");
                        strSQL = "INSERT INTO dbo.ACO_COMM_RAW (indv_sys_id ,MEMFNAME ,MEMLNAME ,BTH_DT ,MBR_ZIP_CD ,NPI ,MPIN ,MKT_NBR ,TIN ,ACO_Name ,Lob,PROV_NM) SELECT indv_sys_id ,MEMFNAME ,MEMLNAME ,BTH_DT ,MBR_ZIP_CD ,NPI ,MPIN ,MKT_NBR ,TIN ,ACO_Name ,Lob ,PROV_NM FROM dbo.ACO_COMM as a WHERE not exists ( select * from ACO_COMM_RAW r where ( ( a.indv_sys_id = r.indv_sys_id Or ( a.indv_sys_id Is Null And r.indv_sys_id Is Null ) ) AND ( a.MEMFNAME = r.MEMFNAME Or ( a.MEMFNAME Is Null And r.MEMFNAME Is Null ) ) AND ( a.MEMLNAME = r.MEMLNAME Or ( a.MEMLNAME Is Null And r.MEMLNAME Is Null ) ) AND ( a.BTH_DT = r.BTH_DT Or ( a.BTH_DT Is Null And r.BTH_DT Is Null ) ) AND ( a.MBR_ZIP_CD = r.MBR_ZIP_CD Or ( a.MBR_ZIP_CD Is Null And r.MBR_ZIP_CD Is Null ) ) AND ( a.NPI = r.NPI Or ( a.NPI Is Null And r.NPI Is Null ) ) AND ( a.MPIN = r.MPIN Or ( a.MPIN Is Null And r.MPIN Is Null ) ) AND ( a.MKT_NBR = r.MKT_NBR Or ( a.MKT_NBR Is Null And r.MKT_NBR Is Null ) ) AND ( a.TIN = r.TIN Or ( a.TIN Is Null And r.TIN Is Null ) ) AND ( a.ACO_Name = r.ACO_Name Or ( a.ACO_Name Is Null And r.ACO_Name Is Null ) ) AND ( a.Lob = r.Lob Or ( a.Lob Is Null And r.Lob Is Null ) ) ) )";
                        intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                        Console.WriteLine(intResultCnt + " rows inserted into dbo.ACO_COMM_RAW");
                        //TRANSFER ILUCA.ACO_COMM TO IL_UCA.ACO_COMM_RAW_DATES FOR ARHIVING
                        Console.WriteLine("Step 1.5 Transfer ILUCA.ACO_COMM to IL_UCA.ACO_COMM_RAW_DATES:");
                        strSQL = "INSERT INTO dbo.ACO_COMM_RAW_DATES ( aco_comm_id, date_stamp) SELECT aco_comm_id, '" + strDateStamp + "' as date_stamp FROM dbo.ACO_COMM_RAW as r WHERE exists ( select * from ACO_COMM a where ( ( a.indv_sys_id = r.indv_sys_id Or ( a.indv_sys_id Is Null And r.indv_sys_id Is Null ) ) AND ( a.MEMFNAME = r.MEMFNAME Or ( a.MEMFNAME Is Null And r.MEMFNAME Is Null ) ) AND ( a.MEMLNAME = r.MEMLNAME Or ( a.MEMLNAME Is Null And r.MEMLNAME Is Null ) ) AND ( a.BTH_DT = r.BTH_DT Or ( a.BTH_DT Is Null And r.BTH_DT Is Null ) ) AND ( a.MBR_ZIP_CD = r.MBR_ZIP_CD Or ( a.MBR_ZIP_CD Is Null And r.MBR_ZIP_CD Is Null ) ) AND ( a.NPI = r.NPI Or ( a.NPI Is Null And r.NPI Is Null ) ) AND ( a.MPIN = r.MPIN Or ( a.MPIN Is Null And r.MPIN Is Null ) ) AND ( a.MKT_NBR = r.MKT_NBR Or ( a.MKT_NBR Is Null And r.MKT_NBR Is Null ) ) AND ( a.TIN = r.TIN Or ( a.TIN Is Null And r.TIN Is Null ) ) AND ( a.ACO_Name = r.ACO_Name Or ( a.ACO_Name Is Null And r.ACO_Name Is Null ) ) AND ( a.Lob = r.Lob Or ( a.Lob Is Null And r.Lob Is Null ) ) ) )";
                        intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                        Console.WriteLine(intResultCnt + " rows inserted into IL_UCA.ACO_COMM_RAW_DATES");
                    }
                    
                    if(blUpdateProcessingSummary)
                    {
                        //GET INITIAL SUMMARY FROM ACO_COMM
                        Console.WriteLine("Step 1.6 Collecting initial 'COMM' counts for dbo.ACO_Processing_Summary...");
                        strSQL = "UPDATE dbo.ACO_Processing_Summary SET total_row_cnt = (SELECT COUNT(*) FROM dbo.ACO_COMM), total_distinct_prov_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT PROV_NM FROM ACO_COMM) as tmp), total_distinct_mbr_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_COMM) as tmp), missing_mpin_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT PROV_NM FROM ACO_COMM WHERE MPIN IS NULL) as tmp), missing_indv_sys_id_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_COMM WHERE indv_sys_id IS NULL) as tmp) WHERE LOB = 'COMM' AND processing_date = '" + strDateStamp + "'; IF @@ROWCOUNT=0 INSERT INTO dbo.ACO_Processing_Summary (LOB ,processing_date ,total_row_cnt ,total_distinct_prov_cnt ,total_distinct_mbr_cnt ,missing_mpin_cnt ,missing_indv_sys_id_cnt) SELECT 'COMM' as LOB, '" + strDateStamp + "' as processing_date, (SELECT COUNT(*) FROM dbo.ACO_COMM) as total_row_cnt , (SELECT COUNT(*) FROM (SELECT DISTINCT PROV_NM FROM ACO_COMM) as tmp) as total_distinct_prov_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_COMM) as tmp) as total_distinct_mbr_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT PROV_NM FROM ACO_COMM WHERE MPIN IS NULL) as tmp) as missing_mpin_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_COMM WHERE indv_sys_id IS NULL) as tmp) as missing_indv_sys_id_cnt";
                        intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                        Console.WriteLine("Initial dbo.ACO_Processing_Summary Updated");
                    }

                }

            if(blCleanCOMM)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////START COMM MPIN CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////START COMM MPIN CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                //........NOT NEEDED YET...
                //........NOT NEEDED YET...
                //........NOT NEEDED YET...
                ////////////////////////////////////////////////////////////////////////////////////////////START COMM indv_sys_id CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////START COMM indv_sys_id CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                //........NOT NEEDED YET...
                //........NOT NEEDED YET...
                //........NOT NEEDED YET...
            }

            if (blRefreshCOMMTable && blUpdateProcessingSummary)
                {
                    //GET FINAL SUMMARY FROM ACO_COMM
                    Console.WriteLine("Step 1.7 Gather Collecting final 'COMM' counts for dbo.ACO_Processing_Summary...");
                    strSQL = "UPDATE dbo.ACO_Processing_Summary SET found_mpin_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT PROV_NM FROM ACO_COMM WHERE MPIN IS NULL) as tmp), found_indv_sys_id_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_COMM WHERE indv_sys_id IS NULL) as tmp) WHERE LOB = 'COMM' AND processing_date = '" + strDateStamp + "';";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine("Final dbo.ACO_Processing_Summary Updated");
                }

                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////
                //////////////////////////////////IL_UCA.ACO_COMM END////////////////////////////////////////////////////////////////////////////





            //SECTION BREAK ON COMMAND PROMPT
            Console.WriteLine("--------------------------------------------------------------------");


            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS START////////////////////////////////////////////////////////////////////////////

          if (blRefreshCSTable)
            {
                //CLEAR OUT IL_UCA.ACO_CS TABLE
                Console.WriteLine("Step 2.1 Truncating table IL_UCA.ACO_CS... ");
                DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "TRUNCATE TABLE [dbo].[ACO_CS]"); //CLEAR OUT LIVE TABLES
                                                                                                        //GET DATA FROM PAIR.ACO_Exec_REGISTRY_CS
                strSQL = "SELECT Distinct CASE WHEN ISNULL(MEDICAID_NO,'') = '' THEN NULL ELSE MEDICAID_NO END AS MEDICAID_NO, CASE WHEN ISNULL(SUBSCRIBER_ID,'') = '' THEN NULL ELSE SUBSCRIBER_ID END AS SUBSCRIBER_ID, CASE WHEN ISNULL(MEMB_FIRST_NAME,'') = '' THEN NULL ELSE MEMB_FIRST_NAME END AS MEMFNAME, CASE WHEN ISNULL(MEMB_LAST_NAME,'') = '' THEN NULL ELSE MEMB_LAST_NAME END AS MEMLNAME, CASE WHEN ISNULL(DOB,'') = '' THEN NULL ELSE convert(date,DOB) END AS BTH_DT, CASE WHEN ISNULL(MEMB_ZIP,'') = '' THEN NULL ELSE left(MEMB_ZIP,5) END AS MBR_ZIP_CD, CASE WHEN ISNULL(CURR_PCP_MPIN,'') = '' THEN NULL ELSE convert(int,substring(CURR_PCP_MPIN, patindex('%^0%',CURR_PCP_MPIN), 10)) END AS MPIN, CASE WHEN ISNULL(CURR_PCP_ID,'') = '' THEN NULL ELSE CURR_PCP_ID END AS CURR_PCP_ID, CASE WHEN ISNULL(CURR_PCP_DEA_NBR,'') = '' THEN NULL ELSE CURR_PCP_DEA_NBR END AS CURR_PCP_DEA_NBR, CASE WHEN ISNULL(CURR_PCP_FIRST_NAME,'') = '' THEN NULL ELSE CURR_PCP_FIRST_NAME END AS CURR_PCP_FIRST_NAME, CASE WHEN ISNULL(CURR_PCP_LAST_NAME,'') = '' THEN NULL ELSE CURR_PCP_LAST_NAME END AS CURR_PCP_LAST_NAME, CASE WHEN ISNULL(IRS_TAX_ID,0) = 0 THEN NULL ELSE convert(int,IRS_TAX_ID) END as TIN, CASE WHEN ISNULL(PRACTICE,'') = '' THEN NULL ELSE UPPER(PRACTICE) END AS ACO_Name, 'CS' as Lob FROM ACO_Exec_REGISTRY_CS as a inner join (select distinct TIN, max(PRACTICE) as PRACTICE from ACO_Exec_CS_ACTIVE_ACO_TIN group by TIN) as t on TIN=IRS_TAX_ID WHERE [CURR_PCP_DEA_NBR] NOT LIKE '%-%'";
                Console.WriteLine("Step 2.2 Gathering data from PAIR.ACO_Exec_REGISTRY_CS... ");
                intResultCnt = (int)DBConnection32.getMSSQLExecuteScalar(strACO_PIAR_ConnectionString, "SELECT COUNT(*) as total FROM (" + strSQL + ") tmp ");
                Console.WriteLine("Step 2.3 Initializing data transfer from PAIR.ACO_Exec_REGISTRY_CS to IL_UCA.ACO_CS:");
                strMessageGlobal = "{$rowCnt} out of " + String.Format("{0:n0}", intResultCnt) + " rows inserted...";
                dtStartTime = DateTime.Now;
                //TRANSFER PAIR.ACO_Exec_REGISTRY_CS TO ILUCA.ACO_CS
                SQLServerBulkImport(strACO_PIAR_ConnectionString, strILUCA_ConnectionString, strSQL, "ACO_CS"); //BULK DATA LOAD
                dtEndTime = DateTime.Now;
                tsTimeSpan = dtEndTime.Subtract(dtStartTime);
                strTimeMessage = (tsTimeSpan.Hours == 0 ? "" : tsTimeSpan.Hours + "hr:") + (tsTimeSpan.Minutes == 0 ? "" : tsTimeSpan.Minutes + "min:") + (tsTimeSpan.Seconds == 0 ? "" : tsTimeSpan.Seconds + "sec");
                Console.Write("\r" + strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", intResultCnt)).Replace("...", ""));
                Console.WriteLine("");
                Console.WriteLine("Transfer completed in:  " + strTimeMessage.TrimEnd(':'));
                if (blUpdateRaw)
                {
                    //TRANSFER ANY UNIQUE NEW ILUCA.ACO_CS TO IL_UCA.ACO_CS_RAW BEFORE CLEANING
                    Console.WriteLine("Step 2.4 Transfer ILUCA.ACO_CS to IL_UCA.ACO_CS_RAW:");
                    strSQL = "INSERT INTO dbo.ACO_CS_RAW ( [MEDICAID_NO] ,[SUBSCRIBER_ID] ,[MEMFNAME] ,[MEMLNAME] ,[BTH_DT] ,[MBR_ZIP_CD] ,[MPIN] ,[CURR_PCP_ID] ,[CURR_PCP_DEA_NBR] ,[CURR_PCP_FIRST_NAME] ,[CURR_PCP_LAST_NAME] ,[TIN] ,[ACO_Name] ,[Lob]) SELECT [MEDICAID_NO] ,[SUBSCRIBER_ID] ,[MEMFNAME] ,[MEMLNAME] ,[BTH_DT] ,[MBR_ZIP_CD] ,[MPIN] ,[CURR_PCP_ID] ,[CURR_PCP_DEA_NBR] ,[CURR_PCP_FIRST_NAME] ,[CURR_PCP_LAST_NAME] ,[TIN] ,[ACO_Name] ,[Lob] FROM dbo.ACO_CS as a WHERE not exists ( select * from ACO_CS_RAW r where ( ( a.MEDICAID_NO = r.MEDICAID_NO OR ( a.MEDICAID_NO Is Null AND r.MEDICAID_NO Is Null ) ) AND (a.SUBSCRIBER_ID = r.SUBSCRIBER_ID OR ( a.SUBSCRIBER_ID Is Null AND r.SUBSCRIBER_ID Is Null ) ) AND ( a.MEMFNAME = r.MEMFNAME OR ( a.MEMFNAME Is Null AND r.MEMFNAME Is Null ) ) AND ( a.MEMLNAME = r.MEMLNAME OR ( a.MEMLNAME Is Null AND r.MEMLNAME Is Null ) ) AND ( a.BTH_DT = r.BTH_DT OR ( a.BTH_DT Is Null AND r.BTH_DT Is Null ) ) AND ( a.MBR_ZIP_CD = r.MBR_ZIP_CD OR ( a.MBR_ZIP_CD Is Null AND r.MBR_ZIP_CD Is Null ) ) AND ( a.MPIN = r.MPIN OR ( a.MPIN Is Null AND r.MPIN Is Null ) ) AND ( a.CURR_PCP_ID = r.CURR_PCP_ID OR ( a.CURR_PCP_ID Is Null AND r.CURR_PCP_ID Is Null ) ) AND ( a.CURR_PCP_DEA_NBR = r.CURR_PCP_DEA_NBR OR ( a.CURR_PCP_DEA_NBR Is Null AND r.CURR_PCP_DEA_NBR Is Null ) ) AND ( a.CURR_PCP_FIRST_NAME = r.CURR_PCP_FIRST_NAME OR ( a.CURR_PCP_FIRST_NAME Is Null AND r.CURR_PCP_FIRST_NAME Is Null ) ) AND ( a.CURR_PCP_LAST_NAME = r.CURR_PCP_LAST_NAME OR ( a.CURR_PCP_LAST_NAME Is Null AND r.CURR_PCP_LAST_NAME Is Null ) ) AND ( a.TIN = r.TIN OR ( a.TIN Is Null AND r.TIN Is Null ) ) AND ( a.ACO_Name = r.ACO_Name OR ( a.ACO_Name Is Null AND r.ACO_Name Is Null ) ) AND ( a.Lob = r.Lob OR ( a.Lob Is Null AND r.Lob Is Null ) ) ) )";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine(intResultCnt + " rows inserted into dbo.ACO_CS_RAW");
                    //TRANSFER ILUCA.ACO_CS TO IL_UCA.ACO_CS_RAW_DATES FOR ARHIVING
                    Console.WriteLine("Step 2.5 Transfer ILUCA.ACO_CS to IL_UCA.ACO_CS_RAW_DATES:");
                    strSQL = "INSERT INTO dbo.ACO_CS_RAW_DATES ( aco_cs_id, date_stamp) SELECT aco_cs_id, '" + strDateStamp + "' as date_stamp FROM dbo.ACO_CS_RAW as r WHERE exists ( select * from ACO_CS a where ( ( a.MEDICAID_NO = r.MEDICAID_NO OR ( a.MEDICAID_NO Is Null AND r.MEDICAID_NO Is Null ) ) AND (a.SUBSCRIBER_ID = r.SUBSCRIBER_ID OR ( a.SUBSCRIBER_ID Is Null AND r.SUBSCRIBER_ID Is Null ) ) AND ( a.MEMFNAME = r.MEMFNAME OR ( a.MEMFNAME Is Null AND r.MEMFNAME Is Null ) ) AND ( a.MEMLNAME = r.MEMLNAME OR ( a.MEMLNAME Is Null AND r.MEMLNAME Is Null ) ) AND ( a.BTH_DT = r.BTH_DT OR ( a.BTH_DT Is Null AND r.BTH_DT Is Null ) ) AND ( a.MBR_ZIP_CD = r.MBR_ZIP_CD OR ( a.MBR_ZIP_CD Is Null AND r.MBR_ZIP_CD Is Null ) ) AND ( a.MPIN = r.MPIN OR ( a.MPIN Is Null AND r.MPIN Is Null ) ) AND ( a.CURR_PCP_ID = r.CURR_PCP_ID OR ( a.CURR_PCP_ID Is Null AND r.CURR_PCP_ID Is Null ) ) AND ( a.CURR_PCP_DEA_NBR = r.CURR_PCP_DEA_NBR OR ( a.CURR_PCP_DEA_NBR Is Null AND r.CURR_PCP_DEA_NBR Is Null ) ) AND ( a.CURR_PCP_FIRST_NAME = r.CURR_PCP_FIRST_NAME OR ( a.CURR_PCP_FIRST_NAME Is Null AND r.CURR_PCP_FIRST_NAME Is Null ) ) AND ( a.CURR_PCP_LAST_NAME = r.CURR_PCP_LAST_NAME OR ( a.CURR_PCP_LAST_NAME Is Null AND r.CURR_PCP_LAST_NAME Is Null ) ) AND ( a.TIN = r.TIN OR ( a.TIN Is Null AND r.TIN Is Null ) ) AND ( a.ACO_Name = r.ACO_Name OR ( a.ACO_Name Is Null AND r.ACO_Name Is Null ) ) AND ( a.Lob = r.Lob OR ( a.Lob Is Null AND r.Lob Is Null ) ) ) )";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine(intResultCnt + " rows inserted into IL_UCA.ACO_CS_RAW_DATES");
                }

                if (blUpdateProcessingSummary)
                {
                    //GET INITIAL SUMMARY FROM ACO_CS
                    Console.WriteLine("Step 2.6 Collecting initial 'CS' counts for dbo.ACO_Processing_Summary");
                    strSQL = "UPDATE dbo.ACO_Processing_Summary SET total_row_cnt = (SELECT COUNT(*) FROM dbo.ACO_CS), total_distinct_prov_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT CURR_PCP_FIRST_NAME, CURR_PCP_LAST_NAME  FROM ACO_CS) as tmp), total_distinct_mbr_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_CS) as tmp), missing_mpin_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT CURR_PCP_FIRST_NAME, CURR_PCP_LAST_NAME  FROM ACO_CS WHERE MPIN IS NULL) as tmp), missing_indv_sys_id_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_CS WHERE indv_sys_id IS NULL) as tmp) WHERE LOB = 'CS' AND processing_date = '" + strDateStamp + "'; IF @@ROWCOUNT=0 INSERT INTO dbo.ACO_Processing_Summary (LOB ,processing_date ,total_row_cnt ,total_distinct_prov_cnt ,total_distinct_mbr_cnt ,missing_mpin_cnt ,missing_indv_sys_id_cnt) SELECT 'CS' as LOB, '" + strDateStamp + "' as processing_date, (SELECT COUNT(*) FROM dbo.ACO_CS) as total_row_cnt , (SELECT COUNT(*) FROM (SELECT DISTINCT CURR_PCP_FIRST_NAME, CURR_PCP_LAST_NAME  FROM ACO_CS) as tmp) as total_distinct_prov_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_CS) as tmp) as total_distinct_mbr_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT CURR_PCP_FIRST_NAME, CURR_PCP_LAST_NAME  FROM ACO_CS WHERE MPIN IS NULL) as tmp) as missing_mpin_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_CS WHERE indv_sys_id IS NULL) as tmp) as missing_indv_sys_id_cnt";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine("Initial dbo.ACO_Processing_Summary Updated");
                }

            }
           

            if (blCleanCS)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////START CS MPIN CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////START CS MPIN CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////

                //STEP 1.1 HISTORICAL IL_UCA.ACO_CS VS IL_UCA.ACO_Provider_Cache BY CURR_PCP_FIRST_NAME,CURR_PCP_LAST_NAME
                Console.WriteLine("Step 2.7 mpin clean: Historical CURR_PCP_FIRST_NAME,CURR_PCP_LAST_NAME");
                strSQL = "update a set a.mpin = b.mpin from dbo.ACO_CS as a inner join ACO_Provider_Cache as b on ISNULL(a.CURR_PCP_FIRST_NAME,'')=ISNULL(b.PROV_FST_NM,'') and ISNULL(a.CURR_PCP_LAST_NAME,'')= ISNULL(b.PROV_LST_NM, '') WHERE a.mpin is NULL AND b.is_matched = 1";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //STEP 1.2 CURRENT IL_UCA.ACO_CS VS IL_UCA.ACO_CS
                Console.WriteLine("Step 2.8 mpin clean: ACO_CS vs ACO_CS");
                strSQL = "update a set a.MPIN = b.MPIN from dbo.ACO_CS as a inner join ( select aa.mpin, aa.CURR_PCP_FIRST_NAME, aa.CURR_PCP_LAST_NAME from ( select DISTINCT mpin,[CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME] FROM dbo.ACO_CS as aa where mpin is not null ) as aa inner join (select DISTINCT[CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME] FROM dbo.ACO_CS as aa where mpin is null ) as bb on aa.CURR_PCP_FIRST_NAME = bb.CURR_PCP_FIRST_NAME and aa.CURR_PCP_LAST_NAME = bb.CURR_PCP_LAST_NAME ) as b on a.CURR_PCP_FIRST_NAME = b.CURR_PCP_FIRST_NAME AND a.CURR_PCP_LAST_NAME = b.CURR_PCP_LAST_NAME where a.mpin is null";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //PREP CS FOR IR_DataScrubber.processQuerySet(DataTable dtQueries
                dtQueries.Clear();
                IR_DataScrubber.strCurrentLOB = "CS";
                IR_DataScrubber.strCurrentMainTable = "ACO_CS";


                //UHN DEA CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "SELECT * FROM (select DISTINCT CAST(LEFT(SUBSTRING(CURR_PCP_DEA_NBR, PATINDEX('%[0-9.-]%', CURR_PCP_DEA_NBR), 8000), PATINDEX('%[^0-9.-]%', SUBSTRING(CURR_PCP_DEA_NBR, PATINDEX('%[0-9.-]%', CURR_PCP_DEA_NBR), 8000) + 'X') - 1) as int) as CURR_PCP_DEA_NBR_INT, CURR_PCP_DEA_NBR ,CURR_PCP_ID,[CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME],SUBSTRING([CURR_PCP_LAST_NAME],1,5) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_CS WHERE MPIN IS NULL AND CURR_PCP_DEA_NBR IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL) tmp where CURR_PCP_DEA_NBR_INT <> 0";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UHN_GENERIC_PROVIDER_SEARCH.Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_create_upt_columns}", "[FirstName] [varchar](50) NULL, [LastName] [varchar](150) NULL, [LastName_Sub] [varchar](15) NULL, [DEANbr] [varchar](25)").Replace("{$tmp_columns}", "FirstName,LastName,LastName_Sub, DEANbr").Replace("{$tmp_insert_values}", "'{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%','%{$CURR_PCP_DEA_NBR_INT}%'").Replace("{$data_source_case}", "CASE WHEN (d.DEANbr LIKE  mp.DEANbr  AND p.FirstName LIKE mp.FirstName AND p.LastName LIKE mp.LastName ) THEN 'UHN_%DEANbr%FN%Ln%' ELSE CASE WHEN (d.DEANbr LIKE  mp.DEANbr  AND p.LastName LIKE  mp.LastName ) THEN 'UHN_%DEANbr%Ln%' ELSE 'UHN_%DEANbr%LnSub%' END END").Replace("{$notes}", "Found in NDB.PROVIDER as p left join NDB.DEA_LICENSE by DEANbr").Replace("{$extra_cols}", ",NULL as taxid, d.DEANbr, p.MiddleName, NULL as SBSCR_MEDCD_RCIP_NBR, NULL as CURR_PCP_ID, p.ProvDegree, p.PrimSpec").Replace("{$joins}", "left join[dbo].[DEA_LICENSE] as d on p.mpin=d.mpin inner join #MissingProvidersTmp as mp on d.DEANbr LIKE mp.DEANbr AND  p.LastName LIKE mp.LastName_Sub").Replace("{$PrSpecs}", "");
                //COLLECT BATCH UPDATE FOR DEA_NBR,PROV_FST_NM,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.DEA_NBR = a.DEA_NBR, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.DEA_NBR, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "CAST(dbo.fnRegExeReplaceCSG(a.DEA_NBR, '[^0-9]') as int) = b.DEA_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + "),DEA_NBR = dbo.fnRegExeReplaceCSG(DEA_NBR, '[^0-9]');"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "DEAFNLN").Replace("{$tmp_index_columns}", "DEA_NBR,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "CAST(dbo.fnRegExeReplaceCSG(a.CURR_PCP_DEA_NBR, '[^0-9]') as int) = CAST(b.DEA_NBR as int) AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0 ")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR DEA_NBR,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.DEA_NBR = a.DEA_NBR, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.DEA_NBR, INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "CAST(dbo.fnRegExeReplaceCSG(a.DEA_NBR, '[^0-9]') as int) = b.DEA_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0")
                    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + "),DEA_NBR = dbo.fnRegExeReplaceCSG(DEA_NBR, '[^0-9]');"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "DEALN").Replace("{$tmp_index_columns}", "DEA_NBR,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "CAST(dbo.fnRegExeReplaceCSG(a.CURR_PCP_DEA_NBR, '[^0-9]') as int) = CAST(b.DEA_NBR as int) AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR DEA_NBR,PROV_LST_NM SUB
                //sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.DEA_NBR = a.DEA_NBR, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.DEA_NBR, INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "CAST(dbo.fnRegExeReplaceCSG(a.DEA_NBR, '[^0-9]') as int) = b.DEA_NBR AND b.PROV_LST_NM LIKE SUBSTRING(a.PROV_LST_NM,1,5) + '%'")
                //    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(PROV_LST_NM,1,5) + '%',DEA_NBR = dbo.fnRegExeReplaceCSG(DEA_NBR, '[^0-9]');"
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "DEALNSUB").Replace("{$tmp_index_columns}", "DEA_NBR,PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "CAST(dbo.fnRegExeReplaceCSG(a.CURR_PCP_DEA_NBR, '[^0-9]') as int) = CAST(b.DEA_NBR as int) AND a.CURR_PCP_LAST_NAME LIKE b.PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",DEA_NBR, PROV_LST_NM,PROV_LST_NM_CLN,PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[DEA_NBR] [varchar](50) NULL, [PROV_LST_NM] [varchar](150) NULL , [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[DEA_NBR] [varchar](50) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",CAST(dbo.fnRegExeReplaceCSG(CURR_PCP_DEA_NBR, '[^0-9]') as int) as DEA_NBR, CURR_PCP_LAST_NAME as PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '^a-zA-Z'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN,SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '^a-zA-Z'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UHN_%DEANbr%FN%Ln%', 'UHN_%DEANbr%Ln%', 'UHN_%DEANbr%LnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "CURR_PCP_DEA_NBR IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by DEANBR in UNH";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUHN_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);



                //UHN TIN CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT [CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME], SUBSTRING([CURR_PCP_LAST_NAME],1,3) AS CURR_PCP_LAST_NAME_SUB, TIN FROM dbo.ACO_CS WHERE MPIN IS NULL AND TIN IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UHN_GENERIC_PROVIDER_SEARCH.Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_create_upt_columns}", "[FirstName] [varchar](50) NULL, [LastName] [varchar](150) NULL, [LastName_Sub] [varchar](15) NULL, [TIN] INT").Replace("{$tmp_columns}", "FirstName,LastName,LastName_Sub,TIN").Replace("{$tmp_insert_values}", "'{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%',{$TIN}").Replace("{$data_source_case}", "CASE WHEN (p.FirstName LIKE mp.FirstName AND p.LastName LIKE mp.LastName ) THEN 'UHN_TaxIdFN%Ln%' ELSE CASE WHEN (p.LastName LIKE  mp.LastName) THEN 'UHN_TaxIdLn%' ELSE 'UHN_TaxIdLnSub%' END END").Replace("{$notes}", "Found in NDB.PROVIDER as p inner join NDB.PROV_TIN_PAY_AFFIL by taxid").Replace("{$extra_cols}", ", a.taxid, NULL as DEANbr, p.MiddleName, NULL as SBSCR_MEDCD_RCIP_NBR, NULL as CURR_PCP_ID, p.ProvDegree, p.PrimSpec").Replace("{$joins}", "inner join PROV_TIN_PAY_AFFIL as a on a.MPIN = p.MPIN inner join #MissingProvidersTmp as mp on a.taxid = mp.TIN AND  p.LastName LIKE mp.LastName_Sub").Replace("{$PrSpecs}", "");
                //COLLECT BATCH UPDATE FOR TIN,PROV_FST_NM,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TNFNLN").Replace("{$tmp_index_columns}", "TIN,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0 ")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR TIN,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, NULL as PROV_FST_NM ").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TNLN").Replace("{$tmp_index_columns}", "TIN,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR TIN,PROV_LST_NM_SUB
                //sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, NULL as PROV_FST_NM ").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND  b.PROV_LST_NM LIKE SUBSTRING(a.PROV_LST_NM,1,5) + '%'")
                //    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(PROV_LST_NM,1,5) + '%';"
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TNLNSUB").Replace("{$tmp_index_columns}", "TIN,PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND a.CURR_PCP_LAST_NAME LIKE b.PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",TIN, PROV_LST_NM,PROV_LST_NM_CLN,PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[TIN] int NULL, [PROV_LST_NM] [varchar](150) NULL , [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[TIN] int NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",TIN,CURR_PCP_LAST_NAME AS PROV_LST_NM,SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '^a-zA-Z'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN,SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '^a-zA-Z'),1," + intSubstrFNLength + ") as PROV_FST_NM").Replace("{$data_source}", "'UHN_TaxIdFN%Ln%','UHN_TaxIdLn%','UHN_TaxIdLnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "TIN IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by TIN in UNH";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUHN_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);



                //UHN SPEC CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT [CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME], SUBSTRING([CURR_PCP_FIRST_NAME],1,1) AS CURR_PCP_FIRST_NAME_SUB  FROM dbo.ACO_CS WHERE MPIN IS NULL AND CURR_PCP_LAST_NAME IS NOT NULL";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UHN_GENERIC_PROVIDER_SEARCH.Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_create_upt_columns}", "[FirstName] [varchar](50) NULL, [LastName] [varchar](150) NULL, [LastNameWild] [varchar](150) NULL, [FirstNameSub] [varchar](20) NULL").Replace("{$tmp_columns}", "FirstName,LastName,LastNameWild,FirstNameSub").Replace("{$tmp_insert_values}", "'{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_FIRST_NAME_SUB}%'").Replace("{$data_source_case}", "CASE WHEN (p.FirstName LIKE mp.FirstName AND p.LastName = mp.LastName ) THEN 'UHN_FN%LnBySpec' ELSE CASE WHEN (p.FirstName LIKE mp.FirstName AND p.LastName LIKE mp.LastNameWild ) THEN 'UHN_FN%Ln%BySpec' ELSE 'UHN_Ln%BySpec' END END").Replace("{$notes}", "Found in NDB.PROVIDER by PrimSpec AND PROV_LST_NM%").Replace("{$extra_cols}", ", NULL as taxid, NULL as DEANbr, p.MiddleName, NULL as SBSCR_MEDCD_RCIP_NBR, NULL as CURR_PCP_ID, p.ProvDegree, p.PrimSpec").Replace("{$joins}", "inner join #MissingProvidersTmp as mp on p.LastName LIKE mp.LastNameWild AND p.FirstName LIKE mp.FirstNameSub").Replace("{$PrSpecs}", "WHERE primSpec in (" + IR_DataScrubber.strPrimarySpecs + ") ");
                //COLLECT BATCH UPDATE FOR SPEC,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SPECFNLN").Replace("{$tmp_index_columns}", "PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_LAST_NAME=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR SPEC,PROV_FST_NM%,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0  AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SPECFNLNW").Replace("{$tmp_index_columns}", "PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR SPEC,PROV_FST_NM%,PROV_LST_NM%
                //sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.PROV_LST_NM, NULL as PROV_FST_NM").Replace("{$data_source}", "UHN_Ln%BySpec").Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(dbo.fnRegExeReplaceCSG(a.PROV_LST_NM, '[^a-zA-Z]'), b.PROV_LST_NM_CLN), 0) > 0")
                //    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SPECLNW").Replace("{$tmp_index_columns}", "PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",PROV_LST_NM,PROV_LST_NM_CLN,PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",CURR_PCP_LAST_NAME AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '[^a-zA-Z]'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UHN_FN%LnBySpec','UHN_FN%Ln%BySpec','UHN_Ln%BySpec'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "CURR_PCP_LAST_NAME IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by Specs in UNH";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUHN_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP CURR_PCP_ID CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT CURR_PCP_ID,[CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME], SUBSTRING([CURR_PCP_LAST_NAME],1,3) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_CS WHERE MPIN IS NULL AND CURR_PCP_ID IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL AND CURR_PCP_FIRST_NAME IS NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_PROVIDER_SEARCH.Replace("{$data_source_case}", "CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM = pp.PROV_LST_NM) THEN 'UGAP_ProvIdFN%Ln' ELSE CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD ) THEN 'UGAP_ProvIdFN%Ln%' ELSE 'UGAP_ProvIdLnSub%' END END").Replace("{$vtt_create_columns}", "PROV_FST_NM_WILD VARCHAR(50), PROV_LST_NM VARCHAR(150), PROV_LST_NM_WILD VARCHAR(150),PROV_LST_NM_SUB VARCHAR(20), PROV_ID VARCHAR(11)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO PotentialProvidersTmp(PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, PROV_ID) VALUES ('{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%','{$CURR_PCP_ID}'); </filter_1_loop>").Replace("{$vtt_columns}", "PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, PROV_ID").Replace("{$notes}", "Found in UHCDM001.PROVIDER by CURR_PCP_ID").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$extra_cols}", ",CAST(p.TIN as INT) as taxid, p.DEA_NBR as DEANbr, CAST(null as varchar(20)) as MiddleName, CAST(null as varchar(20)) as SBSCR_MEDCD_RCIP_NBR, p.PROV_ID as CURR_PCP_ID, CAST(null as varchar(20)) as ProvDegree,CAST(null as varchar(20)) as PrimSpec").Replace("{$final_filter}", "inner join PotentialProvidersTmp as pp on p.PROV_LST_NM LIKE pp.PROV_LST_NM_SUB AND p.PROV_ID=pp.PROV_ID").Replace("{$main_query}", IR_DataScrubber.strSQL_UGAP_PROVIDER_TABLE_BY_PROVIDER);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.CURR_PCP_ID = a.CURR_PCP_ID, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.CURR_PCP_ID, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "CURR_PCP_IDFNWLN").Replace("{$tmp_index_columns}", "CURR_PCP_ID,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND a.CURR_PCP_LAST_NAME=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.CURR_PCP_ID = a.CURR_PCP_ID, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.CURR_PCP_ID, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "CURR_PCP_IDFNWLNW").Replace("{$tmp_index_columns}", "CURR_PCP_ID,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.CURR_PCP_ID = a.CURR_PCP_ID, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.CURR_PCP_ID, INSERTED.PROV_LST_NM, NULL PROV_FST_NM").Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0")
                + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "CURR_PCP_IDLNW").Replace("{$tmp_index_columns}", "CURR_PCP_ID,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",CURR_PCP_ID,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[CURR_PCP_ID] [varchar](20) NULL,[PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[CURR_PCP_ID] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",CURR_PCP_ID AS CURR_PCP_ID,CURR_PCP_LAST_NAME AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '[^a-zA-Z]'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UGAP_ProvIdFN%Ln','UGAP_ProvIdFN%Ln%', 'UGAP_ProvIdLnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "CURR_PCP_ID IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL AND CURR_PCP_FIRST_NAME IS NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                drQuery = dtQueries.NewRow();
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by CURR_PCP_ID in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP TIN CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT TIN,[CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME], SUBSTRING([CURR_PCP_LAST_NAME],1,3) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_CS WHERE MPIN IS NULL AND TIN IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_PROVIDER_SEARCH.Replace("{$data_source_case}", "CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM = pp.PROV_LST_NM) THEN 'UGAP_TaxIdFn%Ln' ELSE CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD ) THEN 'UGAP_TaxIdFn%Ln%' ELSE 'UGAP_TaxIdLnSub%' END END").Replace("{$vtt_create_columns}", "PROV_FST_NM_WILD VARCHAR(50), PROV_LST_NM VARCHAR(150), PROV_LST_NM_WILD VARCHAR(150),PROV_LST_NM_SUB VARCHAR(20), TIN CHAR(9)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO PotentialProvidersTmp(PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, TIN) VALUES ('{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%','{$TIN}'); </filter_1_loop>").Replace("{$vtt_columns}", "PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB,TIN").Replace("{$notes}", "Found in UHCDM001.PROVIDER by TIN").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$extra_cols}", ",CAST(p.TIN as INT) as taxid, p.DEA_NBR as DEANbr, CAST(null as varchar(20)) as MiddleName, CAST(null as varchar(20)) as SBSCR_MEDCD_RCIP_NBR, p.PROV_ID as CURR_PCP_ID, CAST(null as varchar(20)) as ProvDegree,CAST(null as varchar(20)) as PrimSpec").Replace("{$final_filter}", "inner join PotentialProvidersTmp as pp on p.PROV_LST_NM LIKE pp.PROV_LST_NM_SUB AND p.TIN=pp.TIN").Replace("{$main_query}", IR_DataScrubber.strSQL_UGAP_PROVIDER_TABLE_BY_PROVIDER);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TINFNWLN").Replace("{$tmp_index_columns}", "TIN,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND a.CURR_PCP_LAST_NAME=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0  AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TINFNWLNW").Replace("{$tmp_index_columns}", "TIN,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TINLNW").Replace("{$tmp_index_columns}", "TIN,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 ")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",TIN,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[TIN] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[TIN] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",TIN,CURR_PCP_LAST_NAME AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '[^a-zA-Z]'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UGAP_TaxIdFn%Ln','UGAP_TaxIdFn%Ln%','UGAP_TaxIdLnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "TIN IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by TIN in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP DEA_NBR CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT CAST(LEFT(SUBSTRING(CURR_PCP_DEA_NBR, PATINDEX('%[0-9.-]%', CURR_PCP_DEA_NBR), 8000), PATINDEX('%[^0-9.-]%', SUBSTRING(CURR_PCP_DEA_NBR, PATINDEX('%[0-9.-]%', CURR_PCP_DEA_NBR), 8000) + 'X') - 1) as int) as CURR_PCP_DEA_NBR_INT, CURR_PCP_DEA_NBR ,[CURR_PCP_FIRST_NAME],[CURR_PCP_LAST_NAME], SUBSTRING([CURR_PCP_LAST_NAME],1,3) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_CS WHERE MPIN IS NULL AND CURR_PCP_DEA_NBR IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_PROVIDER_SEARCH.Replace("{$data_source_case}", "CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM = pp.PROV_LST_NM AND p.DEA_NBR= pp.DEA_NBR) THEN 'UGAP_DeaNbrFn%Ln' ELSE CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD  AND p.DEA_NBR= pp.DEA_NBR) THEN 'UGAP_DeaNbrFn%Ln%' ELSE 'UGAP_%DeaNbr%LnSub%' END END").Replace("{$vtt_create_columns}", "PROV_FST_NM_WILD VARCHAR(50), PROV_LST_NM VARCHAR(150), PROV_LST_NM_WILD VARCHAR(150),PROV_LST_NM_SUB VARCHAR(20), DEA_NBR VARCHAR(20), DEA_NBR_WILD VARCHAR(20)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO PotentialProvidersTmp(PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, DEA_NBR, DEA_NBR_WILD) VALUES ('{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%','{$CURR_PCP_DEA_NBR}','%{$CURR_PCP_DEA_NBR_INT}%'); </filter_1_loop>").Replace("{$vtt_columns}", "PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB,DEA_NBR").Replace("{$notes}", "Found in UHCDM001.PROVIDER by DEA_NBR").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$extra_cols}", ",CAST(p.TIN as INT) as taxid, p.DEA_NBR as DEANbr, CAST(null as varchar(20)) as MiddleName, CAST(null as varchar(20)) as SBSCR_MEDCD_RCIP_NBR, p.PROV_ID as CURR_PCP_ID, CAST(null as varchar(20)) as ProvDegree,CAST(null as varchar(20)) as PrimSpec").Replace("{$final_filter}", "inner join PotentialProvidersTmp as pp on p.PROV_LST_NM LIKE pp.PROV_LST_NM_SUB AND p.DEA_NBR LIKE pp.DEA_NBR").Replace("{$main_query}", IR_DataScrubber.strSQL_UGAP_PROVIDER_TABLE_BY_PROVIDER);
                //COLLECT BATCH UPDATE FOR DEA_NBR,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.DEA_NBR = a.DEA_NBR, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.DEA_NBR, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.DEA_NBR = b.DEA_NBR AND a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "DEA_NBRFNWLN").Replace("{$tmp_index_columns}", "DEA_NBR,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_DEA_NBR = b.DEA_NBR AND a.CURR_PCP_LAST_NAME=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR DEA_NBR,PROV_FST_NM%,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.DEA_NBR = a.DEA_NBR, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.DEA_NBR, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.DEA_NBR = b.DEA_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0  AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "), PROV_LST_NM = dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]');"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "DEA_NBRFNWLNW").Replace("{$tmp_index_columns}", "DEA_NBR,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_DEA_NBR = b.DEA_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR DEA_NBR,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.DEA_NBR = a.DEA_NBR, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.DEA_NBR, INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "a.DEA_NBR = b.DEA_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "DEA_NBRLNW").Replace("{$tmp_index_columns}", "DEA_NBR,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.CURR_PCP_DEA_NBR = b.DEA_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 ")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",DEA_NBR,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[DEA_NBR] [varchar](20) NULL,[PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[DEA_NBR] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",CURR_PCP_DEA_NBR AS DEA_NBR,CURR_PCP_LAST_NAME AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '[^a-zA-Z]'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UGAP_DeaNbrFn%Ln','UGAP_DeaNbrFn%Ln%','UGAP_%DeaNbr%LnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "CURR_PCP_DEA_NBR IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by DEA_NBR in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP SBSCR_MEDCD_RCIP_NBR CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT MEDICAID_NO as SBSCR_MEDCD_RCIP_NBR,[CURR_PCP_LAST_NAME],[CURR_PCP_FIRST_NAME], TIN FROM dbo.ACO_CS WHERE MPIN IS NULL AND MEDICAID_NO IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_PROVIDER_SEARCH.Replace("{$data_source_case}", "CASE WHEN (m.SBSCR_MEDCD_RCIP_NBR = pp.SBSCR_MEDCD_RCIP_NBR AND p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD) THEN 'UGAP_MedCdNbrFn%Ln%' ELSE CASE WHEN (p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD AND m.SBSCR_MEDCD_RCIP_NBR = pp.SBSCR_MEDCD_RCIP_NBR  AND p.TIN = pp.TIN ) THEN 'UGAP_MedCdNbr%TINLn%' ELSE 'UGAP_MedCdNbrLn%' END END").Replace("{$vtt_create_columns}", "PROV_FST_NM_WILD VARCHAR(50), PROV_LST_NM_WILD VARCHAR(150), SBSCR_MEDCD_RCIP_NBR VARCHAR(11), TIN CHAR(9)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO PotentialProvidersTmp(PROV_FST_NM_WILD,PROV_LST_NM_WILD,SBSCR_MEDCD_RCIP_NBR,TIN) VALUES ('{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}%','{$SBSCR_MEDCD_RCIP_NBR}','{$TIN}'); </filter_1_loop>").Replace("{$vtt_columns}", "PROV_FST_NM_WILD,PROV_LST_NM_WILD,SBSCR_MEDCD_RCIP_NBR,TIN").Replace("{$notes}", "Found in UHCDM001.HP_MEMBER by SBSCR_MEDCD_RCIP_NBR").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$extra_cols}", ",CAST(p.TIN as INT) as taxid, p.DEA_NBR as DEANbr, CAST(null as varchar(20)) as MiddleName, m.SBSCR_MEDCD_RCIP_NBR as SBSCR_MEDCD_RCIP_NBR, p.PROV_ID as CURR_PCP_ID, CAST(null as varchar(20)) as ProvDegree, CAST(null as varchar(20)) as PrimSpec").Replace("{$final_filter}", "inner join PotentialProvidersTmp as pp on p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD AND m.SBSCR_MEDCD_RCIP_NBR=pp.SBSCR_MEDCD_RCIP_NBR").Replace("{$main_query}", IR_DataScrubber.strSQL_UGAP_PROVIDER_TABLE_BY_MEMBER);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.SBSCR_MEDCD_RCIP_NBR, NULL AS TIN,INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.SBSCR_MEDCD_RCIP_NBR= b.SBSCR_MEDCD_RCIP_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0  AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "), PROV_LST_NM = dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]');"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SBSCR_MEDCD_RCIP_NBRFNWLN").Replace("{$tmp_index_columns}", "SBSCR_MEDCD_RCIP_NBR,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.MEDICAID_NO = b.SBSCR_MEDCD_RCIP_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_FIRST_NAME, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR, a.PROV_LST_NM = a.PROV_LST_NM, a.TIN = a.TIN").Replace("{$insert_output_columns}", ",INSERTED.SBSCR_MEDCD_RCIP_NBR, INSERTED.TIN,INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "a.SBSCR_MEDCD_RCIP_NBR= b.SBSCR_MEDCD_RCIP_NBR AND a.TIN= b.TIN AND ISNULL(CHARINDEX( b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SBSCR_MEDCD_RCIP_NBRTINLNW").Replace("{$tmp_index_columns}", "SBSCR_MEDCD_RCIP_NBR,TIN,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.MEDICAID_NO = b.SBSCR_MEDCD_RCIP_NBR AND a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 ")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.SBSCR_MEDCD_RCIP_NBR, NULL AS TIN,INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "a.SBSCR_MEDCD_RCIP_NBR= b.SBSCR_MEDCD_RCIP_NBR AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SBSCR_MEDCD_RCIP_NBRLNW").Replace("{$tmp_index_columns}", "SBSCR_MEDCD_RCIP_NBR,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.MEDICAID_NO = b.SBSCR_MEDCD_RCIP_NBR AND a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0 ")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",SBSCR_MEDCD_RCIP_NBR,TIN,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[SBSCR_MEDCD_RCIP_NBR] [varchar](20) NULL,[TIN] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[SBSCR_MEDCD_RCIP_NBR] [varchar](20) NULL,[TIN] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",MEDICAID_NO AS SBSCR_MEDCD_RCIP_NBR,TIN,CURR_PCP_LAST_NAME AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_LAST_NAME, '[^a-zA-Z]'),1," + intSubstrLNLength + ") as CURR_PCP_FIRST_NAME, SUBSTRING(dbo.fnRegExeReplaceCSG(CURR_PCP_FIRST_NAME, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UGAP_MedCdNbrFn%Ln%','UGAP_MedCdNbr%TINLn%','UGAP_MedCdNbrLn%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "MEDICAID_NO IS NOT NULL AND CURR_PCP_LAST_NAME IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by SBSCR_MEDCD_RCIP_NBR in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);

                //Retrieving ACO_CS.MPIN CACHE 
                //Retrieving ACO_CS.MPIN CACHE 
                //Retrieving ACO_CS.MPIN CACHE 
                Console.WriteLine("Step 2.9 ACO_CS.mpin main cleanup...");
                IR_DataScrubber.processQuerySet(dtQueries, strFilterTagGeneric: "<filter_{$x}_loop>");
                dtQueries.Clear();

                ////////////////////////////////////////////////////////////////////////////////////////////START CS indv_sys_id CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////START CS indv_sys_id CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////

                //STEP 1.1 HISTORICAL IL_UCA.ACO_CS VS IL_UCA.ACO_Member_Cache BY medicaid_no and SUBSCRIBER_ID
                Console.WriteLine("Step 2.10 indv_sys_id clean: Historical SBSCR_MEDCD_RCIP_NBR,SBSCR_NBR");
                strSQL = "update a set a.indv_sys_id = b.indv_sys_id from dbo.ACO_CS as a inner join ACO_Member_Cache as b on a.medicaid_no=b.SBSCR_MEDCD_RCIP_NBR and a.SUBSCRIBER_ID=b.SBSCR_NBR where a.indv_sys_id is null AND b.is_matched = 1";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //STEP 1.2 HISTORICAL IL_UCA.ACO_CS VS IL_UCA.ACO_Member_Cache BY medicaid_no
                Console.WriteLine("Step 2.11 indv_sys_id clean: Historical SBSCR_MEDCD_RCIP_NBR");
                strSQL = "update a set a.indv_sys_id = b.indv_sys_id from dbo.ACO_CS as a inner join ACO_Member_Cache as b on a.medicaid_no=b.SBSCR_MEDCD_RCIP_NBR where a.indv_sys_id is null AND b.is_matched = 1";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //STEP 1.3 HISTORICAL IL_UCA.ACO_CS VS IL_UCA.ACO_Member_Cache BY SUBSCRIBER_ID
                Console.WriteLine("Step 2.12 indv_sys_id clean: Historical SBSCR_NBR");
                strSQL = "update a set a.indv_sys_id = b.indv_sys_id from dbo.ACO_CS as a inner join ACO_Member_Cache as b on a.SUBSCRIBER_ID=b.SBSCR_NBR where a.indv_sys_id is null AND b.is_matched = 1";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //STEP 1.4 HISTORICAL IL_UCA.ACO_CS VS IL_UCA.ACO_Member_Cache BY FN/LN/BD
                Console.WriteLine("Step 2.13 indv_sys_id clean: Historical FN/LN/BD");
                strSQL = "update a set a.indv_sys_id = b.indv_sys_id from dbo.ACO_CS as a inner join ACO_Member_Cache as b on dbo.fnRegExeReplaceCSG(b.MBR_FST_NM,'[^a-zA-Z]') = dbo.fnRegExeReplaceCSG(a.MEMFNAME,'[^a-zA-Z]') AND dbo.fnRegExeReplaceCSG(b.MBR_LST_NM,'[^a-zA-Z]') = dbo.fnRegExeReplaceCSG(a.MEMLNAME,'[^a-zA-Z]')  AND a.[BTH_DT]=b.[BTH_DT] where a.indv_sys_id is null  AND b.is_matched = 1";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");


                //UGAP SBSCR_MEDCD_RCIP_NBR ACO_CS.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT MEDICAID_NO AS SBSCR_MEDCD_RCIP_NBR FROM dbo.ACO_CS WHERE indv_sys_id IS NULL AND MEDICAID_NO IS NOT NULL";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_MedCdNbr").Replace("{$vtt_create_columns}", "SBSCR_MEDCD_RCIP_NBR VARCHAR(16)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(SBSCR_MEDCD_RCIP_NBR) VALUES ('{$SBSCR_MEDCD_RCIP_NBR}'); </filter_1_loop>").Replace("{$vtt_columns}", "SBSCR_MEDCD_RCIP_NBR").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by SBSCR_MEDCD_RCIP_NBR").Replace("{$final_filter}", "WHERE m.SBSCR_MEDCD_RCIP_NBR IN (SELECT SBSCR_MEDCD_RCIP_NBR FROM MissingMembersTmp)").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_MedCdNbr'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "INDV_SYS_ID [int] NULL, SBSCR_MEDCD_RCIP_NBR [varchar](50) NULL").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.SBSCR_MEDCD_RCIP_NBR = a.SBSCR_MEDCD_RCIP_NBR OUTPUT INSERTED.INDV_SYS_ID, INSERTED.SBSCR_MEDCD_RCIP_NBR").Replace("{$table_to_update}", "inner join dbo.ACO_CS as b on a.SBSCR_MEDCD_RCIP_NBR = b.MEDICAID_NO").Replace("{$table_to_update2}", "dbo.ACO_CS as b inner join #Member_Cache_Updated as a on b.MEDICAID_NO = a.SBSCR_MEDCD_RCIP_NBR")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.INDV_SYS_ID's by SBSCR_MEDCD_RCIP_NBR in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP SBSCR_NBR ACO_CS.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT [SUBSCRIBER_ID]  AS SBSCR_NBR FROM dbo.ACO_CS WHERE indv_sys_id IS NULL AND [SUBSCRIBER_ID] IS NOT NULL";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_SbcrNbr").Replace("{$vtt_create_columns}", "SBSCR_NBR VARCHAR(11)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(SBSCR_NBR) VALUES ('{$SBSCR_NBR}'); </filter_1_loop>").Replace("{$vtt_columns}", "SBSCR_NBR").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by SBSCR_NBR").Replace("{$final_filter}", "WHERE m.SBSCR_NBR IN (SELECT SBSCR_NBR FROM MissingMembersTmp)").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_SbcrNbr'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "INDV_SYS_ID [int] NULL, SBSCR_NBR [varchar](50) NULL").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.SBSCR_NBR = a.SBSCR_NBR OUTPUT INSERTED.INDV_SYS_ID, INSERTED.SBSCR_NBR").Replace("{$table_to_update}", "inner join dbo.ACO_CS as b on b.SUBSCRIBER_ID = a.SBSCR_NBR").Replace("{$table_to_update2}", "dbo.ACO_CS as b inner join #Member_Cache_Updated as a on b.SUBSCRIBER_ID = a.SBSCR_NBR")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.INDV_SYS_ID's by SBSCR_NBR in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP FNLNBD ACO_CS.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT SUBSTRING(MEMFNAME, 1, 2) as MEMFNAME_SUB, SUBSTRING(MEMLNAME, 1, 3) as MEMLNAME_SUB,CONVERT(char(10), BTH_DT,126) as BTH_DT FROM dbo.ACO_CS WHERE indv_sys_id IS NULL AND MEMFNAME IS NOT NULL and MEMLNAME IS NOT NULL AND BTH_DT IS NOT NULL";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_FNSub%LNSub%BD").Replace("{$vtt_create_columns}", "MBR_FST_NM VARCHAR(10), MBR_LST_NM VARCHAR(10), BTH_DT DATE").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(MBR_FST_NM,MBR_LST_NM,BTH_DT ) VALUES ('{$MEMFNAME_SUB}%','{$MEMLNAME_SUB}%','{$BTH_DT}'); </filter_1_loop>").Replace("{$vtt_columns}", "MBR_FST_NM,MBR_LST_NM,BTH_DT").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by FNSub%LNSub%BD").Replace("{$final_filter}", "inner join MissingMembersTmp as mm on m.MBR_FST_NM LIKE mm.MBR_FST_NM AND  m.MBR_LST_NM LIKE mm.MBR_LST_NM AND m.BTH_DT=mm.BTH_DT").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_FNSub%LNSub%BD'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "[INDV_SYS_ID] [int] NULL, [MBR_FST_NM] [varchar](50) NULL, [MBR_LST_NM] [varchar](150) NULL, [BTH_DT] [date] NULL").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.MBR_FST_NM = a.MBR_FST_NM, a.MBR_LST_NM = a.MBR_LST_NM, a.BTH_DT = a.BTH_DT OUTPUT INSERTED.INDV_SYS_ID, INSERTED.MBR_FST_NM, INSERTED.MBR_LST_NM , INSERTED.BTH_DT").Replace("{$table_to_update}", "inner join dbo.ACO_CS as b on a.BTH_DT = b.BTH_DT AND ISNULL(CHARINDEX(SUBSTRING(dbo.fnRegExeReplaceCSG(b.MEMFNAME, '[^a-zA-Z]'),1,"+intSubstrFNLength+"), dbo.fnRegExeReplaceCSG(a.MBR_FST_NM, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(SUBSTRING(dbo.fnRegExeReplaceCSG(b.MEMLNAME, '[^a-zA-Z]'),1,"+intSubstrLNLength+"), dbo.fnRegExeReplaceCSG(a.MBR_LST_NM, '[^a-zA-Z]')), 0) > 0").Replace("{$table_to_update2}", "dbo.ACO_CS as b inner join #Member_Cache_Updated as a on a.BTH_DT = b.BTH_DT AND ISNULL(CHARINDEX(SUBSTRING(dbo.fnRegExeReplaceCSG(b.MEMFNAME, '[^a-zA-Z]'),1,"+intSubstrFNLength+"), dbo.fnRegExeReplaceCSG(a.MBR_FST_NM, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(SUBSTRING(dbo.fnRegExeReplaceCSG(b.MEMLNAME, '[^a-zA-Z]'),1,"+intSubstrLNLength+"), dbo.fnRegExeReplaceCSG(a.MBR_LST_NM, '[^a-zA-Z]')), 0) > 0")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.INDV_SYS_ID's by FNLNBD in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //Retrieving ACO_CS.INDV_SYS_ID CACHE 
                //Retrieving ACO_CS.INDV_SYS_ID CACHE 
                //Retrieving ACO_CS.INDV_SYS_ID CACHE 
                Console.WriteLine("Step 2.14 ACO_CS.INDV_SYS_ID main cleanup...");
                IR_DataScrubber.processQuerySet(dtQueries, strFilterTagGeneric: "<filter_{$x}_loop>");
                dtQueries.Clear();
            }//CS CLEAN END


            if (blRefreshCSTable && blUpdateProcessingSummary)
            {
                //GET FINAL SUMMARY FROM ACO_CS
                Console.WriteLine("Step 2.15 Gather Collecting final 'CS' counts for dbo.ACO_Processing_Summary");
                strSQL = "UPDATE dbo.ACO_Processing_Summary SET found_mpin_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT CURR_PCP_FIRST_NAME, CURR_PCP_LAST_NAME FROM ACO_CS WHERE MPIN IS NULL) as tmp), found_indv_sys_id_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_CS WHERE indv_sys_id IS NULL) as tmp) WHERE LOB = 'CS' AND processing_date = '" + strDateStamp + "';";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("Final dbo.ACO_Processing_Summary Updated");
            }

            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_CS END////////////////////////////////////////////////////////////////////////////


            //SECTION BREAK ON COMMAND PROMPT
            Console.WriteLine("--------------------------------------------------------------------");


            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR START////////////////////////////////////////////////////////////////////////////

            if (blRefreshMRTable)
            {
                //CLEAR OUT IL_UCA.ACO_MR TABLE
                Console.WriteLine("Step 3.1 Truncating table IL_UCA.ACO_MR... ");
                DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "TRUNCATE TABLE [dbo].[ACO_MR]");//CLEAR OUT LIVE TABLES
                                                                                                        //GET DATA FROM PAIR.ACO_Exec_REGISTRY_MR_UHC
                                                                                                        //strSQL = "SELECT Distinct CASE WHEN ISNULL(INDV_SYS_ID,0) = 0 THEN NULL ELSE convert(int,INDV_SYS_ID) END AS indv_sys_id, CASE WHEN ISNULL(hicnbr,'') = '' THEN NULL ELSE hicnbr END AS hicnbr, CASE WHEN ISNULL(MBR_FST_NM,'') = '' THEN NULL ELSE MBR_FST_NM END AS MEMFNAME, CASE WHEN ISNULL(MBR_LST_NM,'') = '' THEN NULL ELSE MBR_LST_NM END AS MEMLNAME, CASE WHEN ISNULL(BTH_DT,'') = '' THEN NULL ELSE convert(date,BTH_DT) END AS BTH_DT, CASE WHEN ISNULL(zip_cd_perm,'') = '' THEN NULL ELSE left(zip_cd_perm,5) END AS MBR_ZIP_CD, CASE WHEN ISNULL(SRVC_MPIN,0) = 0 THEN NULL ELSE convert(int,SRVC_MPIN) END AS MPIN, CASE WHEN ISNULL(SRVC_TIN,0) = 0 THEN NULL ELSE convert(int,SRVC_TIN) END AS TIN, CASE WHEN ISNULL(provider_TIN,0) = 0 THEN NULL ELSE convert(int,provider_TIN) END AS prov_TIN, CASE WHEN ISNULL(network_name,'') = '' THEN NULL ELSE UPPER(network_name) END AS ACO_Name, 'MCR' as Lob, CASE WHEN ISNULL(provider_id,'') = '' THEN NULL ELSE provider_id END AS provider_id, CASE WHEN ISNULL(provider_name,'') = '' THEN NULL ELSE provider_name END AS provider_name, CASE WHEN ISNULL(SRVC_SITE_CD,'') = '' THEN NULL ELSE SRVC_SITE_CD END AS prov_SITE_CD, SUBSTRING(member_ID, 0, LEN(member_ID) - (LEN(member_ID) - CHARINDEX('-', member_ID))) as member_ID FROM ACO_Exec_REGISTRY_MR_OHP UNION SELECT Distinct CASE WHEN ISNULL(INDV_SYS_ID,0) = 0 THEN NULL ELSE convert(int,INDV_SYS_ID) END AS indv_sys_id, CASE WHEN ISNULL(hicnbr,'') = '' THEN NULL ELSE hicnbr END AS hicnbr, CASE WHEN ISNULL(first_name,'') = '' THEN NULL ELSE first_name END AS MEMFNAME, CASE WHEN ISNULL(last_name ,'') = '' THEN NULL ELSE last_name END AS MEMLNAME, CASE WHEN ISNULL(date_of_birth,'') = '' THEN NULL ELSE convert(date,date_of_birth) END AS BTH_DT, NULL as MBR_ZIP_CD, CASE WHEN ISNULL(SRVC_MPIN,0) = 0 THEN NULL ELSE convert(int,SRVC_MPIN) END AS MPIN, CASE WHEN ISNULL(SRVC_TIN,0) = 0 THEN NULL ELSE convert(int,SRVC_TIN) END AS TIN, CASE WHEN ISNULL(provider_TIN,0) = 0 THEN NULL ELSE convert(int,provider_TIN) END AS prov_TIN, CASE WHEN ISNULL(network_name,'') = '' THEN NULL ELSE UPPER(network_name) END AS ACO_Name, 'MCR' as Lob, CASE WHEN ISNULL(provider_id,'') = '' THEN NULL ELSE provider_id END AS provider_id, CASE WHEN ISNULL(provider_name,'') = '' THEN NULL ELSE provider_name END AS provider_name, CASE WHEN ISNULL(SRVC_SITE_CD,'') = '' THEN NULL ELSE SRVC_SITE_CD END AS prov_SITE_CD, SUBSTRING(member_ID, 0, LEN(member_ID) - (LEN(member_ID) - CHARINDEX('-', member_ID))) as member_ID FROM dbo.ACO_Exec_REGISTRY_MR_UHC";

                //ADDED 2/25/2022 FOR MISSING PROV_TIN
                strSQL = "SELECT indv_sys_id, hicnbr, MEMFNAME, MEMLNAME, BTH_DT, MBR_ZIP_CD, MPIN, TIN, CASE WHEN prov_TIN = 0 THEN NULL ELSE prov_TIN END as prov_TIN, ACO_Name, Lob, provider_id, provider_name, prov_SITE_CD, member_ID FROM ( SELECT indv_sys_id, hicnbr, MEMFNAME, MEMLNAME, BTH_DT, MBR_ZIP_CD, MPIN, TIN, MAX(isnull(prov_TIN,0)) as prov_TIN, ACO_Name, Lob, provider_id, provider_name, prov_SITE_CD, member_ID FROM ( SELECT Distinct CASE WHEN ISNULL(INDV_SYS_ID,0) = 0 THEN NULL ELSE convert(int,INDV_SYS_ID) END AS indv_sys_id, CASE WHEN ISNULL(hicnbr,'') = '' THEN NULL ELSE hicnbr END AS hicnbr, CASE WHEN ISNULL(MBR_FST_NM,'') = '' THEN NULL ELSE MBR_FST_NM END AS MEMFNAME, CASE WHEN ISNULL(MBR_LST_NM,'') = '' THEN NULL ELSE MBR_LST_NM END AS MEMLNAME, CASE WHEN ISNULL(BTH_DT,'') = '' THEN NULL ELSE convert(date,BTH_DT) END AS BTH_DT, CASE WHEN ISNULL(zip_cd_perm,'') = '' THEN NULL ELSE left(zip_cd_perm,5) END AS MBR_ZIP_CD, CASE WHEN ISNULL(SRVC_MPIN,0) = 0 THEN NULL ELSE convert(int,SRVC_MPIN) END AS MPIN, CASE WHEN ISNULL(SRVC_TIN,0) = 0 THEN NULL ELSE convert(int,SRVC_TIN) END AS TIN, CASE WHEN ISNULL(provider_TIN,0) = 0 THEN NULL ELSE convert(int,provider_TIN) END AS prov_TIN, CASE WHEN ISNULL(network_name,'') = '' THEN NULL ELSE UPPER(network_name) END AS ACO_Name, 'MCR' as Lob, CASE WHEN ISNULL(provider_id,'') = '' THEN NULL ELSE provider_id END AS provider_id, CASE WHEN ISNULL(provider_name,'') = '' THEN NULL ELSE provider_name END AS provider_name, CASE WHEN ISNULL(SRVC_SITE_CD,'') = '' THEN NULL ELSE SRVC_SITE_CD END AS prov_SITE_CD, SUBSTRING(member_ID, 0, LEN(member_ID) - (LEN(member_ID) - CHARINDEX('-', member_ID))) as member_ID FROM ACO_Exec_REGISTRY_MR_OHP UNION SELECT Distinct CASE WHEN ISNULL(INDV_SYS_ID,0) = 0 THEN NULL ELSE convert(int,INDV_SYS_ID) END AS indv_sys_id, CASE WHEN ISNULL(hicnbr,'') = '' THEN NULL ELSE hicnbr END AS hicnbr, CASE WHEN ISNULL(first_name,'') = '' THEN NULL ELSE first_name END AS MEMFNAME, CASE WHEN ISNULL(last_name ,'') = '' THEN NULL ELSE last_name END AS MEMLNAME, CASE WHEN ISNULL(date_of_birth,'') = '' THEN NULL ELSE convert(date,date_of_birth) END AS BTH_DT, NULL as MBR_ZIP_CD, CASE WHEN ISNULL(SRVC_MPIN,0) = 0 THEN NULL ELSE convert(int,SRVC_MPIN) END AS MPIN, CASE WHEN ISNULL(SRVC_TIN,0) = 0 THEN NULL ELSE convert(int,SRVC_TIN) END AS TIN, CASE WHEN ISNULL(provider_TIN,0) = 0 THEN NULL ELSE convert(int,provider_TIN) END AS prov_TIN, CASE WHEN ISNULL(network_name,'') = '' THEN NULL ELSE UPPER(network_name) END AS ACO_Name, 'MCR' as Lob, CASE WHEN ISNULL(provider_id,'') = '' THEN NULL ELSE provider_id END AS provider_id, CASE WHEN ISNULL(provider_name,'') = '' THEN NULL ELSE provider_name END AS provider_name, CASE WHEN ISNULL(SRVC_SITE_CD,'') = '' THEN NULL ELSE SRVC_SITE_CD END AS prov_SITE_CD, SUBSTRING(member_ID, 0, LEN(member_ID) - (LEN(member_ID) - CHARINDEX('-', member_ID))) as member_ID FROM dbo.ACO_Exec_REGISTRY_MR_UHC )tmp GROUP BY indv_sys_id, hicnbr, MEMFNAME, MEMLNAME, BTH_DT, MBR_ZIP_CD, MPIN, TIN, ACO_Name, Lob, provider_id, provider_name, prov_SITE_CD, member_ID ) tmp2";
                Console.WriteLine("Step 3.2 Gathering data from PAIR.ACO_Exec_REGISTRY_MR_UHC... ");
                intResultCnt = (int)DBConnection32.getMSSQLExecuteScalar(strACO_PIAR_ConnectionString, "SELECT COUNT(*) as total FROM (" + strSQL + ") tmp ");
                Console.WriteLine("Step 3.3 Initializing data transfer from PAIR.ACO_Exec_REGISTRY_MR_UHC to IL_UCA.ACO_MR:");
                strMessageGlobal = "--{$rowCnt} out of " + String.Format("{0:n0}", intResultCnt) + " rows inserted...";
                dtStartTime = DateTime.Now;
                //TRANSFER PAIR.ACO_Exec_REGISTRY_MR_UHC DATA TO ILUCA.ACO_MR
                SQLServerBulkImport(strACO_PIAR_ConnectionString, strILUCA_ConnectionString, strSQL, "ACO_MR"); //BULK DATA LOAD
                dtEndTime = DateTime.Now;
                tsTimeSpan = dtEndTime.Subtract(dtStartTime);
                strTimeMessage = (tsTimeSpan.Hours == 0 ? "" : tsTimeSpan.Hours + "hr:") + (tsTimeSpan.Minutes == 0 ? "" : tsTimeSpan.Minutes + "min:") + (tsTimeSpan.Seconds == 0 ? "" : tsTimeSpan.Seconds + "sec");
                Console.Write("\r" + strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", intResultCnt)).Replace("...", ""));
                Console.WriteLine("");
                Console.WriteLine("--Bulk transfer completed in:  " + strTimeMessage.TrimEnd(':'));
                if (blUpdateRaw)
                {
                    //TRANSFER ANY UNIQUE NEW ILUCA.ACO_MR TO IL_UCA.ACO_MR_RAW BEFORE CLEANING
                    Console.WriteLine("Step 3.5 Transfer ILUCA.ACO_MR to IL_UCA.ACO_MR_RAW:");
                    strSQL = "INSERT INTO dbo.ACO_MR_RAW ( indv_sys_id ,hicnbr ,MEMFNAME ,MEMLNAME ,BTH_DT ,MBR_ZIP_CD ,MPIN ,TIN ,prov_TIN ,ACO_Name ,Lob ,provider_id ,provider_name ,prov_fn ,prov_ln ,prov_SITE_CD, member_ID ) SELECT indv_sys_id ,hicnbr ,MEMFNAME ,MEMLNAME ,BTH_DT ,MBR_ZIP_CD ,MPIN ,TIN ,prov_TIN ,ACO_Name ,Lob ,provider_id ,provider_name ,provider_name_cln_fn as prov_fn ,provider_name_cln_ln as prov_ln ,prov_SITE_CD, member_ID FROM dbo.ACO_MR as a WHERE not exists ( select * from ACO_MR_RAW r where ( ( a.indv_sys_id = r.indv_sys_id OR ( a.indv_sys_id Is Null AND r.indv_sys_id Is Null ) ) AND ( a.hicnbr = r.hicnbr OR ( a.hicnbr Is Null AND r.hicnbr Is Null ) ) AND ( a.MEMFNAME = r.MEMFNAME OR ( a.MEMFNAME Is Null AND r.MEMFNAME Is Null ) ) AND ( a.MEMLNAME = r.MEMLNAME OR ( a.MEMLNAME Is Null AND r.MEMLNAME Is Null ) ) AND ( a.BTH_DT = r.BTH_DT OR ( a.BTH_DT Is Null AND r.BTH_DT Is Null ) ) AND ( a.MBR_ZIP_CD = r.MBR_ZIP_CD OR ( a.MBR_ZIP_CD Is Null AND r.MBR_ZIP_CD Is Null ) ) AND ( a.provider_id = r.provider_id OR ( a.provider_id Is Null AND r.provider_id Is Null ) ) AND ( a.MPIN = r.MPIN OR ( a.MPIN Is Null AND r.MPIN Is Null ) ) AND ( a.provider_name = r.provider_name OR ( a.provider_name Is Null AND r.provider_name Is Null ) ) AND ( a.provider_name_cln_fn = r.prov_fn OR ( a.provider_name_cln_fn Is Null AND r.prov_fn Is Null ) ) AND ( a.provider_name_cln_ln = r.prov_ln OR ( a.provider_name_cln_ln Is Null AND r.prov_ln Is Null ) ) AND ( a.prov_SITE_CD = r.prov_SITE_CD OR ( a.prov_SITE_CD Is Null AND r.prov_SITE_CD Is Null ) ) AND ( a.TIN = r.TIN OR ( a.TIN Is Null AND r.TIN Is Null ) ) AND ( a.ACO_Name = r.ACO_Name OR ( a.ACO_Name Is Null AND r.ACO_Name Is Null ) ) AND ( a.Lob = r.Lob OR ( a.Lob Is Null AND r.Lob Is Null ) )AND ( a.prov_TIN = r.prov_TIN OR ( a.prov_TIN Is Null AND r.prov_TIN Is Null ) )  AND  ( a.member_ID = r.member_ID OR ( a.member_ID Is Null AND r.member_ID Is Null ) )  ) )";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine(intResultCnt + " rows inserted into dbo.ACO_MR_RAW");
                    //TRANSFER ILUCA.ACO_MR TO IL_UCA.ACO_MR_RAW_DATES FOR ARHIVING
                    Console.WriteLine("Step 3.6 Transfer ILUCA.ACO_MR to IL_UCA.ACO_MR_RAW_DATES:");
                    strSQL = "INSERT INTO dbo.ACO_MR_RAW_DATES ( aco_mr_id, date_stamp) SELECT aco_mr_id, '" + strDateStamp + "' as date_stamp FROM dbo.ACO_MR_RAW as r WHERE exists ( select * from ACO_MR a where ( ( a.indv_sys_id = r.indv_sys_id OR ( a.indv_sys_id Is Null AND r.indv_sys_id Is Null ) ) AND ( a.hicnbr = r.hicnbr OR ( a.hicnbr Is Null AND r.hicnbr Is Null ) ) AND ( a.MEMFNAME = r.MEMFNAME OR ( a.MEMFNAME Is Null AND r.MEMFNAME Is Null ) ) AND ( a.MEMLNAME = r.MEMLNAME OR ( a.MEMLNAME Is Null AND r.MEMLNAME Is Null ) ) AND ( a.BTH_DT = r.BTH_DT OR ( a.BTH_DT Is Null AND r.BTH_DT Is Null ) ) AND ( a.MBR_ZIP_CD = r.MBR_ZIP_CD OR ( a.MBR_ZIP_CD Is Null AND r.MBR_ZIP_CD Is Null ) ) AND ( a.provider_id = r.provider_id OR ( a.provider_id Is Null AND r.provider_id Is Null ) ) AND ( a.MPIN = r.MPIN OR ( a.MPIN Is Null AND r.MPIN Is Null ) ) AND ( a.provider_name = r.provider_name OR ( a.provider_name Is Null AND r.provider_name Is Null ) ) AND ( a.provider_name_cln_fn = r.prov_fn OR ( a.provider_name_cln_fn Is Null AND r.prov_fn Is Null ) ) AND ( a.provider_name_cln_ln = r.prov_ln OR ( a.provider_name_cln_ln Is Null AND r.prov_ln Is Null ) ) AND ( a.prov_SITE_CD = r.prov_SITE_CD OR ( a.prov_SITE_CD Is Null AND r.prov_SITE_CD Is Null ) ) AND ( a.TIN = r.TIN OR ( a.TIN Is Null AND r.TIN Is Null ) ) AND ( a.ACO_Name = r.ACO_Name OR ( a.ACO_Name Is Null AND r.ACO_Name Is Null ) ) AND ( a.Lob = r.Lob OR ( a.Lob Is Null AND r.Lob Is Null ) )AND ( a.prov_TIN = r.prov_TIN OR ( a.prov_TIN Is Null AND r.prov_TIN Is Null ) )  AND  ( a.member_ID = r.member_ID OR ( a.member_ID Is Null AND r.member_ID Is Null ) )  ) )";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine(intResultCnt + " rows inserted into IL_UCA.ACO_MR_RAW_DATES");
                }
                
                if(blUpdateProcessingSummary)
                {
                    //GET INITIAL SUMMARY FROM ACO_MR
                    Console.WriteLine("Step 3.7 Collecting initial 'MR' counts for dbo.ACO_Processing_Summary");
                    strSQL = "UPDATE dbo.ACO_Processing_Summary SET total_row_cnt = (SELECT COUNT(*) FROM dbo.ACO_MR), total_distinct_prov_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT provider_name  FROM ACO_MR) as tmp), total_distinct_mbr_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_MR) as tmp), missing_mpin_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT provider_name  FROM ACO_MR WHERE MPIN IS NULL) as tmp), missing_indv_sys_id_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_MR WHERE indv_sys_id IS NULL) as tmp) WHERE LOB = 'MR' AND processing_date = '" + strDateStamp + "'; IF @@ROWCOUNT=0 INSERT INTO dbo.ACO_Processing_Summary (LOB ,processing_date ,total_row_cnt ,total_distinct_prov_cnt ,total_distinct_mbr_cnt ,missing_mpin_cnt ,missing_indv_sys_id_cnt) SELECT 'MR' as LOB, '" + strDateStamp + "' as processing_date, (SELECT COUNT(*) FROM dbo.ACO_MR) as total_row_cnt , (SELECT COUNT(*) FROM (SELECT DISTINCT provider_name  FROM ACO_MR) as tmp) as total_distinct_prov_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_MR) as tmp) as total_distinct_mbr_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT provider_name  FROM ACO_MR WHERE MPIN IS NULL) as tmp) as missing_mpin_cnt, (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_MR WHERE indv_sys_id IS NULL) as tmp) as missing_indv_sys_id_cnt";
                    intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                    Console.WriteLine("Initial dbo.ACO_Processing_Summary Updated");
                }
                
                //SPECIAL STEP FOR PARSING THROUGH ACO_MR.provider_name via IR_DataScrubber.cleanupProviderNamesTmpTblUpdate
                Console.WriteLine("Step 3.8 Scrape and capture provider_name segments");
                IR_DataScrubber.cleanupProviderNamesTmpTblUpdate("select DISTINCT provider_name FROM dbo.ACO_MR WHERE provider_name_cln_ln IS NULL AND [provider_name] IS NOT NULL", "DECLARE @Rows int; SET @Rows = 0; CREATE TABLE #TempCleanProviders ( [provider_name] [varchar](150) NULL, [provider_name_cln_fn] [varchar](50) NULL, [provider_name_cln_ln] [varchar](50) NULL, [provider_name_cln_ln2] [varchar](50) NULL,[provider_name_cln_ln3] [varchar](50) NULL, [provider_name_cln_mn] [varchar](50) NULL, [provider_name_cln_mn2] [varchar](50) NULL, [provider_name_cln_provdgr] [varchar](15) NULL, [provider_name_cln_suffix] [varchar](15) NULL );INSERT INTO #TempCleanProviders {$table}; Create NonClustered Index TMP_INDX_CleanProviders On #TempCleanProviders(provider_name); UPDATE a SET a.provider_name_cln_fn = b.provider_name_cln_fn , a.provider_name_cln_ln = b.provider_name_cln_ln , a.provider_name_cln_ln2 = b.provider_name_cln_ln2, a.provider_name_cln_ln3 = b.provider_name_cln_ln3 , a.provider_name_cln_mn = b.provider_name_cln_mn , a.provider_name_cln_mn2 = b.provider_name_cln_mn2 , a.provider_name_cln_provdgr = b.provider_name_cln_provdgr , a.provider_name_cln_suffix = b.provider_name_cln_suffix FROM dbo.ACO_MR as a inner join #TempCleanProviders as b on a.provider_name = b.provider_name WHERE a.provider_name_cln_ln IS NULL; SELECT @Rows=@@ROWCOUNT; SELECT @Rows AS Rows;", strILUCA_ConnectionString);

            }

            if (blCleanMR)
            {
                ////////////////////////////////////////////////////////////////////////////////////////////START MR MPIN CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////START MR MPIN CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////

                //STEP 1.1 HISTORICAL IL_UCA.ACO_MR VS IL_UCA.ACO_Provider_Cache BY CURR_PCP_FIRST_NAME,CURR_PCP_LAST_NAME
                Console.WriteLine("Step 3.9 mpin clean: Historical PROV_FST_NM,PROV_LST_NM");
                strSQL = "update a set a.mpin = b.mpin from dbo.ACO_MR as a inner join ACO_Provider_Cache as b on ISNULL(a.provider_name_cln_fn,'')=ISNULL(b.PROV_FST_NM,'') and ISNULL(a.provider_name_cln_ln,'')= ISNULL(b.PROV_LST_NM, '') WHERE a.mpin is NULL AND b.is_matched = 1";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //STEP 1.2 CURRENT IL_UCA.ACO_MR VS IL_UCA.ACO_MR
                Console.WriteLine("Step 3.10 mpin clean: ACO_MR vs ACO_MR");
                strSQL = "update a set a.MPIN = b.MPIN from dbo.ACO_MR as a inner join ( select aa.mpin, aa.CURR_PCP_FIRST_NAME, aa.CURR_PCP_LAST_NAME from ( select DISTINCT mpin,provider_name_cln_fn as [CURR_PCP_FIRST_NAME],provider_name_cln_ln as [CURR_PCP_LAST_NAME] FROM dbo.ACO_MR as aa where mpin is not null ) as aa inner join (select DISTINCT provider_name_cln_fn AS [CURR_PCP_FIRST_NAME],provider_name_cln_ln AS [CURR_PCP_LAST_NAME] FROM dbo.ACO_MR as aa where mpin is null ) as bb on aa.CURR_PCP_FIRST_NAME = bb.CURR_PCP_FIRST_NAME and aa.CURR_PCP_LAST_NAME = bb.CURR_PCP_LAST_NAME ) as b on a.provider_name_cln_fn = b.CURR_PCP_FIRST_NAME AND a.provider_name_cln_ln = b.CURR_PCP_LAST_NAME where a.mpin is null";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //PREP MR FOR IR_DataScrubber.processQuerySet(DataTable dtQueries
                dtQueries.Clear();
                IR_DataScrubber.strCurrentLOB = "MR";
                IR_DataScrubber.strCurrentMainTable = "ACO_MR";

                //POSSIBLE ADDITIONAL MR.MPIN FILTERS:
                //POSSIBLE ADDITIONAL MR.MPIN FILTERS:
                //POSSIBLE ADDITIONAL MR.MPIN FILTERS:
                //drArrResults = dtNDBDownloadedSourcesByTIN.Select((strTIN != null ? "taxid = " + strTIN + " AND " : "") + "FirstName  LIKE '" + strProvFirstNameGlobal + "%' AND LastName  LIKE '" + strProvLastNameGlobal + "%' AND (('" + strProvMiddleNameGlobal + "%' <> '%' AND MiddleName LIKE '" + strProvMiddleNameGlobal + "%') OR ('" + strProvMiddleNameGlobal2 + "%' <> '%' AND MiddleName LIKE '" + strProvMiddleNameGlobal2 + "%')  OR ('" + strProvMiddleNameGlobal + "%' = '%' AND '" + strProvMiddleNameGlobal2 + "%' = '%')  )   AND ProvDegree = '" + strProvDegreeGlobal + "'");
                //drArrResults = dtUGAPDownloadedSourcesByProvId.Select("PROV_ID = '" + strProvId + "' AND  (PROV_LST_NM  LIKE '" + strProvLastNameGlobal + "%' OR PROV_LST_NM  LIKE '" + strProvLastNameGlobal2 + "%'  ) AND PROV_FST_NM LIKE '" + strProvFirstNameGlobal + "%'");
                //drArrResults = dtUGAPDownloadedSourcesByTIN.Select((strTIN != null ? "TIN = " + strTIN + " AND " : "") + "(PROV_LST_NM  LIKE '" + strProvLastNameGlobal + "%' OR PROV_LST_NM  LIKE '" + strProvLastNameGlobal2 + "%'  ) AND PROV_FST_NM LIKE '" + strProvFirstNameGlobal + "%'");
                // drArrResults = dtNDBLiveSources.Select("ProvDegree='" + strProvDegreeGlobal + "'");

                //UGAP CURR_PCP_ID CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT provider_id as CURR_PCP_ID, provider_name_cln_fn as [CURR_PCP_FIRST_NAME], provider_name_cln_ln as [CURR_PCP_LAST_NAME], SUBSTRING([provider_name_cln_ln],1,3) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_MR WHERE MPIN IS NULL AND provider_id IS NOT NULL AND provider_name_cln_ln IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_PROVIDER_SEARCH.Replace("{$data_source_case}", "CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM = pp.PROV_LST_NM) THEN 'UGAP_ProvIdFN%Ln' ELSE CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD ) THEN 'UGAP_ProvIdFN%Ln%' ELSE 'UGAP_ProvIdLnSub%' END END").Replace("{$vtt_create_columns}", "PROV_FST_NM_WILD VARCHAR(50), PROV_LST_NM VARCHAR(150), PROV_LST_NM_WILD VARCHAR(150),PROV_LST_NM_SUB VARCHAR(20), PROV_ID VARCHAR(11)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO PotentialProvidersTmp(PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, PROV_ID) VALUES ('{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%','{$CURR_PCP_ID}'); </filter_1_loop>").Replace("{$vtt_columns}", "PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, PROV_ID").Replace("{$notes}", "Found in UHCDM001.PROVIDER by CURR_PCP_ID").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$extra_cols}", ",CAST(p.TIN as INT) as taxid, p.DEA_NBR as DEANbr, CAST(null as varchar(20)) as MiddleName, CAST(null as varchar(20)) as SBSCR_MEDCD_RCIP_NBR, p.PROV_ID as CURR_PCP_ID, CAST(null as varchar(20)) as ProvDegree,CAST(null as varchar(20)) as PrimSpec").Replace("{$final_filter}", "inner join PotentialProvidersTmp as pp on p.PROV_LST_NM LIKE pp.PROV_LST_NM_SUB AND p.PROV_ID=pp.PROV_ID").Replace("{$main_query}", IR_DataScrubber.strSQL_UGAP_PROVIDER_TABLE_BY_PROVIDER);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.CURR_PCP_ID = a.CURR_PCP_ID, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.CURR_PCP_ID, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "CURR_PCP_IDFNWLN").Replace("{$tmp_index_columns}", "CURR_PCP_ID,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.provider_id = b.CURR_PCP_ID AND a.provider_name_cln_ln=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.CURR_PCP_ID = a.CURR_PCP_ID, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.CURR_PCP_ID, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "CURR_PCP_IDFNWLNW").Replace("{$tmp_index_columns}", "CURR_PCP_ID,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.provider_id = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.CURR_PCP_ID = a.CURR_PCP_ID, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.CURR_PCP_ID, INSERTED.PROV_LST_NM, NULL PROV_FST_NM").Replace("{$tmp_missing_join}", "a.CURR_PCP_ID = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0")
                + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "CURR_PCP_IDLNW").Replace("{$tmp_index_columns}", "CURR_PCP_ID,PROV_FST_NM,PROV_LST_NM")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.provider_id = b.CURR_PCP_ID AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0")
                + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",CURR_PCP_ID,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[CURR_PCP_ID] [varchar](20) NULL,[PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[CURR_PCP_ID] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",provider_id AS CURR_PCP_ID,provider_name_cln_ln AS PROV_LST_NM,SUBSTRING( dbo.fnRegExeReplaceCSG(provider_name_cln_ln, '[^a-zA-Z]'),1," + intSubstrLNLength + ") as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_fn, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UGAP_ProvIdFN%Ln','UGAP_ProvIdFN%Ln%', 'UGAP_ProvIdLnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "provider_id IS NOT NULL AND provider_name_cln_ln IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by CURR_PCP_ID in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UHN TIN CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT tin as TIN, provider_name_cln_fn as [CURR_PCP_FIRST_NAME], provider_name_cln_ln as [CURR_PCP_LAST_NAME], SUBSTRING([provider_name_cln_ln],1,3) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_MR WHERE MPIN IS NULL AND tin IS NOT NULL AND provider_name_cln_ln IS NOT NULL";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UHN_GENERIC_PROVIDER_SEARCH.Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_create_upt_columns}", "[FirstName] [varchar](50) NULL, [LastName] [varchar](150) NULL, [LastName_Sub] [varchar](15) NULL, [TIN] INT").Replace("{$tmp_columns}", "FirstName,LastName,LastName_Sub,TIN").Replace("{$tmp_insert_values}", "'{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%',{$TIN}").Replace("{$data_source_case}", "CASE WHEN (p.FirstName LIKE mp.FirstName AND p.LastName LIKE mp.LastName ) THEN 'UHN_TaxIdFN%Ln%' ELSE CASE WHEN (p.LastName LIKE  mp.LastName) THEN 'UHN_TaxIdLn%' ELSE 'UHN_TaxIdLnSub%' END END").Replace("{$notes}", "Found in NDB.PROVIDER as p inner join NDB.PROV_TIN_PAY_AFFIL by taxid").Replace("{$extra_cols}", ", a.taxid, NULL as DEANbr, p.MiddleName, NULL as SBSCR_MEDCD_RCIP_NBR, NULL as CURR_PCP_ID, p.ProvDegree, p.PrimSpec").Replace("{$joins}", "inner join PROV_TIN_PAY_AFFIL as a on a.MPIN = p.MPIN inner join #MissingProvidersTmp as mp on a.taxid = mp.TIN AND  p.LastName LIKE mp.LastName_Sub").Replace("{$PrSpecs}", "");
                //COLLECT BATCH UPDATE FOR TIN,PROV_FST_NM,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TNFNLN").Replace("{$tmp_index_columns}", "TIN,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0 ")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR TIN,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, NULL as PROV_FST_NM ").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TNLN").Replace("{$tmp_index_columns}", "TIN,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR TIN,PROV_LST_NM_SUB
                //sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ",a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, NULL as PROV_FST_NM ").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND  b.PROV_LST_NM LIKE SUBSTRING(a.PROV_LST_NM,1,5) + '%'")
                //    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(PROV_LST_NM,1,5) + '%';"
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TNLNSUB").Replace("{$tmp_index_columns}", "TIN,PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND a.provider_name_cln_ln LIKE b.PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",TIN,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[TIN] int NULL, [PROV_LST_NM] [varchar](150) NULL , [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[TIN] int NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",TIN,provider_name_cln_ln as PROV_LST_NM,SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_ln, '^a-zA-Z'),1," + intSubstrFNLength + ") as PROV_LST_NM_CLN,SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_fn, '^a-zA-Z'),1," + intSubstrLNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UHN_TaxIdFN%Ln%','UHN_TaxIdLn%','UHN_TaxIdLnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "tin IS NOT NULL AND provider_name_cln_ln IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by TIN in UNH";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUHN_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP TIN CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT tin as TIN, provider_name_cln_fn as [CURR_PCP_FIRST_NAME], provider_name_cln_ln as [CURR_PCP_LAST_NAME], SUBSTRING([provider_name_cln_ln],1,4) AS CURR_PCP_LAST_NAME_SUB FROM dbo.ACO_MR WHERE MPIN IS NULL AND tin IS NOT NULL AND provider_name_cln_ln IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_PROVIDER_SEARCH.Replace("{$data_source_case}", "CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM = pp.PROV_LST_NM) THEN 'UGAP_TaxIdFn%Ln' ELSE CASE WHEN (p.PROV_FST_NM LIKE pp.PROV_FST_NM_WILD AND p.PROV_LST_NM LIKE pp.PROV_LST_NM_WILD ) THEN 'UGAP_TaxIdFn%Ln%' ELSE 'UGAP_TaxIdLnSub%' END END").Replace("{$vtt_create_columns}", "PROV_FST_NM_WILD VARCHAR(50), PROV_LST_NM VARCHAR(150), PROV_LST_NM_WILD VARCHAR(150),PROV_LST_NM_SUB VARCHAR(20), TIN CHAR(9)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO PotentialProvidersTmp(PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB, TIN) VALUES ('{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%','{$CURR_PCP_LAST_NAME_SUB}%','{$TIN}'); </filter_1_loop>").Replace("{$vtt_columns}", "PROV_FST_NM_WILD,PROV_LST_NM,PROV_LST_NM_WILD,PROV_LST_NM_SUB,TIN").Replace("{$notes}", "Found in UHCDM001.PROVIDER by TIN").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$extra_cols}", ",CAST(p.TIN as INT) as taxid, p.DEA_NBR as DEANbr, CAST(null as varchar(20)) as MiddleName, CAST(null as varchar(20)) as SBSCR_MEDCD_RCIP_NBR, p.PROV_ID as CURR_PCP_ID, CAST(null as varchar(20)) as ProvDegree,CAST(null as varchar(20)) as PrimSpec").Replace("{$final_filter}", "inner join PotentialProvidersTmp as pp on p.PROV_LST_NM LIKE pp.PROV_LST_NM_SUB AND p.TIN=pp.TIN").Replace("{$main_query}", IR_DataScrubber.strSQL_UGAP_PROVIDER_TABLE_BY_PROVIDER);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TINFNWLN").Replace("{$tmp_index_columns}", "TIN,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND a.provider_name_cln_ln=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0  AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TINFNWLNW").Replace("{$tmp_index_columns}", "TIN,PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR CURR_PCP_ID,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.TIN = a.TIN, a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.TIN, INSERTED.PROV_LST_NM, NULL AS PROV_FST_NM").Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "TINLNW").Replace("{$tmp_index_columns}", "TIN,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.TIN = b.TIN AND ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0 ")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",TIN,PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[TIN] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[TIN] [varchar](20) NULL, [PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",TIN,provider_name_cln_ln AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_ln, '[^a-zA-Z]'),1," + intSubstrLNLength+ ") as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_fn, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UGAP_TaxIdFn%Ln','UGAP_TaxIdFn%Ln%','UGAP_TaxIdLnSub%'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "tin IS NOT NULL AND provider_name_cln_ln IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by TIN in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UHN SPEC CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT provider_name_cln_fn as [CURR_PCP_FIRST_NAME], provider_name_cln_ln as [CURR_PCP_LAST_NAME]  FROM dbo.ACO_MR WHERE MPIN IS NULL AND provider_name_cln_ln IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UHN_GENERIC_PROVIDER_SEARCH.Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_create_upt_columns}", "[FirstName] [varchar](50) NULL, [LastName] [varchar](150) NULL, [LastNameWild] [varchar](150) NULL").Replace("{$tmp_columns}", "FirstName,LastName,LastNameWild").Replace("{$tmp_insert_values}", "'{$CURR_PCP_FIRST_NAME}%','{$CURR_PCP_LAST_NAME}','{$CURR_PCP_LAST_NAME}%'").Replace("{$data_source_case}", "CASE WHEN (p.FirstName LIKE mp.FirstName AND p.LastName = mp.LastName ) THEN 'UHN_FN%LnBySpec' ELSE CASE WHEN (p.FirstName LIKE mp.FirstName AND p.LastName LIKE mp.LastNameWild ) THEN 'UHN_FN%Ln%BySpec' ELSE 'UHN_Ln%BySpec' END END").Replace("{$notes}", "Found in NDB.PROVIDER by PrimSpec AND PROV_LST_NM%").Replace("{$extra_cols}", ", NULL as taxid, NULL as DEANbr, p.MiddleName, NULL as SBSCR_MEDCD_RCIP_NBR, NULL as CURR_PCP_ID, p.ProvDegree, p.PrimSpec").Replace("{$joins}", "inner join #MissingProvidersTmp as mp on p.LastName LIKE mp.LastNameWild").Replace("{$PrSpecs}", "WHERE primSpec in (" + IR_DataScrubber.strPrimarySpecs + ") ");
                //COLLECT BATCH UPDATE FOR SPEC,PROV_FST_NM%,PROV_LST_NM
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "a.PROV_LST_NM = b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SPECFNLN").Replace("{$tmp_index_columns}", "PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "a.provider_name_cln_ln=b.PROV_LST_NM AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR SPEC,PROV_FST_NM%,PROV_LST_NM%
                sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.PROV_LST_NM = a.PROV_LST_NM, a.PROV_FST_NM = a.PROV_FST_NM").Replace("{$insert_output_columns}", ",INSERTED.PROV_LST_NM, INSERTED.PROV_FST_NM").Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(b.PROV_LST_NM_CLN, a.PROV_LST_NM_CLN), 0) > 0  AND ISNULL(CHARINDEX(b.PROV_FST_NM_CLN, a.PROV_FST_NM_CLN), 0) > 0 ")
                    + "UPDATE #Provider_Cache_Updated SET PROV_FST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_FST_NM, '[^a-zA-Z]'),1," + intSubstrFNLength + "),PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SPECFNLNW").Replace("{$tmp_index_columns}", "PROV_FST_NM,PROV_LST_NM")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_ln, '[^a-zA-Z]')), 0) > 0 AND ISNULL(CHARINDEX(b.PROV_FST_NM, dbo.fnRegExeReplaceCSG(a.provider_name_cln_fn, '[^a-zA-Z]')), 0) > 0")
                   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //COLLECT BATCH UPDATE FOR SPEC,PROV_FST_NM%,PROV_LST_NM%
                //sbBatchUpdatesContainer.Append(IR_DataScrubber.str_ILUCA_ACO_Provider_MissingCache_Update_Statement.Replace("{$update_output_columns}", ", a.PROV_LST_NM = a.PROV_LST_NM").Replace("{$insert_output_columns}", ",INSERTED.PROV_LST_NM, NULL as PROV_FST_NM").Replace("{$data_source}", "UHN_Ln%BySpec").Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(dbo.fnRegExeReplaceCSG(a.PROV_LST_NM, '[^a-zA-Z]'), b.PROV_LST_NM_CLN), 0) > 0")
                //    + "UPDATE #Provider_Cache_Updated SET PROV_LST_NM = SUBSTRING(dbo.fnRegExeReplaceCSG(PROV_LST_NM, '[^a-zA-Z]'),1," + intSubstrLNLength + ");"
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_CreateIndex_Statement.Replace("{$tmp_index_name}", "SPECLNW").Replace("{$tmp_index_columns}", "PROV_LST_NM")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_MissingACO_Update_Statement.Replace("{$tmp_missing_join}", "ISNULL(CHARINDEX(b.PROV_LST_NM, dbo.fnRegExeReplaceCSG(a.CURR_PCP_LAST_NAME, '[^a-zA-Z]')), 0) > 0")
                //   + IR_DataScrubber.str_ILUCA_ACO_Provider_Missing_Cleanup_Statement);
                //ADD UPDATES FROM ABOVE TO MAIN PROVIDER BATCH {$main_updates_here} = sbBatchUpdatesContainer.ToString()
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Provider_Cache_Batch_Update.Replace("{$main_updates_here}", sbBatchUpdatesContainer.ToString()).Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$tmp_columns}", ",PROV_LST_NM, PROV_LST_NM_CLN, PROV_FST_NM_CLN").Replace("{$tmp_create_upt_columns}", ",[PROV_LST_NM] [varchar](150) NULL, [PROV_FST_NM] [varchar](50) NULL").Replace("{$tmp_create_miss_columns}", ",[PROV_LST_NM] [varchar](150) NULL, [PROV_LST_NM_CLN] [varchar](150) NULL , [PROV_FST_NM_CLN] [varchar](50) NULL").Replace("{$tmp_insert_columns}", ",provider_name_cln_ln AS PROV_LST_NM, SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_ln, '[^a-zA-Z]'),1," + intSubstrLNLength + ")  as PROV_LST_NM_CLN, SUBSTRING(dbo.fnRegExeReplaceCSG(provider_name_cln_fn, '[^a-zA-Z]'),1," + intSubstrFNLength + ") as PROV_FST_NM_CLN").Replace("{$data_source}", "'UHN_FN%LnBySpec','UHN_FN%Ln%BySpec','UHN_Ln%BySpec'")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}", "provider_name_cln_ln IS NOT NULL");
                sbBatchUpdatesContainer.Remove(0, sbBatchUpdatesContainer.Length);
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO.MPIN's by Specs in UNH";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUHN_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Provider_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);

                //Retrieving ACO_MR.MPIN CACHE
                //Retrieving ACO_MR.MPIN CACHE
                //Retrieving ACO_MR.MPIN CACHE 
                Console.WriteLine("Step 3.11 ACO_MR.mpin main cleanup...");
                IR_DataScrubber.processQuerySet(dtQueries, strFilterTagGeneric: "<filter_{$x}_loop>");
                dtQueries.Clear();

                //VERY MANUAL CLEANUP IL_UCA.ACO_MR.MPIN :(
                DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "IF NOT EXISTS(SELECT * FROM ACO_Provider_Cache WHERE provider_id = '0212666776' AND MPIN = 71662) BEGIN INSERT INTO ACO_Provider_Cache (provider_id, provider_name, MPIN, Notes, LOB, data_source) VALUES('0212666776', 'SMITH, JENNIFER REED', 71662, 'Manual Data Cleanup', 'MR', 'Operations'); UPDATE[dbo].[ACO_MR] SET[MPIN] = 71662 WHERE[provider_id] = '0212666776'; END ");
                DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "IF NOT EXISTS(SELECT * FROM ACO_Provider_Cache WHERE provider_id = '0212667412' AND MPIN = 6602504) BEGIN INSERT INTO ACO_Provider_Cache (provider_id, provider_name, MPIN, Notes, LOB, data_source) VALUES('0212667412', 'GONZALEZ, JENNIFER', 6602504, 'Manual Data Cleanup', 'MR', 'Operations'); UPDATE[dbo].[ACO_MR] SET[MPIN] = 6602504 WHERE[provider_id] = '0212667412'; END ");



                ////////////////////////////////////////////////////////////////////////////////////////////START MR indv_sys_id CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////
                ////////////////////////////////////////////////////////////////////////////////////////////START MR indv_sys_id CLEANUP///////////////////////////////////////////////////////////////////////////////////////////////

                //STEP 1.1 HISTORICAL IL_UCA.ACO_MR VS IL_UCA.ACO_Member_Cache BY hicnbr
                Console.WriteLine("Step 3.12 indv_sys_id clean: Historical hicnbr");
                strSQL = "update a set a.indv_sys_id = b.indv_sys_id from dbo.ACO_MR as a inner join ACO_Member_Cache as b on a.hicnbr = b.hicnbr where a.indv_sys_id is null";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");

                //STEP 1.1 HISTORICAL IL_UCA.ACO_MR VS IL_UCA.ACO_Member_Cache BY member_ID
                Console.WriteLine("Step 3.13 indv_sys_id clean: Historical member_ID");
                strSQL = "update a set a.indv_sys_id = b.indv_sys_id from dbo.ACO_MR as a inner join ACO_Member_Cache as b on a.member_ID = b.member_ID where a.indv_sys_id is null";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("--" + intResultCnt + " rows updated");


                //UGAP hicnbr/BD ACO_MR.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select distinct [hicnbr] as HICNBR, CONVERT(char(10), BTH_DT,126) as BTH_DT FROM [dbo].[ACO_MR] WHERE [indv_sys_id] IS NULL AND [hicnbr] IS NOT NULL AND BTH_DT IS NOT NULL;";
                ////USE REUSEABLE SEARCH SCRIPT FOR ANY GIVEN DATASOURCE
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_HICNBD").Replace("{$vtt_create_columns}", "HICN VARCHAR(12), BTH_DT DATE").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(HICN,BTH_DT ) VALUES ('{$HICNBR}','{$BTH_DT}'); </filter_1_loop>").Replace("{$vtt_columns}", "HICN,BTH_DT").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by HICN AND BD").Replace("{$final_filter}", "inner join MissingMembersTmp as mm on mm.HICN=m.HICN AND mm.BTH_DT=m.BTH_DT").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_HICNBD'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "INDV_SYS_ID [int] NULL, hicnbr [varchar](50) NULL, BTH_DT date").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.hicnbr = a.hicnbr, a.BTH_DT = a.BTH_DT OUTPUT INSERTED.INDV_SYS_ID, INSERTED.hicnbr, INSERTED.BTH_DT").Replace("{$table_to_update}", "inner join dbo.ACO_MR as b on a.BTH_DT = b.BTH_DT AND a.hicnbr = b.hicnbr").Replace("{$table_to_update2}", "dbo.ACO_MR as b inner join #Member_Cache_Updated as a on a.BTH_DT = b.BTH_DT AND a.hicnbr = b.hicnbr")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO_MR.INDV_SYS_ID's by HICNBR/BD in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP FNLNBD ACO_MR.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select DISTINCT SUBSTRING(MEMFNAME, 1, 2) as MEMFNAME_SUB, SUBSTRING(MEMLNAME, 1, 3) as MEMLNAME_SUB,CONVERT(char(10), BTH_DT,126) as BTH_DT FROM dbo.ACO_MR WHERE indv_sys_id IS NULL AND MEMFNAME IS NOT NULL and MEMLNAME IS NOT NULL AND BTH_DT IS NOT NULL";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_FNSub%LNSub%BD").Replace("{$vtt_create_columns}", "MBR_FST_NM VARCHAR(10), MBR_LST_NM VARCHAR(10), BTH_DT DATE").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(MBR_FST_NM,MBR_LST_NM,BTH_DT ) VALUES ('{$MEMFNAME_SUB}%','{$MEMLNAME_SUB}%','{$BTH_DT}'); </filter_1_loop>").Replace("{$vtt_columns}", "MBR_FST_NM,MBR_LST_NM,BTH_DT").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by FNSub%LNSub%BD").Replace("{$final_filter}", "inner join MissingMembersTmp as mm on m.MBR_FST_NM LIKE mm.MBR_FST_NM AND  m.MBR_LST_NM LIKE mm.MBR_LST_NM AND m.BTH_DT=mm.BTH_DT").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_FNSub%LNSub%BD'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "[INDV_SYS_ID] [int] NULL , [MBR_FST_NM] [varchar](50) NULL, [MBR_LST_NM] [varchar](150) NULL, [BTH_DT] [date] NULL").Replace("{$update_insert}", ",a.INDV_SYS_ID = a.INDV_SYS_ID, a.MBR_FST_NM = a.MBR_FST_NM, a.MBR_LST_NM = a.MBR_LST_NM, a.BTH_DT = a.BTH_DT OUTPUT INSERTED.INDV_SYS_ID, INSERTED.MBR_FST_NM, INSERTED.MBR_LST_NM, INSERTED.BTH_DT").Replace("{$table_to_update}", "inner join dbo.ACO_MR as b on a.BTH_DT = b.BTH_DT AND ISNULL(CHARINDEX(dbo.fnRegExeReplaceCSG(b.MEMFNAME, '[^a-zA-Z]'), a.MBR_FST_NM), 0) > 0 AND ISNULL(CHARINDEX(dbo.fnRegExeReplaceCSG(b.MEMLNAME, '[^a-zA-Z]'), a.MBR_LST_NM), 0) > 0").Replace("{$table_to_update2}", "dbo.ACO_MR as b inner join #Member_Cache_Updated as a on a.BTH_DT = b.BTH_DT AND ISNULL(CHARINDEX(dbo.fnRegExeReplaceCSG(b.MEMFNAME, '[^a-zA-Z]'), a.MBR_FST_NM), 0) > 0 AND ISNULL(CHARINDEX(dbo.fnRegExeReplaceCSG(b.MEMLNAME, '[^a-zA-Z]'), a.MBR_LST_NM), 0) > 0")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO_MR.INDV_SYS_ID's by FNLNBD in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP hicnbr ACO_MR.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select distinct [hicnbr] as HICNBR FROM [dbo].[ACO_MR] WHERE [indv_sys_id] IS NULL AND [hicnbr] IS NOT NULL;";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_HICN").Replace("{$vtt_create_columns}", "HICN VARCHAR(12)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(HICN) VALUES ('{$HICNBR}'); </filter_1_loop>").Replace("{$vtt_columns}", "HICN").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by HICN").Replace("{$final_filter}", "WHERE m.HICN IN (SELECT HICN FROM MissingMembersTmp)").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_HICN'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "INDV_SYS_ID [int] NULL, hicnbr [varchar](50) NULL").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.hicnbr = a.hicnbr OUTPUT INSERTED.INDV_SYS_ID, INSERTED.hicnbr").Replace("{$table_to_update}", "inner join dbo.ACO_MR as b on a.hicnbr = b.hicnbr").Replace("{$table_to_update2}", "dbo.ACO_MR as b inner join #Member_Cache_Updated as a on a.hicnbr = b.hicnbr")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO_MR.INDV_SYS_ID's by HICNBR in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP member_ID ACO_MR.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select distinct [member_ID] as member_ID FROM [dbo].[ACO_MR] WHERE [indv_sys_id] IS NULL AND [member_ID] IS NOT NULL;";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_MBRSSN").Replace("{$vtt_create_columns}", "mbr_ssn CHAR(9)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(mbr_ssn) VALUES ('{$member_ID}'); </filter_1_loop>").Replace("{$vtt_columns}", "mbr_ssn").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by MBRSSN").Replace("{$final_filter}", "WHERE m.mbr_ssn IN (SELECT mbr_ssn FROM MissingMembersTmp)").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_MBRSSN'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "INDV_SYS_ID [int] NULL, member_ID [varchar](20) NULL").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.member_ID = a.member_ID OUTPUT INSERTED.INDV_SYS_ID, INSERTED.member_ID").Replace("{$table_to_update}", "inner join dbo.ACO_MR as b on a.member_ID = b.member_ID").Replace("{$table_to_update2}", "dbo.ACO_MR as b inner join #Member_Cache_Updated as a on a.member_ID = b.member_ID")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO_MR.INDV_SYS_ID's by MBRSSN in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //UGAP member_ID/hicnbr ACO_MR.indv_sys_id CHECK RETURNED COLUMN NAMES MUST MATCH SEACH COLUMNS IN strSearchSQL
                strMissingSQL = "select distinct  [hicnbr] as HICNBR, [member_ID] as member_ID FROM [dbo].[ACO_MR] WHERE [indv_sys_id] IS NULL AND [member_ID] IS NOT NULL AND [hicnbr] IS NOT NULL;";
                strSearchSQL = IR_DataScrubber.strSQL_UGAP_GENERIC_MEMBER_SEARCH.Replace("{$data_source}", "UGAP_HICN_MBRSSN").Replace("{$vtt_create_columns}", "HICN VARCHAR(12), mbr_ssn CHAR(9)").Replace("{$vtt_insert}", "<filter_1_loop>INSERT INTO MissingMembersTmp(HICN,mbr_ssn ) VALUES ('{$HICNBR}','{$member_ID}'); </filter_1_loop>").Replace("{$vtt_columns}", "HICN,mbr_ssn").Replace("{$notes}", "Found in UHCDM001.HP_member/CS_ENROLLMENT by HICN AND MBRSSN").Replace("{$final_filter}", "inner join MissingMembersTmp as mm on m.HICN = mm.HICN AND  m.mbr_ssn= mm.mbr_ssn").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB);
                strBatchUpdateSQL = IR_DataScrubber.strTSQL_ErrorHandlingTemplate.Replace("{$tsql_here}", IR_DataScrubber.str_ILUCA_ACO_Member_Cache_Batch_Update.Replace("{$data_source}", "'UGAP_HICN_MBRSSN'").Replace("{$LOB}", IR_DataScrubber.strCurrentLOB).Replace("{$mem_cache_columns}", "INDV_SYS_ID [int] NULL, hicnbr [varchar](50) NULL,  member_ID [varchar](20) NULL").Replace("{$update_insert}", ", a.INDV_SYS_ID = a.INDV_SYS_ID, a.hicnbr = a.hicnbr , a.member_ID = a.member_ID OUTPUT INSERTED.INDV_SYS_ID, INSERTED.hicnbr, INSERTED.member_ID").Replace("{$table_to_update}", "inner join dbo.ACO_MR as b on a.hicnbr = b.hicnbr AND a.member_ID = b.member_ID").Replace("{$table_to_update2}", "    dbo.ACO_MR as b inner join #Member_Cache_Updated as a on a.hicnbr = b.hicnbr AND a.member_ID = b.member_ID")).Replace("{$main_table_name}", IR_DataScrubber.strCurrentMainTable).Replace("{$missing_table_filters}","");
                //ADD ABOVE QUERIES AS A NEW ROW TO BE PASSED TO IR_DataScrubber.processQuerySet(DataTable dtQueries ...
                drQuery = dtQueries.NewRow();
                drQuery["MissingSearchDesc"] = "ACO_MR.INDV_SYS_ID's by HICNBR AND MBRSSN in UGAP";
                drQuery["MissingSQL"] = strMissingSQL;
                drQuery["MissingConnectionString"] = strILUCA_ConnectionString;
                drQuery["SearchSQL"] = strSearchSQL;
                drQuery["SearchConnectionString"] = strUGAP_ConnectionString;
                drQuery["SearchLimit"] = intFilterMax;
                drQuery["CachedTableName"] = "ACO_Member_Cache";
                drQuery["CachedInsertConnectionString"] = strILUCA_ConnectionString;
                drQuery["BatchUpdateSQL"] = strBatchUpdateSQL;
                drQuery["BatchUpdateConnectionString"] = strILUCA_ConnectionString;
                drQuery["Exclude"] = false;
                dtQueries.Rows.Add(drQuery);


                //Retrieving ACO_MR.INDV_SYS_ID CACHE 
                //Retrieving ACO_MR.INDV_SYS_ID CACHE 
                //Retrieving ACO_MR.INDV_SYS_ID CACHE 
                Console.WriteLine("Step 3.14 ACO_MR.INDV_SYS_ID main cleanup...");
                IR_DataScrubber.processQuerySet(dtQueries, strFilterTagGeneric: "<filter_{$x}_loop>");
                dtQueries.Clear();

                //VERY MANUAL CLEANUP IL_UCA.ACO_MR.indv_sys_id :(
                DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "IF NOT EXISTS(SELECT * FROM ACO_Member_Cache WHERE hicnbr = '7YC0QE3QF04' AND indv_sys_id = 200219289) BEGIN INSERT INTO ACO_Member_Cache ([hicnbr] ,[MBR_FST_NM] ,[MBR_LST_NM] ,[BTH_DT] ,[member_id] ,[indv_sys_id] ,[Notes], data_source) VALUES('7YC0QE3QF04', 'HEE HONG','HUM', '1936-01-19',821556672,200219289, 'Manual Data Cleanup',  'Operations'); UPDATE[dbo].[ACO_MR] SET [indv_sys_id] = 200219289 WHERE [hicnbr] = '7YC0QE3QF04'; END ");
                DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, "IF NOT EXISTS(SELECT * FROM ACO_Member_Cache WHERE hicnbr = '4WG3JY2JD86' AND indv_sys_id = 497963417) BEGIN INSERT INTO ACO_Member_Cache ([hicnbr] ,[MBR_FST_NM] ,[MBR_LST_NM] ,[BTH_DT] ,[member_id] ,[indv_sys_id] ,[Notes], data_source) VALUES('4WG3JY2JD86', 'DEBORAH','MANSFIELD', '1953-10-30',935361155,497963417, 'Manual Data Cleanup', 'Operations'); UPDATE[dbo].[ACO_MR] SET[indv_sys_id] = 497963417 WHERE [hicnbr] = '4WG3JY2JD86'; END");

            }//MR CLEAN END

            if (blRefreshMRTable && blUpdateProcessingSummary)
            {
                //GET FINAL SUMMARY FROM ACO_COMM
                Console.WriteLine("Step 3.15 Gather Collecting final 'MR' counts for dbo.ACO_Processing_Summary");
                strSQL = "UPDATE dbo.ACO_Processing_Summary SET found_mpin_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT provider_name FROM ACO_MR WHERE MPIN IS NULL) as tmp), found_indv_sys_id_cnt = (SELECT COUNT(*) FROM (SELECT DISTINCT MEMFNAME,MEMLNAME,BTH_DT FROM ACO_MR WHERE indv_sys_id IS NULL) as tmp) WHERE LOB = 'MR' AND processing_date = '" + strDateStamp + "';";
                intResultCnt = DBConnection32.ExecuteMSSQL(strILUCA_ConnectionString, strSQL);
                Console.WriteLine("Final dbo.ACO_Processing_Summary Updated");
            }

            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////IL_UCA.ACO_MR END////////////////////////////////////////////////////////////////////////////


            ////DONE!!!
            ////DONE!!!
            ////DONE!!!
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("DATA LOADER COMPLETED SUCCESSFULLY");
            Console.ReadKey();

        }

        // ALL PURSPOSE MS SQL BULK DATA IMPORTER
        private static void SQLServerBulkImport(string strSourcenConnectionString, string strDestinationConnectionString, string strSQL, string strTableName)
        {

            // GET THE SOURCE DATA
            using (SqlConnection sourceConnection =  new SqlConnection(strSourcenConnectionString))
            {
                SqlCommand myCommand =
                    new SqlCommand(strSQL, sourceConnection);
                sourceConnection.Open();
                SqlDataReader reader = myCommand.ExecuteReader();

                // OPEN THE DESTINATION DATA
                using (SqlConnection destinationConnection =
                            new SqlConnection(strDestinationConnectionString))
                {
                    // OPEN THE CONNECTION
                    destinationConnection.Open();

                    using (SqlBulkCopy bulkCopy =
                    new SqlBulkCopy(destinationConnection.ConnectionString))
                    {
                        bulkCopy.BatchSize = 500;
                        bulkCopy.NotifyAfter = 1;
                        bulkCopy.SqlRowsCopied += new SqlRowsCopiedEventHandler(OnSqlRowsCopied);
                        bulkCopy.DestinationTableName = strTableName;
                        bulkCopy.WriteToServer(reader);
                    }
                }
                reader.Close();
            }
        }


        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write("\r" + strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }





    }


}
