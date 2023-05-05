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

namespace PR_PM_Spec_Profile_Phase3
{
    class PR_PM_Spec_Profile_Phase3
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
                string strBookmarkName = null;

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

                //> than 22
                int intLineBreakCnt = 1;

                string strTinList = "30417049,61469068,131623978,131740114,340714585,941156581,340714585,860800150";
                strTinList = "941156581,134239971,752547668,522107258,582078064,943192446";
                strTinList = "931097258,680273974,340714585,135562308, 390806261,542029018,860800150";


                strTinList = "941156581, 340714585, 942854057, 860800150,131623978, 205392766,251727721";
                //strTinList = "611630276,900733283,311351965,581973570,541124769,271370967,486341644,621529858,112891904,431912860"; //4-12
                strTinList = "941156581, 340714585, 942854057, 860800150,131623978, 205392766,251727721,611630276,900733283,311351965,581973570,541124769,271370967,486341644,621529858,112891904,431912860,134239971,271370967,351972308,240802108,582234927,541595397,371206525,953942828";



                strTinList = "941156581,340714585,391678306,752613493,680273974,591198552,371140016,391128616,362222696,311351965,910851599,205114071,561479712,232700908,262909414,453791448,421645215,752845381,364004749,30179418,453023019,311351965,731477155,330106028,592262342, 453023019,330106028,416011702 ";


                //strTinList = "416011702,340714585";

                //strTinList = "30179418,453023019,311351965,731477155,330106028,592262342";

                //strTinList = "311351965";

                //strTinList = "453023019,330106028";
                //strTinList = "330106028";

                //strTinList = "262909414";

                strTinList = "205506958,205630556,205545820,205529258,205526313,205485346,205476847,205476000,721439296,205607531,205577233,205565623,205563893,205532826,205512734,250815795,232865181,231352630,42743894,42697983,30179418,521749666,205613593,205521741,205515096,205503482,205498259,205493250,205477488,205461242,390816848";

                if (blIsMasked)
                {



                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";


                    strSQL = "select distinct b.TaxID,b.Name as PracticeName, b.Street,b.City,b.[State],b.zipcd, b.TaxID,b.Name as PracticeName, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as d on a.MPIN=d.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as b on b.TaxID=d.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and b.TaxID in (" + strTinList + ")";


                }
                else
                {





                   // strSQL = "select distinct  TOP 100 b.TaxID,b.Name as PracticeName, b.Street,b.City,b.[State],b.zipcd, b.TaxID,b.Name as PracticeName, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as d on a.MPIN=d.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as b on b.TaxID=d.TaxID inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and b.TaxID in (" + strTinList + ")";



                    strSQL = "select  distinct b.TaxID,b.Name as PracticeName, b.Street,b.City,b.[State],b.zipcd, b.TaxID,b.Name as PracticeName, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as d on a.MPIN=d.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as b on b.TaxID=d.TaxID inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2";

                }




                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {
                    blHasProcedural = false;
                    blHasUtilization = false;

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


                    strCorpOwnerName = (dr["PracticeName"] != DBNull.Value ? dr["PracticeName"].ToString().Trim() : "VALUE MISSING");

                    strCorpOwnerNameLC = (dr["PracticeName"] != DBNull.Value ? dr["PracticeName"].ToString().Trim() : "VALUE MISSING");

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




                    MSExcel.addValueToCell(strSheetname, "B1", strTaxIDLabel);


                    MSExcel.addValueToCell(strSheetname, "A3", strCorpOwnerName);

                    MSExcel.addValueToCell(strSheetname, "A4", strStreet);
                    MSExcel.addValueToCell(strSheetname, "A5", strCity + ", " + strState + " " + strZipCd);




                    ///////////////////////////////////////////////////////////////////////////////


                    strSheetname = "MPIN_List";


                    if (blIsMasked)
                    {


                        //strSQL = "select a.MPIN,'Dr.XXXXXXXXXXXXXX' as dr_info   from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN where b.Exclude in(0,4) and attr_cl_rem1>=20 and a.UHN_TIN=" + strTaxID;

                        strSQL = "select d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN where o.Exclude in(0,5) and d.taxid=" + strTaxID + " order by P_LastName";

                    }
                    else
                    {

                        //strSQL = "select d.taxid,d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN where o.Exclude in(0,5) and d.taxid=134010372 order by P_LastName";
                        //strSQL = "select a.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info  from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN where b.Exclude in(0,4) and attr_cl_rem1>=20 and a.UHN_TIN=" + strTaxID + " order by P_LastName";

                        //strSQL = "select d.taxid,d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN where o.Exclude in(0,5) and d.taxid=" + strTaxID + " order by P_LastName";


                        strSQL = "select d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN where o.Exclude in(0,5) and d.taxid=" + strTaxID + " order by P_LastName";
                    }

                    //MASK
                    //strSQL = "SELECT d.MPIN, 'Dr.XXXXXXXXXXXXXX' as dr_info FROM dbo.PBP_outl_demogr_ph2 as d inner join dbo.PBP_outl_ph2 as o on o.MPIN=d.MPIN WHERE o.Exclude in(0,5) AND d.taxid=" + strTaxID + " ORDER BY P_LastName";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 3, 'A');


                    MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Practice_Name>", strCorpOwnerNameLC);



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
                        MSWord.deleteBookmarkComplete(strSheetname);

                    }


