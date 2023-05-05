using System;
using System.Data;
using System.Configuration;
using WCDocumentGenerator;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections;



namespace PCR_Specialty_ch2
{
    class PCR_Specialty_ch2
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


                bool blIsMasked = true;

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

                ArrayList alSectionUtilization = new ArrayList();
                ArrayList alSectionProcedural = new ArrayList();



                int intProfileCnt = 1;
                int intTotalCnt;


                int intEndingRowTmp;

                bool blHasProcedural = false;
                bool blHasUtilization = false;

                //bool blProcedureOnlyBreak = false;


                string strMPINList = "select a.MPIN from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2  ";


                strMPINList = "5689592, 6355280, 5663309, 6204465, 6206594, 3355147, 6071014, 6121382, 5812598, 5508163, 6076431, 5476996, 5756209, 5627941"; //ALL SPECIALTY
                //strMPINList = "2678002,2696106,1936462,3206297,2583257,2027973,3190616"; //UTILIZATION ONLY
                //strMPINList = "1337921,2313899,1912087,2668337,2003948"; //PROCEDURE ONLY
                //strMPINList = "2646637,65820,1683154,1547078,1992708,3475642,2349067,1821698,1466586,2977056";// UTIL  AND PROCEDURE
                //strMPINList = "1813020, 2303192, 1974546, 3206481, 2907393, 2216599, 2095836, 2355276, 2839446, 461380, 5625400, 2140739, 1608278, 1974546, 2094308, 3063045, 5451500, 3456338, 3062716, 5451500, 3482301, 3211852"; //MA Request
                //strMPINList = "130544, 935294, 94020, 64954, 2079503, 210765, 2800444, 3064017, 2984790";//FINAL SAMPLES

                //strMPINList = "4111, 5982, 6353, 6419, 18620, 25159, 27034, 32816, 41023, 44431, 47454, 47969, 49268, 50190, 58578, 59729, 60630, 63918, 64954, 68469, 69049, 76858, 80506, 80771, 81879";//FINAL 25'

                //strMPINList = "1683154,1128651,1406103,1077739,2366423,2011880,1789855,2905625,2349067,2111629,2079503,1259160,860309,5128852,1911320,1900744,1739444,1682629,246131,63918";//MA FINAL

                //strMPINList = "1683154";




                // strMPINList = "130544, 935294, 94020, 64954, 2079503, 210765, 2800444, 3064017, 2984790";

                //strMPINList = "726239, 858217";



                //strMPINList = "2079503,130544";

                //select MAX(a.MPIN) as MPIN, a.Spec_display as NDB_Specialty from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN = b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin = PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN = a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region = b.RGN_NM where a.Exclude in(0, 5) and r.phase_id = 2 AND a.MPIN NOT IN(1849, 5689592, 49439, 6355280, 10559, 5663309, 86, 6204465, 1209, 6206594) GROUP BY a.Spec_display UNION select MIN(a.MPIN) as MPIN, a.Spec_display as NDB_Specialty from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN = b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin = PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN = a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region = b.RGN_NM where a.Exclude in(0, 5) and r.phase_id = 2 AND a.MPIN NOT IN(1849, 5689592, 49439, 6355280, 10559, 5663309, 86, 6204465, 1209) GROUP BY a.Spec_display
                //strMPINList = "1849, 5689592, 49439, 6355280, 10559, 5663309, 86, 6204465, 1209, 6206594, 137783, 3355147, 1590, 6071014, 1599, 6121382, 10544, 5812598, 1949, 5508163";//MaryAnn
                //strMPINList = "2832, 6076431, 5982, 5476996, 27647, 5756209, 6353, 5627941, 6419, 5225893, 61036, 5724330, 27034, 5311289, 4511, 6167451, 4111, 5680326, 675717, 3163581";//Amie
                //strMPINList = "15374, 5625289, 1602, 5812767, 11364, 5805637, 2574, 5457706, 7052, 5878864, 14049, 5447791, 34045, 5194463, 10918, 5627706, 16890, 5124262, 62611, 5723987";//Kristy
                //strMPINList = "29276, 5215675, 4547, 6158241, 7101, 5665825, 1037619, 3144705, 45032, 5622817, 4282, 5701328, 22379, 5760205, 5022, 5195931, 7053, 5684999, 44431, 4220454";//Frances

                //strMPINList = "3063045";//MASKED SAMPLE 3063045


                //strMPINList = "48572";//ISSUES


