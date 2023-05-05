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

namespace PCP_CH3_PR_PM_Profiles
{
    class PCP_CH3_PR_PM_Profiles
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
                //string strRCMO;
                //string strRCMO_title;
                //string strRCMO_title1;


                bool blHasProcedural = false;
                bool blHasUtilization = false;

                int intLineBreakCnt = 1;


                int intPageCheck1 = 0;
                int intPageCheck2 = 0;

                //string strTinList = "select distinct ad.MPIN from dbo.PBP_Outl_ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and b.PTIGroupID>0 and r.phase_id=2";

                string strTinList = "select Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0";


                //strTinList = "SELECT MPIN FROM(SELECT TOP 5 MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures > 2 AND ISNULL(Tot_PX_meas, 0) = 0) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT MPIN FROM(SELECT TOP 5 MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_PX_meas > 1 AND ISNULL(Tot_measures, 0) = 0) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT MPIN FROM(SELECT TOP 5 MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures >= 1 AND Tot_PX_meas >= 1) t) tmp ORDER BY NEWID() ) as t3";

                //FOR TESTING ONLY!!!!!
                //string strCnt = "5";
                //strTinList = "SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures > 2 AND ISNULL(Tot_PX_meas, 0) = 0) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_PX_meas > 1 AND ISNULL(Tot_measures, 0) = 0) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures >= 1 AND Tot_PX_meas >= 1) t) tmp ORDER BY NEWID() ) as t3";


                //strTinList = "1773680, 682172, 75091, 110728, 2230565, 1802665, 1164884, 278225";//BOTH
                //strTinList = "199773, 3209425, 1943764, 3228026, 3449085, 6212319, 4977505";//UTIL ONLY
                //strTinList = "238232,2397262,304860,186301,3130383,642509,180765,3362723,1698660,2503699";//PROC ONLY


                //strTinList = "5305612,3151588,326582";
                //strTinList = "2646109, 3116204, 3151588, 3192417, 326582, 4997857, 609818, 772906";

                //strTinList = "609818,4997857,772906,3151588,326582,3192417,3116204,2646109";

                //strTinList = "2009437,1079361";

                //strTinList = "2230565,2553391,2075269";



                //strTinList = "367706,411622,793408,809014,1858385,2506660,3226749,3260531,3409540,3466519,3556510,3680612,3725189,3759819,5654059";


