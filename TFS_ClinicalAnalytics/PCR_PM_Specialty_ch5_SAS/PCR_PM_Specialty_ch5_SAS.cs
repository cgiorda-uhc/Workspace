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

namespace PCR_PM_Specialty_ch5_SAS
{
    class PCR_PM_Specialty_ch5_SAS
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


                bool blHasProcedural = false;
                bool blHasUtilization = false;


                int intPageCheck1 = 0;
                int intPageCheck2 = 0;

                string strSampleCount = "50";
                string strTinList = "SELECT distinct ad.MPIN FROM ph35.outliers8 as a inner join ph35.UHN_JUN1_DEMOG as b on a.MPIN=b.MPIN inner join ph35.UHN_JUN1_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph35.OUTL_MODELS8 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0";

                //strTinList = "1773680, 682172, 75091, 110728, 2230565, 1802665, 1164884, 278225";//BOTH
                //strTinList = "199773, 3209425, 1943764, 3228026, 3449085, 6212319, 4977505";//UTIL ONLY
                //strTinList = "238232,2397262,304860,186301,3130383,642509,180765,3362723,1698660,2503699";//PROC ONLY





                // strTinList = "1001139";

                //strTinList = "3466519, 1815880, 2230565, 2810314, 2480343, 405882, 2408010, 3482043, 75091, 2022442, 2129303, 3541175, 3122208, 3809152, 2876823, 6465128, 5687113, 288921";

               // strTinList = "5687113, 288921, 2490139, 2182674 , 2038777, 3441823, 461369,  3261964, 2408010, 3482043, 729618, 706140, 1419559, 2819451, 5304026, 100088, 1314859, 3466519, 2230565, 2810314, 3265512, 387283, 3386511, 3379350, 3152103";

                if (blIsMasked)
                {

                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";

                }
                else
                {




                    //strSQL = "SELECT ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.opi_clients) as op_clients, utl_meas as Tot_util_meas, px_meas  AS Tot_PX_meas, Folder_Name, '' as RCMO, '' as RCMO_title, '' as RCMO_title1, '' as NDB_Specialty FROM ph34.outliers as a inner join ph34.UHN_MAY6_DEMOG as b on a.MPIN=b.MPIN inner join Ph34.UHN_MAY6_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join Ph34.spec_handling as h on h.mpin=a.mpin inner join Ph34.OUTL_MODELS as m on m.mpin=a.mpin left join (select PTIGroupID,SUM(outl_idx)+SUM(outl_idx_g) as utl_meas from Ph34.PBP_PROFILE as p inner join Ph34.outliers as o on o.mpin=p.mpin inner join ph34.UHN_MAY6_DEMOG as d on o.MPIN=d.MPIN where attr_clients>=20 group by d.PTIGroupID having utl_meas>0) as ut on ut.PTIGroupID=ad.MPIN left join (select PTIGroupID,SUM(outl_idx)+SUM(outl_idx_g) as px_meas from Ph34.PBP_PROFILE_PX as p inner join Ph34.outliers as o on o.mpin=p.mpin inner join ph34.UHN_MAY6_DEMOG as d on o.MPIN=d.MPIN where o.exclude in(0,5) and Measure_ID not in(40,41,42,44) and Outl_idx is not null group by d.PTIGroupID having px_meas>0) as px on px.PTIGroupID=ad.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND Folder_name is null GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd ,Folder_Name;";


                    strSQL = "SELECT ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, case when sum(model_id)/count(a.mpin)=4 and max(model_id)<>5 then 4 when sum(model_id)/count(a.mpin)=5 then 5 else 0 end as idx, '' as RCMO, '' as RCMO_title, '' as RCMO_title1, '' as NDB_Specialty  FROM ph35.outliers8 as a inner join ph35.UHN_JUN1_DEMOG as b on a.MPIN=b.MPIN inner join ph35.UHN_JUN1_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph35.OUTL_MODELS8 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY  ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd ORDER BY idx;";


                }

                Console.WriteLine("Connecting to SAS Server...");
                //IR_SAS_Connect.create_SAS_instance("Ph35", "/optum/uhs/01datafs/phi/projects/analytics/pbp/Ph35");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());

