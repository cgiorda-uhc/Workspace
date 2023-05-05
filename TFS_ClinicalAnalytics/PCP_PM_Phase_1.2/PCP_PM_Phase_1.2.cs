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

namespace PCP_PM_Phase_1._2
{
    class Program
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

                //Console.WriteLine("Starting Adobe Acrobat Instance...");
                //START ADOBE APP
                //if (blHasPDF)
                //{
                //    AdobeAcrobat.populateAdobeParameters(strReportsPath);
                //    AdobeAcrobat.openAcrobat();
                //}

                //Console.WriteLine("Starting Microsoft Excel Instance...");
                //START EXCEL APP
                MSExcel.populateExcelParameters(blVisibleExcel, blSaveExcel, strReportsPath, strExcelTemplate);
                MSExcel.openExcelApp();

                //Console.WriteLine("Starting Microsoft Word Instance...");
                //START WORD APP
                if (blHasWord)
                {
                    MSWord.populateWordParameters(blVisibleWord, blSaveWord, strReportsPath, strWordTemplate);
                    MSWord.openWordApp();
                }

                object scalarObject = null;
                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;
                string strBookmarkName = null;

                int intProfileCnt = 1;
                int intTotalCnt;


                int intEndingRowTmp;


                string strTaxID;

                string strPracticeID;

                string strTaxIDLabel;

                string strCorpOwnerName;
                string strCorpOwnerNameLC;
                string strStreet;
                string strCity;
                string strState;
                string strZipCd;
                string strRCMO;
                string strRCMO_title;
                string strRCMO_title1;


                bool blHasOpioid = false;
                bool blHasUtilization = false;

                //> than 22
                int intLineBreakCnt = 1;

                string strTinList = "select distinct ad.MPIN  from dbo.PBP_Outl_ph12 as a inner join dbo.PBP_outl_demogr_ph12 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph12 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph12 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and b.PTIGroupID>0 and r.phase_id=2 ";
                // strTinList += " and Opiod_Outl=1 and tot_measures IS NOT NULL";

                // strTinList = "941156581, 680273974, 341768928, 860800150, 760460242, 223487984, 752613493, 131740114, 340714585, 593647972, 340714357, 850105601, 954526112, 465285330, 270473057, 271081647, 752617462,860767800, 311175717, 752613493, 611300608, 611630276";
                //strTinList = "860767800, 311175717, 752613493, 611300608, 611630276";

                //strTinList = "204791426";
                //strTinList = "680273974";

                //strTinList = "593647972";

               // strTinList = "941156581,680273974,341768928,860800150,363149833";
                //strTinList = "204590786, 621529858, 710666911, 202305499, 205339344, 61828814, 201024250, 10758652";
                //strTinList = "10211501, 10594994, 10720785, 10780246, 20547593, 20733769, 20777195,204590786, 621529858, 710666911, 202305499, 205339344, 61828814, 201024250, 10758652, 941156581,680273974,341768928,860800150,363149833";

                //strTinList = "200725457, 200984494, 201024250, 208516866, 208606708, 208839843, 208886987, 204791426, 465006031, 860767101, 630916962, 431715106, 480858702, 631094807, 640913085, 561479712, 271493935, 371140016, 232700908, 231352166, 61469068, 611300608, 630307306, 436003859, 251727721, 300520570";

                if (blIsMasked)
                {



                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";



                }
                else
                {




                    strSQL = "select distinct ad.TaxID,ad.MPIN as PracticeId,ad.Practice_Name,ad.Street,ad.City,ad.State,ad.ZipCd, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name,NULL as Recipient from dbo.PBP_Outl_ph12 as a inner join dbo.PBP_outl_demogr_ph12 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph12 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph12 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and b.PTIGroupID>0 and r.phase_id=2 and ad.MPIN in (" + strTinList + ") and  Special_Handling is not null ";

                }




                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {
                    blHasOpioid = false;
                    blHasUtilization = false;

                    //if (int.Parse(dr["MPIN"].ToString()) < 215108)
                    //{
                    //    continue;
                    //}



                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;





                    strTaxID = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString().Trim() : "VALUE MISSING");

