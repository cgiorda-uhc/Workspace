using System;
using System.Data;
using System.Configuration;
using WCDocumentGenerator;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections;



namespace PCP_CH3_PR_Profiles
{
    class PCP_CH3_PR_Profiles
    {
        static void Main(string[] args)
        {

            string strMPIN = null; 
            string strSQL = null;
            int? intModelId = null;

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


                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;
                string strBookmarkName = null;

                ArrayList alSectionUtilization = new ArrayList();
                ArrayList alSectionProcedural = new ArrayList();



                int intProfileCnt = 1;
                int intTotalCnt;


                int intEndingRowTmp;


                // string strMPINList = "select a.MPIN from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2  ";

                string strMPINList = "select distinct  a.MPIN from dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2";

                //FOR TESTING ONLY!!!!!
                string strCnt = "2";
                strMPINList = "SELECT MPIN FROM(SELECT TOP "+ strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 1) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 2) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 3) t) tmp ORDER BY NEWID() ) as t3 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 4) t) tmp ORDER BY NEWID() ) as t4 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 5) t) tmp ORDER BY NEWID() ) as t5";


                //strMPINList = "SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 1) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 2) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 3) t) tmp ORDER BY NEWID() ) as t3 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 5) t) tmp ORDER BY NEWID() ) as t5";


                if (blIsMasked)
                {



                    //strSQL = "select Top 100 a.MPIN,a.attr_clients as clients,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, 'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as [State],'XXXXXXXXX' as zipcd,'XXXXXXXXX' as taxid, 'XXXXXXXXX' as practice_id,'XXXXXXXXX' as Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1, NULL as Folder_Name  from dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";




                }
                else
                {

                   // strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    //strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1, NULL as Folder_Name  from dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    //strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName, FirstName,P_LastName,P_FirstName,ProvDegree, a.NDB_Specialty, b.Street,b.City,b.[State], b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1, NULL as Folder_Name, outl_idx from dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM left join (select mpin, CAST(sum(outl_idx) as int) as outl_idx from PBP_Profile_Ph13 group by mpin) as pr on pr.mpin=a.mpin where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    strSQL = "SELECT Distinct a.MPIN, a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName, FirstName, P_LastName, P_FirstName, ProvDegree, a.NDB_Specialty, b.Street, b.City, b.[State], b.zipcd, b.taxid, p.MPIN as practice_id, p.Practice_Name, Tot_measures, Tot_PX_meas, RCMO, RCMO_title, RCMO_title1, NULL as Folder_Name, model_id FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


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
                    string opioid_clients = (dr["op_clients"] != DBNull.Value ? dr["op_clients"].ToString().Trim() : "OPIOID CLIENTS MISSING");
                    string abx_clients = (dr["abx_clients"] != DBNull.Value ? dr["abx_clients"].ToString().Trim() : "ABX CLIENTS MISSING");
                    string medAdhere_clients = (dr["medadh_clients"] != DBNull.Value ? dr["medadh_clients"].ToString().Trim() : "MED ADHERE CLIENTS MISSING");




                    string practiceName = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "PRACTICE NAME MISSING");

                    //string ocl = (dr["orig_cl"] != DBNull.Value ? dr["orig_cl"].ToString().Trim() : "ZIPCODE MISSING");
                    // string cl_rem1 = (dr["attr_cl_rem1"] != DBNull.Value ? dr["attr_cl_rem1"].ToString().Trim() : "ZIPCODE MISSING");



                    strMPIN = (dr["MPIN"] != DBNull.Value ? dr["MPIN"].ToString().Trim() : "");
                    string strMPINLabel = null;
                    
                    //int? outlierIndex = (int?)dr["outl_idx"];
                    int proceudralCount = (dr["Tot_PX_meas"] != DBNull.Value ? int.Parse(dr["Tot_PX_meas"].ToString()) : 0);
                    int utilizationCount = (dr["Tot_measures"] != DBNull.Value ? int.Parse(dr["Tot_measures"].ToString()) : 0);


                    intModelId = (dr["model_id"] != DBNull.Value ? (int?)dr["model_id"] : null);

                    bool blHasProceduralSummary = false;
                    bool blHasUtilizationSummary = false;
                    bool blHasProceduralDetails = false;
                    bool blHasUtilizationDetails = false;


                    //POPULATE WITH INNA'S NEW DB COLUMNS
                    if(intModelId == 1)
                    {
                        blHasProceduralSummary = true;
                        blHasUtilizationSummary = true;
                        blHasProceduralDetails = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (intModelId == 2)
                    {
                        blHasProceduralSummary = true;
                        blHasUtilizationSummary = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (intModelId == 3)
                    {
                        blHasProceduralSummary = true;
                        blHasUtilizationSummary = true;
                        blHasProceduralDetails = true;
                    }
                    else if (intModelId == 4)
                    {
                        blHasUtilizationSummary = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (intModelId == 5)
                    {
                        blHasProceduralSummary = true;
                        blHasProceduralDetails = true;
                    }


                    if (blHasProceduralSummary && blHasUtilizationSummary && blHasProceduralDetails && blHasUtilizationDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProc"]; //MODEL 1
                    }
                    else if (blHasProceduralSummary && blHasUtilizationSummary  && blHasUtilizationDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProcUtlDetails"];//MODEL 2

                    }
                    else if (blHasProceduralSummary && blHasUtilizationSummary && blHasProceduralDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProcProcDetails"]; //MODEL 3
                    }
                    else if (blHasUtilizationSummary && blHasUtilizationDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtil"]; //MODEL 4
                    }
                    else if (blHasProceduralSummary && blHasProceduralDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateProc"]; //MODEL 5
                    }
                    else
                    {
                        Console.WriteLine("NO TEMPLATE MATCH FOR " + strMPIN);
                        Console.Beep();
                        Console.ReadLine();
                    }






                    //blHasProcedural = (proceudralCount > 0 ? true : false);
                    //blHasUtilization = (utilizationCount > 0 ? true : false);

                    //if (blHasProcedural == true && blHasUtilization == false)
                    //{
                    //    int n;
                    //    bool isNumeric = int.TryParse(attr_clients, out n);


                    //    if (isNumeric)
                    //    {
                    //        if(n >= 20)
                    //        {
                    //            strSQL = "Select act_display, expected_display, var_display, signif, favorable FROM PBP_Profile_Ph13 where mpin=" + strMPIN + " order by sort_id";
                    //            ///strSQL = "SELECT count(*) FROM [IL_UCA].[dbo].[PBP_outl_Ph13] where exclude in(0,5) and [Tot_measures] is null and mpin = " + strMPIN;
                    //            dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    //            if (dt.Rows.Count > 0)
                    //            {
                    //                if (outlierIndex >= 1)
                    //                {
                    //                    blHasUtilization = true;
                    //                }
                    //                else if (outlierIndex == 0)
                    //                {
                    //                    //blHasException = true;
                    //                }

                    //            }

                    //        }
                    //    }

                    //}





                    //if (blHasException)
                    //{
                    //    intProfileCnt++;
                    //    Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile");
                    //}
                    //else
                    //{
                    //    Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": SKIPPPING");
                    //}

                    //continue;
                    //if(!blHasException)
                    //    continue;
                    //if (blHasException)
                    //{
                    //    MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProcException"];
                    //}
                    //if (blHasException)
                    //{
                    //    MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProcException"];
                    //}
                    //else if (blHasProcedural && blHasUtilization)
                    //{
                    //    MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProc"];
                    //}
                    //else if (blHasUtilization)
                    //{
                    //    MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtil"];
                    //}
                    //else if (blHasProcedural)
                    //{
                    //    MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateProc"];
                    //}






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

                    if (!blOverwriteExisting && blHasWord)
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


                        MSWord.wordReplace("{$P_FirstName}", UCaseFirstName);
                        MSWord.wordReplace("{$P_LastName}", UCaseLastName);

                        if (blIsMasked)
                            MSWord.wordReplace("{$MPIN}", "XXXXXXXXX");
                        else
                            MSWord.wordReplace("{$MPIN}", strMPIN);

                        MSWord.wordReplace("{$attr_clients}", attr_clients);

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

                    MSExcel.addValueToCell(strSheetname, "A4", FirstName + " " + LastName);

                    MSExcel.addValueToCell(strSheetname, "A5", strSpecialty);
                    MSExcel.addValueToCell(strSheetname, "A6", phyAddress);
                    MSExcel.addValueToCell(strSheetname, "A7", phyCity + ", " + phyState + " " + phyZip);

                   // MSExcel.addValueToCell(strSheetname, "B10", practiceName);

                    MSExcel.addValueToCell(strSheetname, "B9", attr_clients);
                    MSExcel.addValueToCell(strSheetname, "B10", opioid_clients);
                    MSExcel.addValueToCell(strSheetname, "B11", abx_clients);
                    MSExcel.addValueToCell(strSheetname, "B12", medAdhere_clients);



                    MSExcel.addValueToCell(strSheetname, "A13", strRCMO);


                    MSExcel.addValueToCell(strSheetname, "A14", strRCMOTitle);

                    MSExcel.addValueToCell(strSheetname, "A15", strRCMOTitle1);


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////ULTILIZATION TOP SECTION///////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasUtilizationSummary) //"|blHasProcedural"
                    {

                        strSheetname = "all_meas";
                        strBookmarkName = "utilization_section_table";

                        //strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_Ph32 as a where MPIN=" + strMPIN + " order by sort_id";

                        strSQL = "Select act_display, expected_display, var_display, signif, favorable FROM PBP_Profile_Ph13 where mpin=" + strMPIN + " order by sort_id";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_LastName>", UCaseLastName);


                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:G14", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);

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
                    if (blHasProceduralSummary)
                    {

                        ///MAKE DYNAMIC


                        strBookmarkName = "procedure_section_table";
                        strSheetname = "Pharmacy_meas";
                        //strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_px_Ph32 as a where measure_id not in(40,41,42) and spec_id=4 and MPIN=" + strMPIN + " order by sort_id";
                        strSQL = "Select Measure_desc, Unit_measure,act_display, expected_display,var_display,signif,signif_g as favorable FROM PBP_Profile_px_Ph13 where measure_id in(38,51,55) AND mpin=" + strMPIN + "order by sort_id";




                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dt, strSheetname, 3, 'A');

                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 6;//FIRST BLANK ROW
                            if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (3 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":G5", strSheetname);//LAST BORDERED ROW
                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":G" + (intEndingRowTmp - 1), strSheetname);

                            }


                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:G" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
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



                    if (blHasUtilizationDetails)
                    {
                        strBookmarkName = "utilization_drilldown_tables";

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=36 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=36 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "NAI_sum_det";
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
                        //strSQL = "select Category, Patient_Count, Visit_Count , Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Measure_desc,Unit_Measure,act_display,expected_display,var_display from dbo.PBP_Profile_Ph13 where measure_id in(14,15) and MPIN=" + strMPIN + " order by sort_Id";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Spec_PCP_sum_det2";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 6;//FIRST BLANK ROW
                            if (dt.Rows.Count < 2)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E5", strSheetname);//LAST BORDERED ROW
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

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=10 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Spec_PCP_sum_det";
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=9 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "OON_sum_det";
                            alSectionUtilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:C2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":C9", strSheetname);//LAST BORDERED ROW
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Mod_sum_det";
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=16 and attr_mpin=" + strMPIN + "  order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=16 and attr_mpin=" + strMPIN + " order by Catg_order";
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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";

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

                       // strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=2 and attr_mpin=" + strMPIN + "  order by Catg_order";

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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=1 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID = 1 and attr_mpin =" + strMPIN + " order by Catg_order";

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


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph32 where Measure_ID=1 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph13 where Measure_ID=50 and attr_mpin=" + strMPIN + " order by Catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Avg_cost_pt_det";
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



                        if (blHasWord)
                            MSWord.deleteBookmarkComplete(strBookmarkName);

                    }


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////////////////////PROCEDURE DRILLDOWN ////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    if (blHasProceduralDetails)
                    {
                        strBookmarkName = "procedure_drilldown_tables";

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        //DYNAMIC



                        //strSQL = "select act_display, expected_display, var_display from dbo.PBP_Profile_px_Ph32 as a where MPIN=" + strMPIN + " and measure_id between 40 and 42 order by measure_id";
                        strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph13 as a where measure_id in (56,57,58,59,60,61) and a.MPIN=" + strMPIN + "  order by Hierarchy_Id";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Pharmacy_Med_Adher_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", UCaseLastName);



                            intEndingRowTmp = 10;//FIRST BLANK ROW
                            if (dt.Rows.Count < 6)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E9", strSheetname);//LAST BORDERED ROW
                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                            }



                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select act_display, expected_display, var_display from dbo.PBP_Profile_px_Ph32 as a where MPIN=" + strMPIN + " and measure_id between 40 and 42 order by measure_id";

                        //DYNAMIC


                        strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph13 as a where measure_id in (52,53,54) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "Pharmacy_Abx_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", UCaseLastName);




                            intEndingRowTmp = 7;//FIRST BLANK ROW
                            if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E6", strSheetname);//LAST BORDERED ROW
                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                            }


                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }



                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "select act_display, expected_display, var_display from dbo.PBP_Profile_px_Ph32 as a where MPIN=" + strMPIN + " and measure_id between 40 and 42 order by measure_id";
                        strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph13 as a where measure_id in (40,41,42) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            strSheetname = "opioids_det";
                            alSectionProcedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A2:E2", strSheetname, "<P_LastName>", UCaseLastName);

                            intEndingRowTmp = 7;//FIRST BLANK ROW
                            if (dt.Rows.Count < 3)//TOTAL BORDERED ROWS
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);//FIRST BORDERED ROW
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E6", strSheetname);//LAST BORDERED ROW
                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                            }


                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }

                        if (blHasWord)
                            MSWord.deleteBookmarkComplete(strBookmarkName);

                    }



                    //UNCOMMENT ME!!!!!
                    if (blHasUtilizationSummary &&  blHasWord)
                    {
                        // MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                        //MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                        processBreaks(alSectionUtilization, 1);
                        processTopBreaks(alSectionUtilization, 1);

                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                        // 


                    }

                    if (blHasProceduralSummary &&  blHasWord)
                    {
                        //MSWord.addpageBreak("procedure_drilldown_pagebreak");

                        processBreaks(alSectionProcedural, 1);
                        processTopBreaks(alSectionProcedural, 1);

                    }

                    //if (blHasUtilization && blHasProcedural && blHasWord)
                    //{
                    //    var bookMark = "procedure_conditional_break";
                    //    var intLineNumber = MSWord.getLineNumber(bookMark);
                    //    if (intLineNumber >= 4)
                    //    {
                    //        MSWord.addLineBreak(bookMark);
                    //        MSWord.deleteBookmarkComplete(bookMark);
                    //    }
                        
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
                Console.WriteLine("MPIN:");
                Console.WriteLine(strMPIN);
                Console.WriteLine("ModelId:");
                Console.WriteLine(intModelId);
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
                        {
                            MSWord.addLineBreak(al[i].ToString());

                        }
                            
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