                if (blIsMasked)
                {
                    // strSQL = "select a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, Spec_display as NDB_Specialty,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XX' as State,'XXXXXXXXX' as zipcd, a.UHN_TIN,'XXXXXXXXX' as  PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2and a.MPIN in (" + strMPINList + ")";



                    strSQL = "select Top 100 a.MPIN,a.attr_clients as clients,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, 'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as [State],'XXXXXXXXX' as zipcd,'XXXXXXXXX' as taxid, 'XXXXXXXXX' as practice_id,'XXXXXXXXX' as Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    


                }
                else
                {

                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    //strSQL += " and  Tot_Util_meas = 0 and Tot_PX_meas = 1";
                }


          
                int intLineBreakCnt = 1;



                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {

                    alSectionProcedural = new ArrayList();
                    alSectionUtilization = new ArrayList();



                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;


                    //PROVIDER PLACEHOLDERS. THESE DB DATA COMES FROM MAIN LOOPING SQL ABOVE
                    string LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "NAME MISSING");
                    string FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "NAME MISSING");
                    string UCaseLastName = (dr["P_LastName"] != DBNull.Value ? dr["P_LastName"].ToString().Trim() : "NAME MISSING");
                    string UCaseFirstName = (dr["P_FirstName"] != DBNull.Value ? dr["P_FirstName"].ToString().Trim() : "NAME MISSING");




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


                    string attr_clients = (dr["clients"] != DBNull.Value ? dr["clients"].ToString().Trim() : "CLIENTS MISSING");

                    int proceudralCount = (dr["Tot_PX_meas"] != DBNull.Value ? int.Parse(dr["Tot_PX_meas"].ToString()) : 0);
                    int utilizationCount = (dr["Tot_Util_meas"] != DBNull.Value ? int.Parse(dr["Tot_Util_meas"].ToString()) : 0);
                    blHasProcedural = (proceudralCount > 0 ? true : false);
                    blHasUtilization = (utilizationCount > 0 ? true : false);


                    if(blHasProcedural && blHasUtilization)
                    {
                        MSWord.strWordTemplate =  ConfigurationManager.AppSettings["WordTemplateUtilAndProc"];
                    }
                    else if (blHasUtilization)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtil"];
                    }
                    else if (blHasProcedural)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateProc"];
                    }


                    string practiceName = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "PRACTICE NAME MISSING");

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



                    string strRCMOFirst = null;
                    string strRCMOLast = null;



                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

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


                    //strFolderName = strFolderNameTmp  + strTIN+ "\\";
                    strFolderName = strFolderNameTmp;



                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    if (blHasWord)
                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


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

                        MSWord.wordReplace("{$Physician Name}", FirstName + " " + LastName);

                        MSWord.wordReplace("{$P_FirstName}", UCaseFirstName);
                        MSWord.wordReplace("{$P_LastName}", UCaseLastName);

                        MSWord.wordReplace("{$Prov_Degree}", strProvDegree);
                        MSWord.wordReplace("{$Specialty}", strSpecialty);


                        MSWord.wordReplace("{$Address 1}", phyAddress);
                        MSWord.wordReplace("{$City}", phyCity);
                        MSWord.wordReplace("{$State}", phyState);
                        MSWord.wordReplace("{$ZIP Code}", phyZip);


                        MSWord.wordReplace("{$RCMO}", strRCMO);
                        MSWord.wordReplace("{$RCMO Title}", strRCMOTitle);

                        if (blIsMasked)
                            MSWord.wordReplace("{$MPIN}", "XXXXXXXXX");
                        else
                            MSWord.wordReplace("{$MPIN}", strMPIN);

                        MSWord.wordReplace("{$attr_clients}", attr_clients);

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

                    MSExcel.addValueToCell(strSheetname, "A5", FirstName + " " + LastName);

                    MSExcel.addValueToCell(strSheetname, "A6", strSpecialty);
                    MSExcel.addValueToCell(strSheetname, "A7", phyAddress);
                    MSExcel.addValueToCell(strSheetname, "A8", phyCity + ", " + phyState + " " + phyZip);

                    MSExcel.addValueToCell(strSheetname, "B10", practiceName);

                    MSExcel.addValueToCell(strSheetname, "B12", attr_clients);

                    MSExcel.addValueToCell(strSheetname, "A14", strRCMO);


                    MSExcel.addValueToCell(strSheetname, "A15", strRCMOTitle);

                    MSExcel.addValueToCell(strSheetname, "A16", strRCMOTitle1);


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////ULTILIZATION TOP SECTION///////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasUtilization)
                    {

                        strSheetname = "all_meas_util";
                        strBookmarkName = "utilization_section_table";

                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_Ph32 as a where MPIN=" + strMPIN + " order by sort_id";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", UCaseLastName);


                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F15", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);

                                MSWord.deleteBookmarkComplete(strBookmarkName);
                            }
                        }
                        else
                        {
                            if (blHasWord)
                                MSWord.cleanBookmark(strBookmarkName);
                        }


                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////PROCEDURAL TOP SECTION///////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (blHasProcedural)
                    {

                        strBookmarkName = "procedure_section_table";

                        switch (strSpecialty.ToUpper())
                        {

                            case "OBGYN":
                                strSheetname = "all_OB_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=4 and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "CARDIOLOGY":
                                strSheetname = "all_Card_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=5 and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "NEPHROLOGY":
                            case "NEUROLOGY":
                            case "RHEUMATOLOGY":
                                strSheetname = "all_Neurol_Rheum_Nephr_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id in(9,10,12) and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = false;
                                break;
                            case "OTOLARYNGOLOGY":
                                strSheetname = "all_ENT_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=14 and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "GENERAL SURGERY":
                                strSheetname = "all_Gen_Surg_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=18 and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "GASTROENTEROLOGY":
                            case "UROLOGY":
                                strSheetname = "all_GI_Urol_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id in(13,15) and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = false;
                                break;
                            case "OPHTHALMOLOGY":
                                strSheetname = "all_Ophthal_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=17 and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = false;
                                break;
                            case "NEUROSURGERY, ORTHOPEDICS AND SPINE":
                                strSheetname = "all_NOS_Proced";
                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=16 and MPIN=" + strMPIN + " order by sort_id";
                                //blProcedureOnlyBreak = true;
                                break;
                            default:
                                break;
                        }


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", UCaseLastName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F" + (dt.Rows.Count + 2), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                MSWord.deleteBookmarkComplete(strBookmarkName);
                            }
                        }
                        else
                        {
                            if (blHasWord)
                                MSWord.cleanBookmark(strBookmarkName);
                        }

                    }
                  

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////ULTILIZATION DRILLDOWN ///////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    if (blHasUtilization)
                    {
                        strBookmarkName = "utilization_drilldown_tables";

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=35 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Spec_diag_det";

                            alSectionUtilization.Add(strSheetname);


                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10; //FIRST BLANK ROW
                            if (dt.Rows.Count < 6) //TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count); //FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=12 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "PCP_Tier3_sum_det";

                            alSectionUtilization.Add(strSheetname);


                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=36 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "NonAdv_img_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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
                        strSQL = "select Category, Patient_Count, Visit_Count , Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Adv_img_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 8;//FIRST BLANK ROW
                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);//LAST BORDERED ROW
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
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=43 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Proc_Mod_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Modifier_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=29 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Consults_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Level4_5_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=16 and attr_mpin=" + strMPIN + "  order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "OON_lab_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

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

                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "LabPath_sum_det";

                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

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
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "ALOS_sum_det";

                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

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
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "IP_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

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

                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=1 and attr_mpin=" + strMPIN + " order by Catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "ED_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

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

                        MSWord.deleteBookmarkComplete(strBookmarkName);

                    }
                  

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////PROCEDURE DRILLDOWN ////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    if (blHasProcedural)
                    {
                        strBookmarkName = "procedure_drilldown_tables";

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        //strSQL = "select meas_cnt as Your_Data, total_cnt as Peers_Data, pct as Comparison_to_Peers from dbo.PBP_act_PX_ph32 where Measure_ID in (40, 41, 42) and mpin= " + strMPIN + " order by measure_id";
                        strSQL = "select act_display, expected_display, var_display from dbo.PBP_Profile_px_Ph32 as a where MPIN=" + strMPIN + " and measure_id between 40 and 42 order by measure_id";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "opioids_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'C');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", UCaseLastName);

                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        strSQL = "select Category, meas_cnt as gr_2_ct_scans_cnt, total_cnt as pt_cnt, Pct as tymp_noncompl_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=49 and MPIN=" + strMPIN + " order by CASE WHEN category='18 to 29 years' THEN 1 WHEN category='30 to 44 years' THEN 2 WHEN category='45 to 65 years' THEN 3 Else 4 END";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Sinusitis_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 8;//FIRST BLANK ROW
                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as non_compl_proc_cnt, total_cnt as proc_cnt, Pct as Tymp_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=45 and MPIN=" + strMPIN + " order by CASE WHEN category='0 to 2 years' THEN 1 WHEN category='3 to 10 years' THEN 2 WHEN category='11 to 17 years' THEN 3 END";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Tymp_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 7;//FIRST BLANK ROW
                            if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D6", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as non_compl_proc_cnt, total_cnt as proc_cnt, Pct as TAD_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=44 and MPIN=" + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "TAD_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as outpt_hosp_proc_cnt, total_cnt as outpt_hosp_asc_proc_Cnt, Pct as outpt_hosp_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=47 and MPIN=" + strMPIN + " ORDER BY CASE WHEN Category='All Others' THEN 3 WHEN Category LIKE 'Other%' THEN 2 Else 1 END";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "OPH_ASC_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as outpt_hosp_proc_cnt, total_cnt as outpt_hosp_asc_proc_Cnt, Pct as outpt_hosp_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=46 and MPIN=" + strMPIN + " ORDER BY CASE WHEN Category='All Others' THEN 3 WHEN Category LIKE 'Other%' THEN 2 Else 1 END";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "OON_Asst_Surg_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as asst_surg_proc_cnt, total_cnt as proc_cnt, Pct as asst_surg_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=48 and MPIN=" + strMPIN + " ORDER BY CASE WHEN Category='All Others' THEN 3 WHEN Category LIKE 'Other%' THEN 2 Else 1 END";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Asst_Surg_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category as Diagnosis_Category, meas_cnt as Spinal_Fusion_Count, total_cnt as Spinal_Laminectomy_Count, pct as Spinal_Fusion_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=28 and mpin= " + strMPIN + " order by pct desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Spinal_Fusion";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 9;//FIRST BLANK ROW
                            if (dt.Rows.Count < 5)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D8", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category as [Procedure], meas_cnt as Redo_Count, total_cnt as Procedure_Count, pct as Redo_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=27 and mpin= " + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Redo";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 21;//FIRST BLANK ROW
                            if (dt.Rows.Count < 17)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D20", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category as [Procedure], meas_cnt as ED_Visit_in_30days, total_cnt as Procedure_Count, pct as Rate from dbo.PBP_act_PX_ph32 where Measure_ID=26 and mpin= " + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Unpl_ED";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category as [Procedure], meas_cnt as Admits30_days, total_cnt as Procedure_Count, pct as Rate from dbo.PBP_act_PX_ph32 where Measure_ID=25 and mpin= " + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Unpl_admit";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category as [Procedure], meas_cnt as Complication_Count, total_cnt as Procedure_Count, pct as Rate from PBP_act_PX_ph32 where Measure_ID=24 and mpin= " + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Complications";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 22;//FIRST BLANK ROW
                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category as Hysterectomy_Type, meas_cnt as Procedure_Count, pct as Hysterectomy_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=19 and mpin= " + strMPIN + " order by Procedure_Count desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Hyst";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 8;//FIRST BLANK ROW
                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":C7", strSheetname);//LAST BORDERED ROW
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

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSQL = "select Category as Age_Category, meas_cnt as Cesarean_Section_Count, total_cnt as Delivery_Count, pct as Cesarean_Section_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=18 and mpin= " + strMPIN + " order by CASE WHEN category='Up to 34 years' THEN 1 WHEN category='35 – 39 years' THEN 2 WHEN category='40 – 44 years' THEN 3 Else 4 END";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Csection";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 8;//FIRST BLANK ROW
                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as nbr_stent, total_cnt as tot_cath_cnt, Pct as stent_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=23 and MPIN=" + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Stent_Rate";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 12;//FIRST BLANK ROW
                            if (dt.Rows.Count < 8)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D11", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as neg_cath_cnt, total_cnt as nbr_caths, Pct as neg_cath_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=22 and MPIN=" + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Neg_Cath";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 13;//FIRST BLANK ROW
                            if (dt.Rows.Count < 9)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D12", strSheetname);//LAST BORDERED ROW
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

                        strSQL = "select Category, meas_cnt as nbr_caths, total_cnt as cath_cnt, Pct as cath_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=21 and MPIN=" + strMPIN + " order by meas_cnt desc";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "PreCath_Testing";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 17;//FIRST BLANK ROW
                            if (dt.Rows.Count < 13)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D16", strSheetname);//LAST BORDERED ROW
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

                        MSWord.deleteBookmarkComplete(strBookmarkName);

                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strBookmarkName = "appendix";


                    if (blHasProcedural)
                    {

                        switch (strSpecialty.ToUpper())
                        {

                            case "OBGYN":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_OB_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_OB_pg1", "A1:C8", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "CARDIOLOGY":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_CARD_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_CARD_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "NEPHROLOGY":
                            case "NEUROLOGY":
                            case "RHEUMATOLOGY":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Neurol_Rheum_Nephr", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "OTOLARYNGOLOGY":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_ENT_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_ENT_pg1", "A1:C10", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "GENERAL SURGERY":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Gen_Surg_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Gen_Surg_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "GASTROENTEROLOGY":
                            case "UROLOGY":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_GI_urol_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_GI_urol_pg1", "A1:C7", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "OPHTHALMOLOGY":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Ophthal", "A1:C6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            case "NEUROSURGERY, ORTHOPEDICS AND SPINE":
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_NOS_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_NOS_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                                break;
                            default:
                                break;
                        }

                    }

                    if (blHasUtilization)
                    {


                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_2", "A1:C6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

                    }

                    MSWord.deleteBookmarkComplete(strBookmarkName);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    //UNCOMMENT ME!!!!!
                    if (blHasUtilization)
                    {
                        // MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                        //MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                        processBreaks(alSectionUtilization, 1);
                        processTopBreaks(alSectionUtilization, 1);

                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                        // 


                    }




                    if (blHasProcedural)
                    {
                        //MSWord.addpageBreak("procedure_drilldown_pagebreak");

                        processBreaks(alSectionProcedural, 1);
                        processTopBreaks(alSectionProcedural, 1);

                    }


                    //COMMENT ME!!!!!
                    //if (blHasUtilization)
                    //{
                    //    // MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                    //    //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                    //    //MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                    //    processBreaks(alSectionUtilization, 1);
                    //    processTopBreaks(alSectionUtilization, 1);

                    //    //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                    //    // 


                    //}


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





                    ///////////////////////////////////////////////////////////////////////////////

                    ////WRITE WORD TO PDF
                    if (blHasPDF)
                    {
                        //AdobeAcrobat.tryCnt = 0;
                        //AdobeAcrobat.createPDF(strFinalReportFileName);
                        //AdobeAcrobat.tryCnt = 0;

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


                    //if (intProfileCnt > 4)
                    //    break;


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


























//namespace PCR_Specialty_ch2
//{
//    class PCR_Specialty_ch2
//    {
//        static void Main(string[] args)
//        {


//            string strSQL = null;

//            try
//            {

//                //int intSpecialHandlingMax = 1000000;

//                Console.WriteLine("Wiser Choices Profiles Generator");


//                //PLACE APP.CONFIG FILE DATA INTO VARIABLES START
//                string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
//                bool blVisibleExcel = Boolean.Parse(ConfigurationManager.AppSettings["VisibleExcel"]);
//                bool blSaveExcel = Boolean.Parse(ConfigurationManager.AppSettings["SaveExcel"]);
//                bool blVisibleWord = Boolean.Parse(ConfigurationManager.AppSettings["VisibleWord"]);
//                bool blSaveWord = Boolean.Parse(ConfigurationManager.AppSettings["SaveWord"]);
//                string strExcelTemplate = ConfigurationManager.AppSettings["ExcelTemplate"];
//                string strWordTemplate = ConfigurationManager.AppSettings["WordTemplate"];
//                bool blOverwriteExisting = Boolean.Parse(ConfigurationManager.AppSettings["OverwriteExisting"]);
//                string strStartDate = ConfigurationManager.AppSettings["StartDate"];
//                string strEndDate = ConfigurationManager.AppSettings["EndDate"];
//                string strDisplayDate = ConfigurationManager.AppSettings["ProfileDate"];
//                string strReportsPath = ConfigurationManager.AppSettings["ReportsPath"];
//                string strPhase = ConfigurationManager.AppSettings["Phase"];
//                string strSpecialtyId = ConfigurationManager.AppSettings["SpecialtyId"];


//                string strPEIPath = ConfigurationManager.AppSettings["PEIPath"];


//                string strEpisodeCount = ConfigurationManager.AppSettings["EpisodeCount"];



//                if (String.IsNullOrEmpty(strSpecialtyId))
//                    strSpecialtyId = null;


//                //PLACE CONFIG FILE DATA INTO VARIABLES END

//                string strMonthYear = DateTime.Now.Month + "_" + DateTime.Now.Year;
//                string strFinalReportFileName;


//                bool blHasWord = true;
//                bool blHasPDF = true;


//                bool blIsMasked = false;

//                //START EXCEL APP

//                MSExcel.populateExcelParameters(blVisibleExcel, blSaveExcel, strReportsPath, strExcelTemplate);
//                MSExcel.openExcelApp();

//                //Console.WriteLine("Starting Microsoft Word Instance...");
//                //START WORD APP
//                if (blHasWord)
//                {
//                    MSWord.populateWordParameters(blVisibleWord, blSaveWord, strReportsPath, strWordTemplate);
//                    MSWord.openWordApp();
//                }


//                DataTable dt = null;
//                Hashtable htParam = new Hashtable();
//                string strSheetname = null;
//                string strBookmarkName = null;

//                ArrayList alSectionUtilization = new ArrayList();
//                ArrayList alSectionProcedural = new ArrayList();



//                int intProfileCnt = 1;
//                int intTotalCnt;


//                int intEndingRowTmp;

//                bool blHasProcedural = false;
//                bool blHasUtilization = false;

//                string strMPINList = "select a.MPIN from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 ";


//                //strMPINList = "5689592, 6355280, 5663309, 6204465, 6206594, 3355147, 6071014, 6121382, 5812598, 5508163, 6076431, 5476996, 5756209, 5627941"; //ALL SPECIALTY
//                //strMPINList = "2678002,2696106,1936462,3206297,2583257,2027973,3190616"; //UTILIZATION ONLY
//                //strMPINList = "1337921,2313899,1912087,2668337,2003948"; //PROCEDURE ONLY

//                strMPINList = "2678002";
//                if (blIsMasked)
//                {
//                    // strSQL = "select a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, Spec_display as NDB_Specialty,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XX' as State,'XXXXXXXXX' as zipcd, a.UHN_TIN,'XXXXXXXXX' as  PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2and a.MPIN in (" + strMPINList + ")";
//                }
//                else
//                {

//                    strSQL = "select Top 10 a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


//                }


//                int intLineBreakCnt = 1;



//                DataTable dtMain = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                Console.WriteLine("Gathering targeted physicians...");
//                intTotalCnt = dtMain.Rows.Count;
//                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
//                {

//                    alSectionProcedural = new ArrayList();
//                    alSectionUtilization = new ArrayList();



//                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;


//                    //PROVIDER PLACEHOLDERS. THESE DB DATA COMES FROM MAIN LOOPING SQL ABOVE
//                    string LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "NAME MISSING");
//                    string FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "NAME MISSING");
//                    string UCaseLastName = (dr["P_LastName"] != DBNull.Value ? dr["P_LastName"].ToString().Trim() : "NAME MISSING");
//                    string UCaseFirstName = (dr["P_FirstName"] != DBNull.Value ? dr["P_FirstName"].ToString().Trim() : "NAME MISSING");




//                    string phyAddress = (dr["Street"] != DBNull.Value ? dr["Street"].ToString().Trim() : "ADDRESS MISSING");
//                    string phyCity = (dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "CITY MISSING");
//                    string phyState = (dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "STATE MISSING");
//                    string phyZip = (dr["zipcd"] != DBNull.Value ? dr["zipcd"].ToString().Trim() : "ZIPCODE MISSING");



//                    string strTIN = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString().Trim() : "");

//                    string strProvDegree = (dr["ProvDegree"] != DBNull.Value ? dr["ProvDegree"].ToString().Trim() : "PROV DEGREE MISSING");
//                    string strSpecialty = (dr["NDB_Specialty"] != DBNull.Value ? dr["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");

//                    string strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "RCMO MISSING");
//                    string strRCMOTitle = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "RCMO TITLE MISSING");
//                    string strRCMOTitle1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "RCMO TITLE 1 MISSING");


//                    string attr_clients = (dr["clients"] != DBNull.Value ? dr["clients"].ToString().Trim() : "CLIENTS MISSING");

//                    int proceudralCount = (dr["Tot_PX_meas"] != DBNull.Value ? int.Parse(dr["Tot_PX_meas"].ToString()) : 0);
//                    int utilizationCount = (dr["Tot_Util_meas"] != DBNull.Value ? int.Parse(dr["Tot_Util_meas"].ToString()) : 0);
//                    blHasProcedural = (proceudralCount > 0 ? true : false);
//                    blHasUtilization = (utilizationCount > 0 ? true : false);



//                    string practiceName = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "PRACTICE NAME MISSING");

//                    //string ocl = (dr["orig_cl"] != DBNull.Value ? dr["orig_cl"].ToString().Trim() : "ZIPCODE MISSING");
//                    // string cl_rem1 = (dr["attr_cl_rem1"] != DBNull.Value ? dr["attr_cl_rem1"].ToString().Trim() : "ZIPCODE MISSING");



//                    string strMPIN = (dr["MPIN"] != DBNull.Value ? dr["MPIN"].ToString().Trim() : "");
//                    string strMPINLabel = null;


//                    if (blIsMasked)
//                    {
//                        strMPINLabel = "123456" + intProfileCnt;
//                    }
//                    else
//                    {
//                        strMPINLabel = strMPIN;
//                    }



//                    string strRCMOFirst = null;
//                    string strRCMOLast = null;



//                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

//                    string strFolderName = "";



//                    //NOT QA UNCOMMENT
//                    if (!String.IsNullOrEmpty(strFolderNameTmp))
//                    {
//                        strFolderNameTmp = "SpecialHandling\\" + strFolderNameTmp + strTIN + "\\";
//                    }
//                    else
//                    {
//                        strFolderNameTmp = "RegularMailing\\" + strFolderNameTmp;
//                    }


//                    //strFolderName = strFolderNameTmp  + strTIN+ "\\";
//                    strFolderName = strFolderNameTmp;



//                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

//                    if (blHasWord)
//                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


//                    strFinalReportFileName = strMPINLabel + "_" + LastName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_" + phyState + "_" + strMonthYear;

//                    if (!blOverwriteExisting)
//                    {
//                        //...CHECK IF PROFILE EXISTS...
//                        if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "QA") + "word\\" + strFinalReportFileName + ".doc"))
//                        {
//                            Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
//                            intProfileCnt++;
//                            //...IF PROFILE EXISTS MOVE TO NEXT MPIN
//                            continue;
//                        }
//                    }


//                    //OPEN EXCEL WB
//                    MSExcel.openExcelWorkBook();


//                    if (blHasWord)
//                    {

//                        //OPEN WORD DOCUMENT
//                        MSWord.openWordDocument();

//                        //GENERAL PLACE HOLDERS. WE USE VARIABLES TO REPLACE PLACEHOLDERS WITHIN THE WORD DOC

//                        MSWord.wordReplace("{$Date}", strDisplayDate);

//                        MSWord.wordReplace("{$Physician Name}", FirstName + " " + LastName);

//                        MSWord.wordReplace("{$P_FirstName}", UCaseFirstName);
//                        MSWord.wordReplace("{$P_LastName}", UCaseLastName);

//                        MSWord.wordReplace("{$Prov_Degree}", strProvDegree);
//                        MSWord.wordReplace("{$Specialty}", strSpecialty);


//                        MSWord.wordReplace("{$Address 1}", phyAddress);
//                        MSWord.wordReplace("{$City}", phyCity);
//                        MSWord.wordReplace("{$State}", phyState);
//                        MSWord.wordReplace("{$ZIP Code}", phyZip);


//                        MSWord.wordReplace("{$RCMO}", strRCMO);
//                        MSWord.wordReplace("{$RCMO Title}", strRCMOTitle);

//                        MSWord.wordReplace("{$MPIN}", strMPIN);

//                        MSWord.wordReplace("{$attr_clients}", attr_clients);

//                        if (strRCMO == "Jack S. Weiss, M.D.")
//                        {
//                            strRCMOFirst = "Jack";
//                            strRCMOLast = "Weiss";
//                        }
//                        else
//                        {
//                            strRCMOFirst = "Janice";
//                            strRCMOLast = "Huckaby";
//                        }

//                        MSWord.addSignature(strRCMOFirst, strRCMOLast);

//                        MSWord.deleteBookmarkComplete("signature");
//                    }

//                    //END WORD DOCUMENT PAGE 1
//                    //END WORD DOCUMENT PAGE 1
//                    //END WORD DOCUMENT PAGE 1






//                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES
//                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES
//                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES
//                    /////////////////////////ADD DR TO ALL GRAPHS AND TABLES


//                    ///////////////////////////////////////////////////////////////pg 2 - ETG table, graph/////////////////////////////////////////////////////////////////////////////////////////

//                    //START EXCEL SHEET: Cardiac_Procs_MCR
//                    //START EXCEL SHEET: Cardiac_Procs_MCR
//                    //START EXCEL SHEET: Cardiac_Procs_MCR

//                    strSheetname = "general info";

//                    MSExcel.addValueToCell(strSheetname, "B2", strMPINLabel);
//                    MSExcel.addValueToCell(strSheetname, "B3", strTIN);

//                    MSExcel.addValueToCell(strSheetname, "A5", FirstName + " " + LastName);

//                    MSExcel.addValueToCell(strSheetname, "A6", strSpecialty);
//                    MSExcel.addValueToCell(strSheetname, "A7", phyAddress);
//                    MSExcel.addValueToCell(strSheetname, "A8", phyCity + ", " + phyState + " " + phyZip);

//                    MSExcel.addValueToCell(strSheetname, "B10", practiceName);

//                    MSExcel.addValueToCell(strSheetname, "B12", attr_clients);

//                    MSExcel.addValueToCell(strSheetname, "A14", strRCMO);


//                    MSExcel.addValueToCell(strSheetname, "A15", strRCMOTitle);

//                    MSExcel.addValueToCell(strSheetname, "A16", strRCMOTitle1);


//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    ///////////////////////////////////////ULTILIZATION TOP SECTION///////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                    if (blHasUtilization)
//                    {

//                        MSWord.deleteBookmarkComplete("utilization_section");


//                        strSheetname = "all_meas_util";
//                        strBookmarkName = "utilization_section_table";

//                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_Ph32 as a where MPIN=" + strMPIN + " order by sort_id";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

//                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", UCaseLastName);


//                            if (blHasWord)
//                            {
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F15", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);

//                                MSWord.deleteBookmarkComplete(strBookmarkName);
//                            }
//                        }
//                        else
//                        {
//                            if (blHasWord)
//                                MSWord.cleanBookmark(strBookmarkName);
//                        }


//                    }
//                    else
//                    {

//                        MSWord.cleanBookmark("utilization_section");
//                        MSWord.deleteBookmarkComplete("utilization_section");
//                    }



//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    ///////////////////////////////////////PROCEDURAL TOP SECTION///////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    if (blHasProcedural)
//                    {

//                        MSWord.deleteBookmarkComplete("procedure_section");

//                        strBookmarkName = "procedure_section_table";

//                        switch (strSpecialty.ToUpper())
//                        {

//                            case "OBGYN":
//                                strSheetname = "all_OB_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=4 and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "CARDIOLOGY":
//                                strSheetname = "all_Card_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=5 and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "NEPHROLOGY":
//                            case "NEUROLOGY":
//                            case "RHEUMATOLOGY":
//                                strSheetname = "all_Neurol_Rheum_Nephr_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id in(9,10,12) and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "OTOLARYNGOLOGY":
//                                strSheetname = "all_ENT_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=14 and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "GENERAL SURGERY":
//                                strSheetname = "all_Gen_Surg_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=18 and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "GASTROENTEROLOGY":
//                            case "UROLOGY":
//                                strSheetname = "all_GI_Urol_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id in(13,15) and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "OPHTHALMOLOGY":
//                                strSheetname = "all_Ophthal_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=17 and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            case "NEUROSURGERY, ORTHOPEDICS AND SPINE":
//                                strSheetname = "all_NOS_Proced";
//                                strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=16 and MPIN=" + strMPIN + " order by sort_id";
//                                break;
//                            default:
//                                break;
//                        }


//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {
//                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

//                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", UCaseLastName);

//                            if (blHasWord)
//                            {

//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F" + (dt.Rows.Count + 2), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
//                                MSWord.deleteBookmarkComplete(strBookmarkName);
//                            }
//                        }
//                        else
//                        {
//                            if (blHasWord)
//                                MSWord.cleanBookmark(strBookmarkName);
//                        }

//                    }
//                    else
//                    {

//                        MSWord.cleanBookmark("procedure_section");
//                        MSWord.deleteBookmarkComplete("procedure_section");
//                    }





//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    ///////////////////////////////////////ULTILIZATION DRILLDOWN ///////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



//                    if (blHasUtilization)
//                    {
//                        MSWord.deleteBookmarkComplete("utilization_drilldown_section");
//                        strBookmarkName = "utilization_drilldown_tables";

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=35 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Spec_diag_det";

//                            alSectionUtilization.Add(strSheetname);


//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10; //FIRST BLANK ROW
//                            if (dt.Rows.Count < 6) //TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count); //FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=12 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "PCP_Tier3_sum_det";

//                            alSectionUtilization.Add(strSheetname);


//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=36 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "NonAdv_img_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }



//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count , Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "Adv_img_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 8;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }



//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=43 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "Proc_Mod_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }




//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "Modifier_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }





//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=29 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "Consults_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }





//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "Level4_5_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=16 and attr_mpin=" + strMPIN + "  order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "OON_lab_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;
//                            if (dt.Rows.Count < 6)
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }


//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "LabPath_sum_det";

//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;
//                            if (dt.Rows.Count < 6)
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }

//                            }
//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";

//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "ALOS_sum_det";

//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;
//                            if (dt.Rows.Count < 6)
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);
//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";

//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "IP_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;
//                            if (dt.Rows.Count < 6)
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }
//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=1 and attr_mpin=" + strMPIN + " order by Catg_order";

//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "ED_sum_det";
//                            alSectionUtilization.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 10;
//                            if (dt.Rows.Count < 6)
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D9", strSheetname);
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        MSWord.deleteBookmarkComplete(strBookmarkName);

//                    }
//                    else
//                    {
//                        MSWord.cleanBookmark("utilization_drilldown_section");
//                        MSWord.deleteBookmarkComplete("utilization_drilldown_section");
//                    }



//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    //////////////////////////////////////////////////PROCEDURE DRILLDOWN ////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//                    if (blHasProcedural)
//                    {
//                        MSWord.deleteBookmarkComplete("procedure_drilldown_section");
//                        strBookmarkName = "procedure_drilldown_tables";

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as gr_2_ct_scans_cnt, total_cnt as pt_cnt, Pct as tymp_noncompl_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=49 and MPIN=" + strMPIN + " order by CASE WHEN category='18 to 29 years' THEN 1 WHEN category='30 to 44 years' THEN 2 WHEN category='45 to 65 years' THEN 3 Else 4 END";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Sinusitis_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 8;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as non_compl_proc_cnt, total_cnt as proc_cnt, Pct as Tymp_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=45 and MPIN=" + strMPIN + " order by CASE WHEN category='0 to 2 years' THEN 1 WHEN category='3 to 10 years' THEN 2 WHEN category='11 to 17 years' THEN 3 END";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Tymp_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 7;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D6", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as non_compl_proc_cnt, total_cnt as proc_cnt, Pct as TAD_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=44 and MPIN=" + strMPIN + " order by Category";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "TAD_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as outpt_hosp_proc_cnt, total_cnt as outpt_hosp_asc_proc_Cnt, Pct as outpt_hosp_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=47 and MPIN=" + strMPIN + " ORDER BY CASE WHEN Category='All Others' THEN 3 WHEN Category LIKE 'Other%' THEN 2 Else 1 END";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "OPH_ASC_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as outpt_hosp_proc_cnt, total_cnt as outpt_hosp_asc_proc_Cnt, Pct as outpt_hosp_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=46 and MPIN=" + strMPIN + " ORDER BY CASE WHEN Category='All Others' THEN 3 WHEN Category LIKE 'Other%' THEN 2 Else 1 END";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "OON_Asst_Surg_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as asst_surg_proc_cnt, total_cnt as proc_cnt, Pct as asst_surg_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=48 and MPIN=" + strMPIN + " ORDER BY CASE WHEN Category='All Others' THEN 3 WHEN Category LIKE 'Other%' THEN 2 Else 1 END";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Asst_Surg_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        //strSQL = "select meas_cnt as Your_Data, total_cnt as Peers_Data, pct as Comparison_to_Peers from dbo.PBP_act_PX_ph32 where Measure_ID in (40, 41, 42) and mpin= " + strMPIN + " order by measure_id";
//                        strSQL = "select act_display, expected_display, var_display from dbo.PBP_Profile_px_Ph32 as a where MPIN=" + strMPIN + " and measure_id between 40 and 42 order by measure_id";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
//                        if (dt.Rows.Count > 0)
//                        {