                if (blIsMasked)
                {

                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";

                }
                else
                {
                    //strSQL = "SELECT ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, RCMO, RCMO_title, RCMO_title1, Special_Handling, Folder_Name, Recipient, SUM(Tot_Util_meas) as Tot_Util_meas, SUM(Tot_PX_meas) as Tot_PX_meas FROM dbo.PBP_Outl_ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND r.phase_id=2  and ad.MPIN in (" + strTinList + ") Group by ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, RCMO, RCMO_title, RCMO_title1, Special_Handling, Folder_Name, Recipient";


                    //strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, b.Street, b.City, b.State, b.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, a.Tot_measures, a.Tot_PX_meas, NULL as Folder_Name FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, b.Street, b.City, b.State, b.ZipCd, a.Tot_measures, a.Tot_PX_meas";

                   // strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, sum(a.Tot_measures) as Tot_measures, sum(a.Tot_PX_meas) as Tot_PX_meas, NULL as Folder_Name FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in  (" + strTinList + ")  GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd";


                    //strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, sum(a.Tot_measures) as Tot_measures, sum(a.Tot_PX_meas) as Tot_PX_meas, NULL as Folder_Name , sum(outl_idx) as outl_idx FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd left join (select mpin, CAST(sum(outl_idx) as int) as outl_idx from PBP_Profile_Ph13 group by mpin) as pr on pr.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd";


                    //strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, (sum(m.outl_idx_pr)+sum(outl_idx_g_pr)) as Tot_measures, (sum(outl_idx_px)+sum(outl_idx_g_px)) as Tot_PX_meas, NULL as Folder_Name,sum(outl_idx) as outl_idx FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd";


                    strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, (sum(isnull(m.outl_idx_pr,0))+sum(isnull(outl_idx_g_pr,0))) as Tot_measures, (sum(isnull(outl_idx_px,0))+sum(isnull(outl_idx_g_px,0))) as Tot_PX_meas, NULL as Folder_Name,sum(outl_idx) as outl_idx FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd";



                    //strSQL += " HAVING SUM(Tot_Util_meas) = 0 AND SUM(Tot_PX_meas) > 10";
                }




                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {



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
                    //strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "VALUE MISSING");
                    //strRCMO_title = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "VALUE MISSING");
                    //strRCMO_title1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "VALUE MISSING");




                    //string strRCMOFirst = null;
                    //string strRCMOLast = null;

                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

                    string strFolderName = "";

                    string strBulkPath = "";


                    string attr_clients = (dr["attr_clients"] != DBNull.Value ? dr["attr_clients"].ToString().Trim() : "CLIENTS MISSING");
                    //int? outlierIndex = (int?)dr["outl_idx"];

                    int proceudralCount = (dr["Tot_PX_meas"] != DBNull.Value ? int.Parse(dr["Tot_PX_meas"].ToString()) : 0);
                    int utilizationCount = (dr["Tot_measures"] != DBNull.Value ? int.Parse(dr["Tot_measures"].ToString()) : 0);
                    blHasProcedural = (proceudralCount > 0 ? true : false);
                    blHasUtilization = (utilizationCount > 0 ? true : false);


                    //if (blHasProcedural == true && blHasUtilization == false)
                    //{
                    //    int n;
                    //    bool isNumeric = int.TryParse(attr_clients, out n);


                    //    if (isNumeric)
                    //    {
                    //        if (n >= 20)
                    //        {
                    //            //strSQL = "Select act_display, expected_display, var_display, signif, favorable FROM PBP_Profile_Ph13 where mpin=" + strMPIN + " order by sort_id";
                    //            strSQL = "select SUM(Outl_idx) as tot_meas, SUM(Outl_idx_g) as fav_meas from dbo.PBP_Profile_ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";
                    //            ///strSQL = "SELECT count(*) FROM [IL_UCA].[dbo].[PBP_outl_Ph13] where exclude in(0,5) and [Tot_measures] is null and mpin = " + strMPIN;
                    //            dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    //            if (dt.Rows.Count > 0)
                    //            {
                    //                if (outlierIndex >= 1)
                    //                {
                    //                    blHasUtilization = true;
                    //                }
                    //                //else if (outlierIndex == 0)
                    //                //{
                    //                //    blHasException = true;
                    //                //}

                    //            }

                    //        }
                    //    }

                    //}



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
                    if (!blOverwriteExisting && blHasWord)
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

                        MSWord.wordReplace("{$Date}", strDisplayDate);


                        MSWord.wordReplace("{$Practice_Name}", strCorpOwnerName);
                        MSWord.wordReplace("{$Address1}", strStreet);
                        MSWord.wordReplace("{$City}", strCity);
                        MSWord.wordReplace("{$State}", strState);
                        MSWord.wordReplace("{$Zip_Code}", strZipCd);



                        //MSWord.wordReplace("{$RCMO}", strRCMO);
                        //MSWord.wordReplace("{$RCMO_Title}", strRCMO_title);

                        MSWord.wordReplace("{$Provider_TIN}", strTaxIDLabel);

                        //if (strRCMO == "Jack S. Weiss, M.D.")
                        //{
                        //    strRCMOFirst = "Jack";
                        //    strRCMOLast = "Weiss";
                        //}
                        //else
                        //{
                        //    strRCMOFirst = "Janice";
                        //    strRCMOLast = "Huckaby";
                        //}


                        //MSWord.addSignature(strRCMOFirst, strRCMOLast);

                        //MSWord.deleteBookmarkComplete("signature");

                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    strSheetname = "General Info";


                    MSExcel.addValueToCell(strSheetname, "B3", strTaxID);


                    //MSExcel.addValueToCell(strSheetname, "B1", strPracticeID);


                    MSExcel.addValueToCell(strSheetname, "A5", strCorpOwnerName);

                    MSExcel.addValueToCell(strSheetname, "A6", strStreet);
                    MSExcel.addValueToCell(strSheetname, "A7", strCity + ", " + strState + " " + strZipCd);



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

                        //strSQL = "select d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph32 as d inner join dbo.PBP_outl_ph32 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";


                        //strSQL = "select d.PTIGroupID,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph13 as d inner join dbo.PBP_outl_ph13 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";

                        strSQL = "select  d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph13 as d inner join dbo.PBP_outl_ph13 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";


                    }

