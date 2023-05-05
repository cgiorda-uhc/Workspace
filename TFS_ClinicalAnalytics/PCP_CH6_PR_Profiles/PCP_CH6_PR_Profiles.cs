using System;
using System.Data;
using System.Configuration;
using WCDocumentGenerator;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections;



namespace PCP_CH6_PR_Profiles
{
    class PCP_CH6_PR_Profiles
    {
        static bool blSasConnectedGLOBAL = false;
        static DataTable dtMainGLOBAL = null;
        static void Main(string[] args)
        {

            bool blThrewError = false;
            string strMPIN = null;
            string strSQL = null;
            double? intModelId = null;

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
                blThrewError = false;

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

                //PR 4 UPDATES
                //START END DATE FORMATE!!!!!!!!!!!!!!!!
                //START END DATE FORMATE!!!!!!!!!!!!!!!!
                //START END DATE FORMATE!!!!!!!!!!!!!!!!




                //1826956, 6219808





                // string strMPINList = "select a.MPIN from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2  ";

                string strMPINList = "select distinct a.MPIN from ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join ph16.outl_models6 as m on m.mpin=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2";


                //strMPINList = "3660151, 3660302, 3663030, 3663956, 3666117, 3667157";

                //strMPINList = "2009,8375,19507,32563,26498,38011,29080,2556,41631,41822,3660151, 3660302, 3663030, 3663956, 3666117";

                //strMPINList = "3660151, 3659129,3658573,3657869,	3657816,	3657295,	3584188,	3582480,	1165577,	1165676,	1166548,	1172326,	1168573,	1157745,	1153962,	1148219,	1145749,	1142346,	1136404,	1135775,	1117635,	1101285,	1088959,	1086740";

                //strMPINList = "1978059,1978619,	1979494,	1980597,	1981562,	1982385,	1984924,	1986007,	1991569,	1996942,	1997257,	2000100,	2004021,	2008791,	2022307,	2063671,	2076688,	2084907,	2743693,	2801137,	3006898";

                //strMPINList = "2200290, 2354844, 1103631, 348980, 2724663, 1745532 , 700877 , 1738437 , 696186 , 694905 , 787850 , 6579765 , 1826697 , 1825138 , 1824442 , 2493711 , 2486044 , 2485902 , 1317423 , 2482434 , 910864 , 1934913 , 7231802 , 902982 , 7225028 ";

                //strMPINList = "2108698,1781341,517846,2387500,3049215";

                //strMPINList = "2009";


                //strMPINList = "192222,192810,195653,197699,3467447, 3480366, 3494063, 3529088, 539914, 540658, 543531, 550543, 2319122, 2319854, 2319958, 2326214, 3315503, 3326076, 3326241, 3329619";


                //strMPINList = "1071 , 5435 , 9500 , 10753 , 11190 , 85498 , 88759 , 112129 , 228019 , 263307 , 10290 , 11831 , 17478 , 38011 , 43784 , 2556 , 8375 , 8662 , 10247 , 17697 , 12768 , 16835 , 30901 , 36325 , 38489";

                //strMPINList = "11739 , 14138 , 16312 , 17479 , 19739 , 410938 , 413346 , 428555 , 436905 , 610572 , 46968 , 49100 , 49388 , 50546 , 57022 , 22107 , 28371 , 29080 , 29088 , 31079 , 45313 , 56616 , 61074 , 69144 , 69145";

                //strMPINList = "11739, 610572 , 46968, 56616 , 61074";



               // strMPINList = "61600 , 62734 , 63268 , 70119 , 73499 , 1182628 , 1196488 , 1224659 , 1229298 , 1237146 , 393715 , 394564 , 395872 , 404115 , 408016 , 931872 , 931943 , 935954 , 943281 , 953647 , 1377180 , 1378720 , 1378884 , 1381321 , 1381479 ";

                //strMPINList = "315862 , 317856 , 319777 , 323791 , 329567 , 3388122 , 3405488 , 3407676 , 3416868 , 3482490 , 217549 , 219912 , 221281 , 236545 , 240246 , 217825 , 223874 , 224404 , 224670 , 228570 , 795617 , 819383 , 824133 , 852928 , 854494";


                //FOR TESTING ONLY!!!!!
                // string strCnt = "2";
                // strMPINList = "SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph14 as a inner join dbo.PBP_Outl_Ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 1) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph14 as a inner join dbo.PBP_Outl_Ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 2) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph14 as a inner join dbo.PBP_Outl_Ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 3) t) tmp ORDER BY NEWID() ) as t3 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph14 as a inner join dbo.PBP_Outl_Ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 4) t) tmp ORDER BY NEWID() ) as t4 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph14 as a inner join dbo.PBP_Outl_Ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 5) t) tmp ORDER BY NEWID() ) as t5";


                //strMPINList = "116668, 134679, 145602, 146862, 148479, 150672, 202626, 209985, 251632, 257989";

                //strMPINList = "SELECT MPIN FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 1) t) tmp ORDER BY NEWID() ) as t1 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 2) t) tmp ORDER BY NEWID() ) as t2 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 3) t) tmp ORDER BY NEWID() ) as t3 UNION SELECT * FROM(SELECT TOP " + strCnt + " MPIN FROM(SELECT distinct t.MPIN FROM(SELECT Distinct a.MPIN FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND model_id = 5) t) tmp ORDER BY NEWID() ) as t5";


                if (blIsMasked)
                {



                    //strSQL = "select Top 100 a.MPIN,a.attr_clients as clients,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, a.Spec_display as NDB_Specialty, 'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as [State],'XXXXXXXXX' as zipcd,'XXXXXXXXX' as taxid, 'XXXXXXXXX' as practice_id,'XXXXXXXXX' as Practice_Name,Tot_Util_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    //strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName,FirstName,P_LastName,P_FirstName,ProvDegree, a.NDB_Specialty, b.Street,b.City,b.[State],b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1, NULL as Folder_Name  from dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    //strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, 'XXXXXXXXX' as LastName, 'XXXXXXXXX' as FirstName, 'XXXXXXXXX' as P_LastName, 'XXXXXXXXX' as P_FirstName,ProvDegree, a.NDB_Specialty, 'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as [State], 'XXXXXXXXX' as  zipcd,'XXXXXXXXX' as taxid, 'XXXXXXXXX'  as practice_id, 'XXXXXXXXX' as Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1,Folder_Name, model_id from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_Outl_ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM inner join dbo.PBP_spec_handl_Ph14 as h on h.mpin=a.mpin WHERE a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    strSQL = "select a.MPIN,a.attr_clients as clients,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree,b.spec_display as NDB_Specialty, 'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XXXXXXXXX' as State,'XXXXXXXXX' as zipcd,'XXXXXXXXX' as taxid, 'XXXXXXXXX' as practice_id,'XXXXXXXXX' as Practice_Name,Tot_Util_meas,Tot_PX_meas, model_id,opi_clients, RCMO,RCMO_title,RCMO_title1,'' as Special_Handling,'' as Folder_Name, '' as abx_clients, '' as medadh_clients, -9999 as Tot_measures from ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join ph16.outl_models6 as m on m.mpin=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ") order by model_id";

                }
                else
                {


                    // strSQL = "SELECT Distinct a.MPIN, a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName, FirstName, P_LastName, P_FirstName, ProvDegree, a.NDB_Specialty, b.Street, b.City, b.[State], b.zipcd, b.taxid, p.MPIN as practice_id, p.Practice_Name, Tot_measures, Tot_PX_meas, RCMO, RCMO_title, RCMO_title1, NULL as Folder_Name, model_id FROM dbo.PBP_Outl_Ph13 as a inner join dbo.PBP_Outl_Ph13_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_Ph13 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_Ph13 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    // strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName, FirstName,P_LastName,P_FirstName,ProvDegree, Spec_display as NDB_Specialty,   b.Street,b.City,b.[State], b.zipcd,b.taxid, p.MPIN as practice_id, p.Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1, Folder_Name as Folder_Name, model_id from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_Outl_ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";


                    // strSQL = "select distinct a.MPIN,a.attr_clients as clients, op_clients, abx_clients, medadh_clients, LastName, FirstName,P_LastName,P_FirstName,ProvDegree, a.NDB_Specialty, b.Street,b.City,b.[State], b.zipcd,b.taxid, p.MPIN as practice_id, p.Practice_Name,Tot_measures,Tot_PX_meas,RCMO,RCMO_title,RCMO_title1,Folder_Name, model_id from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_Outl_ph14_models as m on m.mpin=a.mpin inner join dbo.PBP_outl_demogr_ph14 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph14 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM inner join dbo.PBP_spec_handl_Ph14 as h on h.mpin=a.mpin WHERE a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,P_LastName,FirstName,P_FirstName,ProvDegree,'' as NDB_Specialty, b.Street,b.City,b.State,b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, model_id,opi_clients, RCMO,RCMO_title,RCMO_title1,'' as Special_Handling,'' as Folder_Name, '' as abx_clients, '' as medadh_clients, -9999 as Tot_measures from ph16.outliers6 as a inner join ph16.UHN_FEB2_DEMOG as b on a.MPIN=b.MPIN inner join ph16.UHN_FEB2_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join ph16.outl_models6 as m on m.mpin=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ") order by model_id";
                }



                int intLineBreakCnt = 1;


                if (!blSasConnectedGLOBAL)
                {
                    Console.WriteLine("Connecting to SAS Server...");
                    IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());
                    blSasConnectedGLOBAL = true;
                }


