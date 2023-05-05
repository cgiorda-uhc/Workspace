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

namespace PCR_Oncology_C2_SAS
{
    class PCR_Oncology_C2_SAS
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


                IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
                IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
                IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
                IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
                IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
                IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
                IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];


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

                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;
                string strBookmarkName = null;




                int intEndingRowTmp;


                string strTaxID;

               // string strPracticeID;

                string strTaxIDLabel;

                string strCorpOwnerName;
                string strCorpOwnerNameLC;
                string strStreet;
                string strCity;
                string strState;
                string strZipCd;
                string strModelId;


                string strSampleCount = "50";
                string strTinList = "SELECT d.TaxID FROM Onc.Onc_TIN_demog as d inner join Onc.ONC_TIN_ADDR as a on d.TaxID=a.TaxID where Exclude is null  ";
                strTinList = "471859521";
                //strTinList = "1773680, 682172, 75091, 110728, 2230565, 1802665, 1164884, 278225";//BOTH
                //strTinList = "199773, 3209425, 1943764, 3228026, 3449085, 6212319, 4977505";//UTIL ONLY
                //strTinList = "238232,2397262,304860,186301,3130383,642509,180765,3362723,1698660,2503699";//PROC ONLY
                //strTinList = "900477349";


                //strTinList = "10357684";

                ArrayList alSection = new ArrayList();


                //strTinList = "956419205,954540991";

                if (blIsMasked)
                {

                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";

                    strSQL = "SELECT d.TaxID, d.CorpOwnerName as Practice_Name, a.Street, a.City, a.State, a.ZipCd, o.model_id FROM Onc.ONC_TIN_OUTLIER as o inner join Onc.Onc_TIN_demog as d on d.taxid=o.taxid inner join Onc.ONC_TIN_ADDR as a on d.TaxID=a.TaxID where Exclude is null and d.taxid in (" + strTinList + ")  and o.model_id in (1)  ";

                }
                else
                {




                    strSQL = "SELECT distinct d.TaxID, d.CorpOwnerName as Practice_Name, a.Street, a.City, a.State, a.ZipCd, o.model_id FROM Onc.ONC_TIN_OUTLIER as o inner join Onc.Onc_TIN_demog as d on d.taxid=o.taxid inner join Onc.ONC_TIN_ADDR as a on d.TaxID=a.TaxID where Exclude is null and d.taxid in (" + strTinList + ") and o.model_id in (1) order by d.TaxID asc ";


                }

                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


                DataTable dtMain = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {
                    alSection = new ArrayList();

                    //if (intProfileCnt < 76 || intProfileCnt > 95)
                    //{
                    //    intProfileCnt++;
                    //    continue;
                    //}



                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

                    strTaxID = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString().Trim() : "VALUE MISSING");

                   // strPracticeID = (dr["PracticeId"] != DBNull.Value ? dr["PracticeId"].ToString().Trim() : "VALUE MISSING");

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



                    if (blIsMasked)
                    {
                        strCorpOwnerName = "XXXXXXX XXXXXXX";

                        strCorpOwnerNameLC = "XXXXXXX XXXXXXX";


                        strStreet = "XXXXXXX";
                        strCity = "XXXXXXX";
                        strState = "XX";
                        strZipCd =  "XXXXXXX";
                    }



                        strModelId = (dr["model_id"] != DBNull.Value ? dr["model_id"].ToString() : null);
                    if (strModelId == "2")
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateND"]; //MODEL 2
                    }

                    //string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value && dr["Folder_Name"] + "" != "" ? dr["Folder_Name"].ToString().Trim() + "\\" : "");
                    string strFolderNameTmp = "";
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

                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    if (blHasWord)
                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


                    strFinalReportFileName = strTaxIDLabel + "_" + strCorpOwnerName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_Oncology_" + strMonthYear;


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

                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();


                    if (blHasWord)
                    {
                        //OPEN WORD DOCUMENT
                        MSWord.openWordDocument();


                        //GENERAL PLACE HOLDERS. WE USE VARIABLES TO REPLACE PLACEHOLDERS WITHIN THE WORD DOC

                        MSWord.wordReplace("{$Date}", strStartDate);
                        MSWord.wordReplace("{$PracticeName}", strCorpOwnerName);
                        MSWord.wordReplace("{$Address1}", strStreet );
                        MSWord.wordReplace("{$Address2}", strStreet);
                        MSWord.wordReplace("{$City}", strCity);
                        MSWord.wordReplace("{$State}", strState);
                        MSWord.wordReplace("{$ZIPCode}", strZipCd);
                        MSWord.wordReplace("{$TIN}", strTaxIDLabel);

                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    strSheetname = "General Info";




                    //MSExcel.addValueToCell(strSheetname, "B1", strTaxIDLabel);


                    MSExcel.addValueToCell(strSheetname, "A3", strCorpOwnerName);

                    MSExcel.addValueToCell(strSheetname, "A4", strStreet);
                    MSExcel.addValueToCell(strSheetname, "A5", strCity + ", " + strState + " " + strZipCd);

                    MSExcel.addValueToCell(strSheetname, "B7", strTaxIDLabel);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    strBookmarkName = "all_perform_meas";
                    //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                    //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                    strSQL = "SELECT act_display, expected_display, signif , Favorable FROM Ph34.PBP_Profile_onc where taxid=" + strTaxID +" order by measure_id;";
                    dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {
                        strSheetname = "all_perform_meas";

                        //alSection.Add(strSheetname);


                        MSExcel.populateTable(dt, strSheetname, 3, 'C');

                        MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "{$Group_Name}", strCorpOwnerName);

                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F7", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);

                            MSWord.deleteBookmarkComplete(strBookmarkName);
                        }
                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if(strModelId == "1")
                    {

                        strBookmarkName = "drilldown_tables";


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 37 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 37 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=69 and TIN=" + strTaxID + " order by catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Admit_All_Cause_sum_det";
                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "{$Group_Name}", strCorpOwnerName);

                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":D" + (intEndingRowTmp), strSheetname);
                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 43 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 43 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=68 and TIN=" + strTaxID + " order by catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "ER_All_Cause_sum_det";
                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "{$Group_Name}", strCorpOwnerName);

                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":D" + (intEndingRowTmp), strSheetname);
                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select Category, Patient_Count, Visit_Count , Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 17 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost,Catg_order from Ph34.PBP_act where Measure_ID eq 17 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=67 and TIN=" + strTaxID + " order by catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Admit_sum_det";
                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "{$Group_Name}", strCorpOwnerName);

                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":D" + (intEndingRowTmp), strSheetname);
                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "ER_sum_det";
                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "{$Group_Name}", strCorpOwnerName);

                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":D" + (intEndingRowTmp), strSheetname);
                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        strSQL = "select Num_Hospice, Denom_Hospice,Rate_display from onc.hospice_drilldown where TaxID=" + strTaxID + ";";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "hospice_det";

                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'B');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "{$Group_Name}", strCorpOwnerName);


                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":D" + (intEndingRowTmp), strSheetname);

                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }


                        MSWord.deleteBookmarkComplete(strBookmarkName);


                        //UNCOMMENT ME!!!!!
                        if (blHasWord)
                        {
                            processBreaks(alSection, 1);
                            processTopBreaks(alSection, 1);

                        }


                    }
                    else if (strModelId == "3")
                    {

                        strBookmarkName = "drilldown_tables";


                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        strSQL = "select Num_Hospice, Denom_Hospice,Rate_display from onc.hospice_drilldown where TaxID=" + strTaxID + ";";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "hospice_det";

                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'B');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "{$Group_Name}", strCorpOwnerName);


                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":D" + (intEndingRowTmp), strSheetname);

                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }


                        MSWord.deleteBookmarkComplete(strBookmarkName);


                        //UNCOMMENT ME!!!!!
                        if (blHasWord)
                        {
                            processBreaks(alSection, 1);
                            processTopBreaks(alSection, 1);

                        }


                    }


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
                    Console.WriteLine("Closing Adobe Acrobat Instance...");
                    //CLOSE ADOBE APP
                    //AdobeAcrobat.closeAcrobat();

                    Console.WriteLine("Closing Microsoft Excel Instance...");
                    //CLOSE EXCEL APP
                    MSExcel.closeExcelApp();

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
