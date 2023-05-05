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

namespace PR_profiles_phase2_v1
{
    class PR_PCP_letter_remeasure
    {
        static void Main(string[] args)
        {


            string strSQL = null;

            try
            {


                Console.WriteLine("Wiser Choices Profiles Generator");


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

                //Console.WriteLine("Starting Microsoft Word Instance...");
                //START WORD APP
                if (blHasWord)
                {
                    MSWord.populateWordParameters(blVisibleWord, blSaveWord, strReportsPath, strWordTemplate);
                    MSWord.openWordApp();
                }


                DataTable dtActionableItems = null;
                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;
                string strBookmarkName = null;


                int intProfileCnt = 1;
                int intTotalCnt;


                int intEndingRowTmp;

                bool blHasProcedural = false;
                bool blHasUtilization = false;

                string strMPINList = "2671122,1122204,54399,1052393,2825664,2118189,3536528,2437298,1953413,600434, 809394, 827945, 1241958, 1441156, 1460693, 1465290, 1755481, 1969717, 2013264, 2033714, 2117914, 2270024, 2543962, 2952126, 3664681";


                strMPINList = "2671122,1122204,54399,1052393,2825664,2118189,3536528,2437298,1953413,600434, 809394, 827945, 1241958, 1441156, 1460693, 1465290, 1755481, 1969717, 2013264, 2033714, 2117914, 2270024, 2543962, 2952126, 3664681,250123, 251862, 289584, 373966, 412339, 412695, 2015006, 2043234, 3504456, 3506434, 3648796, 3799803, 3812094, 4722607";


                strMPINList = "451311, 3575652, 3389446, 2934269, 2897464, 2806845, 2583472, 2508929, 2478929, 2286593, 2106472, 1831218, 1803826, 1786987, 1637290, 1396372, 151504, 106485, 3488485, 3346155, 3197392, 3001829, 2968495, 2950135, 2814071, 2329479, 2011498, 1820435, 1354098, 1148863, 799250, 649012, 3775579, 3223748, 3001411, 2810598, 260119, 1856769, 1756103, 1458425, 1435180, 1354372, 1198579, 897452, 892883, 778686, 520569,1586447, 1610548, 433248, 509601, 786793, 66835, 74668, 76452, 100666";


               // strMPINList= "1586447, 1610548, 433248, 509601, 786793, 66835, 74668, 76452, 100666";

                //PEI LETTER MOVE RUN 

                if (blIsMasked)
                {
                    strSQL = "select a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, NDB_Specialty,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XX' as State,'XXXXXXXXX' as zipcd, a.UHN_TIN,'XXXXXXXXX' as  PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2and a.MPIN in (" + strMPINList + ")";
                }
                else
                {

               
                    //strSQL = "select TOP  20 a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, NDB_Specialty,a.Street,a.City,a.State,a.zipcd, a.UHN_TIN,ad.CorpOwnerName as PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.MPIN in (" + strMPINList + ")";



                    strSQL = "select a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, NDB_Specialty,a.Street,a.City,a.State,a.zipcd, a.UHN_TIN,ad.CorpOwnerName as PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2";
                }


                int intActionRowCnt = 0;
                int intLineBreakCnt = 1;

                Int16 intInnerCounter = 0;


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


                    //PROVIDER PLACEHOLDERS. THESE DB DATA COMES FROM MAIN LOOPING SQL ABOVE
                    string LastName = (dr["P_LastName"] != DBNull.Value ? dr["P_LastName"].ToString().Trim() : "NAME MISSING");
                    string FirstName = (dr["P_FirstName"] != DBNull.Value ? dr["P_FirstName"].ToString().Trim() : "NAME MISSING");
                    string phyName = (dr["P_LastName"] != DBNull.Value ? (dr["P_FirstName"].ToString().Trim() + " " + dr["P_LastName"].ToString().Trim()) : "NAME MISSING");
                    string UCaseLastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "NAME MISSING");
                    string UCaseFirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "NAME MISSING");

                    string LCphyName = (dr["LastName"] != DBNull.Value ? (dr["FirstName"].ToString().Trim() + " " + dr["LastName"].ToString().Trim()) : "NAME MISSING");