                if (dtMainGLOBAL == null)
                {
                    Console.WriteLine("Gathering targeted physicians...");
                    dtMainGLOBAL = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                }
                    

                
                intTotalCnt = dtMainGLOBAL.Rows.Count;

                foreach (DataRow dr in dtMainGLOBAL.Rows)//MAIN LOOP START
                {

                    alSectionProcedural = new ArrayList();
                    alSectionUtilization = new ArrayList();

                    //throw new Exception();

                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;


                    //PROVIDER PLACEHOLDERS. THESE DB DATA COMES FROM MAIN LOOPING SQL ABOVE
                    string LastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "");
                    string FirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "");
                    string UCaseLastName = (dr["P_LastName"] != DBNull.Value ? dr["P_LastName"].ToString().Trim() : "");
                    string UCaseFirstName = (dr["P_FirstName"] != DBNull.Value ? dr["P_FirstName"].ToString().Trim() : "");



                    //if (!String.IsNullOrEmpty(FirstName))
                    //{
                    //    FirstName = "Dr. " + FirstName;
                    //    UCaseFirstName = "Dr. " + UCaseFirstName;
                    //}



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
                    string opioid_clients = (dr["opi_clients"] != DBNull.Value ? dr["opi_clients"].ToString().Trim() : "OPIOID CLIENTS MISSING");
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