                    //MASK

                    MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "{$Group_Name}", strCorpOwnerNameLC);


                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 3, 'A');


                    

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
                        strSheetname = "All_meas";


                        //strSQL = "select SUM(Outl_idx) as tot_meas from dbo.PBP_Profile_ph32 as p inner join dbo.PBP_outl_ph32 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph32 as d on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc,measure_id order by sort_ID";

                        //strSQL = "select SUM(Outl_idx) as tot_meas, SUM(Outl_idx_g) as fav_meas from dbo.PBP_Profile_ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        strSQL = "select SUM(case when attr_clients<20 then 0 else Outl_idx end) as tot_meas, SUM(case when attr_clients<20 then 0 else Outl_idx_g end) as fav_meas from dbo.PBP_Profile_ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            MSExcel.populateTable(dt, strSheetname, 3, 'B');


                            MSExcel.ReplaceInTableTitle("A1:C1", strSheetname, "{$Group_Name}", strCorpOwnerName);


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord("utilization_table", strSheetname, "A1:C14", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
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


                       // strSQL = "select Measure_desc, SUM(Outl_idx) as tot_meas from dbo.PBP_Profile_px_ph32 as p inner join dbo.PBP_outl_ph32 as o on o.MPIN = p.MPIN inner join dbo.PBP_outl_demogr_ph32 as d on o.MPIN = d.MPIN where Exclude in(0,5) and Outl_idx is not null and d.PTIGroupID = " + strPracticeID + " group by d.PTIGroupID,Measure_desc,measure_id order by measure_id";

                        //strSQL = "select Measure_desc,Count(signif) as tot_meas, Count(signif_g) as fav_meas from dbo.PBP_Profile_px_Ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(40,41,42,52,53,54,56,57,58,59,60,61) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        strSQL = "select Measure_desc,ISNULL(sum(Outl_idx),0) as tot_meas, ISNULL(sum(Outl_idx_g),0) as fav_meas from dbo.PBP_Profile_px_Ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(40,41,42,52,53,54,56,57,58,59,60,61) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc having sum(Outl_idx)>0 or sum(Outl_idx_g)>0 order by sort_ID";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        MSExcel.populateTable(dt, strSheetname, 3, 'A');


                        MSExcel.ReplaceInTableTitle("A1:C1", strSheetname, "{$Group_Name}", strCorpOwnerNameLC);

                        intEndingRowTmp = 6;//FIRST BLANK ROW
                        if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
                        {
                            intEndingRowTmp = (3 + dt.Rows.Count);//FIRST BORDERED ROW
                            MSExcel.deleteRows("A" + intEndingRowTmp + ":C5", strSheetname);//LAST BORDERED ROW
                            MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":C" + (intEndingRowTmp - 1), strSheetname);

                        }




                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord("procedure_table", strSheetname, "A1:C" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                            MSWord.deleteBookmarkComplete("procedure_table");

                        }
                    }

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //strBookmarkName = "appendix";

                    ////MSWord.addLineBreak(strBookmarkName);

                    //if (blHasProcedural)
                    //{
                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_pg2", "A1:C3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_pg1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

                    //}

                    //if (blHasUtilization)
                    //{


                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_2", "A1:C6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                    //    MSWord.tryCount = 0;
                    //    MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_1", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

                    //}

                    //MSWord.deleteBookmarkComplete(strBookmarkName);

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasWord)
                    {
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