//                            strSheetname = "opioids_det";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'C');

//                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", UCaseLastName);

//                            if (blHasWord)
//                            {
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                            }
//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as Hysterectomy_Type, meas_cnt as Procedure_Count, pct as Hysterectomy_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=19 and mpin= " + strMPIN + " order by Procedure_Count desc";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Hyst";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 8;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":C7", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":C" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:C" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as Age_Category, meas_cnt as Cesarean_Section_Count, total_cnt as Delivery_Count, pct as Cesarean_Section_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=18 and mpin= " + strMPIN + " order by CASE WHEN category='Up to 34 years' THEN 1 WHEN category='35 – 39 years' THEN 2 WHEN category='40 – 44 years' THEN 3 Else 4 END";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Csection";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 8;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 4)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as Diagnosis_Category, meas_cnt as Spinal_Fusion_Count, total_cnt as Spinal_Laminectomy_Count, pct as Spinal_Fusion_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=28 and mpin= " + strMPIN + " order by meas_cnt desc";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Spinal_Fusion";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 9;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 5)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D8", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as [Procedure], meas_cnt as Redo_Count, total_cnt as Procedure_Count, pct as Redo_Rate from dbo.PBP_act_PX_ph32 where Measure_ID=27 and mpin= " + strMPIN + " order by meas_cnt desc";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Redo";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 21;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 17)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D20", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }


