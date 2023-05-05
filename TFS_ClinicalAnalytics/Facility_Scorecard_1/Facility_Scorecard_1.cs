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

namespace Facility_Scorecard_1
{
    class Facility_Scorecard_1
    {
        static void Main(string[] args)
        {

            string strSQL = null;

            try
            {

                //killProcesses();

                //Decimal.Parse("Test");
                Console.WriteLine("Wiser Choices Profiles Generator");
                //Console.WriteLine("Gathering Configuration Values...");


                //PLACE APP.CONFIG FILE DATA INTO VARIABLES START
                string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
                bool blVisibleExcel = Boolean.Parse(ConfigurationManager.AppSettings["VisibleExcel"]);
                bool blSaveExcel = Boolean.Parse(ConfigurationManager.AppSettings["SaveExcel"]);
                bool blVisibleWord = Boolean.Parse(ConfigurationManager.AppSettings["VisibleWord"]);
                bool blSaveWord = Boolean.Parse(ConfigurationManager.AppSettings["SaveWord"]);
                string strExcelTemplate = ConfigurationManager.AppSettings["ExcelTemplate"];
                string strWordTemplate = ConfigurationManager.AppSettings["WordTemplate"];
                bool blOverwriteExisting = Boolean.Parse(ConfigurationManager.AppSettings["OverwriteExisting"]);
                string strStartDate = ConfigurationManager.AppSettings["StartDate"];
                string strEndDate = ConfigurationManager.AppSettings["EndDate"];
                string strDisplayDate = ConfigurationManager.AppSettings["ProfileDate"];
                string strReportsPath = ConfigurationManager.AppSettings["ReportsPath"];
                string strPhase = ConfigurationManager.AppSettings["Phase"];
                string strSpecialtyId = ConfigurationManager.AppSettings["SpecialtyId"];
                string strCAVerbiage = "";
                string strCAVerbiage2 = "";
                //strSpecialtyId = null; //ALL BUT 4
                //strSpecialtyId = -99999;  //ALL SPECIALTIES
                //strSpecialtyId = 2; //SPECIFIC SPECIALTY


                string strPEIPath = ConfigurationManager.AppSettings["PEIPath"];

                string strEpisodeCount = ConfigurationManager.AppSettings["EpisodeCount"];



                if (String.IsNullOrEmpty(strSpecialtyId))
                    strSpecialtyId = null;


                //PLACE CONFIG FILE DATA INTO VARIABLES END

                string strMonthYear = DateTime.Now.Month + "_" + DateTime.Now.Year;
                string strFinalReportFileName;


                bool blHasWord = true;
                bool blHasPDF = true;

                bool blIsMasked = false;


                //START EXCEL APP
                MSExcel.populateExcelParameters(blVisibleExcel, blSaveExcel, strReportsPath, strExcelTemplate);
                MSExcel.openExcelApp();

                //START WORD APP
                if (blHasWord)
                {
                    MSWord.populateWordParameters(blVisibleWord, blSaveWord, strReportsPath, strWordTemplate);
                    MSWord.openWordApp();
                }

               

                string strSheetname = null;

                int intProfileCnt = 1;
                int intTotalCnt;


                int intEndingRowTmp;


                string strPracticeID;


                string strStreet;
                string strCity;
                string strState;
                string strZipCd;
                string strMPIN;
                string strContact;
                string strHospitalName;
                string strModelId;
                string strCAIndicator;
                string strBundleIndx;
                string strFolderName;
                string strFinalFolderName = "";
                string strDear;

                string strLFId;
                string strMedId;
                string strLFStreet;
                string strLFCity;
                string strLFState;
                string strLFZipCd;
                string strLFHospitalName;





                int intPageCheck1 = 0;
                int intPageCheck2 = 0;


                Int16 intAddressCnt = 0;

                string strWordDocLetter = MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordLetter"];
                string strWordDocMain = MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordMain"];
                string strWordDocOpioid = ConfigurationManager.AppSettings["WordOpioid"];
                string strWordDocEnd = ConfigurationManager.AppSettings["WordEnd"];
                


                string strSampleCount = "1";
                string strMPINList = "Select DISTINCT TOP "+ strSampleCount + " MPIN  FROM [IL_UCA].[dbo].[FAC_SC_mailing] where [ltr_model_id]<4";
                //strMPINList = "Select DISTINCT TOP " + strSampleCount + " MPIN  FROM [IL_UCA].[dbo].[FAC_SC_mailing] where [ltr_model_id] = 1";

                //strMPINList = "189864";
                strMPINList = "SELECT MPIN FROM ( Select TOP " + strSampleCount + " MPIN FROM [IL_UCA].[dbo].[FAC_SC_mailing] where [ltr_model_id] = 1 AND MPIN in ( select t.mpin FROM ( select f.mpin, Count(f.Leapfrog_ID) as cnt FROM [IL_UCA].[dbo].[FAC_SC_Surg] f group by f.mpin having Count(f.Leapfrog_ID) > 10 ) t ) ORDER BY NEWID() ) tmp UNION SELECT MPIN FROM ( Select TOP " + strSampleCount + " MPIN FROM [IL_UCA].[dbo].[FAC_SC_mailing] where [ltr_model_id] = 2 ORDER BY NEWID() ) tmp UNION SELECT MPIN FROM ( Select TOP " + strSampleCount + " MPIN FROM [IL_UCA].[dbo].[FAC_SC_mailing] where [ltr_model_id] = 3 ORDER BY NEWID() UNION SELECT TOP " + strSampleCount + " MPIN FROM ( SELECT mpin FROM ( SELECT mpin, COUNT(POS_Address) as cnt FROM dbo.FAC_DEMOG_NTWK_reviewed WHERE Suppress_id in(0,1) group by mpin HAVING COUNT(POS_Address) > 20 ) t ) tmp ORDER BY NEWID() ) tmp";




                //strMPINList = "SELECT MPIN FROM ( SELECT TOP " + strSampleCount + " MPIN FROM ( SELECT mpin FROM ( SELECT mpin, COUNT(POS_Address) as cnt FROM dbo.FAC_DEMOG_NTWK_reviewed WHERE Suppress_id in(0,1) group by mpin HAVING COUNT(POS_Address) > 30 and COUNT(POS_Address) < 1500 ) t ) tmp ORDER BY NEWID() ) tmp";





                strMPINList = "464287, 512960, 29160, 622163, 1419124, 656346, 211487";



                strSQL = "Select MPIN ,Hosp_name ,[Contact] ,[street] ,[City] ,[State] ,[ZipCd], ltr_model_id,[CA_idx],[budnle_idx],[Folder_name], [dear] FROM [IL_UCA].[dbo].[FAC_SC_mailing] where [ltr_model_id]<4 AND MPIN IN (" + strMPINList + ");";




                
                MSWord.footerFontType = "Arial";
                MSWord.footerFontSize = 9;



                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                DataTable dt = null;
                DataTable dtLeapFrogIDs = null;
                DataTable dtLeapFrog = null;
                DataTable dtLeapFrogISV = null;
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {
                    MSWord.strWordTemplate = null;
                    intAddressCnt = 0;

                    strMPIN = (dr["MPIN"] != DBNull.Value ? dr["MPIN"].ToString().Trim() : "VALUE MISSING");
                    strStreet = (dr["Street"] != DBNull.Value ? dr["Street"].ToString().Trim() : "VALUE MISSING");
                    strCity = (dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "VALUE MISSING");
                    strState = (dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "VALUE MISSING");
                    strZipCd = (dr["ZipCd"] != DBNull.Value ? dr["ZipCd"].ToString().Trim() : "VALUE MISSING");
                    strContact = (dr["Contact"] != DBNull.Value ? dr["Contact"].ToString().Trim() : "VALUE MISSING");
                    strHospitalName = (dr["Hosp_name"] != DBNull.Value ? dr["Hosp_name"].ToString().Trim() : "VALUE MISSING");
                    strModelId = (dr["ltr_model_id"] != DBNull.Value ? dr["ltr_model_id"].ToString().Trim() : "VALUE MISSING");
                    strCAIndicator = (dr["CA_idx"] != DBNull.Value ? dr["CA_idx"].ToString().Trim() : "VALUE MISSING");
                    strBundleIndx = (dr["budnle_idx"] != DBNull.Value ? dr["budnle_idx"].ToString().Trim() : "VALUE MISSING");
                    strFolderName = (dr["Folder_name"] != DBNull.Value ? dr["Folder_name"].ToString().Trim() : "VALUE MISSING");
                    strDear = (dr["dear"] != DBNull.Value ? dr["dear"].ToString().Trim() : "VALUE MISSING");


                    if (strBundleIndx == "0")
                        strFinalFolderName = strFolderName;
                    else if (strBundleIndx == "1")
                        strFinalFolderName = "Bundle_mail\\" + strFolderName;
                    else if (strBundleIndx == "9")
                        strFinalFolderName = "Special_handling\\" + strFolderName;



                    //COMMENT ME!!!!!!!!!!!!
                    //strFinalFolderName = "testing";

                    //strCAIndicator = "1";

                    MSExcel.strReportsPath = strReportsPath ;
                    strFinalReportFileName = strMPIN + "_" + strHospitalName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_Scorcards_" + strMonthYear;

                    //IF THE CURRENT PROFILE ALREADY EXISTS WE DO OR DONT WANT TO OVERWRITE PROFILE (SEE APP.CONFIG)...
                    if (!blOverwriteExisting)
                    {
                        //...CHECK IF PROFILE EXISTS...
                        if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "Final\\" + strFinalFolderName) + strFinalReportFileName + ".pdf"))
                        {
                            Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                            intProfileCnt++;
                            //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                            continue;
                        }
                    }


                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();

