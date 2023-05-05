using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
using System.Text;
using WCDocumentGenerator;
using Microsoft.Office.Interop;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections;

namespace tin_group_reporting
{
    class tin_group_reporting
    {

        static DataTable getLib()
        {
            DataTable dtLib = new DataTable();
            dtLib.Columns.Add("Alias", typeof(string));
            dtLib.Columns.Add("Path", typeof(string));

            DataRow drLib = dtLib.NewRow();
            drLib["Alias"] = "Ph34";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph34";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "CARD";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Card/Cath/Data_Spec_2019";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "SF";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph34/SpineFusion";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "postopms";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/PBC/May2019/postopms";
            dtLib.Rows.Add(drLib);

            drLib = dtLib.NewRow();
            drLib["Alias"] = "tymp";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph34/ENT/Tympanostomy";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "sin";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Px/Sinusitis/2019_Q2/Output";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "RX";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/RX_Scorecard/Spec/Data_2019";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "SOS";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/SOS/Data/Spec_2019";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "astsur";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/AsstSurg/Data/Spec_2019";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "OONAS";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph34/OONAS/Data/Spec_2019";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "slsd";
            drLib["Path"] = "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph34/SleepStd";
            dtLib.Rows.Add(drLib);


            drLib = dtLib.NewRow();
            drLib["Alias"] = "onc";
            drLib["Path"] = "/optum/uhs/01datafs/phi/onc/opchemo/rpt";
            dtLib.Rows.Add(drLib);


            return dtLib;
        }



