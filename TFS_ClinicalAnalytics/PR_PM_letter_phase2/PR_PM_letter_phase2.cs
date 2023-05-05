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

namespace PR_PM_letter_phase2
{
    class PR_PM_letter_phase2
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


                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;


                int intProfileCnt = 1;
                int intTotalCnt;


                int intEndingRowTmp;


                string strTaxID;

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



                bool blHasProcedural = false;
                bool blHasUtilization = false;









                //strSQL = "select distinct ad.TaxID,Name,ad.Street,ad.City,ad.State,ad.ZipCd, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and r.phase_id=2";


                string strTinList = "10331554,60646973,61081232,61330879,61483728,112569522";


                //strTinList = "61469068, 941156581,340714585,223537011,231352166,752613493,390452970,232730785,521467441,363479824,421442443,540886561,352120905 ";

                strTinList = "61081232,61330879,112569522";
                //strTinList = "112569522";




                strTinList = "582345264,640503682,522220700,201079808,611276316,742845471,742958277,931271596,954829020,592579846,542129332";

                strTinList = "10331554, 42704683, 341768928, 431696710, 582345264, 751364675";
                // strTinList = "61330879";

                strTinList = "10350600, 200444683, 201178891, 201357375,200444683, 201178891, 201357375, 201585810, 203428324, 203522125, 260834681, 261356310, 262349105, 262991309, 264074239, 264596860, 271695094, 271789460";

                //strTinList = "10350600, 200444683, 201178891, 201357375";

                strTinList = "570359174, 592825211, 132655001, 260609255, 462602656, 462602656, 931271596, 132655001, 250815795, 61483728, 450537391, 137059154, 542129332, 582483738, 582483738";




                if (blIsMasked)
                {
                    strSQL = "select distinct ad.TaxID,'XXXXXXXXX' as LC_Name,'XXXXXXX' as UC_Name,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XX' as State,'XXXXX' as ZipCd, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and r.phase_id=2 and ad.TaxID in (" + strTinList + ")";
                }
                else
                {

                    //strSQL = "select distinct ad.TaxID,P_Name as LC_Name,Name as UC_Name,ad.Street,ad.City,ad.State,ad.ZipCd, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and r.phase_id=2 and ad.TaxID in (" + strTinList + ")";



                    strSQL = "select distinct ad.TaxID,P_Name as LC_Name,Name as UC_Name, ad.Street,ad.City,ad.State,ad.ZipCd, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and r.phase_id=2";
                }









                    DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {


                    //if (int.Parse(dr["MPIN"].ToString()) < 215108)
                    //{
                    //    continue;
                    //}



                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;





                    strTaxID = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString().Trim() : "VALUE MISSING");



                    if (blIsMasked)
                    {
                        strTaxIDLabel = "123456789" + intProfileCnt;
                    }
                    else
                    {
                        strTaxIDLabel = strTaxID;
                    }


                    strCorpOwnerName = (dr["UC_Name"] != DBNull.Value ? dr["UC_Name"].ToString().Trim() : "VALUE MISSING");

                    strCorpOwnerNameLC = (dr["LC_Name"] != DBNull.Value ? dr["LC_Name"].ToString().Trim() : "VALUE MISSING");

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



                    strFinalReportFileName = strTaxIDLabel + "_" + strCorpOwnerName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_PM_" + strMonthYear;