                    MSWord.strWordTemplate = strWordDocLetter;
                    MSWord.openWordDocument();

                    MSWord.wordReplace("{$Date}", strStartDate);
                    MSWord.wordReplace("{$HospitalAdminName}", strContact);
                    MSWord.wordReplace("{$HospitalAdminNameDear}", strDear);
                    MSWord.wordReplace("{$HospitalPreferredName}", strHospitalName);
                    MSWord.wordReplace("{$PostalAddress}", strStreet);
                    MSWord.wordReplace("{$City}", strCity);
                    MSWord.wordReplace("{$State}", strState);
                    MSWord.wordReplace("{$Zip}", strZipCd);
                    MSWord.wordReplace("{$MPIN}", strMPIN);

                    strCAVerbiage = "";
                    strCAVerbiage2 = "";
                    if (strCAIndicator == "1")
                    {
                        strCAVerbiage = ConfigurationManager.AppSettings["CA_Verbiage"];
                        strCAVerbiage2 = ConfigurationManager.AppSettings["CA_Verbiage2"];
                        MSWord.addLineBreak("ca_verbiage_start");
                        MSWord.addLineBreak("ca_verbiage_end");
                        MSWord.addLineBreak("ca_verbiage2_start");
                    }

                    MSWord.wordReplaceLong("{$ca_verbiage}", strCAVerbiage);
                    MSWord.wordReplaceLong("{$ca_verbiage2}", strCAVerbiage2);
                    MSWord.deleteBookmarkComplete("ca_verbiage_start");
                    MSWord.deleteBookmarkComplete("ca_verbiage_end");
                    MSWord.deleteBookmarkComplete("ca_verbiage2_start");