        static void Main(string[] args)
        {

            Start:


            string strSQL = null;
            int intProfileCnt = 1;
            int intTotalCnt = 0;


            try
            {


                //killProcesses();

                //Decimal.Parse("Test");
                Console.WriteLine("Wiser Choices Profiles Generator");
                //Console.WriteLine("Gathering Configuration Values...");


                //PLACE APP.CONFIG FILE DATA INTO VARIABLES START
                string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
                string strNDBConnectionString = ConfigurationManager.AppSettings["UHN_Database"];

                //strSpecialtyId = null; //ALL BUT 4
                //strSpecialtyId = -99999;  //ALL SPECIALTIES
                //strSpecialtyId = 2; //SPECIFIC SPECIALTY


                IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
                IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
                IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
                IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
                IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
                IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
                IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];


                string strWordTemplate = ConfigurationManager.AppSettings["Word_Template"];
                string strInputPath = ConfigurationManager.AppSettings["TIN_Reporting_Input_Path"];
                string strOutputPath = ConfigurationManager.AppSettings["TIN_Reporting_Output_Path"];



                //PLACE CONFIG FILE DATA INTO VARIABLES END

                string strMonthYear = DateTime.Now.Month + "_" + DateTime.Now.Year;

                bool blHasWord = true;
                bool blHasExcel = true;

                bool blVisibleExcel = false;
                bool blVisibleWord = false;
                //START EXCEL APP
                if (blHasExcel)
                {
                    MSExcel.populateExcelParameters(blVisibleExcel, false, "", "");
                    MSExcel.openExcelApp();
                }

                //START WORD APP
                if (blHasWord)
                {
                    MSWord.populateWordParameters(blVisibleWord, false, "", "");
                    MSWord.openWordApp();
                }




                Hashtable htParam = new Hashtable();
                string strSheetname = null;


                ArrayList alSection = new ArrayList();


                DataTable dtFinal = null;

                Int16 intConsoleCnt = 0;
                Console.WriteLine(intConsoleCnt++ + " - connecting to SAS server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


                string strTinList = null;



                object obj_tincnt = null;
                object obj_contr = null;
                object obj_pcr = null;
                object obj_outl = null;
                object obj_utmcnt = null;
                object obj_pxmcnt = null;
                string str_t3bmeas = null;
                string str_t3gmeas = null;
                string str_facilityName = null;

                StringBuilder sbDescConcat = new StringBuilder();
                StringBuilder sbSQL = new StringBuilder();


     
                Int16 intlimitCnt = 0;



                string[] files = Directory.GetFiles(strInputPath, "*.xlsx", SearchOption.TopDirectoryOnly);
                foreach (string sFile in files)
                {
                    MSExcel.strExcelTemplate = sFile;
                    MSWord.strWordTemplate = strWordTemplate;
                    //OPEN EXCEL WB
                    Console.WriteLine(intConsoleCnt++ + " - opening excel...");
                    MSExcel.openExcelWorkBook();

                    //OPEN WORD DOC
                    Console.WriteLine(intConsoleCnt++ + " - opening word...");
                    MSWord.openWordDocument();


                    strSheetname = "Automation_Changes";
                    str_facilityName = MSExcel.GetCellValue(strSheetname, "B6");
                    strTinList = MSExcel.GetCellValue(strSheetname, "B7");
                    MSExcel.addFocusToCell(strSheetname, "A1");

                    //List of TINs SHEET START
                    //List of TINs SHEET START
                    //List of TINs SHEET START
                    strSheetname = "List of TINs";

                    //CREATE SAS TMP TABLE taxid
                    Console.WriteLine(intConsoleCnt++ + " - create table taxid");
                    strSQL = "proc sql;create table taxid as select distinct taxid,max(Corpownername) as Practice_name from UHN.taxid where taxid in(" + strTinList + ") group by taxid;quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);


                    //CREATE SAS TMP TABLE mpins_pcp
                    Console.WriteLine(intConsoleCnt++ + " - create table mpins_pcp");
                    strSQL = "proc sql;create table mpins_pcp as select t.taxid, d.MPIN, d.PTIGroupName ,FirstName, LastName, d.RGN_NM, d.MKT_RLLP_NM,State, d.NDB_Specialty ,case when p.mpin is not null then 1 end as T1 ,case when c.mpin is not null then 1 end as ACO from taxid as t inner join Ph14.UHN_JAN22_DEMOG as d on t.taxid = d.taxid left join(select MPIN, T1 from Ph34.UHPD_MAY6 where T1= 1) as p on p.mpin = d.mpin left join Ph34.ACO as c on c.mpin = d.mpin;quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);


                    //CREATE SAS TMP TABLE mpins_spec
                    Console.WriteLine(intConsoleCnt++ + " - create table mpins_spec");
                    strSQL = "proc sql;create table mpins_spec as select t.taxid, d.MPIN, d.PTIGroupName ,FirstName, LastName, d.RGN_NM, d.MKT_RLLP_NM,State, d.NDB_Specialty ,case when p.mpin is not null then 1 end as T1 ,case when c.mpin is not null then 1 end as ACO from taxid as t inner join Ph34.UHN_MAY6_DEMOG as d on t.taxid = d.taxid left join(select MPIN, T1 from Ph34.UHPD_MAY6 where T1= 1) as p on p.mpin = d.mpin left join Ph34.ACO as c on c.mpin = d.mpin;quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);


                    //CREATE SAS TMP TABLE tin_cnt
                    Console.WriteLine(intConsoleCnt++ + " - create table tin_cnt");
                    strSQL = "proc sql;create table tin_cnt as select t.taxid,t.practice_name,c.mpins as PCP_mpins,s.mpins as SP_mpins from taxid as t left join (select taxid,count(distinct MPIN) as mpins ,sum(t1) as T1_mpins,sum(ACO) as ACO_mpins from mpins_pcp group by taxid) as c on t.taxid=c.taxid left join (select taxid,count(distinct MPIN) as mpins ,sum(t1) as T1_mpins,sum(ACO) as ACO_mpins from mpins_spec group by taxid) as s on t.taxid=s.taxid;quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);



                    //GENERATE FINAL OUTPUT FOR List of TINs SHEET
                    Console.WriteLine(intConsoleCnt++ + " - populating sheet:'List of TINs'");
                    strSQL = "select taxid,practice_name,PCP_mpins,SP_mpins from tin_cnt order by practice_name;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel)
                    {
                        if (dtFinal.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dtFinal, strSheetname, 2, 'A', true);
                        }
                    }
                    MSExcel.addValueToCell(strSheetname, "C" + (dtFinal.Rows.Count + 2), "=SUM(C2:C"+ (dtFinal.Rows.Count + 1) + ")");
                    MSExcel.addValueToCell(strSheetname, "D" + (dtFinal.Rows.Count + 2), "=SUM(D2:D" + (dtFinal.Rows.Count + 1) + ")");
                    MSExcel.addFocusToCell(strSheetname, "A1");
                    //List of TINs SHEET END
                    //List of TINs SHEET END
                    //List of TINs SHEET END



                    //PCP Summary SHEET START
                    //PCP Summary SHEET START
                    //PCP Summary SHEET START
                    strSheetname = "PCP Summary";



                    //CREATE SAS TMP TABLE pcp_outp
                    Console.WriteLine(intConsoleCnt++ + " - create table pcp_outp");
                    strSQL = "proc sql;create table pcp_outp as select count(*) as mpins, sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'contr' as type,1 as ord from mpins_pcp UNION select count(*) as mpins, sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'pcr' as type,2 as ord from (select distinct MPIN from ph14.Profile UNION select distinct MPIN from ph14.Profile_px) as p inner join mpins_pcp as h on p.mpin=h.mpin UNION select count(*) as mpins, sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'utl' as type,4 as ord from (select distinct p.MPIN,T1,ACO from ph14.Profile as p inner join mpins_pcp as h on p.mpin=h.mpin where measure_id not in(14,15)) UNION select count(*) as mpins, sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'px' as type,5 as ord from (select distinct p.MPIN,T1,ACO from ph14.Profile_px as p inner join mpins_pcp as h on p.mpin=h.mpin where measure_id in(38,51,55)) UNION select count(*) as mpins, sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'outl' as type,3 as ord from ph14.Outliers as p inner join mpins_pcp as h on p.mpin=h.mpin; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);

                    //CREATE SAS TMP TABLE spec_sum_cnt
                    Console.WriteLine(intConsoleCnt++ + " - populating placeholders");
                    strSQL = "SELECT mpins ,t1mpins ,nt1mpins ,acompins ,nacompins FROM pcp_outp order by ord";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 4, 'D', false, new char[] { 'D', 'F', 'H', 'J', 'L' });
                    }




                    //CREATE SAS TMP TABLE ubsmr_pcp
                    Console.WriteLine(intConsoleCnt++ + " - create table ubsmr_pcp");
                    strSQL = "proc sql;create table ubsmr_pcp as select distinct p.measure_id,Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, count(distinct case when ACO=1 then p.MPIN end) as aco_mp, calculated all_mp-calculated aco_mp as naco_mp, sum(outl_idx) as bo, calculated bo/calculated all_mp as bo_pct, sum(case when T1=1 then outl_idx else 0 end) as bo_t1, calculated bo_t1/calculated t1_mp as bo_t1_pct, sum(case when T1<>1 then outl_idx else 0 end) as bo_nt1, calculated bo_nt1/calculated nt1_mp as bo_nt1_pct, sum(case when ACO=1 then outl_idx else 0 end) as bo_aco, calculated bo_aco/calculated aco_mp as bo_aco_pct, sum(case when ACO<>1 then outl_idx else 0 end) as bo_naco, calculated bo_nt1/calculated nt1_mp as bo_naco_pct from ph14.Profile as p inner join mpins_pcp as h on p.mpin=h.mpin where measure_id not in(14,15) group by p.measure_id,Measure_desc; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D9 TO M21
                    Console.WriteLine(intConsoleCnt++ + " - populating D9 TO M21");
                    strSQL = "SELECT bo,bo_pct, bo_t1, bo_t1_pct, bo_nt1, bo_nt1_pct, bo_aco,bo_aco_pct, bo_naco, bo_naco_pct FROM ubsmr_pcp order by sort_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 9, 'D', false);
                    }