                DataTable dtMain = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {


                    //if (intProfileCnt < 76 || intProfileCnt > 95)
                    //{
                    //    intProfileCnt++;
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


                    string strSpecialty = (dr["NDB_Specialty"] != DBNull.Value ? dr["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");
                    //string strSpecialtyProperCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strSpecialty.ToLower()).Replace(" And ", " and ");



                    strStreet = (dr["Street"] != DBNull.Value ? dr["Street"].ToString().Trim() : "VALUE MISSING");
                    strCity = (dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "VALUE MISSING");
                    strState = (dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "VALUE MISSING");
                    strZipCd = (dr["ZipCd"] != DBNull.Value ? dr["ZipCd"].ToString().Trim() : "VALUE MISSING");
                    strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "VALUE MISSING");
                    strRCMO_title = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "VALUE MISSING");
                    strRCMO_title1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "VALUE MISSING");


                    //string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value && dr["Folder_Name"] + "" != "" ? dr["Folder_Name"].ToString().Trim() + "\\" : "");
                    string strFolderNameTmp = "";
                    string strFolderName = "";

                    string strBulkPath = "";




                    // strSQL = "SELECT SUM(p.outl_idx) + SUM(p.outl_idx_g) as all_util FROM dbo.PBP_Profile_ph33 as p inner join dbo.PBP_Outl_ph33 as o on o.mpin = p.mpin inner join dbo.PBP_outl_demogr_ph33 as d on o.MPIN = d.MPIN WHERE attr_clients >= 20 AND o.exclude in(0, 5) AND d.PTIGroupID = " + strPracticeID ;






                    //strSQL = "SELECT SUM(Outl_idx) + SUM(Outl_idx_g) as all_proc FROM dbo.PBP_Profile_px_ph33 as p inner join dbo.PBP_outl_ph33 as o on o.MPIN = p.MPIN inner join dbo.PBP_outl_demogr_ph33 as d on o.MPIN = d.MPIN WHERE Exclude in(0, 5) AND Measure_ID not in(40,41,42,44) AND Outl_idx is not null AND d.PTIGroupID = " + strPracticeID;



                    int idx = (dr["idx"] != DBNull.Value ? int.Parse(dr["idx"].ToString()) : 0);

                    //int proceudralCount = (dr["Tot_PX_meas"] != DBNull.Value ? int.Parse(dr["Tot_PX_meas"].ToString()) : 0);
                    //int utilizationCount = (dr["Tot_Util_meas"] != DBNull.Value ? int.Parse(dr["Tot_Util_meas"].ToString()) : 0);
                    //blHasProcedural = (proceudralCount > 0 ? true : false);
                    //blHasUtilization = (utilizationCount > 0 ? true : false);

                    //case when sum(model_id)/count(a.mpin)=4 and max(model_id)<>5 then 4 when sum(model_id)/count(a.mpin)=5 then 5 else 0 end as idx
                    //idx = 4 ? blHasUtilization
                    //idx = 5 ? blHasProcedural 
                    //idx = 0 ? blHasProcedural && blHasUtilization
                    if (idx == 4)
                    {
                        blHasProcedural = false;
                        blHasUtilization = true;
                    }
                    else if (idx == 5)
                    {
                        blHasProcedural = true;
                        blHasUtilization = false;
                    }
                    else if (idx == 0)
                    {
                        blHasProcedural = true;
                        blHasUtilization = true;
                    }


                    //idx?????
                    if (blHasProcedural && blHasUtilization)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProc"];
                    }
                    else if (blHasUtilization)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtil"];
                    }
                    else if (blHasProcedural)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateProc"];
                    }



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

                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();


                    if (blHasWord)
                    {
                        //OPEN WORD DOCUMENT
                        MSWord.openWordDocument();


                        //GENERAL PLACE HOLDERS. WE USE VARIABLES TO REPLACE PLACEHOLDERS WITHIN THE WORD DOC


                        MSWord.wordReplace("{$Practice_Name}", strCorpOwnerName);
                        MSWord.wordReplace("{$Provider_TIN}", strTaxIDLabel);
                        MSWord.wordReplace("{$Provider_Specialty}", strSpecialty);//strSpecialty????


                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    strSheetname = "General Info";




                    MSExcel.addValueToCell(strSheetname, "B1", strPracticeID);


                    MSExcel.addValueToCell(strSheetname, "A3", strCorpOwnerName);

                    MSExcel.addValueToCell(strSheetname, "A4", strStreet);
                    MSExcel.addValueToCell(strSheetname, "A5", strCity + ", " + strState + " " + strZipCd);

                    MSExcel.addValueToCell(strSheetname, "B7", strTaxIDLabel);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    strSheetname = "MPIN_List";

                    if (blIsMasked)
                    {


                        //strSQL = "select a.MPIN,'Dr.XXXXXXXXXXXXXX' as dr_info   from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN where b.Exclude in(0,4) and attr_cl_rem1>=20 and a.UHN_TIN=" + strTaxID;


                    }
                    else
                    {

                        //strSQL = "select d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph33 as d inner join dbo.PBP_outl_ph33 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";

                       // strSQL = "select d.MPIN, case when P_FirstName is null then trim(P_LastName) else 'Dr.'||' '||trim(P_FirstName)||' '||trim(P_LastName) end as dr_info from ph34.UHN_MAY6_DEMOG as d inner join ph34.outliers as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName;";

                        strSQL = "select d.MPIN, case when P_FirstName is null then trim(P_LastName) else 'Dr.'||' '||trim(P_FirstName)||' '||trim(P_LastName) end as dr_info from ph35.UHN_JUN1_DEMOG as d inner join ph35.outliers8 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName;";

                    }

                    //MASK


                    dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 3, 'A');


                    MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "<Practice_Name>", strCorpOwnerNameLC);

                    intEndingRowTmp = dt.Rows.Count + 2;
                    MSExcel.addBorders("A1" + ":B" + (intEndingRowTmp), strSheetname);

                    if (blHasWord)
                    {
                        MSWord.tryCount = 0;
                        MSWord.pasteLargeExcelTableToWord("mpin_table", strSheetname, "A1:B" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);
                        MSWord.deleteBookmarkComplete("mpin_table");

                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (blHasUtilization)
                    {
                        strSheetname = "Utiliz_meas";


                        //strSQL = "select SUM(p.outl_idx) as tot_meas,SUM(p.outl_idx_g) as fav_meas from dbo.PBP_Profile_ph33 as p inner join dbo.PBP_Outl_ph33 as o on o.mpin=p.mpin inner join dbo.PBP_outl_demogr_ph33 as d on o.MPIN=d.MPIN where attr_clients>=20 and o.exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID ";

                        //strSQL = "select SUM(outl_idx) as tot_meas,SUM(outl_idx_g) as fav_meas from Ph34.PBP_PROFILE as p inner join Ph34.outliers as o on o.mpin=p.mpin inner join ph34.UHN_MAY6_DEMOG as d on o.MPIN=d.MPIN where attr_clients>=20 and o.exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID;";

                        strSQL = "select SUM(outl_idx) as tot_meas,SUM(outl_idx_g) as fav_meas from Ph35.PBP_PROFILE as p inner join Ph35.outliers8 as o on o.mpin=p.mpin inner join ph35.UHN_JUN1_DEMOG as d on o.MPIN=d.MPIN where attr_clients>=20 and o.exclude in(0,5) and  measure_id in (1,2,3,5,29,37,43,35) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID;";


                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            MSExcel.populateTable(dt, strSheetname, 3, 'B');


                            MSExcel.ReplaceInTableTitle("A1:C1", strSheetname, "<Practice_Name>", strCorpOwnerName);


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord("utilization_table", strSheetname, "A1:C10", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                MSWord.deleteBookmarkComplete("utilization_table");
                            }


                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (blHasProcedural)
                    {
                        strSheetname = "Pharmacy_meas";

                        //strSQL = "select Measure_desc, SUM(Outl_idx) as tot_meas, SUM(Outl_idx_g) as fav_meas from dbo.PBP_Profile_px_ph33 as p inner join dbo.PBP_outl_ph33 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph33 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(40,41,42,44) and Outl_idx is not null and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                       // strSQL = "select measure_desc,SUM(outl_idx) as tot_meas,SUM(outl_idx_g) as fav_meas from Ph34.PBP_PROFILE_PX as p inner join Ph34.outliers as o on o.mpin=p.mpin inner join ph34.UHN_MAY6_DEMOG as d on o.MPIN=d.MPIN where o.exclude in(0,5) and Measure_ID not in(40,41,42,44) and Outl_idx is not null and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID;";

                        strSQL = "select measure_desc,SUM(outl_idx) as tot_meas,SUM(outl_idx_g) as fav_meas from Ph35.PBP_PROFILE_PX as p inner join Ph35.outliers8 as o on o.mpin=p.mpin inner join ph35.UHN_JUN1_DEMOG as d on o.MPIN=d.MPIN where o.exclude in(0,5) and Measure_ID in(38) and Outl_idx is not null and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID;";

                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

                        MSExcel.populateTable(dt, strSheetname, 3, 'A');


                        MSExcel.ReplaceInTableTitle("A1:C1", strSheetname, "<Practice_Name>", strCorpOwnerNameLC);


                        intEndingRowTmp = dt.Rows.Count + 2;
                        MSExcel.addBorders("A1" + ":C" + (intEndingRowTmp), strSheetname);

                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord("procedure_table", strSheetname, "A1:C" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                            MSWord.deleteBookmarkComplete("procedure_table");

                        }
                    }



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    if (blHasUtilization && blHasProcedural)
                    {
                        intPageCheck1 = MSWord.getPageNumber("utilization_start");
                        intPageCheck2 = MSWord.getPageNumber("utilization_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak("utilization_start");

                        MSWord.deleteBookmarkComplete("utilization_start");
                        MSWord.deleteBookmarkComplete("utilization_end");

                        intPageCheck1 = MSWord.getPageNumber("procedure_start");
                        intPageCheck2 = MSWord.getPageNumber("procedure_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak2("procedure_start");//NO LINEBREAK!

                        MSWord.deleteBookmarkComplete("procedure_start");
                        MSWord.deleteBookmarkComplete("procedure_end");

                    }
                    else if (blHasUtilization)
                    {
                        intPageCheck1 = MSWord.getPageNumber("utilization_start");
                        intPageCheck2 = MSWord.getPageNumber("utilization_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak("utilization_start");

                        MSWord.deleteBookmarkComplete("utilization_start");
                        MSWord.deleteBookmarkComplete("utilization_end");
                    }
                    else if (blHasProcedural)
                    {
                        intPageCheck1 = MSWord.getPageNumber("procedure_start");
                        intPageCheck2 = MSWord.getPageNumber("procedure_end");
                        if (intPageCheck1 != intPageCheck2)
                            MSWord.addpageBreak("procedure_start");

                        MSWord.deleteBookmarkComplete("procedure_start");
                        MSWord.deleteBookmarkComplete("procedure_end");
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

    }
}