                    string phyAddress = (dr["Street"] != DBNull.Value ? dr["Street"].ToString().Trim() : "ADDRESS MISSING");
                    string phyCity = (dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "CITY MISSING");
                    string phyState = (dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "STATE MISSING");
                    string phyZip = (dr["zipcd"] != DBNull.Value ? dr["zipcd"].ToString().Trim() : "ZIPCODE MISSING");

                    string ocl = (dr["orig_cl"] != DBNull.Value ? dr["orig_cl"].ToString().Trim() : "ZIPCODE MISSING");
                    string cl_rem1 = (dr["attr_cl_rem1"] != DBNull.Value ? dr["attr_cl_rem1"].ToString().Trim() : "ZIPCODE MISSING");

                    string strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "RCMO MISSING");
                    string strRCMOTitle = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "RCMO TITLE MISSING");
                    string strRCMOTitle1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "RCMO TITLE 1 MISSING");

                    string strMPIN = (dr["MPIN"] != DBNull.Value ? dr["MPIN"].ToString().Trim() : "");
                    string strMPINLabel = null;


                    if (blIsMasked)
                    {
                        strMPINLabel = "123456" + intProfileCnt;
                    }
                    else
                    {
                        strMPINLabel = strMPIN;
                    }
          
                     
                    string strTIN = (dr["UHN_TIN"] != DBNull.Value ? dr["UHN_TIN"].ToString().Trim() : "");

                    string strSpecialty = (dr["NDB_Specialty"] != DBNull.Value ? dr["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");
                    string strProvDegree = (dr["ProvDegree"] != DBNull.Value ? dr["ProvDegree"].ToString().Trim() : "PROV DEGREE MISSING");



                    string strPracticeName = (dr["PracticeName"] != DBNull.Value ? dr["PracticeName"].ToString().Trim() : "NAME MISSING");


                    string strRCMOFirst = null;
                    string strRCMOLast = null;


                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

                    string strFolderName = "";



                    //if (dr["spec_handl_gr_id"].ToString().Equals("0"))
                    //{
                    //    strFolderName = dr["Special_Handling"].ToString() + "\\";
                    //}
                    //else
                    //{
                    //    strFolderName = dr["Special_Handling"] + "\\" + dr["UHN_TIN"] + "\\";
                    //}


                    if(!String.IsNullOrEmpty(strFolderNameTmp))
                    {
                        strFolderNameTmp = "SpecialHandling\\" + strFolderNameTmp + strTIN + "\\";
                    }
                    else
                    {
                        strFolderNameTmp = "RegularMailing\\" + strFolderNameTmp;
                    }


                    //strFolderName = strFolderNameTmp  + strTIN+ "\\";
                    strFolderName = strFolderNameTmp;



                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    if (blHasWord)
                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


                    if (LastName.Contains("-"))
                    {
                        string s = "";
                    }



                    strFinalReportFileName = strMPINLabel + "_" + LastName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_" + phyState + "_" + strMonthYear;



                    //IF THE CURRENT PROFILE ALREADY EXISTS WE DO OR DONT WANT TO OVERWRITE PROFILE (SEE APP.CONFIG)...
                    //if (!blOverwriteExisting)
                    //{
                    //    //...CHECK IF PROFILE EXISTS...
                    //    if (File.Exists(MSWord.strReportsPath + "word\\" + strFinalReportFileName + ".doc"))
                    //    {
                    //        Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                    //        intProfileCnt++;
                    //        //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                    //        continue;
                    //    }
                    //}


                    //if (!blOverwriteExisting)
                    //{
                    //    //...CHECK IF PROFILE EXISTS...
                    //    if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "Final") + strFinalReportFileName + ".pdf"))
                    //    {
                    //        Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                    //        intProfileCnt++;
                    //        //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                    //        continue;
                    //    }
                    //}



                    if (!blOverwriteExisting)
                    {
                        //...CHECK IF PROFILE EXISTS...
                        if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "QA") + "word\\" +strFinalReportFileName + ".doc"))
                        {
                            Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                            intProfileCnt++;
                            //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                            continue;
                        }
                    }






                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();


                    if (blHasWord)
                    {

                        //OPEN WORD DOCUMENT
                        MSWord.openWordDocument();


                        //GENERAL PLACE HOLDERS. WE USE VARIABLES TO REPLACE PLACEHOLDERS WITHIN THE WORD DOC

                        MSWord.wordReplace("<Date>", strDisplayDate);


                        //MSWord.wordReplace("<Physician Name>", FirstName + " " + LastName);
                        MSWord.wordReplace("<Physician Name>", UCaseFirstName + " " + UCaseLastName);




                        MSWord.wordReplace("<Physician Name LC>", FirstName + " " + LastName);


                        MSWord.wordReplace("<P_LastName>", LastName);

                        MSWord.wordReplace("<ProvDegree>", strProvDegree);
                        MSWord.wordReplace("<Provider Specialty>", strSpecialty);
                       // MSWord.wordReplace("<Specialty>", strSpecialtyLongDesc);
                        MSWord.wordReplace("<Address 1>", phyAddress);
                        MSWord.wordReplace("<City>", phyCity);
                        MSWord.wordReplace("<State>", phyState);
                        MSWord.wordReplace("<ZIP Code>", phyZip);


                        MSWord.wordReplace("<attributed patient count>", ocl);
                        MSWord.wordReplace("<remeasure attributed patient count>", cl_rem1);

                        MSWord.wordReplace("<MPIN>", strMPINLabel);


                        MSWord.wordReplace("<Provider MPIN>", strMPINLabel);

                        MSWord.wordReplace("<Provider Name>", FirstName + " " + LastName);


                        MSWord.wordReplace("<Group TINName>", strPracticeName);


                        MSWord.wordReplace("<RCMO>", strRCMO);
                        MSWord.wordReplace("<RCMO title>", strRCMOTitle);
                        //MSWord.wordReplace("<RCMO_title1>", strRCMOTitle1);

                       // MSWord.wordReplace("<attr_clients>", strAttrClients);



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

                    strSheetname = "general info";


                    MSExcel.addValueToCell(strSheetname, "B1", strMPINLabel);
                    
                    
                    MSExcel.addValueToCell(strSheetname, "A3", LCphyName);

                    MSExcel.addValueToCell(strSheetname, "A4", strSpecialty);
                    MSExcel.addValueToCell(strSheetname, "A5", phyAddress);
                    MSExcel.addValueToCell(strSheetname, "A6", phyCity + ", " + phyState + " " + phyZip);



                    MSExcel.addValueToCell(strSheetname, "B8", ocl);
                    MSExcel.addValueToCell(strSheetname, "B9", cl_rem1);





                    MSExcel.addValueToCell(strSheetname, "B11", strTIN);

                    //MSExcel.addValueToCell(strSheetname, "A27", "Dear " + phyName);
                    MSExcel.addValueToCell(strSheetname, "B13", strPracticeName);


                    MSExcel.addValueToCell(strSheetname, "A15", strRCMO);


                    MSExcel.addValueToCell(strSheetname, "A16", strRCMOTitle);

                    MSExcel.addValueToCell(strSheetname, "A17", strRCMOTitle1);


                    ///////////////////////////////////////////////////////////////////////////////


                    strSheetname = "Top_3_meas";
                    strBookmarkName = "Top_3_meas";


                    //strSQL = "select Measure_desc,Unit_Measure,Unit_Measure_add, act_display, curr_data, Trend from dbo.VW_PBP_Rem_Ph1_1 where for_page1=1 and MPIN=" + strMPIN + " order by Hierarchy_Id";
                    //strSQL = "select Measure_desc,Unit_Measure +' '+ ISNULL(Unit_Measure_add,'') as Unit_Measure,act_display, curr_data, Trend from dbo.VW_PBP_Rem_Ph1_1 where for_page1=1 and act_pt_display_orig<>'na' and MPIN=" + strMPIN + " order by Hierarchy_Id";


                   // strSQL = "select Measure_desc,Unit_Measure_rem as Unit_Measure, act_pt_display_orig, Outl_idx_orig, act_pt_display_curr, Outl_idx_curr, Trend from dbo.VW_PBP_Rem_Ph1_1 where for_page1=1 and act_pt_display_orig<>'na' and MPIN=" + strMPIN + " order by Hierarchy_Id";


                    strSQL = "select Measure_desc,Unit_Measure , act_pt_display_orig, Outl_idx_orig, act_pt_display_curr, Outl_idx_curr, Trend from dbo.VW_PBP_Rem_Ph1_1 where for_page1=1 and act_pt_display_orig<>'na' and MPIN=" + strMPIN + " order by Hierarchy_Id";


                    dtActionableItems = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dtActionableItems, strSheetname, 4, 'A');

                    MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_FirstName>", FirstName);
                    MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_LastName>", LastName);


                    intEndingRowTmp = 7;
                    if (dtActionableItems.Rows.Count < 3)
                    {
                        intEndingRowTmp = (4 + dtActionableItems.Rows.Count);
                        MSExcel.deleteRows("A" + intEndingRowTmp + ":G6", strSheetname);
                        MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":G" + (intEndingRowTmp - 1), strSheetname);

                    }


                    if (blHasWord)
                    {
                        if (dtActionableItems.Rows.Count > 0)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:G" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);


                        }


                        //MSWord.cleanBookmark(strBookmarkName + "_whole");
                        MSWord.deleteBookmarkComplete(strBookmarkName);

                    }



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strBookmarkName = "all_meas";

                    strSheetname = "all_meas";


                        
                   // strSQL = "select Measure_desc,Unit_Measure,Unit_Measure_add, act_display, curr_data, Trend from dbo.VW_PBP_Rem_Ph1_1 where MPIN=" + strMPIN + " order by measure_id";
                    //strSQL = "select Measure_desc,Unit_Measure +' '+ ISNULL(Unit_Measure_add,'') as Unit_Measure, act_display, curr_data, Trend from dbo.VW_PBP_Rem_Ph1_1 where MPIN=" + strMPIN + " order by measure_id";
                    strSQL = "select  act_display, curr_data, Trend from dbo.VW_PBP_Rem_Ph1_1 where MPIN=" + strMPIN + " order by measure_id";

                    strSQL = "select act_pt_display_orig, Outl_idx_orig, act_pt_display_curr, Outl_idx_curr, Trend from dbo.VW_PBP_Rem_Ph1_1 where MPIN=" + strMPIN + "  order by measure_id";


                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 4, 'C');


                    MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_FirstName>", FirstName);
                    MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_LastName>", LastName);

                    if (blHasWord)
                    {

                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E16", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);

                        MSWord.deleteBookmarkComplete(strBookmarkName);
                    }
                




                    ///////////////////////////////////////////////////////////////////////////////

                    ////WRITE WORD TO PDF
                    if (blHasPDF)
                    {
                        //AdobeAcrobat.tryCnt = 0;
                        //AdobeAcrobat.createPDF(strFinalReportFileName);
                        //AdobeAcrobat.tryCnt = 0;

                        MSWord.convertWordToPDF(strFinalReportFileName, "Final", strPEIPath);
                    }

                    //CLEANUP SECTION PAGE FOR ORIENTATION
                    //strBookmarkName = "section_break";

                    //if (MSWord.BookmarkExists(strBookmarkName + "_3"))
                    //{
                    //    if (blHasProcedural == false)
                    //        MSWord.cleanBookmark(strBookmarkName + "_3");

                    //    MSWord.deleteBookmarkComplete(strBookmarkName + "_3");
                    //}


                    //if (MSWord.BookmarkExists(strBookmarkName + "_2"))
                    //{
                    //    MSWord.cleanBookmark(strBookmarkName + "_2");
                    //    MSWord.deleteBookmarkComplete(strBookmarkName + "_2");
                    //}

                    //if (MSWord.BookmarkExists(strBookmarkName))
                    //{
                    //    MSWord.cleanBookmark(strBookmarkName);
                    //    MSWord.deleteBookmarkComplete(strBookmarkName);
                    //}




                    //CLOSE EXCEL WB
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "QA");


                    if (blHasWord)
                    {
                        //CLOSE WORD DOCUMENTfor t
                        MSWord.closeWordDocument(strFinalReportFileName, "QA");
                    }

                    //CLOSE DOC END
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
