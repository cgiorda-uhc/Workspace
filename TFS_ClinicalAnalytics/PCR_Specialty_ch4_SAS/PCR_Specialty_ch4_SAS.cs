using System;
using System.Data;
using System.Configuration;
using WCDocumentGenerator;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Data.OleDb;

namespace PCR_Specialty_ch4_SAS
{
    class PCR_Specialty_ch4_SAS
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


            //LINK STUFF
            //
            //
            //
            Start:

          //  foreach (Process Proc in Process.GetProcesses())
               // if (Proc.ProcessName.Equals("EXCEL") || Proc.ProcessName.Equals("WINWORD"))  //Process Excel?
                 //   Proc.Kill();


            int intProfileCnt = 1;
            int intTotalCnt = 0;


            string strMPIN = null;
            string strSQL = null;
            string strModelId = null;
            try
            {

                //foreach (Process Proc in Process.GetProcesses())
                //    if (Proc.ProcessName.Equals("EXCEL") || Proc.ProcessName.Equals("WINWORD"))  //Process Excel?
                //        Proc.Kill();

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


                bool blIsMasked = true;

                //START EXCEL APP

                MSExcel.populateExcelParameters(blVisibleExcel, blSaveExcel, strReportsPath, strExcelTemplate);
                MSExcel.openExcelApp();

                //Console.WriteLine("Starting Microsoft Word Instance...");
                //START WORD APP
                if (blHasWord)
                {
                    MSWord.populateWordParameters(blVisibleWord, blSaveWord, strReportsPath, strWordTemplate);
                    //MSWord.openWordApp();
                    MSWord.openWordApp();
                }


                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;
                string strBookmarkName = null;

                ArrayList alSectionUtilization = new ArrayList();
                ArrayList alSectionProcedural = new ArrayList();

                int intEndingRowTmp;

                bool blHasProcedural = false;
                bool blHasUtilization = false;


                //libname onc '/optum/uhs/01datafs/phi/onc/opchemo/rpt';
                string strSASPhase = "Ph34";
                string strSASUHNCycle = "UHN_May6";

                //options set = TRUNCATE_BIGINT = YES; -this is not a variable but I have to run it every time, since SAS do not see big integers if do not run
                //% let root =/ optum / uhs / 01datafs / phi / projects / analytics / pbp;
                //% let cycle = May6;/*date of UHN reporting refresh for Cohort 34*/
                //% let ph = Ph34;
                //libname & ph. "&root./&ph.";
                //% let uhn = UHN_ & cycle.;
                //% let uhn_addr = &uhn._addr;

                string strSampleCount = "50";
                string strMPINList = "select a.MPIN from " + strSASPhase + ".outliers (OBS=" + strSampleCount + ") as a inner join " + strSASPhase + "." + strSASUHNCycle + "_demog as b on a.MPIN=b.MPIN inner join " + strSASPhase + "." + strSASUHNCycle + "_pti_demog as p on p.mpin=PTIGroupID_upd";


                strMPINList = "2647, 15562, 1407426, 151492, 42680, 84096, 359, 424123, 41898, 25775, 440347, 15201, 107785, 11967, 4111, 81879, 13772, 91290, 1704, 132, 285530, 25440, 79256, 16577, 97281, 844269, 628017, 6352649, 15374, 2095002, 40827, 969, 126529, 185, 159981, 1509, 428, 1592, 10553, 14721, 10541, 6581, 2314, 42460, 882353, 11288, 3485, 829434, 33330, 50257, 718725, 34045, 49268, 8714, 1383595, 36030, 3190, 98361";


                strMPINList = "16835,1389430";

                //strMPINList = "2647, 15562, 1407426";


                //strMPINList = "10073";

                //strMPINList = "68252";

                //strMPINList = "60800";

                //strMPINList = "SELECT Distinct TOP " + strSampleCount + " a.MPIN FROM dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_Outl_ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 1 UNION SELECT Distinct TOP " + strSampleCount + " a.MPIN FROM dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_Outl_ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 2 UNION SELECT Distinct TOP " + strSampleCount + " a.MPIN FROM dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_Outl_ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 3 UNION SELECT Distinct TOP " + strSampleCount + " a.MPIN FROM dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_Outl_ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 4 UNION SELECT Distinct TOP " + strSampleCount + " a.MPIN FROM dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_Outl_ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 5";


                if (blIsMasked)
                {


                   // strSQL = "select Top 100 a.MPIN,a.attr_clients as clients,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, 'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as [State],'XXXXXXXXX' as zipcd,'XXXXXXXXX' as taxid, 'XXXXXXXXX' as practice_id,'XXXXXXXXX' as Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    strSQL = "select a.MPIN,a.attr_clients as clients,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, b.Spec_display as NDB_Specialty,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as State,'XXXXXXXXX' as zipcd,b.taxid, 'XXXXXXXXX'  as practice_id,'XXXXXXXXX' as Practice_Name,Tot_Util_meas,Tot_PX_meas, '' as RCMO,'' as RCMO_title,'' as RCMO_title1, model_id, h.Folder_Name from " + strSASPhase + ".outliers  as a inner join " + strSASPhase + ".outl_models as m on m.mpin=a.mpin inner join " + strSASPhase + "." + strSASUHNCycle + "_demog as b on a.MPIN=b.MPIN inner join " + strSASPhase + "." + strSASUHNCycle + "_pti_demog as p on p.mpin=PTIGroupID_upd inner join Ph34.spec_handling as h on h.mpin=a.mpin WHERE a.MPIN  in (" + strMPINList + ") ;"; // WHERE a.MPIN not in (" + strMPINList + ") and model_id = 1


                }
                else
                {

                    //strSQL = "select  a.MPIN,a.attr_clients as clients,P_LastName,P_FirstName,LastName,FirstName,ProvDegree, a.Spec_display as NDB_Specialty,b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1, NULL as Folder_Name,model_id from dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_Ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_Outl_ph33_models as m on m.mpin=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2  and a.MPIN in (" + strMPINList + ")";


                    //strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, b.Spec_display as NDB_Specialty,b.Street,b.City,b.State,b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1, '1' as model_id, '' as Folder_Name from " + strSASPhase + ".outliers as a inner join " + strSASPhase + "." + strSASUHNCycle + "_demog as b on a.MPIN=b.MPIN inner join " + strSASPhase + "." + strSASUHNCycle + "_pti_demog as p on p.mpin=PTIGroupID_upd inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude is null and r.phase_id=2 AND a.MPIN in (" + strMPINList + "); ";

                   // strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, b.Spec_display as NDB_Specialty,b.Street,b.City,b.State,b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, '' as RCMO,'' as RCMO_title,'' as RCMO_title1, model_id, '' as Folder_Name from " + strSASPhase + ".outliers as a inner join " + strSASPhase + ".outl_models as m on m.mpin=a.mpin inner join " + strSASPhase + "." + strSASUHNCycle + "_demog as b on a.MPIN=b.MPIN inner join " + strSASPhase + "." + strSASUHNCycle + "_pti_demog as p on p.mpin=PTIGroupID_upd WHERE a.MPIN in (" + strMPINList + ");";


                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, b.Spec_display as NDB_Specialty,b.Street,b.City,b.State,b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, '' as RCMO,'' as RCMO_title,'' as RCMO_title1, model_id, h.Folder_Name from " + strSASPhase + ".outliers  as a inner join " + strSASPhase + ".outl_models as m on m.mpin=a.mpin inner join " + strSASPhase + "." + strSASUHNCycle + "_demog as b on a.MPIN=b.MPIN inner join " + strSASPhase + "." + strSASUHNCycle + "_pti_demog as p on p.mpin=PTIGroupID_upd inner join Ph34.spec_handling as h on h.mpin=a.mpin WHERE a.MPIN in (" + strMPINList + ")  ;"; // WHERE a.MPIN not in (" + strMPINList + ") and model_id = 1

                }


                int intLineBreakCnt = 1;

                
                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(getLib());



                DataTable dtMain = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {

                    alSectionProcedural = new ArrayList();
                    alSectionUtilization = new ArrayList();

                    //if (intProfileCnt < 201 || intProfileCnt > 220)
                    //{
                    //    intProfileCnt++;
                    //    continue;
                    //}



                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;


                    //PROVIDER PLACEHOLDERS. THESE DB DATA COMES FROM MAIN LOOPING SQL ABOVE
                    string LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "NAME MISSING");
                    string FirstName = (dr["FirstName"] != DBNull.Value ?  dr["FirstName"].ToString().Trim() : "");
                    string UCaseLastName = (dr["P_LastName"] != DBNull.Value ? dr["P_LastName"].ToString().Trim() : "NAME MISSING");
                    string UCaseFirstName = (dr["P_FirstName"] != DBNull.Value ?  dr["P_FirstName"].ToString().Trim() : "");


                    if(!String.IsNullOrEmpty(FirstName))
                    {
                        FirstName = "Dr. " + FirstName;
                        UCaseFirstName = "Dr. " + UCaseFirstName;
                    }




                    string phyAddress = (dr["Street"] != DBNull.Value ? dr["Street"].ToString().Trim() : "ADDRESS MISSING");
                    string phyCity = (dr["City"] != DBNull.Value ? dr["City"].ToString().Trim() : "CITY MISSING");
                    string phyState = (dr["State"] != DBNull.Value ? dr["State"].ToString().Trim() : "STATE MISSING");
                    string phyZip = (dr["zipcd"] != DBNull.Value ? dr["zipcd"].ToString().Trim() : "ZIPCODE MISSING");



                    string strTIN = (dr["TaxID"] != DBNull.Value ? dr["TaxID"].ToString().Trim() : "");

                    string strProvDegree = (dr["ProvDegree"] != DBNull.Value ? dr["ProvDegree"].ToString().Trim() : "PROV DEGREE MISSING");
                    string strSpecialty = (dr["NDB_Specialty"] != DBNull.Value ? dr["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");

                    strMPIN = (dr["MPIN"] != DBNull.Value ? dr["MPIN"].ToString().Trim() : "");

                    string strSpecialtyProperCase =  strSpecialty;

                    //string strSpecialtyProperCase = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(strSpecialty.ToLower()).Replace(" And ", " and ");
                    //System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(s.ToLower())


                    string strRCMO = (dr["RCMO"] != DBNull.Value ? dr["RCMO"].ToString().Trim() : "RCMO MISSING");
                    string strRCMOTitle = (dr["RCMO_title"] != DBNull.Value ? dr["RCMO_title"].ToString().Trim() : "RCMO TITLE MISSING");
                    string strRCMOTitle1 = (dr["RCMO_title1"] != DBNull.Value ? dr["RCMO_title1"].ToString().Trim() : "RCMO TITLE 1 MISSING");


                    string attr_clients = (dr["clients"] != DBNull.Value ? dr["clients"].ToString().Trim() : "CLIENTS MISSING");

                    int proceudralCount = (dr["Tot_PX_meas"] != DBNull.Value ? int.Parse(dr["Tot_PX_meas"].ToString()) : 0);
                    int utilizationCount = (dr["Tot_Util_meas"] != DBNull.Value ? int.Parse(dr["Tot_Util_meas"].ToString()) : 0);
                    blHasProcedural = (proceudralCount > 0 ? true : false);
                    blHasUtilization = (utilizationCount > 0 ? true : false);


                    strModelId = (dr["model_id"] != DBNull.Value ? dr["model_id"].ToString() : null);
                    //intModelId = 1;

                    bool blHasProceduralSummary = false;
                    bool blHasUtilizationSummary = false;
                    bool blHasProceduralDetails = false;
                    bool blHasUtilizationDetails = false;


                    //POPULATE WITH INNA'S NEW DB COLUMNS
                    if (strModelId == "1")
                    {
                        blHasProceduralSummary = true;
                        blHasUtilizationSummary = true;
                        blHasProceduralDetails = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (strModelId == "2")
                    {
                        blHasProceduralSummary = true;
                        blHasUtilizationSummary = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (strModelId == "3")
                    {
                        blHasProceduralSummary = true;
                        blHasUtilizationSummary = true;
                        blHasProceduralDetails = true;
                    }
                    else if (strModelId == "4")
                    {
                        blHasUtilizationSummary = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (strModelId == "5")
                    {
                        blHasProceduralSummary = true;
                        blHasProceduralDetails = true;
                    }


                    if(blHasWord)
                    {
                        if (blHasProceduralSummary && blHasUtilizationSummary && blHasProceduralDetails && blHasUtilizationDetails)
                        {
                            MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndProc"]; //MODEL 1
                        }
                        else if (blHasProceduralSummary && blHasUtilizationSummary && blHasUtilizationDetails)
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
                    }
                    



                    string practiceName = (dr["Practice_Name"] != DBNull.Value ? dr["Practice_Name"].ToString().Trim() : "PRACTICE NAME MISSING");

                    //string ocl = (dr["orig_cl"] != DBNull.Value ? dr["orig_cl"].ToString().Trim() : "ZIPCODE MISSING");
                    // string cl_rem1 = (dr["attr_cl_rem1"] != DBNull.Value ? dr["attr_cl_rem1"].ToString().Trim() : "ZIPCODE MISSING");




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



                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value && dr["Folder_Name"] + "" != "" ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

                    string strFolderName = "";


                    //DELETE ME 2019!!!!!!
                    //strFolderNameTmp = "";
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

                    //strFinalReportFileName = strModelId + "_" + strSpecialty + "_" + strMPINLabel + "_" + LastName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_" + phyState + "_" + strMonthYear;
                    strFinalReportFileName =  strMPINLabel + "_" + LastName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_" + phyState + "_" + strMonthYear;

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


                       // if (intProfileCnt == 3)
                          //  strStartDate = "";


                        //OPEN WORD DOCUMENT
                        MSWord.openWordDocument();

                        //GENERAL PLACE HOLDERS. WE USE VARIABLES TO REPLACE PLACEHOLDERS WITHIN THE WORD DOC

                        MSWord.wordReplace("{$start_date}", strStartDate);
                        MSWord.wordReplace("{$end_date}", strEndDate);

                        MSWord.wordReplace("{$Physician Name}", UCaseFirstName + " " + UCaseLastName);


                        MSWord.wordReplace("{$Specialty}", strSpecialtyProperCase); //CASE SENSITIVITY (and)


                        if (blIsMasked)
                            MSWord.wordReplace("{$Physician MPIN}", "XXXXXXXXX");
                        else
                            MSWord.wordReplace("{$Physician MPIN}", strMPIN);

                        MSWord.wordReplace("{$attr clients}", attr_clients);


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

                    //strSheetname = "general info";
                    strSheetname = "general info, dates";

                    MSExcel.addValueToCell(strSheetname, "B2", strMPINLabel);
                    MSExcel.addValueToCell(strSheetname, "B3", strTIN);

                    MSExcel.addValueToCell(strSheetname, "A6", FirstName + " " + LastName);

                    MSExcel.addValueToCell(strSheetname, "A7", strSpecialty);
                    MSExcel.addValueToCell(strSheetname, "A8", phyAddress);
                    MSExcel.addValueToCell(strSheetname, "A9", phyCity + ", " + phyState + " " + phyZip);

                    MSExcel.addValueToCell(strSheetname, "C11", practiceName);

                    MSExcel.addValueToCell(strSheetname, "B13", attr_clients);

                    MSExcel.addValueToCell(strSheetname, "A15", strRCMO);


                    MSExcel.addValueToCell(strSheetname, "A16", strRCMOTitle);

                    MSExcel.addValueToCell(strSheetname, "A17", strRCMOTitle1);

                    MSExcel.addValueToCell(strSheetname, "A18", practiceName);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ///////////////////////////////////////ULTILIZATION TOP SECTION///////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasUtilizationSummary)
                    {

                        strSheetname = "all_meas_util";
                        strBookmarkName = "utilization_section_table";

                        //strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_Ph32 as a where MPIN eq " + strMPIN + " order by sort_id";
                        //strSQL = "select act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Profile_Ph33 as a where MPIN eq " + strMPIN + " order by sort_id";

                        strSQL = "select act_display, expected_display, var_display,signif,Favorable from Ph34.PBP_Profile as a where MPIN eq " + strMPIN + " order by sort_id;";

                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        strBookmarkName = "procedure_section_table";

                        switch (strSpecialty.ToUpper())
                        {

                            case "OBGYN":
                                strSheetname = "all_OB_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 4 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 4 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "CARDIOLOGY":
                                strSheetname = "all_Card_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 5 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 5 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "NEPHROLOGY":
                            case "RHEUMATOLOGY":
                                strSheetname = "all_Rheum_Nephr_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id in(12,9) and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id in(12,9) and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;"; 
                                //blProcedureOnlyBreak = false;
                                break;
                            case "NEUROLOGY":

                                strSheetname = "all_Neurol";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 10 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 10 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = false;
                                break;

                            case "OTOLARYNGOLOGY":
                                strSheetname = "all_ENT_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 14 and measure_id not in(40,41DBConnection.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);,42,44) and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 14 and measure_id not in(40,41,42,44) and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "GENERAL SURGERY":
                                strSheetname = "all_Gen_Surg_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 18 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 18 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;
                            case "GASTROENTEROLOGY":
                            case "UROLOGY":
                                strSheetname = "all_GI_Urol_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id in(13,15) and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id in(13,15) and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = false;
                                break;
                            case "NEUROSURGERY, ORTHOPEDICS AND SPINE":
                                strSheetname = "all_NOS_Proced";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 16 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 16 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;

                            case "PULMONARY":
                                strSheetname = "all_Pulm";
                                //strSQL = "select act_display, expected_display,var_display,signif,signif_g from dbo.PBP_Profile_px_Ph33 as a where spec_id eq 11 and measure_id not between 40 and 42 and MPIN eq " + strMPIN + " order by sort_id";
                                strSQL = "select act_display, expected_display,var_display,signif,signif_g from Ph34.PBP_Profile_px as a where spec_id eq 11 and measure_id eq 62 and MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;

                            //??????????????????????????????????????????????????????????????????????????????????
                            //??????????????????????????????????????????????????????????????????????????????????
                            //??????????????????????????????????????????????????????????????????????????????????
                            //??????????????????????????????????????????????????????????????????????????????????
                            //??????????????????????????????????????????????????????????????????????????????????
                            case "ALL":
                                strSheetname = "all_proc_meas";
                                strSQL = "select act_display, expected_display, var_display,signif,signif_g from Ph34.PBP_PROFILE_PX as a where MPIN eq " + strMPIN + " order by sort_id;";
                                //blProcedureOnlyBreak = true;
                                break;




                            default:
                                break;
                        }


                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_LastName>", UCaseLastName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:G" + (dt.Rows.Count + 2), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 35 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 36 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count , Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 17 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost,Catg_order from Ph34.PBP_act where Measure_ID eq 17 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 43 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 43 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 37 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 37 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 29 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 29 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 5 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 5 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 16 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 16 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 4 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 4 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 3 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 3 and attr_MPIN eq " + strMPIN + " order by Catg_order;";

                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 2 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 2 and attr_MPIN eq " + strMPIN + " order by Catg_order;"; 

                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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

                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph33 where Measure_ID eq 1 and attr_MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from Ph34.PBP_act where Measure_ID eq 1 and attr_MPIN eq " + strMPIN + " order by Catg_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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


                    if (blHasProceduralDetails)
                    {
                        strBookmarkName = "procedure_drilldown_tables";

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        //strSQL = "select act_display, expected_display, var_display FROM PBP_Profile_px_Ph33 Where measure_id in (40,41,42) and MPIN eq " + strMPIN + " order by sort_id";
                        strSQL = "Select act_display, expected_display, var_display From PH34.PBP_PROFILE_PX Where measure_id in (40,41,42) and MPIN eq " + strMPIN + " Order By sort_id;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 62 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 62 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Sleep_study_uncomplic_apnea_det";
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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 47 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 47 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 46 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 46 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 48 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 48 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 49 and MPIN eq " + strMPIN + " and meas_cnt > 0 order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 49 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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
                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 45 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 45 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 28 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 28 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 64 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 64 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 63 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 63 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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
                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 24 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 24 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 23 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 23 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 22 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 22 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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

                        //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 21 and MPIN eq " + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 21 and MPIN eq " + strMPIN + "  order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

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


                        //MAKE DYNAMIC (60800)
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from dbo.PBP_act_PX_ph33 where Measure_ID eq 18 and MPIN eq " + strMPIN + " order by Catg_order";
                    strSQL = "select Category, Meas_Cnt, Total_Cnt, Pct from PH34.PBP_ACT_PX where Measure_ID eq 18 and MPIN eq " + strMPIN + " order by categ_order;";
                        dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            strSheetname = "Csection";
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



                        if (blHasWord)
                            MSWord.deleteBookmarkComplete(strBookmarkName);

                    }

                    

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    //UNCOMMENT ME!!!!!
                    if (blHasUtilizationSummary && blHasWord)
                    {
                        // MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                        //MSWord.deleteBookmarkComplete("procedure_drilldown_pagebreak");
                        processBreaks(alSectionUtilization, 1);
                        processTopBreaks(alSectionUtilization, 1);

                        //MSWord.cleanBookmark("procedure_drilldown_pagebreak");
                        // 


                    }




                    if (blHasProceduralSummary && blHasWord)
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

                //Console.Beep();


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