                    intModelId = (dr["model_id"] != DBNull.Value ? (double?)dr["model_id"] : null);

                    bool blHasPharmSummary = false;
                    bool blHasUtilizationSummary = false;
                    bool blHasPharmDetails = false;
                    bool blHasUtilizationDetails = false;


                    //POPULATE WITH INNA'S NEW DB COLUMNS
                    if (intModelId == 1)
                    {
                        blHasPharmSummary = true;
                        blHasUtilizationSummary = true;
                        blHasPharmDetails = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (intModelId == 2)
                    {
                        blHasPharmSummary = true;
                        blHasUtilizationSummary = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (intModelId == 3)
                    {
                        blHasPharmSummary = true;
                        blHasUtilizationSummary = true;
                        blHasPharmDetails = true;
                    }
                    else if (intModelId == 4)
                    {
                        blHasUtilizationSummary = true;
                        blHasUtilizationDetails = true;
                    }
                    else if (intModelId == 5)
                    {
                        blHasPharmSummary = true;
                        blHasPharmDetails = true;
                    }


                    if (blHasPharmSummary && blHasUtilizationSummary && blHasPharmDetails && blHasUtilizationDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndPharm"]; //MODEL 1
                    }
                    else if (blHasPharmSummary && blHasUtilizationSummary && blHasUtilizationDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndPharmUtlDetails"];//MODEL 2

                    }
                    else if (blHasPharmSummary && blHasUtilizationSummary && blHasPharmDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtilAndPharmPharmDetails"]; //MODEL 3
                    }
                    else if (blHasUtilizationSummary && blHasUtilizationDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplateUtil"]; //MODEL 4
                    }
                    else if (blHasPharmSummary && blHasPharmDetails)
                    {
                        MSWord.strWordTemplate = ConfigurationManager.AppSettings["WordTemplatePharm"]; //MODEL 5
                    }
                    else
                    {
                        Console.WriteLine("NO TEMPLATE MATCH FOR " + strMPIN);
                        Console.Beep();
                        Console.ReadLine();
                    }



                    if (blIsMasked)
                    {
                        strMPINLabel = "123456" + intProfileCnt;
                    }
                    else
                    {
                        strMPINLabel = strMPIN;
                    }


                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

                    string strFolderName = "";


                    //DELETE ME 2020!!!!!!
                    strFolderNameTmp = "";
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

                        MSWord.wordReplace("{$StartDate}", strStartDate);
                        MSWord.wordReplace("{$EndDate}", strEndDate);
                        MSWord.wordReplace("{$Specialty}", strSpecialty);
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


                        //strSQL = "Select act_display, expected_display, var_display, signif, favorable FROM PBP_Profile_Ph14 where mpin=" + strMPIN + " order by sort_id";
                        strSQL = "Select act_display, expected_display, var_display, signif, favorable FROM ph16.Profile where measure_id in(1,2,3,5,37,50) and mpin=" + strMPIN + " order by sort_id";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                        // dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {
                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_FirstName>", UCaseFirstName);
                            MSExcel.ReplaceInTableTitle("A1:G1", strSheetname, "<P_LastName>", UCaseLastName);


                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:G8", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: false);

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
                    if (blHasPharmSummary)
                    {

                        ///MAKE DYNAMIC


                        strBookmarkName = "pharmacy_section_table";
                        strSheetname = "Pharmacy_meas";


                        //strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display,var_display,signif,signif_g as favorable FROM PBP_Profile_px_Ph14 where measure_id in(38,51,55) and mpin=" + strMPIN + " order by sort_id";
                        strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display,var_display,signif,signif_g as favorable FROM ph16.profile_px where measure_id in(38,51,55) and mpin=" + strMPIN + "  order by sort_id";



                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":G" + (intEndingRowTmp - 1), strSheetname);

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
                        //strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from ph16.PBP_act where Measure_ID=50 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from ph16.PBP_act where Measure_ID=37 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph14 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from ph16.PBP_act where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

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
                        //strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph14 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from ph16.PBP_act where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);
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


                        //strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph14 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select category,Patient_Count,Visit_Count,Pct_Cost from ph16.PBP_act where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order;";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

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

                        //strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph14 where Measure_ID=1 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category,Patient_Count,Visit_Count,Pct_Cost from ph16.PBP_act where Measure_ID=1 and attr_mpin=" + strMPIN + "  order by Catg_order;";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

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

                        //strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph14 where Measure_ID=50 and attr_mpin=" + strMPIN + " order by Catg_order";
                        strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from ph16.PBP_act where Measure_ID=50 and attr_mpin=" + strMPIN + " order by Catg_order";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":D" + (intEndingRowTmp - 1), strSheetname);

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
                    //////////////////////////////////////////////////PHARMACY DRILLDOWN ////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    if (blHasPharmDetails)
                    {
                        strBookmarkName = "pharmacy_drilldown_tables";

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        //DYNAMIC


                        //strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph14 as a where measure_id in (56,57,58,59,60,61) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        strSQL = "Select Measure_desc, Unit_measure,act_display, expected_display, var_display from ph16.Profile_Px as a where measure_id in (56,57,58,59,60,61) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

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


                        //strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph14 as a where measure_id in (52,53,54) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        strSQL = "Select Measure_desc, Unit_measure,act_display, expected_display, var_display from ph16.Profile_Px as a where measure_id in (52,53,54) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                            }


                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }



                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        //strSQL = "Select Measure_desc, Unit_measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_Ph14 as a where measure_id in (40,41,42) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        strSQL = "Select Measure_desc, Unit_measure,act_display, expected_display, var_display from ph16.Profile_Px as a where measure_id in (40,41,42) and a.MPIN=" + strMPIN + " order by Hierarchy_Id";
                        dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
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
                                //MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

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
                    if (blHasUtilizationSummary && blHasWord)
                    {
                        processBreaks(alSectionUtilization, 1);
                        processTopBreaks(alSectionUtilization, 1);


                    }

                    if (blHasPharmSummary && blHasWord)
                    {
                        processBreaks(alSectionProcedural, 1);
                        processTopBreaks(alSectionProcedural, 1);

                    }

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

                //Console.Beep();

                blThrewError = true;
                //Console.ReadLine();


            }
            finally
            {


                try
                {
                    if(!blThrewError)
                    {
                        DBConnection32.getOleDbDataTableGlobalClose();
                        IR_SAS_Connect.destroy_SAS_instance();
                    }



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

                //TRY AGAIN!
                if (blThrewError)
                    Main(null);


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