//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as [Procedure], meas_cnt as ED_Visit_in_30days, total_cnt as Procedure_Count, pct as Rate from dbo.PBP_act_PX_ph32 where Measure_ID=26 and mpin= " + strMPIN + " order by meas_cnt desc";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Unpl_ED";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as [Procedure], meas_cnt as Admits30_days, total_cnt as Procedure_Count, pct as Rate from dbo.PBP_act_PX_ph32 where Measure_ID=25 and mpin= " + strMPIN + " order by meas_cnt desc";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Unpl_admit";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category as [Procedure], meas_cnt as Complication_Count, total_cnt as Procedure_Count, pct as Rate from PBP_act_PX_ph32 where Measure_ID=24 and mpin= " + strMPIN + " order by meas_cnt desc";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Complications";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 22;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 18)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as nbr_stent, total_cnt as tot_cath_cnt, Pct as stent_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=23 and MPIN=" + strMPIN + " order by Category";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Stent_Rate";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 12;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 8)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D11", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as neg_cath_cnt, total_cnt as nbr_caths, Pct as neg_cath_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=22 and MPIN=" + strMPIN + " order by Category";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "Neg_Cath";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 13;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 9)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D12", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//                        strSQL = "select Category, meas_cnt as nbr_caths, total_cnt as cath_cnt, Pct as cath_rate from dbo.PBP_act_px_ph32 as a where Measure_ID=21 and MPIN=" + strMPIN + " order by Category";
//                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

