using System;
using System.Data;
using System.Configuration;
using WCDocumentGenerator;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections;

namespace PCP_Phase_4
{
    class PCP_Phase_4
    {
        static void Main(string[] args)
        {

            string strSQL = null;

            try
            {

                //int intSpecialHandlingMax = 1000000;

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

                ArrayList alSectionTables = new ArrayList();


                int intProfileCnt = 1;
                int intTotalCnt;


                int intProcRowTotal = 0;



                int intEndingRowTmp;

                bool blHasOpioid = false;
                bool blHasUtilization = false;

                string strMPINList = "select a.MPIN from dbo.PBP_Outl_Ph12 as a inner join dbo.PBP_outl_demogr_Ph12 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph12 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_Ph12 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2";


                // strMPINList = "7611,32563,1432767,2013820,2152328,2162744, 2388,2556370,2688629,2155324,2160443,2162314,2563790,2197559,1758897,3148394,410760";
                //strMPINList = "2388,7611,1758897";
                
                //strMPINList = "295491,75700,437472,389732,176751,494798,207765,468967,512404,214566,116768,38702,283148,69145,310520,307646,306992,224087,403094,395872,317856,140457,419712,251107,119209";
                strMPINList = "76514, 10753,9246,97772, 2013820,5817799,5803563,5585086, 3173203,3156177,3145692,3137183,3111358, 2388, 3465,4534, 126444, 3428773, 3361186,3359727";
                strMPINList = "3359727, 3361186, 2013820 ";
                //PEI LETTER MOVE RUN 

                if (blIsMasked)
                {
                    // strSQL = "select a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, Spec_display as NDB_Specialty,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XX' as State,'XXXXXXXXX' as zipcd, a.UHN_TIN,'XXXXXXXXX' as  PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2and a.MPIN in (" + strMPINList + ")";


                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, Spec_display as NDB_Specialty,b.Street,b.City,b.[State],b.zipcd, b.TaxID,ad.Name as PracticeName,Tot_utl_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";
                }
                else
                {

                    //strSQL = "select TOP 80 a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name as PracticeName,Tot_measures,Opiod_Outl, RCMO,RCMO_title,RCMO_title1,NULL As Special_Handling,NULL As Folder_Name, NULL As Folder_Name2, op_clients  from dbo.PBP_Outl_Ph12 as a inner join dbo.PBP_outl_demogr_Ph12 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph12 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_measures,Opiod_Outl, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name, op_clients  from dbo.PBP_Outl_Ph12 as a inner join dbo.PBP_outl_demogr_Ph12 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph12 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_Ph12 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";



                }


                int intActionRowCnt = 0;
                int intLineBreakCnt = 1;

                Int16 intInnerCounter = 0;


                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {



                    alSectionTables = new ArrayList();


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



                    string strTIN = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString().Trim() : "");

                    string strProvDegree = (dr["ProvDegree"] != DBNull.Value ? dr["ProvDegree"].ToString().Trim() : "PROV DEGREE MISSING");
                    string strSpecialty = (dr["NDB_Specialty"] != DBNull.Value ? dr["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");

                    string strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "RCMO MISSING");
                    string strRCMOTitle = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "RCMO TITLE MISSING");
                    string strRCMOTitle1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "RCMO TITLE 1 MISSING");



                    //string attr_clients = (dr["clients"] != DBNull.Value ? dr["clients"].ToString().Trim() : "CLIENTS MISSING");
                    string attr_clients = (dr["clients"] != DBNull.Value ? dr["clients"].ToString().Trim() : "CLIENTS COUNT MISSING");

                    int utilizationCount = (dr["Tot_measures"] != DBNull.Value ? int.Parse(dr["Tot_measures"].ToString()) : 0);
                    int opiodCount = (dr["Opiod_Outl"] != DBNull.Value ? int.Parse(dr["Opiod_Outl"].ToString()) : 0);
                    blHasUtilization = (dr["Tot_measures"] != DBNull.Value ? true : false);
                    blHasOpioid = (dr["Opiod_Outl"] != DBNull.Value ? true : false);