                    strPracticeID = (dr["PracticeId"] != DBNull.Value ? dr["PracticeId"].ToString().Trim() : "VALUE MISSING");

                    if (blIsMasked)
                    {
                        strTaxIDLabel = "123456789" + intProfileCnt;
                    }
                    else
                    {
                        strTaxIDLabel = strTaxID;
                    }


                    strCorpOwnerName = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "VALUE MISSING");

                    strCorpOwnerNameLC = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "VALUE MISSING");

                    strStreet = (dr["Street"] != DBNull.Value ? dr["Street"].ToString().Trim() : "VALUE MISSING");
                    strCity = (dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "VALUE MISSING");
                    strState = (dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "VALUE MISSING");
                    strZipCd = (dr["ZipCd"] != DBNull.Value ? dr["ZipCd"].ToString().Trim() : "VALUE MISSING");
                    strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "VALUE MISSING");
                    strRCMO_title = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "VALUE MISSING");
                    strRCMO_title1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "VALUE MISSING");




                    string strRCMOFirst = null;
                    string strRCMOLast = null;

                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

                    string strFolderName = "";

                    string strBulkPath = "";



                    if (!String.IsNullOrEmpty(strFolderNameTmp))
                    {
                        strFolderNameTmp = "SpecialHandling\\" + strFolderNameTmp;
                    }
                    else
                    {
                        strBulkPath = "\\RegularMailing";
                    }


                    strFolderName = strFolderNameTmp;

                    //if (blHasPDF)
                    //AdobeAcrobat.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    if (blHasWord)
                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));



                    //strFinalReportFileName = strTaxIDLabel + "_" + strCorpOwnerName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_PM_" + strMonthYear;


                    strFinalReportFileName = strPracticeID + "_" + strCorpOwnerName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_PM_" + strMonthYear;


                    

                    //IF THE CURRENT PROFILE ALREADY EXISTS WE DO OR DONT WANT TO OVERWRITE PROFILE (SEE APP.CONFIG)...
                    if (!blOverwriteExisting)
                    {
                        //...CHECK IF PROFILE EXISTS...
                        if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "Final") + strFinalReportFileName + ".pdf"))
                        {
                            Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                            intProfileCnt++;
                            //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                            continue;
                        }
                    }

                    //Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Generating new spreadsheet for '" + strFinalReportFileName + "'");
                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();
                    //ADD SQL TO CURRENT EXCEL FOR QA
                    // MSExcel.addValueToCell("MainSQL", "B1", strSQL);




                    if (blHasWord)
                    {
                        //Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Generating new document for '" + strFinalReportFileName + "'");
                        //OPEN WORD DOCUMENT
                        MSWord.openWordDocument();



                        //Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Replacing placeholder values for '" + strFinalReportFileName + "'");
                        //GENERAL PLACE HOLDERS. WE USE VARIABLES TO REPLACE PLACEHOLDERS WITHIN THE WORD DOC

                        MSWord.wordReplace("<Date>", strDisplayDate);


                        MSWord.wordReplace("<Practice_Name>", strCorpOwnerName);
                        MSWord.wordReplace("<Address 1>", strStreet);
                        MSWord.wordReplace("<City>", strCity);
                        MSWord.wordReplace("<State>", strState);
                        MSWord.wordReplace("<ZIP Code>", strZipCd);



                        MSWord.wordReplace("<RCMO>", strRCMO);
                        MSWord.wordReplace("<RCMO title>", strRCMO_title);





                        if (strRCMO == "Jack S. Weiss, M.D.")
                        {
                            strRCMOFirst = "Jack";
                            strRCMOLast = "Weiss";
                        }
                        else if (strRCMO == "Janice Huckaby, M.D.")
                        {
                            strRCMOFirst = "Janice";
                            strRCMOLast = "Huckaby";
                        }
                        else
                        {
                            strRCMOFirst = "Catherine";
                            strRCMOLast = "Palmier";
                        }


                        MSWord.addSignature(strRCMOFirst, strRCMOLast);

                        MSWord.deleteBookmarkComplete("Signature");


                    }

                    //END WORD DOCUMENT PAGE 1
                    //END WORD DOCUMENT PAGE 1
                    //END WORD DOCUMENT PAGE 1






                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES
                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES
                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES
                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES


                    ///////////////////////////////////////////////////////////////pg 2 - ETG table, graph/////////////////////////////////////////////////////////////////////////////////////////

                    //START EXCEL SHEET: Cardiac_Procs_MCR
                    //START EXCEL SHEET: Cardiac_Procs_MCR
                    //START EXCEL SHEET: Cardiac_Procs_MCR

                    strSheetname = "General Info";




                    MSExcel.addValueToCell(strSheetname, "B3", strPracticeID);


                    MSExcel.addValueToCell(strSheetname, "A5", strCorpOwnerName);

                    MSExcel.addValueToCell(strSheetname, "A6", strStreet);
                    MSExcel.addValueToCell(strSheetname, "A7", strCity + ", " + strState + " " + strZipCd);




                    ///////////////////////////////////////////////////////////////////////////////


                    strSheetname = "MPIN_List";


                    if (blIsMasked)
                    {


                        //strSQL = "select a.MPIN,'Dr.XXXXXXXXXXXXXX' as dr_info   from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN where b.Exclude in(0,4) and attr_cl_rem1>=20 and a.UHN_TIN=" + strTaxID;


                    }
                    else
                    {


                        //strSQL = "select d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN where o.Exclude in(0,5) and d.taxid=" + strTaxID + " order by P_LastName";


                        strSQL = "select d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph12 as d inner join dbo.PBP_outl_ph12 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";
                    }

                    //MASK


                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 3, 'A');


                    MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "{Practice_Name}", strCorpOwnerNameLC);




                    intEndingRowTmp = dt.Rows.Count + 2;
                    MSExcel.addBorders("A1" + ":B" + (intEndingRowTmp), strSheetname);



                    if (blHasWord)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteLargeExcelTableToWord(strSheetname, strSheetname, "A1:B" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);
                        MSWord.deleteBookmarkComplete(strSheetname);

                    }


                    int intRowCnt = dt.Rows.Count;

                    //select TaxID, count(d.MPIN) cnt  from dbo.PBP_outl_demogr_ph12 as d inner join dbo.PBP_outl_ph12 as o on o.MPIN=d.MPIN where Exclude in(0,5)  group by TaxID order by cnt DESC



                    if ((intRowCnt >= 3 && intRowCnt <= 5) || (intRowCnt >= 11 && intRowCnt <= 12) || (intRowCnt >= 14 && intRowCnt <= 15) || (intRowCnt >= 18 && intRowCnt <= 37) || (intRowCnt >= 41 && intRowCnt <= 42) || (intRowCnt == 49) || (intRowCnt >= 51 && intRowCnt <= 52) || (intRowCnt >= 55 && intRowCnt <= 74) || (intRowCnt >= 78 && intRowCnt <= 79) || (intRowCnt >= 86))
                    {
                        //DO NOTHING

                        //MSWord.addLineBreak("Paragraph1Break");
                        //MSWord.addLineBreak("Paragraph2Break");
                        //MSWord.addLineBreak("Paragraph3Break");
                        //MSWord.addLineBreak("Paragraph4Break");

                    }
                    else if ((intRowCnt >= 1 && intRowCnt <= 2) || (intRowCnt >= 38 && intRowCnt <= 40) || (intRowCnt >= 75 && intRowCnt <= 85))
                    {
                        MSWord.addpageBreak2("Paragraph4Break");

                        //MSWord.addLineBreak("Paragraph1Break");
                        //MSWord.addLineBreak("Paragraph2Break");
                        //MSWord.addLineBreak("Paragraph3Break");



                    }
                    else if ((intRowCnt >= 6 && intRowCnt <= 10) || (intRowCnt >= 43 && intRowCnt <= 48) || (intRowCnt >= 80 && intRowCnt <= 77))
                    {
                        MSWord.addpageBreak2("Paragraph3Break");

                        //MSWord.addLineBreak("Paragraph1Break");
                        //MSWord.addLineBreak("Paragraph2Break");
                        //MSWord.addLineBreak("Paragraph4Break");
                    }

                    else if ((intRowCnt == 13) || (intRowCnt == 50))
                    {
                        MSWord.addpageBreak2("Paragraph2Break");


                        //MSWord.addLineBreak("Paragraph1Break");
                        //MSWord.addLineBreak("Paragraph3Break");
                        //MSWord.addLineBreak("Paragraph4Break");

                    }

                    else if ((intRowCnt >= 16 && intRowCnt <= 17) || (intRowCnt >= 53 && intRowCnt <= 54))
                    {
                        MSWord.addpageBreak2("Paragraph1Break");


                        //MSWord.addLineBreak("Paragraph2Break");
                        //MSWord.addLineBreak("Paragraph3Break");
                        //MSWord.addLineBreak("Paragraph4Break");

                    }


                    MSWord.deleteBookmarkComplete("Paragraph1Break");
                    MSWord.deleteBookmarkComplete("Paragraph2Break");
                    MSWord.deleteBookmarkComplete("Paragraph3Break");
                    MSWord.deleteBookmarkComplete("Paragraph4Break");




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




                    strSheetname = "All_meas";




                   // strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_Profile_ph12 as p inner join dbo.PBP_outl_ph12 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph12 as d on o.MPIN=d.MPIN where Exclude in(0,5) and for_page1=1 and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                    strSQL = "select SUM(case when for_page1=1 then for_page1 else 0 end) as tot_meas from dbo.PBP_Profile_ph12 as p inner join dbo.PBP_outl_ph12 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph12 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {


                        blHasUtilization = true;

                        MSExcel.populateTable(dt, strSheetname, 3, 'B');


                        MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "{Practice_Name}", strCorpOwnerName);


                        if (blHasWord)
                        {

                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord("Utiliz_meas_section", strSheetname, "A1:B15", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                        }


                    }




                    //MSWord.deleteBookmarkComplete("Utiliz_meas_section");

                    //full_Utiliz_meas_section

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    strSheetname = "Opioids";



                    //strSQL = "select case when measure_id=28 then Measure_desc+' (reported at MPIN level)' else Measure_desc end, SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_px_ph3 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,case when measure_id=28 then Measure_desc+' (reported at MPIN level)' else Measure_desc end order by sort_ID";

                    strSQL = "select count(*) as opioidCount from dbo.PBP_outl_ph12 as o inner join dbo.PBP_outl_demogr_ph12 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Opiod_Outl=1 and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID";




                    //dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    scalarObject = DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL);
                    if(scalarObject !=  null)
                    {
                        if (int.Parse(scalarObject.ToString()) > 0)
                        {
                            blHasOpioid = true;


                            //MSExcel.populateTable(dt, strSheetname, 3, 'A');


                            MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "{Practice_Name}", strCorpOwnerName);


                            MSExcel.addValueToCell(strSheetname, "B3", scalarObject.ToString());

                            if (blHasWord)
                            {
                                if (int.Parse(scalarObject.ToString()) > 0)
                                {
                                    MSWord.tryCount = 0;
                                    MSWord.pasteExcelTableToWord("opioid_section", strSheetname, "A1:B3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                }
                            }

                        }
    
                    }


                    ///full_opioid_section
                    ///

                    //if (blHasUtilization)
                    //{
                    //    int lineNumber = MSWord.getLineNumber("Proced_meas_brk");
                    //    if (lineNumber > 2)
                    //        MSWord.addpageBreak("Proced_meas_brk");
                    //}


                    //MSWord.deleteBookmarkComplete("Proced_meas_brk");
                    //MSWord.deleteBookmarkComplete("Proced_meas_section");


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strBookmarkName = "appendix";

                    //MSWord.addLineBreak(strBookmarkName);

                    if(blHasOpioid == true && blHasUtilization == true)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg2_all", "A1:C8", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg1_all", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    }
                    else if (blHasOpioid == true)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Opioid_only", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    }
                    else if (blHasUtilization == true)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg1", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    }


                    //}
                    MSWord.deleteBookmarkComplete(strBookmarkName);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    int intLineNumber2 = MSWord.getLineNumber("each_peer");
                    if (intLineNumber2 >= 20)
                    {
                       MSWord.addpageBreak2("each_peer");
                    }

                    MSWord.deleteBookmarkComplete("each_peer");




                    if (blHasUtilization)
                    {
      
                        MSWord.breaksByLimit2("Utiliz_meas_section_break", 20);

                        MSWord.deleteBookmarkComplete("Utiliz_meas_section_break");
                        MSWord.deleteBookmarkComplete("Utiliz_meas_section");
                        MSWord.deleteBookmarkComplete("full_Utiliz_meas_section");
                    }
                    else
                    {
                        MSWord.cleanBookmark("full_Utiliz_meas_section");
                        MSWord.deleteBookmarkComplete("full_Utiliz_meas_section");
                        MSWord.deleteBookmarkComplete("Utiliz_meas_section_break");
                    }
                   

                    if (blHasOpioid)
                    {
                        //LINE BREAK LOGIC HERE
                        //opioid_section_break

                        int limit = 40;
                       if (blHasUtilization)
                            limit = 14;


                        MSWord.breaksByLimit2("opioid_section_break", limit);

                        MSWord.deleteBookmarkComplete("opioid_section_break");
                        MSWord.deleteBookmarkComplete("opioid_section");
                        MSWord.deleteBookmarkComplete("full_opioid_section");
                    }
                    else
                    {

                        MSWord.cleanBookmark("full_opioid_section");
                        MSWord.deleteBookmarkComplete("full_opioid_section");
                        MSWord.deleteBookmarkComplete("opioid_section_break");
                    }


                    ////941156581_PALOALTOMEDICALFOUNDATION_PR_PM_3_2017
                    if (!blHasOpioid)
                    {
                       int intLineNumber = MSWord.getLineNumber("recommended_actions");
                       if (intLineNumber != 1)
                            MSWord.addLineBreak("recommended_actions");
                    }
                    MSWord.deleteBookmarkComplete("recommended_actions");

                    //MSWord.breaksByLimit2("lets_continue", 1);
                    //if(!blHasOpioid)
                        //MSWord.lineBreakIfNot("lets_continue", 1);

                    //int intLineNumber = MSWord.getLineNumber("lets_continue");
                    //if (intLineNumber != 1)
                    //{
                    //    if(intLineNumber < 24)
                    //        MSWord.addLineBreak("lets_continue");
                    //    else
                    //        MSWord.addpageBreak2("lets_continue");
                    //}
         
                   MSWord.deleteBookmarkComplete("lets_continue");




                    //Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Finalizing PDF for '" + strFinalReportFileName + "'");
                    //WRITE WORD TO PDF
                    if (blHasPDF)
                    {

                        MSWord.convertWordToPDF(strFinalReportFileName, "Final", strPEIPath);

                    }

                    //CLOSE EXCEL WB
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "QA" + strBulkPath);


                    if (blHasWord)
                    {
                        //CLOSE WORD DOCUMENTfor t
                        MSWord.closeWordDocument(strFinalReportFileName, "QA" + strBulkPath);
                    }

                    //CLOSE DOC END


                   // Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Completed profile for TIN '" + strTaxID + "'");
                    Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Completed profile for TIN '" + strPracticeID + "'");
                    

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