//                        if (dt.Rows.Count > 0)
//                        {
//                            strSheetname = "PreCath_Testing";
//                            alSectionProcedural.Add(strSheetname);

//                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", UCaseFirstName);
//                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", UCaseLastName);

//                            intEndingRowTmp = 17;//FIRST BLANK ROW
//                            if (dt.Rows.Count < 13)//TOTAL BORDERED ROWS
//                            {
//                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
//                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D16", strSheetname);//LAST BORDERED ROW
//                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

//                            }

//                            if (blHasWord)
//                            {
//                                if (dt.Rows.Count > 0)
//                                {
//                                    MSWord.tryCount = 0;
//                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
//                                }
//                            }

//                        }

//                        MSWord.deleteBookmarkComplete(strBookmarkName);

//                    }

//                    else
//                    {
//                        MSWord.cleanBookmark("procedure_drilldown_section");
//                        MSWord.deleteBookmarkComplete("procedure_drilldown_section");
//                    }



//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    strBookmarkName = "appendix";


//                    if (blHasProcedural)
//                    {

//                        switch (strSpecialty.ToUpper())
//                        {

//                            case "OBGYN":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_OB_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_OB_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "CARDIOLOGY":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_CARD_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_CARD_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "NEPHROLOGY":
//                            case "NEUROLOGY":
//                            case "RHEUMATOLOGY":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Neurol_Rheum_Nephr", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "OTOLARYNGOLOGY":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_ENT_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_ENT_pg1", "A1:C10", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "GENERAL SURGERY":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Gen_Surg_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Gen_Surg_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "GASTROENTEROLOGY":
//                            case "UROLOGY":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_GI_urol_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_GI_urol_pg1", "A1:C7", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "OPHTHALMOLOGY":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Ophthal", "A1:C6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            case "NEUROSURGERY, ORTHOPEDICS AND SPINE":
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_NOS_pg2", "A1:C4", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                MSWord.tryCount = 0;
//                                MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_NOS_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                                break;
//                            default:
//                                break;
//                        }

