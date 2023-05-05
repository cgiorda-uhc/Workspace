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

namespace PCP_CH6_PR_PM_Profiles
{
    class PCP_CH6_PR_PM_Profiles
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

                IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
                IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
                IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
                IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
                IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
                IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
                IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];

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


                bool blHasPharmacy = false;
                bool blHasUtilization = false;

                int intLineBreakCnt = 1;


                int intPageCheck1 = 0;
                int intPageCheck2 = 0;

                //string strTinList = "select distinct ad.MPIN from dbo.PBP_Outl_ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on ad.RGN_NM=r.Region where a.Exclude in(0,5) and b.PTIGroupID>0 and r.phase_id=2";

                string strTinList = "select distinct ad.MPIN  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd where a.Exclude in(0,5) and b.PTIGroupID>0";


                strTinList = "SELECT distinct ad.MPIN FROM ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph16.OUTL_MODELS6 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0";


                //strTinList = "7239366, 7244802, 7265770, 7276506, 7277056, 2174, 18590, 19764 , 22682 , 24440 ,3681,26057,49459,94226,95512";


                // strTinList = "3770549 , 285685 , 2343231 , 2196683 , 5749080 , 757295 , 6298359 , 3710654 , 2512403 , 678021 , 3709175 , 2272888 , 3708516 , 676341 , 3696501 , 2909162 , 95512 , 2897954 , 2895979 , 1834014 , 973223 , 971290 , 2852491";

                //strTinList = "597489, 660984, 1450431, 1569166, 2033568, 2047385, 2410348, 2446873, 2801137, 2803332, 3233521, 3234900, 3258778, 3267203, 3297362, 3430747, 3680612, 5922776,2399247,2668316";



                // strTinList = "1270141";

                //strTinList = "5305612 , 1856773 , 6984550 , 1174112 , 971492 , 497470 , 660984 , 3540497 , 513090 , 1350883 , 6486446 , 3776924 , 7181705 , 533449 , 1333574 , 1005659 , 2497237 , 3663737 , 2354898 , 3483826 , 6892957 , 3873708 , 1354143 , 2739121 , 5861119";

                // strTinList = "5305612";


                //strTinList = "2528168 , 28904 , 7372099 , 6266882 , 2874389 , 1388748 , 6984550 , 1320018 , 1463603 , 1433384 , 2254929 , 3189062 , 3358676 , 729618 , 7475216 , 2481012 , 3146470 , 5496161 , 1972668 , 982359 , 36582 , 801093 , 1405209 , 5174018 , 5182456 , 5194214";


                //strTinList = "1204253, 28904 , 2023118 , 1282810 , 2138004 , 466323 , 3157094 , 6266882 , 2874389 , 1014965 , 63072 , 1388748 , 6984550 , 1320018 , 832092 , 344954 , 1177219 , 2236146 , 5144286 , 2040497 , 7475216 , 5496161 , 1333574 , 1209472 , 1405209 , 357149";

                //strTinList = "1177219";

                // strTinList = "1282810 , 28904 , 2138004 , 466323 , 2230565 , 2188028 , 6436558 , 2663029 , 1890916 , 1338548 , 3292527 , 2383373 , 1378971 , 6445500 , 2184881 , 3587030 , 1324966 , 217100 ,1270141, 5305612, 1324966, 68129, 832092, 466323, 1177219";

                // strTinList = "6984550 , 1320018";


                //strTinList = "1204253 , 1478336 , 1282810 , 3466519 , 2138004 , 6984550 , 1320018 , 1433384 , 2254929 , 1969808 , 3540497 , 2954672 , 2886071 , 2182172 , 3695072 , 7031560 , 7327708 , 2905075 , 6971724 , 3332536 , 1938817 , 1168620 , 2757018 , 7019530 , 5119120 ";

                // strTinList = "28904 , 2023118 , 3695072, 1282810 , 2138004 , 466323 , 3157094 , 6266882 , 2874389 , 1014965";

                // string strCnt = "5";
                //strTinList = "select distinct TOP " + strCnt + " ad.MPIN from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin  where a.Exclude in(0,5) and b.PTIGroupID>0  and indx UNION select distinct TOP " + strCnt + " ad.MPIN from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin  where a.Exclude in(0,5) and b.PTIGroupID>0 GROUP BY ad.MPIN HAVING SUM(ISNULL(Tot_measures,0)) > 0 AND SUM(ISNULL(Tot_PX_meas,0)) <= 0 UNION select distinct TOP " + strCnt + " ad.MPIN from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin  where a.Exclude in(0,5) and b.PTIGroupID>0 GROUP BY ad.MPIN HAVING SUM(ISNULL(Tot_measures,0)) <= 0 AND SUM(ISNULL(Tot_PX_meas,0)) > 0";

                //strTinList = "SELECT Distinct TOP " + strCnt + " ad.MPIN FROM dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 GROUP BY ad.MPIN HAVING sum(model_id)/count(a.mpin)=4 and max(model_id)<>5 UNION ALL SELECT Distinct TOP " + strCnt + " ad.MPIN FROM dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 GROUP BY ad.MPIN HAVING sum(model_id) / count(a.mpin) = 5 UNION ALL SELECT Distinct TOP " + strCnt + " ad.MPIN FROM dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 GROUP BY ad.MPIN HAVING sum(model_id) / count(a.mpin) not in (4,5)";


                //strTinList = "4609813, 109587, 6609361";


                //strTinList = "SELECT MPIN FROM(SELECT TOP 5 MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures > 2 AND ISNULL(Tot_PX_meas, 0) = 0) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT MPIN FROM(SELECT TOP 5 MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_PX_meas > 1 AND ISNULL(Tot_measures, 0) = 0) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT MPIN FROM(SELECT TOP 5 MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures >= 1 AND Tot_PX_meas >= 1) t) tmp ORDER BY NEWID() ) as t3";

                //FOR TESTING ONLY!!!!!
                //string strCnt = "5";
                //strTinList = "SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures > 2 AND ISNULL(Tot_PX_meas, 0) = 0) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_PX_meas > 1 AND ISNULL(Tot_measures, 0) = 0) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct ad.MPIN FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and Tot_measures >= 1 AND Tot_PX_meas >= 1) t) tmp ORDER BY NEWID() ) as t3";


                //strTinList = "367706,411622,793408,809014,1858385,2506660,3226749,3260531,3409540,3466519,3556510,3680612,3725189,3759819,5654059";


                if (blIsMasked)
                {

                    // strSQL = "select distinct a.UHN_TIN as TaxID,'XXXXXXX' as UC_Name,'XXXXXXX' as LC_Name,'XXXXXXX' as Street,'XXXXXXX' as City,'XXXXXXX' as State,'XXXXXXX' as ZipCd, r.RCMO,r.RCMO_title,r.RCMO_title1,Special_Handling,Folder_Name,Recipient from dbo.PBP_Outl_ph1 as a inner join dbo.PBP_outl_demogr_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2 and a.UHN_TIN in (" + strTinList + ")";

                }
                else
                {

                    //strSQL = "SELECT Distinct ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients, sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, (sum(isnull(m.outl_idx_pr,0))+sum(isnull(outl_idx_g_pr,0))) as Tot_measures, (sum(isnull(outl_idx_px,0))+sum(isnull(outl_idx_g_px,0))) as Tot_PX_meas, NULL as Folder_Name,sum(outl_idx) as outl_idx FROM dbo.PBP_Outl_ph13 as a inner join dbo.PBP_outl_demogr_ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_outl_Ph13_models as m on m.MPIN=a.MPIN WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd";


                    //strSQL = "select ad.TaxID,ad.MPIN as PracticeId,ad.Practice_Name,ad.Street,ad.City,ad.State,ad.ZipCd ,sum(a.attr_clients) as attr_clients,sum(a.op_clients) as op_clients, sum(a.abx_clients) as abx_clients, sum(a.medadh_clients) as medadh_clients, SUM(Tot_measures) as Tot_measures, SUM(Tot_PX_meas) AS Tot_PX_meas, h.Folder_Name from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_Ph14 as h on h.mpin=a.mpin where a.Exclude in(0,5) and b.PTIGroupID>0 and ad.MPIN in (" + strTinList + ")  Group By ad.TaxID,ad.MPIN,ad.Practice_Name,ad.Street,ad.City,ad.State,ad.ZipCd,h.Folder_Name";

                    //strSQL = "SELECT ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, case when sum(model_id)/count(a.mpin)=4 and max(model_id)<>5 then 4 when sum(model_id)/count(a.mpin)=5 then 5 else 0 end as idx, Folder_Name FROM dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as ad on ad.mpin=b.PTIGroupID_upd inner join dbo.PBP_spec_handl_Ph14 as h on h.mpin=a.mpin inner join dbo.PBP_outl_ph14_models as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 and ad.MPIN in (" + strTinList + ")  GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd,Folder_Name";


                    strSQL = "SELECT ad.TaxID, ad.MPIN as PracticeId, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd, sum(a.attr_clients) as attr_clients,     sum(a.attr_clients) as attr_clients, case when sum(model_id)/count(a.mpin)=4 and max(model_id)<>5 then 4 when sum(model_id)/count(a.mpin)=5 then 5 else 0 end as idx, '' as Folder_Name FROM ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as ad on ad.mpin=b.PTIGroupID_upd inner join ph16.OUTL_MODELS6 as m on m.mpin=a.mpin WHERE a.Exclude in(0,5) AND b.PTIGroupID>0 AND ad.MPIN in (" + strTinList + ") GROUP BY ad.TaxID, ad.MPIN, ad.Practice_Name, ad.Street, ad.City, ad.State, ad.ZipCd order by idx";


                    //strSQL += " HAVING SUM(Tot_Util_meas) = 0 AND SUM(Tot_PX_meas) > 10";
                }






                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());



                DataTable dtMain = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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

                    //int? outlierIndex = (int?)dr["outl_idx"];
                    blHasPharmacy = true;
                    blHasUtilization = true;
                    int indx = (dr["idx"] != DBNull.Value ? int.Parse(dr["idx"].ToString()) : 0);
                    if (indx == 4)
                    {
                        blHasPharmacy = false;
                    }
                    else if (indx == 5)
                    {
                        blHasUtilization = false;
                    }



                    if (blHasPharmacy && blHasUtilization)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndPharm"];
                    }
                    else if (blHasUtilization)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtil"];
                    }
                    else if (blHasPharmacy)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplatePharm"];
                    }


                    //DELETE ME 2020!!!!!!
                    strFolderNameTmp = "";
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

                        //MSWord.wordReplace("{$Date}", strDisplayDate);


                        MSWord.wordReplace("{$Practice_Name}", strCorpOwnerName);
                        //MSWord.wordReplace("{$Address1}", strStreet);
                        //MSWord.wordReplace("{$City}", strCity);
                        //MSWord.wordReplace("{$State}", strState);
                        //MSWord.wordReplace("{$Zip_Code}", strZipCd);



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


                        //strSQL = "select  d.MPIN,'Dr.'+' '+P_FirstName+' '+P_LastName as dr_info from dbo.PBP_outl_demogr_ph13 as d inner join dbo.PBP_outl_ph13 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";

                        // strSQL = "select d.MPIN as MPIN,CASE WHEN P_FirstName IS NOT NULL THEN 'Dr.'+' '+P_FirstName+' '+P_LastName ELSE P_LastName END as dr_info from dbo.PBP_outl_demogr_ph14 as d inner join dbo.PBP_outl_ph14 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";

                        //strSQL = "select d.PTIGroupID, 'Dr.'||' '||trim(P_FirstName)||' '||trim(P_LastName) as dr_info from ph16.UHN_FEB2_DEMOG as d inner join ph16.outliers6 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + " order by P_LastName";


                        strSQL = "select d.MPIN, TRIM((CASE WHEN P_FirstName IS NOT NULL THEN 'Dr. ' || trim(P_FirstName) || ' ' ELSE '' END ) ||trim(P_LastName)) as dr_info from ph16.UHN_FEB2_DEMOG as d inner join ph16.outliers6 as o on o.MPIN=d.MPIN where Exclude in(0,5) and d.PTIGroupID=" + strPracticeID + "order by P_LastName";



                    }

                    //MASK

                    MSExcel.ReplaceInTableTitle("A1:B1", strSheetname, "{$Group_Name}", strCorpOwnerNameLC );

                    dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    //dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    MSExcel.populateTable(dt, strSheetname, 3, 'A');




                    intEndingRowTmp = dt.Rows.Count + 2;
                    //MSExcel.addBorders("A1" + ":B" + (intEndingRowTmp), strSheetname);

                    if (blHasWord)
                    {
                        MSWord.tryCount = 0;
                        //MSWord.pasteLargeExcelTableToWord("mpin_table", strSheetname, "A1:B" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);
                        MSWord.pasteExcelTableToWord("mpin_table", strSheetname, "A1:B" + (intEndingRowTmp), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet);
                        MSWord.deleteBookmarkComplete("mpin_table");

                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (blHasUtilization)
                    {
                        strSheetname = "All_meas";



                        //strSQL = "select SUM(case when attr_clients<20 then 0 else Outl_idx end) as tot_meas, SUM(case when attr_clients<20 then 0 else Outl_idx_g end) as fav_meas from dbo.PBP_Profile_ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";


                        //strSQL = "select SUM(Outl_idx) as tot_meas, SUM(Outl_idx_g) as fav_meas from dbo.PBP_Profile_ph14 as p inner join dbo.PBP_outl_ph14 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph14 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        //strSQL = "select SUM(p.Outl_idx) as tot_meas, SUM(p.Outl_idx_g) as fav_meas from dbo.PBP_Profile_ph14 as p inner join dbo.PBP_outl_ph14 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph14 as d on o.MPIN=d.MPIN inner join dbo.PBP_outl_ph14_models as m on m.mpin=o.mpin where Exclude in(0,5) and m.model_id<>5 and Measure_ID not in(14,15) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        strSQL = "select  SUM(p.Outl_idx) as tot_meas, SUM(p.Outl_idx_g) as fav_meas from ph16.Profile as p inner join ph16.outliers6 as o on o.MPIN=p.MPIN inner join ph16.UHN_FEB2_DEMOG as d on o.MPIN=d.MPIN inner join ph16.outl_models6 as m on m.mpin=o.mpin where Exclude in(0,5) and m.model_id<>5 and measure_id in (1,2,3,5,37,50) and d.PTIGroupID=" + strPracticeID + "  group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        //dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            MSExcel.populateTable(dt, strSheetname, 2, 'B');


                            //MSExcel.ReplaceInTableTitle("A1:C1", strSheetname, "{$Group_Name}", strCorpOwnerName);


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord("utilization_table", strSheetname, "A1:C7", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                                MSWord.deleteBookmarkComplete("utilization_table");
                            }


                        }
                    }
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (blHasPharmacy)
                    {
                        strSheetname = "Pharmacy_meas";


                        // strSQL = "select Measure_desc,ISNULL(sum(Outl_idx),0) as tot_meas, ISNULL(sum(Outl_idx_g),0) as fav_meas from dbo.PBP_Profile_px_Ph13 as p inner join dbo.PBP_outl_ph13 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph13 as d on o.MPIN=d.MPIN where Exclude in(0,5) and Measure_ID not in(40,41,42,52,53,54,56,57,58,59,60,61) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc having sum(Outl_idx)>0 or sum(Outl_idx_g)>0 order by sort_ID";

                        //strSQL = "select p.Measure_desc, sum(p.Outl_idx) as tot_meas, sum(p.Outl_idx_g) as fav_meas from dbo.PBP_Profile_px_ph14 as p inner join dbo.PBP_outl_ph14 as o on o.MPIN=p.MPIN inner join dbo.PBP_outl_demogr_ph14 as d on o.MPIN=d.MPIN inner join dbo.PBP_outl_ph14_models as m on m.mpin=o.mpin where Exclude in(0,5)  and m.model_id<>4 and Measure_ID in(38,51,55) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";


                        //strSQL = "select Measure_desc, sum(m.Outl_idx) as tot_meas, sum(m.Outl_idx_g) as fav_meas from ph16.Profile_px as p inner join ph16.outliers6 as o on o.MPIN=p.MPIN inner join ph16.UHN_FEB2_DEMOG as d on o.MPIN=d.MPIN inner join ph16.outl_models6 as m on m.mpin=o.mpin where Exclude in(0,5) and m.model_id<>4 and Measure_ID in(38,51,55) and d.PTIGroupID=" + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";


                        strSQL = "select Measure_desc, sum(p.Outl_idx) as tot_meas, sum(p.Outl_idx_g) as fav_meas from ph16.Profile_px as p inner join ph16.outliers6 as o on o.MPIN=p.MPIN inner join ph16.UHN_FEB2_DEMOG as d on o.MPIN=d.MPIN inner join ph16.outl_models6 as m on m.mpin=o.mpin where Exclude in(0,5) and m.model_id<>4 and Measure_ID in(38,51,55) and d.PTIGroupID = " + strPracticeID + " group by d.PTIGroupID,sort_ID,Measure_desc order by sort_ID";

                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        //dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        MSExcel.populateTable(dt, strSheetname, 2, 'A');


                        //MSExcel.ReplaceInTableTitle("A1:C1", strSheetname, "{$Group_Name}", strCorpOwnerNameLC);

                        intEndingRowTmp = 5;//FIRST BLANK ROW
                        if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
                        {
                            intEndingRowTmp = (2 + dt.Rows.Count);//FIRST BORDERED ROW
                            MSExcel.deleteRows("A" + intEndingRowTmp + ":C4", strSheetname);//LAST BORDERED ROW
                            //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":C" + (intEndingRowTmp - 1), strSheetname);

                        }


                        if (blHasWord)
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord("pharmacy_table", strSheetname, "A1:C" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
                            MSWord.deleteBookmarkComplete("pharmacy_table");

                        }
                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasWord)
                    {
                        if (blHasUtilization && blHasPharmacy)
                        {
                            intPageCheck1 = MSWord.getPageNumber("utilization_start");
                            intPageCheck2 = MSWord.getPageNumber("utilization_end");
                            if (intPageCheck1 != intPageCheck2)
                                MSWord.addpageBreak("utilization_start");

                            MSWord.deleteBookmarkComplete("utilization_start");
                            MSWord.deleteBookmarkComplete("utilization_end");

                            intPageCheck1 = MSWord.getPageNumber("pharmacy_start");
                            intPageCheck2 = MSWord.getPageNumber("pharmacy_end");
                            if (intPageCheck1 != intPageCheck2)
                                MSWord.addpageBreak2("pharmacy_start");//NO LINEBREAK!

                            MSWord.deleteBookmarkComplete("pharmacy_start");
                            MSWord.deleteBookmarkComplete("pharmacy_end");

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
                        else if (blHasPharmacy)
                        {
                            intPageCheck1 = MSWord.getPageNumber("pharmacy_start");
                            intPageCheck2 = MSWord.getPageNumber("pharmacy_end");
                            if (intPageCheck1 != intPageCheck2)
                                MSWord.addpageBreak("pharmacy_start");

                            MSWord.deleteBookmarkComplete("pharmacy_start");
                            MSWord.deleteBookmarkComplete("pharmacy_end");
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



                try
                {
                    DBConnection32.getOleDbDataTableGlobalClose();
                    IR_SAS_Connect.destroy_SAS_instance();


                }
                catch (Exception)
                {

                }




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