                    int intRowCnt = dt.Rows.Count;






                    if ((intRowCnt == 1 ) || (intRowCnt == 5) || (intRowCnt >= 10 && intRowCnt <= 11) || (intRowCnt >= 14 && intRowCnt <= 35) || (intRowCnt >= 39 && intRowCnt <= 40) || (intRowCnt == 45) || (intRowCnt >= 48 && intRowCnt <= 69) || (intRowCnt >= 73 && intRowCnt <= 74) || (intRowCnt == 79) || (intRowCnt >= 82 && intRowCnt <= 103) || (intRowCnt >= 107 && intRowCnt <= 108) || (intRowCnt == 113) || (intRowCnt >= 116 && intRowCnt <= 137) || (intRowCnt >= 141 && intRowCnt <= 142) || (intRowCnt == 147) || (intRowCnt >= 150 && intRowCnt != 285))
                    {
                        //DO NOTHING

                    }
                    else if ((intRowCnt >= 2 && intRowCnt <= 4) || (intRowCnt >= 36 && intRowCnt <= 38) || (intRowCnt >= 70 && intRowCnt <= 72) || (intRowCnt >= 104 && intRowCnt <= 106) || (intRowCnt >= 138 && intRowCnt <= 140))
                    {
                        MSWord.addpageBreak("Paragraph3Break");
                    }

                    else if ((intRowCnt >= 6 && intRowCnt <= 9) || (intRowCnt >= 41 && intRowCnt <= 44) || (intRowCnt >= 75 && intRowCnt <= 78) || (intRowCnt >= 109 && intRowCnt <= 112) || (intRowCnt >= 143 && intRowCnt <= 146))
                    {
                        MSWord.addpageBreak("Paragraph2Break");
                    }

                    else if ((intRowCnt >= 12 && intRowCnt <= 13) || (intRowCnt >= 46 && intRowCnt <= 47) || (intRowCnt >= 80 && intRowCnt <= 81) || (intRowCnt >= 114 && intRowCnt <= 115) || (intRowCnt >= 148 && intRowCnt <= 149) || (intRowCnt  == 285))
                    {
                        MSWord.addpageBreak("Paragraph1Break");
                    }


                    //if ((intRowCnt >= 1 && intRowCnt <= 2) || (intRowCnt >= 6 && intRowCnt <= 7) || (intRowCnt >= 12 && intRowCnt <= 13) || (intRowCnt >= 16 && intRowCnt <= 24) || (intRowCnt >= 43 && intRowCnt <= 44) || (intRowCnt >= 49 && intRowCnt <= 50) || (intRowCnt >= 53 && intRowCnt <= 61) || (intRowCnt >= 80 && intRowCnt <= 81) || (intRowCnt >= 86 && intRowCnt <= 87) || (intRowCnt >= 90 && intRowCnt <= 98) || (intRowCnt >= 117))
                    //{
                    //    //DO NOTHING

                    //}
                    //else if ((intRowCnt >= 3 && intRowCnt <= 5) || (intRowCnt >= 25 && intRowCnt <= 42) || (intRowCnt >= 62 && intRowCnt <= 79) || (intRowCnt >= 99 && intRowCnt <= 116))
                    //{
                    //    MSWord.addpageBreak("Paragraph3Break");
                    //}