//                    }

//                    if (blHasUtilization)
//                    {


//                        MSWord.tryCount = 0;
//                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_2", "A1:C6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
//                        MSWord.tryCount = 0;
//                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

//                    }

//                    MSWord.deleteBookmarkComplete(strBookmarkName);
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





//                    if (blHasUtilization)
//                    {
//                        // MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
//                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
//                        //MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
//                        processBreaks(alSectionUtilization, 1);
//                        processTopBreaks(alSectionUtilization, 1);

//                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
//                       // 


//                    }
//                    else if (blHasProcedural)
//                    {
//                        MSWord.addpageBreak("procedure_drilldown_pagebreak");

//                        processBreaks(alSectionProcedural, 1);
//                        processTopBreaks(alSectionProcedural, 1);

//                    }


//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////





//                    ///////////////////////////////////////////////////////////////////////////////

//                    ////WRITE WORD TO PDF
//                    if (blHasPDF)
//                    {
//                        //AdobeAcrobat.tryCnt = 0;
//                        //AdobeAcrobat.createPDF(strFinalReportFileName);
//                        //AdobeAcrobat.tryCnt = 0;

//                        MSWord.convertWordToPDF(strFinalReportFileName, "Final", strPEIPath);
//                    }

//                    //CLOSE EXCEL WB
//                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "QA");