                    //NEXT
                    if (strModelId == "1" || strModelId == "2")
                    {

                        dtLeapFrogIDs = DBConnection64.getMSSQLDataTable(strConnectionString, "select distinct l.Leapfrog_ID from dbo.FAC_SC_Leapfrog_2018_unpivot as l inner join dbo.fac_sc_identified_addr as a on l.Leapfrog_ID=a.Leapfrog_ID inner join dbo.FAC_DEMOG_NTWK_reviewed as r on r.mpin=a.mpin and r.AddrSeq=a.AddrSeq where Suppress_id in(0,2) and a.mpin=" + strMPIN);

                        if (dtLeapFrogIDs.Rows.Count > 0)
                        {

                            foreach (DataRow drLF_ID in dtLeapFrogIDs.Rows)//MAIN LOOP START
                            {

                                //if (MSWord.strWordTemplate == null)
                                //{
                                //    MSWord.strWordTemplate = strWordDocMain;
                                //    MSWord.openWordDocument();
                                //}
                                //else
                                //{
                                //APPEND LF MAIN!!!
                                //MSWord.appendWordDocument(strWordDocMain);
                                MSWord.footerRange = MSWord.addWordDocument(strWordDocMain);
                               // MSWord.AddFooterToRange("CSG 1");


                                strLFId = (drLF_ID["Leapfrog_ID"] != DBNull.Value ? drLF_ID["Leapfrog_ID"].ToString().Trim() : "VALUE MISSING");

                                dtLeapFrog = DBConnection64.getMSSQLDataTable(strConnectionString, "SELECT MPIN ,LastName as [UHC Hospital Name] ,Leapfrog_ID ,Medicare_Provider_Number as [Medicare ID] ,upper(Hospital) as [Leapfrog Hospital Name] ,Street ,City ,State ,ZipCd FROM dbo.fac_sc_identified_addr where Leapfrog_ID='" + strLFId + "'");



                                strMedId = (dtLeapFrog.Rows[0]["Medicare ID"] != DBNull.Value ? dtLeapFrog.Rows[0]["Medicare ID"].ToString().Trim() : "VALUE MISSING");
                                strLFStreet = (dtLeapFrog.Rows[0]["Street"] != DBNull.Value ? dtLeapFrog.Rows[0]["Street"].ToString().Trim() : "VALUE MISSING");
                                strLFCity = (dtLeapFrog.Rows[0]["City"] != DBNull.Value ? dtLeapFrog.Rows[0]["City"].ToString().Trim() : "VALUE MISSING");
                                strLFState = (dtLeapFrog.Rows[0]["State"] != DBNull.Value ? dtLeapFrog.Rows[0]["State"].ToString().Trim() : "VALUE MISSING");
                                strLFZipCd = (dtLeapFrog.Rows[0]["ZipCd"] != DBNull.Value ? dtLeapFrog.Rows[0]["ZipCd"].ToString().Trim() : "VALUE MISSING");
                                strLFHospitalName = (dtLeapFrog.Rows[0]["Leapfrog Hospital Name"] != DBNull.Value ? dtLeapFrog.Rows[0]["Leapfrog Hospital Name"].ToString().Trim() : "VALUE MISSING");


                                MSWord.wordReplace("{$UHC_HOSP_NM}", strHospitalName);
                                MSWord.wordReplace("{$LF_HOSP_NM}", strLFHospitalName);
                                MSWord.wordReplace("{$HOSP_ADDR}", strLFStreet + ", " + strLFCity + ", " + strLFState+ "-" + strLFZipCd);
                                MSWord.wordReplace("{$MPIN}", strMPIN);
                                MSWord.wordReplace("{$MED_ID}", strMedId);
                                MSWord.wordReplace("{$LF_ID}", strLFId);



                                //dtLeapFrogISV = DBConnection.getMSSQLDataTable(strConnectionString, "SELECT HospVol_displ ,Threshold ,message_displ FROM dbo.FAC_SC_Surg where Leapfrog_ID='" + strLFId + "' order by Measure_Description");

                                dtLeapFrogISV = DBConnection64.getMSSQLDataTable(strConnectionString, "select case when HospVol>0 then convert(varchar(3),HospVol) else case when HospVol is null and Results='Declined to Respond' then 'na' else '-' end end as HospVol_displ, case when HospVol>=Threshold then 'Meets Benchmark' when HospVol<Threshold then 'Below Benchmark' else case when HospVol is null and Results='Declined to Respond' then 'Data Not Available' else 'Procedure Not Performed' end end as message_displ from dbo.FAC_SC_Leapfrog_2018_unpivot as l inner join dbo.FAC_SC_dim_measures as m on l.PX_id=Measure_id inner join dbo.fac_sc_identified_addr as a on l.Leapfrog_ID=a.Leapfrog_ID inner join dbo.FAC_DEMOG_NTWK_reviewed as r on r.mpin=a.mpin and r.AddrSeq=a.AddrSeq where Suppress_id in(0,2) and l.Leapfrog_ID='" + strLFId + "' order by Measure_Description");




                                //dtLeapFrogISV = DBConnection.getMSSQLDataTable(strConnectionString, 1234567890);



                                MSWord.wordReplace("{$BSWL_HV}", (dtLeapFrogISV.Rows[0]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[0]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$BSWL_BC}", (dtLeapFrogISV.Rows[0]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[0]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$CE_HV}", (dtLeapFrogISV.Rows[1]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[1]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$CE_BC}", (dtLeapFrogISV.Rows[1]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[1]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$ERC_HV}", (dtLeapFrogISV.Rows[2]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[2]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$ERC_BC}", (dtLeapFrogISV.Rows[2]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[2]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$LRC_HV}", (dtLeapFrogISV.Rows[3]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[3]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$LRC_BC}", (dtLeapFrogISV.Rows[3]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[3]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$MVR_HV}", (dtLeapFrogISV.Rows[4]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[4]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$MVR_BC}", (dtLeapFrogISV.Rows[4]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[4]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$OAAA_HV}", (dtLeapFrogISV.Rows[5]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[5]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$OAAA_BC}", (dtLeapFrogISV.Rows[5]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[5]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$PRC_HV}", (dtLeapFrogISV.Rows[6]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[6]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$PRC_BC}", (dtLeapFrogISV.Rows[6]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[6]["message_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$RCS_HV}", (dtLeapFrogISV.Rows[7]["HospVol_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[7]["HospVol_displ"].ToString().Trim() : "-"));
                                MSWord.wordReplace("{$RCS_BC}", (dtLeapFrogISV.Rows[7]["message_displ"] != DBNull.Value ? dtLeapFrogISV.Rows[7]["message_displ"].ToString().Trim() : "-"));



                            }


                        }

                    }



                    if (strModelId == "1" || strModelId == "3")
                    {



                        //if (MSWord.strWordTemplate == null)
                        //{
                        //    MSWord.strWordTemplate = strWordDocOpioid;
                        //    MSWord.openWordDocument();
                        //}
                        //else
                        //{
                        //APPEND OPIOID!!!
                        //MSWord.appendWordDocument(strWordDocOpioid);
                        MSWord.footerRange = MSWord.addWordDocument(strWordDocOpioid);
                        //MSWord.AddFooterToRange("CSG 2");

                        //}



                        MSWord.wordReplace("{$UHC_HOSP_NM}", strHospitalName);
                        MSWord.wordReplace("{$MPIN}", strMPIN);





                        strSheetname = "Opioids_hosp_addr";

                        //dt = DBConnection.getMSSQLDataTable(strConnectionString, "select POS_Address,POS_City,POS_State,POS_ZIP from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1) and mpin=" + strMPIN + " order by POS_City,POS_Address");

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, "Select POS_Address,POS_City,POS_State,POS_ZIP FROM ( select a.MPIN,a.POS_Address,a.POS_City,a.POS_State,a.POS_ZIP from FAC_DEMOG_DUPLICATE_ADDR as a inner join FAC_DEMOG_NTWK_reviewed as r on r.mpin=a.mpin and a.AddrSeq=r.AddrSeq where opi_idx=1 and r.Suppress_id in(0,1) UNION select a.MPIN,a.POS_Address,a.POS_City,a.POS_State,a.POS_ZIP from FAC_DEMOG_NTWK_reviewed as a left join FAC_DEMOG_DUPLICATE_ADDR as r on r.mpin=a.mpin and a.AddrSeq=r.AddrSeq where r.mpin is null and a.Suppress_id in(0,1) ) as t where mpin=" + strMPIN + " order by POS_Address");


                        intAddressCnt = (Int16)dt.Rows.Count;
                        MSExcel.populateTable(dt, strSheetname, 3, 'A');


                        intEndingRowTmp = dt.Rows.Count + 2;
                        //MSExcel.addBorders("A1" + ":B" + (intEndingRowTmp), strSheetname);

                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteLargeExcelTableToWord(strSheetname, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);
                            MSWord.deleteBookmarkComplete(strSheetname);

                        }


                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        strSheetname = "Opioid_Composite_Rates";


                        //strSQL = "select act_opioids,tot_Denom ,CASE WHEN tot_Denom = 0 THEN 0.000023 ELSE convert(float,act_opioids)/convert(float,tot_Denom) END as hosp_rate, CASE WHEN tot_Denom = 0 THEN 0.000023 ELSE exp_opioids/tot_Denom END as nat_rate ,case when Desig_95='High' then 'Higher Rate' when Desig_95='Low' then 'Lower Rate' when Desig_95='Avg' then 'No Different Rate' else 'na' end as [Statistical Difference] from dbo.FAC_SC_OPIOID_RAW_DATA as o inner join (select distinct MPIN from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1)) as r on o.mpin=r.mpin where o.mpin=" + strMPIN + " order by MeasNbr";


                        strSQL = "select case when Desig_95='na' then 'na' else convert(varchar(4),left(CONVERT(varchar(50),CAST(act_opioids AS money),1),len(CONVERT(varchar(50),CAST(act_opioids AS money),1))-3)) end as act ,case when Desig_95='na' then 'na' else convert(varchar(5),left(CONVERT(varchar(50),CAST(tot_Denom AS money),1),len(CONVERT(varchar(50),CAST(tot_Denom AS money),1))-3)) end as denom ,case when Desig_95='na' then 'na' else convert(varchar(10),convert(decimal(9,1),Rate*100))+'%' end as Rate ,NatlAvg_Rate ,case when Desig_95='High' then 'Higher rate' when Desig_95='Low' then 'Lower rate' when Desig_95='Avg' then 'No Different' when Desig_95='Insuff' then 'Not Enough Data' else 'Data Not Available' end as [Statistical Difference] from dbo.FAC_SC_Opioid_meas_MPIN as o inner join dbo.FAC_SC_dim_measures as d on o.Measure_id=d.Measure_id inner join (select distinct MPIN from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1)) as r on o.mpin=r.mpin where o.measure_id between 202 and 204 and o.mpin=" + strMPIN + " order by Sort_id";



                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:G5", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                MSWord.deleteBookmarkComplete(strSheetname);
                            }


                        }



                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        strSheetname = "Opioids_ER_Rates";


                        //strSQL = "select act_opioids,tot_Denom ,CASE WHEN tot_Denom = 0 THEN 0.000023 ELSE convert(float,act_opioids)/convert(float,tot_Denom) END as hosp_rate, CASE WHEN tot_Denom = 0 THEN 0.000023 ELSE exp_opioids/tot_Denom END as nat_rate ,case when Desig_95='High' then 'Higher Rate' when Desig_95='Low' then 'Lower Rate' when Desig_95='Avg' then 'No Different Rate' else 'na' end as [Statistical Difference] from dbo.FAC_SC_OPIOID_RAW_DATA as o inner join (select distinct MPIN from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1)) as r on o.mpin=r.mpin where o.mpin=" + strMPIN + " order by MeasNbr";


                        strSQL = "select case when Desig_95='na' then 'na' else convert(varchar(4),left(CONVERT(varchar(50),CAST(act_opioids AS money),1),len(CONVERT(varchar(50),CAST(act_opioids AS money),1))-3)) end as act ,case when Desig_95='na' then 'na' else convert(varchar(5),left(CONVERT(varchar(50),CAST(tot_Denom AS money),1),len(CONVERT(varchar(50),CAST(tot_Denom AS money),1))-3)) end as denom ,case when Desig_95='na' then 'na' else convert(varchar(10),convert(decimal(9,1),Rate*100))+'%' end as Rate ,NatlAvg_Rate ,case when Desig_95='High' then 'Higher rate' when Desig_95='Low' then 'Lower rate' when Desig_95='Avg' then 'No Different' when Desig_95='Insuff' then 'Not Enough Data' else 'Data Not Available' end as [Statistical Difference] from dbo.FAC_SC_Opioid_meas_MPIN as o inner join dbo.FAC_SC_dim_measures as d on o.Measure_id=d.Measure_id inner join (select distinct MPIN from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1)) as r on o.mpin=r.mpin and o.mpin=" + strMPIN + " where o.measure_id in(2022,2032,2042) order by Sort_id";



                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:G5", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                MSWord.deleteBookmarkComplete(strSheetname);
                            }


                        }

                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        strSheetname = "Opioids_IP_Rates";


                       // strSQL = "select act_opioids,tot_Denom ,CASE WHEN tot_Denom = 0 THEN 0.000023 ELSE convert(float,act_opioids)/convert(float,tot_Denom) END as hosp_rate, CASE WHEN tot_Denom = 0 THEN 0.000023 ELSE exp_opioids/tot_Denom END as nat_rate ,case when Desig_95='High' then 'Higher Rate' when Desig_95='Low' then 'Lower Rate' when Desig_95='Avg' then 'No Different Rate' else 'na' end as [Statistical Difference] from dbo.FAC_SC_OPIOID_RAW_DATA as o inner join (select distinct MPIN from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1)) as r on o.mpin=r.mpin where o.mpin=" + strMPIN + " order by MeasNbr";

                        strSQL = "select case when Desig_95='na' then 'na' else convert(varchar(4),left(CONVERT(varchar(50),CAST(act_opioids AS money),1),len(CONVERT(varchar(50),CAST(act_opioids AS money),1))-3)) end as act ,case when Desig_95='na' then 'na' else convert(varchar(5),left(CONVERT(varchar(50),CAST(tot_Denom AS money),1),len(CONVERT(varchar(50),CAST(tot_Denom AS money),1))-3)) end as denom ,case when Desig_95='na' then 'na' else convert(varchar(10),convert(decimal(9,1),Rate*100))+'%' end as Rate ,NatlAvg_Rate ,case when Desig_95='High' then 'Higher rate' when Desig_95='Low' then 'Lower rate' when Desig_95='Avg' then 'No Different' when Desig_95='Insuff' then 'Not Enough Data' else 'Data Not Available' end as [Statistical Difference] from dbo.FAC_SC_Opioid_meas_MPIN as o inner join dbo.FAC_SC_dim_measures as d on o.Measure_id=d.Measure_id inner join (select distinct MPIN from dbo.FAC_DEMOG_NTWK_reviewed where Suppress_id in(0,1)) as r on o.mpin=r.mpin and o.mpin=" + strMPIN + " where o.measure_id in(2021,2031,2041) order by Sort_id";




                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:G5", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                MSWord.deleteBookmarkComplete(strSheetname);
                            }


                        }



                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if(intAddressCnt > 6 && intAddressCnt < 14)
                            MSWord.addpageBreak("Opioid_Composite_Rates_start");



                        intPageCheck1 = MSWord.getPageNumber("Opioid_Composite_Rates_start");
                        intPageCheck2 = MSWord.getPageNumber("Opioid_Composite_Rates_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak("Opioid_Composite_Rates_start");

                        MSWord.deleteBookmarkComplete("Opioid_Composite_Rates_start");
                        MSWord.deleteBookmarkComplete("Opioid_Composite_Rates_end");

                        intPageCheck1 = MSWord.getPageNumber("Opioids_ER_Rates_start");
                        intPageCheck2 = MSWord.getPageNumber("Opioids_ER_Rates_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak2("Opioids_ER_Rates_start");//NO LINEBREAK!

                        MSWord.deleteBookmarkComplete("Opioids_ER_Rates_start");
                        MSWord.deleteBookmarkComplete("Opioids_ER_Rates_end");


                        intPageCheck1 = MSWord.getPageNumber("Opioids_IP_Rates_start");
                        intPageCheck2 = MSWord.getPageNumber("Opioids_IP_Rates_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak2("Opioids_IP_Rates_start");//NO LINEBREAK!

                        MSWord.deleteBookmarkComplete("Opioids_IP_Rates_start");
                        MSWord.deleteBookmarkComplete("Opioids_IP_Rates_end");




                    }


                    MSWord.appendWordDocument(strWordDocEnd);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //strSheetname = "General Info";




                    //MSExcel.addValueToCell(strSheetname, "B1", strPracticeID);


                    //MSExcel.addValueToCell(strSheetname, "A3", strCorpOwnerName);

                    //MSExcel.addValueToCell(strSheetname, "A4", strStreet);
                    //MSExcel.addValueToCell(strSheetname, "A5", strCity + ", " + strState + " " + strZipCd);

                    //MSExcel.addValueToCell(strSheetname, "B7", strTaxIDLabel);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    //Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Finalizing PDF for '" + strFinalReportFileName + "'");
                    //WRITE WORD TO PDF
                    if (blHasPDF)
                    {

                        MSWord.convertWordToPDF(strFinalReportFileName, "Final\\" + strFinalFolderName, strPEIPath);

                    }

                    //CLOSE EXCEL WB
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "QA" + "\\" + strFinalFolderName);


                    if (blHasWord)
                    {
                        //CLOSE WORD DOCUMENTfor t
                        MSWord.closeWordDocument(strFinalReportFileName, "QA" + "\\" + strFinalFolderName);
                    }

                    //CLOSE DOC END


                    // Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Completed profile for TIN '" + strTaxID + "'");
                    Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Completed profile for MPIN '" + strMPIN + "'");


                    intProfileCnt++;
                    //break;

                }//MAIN LOOP END

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

                Console.Beep();


                Console.ReadLine();


            }
            finally
            {
                Console.WriteLine("Closing Adobe Acrobat Instance...");
                //CLOSE ADOBE APP
                //AdobeAcrobat.closeAcrobat();

                Console.WriteLine("Closing Microsoft Excel Instance...");
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();

                Console.WriteLine("Closing Microsoft Word Instance...");
                //CLOSE WORD APP
                MSWord.closeWordApp();



                foreach (Process Proc in Process.GetProcesses())
                    if (Proc.ProcessName.Equals("EXCEL") || Proc.ProcessName.Equals("WINWORD"))  //Process Excel?
                        Proc.Kill();
            }
        }
    }
}