                    string practiceName = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "PRACTICE NAME MISSING");


                    string op_clients = (dr["op_clients"] != DBNull.Value ? dr["op_clients"].ToString().Trim() : "OP CLIENTS COUNT MISSING");

                    //string ocl = (dr["orig_cl"] != DBNull.Value ? dr["orig_cl"].ToString().Trim() : "ZIPCODE MISSING");
                    // string cl_rem1 = (dr["attr_cl_rem1"] != DBNull.Value ? dr["attr_cl_rem1"].ToString().Trim() : "ZIPCODE MISSING");



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









                    //string strPracticeName = (dr["PracticeName"] != DBNull.Value ? dr["PracticeName"].ToString().Trim() : "NAME MISSING");


                    string strRCMOFirst = null;
                    string strRCMOLast = null;



                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");


                    //string strFolderNameTmp2 = (dr["Folder_Name2"] != DBNull.Value ? dr["Folder_Name2"].ToString().Trim() + "\\" : "");


                    string strFolderName = "";



                    //NOT QA UNCOMMENT
                    if (!String.IsNullOrEmpty(strFolderNameTmp))
                    {
                        strFolderNameTmp = "SpecialHandling\\" + strFolderNameTmp + strTIN + "\\";
                    }
                    else
                    {
                        strFolderNameTmp = "RegularMailing\\" + strFolderNameTmp;
                    }


                    strFolderName = strFolderNameTmp;



                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    if (blHasWord)
                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


                    if (LastName.Contains("-"))
                    {
                        string s = "";
                    }



                    strFinalReportFileName = strMPINLabel + "_" + LastName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_" + phyState + "_" + strMonthYear;