                    //else if ((intRowCnt >= 8 && intRowCnt <= 11) || (intRowCnt >= 45 && intRowCnt <= 48) || (intRowCnt >= 82 && intRowCnt <= 85))
                    //{
                    //    MSWord.addpageBreak("Paragraph2Break");
                    //}

                    //else if ((intRowCnt >= 14 && intRowCnt <= 15) || (intRowCnt >= 51 && intRowCnt <= 52) || (intRowCnt >= 88 && intRowCnt <= 89))
                    //{
                    //    MSWord.addpageBreak("Paragraph1Break");
                    //}


                    MSWord.deleteBookmarkComplete("Paragraph1Break");
                    MSWord.deleteBookmarkComplete("Paragraph2Break");
                    MSWord.deleteBookmarkComplete("Paragraph3Break");





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////




                    strSheetname = "Utiliz_meas";

                    //strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_ph3 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,Measure_desc order by sort_ID " ;

                    strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_ph3 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,Measure_desc order by sort_ID";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {


                        blHasUtilization = true;

                        MSExcel.populateTable(dt, strSheetname, 3, 'B');


                        MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Practice_Name>", strCorpOwnerName);


                        if (blHasWord)
                        {

                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:B20", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                        }




                        MSWord.deleteBookmarkComplete(strSheetname);

                    }
                    else
                    {
                        MSWord.cleanBookmark("Utiliz_meas_section");

                    }

                    MSWord.deleteBookmarkComplete("Utiliz_meas_section");

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    strSheetname = "Proced_meas";

                    //strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_px_ph3 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,Measure_desc order by sort_ID";

                    // strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_px_ph3 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,Measure_desc order by sort_ID";

                    strSQL = "select case when measure_id=28 then Measure_desc+' (reported at MPIN level)' else Measure_desc end, SUM(Outl_idx) as tot_meas from dbo.PBP_outl_demogr_ph3 as d inner join dbo.PBP_outl_ph3 as o on o.MPIN=d.MPIN inner join dbo.PBP_Profile_px_ph3 as p on p.MPIN=o.MPIN where o.Exclude in(0,5) and taxid=" + strTaxID + " group by taxid,sort_ID,case when measure_id=28 then Measure_desc+' (reported at MPIN level)' else Measure_desc end order by sort_ID";




                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {
                        blHasProcedural = true;


                        MSExcel.populateTable(dt, strSheetname, 3, 'A');


                        MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Practice_Name>", strCorpOwnerName);





                        intEndingRowTmp = 11;
                        if (dt.Rows.Count < 8)
                        {
                            intEndingRowTmp = (3 + dt.Rows.Count);
                            MSExcel.deleteRows("A" + intEndingRowTmp + ":B10", strSheetname);
                            MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":B" + (intEndingRowTmp - 1), strSheetname);

                        }


                        if (blHasWord)
                        {
                            if (dt.Rows.Count > 0)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strSheetname, strSheetname, "A1:B" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }



                        MSWord.deleteBookmarkComplete(strSheetname);

                    }
                    else
                    {
                        MSWord.cleanBookmark("Proced_meas_section");

                    }

                    if (blHasUtilization)
                    {
                        int lineNumber = MSWord.getLineNumber("Proced_meas_brk");
                        if (lineNumber > 2)
                            MSWord.addpageBreak("Proced_meas_brk");
                    }


                    MSWord.deleteBookmarkComplete("Proced_meas_brk");
                    MSWord.deleteBookmarkComplete("Proced_meas_section");


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strBookmarkName = "appendix";

                    //MSWord.addLineBreak(strBookmarkName);

                    if (blHasProcedural == true)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_all", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    }

                    if (blHasUtilization == true)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_2", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_1", "A1:C11", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    }


                    //if (blHasProcedural && blHasUtilization)
                    //{
                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Util_Proc_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Util_Proc_pg1", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);


                    //}
                    //else if (blHasProcedural)
                    //{

                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_only", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

                    //}
                    //else if (blHasUtilization)
                    //{

                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Util_only", "A1:C12", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

                    //}
                    MSWord.deleteBookmarkComplete(strBookmarkName);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



















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