                    //CREATE SAS TMP TABLE pbsmr_pcp 
                    Console.WriteLine(intConsoleCnt++ + " - create table pbsmr_pcp");
                    strSQL = "proc sql;create table pbsmr_pcp as select m.measure_id,m.Measure_desc,sort_id, all_mp, t1_mp, nt1_mp,aco_mp,naco_mp, bo,bo_pct, bo_t1,bo_t1_pct,bo_nt1,bo_nt1_pct,bo_aco,bo_aco_pct, bo_naco,bo_naco_pct from (select measure_id,Measure_desc from IL_UCA.PBP_dim_Measures where measure_id in(38,51,55)) as m left join (select distinct p.measure_id,p.Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, count(distinct case when ACO=1 then p.MPIN end) as aco_mp, calculated all_mp-calculated aco_mp as naco_mp, sum(outl_idx) as bo, calculated bo/calculated all_mp as bo_pct, sum(case when T1=1 then outl_idx else 0 end) as bo_t1, calculated bo_t1/calculated t1_mp as bo_t1_pct, sum(case when T1<>1 then outl_idx else 0 end) as bo_nt1, calculated bo_nt1/calculated nt1_mp as bo_nt1_pct, sum(case when ACO=1 then outl_idx else 0 end) as bo_aco, calculated bo_aco/calculated aco_mp as bo_aco_pct, sum(case when ACO<>1 then outl_idx else 0 end) as bo_naco, calculated bo_nt1/calculated nt1_mp as bo_naco_pct from ph14.Profile_px as p inner join mpins_pcp as h on p.mpin=h.mpin group by p.measure_id,p.Measure_desc) as t on t.measure_id=m.measure_id; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D22 TO M24
                    Console.WriteLine(intConsoleCnt++ + " - populating D22 TO M24");
                    strSQL = "SELECT  bo,bo_pct, bo_t1, bo_t1_pct, bo_nt1, bo_nt1_pct,bo_aco, bo_aco_pct, bo_naco,bo_naco_pct FROM pbsmr_pcp order by measure_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 22, 'D', false);
                    }


                    //CREATE SAS TMP TABLE ugsmr_pcp 
                    Console.WriteLine(intConsoleCnt++ + " - create table ugsmr_pcp");
                    strSQL = "proc sql;create table ugsmr_pcp as select distinct p.measure_id,Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, count(distinct case when ACO=1 then p.MPIN end) as aco_mp, calculated all_mp-calculated aco_mp as naco_mp, sum(outl_idx_g) as go, calculated go/calculated all_mp as go_pct, sum(case when T1=1 then outl_idx_g else 0 end) as go_t1, calculated go_t1/calculated t1_mp as go_t1_pct, sum(case when T1<>1 then outl_idx_g else 0 end) as go_nt1, calculated go_nt1/calculated nt1_mp as go_nt1_pct, sum(case when ACO=1 then outl_idx_g else 0 end) as go_aco, calculated go_aco/calculated aco_mp as go_aco_pct, sum(case when ACO<>1 then outl_idx_g else 0 end) as go_naco, calculated go_nt1/calculated nt1_mp as go_naco_pct from ph14.Profile as p inner join mpins_pcp as h on p.mpin=h.mpin where measure_id not in(14,15) group by p.measure_id,Measure_desc; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D25 TO M37
                    Console.WriteLine(intConsoleCnt++ + " - populating D25 TO M37");
                    strSQL = "SELECT go, go_pct,go_t1, go_t1_pct, go_nt1, go_nt1_pct, go_aco,go_aco_pct,go_naco, go_naco_pct  FROM ugsmr_pcp order by sort_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 25, 'D', false);
                    }


                    //CREATE SAS TMP TABLE pgsmr_pcp
                    Console.WriteLine(intConsoleCnt++ + " - create table pgsmr_pcp");
                    strSQL = "proc sql;create table pgsmr_pcp  as select m.measure_id,m.Measure_desc,sort_id, all_mp, t1_mp, nt1_mp,aco_mp,naco_mp, go,go_pct, go_t1,go_t1_pct,go_nt1,go_nt1_pct,go_aco,go_aco_pct, go_naco,go_naco_pct from (select measure_id,Measure_desc from IL_UCA.PBP_dim_Measures where measure_id in(38,51,55)) as m left join (select distinct p.measure_id,p.Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, count(distinct case when ACO=1 then p.MPIN end) as aco_mp, calculated all_mp-calculated aco_mp as naco_mp, sum(outl_idx_g) as go, calculated go/calculated all_mp as go_pct, sum(case when T1=1 then outl_idx_g else 0 end) as go_t1, calculated go_t1/calculated t1_mp as go_t1_pct, sum(case when T1<>1 then outl_idx_g else 0 end) as go_nt1, calculated go_nt1/calculated nt1_mp as go_nt1_pct, sum(case when ACO=1 then outl_idx_g else 0 end) as go_aco, calculated go_aco/calculated aco_mp as go_aco_pct, sum(case when ACO<>1 then outl_idx_g else 0 end) as go_naco, calculated go_nt1/calculated nt1_mp as go_naco_pct from ph14.Profile_px as p inner join mpins_pcp as h on p.mpin=h.mpin group by p.measure_id,p.Measure_desc) as t on t.measure_id=m.measure_id; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D38 TO M40
                    Console.WriteLine(intConsoleCnt++ + " - populating D38 TO M40");
                    strSQL = "SELECT go, go_pct,go_t1, go_t1_pct, go_nt1, go_nt1_pct, go_aco,go_aco_pct,go_naco, go_naco_pct FROM pgsmr_pcp order by measure_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 38, 'D', false);
                    }


                    //GET ALL SCALAR VARIABLES
                    Console.WriteLine(intConsoleCnt++ + " - getting scalar values for 'Automation_Changes' sheet");
                    strSQL = "SELECT count(distinct taxid) FROM tin_cnt WHERE PCP_mpins> 0";
                    obj_tincnt = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT mpins FROM pcp_outp WHERE ord = 1";
                    obj_contr = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT   mpins FROM  pcp_outp WHERE ord = 2";
                    obj_pcr = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT mpins FROM pcp_outp WHERE ord = 3";
                    obj_outl = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT count(*) FROM ubsmr_pcp";
                    obj_utmcnt = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "select count(*) from pbsmr_pcp where all_mp is not null ";
                    obj_pxmcnt = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "select Measure_desc from ubsmr_pcp where measure_id<>50 order by bo_pct des";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dtFinal.Rows.Count > 0)
                    {
                        intlimitCnt = 0;
                        foreach (DataRow dr in dtFinal.Rows)//MAIN LOOP START
                        {
                            intlimitCnt++;
                            sbDescConcat.Append(intlimitCnt + ") " + (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!") + ".");
                            MSWord.wordReplace("{$pcpt3bmeas" + intlimitCnt + "}", (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!"));
                            if (intlimitCnt == 3)
                                break;
                        }

                    }
                    str_t3bmeas = sbDescConcat.ToString();
                    sbDescConcat.Remove(0, sbDescConcat.Length);

                    strSQL = "select Measure_desc from ugsmr_pcp where measure_id <> 50 order by go_pct desc";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dtFinal.Rows.Count > 0)
                    {
                        intlimitCnt = 0;
                        foreach (DataRow dr in dtFinal.Rows)//MAIN LOOP START
                        {
                            intlimitCnt++;
                            sbDescConcat.Append(intlimitCnt + ") " + (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!") + ".");
                            MSWord.wordReplace("{$pcpt3gmeas" + intlimitCnt + "}", (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!"));
                            if (intlimitCnt == 3)
                                break;
                        }

                    }
                    str_t3gmeas = sbDescConcat.ToString();
                    sbDescConcat.Remove(0, sbDescConcat.Length);


                    //ADD ALL SCALAR VALUES TO WORKSHEET
                    Console.WriteLine(intConsoleCnt++ + " - adding scalar values to 'Automation_Changes' sheet");
                    strSheetname = "Automation_Changes";
                    MSExcel.addValueToCell(strSheetname, "B21", obj_tincnt.ToString());
                    MSExcel.addValueToCell(strSheetname, "B22", obj_contr.ToString());
                    MSExcel.addValueToCell(strSheetname, "B23", obj_pcr.ToString());
                    MSExcel.addValueToCell(strSheetname, "B24", obj_outl.ToString());
                    MSExcel.addValueToCell(strSheetname, "B25", obj_utmcnt.ToString());
                    MSExcel.addValueToCell(strSheetname, "B26", obj_pxmcnt.ToString());
                    MSExcel.addValueToCell(strSheetname, "B27", str_t3bmeas);
                    MSExcel.addValueToCell(strSheetname, "B28", str_t3gmeas);

                    MSExcel.addFocusToCell(strSheetname, "A1");


                    MSWord.wordReplace("{$facility_name}", str_facilityName);
                    MSWord.wordReplace("{$pcptincnt}", obj_tincnt.ToString());
                    MSWord.wordReplace("{$pcpcontr}", obj_contr.ToString());
                    MSWord.wordReplace("{$pcppcr}", obj_pcr.ToString());
                    MSWord.wordReplace("{$pcp_outler}", obj_outl.ToString());
                    MSWord.wordReplace("{$pcputmct}", obj_utmcnt.ToString());
                    MSWord.wordReplace("{$pcppxcnt}", obj_pxmcnt.ToString());
                    //PCP Summary SHEET END
                    //PCP Summary SHEET END
                    //PCP Summary SHEET END




                    //SPEC Summary SHEET START
                    //SPEC Summary SHEET START
                    //SPEC Summary SHEET START
                    strSheetname = "SPEC Summary";



                    //CREATE SAS TMP TABLE sp_outp
                    Console.WriteLine(intConsoleCnt++ + " - create table sp_outp");
                    strSQL = "proc sql;create table sp_outp as select count(*) as mpins,sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'contr' as type,1 as ord from mpins_spec UNION select count(*) as mpins,sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'pcr' as type,2 as ord from (select distinct MPIN from ph34.Profile UNION select distinct MPIN from ph34.Profile_px) as p inner join mpins_spec as h on p.mpin=h.mpin UNION select count(*) as mpins,sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'utl' as type,4 as ord from (select distinct p.MPIN,T1,ACO from ph34.Profile as p inner join mpins_spec as h on p.mpin=h.mpin) UNION select count(*) as mpins,sum(T1) as t1mpins, calculated MPINs-calculated t1mpins as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'px' as type,5 as ord from (select distinct p.MPIN,T1,ACO from ph34.Profile_px as p inner join mpins_spec as h on p.mpin=h.mpin) UNION select count(*) as mpins,sum(T1) as t1mpins, calculated MPINs-calculated T1MPINs as nt1mpins, sum(ACO) as acompins, calculated MPINs-calculated acompins as nacompins, 'outl' as type,3 as ord from ph34.Outliers as p inner join mpins_spec as h on p.mpin=h.mpin; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //CREATE SAS TMP TABLE spec_sum_cnt
                    Console.WriteLine(intConsoleCnt++ + " - populating placeholders");
                    strSQL = "SELECT mpins ,t1mpins ,nt1mpins FROM sp_outp order by ord";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 4, 'D', false, new char[] { 'D', 'F', 'H' });
                    }




                    //CREATE SAS TMP TABLE ubsmr_sp
                    Console.WriteLine(intConsoleCnt++ + " - create table ubsmr_sp");
                    strSQL = "proc sql;create table ubsmr_sp as select distinct p.measure_id,Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, sum(outl_idx) as bo, calculated bo/calculated all_mp as bo_pct, sum(case when T1=1 then outl_idx else 0 end) as bo_t1, calculated bo_t1/calculated t1_mp as bo_t1_pct, sum(case when T1<>1 then outl_idx else 0 end) as bo_nt1, calculated bo_nt1/calculated nt1_mp as bo_nt1_pct from ph34.Profile as p inner join mpins_spec as h on p.mpin=h.mpin group by p.measure_id,Measure_desc; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D9 TO I20
                    Console.WriteLine(intConsoleCnt++ + " - populating D9 TO I20");
                    strSQL = "SELECT bo,bo_pct, bo_t1, bo_t1_pct, bo_nt1, bo_nt1_pct FROM ubsmr_sp order by sort_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 9, 'D', false);
                    }



                    //CREATE SAS TMP TABLE pbsmr_sp
                    Console.WriteLine(intConsoleCnt++ + " - create table pbsmr_sp");
                    strSQL = "proc sql;create table pbsmr_sp as select m.measure_id,m.Measure_desc,sort_id, all_mp, t1_mp, nt1_mp,bo,bo_pct,bo_t1,bo_t1_pct,bo_nt1,bo_nt1_pct from (select measure_id,Measure_desc from IL_UCA.PBP_dim_Measures where measure_id not in(40,41,42,19,20,25,26,27,44) and meas_type in('Opioid','Px')) as m left join (select distinct p.measure_id,p.Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, sum(outl_idx) as bo, calculated bo/calculated all_mp as bo_pct, sum(case when T1=1 then outl_idx else 0 end) as bo_t1, calculated bo_t1/calculated t1_mp as bo_t1_pct, sum(case when T1<>1 then outl_idx else 0 end) as bo_nt1, calculated bo_nt1/calculated nt1_mp as bo_nt1_pct from ph34.Profile_px as p inner join mpins_spec as h on p.mpin=h.mpin group by p.measure_id,p.Measure_desc) as t on t.measure_id=m.measure_id; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D21 TO I35
                    Console.WriteLine(intConsoleCnt++ + " - populating D21 TO I3");
                    strSQL = "SELECT  bo,bo_pct, bo_t1, bo_t1_pct, bo_nt1, bo_nt1_pct FROM pbsmr_sp order by measure_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 21, 'D', false);
                    }


                    //CREATE SAS TMP TABLE ugsmr_sp
                    Console.WriteLine(intConsoleCnt++ + " - create table ugsmr_sp");
                    strSQL = "proc sql;create table ugsmr_sp as select distinct p.measure_id,Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, sum(outl_idx_g) as go, calculated go/calculated all_mp as go_pct, sum(case when T1=1 then outl_idx_g else 0 end) as go_t1, calculated go_t1/calculated t1_mp as go_t1_pct, sum(case when T1<>1 then outl_idx_g else 0 end) as go_nt1, calculated go_nt1/calculated nt1_mp as go_nt1_pct from ph34.Profile as p inner join mpins_spec as h on p.mpin=h.mpin group by p.measure_id,Measure_desc; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D36 TO I47
                    Console.WriteLine(intConsoleCnt++ + " - populating D36 TO I47");
                    strSQL = "SELECT go, go_pct,go_t1, go_t1_pct, go_nt1, go_nt1_pct FROM ugsmr_sp order by sort_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 36, 'D', false);
                    }


                    //CREATE SAS TMP TABLE pgsmr_sp
                    Console.WriteLine(intConsoleCnt++ + " - create table pgsmr_sp");
                    //strSQL = "proc sql;create table pgsmr_sp as select m.measure_id,m.Measure_desc,sort_id, all_mp, t1_mp, nt1_mp,go,go_pct,go_t1,go_t1_pct,go_nt1,go_nt1_pct from (select measure_id,Measure_desc from IL_UCA.PBP_dim_Measures where measure_id not in(40,41,42,19,20,25,26,27,44) and meas_type in('Opioid','Px')) as m left join (select distinct p.measure_id,p.Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, count(distinct case when ACO=1 then p.MPIN end) as aco_mp, calculated all_mp-calculated aco_mp as naco_mp, sum(outl_idx_g) as go, calculated go/calculated all_mp as go_pct, sum(case when T1=1 then outl_idx_g else 0 end) as go_t1, calculated go_t1/calculated t1_mp as go_t1_pct, sum(case when T1<>1 then outl_idx_g else 0 end) as go_nt1, calculated go_nt1/calculated nt1_mp as go_nt1_pct from ph34.Profile_px as p inner join mpins_pcp as h on p.mpin=h.mpin group by p.measure_id,p.Measure_desc) as t on t.measure_id=m.measure_id; quit;";
                    strSQL = "proc sql;create table pgsmr_sp as select m.measure_id,m.Measure_desc,sort_id, all_mp, t1_mp, nt1_mp,go,go_pct,go_t1,go_t1_pct,go_nt1,go_nt1_pct from (select measure_id,Measure_desc from IL_UCA.PBP_dim_Measures where measure_id not in(40,41,42,19,20,25,26,27,44) and meas_type in('Opioid','Px')) as m left join (select distinct p.measure_id,p.Measure_desc,sort_id, count(distinct p.MPIN) as all_mp, count(distinct case when T1=1 then p.MPIN end) as t1_mp, calculated all_mp-calculated t1_mp as nt1_mp, sum(outl_idx_g) as go, calculated go/calculated all_mp as go_pct, sum(case when T1=1 then outl_idx_g else 0 end) as go_t1, calculated go_t1/calculated t1_mp as go_t1_pct, sum(case when T1<>1 then outl_idx_g else 0 end) as go_nt1, calculated go_nt1/calculated nt1_mp as go_nt1_pct from ph34.Profile_px as p inner join mpins_spec as h on p.mpin=h.mpin group by p.measure_id,p.Measure_desc) as t on t.measure_id=m.measure_id; quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    //DATA FOR TOP SECTION D48 TO M62
                    Console.WriteLine(intConsoleCnt++ + " - populating D48 TO M62");
                    strSQL = "SELECT go, go_pct,go_t1, go_t1_pct, go_nt1, go_nt1_pct FROM pgsmr_sp order by sort_id;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 48, 'D', false);
                    }


                    //GET ALL SCALAR VARIABLES
                    Console.WriteLine(intConsoleCnt++ + " - getting scalar values for 'Automation_Changes' sheet");
                    strSQL = "SELECT count(distinct taxid) FROM tin_cnt WHERE SP_mpins> 0";
                    obj_tincnt = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT mpins FROM sp_outp WHERE ord = 1";
                    obj_contr = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT   mpins FROM  sp_outp WHERE ord = 2";
                    obj_pcr = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT mpins FROM sp_outp WHERE ord = 3";
                    obj_outl = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "SELECT count(*) FROM ubsmr_sp";
                    obj_utmcnt = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "select count(*) from pbsmr_sp where all_mp is not null ";
                    obj_pxmcnt = DBConnection64.getOleDbExecuteScalar(IR_SAS_Connect.strSASConnectionString, strSQL);

                    strSQL = "select Measure_desc from ubsmr_sp where measure_id<>50 order by bo_pct des";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dtFinal.Rows.Count > 0)
                    {
                        intlimitCnt = 0;
                        foreach (DataRow dr in dtFinal.Rows)//MAIN LOOP START
                        {
                            intlimitCnt++;
                            sbDescConcat.Append(intlimitCnt + ") " + (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!") + ".");
                            MSWord.wordReplace("{$spt3gumeas" + intlimitCnt + "}", (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!"));
                            if (intlimitCnt == 3)
                                break;
                        }

                    }
                    str_t3bmeas = sbDescConcat.ToString();
                    sbDescConcat.Remove(0, sbDescConcat.Length);


                    strSQL = "select Measure_desc from ugsmr_sp where measure_id <> 50 order by go_pct desc";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dtFinal.Rows.Count > 0)
                    {
                        intlimitCnt = 0;
                        foreach (DataRow dr in dtFinal.Rows)//MAIN LOOP START
                        {
                            intlimitCnt++;
                            sbDescConcat.Append(intlimitCnt + ") " + (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!") + ".");
                            MSWord.wordReplace("{$spt3gpmeas" + intlimitCnt + "}", (dr["Measure_desc"] != DBNull.Value ? dr["Measure_desc"].ToString().Trim() : "NAME MISSING!!!"));
                            if (intlimitCnt == 3)
                                break;
                        }

                    }
                    str_t3gmeas = sbDescConcat.ToString();
                    sbDescConcat.Remove(0, sbDescConcat.Length);

                    //ADD ALL SCALAR VALUES TO WORKSHEET
                    Console.WriteLine(intConsoleCnt++ + " - adding scalar values to 'Automation_Changes' sheet");
                    strSheetname = "Automation_Changes";
                    MSExcel.addValueToCell(strSheetname, "B31", obj_tincnt.ToString());
                    MSExcel.addValueToCell(strSheetname, "B32", obj_contr.ToString());
                    MSExcel.addValueToCell(strSheetname, "B33", obj_pcr.ToString());
                    MSExcel.addValueToCell(strSheetname, "B34", obj_outl.ToString());
                    MSExcel.addValueToCell(strSheetname, "B35", obj_utmcnt.ToString());
                    MSExcel.addValueToCell(strSheetname, "B36", obj_pxmcnt.ToString());
                    MSExcel.addValueToCell(strSheetname, "B37", str_t3bmeas);
                    MSExcel.addValueToCell(strSheetname, "B38", str_t3gmeas);

                    MSExcel.addFocusToCell(strSheetname, "A1");

                    MSWord.wordReplace("{$sptincnt}", obj_tincnt.ToString());
                    MSWord.wordReplace("{$spcontr}", obj_contr.ToString());
                    MSWord.wordReplace("{$sppcr}", obj_pcr.ToString());
                    MSWord.wordReplace("{$sp_outler}", obj_outl.ToString());
                    MSWord.wordReplace("{$sputmcnt}", obj_utmcnt.ToString());
                    MSWord.wordReplace("{$sppxmcnt}", obj_pxmcnt.ToString());
                    //SPEC Summary SHEET END
                    //SPEC Summary SHEET END
                    //SPEC Summary SHEET END


                    //POPULATE Hist_Data_All START
                    //POPULATE Hist_Data_All START
                    //POPULATE Hist_Data_All START
                    strSheetname = "Hist_Data_All";

                    //CREATE SAS TMP TABLE dtl 
                    Console.WriteLine(intConsoleCnt++ + " - create table dtl");
                    strSQL = "proc sql;create table dtl as select t.TAXID,t.PTIGroupName as Practice_Name,t.MPIN, case when FirstName is null then trim(LastName) else trim(FirstName)||' '||trim(LastName) end as Phys_name, t.RGN_NM,t.MKT_RLLP_NM,State, t.NDB_Specialty,a.attr_clients,T1, ACO, case when o.mpin is not null then 1 end as PCR_outl, Measure_ID,Measure_desc,act,expected,oe_ratio,Outl_idx,Outl_idx_g from mpins_pcp as t inner join (select MPIN,Measure_ID,Measure_desc,act,expected,oe_ratio,Outl_idx,Outl_idx_g from PH14.Profile where measure_id not in(14,15) UNION select MPIN,Measure_ID,Measure_desc,act,expected,oe_ratio,Outl_idx,Outl_idx_g from PH14.Profile_px where measure_id in(38,51,55) UNION select attr_mpin,100 as Measure_ID,'Total Cost Attr Mbrs Current Period' as Measure_desc, act_Total_allw as act,exp_Total_allw as expected,OE_allw as oe_ratio,. as Outl_idx,. as Outl_idx_g from Ph14.Saving_MPIN UNION select attr_mpin,101 as Measure_ID,'Total Cost Attr Mbrs Prior Period' as Measure_desc, act_Total_allw as act,exp_Total_allw as expected,OE_allw as oe_ratio,. as Outl_idx,. as Outl_idx_g from Ph14.Saving_MPIN_PR) as u on u.mpin=t.mpin left join PH14.Outliers as o on o.mpin=t.mpin left join (select attr_mpin,count(*) as attr_clients from ph14.MPIN_CLIENT_JAN22_TRSET group by attr_mpin) as a on a.attr_mpin=t.mpin UNION select t.TAXID,t.PTIGroupName as Practice_Name,t.MPIN, case when FirstName is null then trim(LastName) else trim(FirstName)||' '||trim(LastName) end as Phys_name, t.RGN_NM,t.MKT_RLLP_NM,State, t.NDB_Specialty,a.attr_clients,T1, ACO, case when o.mpin is not null then 1 end as PCR_outl, Measure_ID,Measure_desc,act,expected,oe_ratio,Outl_idx,Outl_idx_g from mpins_spec as t inner join (select MPIN,Measure_ID,Measure_desc,act,expected,oe_ratio,Outl_idx,Outl_idx_g from PH34.Profile UNION select MPIN,Measure_ID,Measure_desc,act,expected,oe_ratio,Outl_idx,Outl_idx_g from PH34.Profile_px where measure_id not in(40,41,42) UNION select attr_mpin,100 as Measure_ID,'Total Cost Attr Mbrs Current Period' as Measure_desc, act_Total_allw as act,exp_Total_allw as expected,OE_allw as oe_ratio,. as Outl_idx,. as Outl_idx_g from PH34.Saving_MPIN UNION select attr_mpin,101 as Measure_ID,'Total Cost Attr Mbrs Prior Period' as Measure_desc, act_Total_allw as act,exp_Total_allw as expected,OE_allw as oe_ratio,. as Outl_idx,. as Outl_idx_g from PH34.Saving_MPIN_PR) as u on u.mpin=t.mpin left join PH34.Outliers as o on o.mpin=t.mpin left join (select attr_mpin,count(*) as attr_clients from Ph34.MPIN_CLIENT_MAY6_TRSET group by attr_mpin) as a on a.attr_mpin=t.mpin;quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);

                    Console.WriteLine(intConsoleCnt++ + " - create tables Hist_T1_ACO and Hist_T1");
                    sbSQL.Append("proc sort data=dtl OUT=dtl;");
                    sbSQL.Append("by NDB_Specialty Measure_desc T1 ACO;");
                    //CREATE SAS TMP TABLE Hist_T1_ACO
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by NDB_Specialty Measure_desc T1 ACO;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_T1_ACO;");
                    sbSQL.Append("run;");
                    //CREATE SAS TMP TABLE Hist_T1
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by NDB_Specialty Measure_desc T1;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_T1;");
                    sbSQL.Append("run;");
                    IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString(), false);
                    sbSQL.Remove(0, sbSQL.Length);

                    Console.WriteLine(intConsoleCnt++ + " - create tables Hist_ACO and Hist_All");
                    sbSQL.Append("proc sort data=dtl OUT=dtl;");
                    sbSQL.Append("by NDB_Specialty Measure_desc ACO;");
                    //CREATE SAS TMP TABLE Hist_ACO;
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by NDB_Specialty Measure_desc ACO;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_ACO;");
                    sbSQL.Append("run;");
                    //CREATE SAS TMP TABLE Hist_All
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by NDB_Specialty Measure_desc;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_All;");
                    sbSQL.Append("run;");
                    IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString(), false);
                    sbSQL.Remove(0, sbSQL.Length);

                    Console.WriteLine(intConsoleCnt++ + " - create tables Hist_All_NoSpec, Hist_T1ACO_NoSpec and Hist_T1_NoSpec");
                    sbSQL.Append("proc sort data=dtl OUT=dtl;");
                    sbSQL.Append("by Measure_desc T1 ACO;");
                    //CREATE SAS TMP TABLE Hist_All_NoSpec
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by Measure_desc;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_All_NoSpec;");
                    sbSQL.Append("run;");
                    //CREATE SAS TMP TABLE Hist_T1ACO_NoSpec
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by Measure_desc T1 ACO;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_T1ACO_NoSpec;");
                    sbSQL.Append("run;");
                    //CREATE SAS TMP TABLE Hist_T1_NoSpec
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by Measure_desc T1;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_T1_NoSpec;");
                    sbSQL.Append("run;");
                    IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString(), false);
                    sbSQL.Remove(0, sbSQL.Length);

                    Console.WriteLine(intConsoleCnt++ + " - create table Hist_ACO_NoSpec");
                    sbSQL.Append("proc sort data=dtl OUT=dtl;");
                    sbSQL.Append("by Measure_desc ACO;");
                    //CREATE SAS TMP TABLE Hist_ACO_NoSpec
                    sbSQL.Append("ODS Select Histogram;");
                    sbSQL.Append("PROC Univariate data = dtl robustscale plot noprint;");
                    sbSQL.Append("var OE_ratio;");
                    sbSQL.Append("by Measure_desc ACO;");
                    sbSQL.Append("histogram OE_ratio / normal(mu = est sigma = est color = blue w = 2.5) barlabel = percent outhistogram = Hist_ACO_NoSpec;");
                    sbSQL.Append("run;");
                    IR_SAS_Connect.runProcSQLCommands(sbSQL.ToString(), false);
                    sbSQL.Remove(0, sbSQL.Length);



                    //CREATE SAS TMP TABLE Hist
                    Console.WriteLine(intConsoleCnt++ + " - create table Hist");
                    strSQL = "proc sql;create table Hist as Select Distinct 1 as spec_Ind, Case when T1=1 then 'Yes' Else 'No' END AS T1, Case when ACO=1 then 'Yes' Else 'No' END AS ACO, NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_T1_ACO UNION Select Distinct 1 as spec_Ind,'Combined' as T1,'Combined' as ACO, NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_All UNION Select Distinct 1 as spec_Ind, Case when T1=1 then 'Yes' Else 'No' END AS T1, 'Combined' AS ACO, NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_T1 UNION Select Distinct 1 as spec_Ind, 'Combined' AS T1, Case when ACO=1 then 'Yes' Else 'No' END AS ACO, NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_ACO UNION Select Distinct 0 as spec_Ind, 'Combined' as T1,'Combined' as ACO, '' as NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_All_NoSpec UNION Select Distinct 0 as spec_Ind, Case when T1=1 then 'Yes' Else 'No' END AS T1, Case when ACO=1 then 'Yes' Else 'No' END AS ACO, '' as NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_T1ACO_NoSpec UNION Select Distinct 0 as spec_Ind, Case when T1=1 then 'Yes' Else 'No' END AS T1, 'Combined' as ACO, '' as NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_T1_NoSpec UNION Select Distinct 0 as spec_Ind, 'Combined' AS T1, Case when ACO=1 then 'Yes' Else 'No' END as ACO, '' as NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_/100 as _OBSPCT_ format percent10.2,_COUNT_,_CURVE_,_EXPPCT_ FROM Hist_ACO_NoSpec;quit;";
                    IR_SAS_Connect.runProcSQLCommands(strSQL);
                    Console.WriteLine(intConsoleCnt++ + " - retreiving data for sheet 'Hist_Data_All'");
                    strSQL = "SELECT spec_Ind, T1, ACO, NDB_Specialty,Measure_desc,_VAR_,_MIDPT_,_OBSPCT_,_COUNT_,_CURVE_,_EXPPCT_  FROM Hist;";
                    dtFinal = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    Console.WriteLine(intConsoleCnt++ + " - popluating sheet 'Hist_Data_All'");
                    if (blHasExcel && dtFinal.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dtFinal, strSheetname, 2, 'A', false);
                    }

                    //REFRESH PIVOT TO UPDATE NEW Hist_Data_All INTO Measure Histogram
                    Console.WriteLine(intConsoleCnt++ + " - refreshing sheet 'Hist_Data_All'");
                    MSExcel.RefreshPivot(strSheetname, "Spec_T1", "N:N");

                    MSExcel.addFocusToCell(strSheetname, "A1");
                    //POPULATE Hist_Data_All END
                    //POPULATE Hist_Data_All END
                    //POPULATE Hist_Data_All END


                    //FINAL EXCEL CLEANUP
                    //FINAL EXCEL CLEANUP
                    MSExcel.hideWorkSheet("Automation_Changes");
                    MSExcel.hideWorkSheet("Hist_Data_All");
                    MSExcel.addFocusToCell("PCR Background", "A1");





                    var strDateTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
                    var strFileName = Path.GetFileName(sFile).ToLower().Replace(".xlsx", "") + "_" + strDateTime ;
                    var strFinalPath = strOutputPath + "\\" + strFileName;


                    Console.WriteLine(intConsoleCnt++ + " - Saving Report " + strFileName);
                    MSExcel.closeExcelWorkbook(strFinalPath + ".xlsx");


                    MSWord.closeWordDocument(strFinalPath + ".docx");



                    Utilities.ZipFileCreator.CreateZipFile(strFinalPath + ".zip", (new string[] { strFinalPath + ".docx", strFinalPath + ".xlsx" }).AsEnumerable<string>());

                    File.Delete(strFinalPath + ".docx");
                    File.Delete(strFinalPath + ".xlsx");


                    Console.WriteLine(intConsoleCnt++ + " - Archiving Template " + Path.GetFileName(sFile));
                    File.Move(sFile, strInputPath + "\\Archive\\" + strFileName + ".xlsx");

                }

              


            }
            catch (Exception ex)
            {



                if (!EventLog.SourceExists("Wiser Choices"))
                    EventLog.CreateEventSource("Wiser Choices", "Application");


                EventLog.WriteEntry("Wiser Choices", ex.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine + strSQL, EventLogEntryType.Error, 234);


                Console.WriteLine("There was an error, see details below");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();
                Console.WriteLine("SQL:");
                Console.WriteLine(strSQL);

                // Console.Beep();


                //Console.ReadLine();


            }
            finally
            {

                try
                {
                    DBConnection64.getOleDbDataTableGlobalClose();
                    IR_SAS_Connect.destroy_SAS_instance();


                }
                catch (Exception)
                {

                }




                try
                {

                    Console.WriteLine("Closing Microsoft Excel Instance...");
                    //CLOSE EXCEL APP
                    MSExcel.closeExcelApp();

                }
                catch (Exception)
                {

                }

                try
                {

                    Console.WriteLine("Closing Microsoft Word Instance...");
                    //CLOSE WORD APP
                    MSWord.closeWordApp();

                }
                catch (Exception)
                {

                }



                try
                {
                    foreach (Process Proc in Process.GetProcesses())
                        if (Proc.ProcessName.Equals("EXCEL") || Proc.ProcessName.Equals("WINWORD"))  //Process Excel?
                            Proc.Kill();
                }
                catch (Exception)
                {
                    try
                    {
                        foreach (Process Proc in Process.GetProcesses())
                            if (Proc.ProcessName.Equals("EXCEL") || Proc.ProcessName.Equals("WINWORD"))  //Process Excel?
                                Proc.Kill();
                    }
                    catch (Exception)
                    {

                    }
                }
            }


            if (intProfileCnt < intTotalCnt)
                goto Start;
        }



    }
}