                    if (!blOverwriteExisting)
                    {
                        //...CHECK IF PROFILE EXISTS...
                        if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "QA") + "word\\" + strFinalReportFileName + ".doc"))
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

                        MSWord.wordReplace("{$Date}", strDisplayDate);


                        MSWord.wordReplace("{$Physician Name}", UCaseFirstName + " " + UCaseLastName);
                        MSWord.wordReplace("{$Physician Name2}", FirstName + " " + LastName);

                        MSWord.wordReplace("{$UCFirstName}", UCaseFirstName);
                        MSWord.wordReplace("{$UCLastName}", UCaseLastName);



                        MSWord.wordReplace("{$FirstName}", FirstName);
                        MSWord.wordReplace("{$LastName}", LastName);

                        MSWord.wordReplace("{$PracticeName}", practiceName);

                        MSWord.wordReplace("{$Physician Name LC}", FirstName + " " + LastName);

                        MSWord.wordReplace("{$P_LastName}",  LastName);

                        MSWord.wordReplace("{$Prov_Degree}", strProvDegree);
                        MSWord.wordReplace("{$ProvDegree}", strProvDegree);
                        MSWord.wordReplace("{$Specialty}", strSpecialty);
                        // MSWord.wordReplace("<Specialty>", strSpecialtyLongDesc);
                        MSWord.wordReplace("{$Address 1}", phyAddress);
                        MSWord.wordReplace("{$City}", phyCity);
                        MSWord.wordReplace("{$State}", phyState);
                        MSWord.wordReplace("{$ZIP Code}", phyZip);


                        MSWord.wordReplace("{$RCMO}", strRCMO);
                        MSWord.wordReplace("{$RCMO title}", strRCMOTitle);

                        MSWord.wordReplace("{$MPIN}", strMPIN);

                        MSWord.wordReplace("{$practice_name}", UCaseFirstName + " " + UCaseLastName);
                        MSWord.wordReplace("{$Provider Name}", UCaseFirstName + " " + UCaseLastName);
                        MSWord.wordReplace("{$Provider MPIN}", strMPIN);
                        MSWord.wordReplace("{$Group TINName}", strTIN);

                        MSWord.wordReplace("{$attrib_clients}", attr_clients);

                        //MSWord.wordReplace("<RCMO_title1>", strRCMOTitle1);

                        // MSWord.wordReplace("<attr_clients>", strAttrClients);

                        MSWord.wordReplace("{$patients}", op_clients);

                        if (strRCMO == "Jack S. Weiss, M.D.")
                        {
                            strRCMOFirst = "Jack";
                            strRCMOLast = "Weiss";
                        }
                        else
                        {
                            strRCMOFirst = "Janice";
                            strRCMOLast = "Huckaby";
                        }
        


                        MSWord.addSignature(strRCMOFirst, strRCMOLast);

                        MSWord.deleteBookmarkComplete("signature");




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


                    MSExcel.addValueToCell(strSheetname, "B2", strMPINLabel);
                    MSExcel.addValueToCell(strSheetname, "B3", strTIN);


                    MSExcel.addValueToCell(strSheetname, "A4", LCphyName);

                    MSExcel.addValueToCell(strSheetname, "A5", strSpecialty);
                    MSExcel.addValueToCell(strSheetname, "A6", phyAddress);
                    MSExcel.addValueToCell(strSheetname, "A7", phyCity + ", " + phyState + " " + phyZip);



                    MSExcel.addValueToCell(strSheetname, "B9", attr_clients);
                    //MSExcel.addValueToCell(strSheetname, "B9", cl_rem1);

                   // MSExcel.addValueToCell(strSheetname, "B17", practiceName);

                    //MSExcel.addValueToCell(strSheetname, "A27", "Dear " + phyName);
                    //MSExcel.addValueToCell(strSheetname, "B13", strPracticeName);


                    MSExcel.addValueToCell(strSheetname, "A11", strRCMO);


                    MSExcel.addValueToCell(strSheetname, "A12", strRCMOTitle);

                    MSExcel.addValueToCell(strSheetname, "A13", strRCMOTitle1);


                    MSExcel.addValueToCell(strSheetname, "B10", op_clients);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasUtilization)
                    {
                        //WE DONT NEED THIS FOR DELETING THIS SECTION SO GET RID OF THEM
                        MSWord.deleteBookmarkComplete("utilization_section");

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strBookmarkName = "utilization_table";

                        strSheetname = "all_meas"; //CHECK!!!

                        //strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_Ph3 as a where MPIN=" + strMPIN + " order by sort_id";
                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant'  from dbo.PBP_Profile_Ph12 as a where measure_id not in(14,15) and MPIN=" + strMPIN + " order by sort_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            //alSectionTables.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');


                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", LastName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F15", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);


                            }
                        }

                        MSWord.deleteBookmarkComplete(strBookmarkName);
                    }

                    else
                    {

                        MSWord.cleanBookmark("utilization_section");
                        //MSWord.cleanBookmark("utilization_table");

                        MSWord.deleteBookmarkComplete("utilization_section");
                        //MSWord.deleteBookmarkComplete("utilization_table");


                    }



                if (blHasOpioid)
                {

                    //WE DONT NEED THIS FOR DELETING THIS SECTION SO GET RID OF THEM
                    MSWord.deleteBookmarkComplete("opioid_section");

                    strBookmarkName = "opioid_table";

                    strSheetname = "opioids"; //CHECK!!!!


                    //strSQL = "select Category, Patient_Count, Visit_Count as [Script Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=35 and attr_mpin=" + strMPIN + " order by Catg_order";
                    strSQL = "Select act_display, expected_display, var_display,signif from dbo.PBP_Profile_Px_Ph12 as a where measure_id=38 and a.MPIN=" + strMPIN;

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {

                            //alSectionTables.Add(strSheetname);


                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                        MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", FirstName);
                        MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", LastName);

                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                        }
                    }


                    MSWord.deleteBookmarkComplete(strBookmarkName);
                }
                else
                {

                    MSWord.cleanBookmark("opioid_section");

                    MSWord.deleteBookmarkComplete("opioid_section");
                    //MSWord.deleteBookmarkComplete("opioid_only_description");

                }



                    strBookmarkName = "table_details";

                    if (blHasOpioid)
                    {

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "opioids_det"; //CHECK !!!

                        strSQL = "Select act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph12 as a where measure_id between 40 and 42 and a.MPIN=" + strMPIN + " order by Hierarchy_Id";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            alSectionTables.Add(strSheetname);


                            MSExcel.populateTable(dt, strSheetname, 4, 'C');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", LastName);

                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }
                    }




                    strSQL = "select Measure_desc from dbo.PBP_Profile_Ph12 as a where measure_id not in(14,15) and MPIN=" + strMPIN + " and outl_idx <> 0 order by sort_Id  DESC";
                    dtActionableItems = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    foreach (DataRow row in dtActionableItems.Rows)
                    {


                        if (row["Measure_desc"].ToString().Trim().ToLower().Equals("your tier 3 pharmacy utilization"))
                        {

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "PCP_Tier3_sum_det"; //CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count as [Script Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=12 and attr_mpin=" + strMPIN + " order by Catg_order";

                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=12 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);


                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("tier 3 pharmacy utilization by all other physicians seeing your patients"))
                        {

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "Spec_Tier3_sum_det"; //CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count as [Script Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=12 and attr_mpin=" + strMPIN + " order by Catg_order";

                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=13 and attr_mpin=" + strMPIN + "order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);


                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("advanced imaging utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "Adv_img_sum_det"; //CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 8;
                                if (dt.Rows.Count < 4)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("non-advanced imaging utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "NAI_sum_det"; //CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=36 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("level 4 & 5 visit rate"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "Level4_5_sum_det";//CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier utilization rate"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "Mod_sum_det";//CHECK !!!


                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("out of network (oon)"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "OON_sum_det";//CHECK !!!

                            strSQL = "select Category, Patient_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID = 9 and attr_mpin =" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":C9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":C" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:C" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("out of network (oon) lab utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "OON_lab_sum_det"; //CHECK !!!


                            //strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=16 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=16 and attr_mpin=" + strMPIN + " order by Catg_order";



                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("specialty physician utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "Spec_PCP_sum_det"; //CHECK !!!


                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=10 and attr_mpin=" + strMPIN + " order by Catg_order";



                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }

                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "Spec_PCP_sum_det2"; //CHECK !!!

                            strSQL = "select Measure_desc,Unit_Measure,act_display,expected_display,var_display from dbo.PBP_Profile_Ph12 where measure_id in(14,15) and MPIN=" + strMPIN + " order by sort_Id";



                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {

                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 6;
                                if (dt.Rows.Count < 2)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":E5", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                                }


                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }



                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("laboratory/pathology utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "LabPath_sum_det"; //CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }

                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }

                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("average length of stay (alos)"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "ALOS_sum_det";//CHECK !!!

                            //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);
                                }

                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("inpatient admission utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "IP_sum_det";//CHECK !!!

                            //strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";
                            strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }

                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }
                            }
                        }
                        else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("emergency department (ed) utilization"))
                        {
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            strSheetname = "ED_sum_det";//CHECK!!!

                            //strSQL = "SELECT Category, Patient_Count, Visit_Count, Pct_Cost FROM dbo.PBP_act_ph3 WHERE Measure_ID=1 AND attr_mpin=" + strMPIN + " ORDER BY Catg_order";
                            strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph12 where Measure_ID=1 and attr_mpin=" + strMPIN + " order by Catg_order";

                            dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                            if (dt.Rows.Count > 0)
                            {
                                alSectionTables.Add(strSheetname);

                                MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                intEndingRowTmp = 10;
                                if (dt.Rows.Count < 6)
                                {
                                    intEndingRowTmp = (4 + dt.Rows.Count);
                                    MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
                                    MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

                                }

                                if (blHasWord)
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        MSWord.tryCount = 0;
                                        MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                                    }
                                }

                            }
                        }
                    }//ACTION ITEMS FOR LOOP END

                    MSWord.deleteBookmarkComplete(strBookmarkName);


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strBookmarkName = "appendix";

                   
                    if (blHasUtilization && blHasOpioid)
                    {
                        //MSWord.tryCount = 0;
                        //MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg2_all", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: !blHasOpioid);
                        //MSWord.tryCount = 0;
                        //MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg1_all", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false );
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg2_all", "A1:C8", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg1_all", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false);


                    }
                    else if (blHasUtilization)
                    {
                        //MSWord.tryCount = 0;
                        //MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg2_all", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: !blHasOpioid);
                        //MSWord.tryCount = 0;
                        //MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg1_all", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false );
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg1", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false);


                    }
                    else if (blHasOpioid)
                    {
                        //MSWord.tryCount = 0;
                        //MSWord.pasteExcelTableToWord(strBookmarkName, "Opioid_only", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Opioid_only", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true, blAddBookmark: false);


                    }



                    MSWord.deleteBookmarkComplete(strBookmarkName);


                    //MSWord.addLineBreak((blHasOpioid == true ? "Opioid_only" : "Util_pg2_all"));
                    //MSWord.deleteBookmarkComplete((blHasOpioid == true ? "Opioid_only" : "Util_pg2_all"));

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    if (blHasUtilization && blHasOpioid)
                    {
                        MSWord.addBreak("opioid_breakPoint");
                        MSWord.addLineBreak("table_details_breakpoint");
                    }
                    else if (blHasOpioid)
                    {
                        MSWord.addLineBreak("table_details_breakpoint");
                    }
                    else if (blHasUtilization)
                    {
                        MSWord.addBreak("table_details_breakpoint");
                    }

                    MSWord.deleteBookmarkComplete("table_details_breakpoint");
                    MSWord.deleteBookmarkComplete("opioid_breakPoint");



                    //ALWAYS RUN LAST!!!! FINAL BREAKING
                    if (blHasUtilization || blHasOpioid) //ALWAYS TRUE
                    {
                        processBreaks(alSectionTables, 1);
                        processTopBreaks(alSectionTables, 1);
                        //DELETE BOOKMARKS
                        //for (int i = 0; i < alSectionTables.Count; i++)
                        //{
                        //    MSWord.deleteBookmarkComplete(alSectionTables[i].ToString());
                        //}
                    }








                    ////WRITE WORD TO PDF
                    if (blHasPDF)
                    {

                        MSWord.convertWordToPDF(strFinalReportFileName, "Final", strPEIPath);
                    }

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


        private static void processBreaks(ArrayList al, int iArrayType)
        {

            if (al.Count > 0)
            {
                al.Reverse();
                int intLineNumber = 0;
                for (int i = 0; i < al.Count; i++)
                {

                    intLineNumber = MSWord.getLineNumber(al[i].ToString());


                    if ((i + 1) < al.Count)
                    {
                        if ((iArrayType == 1 && intLineNumber < 25) || (iArrayType == 2 && intLineNumber <= 7) || (iArrayType == 3))
                            MSWord.addLineBreak(al[i].ToString());
                    }




                }

            }
        }

        private static void processTopBreaks(ArrayList al, int iArrayType)
        {
            string s = "";

           if (al.Count > 0)
            {
                //al.Reverse();
                string strLastBookMark = null;
                int intLineNumber = 0;
                for (int i = 0; i < al.Count; i++)
                {

                    intLineNumber = MSWord.getLineNumber(al[i].ToString());

                    if (intLineNumber == 1)
                    {
                        while (intLineNumber == 1 && strLastBookMark != null)
                        {
                            intLineNumber = MSWord.getLineNumber(al[i].ToString());


                            if (intLineNumber == 1)
                            {
                                MSWord.addLineBreak(strLastBookMark);
                            }
                        }
                    }

                    strLastBookMark = al[i].ToString();
                }


                //DELETE BOOKMARKS
                for (int i = 0; i < al.Count; i++)
                {
                    MSWord.deleteBookmarkComplete(al[i].ToString());
                }


            }
        }


    }
}
