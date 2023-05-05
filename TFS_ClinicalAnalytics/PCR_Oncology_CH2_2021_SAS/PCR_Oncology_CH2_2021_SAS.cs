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

namespace PCR_Oncology_CH2_2021_SAS
{
    class PCR_Oncology_CH2_2021_SAS
    {
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


                //blVisibleWord = true;



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

                bool blIsMasked = true;


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
                //string strModelId;

                //SELECT taxid, count(Measure_desc) as cnt FROM Ph35.PBP_Profile_onc group by taxid order by cnt desc
                string strSampleCount = "50";
                string strTinList = "SELECT d.TaxID FROM ph35.Onc_TIN_demog as d inner join ph35.ONC_TIN_ADDR as a on d.TaxID = a.TaxID inner join ph35.ONC_NWK_REVIEW as n on n.taxid = d.taxid where d.exclude is null";
                //strTinList = "860461589";
                //strTinList = "1773680, 682172, 75091, 110728, 2230565, 1802665, 1164884, 278225";//BOTH


                ArrayList alSection = new ArrayList();


                //strTinList = "470376604,362167864,450226700";
                //strTinList = "943246234, 560543246, 520591657, 141799724, 383435392, 383369578, 561176820, 880438553, 621099283, 621041368, 411806657, 411804205, 860492210, 42121317, 205755130, 454366531, 203057845, 454361786, 540261840, 530196580, 310645626, 260656038, 431912860, 593443182, 264282204, 43397450";


                //strTinList = "621526296 , 161475278 , 460224743 , 931273254 , 391824445 , 570777346 , 390813418 , 273930084 , 341967952 , 263328413 , 263187119 , 450409348 , 341369963 , 561274107 , 60881828 , 208401637 , 270790915 , 462743463 , 270712680 , 203848077";


                //strTinList = "870281028 , 640619700 , 510064318 , 135598093 , 20222131 , 742823514 ,770356364 , 341935236 , 581953215 , 464705056 , 464534925 , 464519560 ,590624462 , 208484894 , 320356835 , 570944112 , 730612674 , 590657322 , 260707611 , 452047948";

                // strTinList = "20222140 , 201842623 , 340714585 , 526049658 , 452681845 , 384036080 , 562669185 , 208770785 , 480543778 , 850275777 , 582423502 , 340753531 , 411620386 , 311351965 , 481088982 , 580705892 , 581690520 , 911770748 , 582307485 , 370662580 , 202601712 , 371704041 , 582091280 , 339700120 , 61209954 , 581715324 , 582144788 , 221487322 , 221487307 , 10130427 , 990301828 , 582028476 , 581973570 , 581911751 , 331123019 , 582095884 , 20463164 , 582089405 , 10593723 , 340714538 , 582032904 , 582022093 , 581972231 , 136171197 ";

                //strTinList = " 20354549 , 141348692 , 203850829 , 223109987 ,  451154796   300502591 , 510172171 , 300413949 , 134167266 , 320552496 , 362979491 , 640362400 ,  590855412 , 590774199 , 350876394 , 590724459 , 204600249 , 363025341 ,  562135414 , 561965983 ";

                //strTinList = "462942984 , 310957876 ,426004813 , 141338471 ,263674950 , 510511380 , 570314381 , 562187873 ,746000705 , 262400924 , 473238987 , 273667149,362736715 , 630693892 , 208629620 , 208628418 , 113492255 , 202800376, 272606014 , 363139231 ";

                //strTinList = "271444665 , 390816845 , 646008520 ,  352221941 ,  454778647 ,  203059260 , 651205795 , 320058309 ,  222768204 , 60866691 , 341764515 ,   581587307 , 262865516 , 581268632 , 590594631 , 946036494 , 330599494 , 561981186, 591263145 , 311164903 ";