                    //IF THE CURRENT PROFILE ALREADY EXISTS WE DO OR DONT WANT TO OVERWRITE PROFILE (SEE APP.CONFIG)...
                    if (!blOverwriteExisting)
                    {
                        //...CHECK IF PROFILE EXISTS...
                        if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "Final")  + strFinalReportFileName + ".pdf"))
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




                    MSExcel.addValueToCell(strSheetname, "B1", strTaxIDLabel);


                    MSExcel.addValueToCell(strSheetname, "A3", strCorpOwnerName);

                    MSExcel.addValueToCell(strSheetname, "A4", strStreet);
                    MSExcel.addValueToCell(strSheetname, "A5", strCity + ", " + strState + " " + strZipCd);




                    ///////////////////////////////////////////////////////////////////////////////


                    strSheetname = "MPIN_List";


                    if (blIsMasked)
                    {
                        strSQL = "SELECT 'XXXXXX' as MPIN, 'Dr.XXXXXXXXXXXXXX' as dr_info FROM dbo.PBP_outl_demogr_ph2 as d inner join dbo.PBP_outl_ph2 as o on o.MPIN=d.MPIN WHERE o.Exclude in(0,5) AND d.taxid=" + strTaxID + " ORDER BY P_LastName";
                    }
                    else
                    {
                        strSQL = "SELECT d.MPIN, 'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info FROM dbo.PBP_outl_demogr_ph2 as d inner join dbo.PBP_outl_ph2 as o on o.MPIN=d.MPIN WHERE o.Exclude in(0,5) AND d.taxid=" + strTaxID + " ORDER BY P_LastName";
                    }

                    //MASK
                    //strSQL = "SELECT d.MPIN, 'Dr.XXXXXXXXXXXXXX' as dr_info FROM dbo.PBP_outl_demogr_ph2 as d inner join dbo.PBP_outl_ph2 as o on o.MPIN=d.MPIN WHERE o.Exclude in(0,5) AND d.taxid=" + strTaxID + " ORDER BY P_LastName";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 3, 'A');


                    MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Physician Group Name>", strCorpOwnerNameLC);



                    //MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<P_FirstName>", FirstName);
                    //MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<P_LastName>", LastName);


                    intEndingRowTmp = dt.Rows.Count + 2;
                    MSExcel.addBorders("A1" + ":B" + (intEndingRowTmp), strSheetname);
                    //if (dt.Rows.Count < 3)
                    //{
                    //    intEndingRowTmp = (3 + dt.Rows.Count);
                    //    MSExcel.deleteRows("A" + intEndingRowTmp + ":E5", strSheetname);
                    //    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                    //}


                    if (blHasWord)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteLargeExcelTableToWord(strSheetname, strSheetname, "A1:B" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);

                    }

                    MSWord.deleteBookmarkComplete(strSheetname);




                    ///////////////////////////////////////////////////////////////////////////////


                    strSheetname = "Utiliz_meas";


                    strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph2 as d inner join dbo.PBP_outl_ph2 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_Ph2 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,Measure_desc order by sort_ID";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);


                    if (dt.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dt, strSheetname, 3, 'B');

                        MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Physician Group Name>", strCorpOwnerNameLC);

                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:B13", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, 1);
                        }

                        MSWord.deleteBookmarkComplete(strSheetname);
                        MSWord.deleteBookmarkComplete(strSheetname + "_whole");
                        blHasUtilization = true;

                    }
                    else
                    {
                        MSWord.cleanBookmark(strSheetname + "_whole");
                        MSWord.deleteBookmarkComplete(strSheetname + "_whole");
                        blHasUtilization = false;
                    }




                    ///////////////////////////////////////////////////////////////////////////////


                    strSheetname = "Proced_meas";


                    strSQL = "select  sum(case when contr=1 then 1 else 0 end) as MPINs from dbo.PBP_Profile_Px_Ph2 as a where Tin=" + strTaxID + " group by Measure_desc,Measure_ID order by Measure_ID";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);




                    if (dt.Rows.Count > 0)
                    {
                        MSExcel.populateTable(dt, strSheetname, 3, 'B');

                        MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Physician Group Name>", strCorpOwnerNameLC);

                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:B5", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, 1);
                        }

                        MSWord.deleteBookmarkComplete(strSheetname);
                        MSWord.deleteBookmarkComplete(strSheetname + "_whole");
                        blHasProcedural = true;

                    }
                    else
                    {
                        MSWord.cleanBookmark(strSheetname + "_whole");
                        MSWord.deleteBookmarkComplete(strSheetname + "_whole");
                        blHasProcedural = false;
                    }



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strSheetname = "appendix";



                    if (blHasProcedural && blHasUtilization)
                    {

                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strSheetname, "Util_Proc_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, 1, true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strSheetname, "Util_Proc_pg1", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, 1, true);
                    }
                    else if (blHasProcedural)
                    {

                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strSheetname, "Proc_only", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, 1, true);
                    }
                    else if (blHasUtilization)
                    {

                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strSheetname, "Util_only", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, 1, true);

                    }

                    MSWord.deleteBookmarkComplete(strSheetname);






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


                    Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Completed profile for TIN '" + strTaxID + "'");



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