//                    if (blHasWord)
//                    {
//                        //CLOSE WORD DOCUMENTfor t
//                        MSWord.closeWordDocument(strFinalReportFileName, "QA");
//                    }

//                    //CLOSE DOC END
//                    Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Completed profile for MPIN '" + strMPIN + "'");

//                    intProfileCnt++;
//                    //break;


//                    //if (intProfileCnt > 4)
//                    //    break;


//                }//MAIN LOOP END

//            }
//            catch (Exception ex)
//            {



//                if (!EventLog.SourceExists("Wiser Choices"))
//                    EventLog.CreateEventSource("Wiser Choices", "Application");


//                EventLog.WriteEntry("Wiser Choices", ex.ToString() + Environment.NewLine + Environment.NewLine + Environment.NewLine + strSQL, EventLogEntryType.Error, 234);


//                Console.WriteLine("There was an error, see details below");
//                Console.WriteLine(ex.ToString());
//                Console.WriteLine();
//                Console.WriteLine("SQL:");
//                Console.WriteLine(strSQL);

//                Console.Beep();


//                Console.ReadLine();


//            }
//            finally
//            {
//                Console.WriteLine("Closing Adobe Acrobat Instance...");
//                //CLOSE ADOBE APP
//                //AdobeAcrobat.closeAcrobat();

//                Console.WriteLine("Closing Microsoft Excel Instance...");
//                //CLOSE EXCEL APP
//                MSExcel.closeExcelApp();

//                Console.WriteLine("Closing Microsoft Word Instance...");
//                //CLOSE WORD APP
//                MSWord.closeWordApp();


//                foreach (Process Proc in Process.GetProcesses())
//                    if (Proc.ProcessName.Equals("EXCEL") || Proc.ProcessName.Equals("WINWORD"))  //Process Excel?
//                        Proc.Kill();


//            }

//        }


//        private static void processBreaks(ArrayList al, int iArrayType)
//        {

//            if (al.Count > 0)
//            {
//                al.Reverse();
//                int intLineNumber = 0;
//                for (int i = 0; i < al.Count; i++)
//                {

//                    intLineNumber = MSWord.getLineNumber(al[i].ToString());


//                    if ((i + 1) < al.Count)
//                    {
//                        if ((iArrayType == 1 && intLineNumber < 25) || (iArrayType == 2 && intLineNumber <= 7) || (iArrayType == 3))
//                            MSWord.addLineBreak(al[i].ToString());
//                    }




//                }

//            }
//        }

//        private static void processTopBreaks(ArrayList al, int iArrayType)
//        {
//            string s = "";

//            if (al.Count > 0)
//            {
//                //al.Reverse();
//                string strLastBookMark = null;
//                int intLineNumber = 0;
//                for (int i = 0; i < al.Count; i++)
//                {

//                    intLineNumber = MSWord.getLineNumber(al[i].ToString());

//                    if (intLineNumber == 1)
//                    {
//                        while (intLineNumber == 1 && strLastBookMark != null)
//                        {
//                            intLineNumber = MSWord.getLineNumber(al[i].ToString());


//                            if (intLineNumber == 1)
//                            {
//                                MSWord.addLineBreak(strLastBookMark);
//                            }
//                        }
//                    }

//                    strLastBookMark = al[i].ToString();
//                }


//                //DELETE BOOKMARKS
//                for (int i = 0; i < al.Count; i++)
//                {
//                    MSWord.deleteBookmarkComplete(al[i].ToString());
//                }


//            }
//        }

//    }
//}