                //strTinList = "42673273 , 205399370, 311158699 , 133739458 , 522070717 , 351754711 , 60990617 ,350593390 , 341832420 , 581662472 , 830382654 , 590637874 ,161611703 , 460676654 , 831025565 , 421493891 ,264394436 , 203064911 ,271094417 , 731477155 ";

                //strTinList = "161611703";

                strTinList = "161611703";

                if (blIsMasked)
                {

                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";

                    //strSQL = "SELECT d.TaxID, d.CorpOwnerName as Practice_Name, a.Street, a.City, a.State, a.ZipCd, o.model_id FROM Ph35.ONC_TIN_OUTLIER as o inner join Ph35.Onc_TIN_demog as d on d.taxid=o.taxid inner join Ph35.ONC_TIN_ADDR as a on d.TaxID=a.TaxID where Exclude is null and d.taxid in (" + strTinList + ")  and o.model_id in (1)  ";


                    strSQL = "SELECT d.TaxID, 'XXXXXXX' as Practice_Name, 'XXXXXXX' as Street, 'XXXXXXX' as City, 'XXXXXXX' as State, 'XXXXXXX' as ZipCd FROM ph35.Onc_TIN_demog as d inner join ph35.ONC_TIN_ADDR as a on d.TaxID=a.TaxID inner join ph35.ONC_NWK_REVIEW as n on n.taxid=d.taxid WHERE d.TaxID in (" + strTinList + ") and d.exclude is null ";

                }
                else
                {
                    //strSQL = "SELECT distinct d.TaxID, d.CorpOwnerName as Practice_Name, a.Street, a.City, a.State, a.ZipCd, o.model_id FROM Ph35.ONC_TIN_OUTLIER as o inner join Ph35.Onc_TIN_demog as d on d.taxid=o.taxid inner join Ph35.ONC_TIN_ADDR as a on d.TaxID=a.TaxID where Exclude is null and d.taxid in (" + strTinList + ") and o.model_id in (1) order by d.TaxID asc ";

                    strSQL = "SELECT d.TaxID, d.TIN_name as Practice_Name, a.Street, a.City, a.State, a.ZipCd FROM ph35.Onc_TIN_demog as d inner join ph35.ONC_TIN_ADDR as a on d.TaxID=a.TaxID inner join ph35.ONC_NWK_REVIEW as n on n.taxid=d.taxid WHERE d.TaxID in (" + strTinList + ") and d.exclude is null ";

                }

                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


                DataTable dtMain = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        strZipCd = "XXXXXXX";
                    }



                    //strModelId = (dr["model_id"] != DBNull.Value ? dr["model_id"].ToString() : null);
                    //if (strModelId == "2")
                    //{
                    //    MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateND"]; //MODEL 2
                    //}

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
                        MSWord.wordReplace("{$Address1}", strStreet);
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

                    MSExcel.addValueToCell(strSheetname, "A14", strTaxIDLabel);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    strBookmarkName = "all_perform_meas";
                    //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                    //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                    //strSQL = "SELECT act_display, expected_display, signif , Favorable FROM Ph34.PBP_Profile_onc where taxid=" + strTaxID + " order by measure_id;";
                    strSQL = "SELECT Measure_desc, Unit_Measure, act_display, expected_display, signif , Favorable FROM Ph35.PBP_Profile_onc where taxid=" + strTaxID + " order by measure_desc;";
                    dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {


                        strSheetname = "all_perform_meas";
                        //alSection.Add(strSheetname);

                        MSExcel.populateTable(dt, strSheetname, 4, 'A');

                        MSExcel.ReplaceInTableTitle("A2:F2", strSheetname, "{$PracticeName}", strCorpOwnerName);

                        intEndingRowTmp = dt.Rows.Count + 3;
                        MSExcel.addBorders("A4" + ":F" + (intEndingRowTmp), strSheetname);
                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);

                            MSWord.deleteBookmarkComplete(strBookmarkName);

                        }
                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strBookmarkName = "drilldown_tables";


                        

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 43 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 43 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=68 and TIN=" + strTaxID + " order by catg_order;";
                        strSQL = "select Category , numerator, Adverse_Event_Count , denominator , rate from ph35.PBP_act_onc where measure_id=73 and TaxID=" + strTaxID + " order by catg_order;";

                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "SOS_Rad";
                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "{$Group_Name}", strCorpOwnerName);

                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":E" + (intEndingRowTmp), strSheetname);
                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //strSQL = "select Category, Patient_Count, Visit_Count , Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 17 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                    //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost,Catg_order from Ph34.PBP_act where Measure_ID eq 17 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                    //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=67 and TIN=" + strTaxID + " order by catg_order;";
                    strSQL = "select Category , numerator , Adverse_Event_Count, denominator , rate from ph35.PBP_act_onc where measure_id=72 and TaxID=" + strTaxID + " order by catg_order;";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "SOS_Chemo";
                            alSection.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "{$Group_Name}", strCorpOwnerName);

                            intEndingRowTmp = dt.Rows.Count + 3;
                            MSExcel.addBorders("A4" + ":E" + (intEndingRowTmp), strSheetname);
                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                            }
                        }



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 37 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                    //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 37 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                    //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=69 and TIN=" + strTaxID + " order by catg_order;";
                    strSQL = "select denom_pt_cnt,overall_rate,star_bmk from PH35.ONC_CMS_TOBAC_unq where TaxID=" + strTaxID + ";";
                    dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {

                        strSheetname = "Prevent_Care_Smoking";
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



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                    //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                    //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                    strSQL = "select Category , numerator , denominator , rate from ph35.PBP_act_onc where measure_id=69 and TaxID=" + strTaxID + " order by catg_order;;";
                    dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count > 0)
                    {

                        strSheetname = "Post_Chemo_Hosp_Admit_det";
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
                    //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                    strSQL = "select Category , numerator , denominator , rate from ph35.PBP_act_onc where measure_id=68 and TaxID=" + strTaxID + " order by catg_order;";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Post_Chemo_ED_A_det";
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
                        //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                        strSQL = "select Numerator,Denom,phy_rate from Ph35.Onc_TIN_Outlier_Rpt where measure_id=75 and Denom>0 and TaxID=" + strTaxID + ";";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Performance_Status";
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


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                        strSQL = "select Numerator,Denom,phy_rate from Ph35.Onc_TIN_Outlier_Rpt where measure_id=74 and Denom>0 and TaxID=" + strTaxID + ";";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Oncology_Pathway_Adherence";
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


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                        //strSQL = "select Numerator,Adverse_Event_Count,Denom,phy_rate from Ph35.Onc_TIN_Outlier_Rpt where measure_id=70 and Denom>0 and TaxID=" + strTaxID + ";";
                        strSQL = "select Numerator,Denom,phy_rate from Ph35.Onc_TIN_Outlier_Rpt where measure_id=70 and Denom>0 and TaxID=" + strTaxID + ";";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "hospice_utilization";
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


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                        strSQL = "select Numerator,Denom,phy_rate from Ph35.Onc_TIN_Outlier_Rpt where measure_id=71 and Denom>0 and TaxID=" + strTaxID + ";";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Admission_ICU";
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


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        //strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        //strSQL = "select Category, numerator, denominator, rate from ph34.PBP_act_onc where measure_id=66 and TIN=" + strTaxID + " order by catg_order;";
                        strSQL = "select Numerator,Denom,phy_rate from Ph35.Onc_TIN_Outlier_Rpt where measure_id=65 and Denom>0 and TaxID=" + strTaxID + ";";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "admission_hospice_";
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



                    //if(intProfileCnt == 21)
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

                // Console.Beep();


                //Console.ReadLine();


            }
            finally
            {

                try
                {
                    DBConnection32.getOleDbDataTableGlobalClose();
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
