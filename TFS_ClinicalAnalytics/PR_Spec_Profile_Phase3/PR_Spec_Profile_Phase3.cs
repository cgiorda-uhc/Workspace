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

namespace PR_Spec_Profile_Phase3
{
    class PR_Spec_Profile_Phase3
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


                DataTable dtActionableItems = null;
                DataTable dt = null;
                Hashtable htParam = new Hashtable();
                string strSheetname = null;
                string strBookmarkName = null;

                ArrayList alSection1Procedural = new ArrayList();
                ArrayList alSection2Procedural= new ArrayList();
                ArrayList alSection3Procedural = new ArrayList();
                ArrayList alSection1Utilization = new ArrayList();
                ArrayList alSection2Utilization = new ArrayList();
                ArrayList alSection3Utilization = new ArrayList();

                int intProfileCnt = 1;
                int intTotalCnt;


                int intProcRowTotal = 0;



                int intEndingRowTmp;

                bool blHasProcedural = false;
                bool blHasUtilization = false;

                string strMPINList = "2671122,1122204,54399,1052393,2825664,2118189,3536528,2437298,1953413,600434, 809394, 827945, 1241958, 1441156, 1460693, 1465290, 1755481, 1969717, 2013264, 2033714, 2117914, 2270024, 2543962, 2952126, 3664681";

                strMPINList = "1445329,3044456,2797659,3105604,2344079,3179251,1729008";

                strMPINList = "2809952,1445329,2086247"; //U ONLY
                strMPINList = "2344079,323466,5530443"; //P ONLY
                strMPINList = "3044456,3174279,2664069"; //P AND U ONLY

                strMPINList = "3044456,3174279,2664069,2809952,1445329,2086247,2344079,323466,5530443,243158,2344079";

                //strMPINList = "2344079";


                //strMPINList = "243158,2344079";

               // strMPINList = "2344079";

                 strMPINList = "3044456,3174279,2664069,2809952,1445329,2086247,2344079,323466,5530443,243158,2344079,2344079, 32103, 61036, 99329, 109704, 170869, 178845, 291658, 319211, 339454, 388413, 230047, 231110, 242514, 242949, 243158, 243578, 1258788, 1298084, 1385661, 1427997, 1039560, 1045578, 1052257, 1070882, 1076349, 1096322, 32158, 32465, 33053, 33629, 34062, 34238, 42446, 57794";



                strMPINList = "2088848,243158,2344079,388413, 621057, 2024193, 3480248, 3120726, 1188912, 3179507,3172319,3159266,3174868,3125293,3120726,3107611,3100763,3089591,3086479,3085162";


               


                                    strMPINList="435,6595,6640,806907,25139,45538,47969,50190,63941,70053,74281,75829,76857,77567,77956,79016,81142,88823,92941,95880,96094,96706,104055,108009,110759,113066,123033,127728,130975,136990,139062,139245,148731,149434,159151,162530,168642,168767,170239,170669,175354,181486,182085,182509,183786,198948,199938,234324,207943,207945,211856,215102,215401,221576,222853,231110,241222,242514,243158,243578,247694,246135,257885,259510,265608,273596,278266,278336,278815,284038,295034,295069,307011,313601,314371,321616,324792,328116,338089,354911,357859,359934,367649,388413,388993,391528,391927,393003,394638,404560,410150,417406,426254,426853,432114,433471,436314,441047,443033,454446,458035,461571,476975,477236,480875,492751,497229,501590,504360,508018,512154,519905,558681,546258,546471,551562,557054,559702,574314,577794,578781,583837,586304,588198,604208,621057,621509,649873,654428,658113,673754,682236,682674,686567,693122,705556,709347,726239,728529,729981,740441,751047,756319,756347,758099,759396,759980,763466,763775,764543";

                //strMPINList = "388413";

                //strMPINList = "3100763";

                //strMPINList = "2088848";


                //strMPINList = "2088848,243158,2344079";


                //strMPINList = "2088848";


                //strMPINList = "243158";

                //strMPINList = "3174279";


                //strMPINList = "2344079";

                //strMPINList = "323466,2344079";

                // strMPINList= "1586447, 1610548, 433248, 509601, 786793, 66835, 74668, 76452, 100666";
                strMPINList = "3179507,170669,278366,182085,168642,215401";



                strMPINList = "SELECT Distinct MPIN FROM ( select TOP 200 a.MPIN from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and Tot_utl_meas is not null and Tot_PX_meas is not null UNION select TOP 200 a.MPIN from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and Tot_utl_meas is not null and Tot_PX_meas is null UNION select TOP 200 a.MPIN from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and Tot_utl_meas is null and Tot_PX_meas is not null ) t";



                strMPINList = "1165148, 1151195";

                strMPINList = "2088848,243158,2344079,388413, 621057, 2024193, 3480248, 3120726, 1188912, 3179507,3172319,3159266,3174868,3125293,3120726,3107611,3100763,3089591,3086479,3085162";


                strMPINList = "1165148, 1151195, 1993157, 1884906";


                strMPINList = "3179507, 170669, 278366, 182085, 168642, 215401, 1157601, 2346163";


                strMPINList = "608071, 3337296, 278336, 2521709,2529493,2564126,2575707";

                //PEI LETTER MOVE RUN 

                if (blIsMasked)
                {
                    // strSQL = "select a.MPIN,b.attr_clients as orig_cl,b.attr_cl_rem1,'XXXXXXXXX' as LastName,'XXXXXXXXX' as FirstName,'XXXXXXXXX' as P_LastName,'XXXXXXXXX' as P_FirstName,ProvDegree, Spec_display as NDB_Specialty,'XXXXXXXXX' as Street,'XXXXXXXXX' as City,'XX' as State,'XXXXXXXXX' as zipcd, a.UHN_TIN,'XXXXXXXXX' as  PracticeName, r.RCMO,r.RCMO_title,r.RCMO_title1,spec_handl_gr_id,Special_Handling,Folder_Name from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_TIN_addr_Ph1 as ad on ad.TaxID=a.UHN_TIN inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN inner join dbo.PBP_spec_handl_ph1 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where b.Exclude in(0,4) and attr_cl_rem1>=20 and r.phase_id=1 and mailing_id=2and a.MPIN in (" + strMPINList + ")";


                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, Spec_display as NDB_Specialty,b.Street,b.City,b.[State],b.zipcd, b.TaxID,ad.Name as PracticeName,Tot_utl_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";
                }
                else
                {



                    //strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, Spec_display as NDB_Specialty,b.Street,b.City,b.[State],b.zipcd, b.TaxID,ad.Name as PracticeName,Tot_utl_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name, NULL As Folder_Name2 from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN in (" + strMPINList + ")";

                    strSQL = "select a.MPIN,a.attr_clients as clients,LastName,FirstName,P_LastName,P_FirstName,ProvDegree, Spec_display as NDB_Specialty,b.Street,b.City,b.[State],b.zipcd, b.TaxID,ad.Name as PracticeName,Tot_utl_meas,Tot_PX_meas, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name, NULL As Folder_Name2 from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 ";

                    //FOR QA!!!!
                    //strSQL = "SELECT MPIN, clients, LastName, FirstName, P_LastName, P_FirstName, ProvDegree, NDB_Specialty, Street, City, [State], zipcd, TaxID, PracticeName, Tot_utl_meas, Tot_PX_meas, RCMO, RCMO_title, RCMO_title1, Special_Handling, CASE WHEN Row % 3 = 0 THEN 'Cook_Amie' WHEN Row % 2 = 0 THEN 'Koepke_Kristine' WHEN Row % 1 = 0 THEN 'Dimartino_MaryAnn' ELSE 'NONE' END as Folder_Name,   Folder_Name as Folder_Name2 FROM ( SELECT ROW_NUMBER() OVER(ORDER BY a.MPIN DESC) AS Row, a.MPIN, a.attr_clients as clients, LastName, FirstName, P_LastName, P_FirstName, ProvDegree, Spec_display as NDB_Specialty, b.Street, b.City, b.[State], b.zipcd, b.TaxID, ad.Name as PracticeName, Tot_utl_meas, Tot_PX_meas, RCMO, RCMO_title, RCMO_title1, Special_Handling, Folder_Name FROM dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND a.MPIN IN (" + strMPINList + " ))tmp";


                }


                int intActionRowCnt = 0;
                int intLineBreakCnt = 1;

                Int16 intInnerCounter = 0;


                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                Console.WriteLine("Gathering targeted physicians...");
                intTotalCnt = dtMain.Rows.Count;
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {



                    alSection1Procedural = new ArrayList();
                    alSection2Procedural = new ArrayList();
                    alSection3Procedural = new ArrayList();
                    alSection1Utilization = new ArrayList();
                    alSection2Utilization = new ArrayList();
                    alSection3Utilization = new ArrayList();

                    //if (int.Parse(dr["MPIN"].ToString()) < 215108)
                    //{
                    //    continue;
                    //}



                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;


                    //PROVIDER PLACEHOLDERS. THESE DB DATA COMES FROM MAIN LOOPING SQL ABOVE
                    string LastName = (dr["P_LastName"] != DBNull.Value ? dr["P_LastName"].ToString().Trim() : "NAME MISSING");
                    string FirstName = (dr["P_FirstName"] != DBNull.Value ? dr["P_FirstName"].ToString().Trim() : "NAME MISSING");
                    string phyName = (dr["P_LastName"] != DBNull.Value ? (dr["P_FirstName"].ToString().Trim() + " " + dr["P_LastName"].ToString().Trim()) : "NAME MISSING");
                    string UCaseLastName = (dr["LastName"] != DBNull.Value ? dr["LastName"].ToString().Trim() : "NAME MISSING");
                    string UCaseFirstName = (dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString().Trim() : "NAME MISSING");

                    string LCphyName = (dr["LastName"] != DBNull.Value ? (dr["FirstName"].ToString().Trim() + " " + dr["LastName"].ToString().Trim()) : "NAME MISSING");


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
                    int utilizationCount = (dr["Tot_utl_meas"] != DBNull.Value ? int.Parse(dr["Tot_utl_meas"].ToString()) : 0);
                    blHasProcedural = (dr["Tot_PX_meas"] != DBNull.Value ? true : false);
                    blHasUtilization = (dr["Tot_utl_meas"] != DBNull.Value ? true : false);



                    string practiceName = (dr["PracticeName"] != DBNull.Value ? dr["PracticeName"].ToString().Trim() : "PRACTICE NAME MISSING");

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


                    

                    
                    



                    //string strPracticeName = (dr["PracticeName"] != DBNull.Value ? dr["PracticeName"].ToString().Trim() : "NAME MISSING");


                    string strRCMOFirst = null;
                    string strRCMOLast = null;



                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");


                    string strFolderNameTmp2 = (dr["Folder_Name2"] != DBNull.Value ? dr["Folder_Name2"].ToString().Trim() + "\\" : "");


                    string strFolderName = "";



                    //if (dr["spec_handl_gr_id"].ToString().Equals("0"))
                    //{
                    //    strFolderName = dr["Special_Handling"].ToString() + "\\";
                    //}
                    //else
                    //{
                    //    strFolderName = dr["Special_Handling"] + "\\" + dr["UHN_TIN"] + "\\";
                    //}



                   //NOT QA UNCOMMENT
                    if (!String.IsNullOrEmpty(strFolderNameTmp))
                    {
                        strFolderNameTmp = "SpecialHandling\\" + strFolderNameTmp + strTIN + "\\";
                    }
                    else
                    {
                        strFolderNameTmp = "RegularMailing\\" + strFolderNameTmp;
                    }


                    ////FOR QA!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //if (!String.IsNullOrEmpty(strFolderNameTmp2))
                    //{
                    //    strFolderNameTmp = strFolderNameTmp + "SpecialHandling\\" + strTIN + "\\";
                    //}
                    //else
                    //{
                    //    strFolderNameTmp = strFolderNameTmp + "RegularMailing\\";
                    //}





                    //strFolderName = strFolderNameTmp  + strTIN+ "\\";
                    strFolderName = strFolderNameTmp;



                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));

                    if (blHasWord)
                        MSWord.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


                    if (LastName.Contains("-"))
                    {
                        string s = "";
                    }



                    strFinalReportFileName = strMPINLabel + "_" + LastName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_PR_" + phyState + "_" + strMonthYear;



                    //IF THE CURRENT PROFILE ALREADY EXISTS WE DO OR DONT WANT TO OVERWRITE PROFILE (SEE APP.CONFIG)...
                    //if (!blOverwriteExisting)
                    //{
                    //    //...CHECK IF PROFILE EXISTS...
                    //    if (File.Exists(MSWord.strReportsPath + "word\\" + strFinalReportFileName + ".doc"))
                    //    {
                    //        Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                    //        intProfileCnt++;
                    //        //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                    //        continue;
                    //    }
                    //}


                    //if (!blOverwriteExisting)
                    //{
                    //    //...CHECK IF PROFILE EXISTS...
                    //    if (File.Exists(MSWord.strReportsPath.Replace("{$profileType}", "Final") + strFinalReportFileName + ".pdf"))
                    //    {
                    //        Console.WriteLine(intProfileCnt + " of " + intTotalCnt + ": Profile '" + strFinalReportFileName + "' already exisits, this will be skipped");
                    //        intProfileCnt++;
                    //        //...IF PROFILE EXISTS MOVE TO NEXT MPIN
                    //        continue;
                    //    }
                    //}



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

                        MSWord.wordReplace("<<Date>>", strDisplayDate);


                        //MSWord.wordReplace("<Physician Name>", FirstName + " " + LastName);
                        MSWord.wordReplace("<<Physician Name>>", UCaseFirstName + " " + UCaseLastName);


                        MSWord.wordReplace("<<UCFirstName>>", UCaseFirstName);
                        MSWord.wordReplace("<<UCLastName>>", UCaseLastName);



                        MSWord.wordReplace("<<FirstName>>", FirstName);
                        MSWord.wordReplace("<<LastName>>", LastName);

                        MSWord.wordReplace("<<PracticeName>>", practiceName);

                        MSWord.wordReplace("<<Physician Name LC>>", FirstName + " " + LastName);



                        MSWord.wordReplace("<<ProvDegree>>", strProvDegree);
                        MSWord.wordReplace("<<Specialty>>", strSpecialty);
                        // MSWord.wordReplace("<Specialty>", strSpecialtyLongDesc);
                        MSWord.wordReplace("<<Address 1>>", phyAddress);
                        MSWord.wordReplace("<<City>>", phyCity);
                        MSWord.wordReplace("<<State>>", phyState);
                        MSWord.wordReplace("<<ZIP Code>>", phyZip);


                        MSWord.wordReplace("<<RCMO>>", strRCMO);
                        MSWord.wordReplace("<<RCMO title>>", strRCMOTitle);




                        MSWord.wordReplace("<<Provider Name>>", UCaseFirstName + " " + UCaseLastName);
                       MSWord.wordReplace("<<Provider MPIN>>", strMPIN);
                         MSWord.wordReplace("<<Group TINName>>", strTIN);

                        MSWord.wordReplace("<attributed patient count>", attr_clients);

                        //MSWord.wordReplace("<RCMO_title1>", strRCMOTitle1);

                        // MSWord.wordReplace("<attr_clients>", strAttrClients);



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

                    strSheetname = "general info";


                    MSExcel.addValueToCell(strSheetname, "B2", strMPINLabel);
                    MSExcel.addValueToCell(strSheetname, "B3", strTIN);


                    MSExcel.addValueToCell(strSheetname, "A5", LCphyName);

                    MSExcel.addValueToCell(strSheetname, "A6", strSpecialty);
                    MSExcel.addValueToCell(strSheetname, "A7", phyAddress);
                    MSExcel.addValueToCell(strSheetname, "A8", phyCity + ", " + phyState + " " + phyZip);



                    MSExcel.addValueToCell(strSheetname, "B12", attr_clients);
                    //MSExcel.addValueToCell(strSheetname, "B9", cl_rem1);

                    MSExcel.addValueToCell(strSheetname, "B17", practiceName);

                    //MSExcel.addValueToCell(strSheetname, "A27", "Dear " + phyName);
                    //MSExcel.addValueToCell(strSheetname, "B13", strPracticeName);


                    MSExcel.addValueToCell(strSheetname, "A14", strRCMO);


                    MSExcel.addValueToCell(strSheetname, "A15", strRCMOTitle);

                    MSExcel.addValueToCell(strSheetname, "A16", strRCMOTitle1);




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    if (blHasUtilization)
                    {

                        MSWord.deleteBookmarkComplete("utilization1_start_section");
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        strSheetname = "Top_3_meas";
                        strBookmarkName = "utilization1_start";
                        


                        strSQL = "select Measure_desc, Unit_Measure, act_display, expected_display, var_display from dbo.PBP_Profile_Ph3 as a where for_page1=1 and MPIN=" + strMPIN + " order by Hierarchy_Id";


                        dtActionableItems = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dtActionableItems.Rows.Count > 0)
                        {

                            alSection1Utilization.Add(strSheetname);

                            MSExcel.populateTable(dtActionableItems, strSheetname, 3, 'A');

                            MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<P_LastName>", LastName);


                            intEndingRowTmp = 6;
                            if (dtActionableItems.Rows.Count < 3)
                            {
                                intEndingRowTmp = (3 + dtActionableItems.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E5", strSheetname);
                                MSExcel.addBorders("A" + (intEndingRowTmp - 1) + ":E" + (intEndingRowTmp - 1), strSheetname);

                            }


                            if (blHasWord)
                            {
                                if (dtActionableItems.Rows.Count > 0)
                                {
                                    MSWord.tryCount = 0;
                                    MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);


                                }

                                MSWord.deleteBookmarkComplete(strBookmarkName);

                            }
                        }
                        else
                        {
                            MSWord.cleanBookmark(strBookmarkName);
                        }

                           
                    }
                    else
                    {
                        
                        MSWord.cleanBookmark("utilization1_start_section");
                        MSWord.deleteBookmarkComplete("utilization1_start_section");
                    }



                    //if (blHasProcedural && blHasUtilization && proceudralCount > 2)//BOTH SECTIONS SO BREAKEM
                    //    MSWord.addpageBreak("p_u_pagebreak1");
                    //else
                    //    MSWord.deleteBookmarkComplete("p_u_pagebreak1");

                    //MSWord.deleteBookmarkComplete("p_u_pagebreak1");



                    if (blHasProcedural)
                    {

                        MSWord.deleteBookmarkComplete("procedural1_start_section");

                        intProcRowTotal = 0;
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strBookmarkName = "procedural1_start";
                        strSheetname = "NOS_Proced_outl2";

                        

                        strSQL = "select Measure_desc,Unit_Measure, act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where for_page1=1 and measure_id=28 and NDB_Specialty ='NOS' and a.MPIN=" + strMPIN + " order by Hierarchy_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            intProcRowTotal = dt.Rows.Count;
                            alSection1Procedural.Add(strSheetname);
                            MSExcel.populateTable(dt, strSheetname, 3, 'A');


                            MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<P_LastName>", LastName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:E3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "NOS_Proced_outl1";

                        strSQL = "select Measure_desc,Unit_Measure, act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where for_page1=1 and measure_id between 24 and 27 and NDB_Specialty ='NOS' and a.MPIN=" + strMPIN + " order by Hierarchy_Id";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        if (dt.Rows.Count > 0)
                        {

                            intProcRowTotal += dt.Rows.Count;

                            alSection1Procedural.Add(strSheetname);
                            MSExcel.populateTable(dt, strSheetname, 3, 'A');

                            MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 7;
                            if (dt.Rows.Count < 4)
                            {
                                intEndingRowTmp = (3 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E6", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "ENT_GS_GI_Urol_Opht_Proced_outl";
                        

                        strSQL = "select Measure_desc,Unit_Measure, act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where for_page1=1 and measure_id between 24 and 27 and NDB_Specialty not in('CARDIOLOGY','NOS') and a.MPIN=" + strMPIN + "  order by Hierarchy_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            intProcRowTotal += dt.Rows.Count;

                            alSection1Procedural.Add(strSheetname);
                            MSExcel.populateTable(dt, strSheetname, 3, 'A');

                            MSExcel.ReplaceInTableTitle("A1:E1", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 7;
                            if (dt.Rows.Count < 4)
                            {
                                intEndingRowTmp = (3 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E6", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "Card_Proced_outl";
                        

                        strSQL = "select Measure_desc,Unit_Measure, act_display, expected_display, var_display from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where for_page1=1 and measure_id between 21 and 27 and NDB_Specialty like 'CARD%' and a.MPIN=" + strMPIN;


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            intProcRowTotal += dt.Rows.Count;

                            alSection1Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'A');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<PracticeName>", practiceName);


                            intEndingRowTmp = 10;
                            if (dt.Rows.Count < 7)
                            {
                                intEndingRowTmp = (3 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":E9", strSheetname);
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

                        MSWord.deleteBookmarkComplete(strBookmarkName);

                    }
                    else
                    {
                        MSWord.cleanBookmark("procedural1_start_section");
                        MSWord.deleteBookmarkComplete("procedural1_start_section");
                    }






                    if (blHasUtilization)
                    {
                        strBookmarkName = "utilization2_start";

                        MSWord.deleteBookmarkComplete("utilization2_start_section");


                        strSQL = "select Measure_desc from dbo.PBP_Profile_Ph3 as a where for_page1=1 and MPIN=" + strMPIN + " order by Hierarchy_Id DESC";


                        dtActionableItems = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        foreach(DataRow row in dtActionableItems.Rows)
                        {

                            if (row["Measure_desc"].ToString().Trim().ToLower().Equals("specialty specific diagnostics"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                

                                strSheetname = "Spec_diag_det";
                               

                                strSQL = "select Category, Patient_Count, Visit_Count as [Script Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=35 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);


                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                                            MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:D" + (intEndingRowTmp - 1), MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark:true);
                                        }
                                    }
                                }
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("your tier 3 pharmacy utilization"))
                            {

                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "PCP_Tier3_sum_det";
                            
                                strSQL = "select Category, Patient_Count, Visit_Count as [Script Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=12 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);


                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("advanced imaging utilization"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Adv_img_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=17 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                                    intEndingRowTmp = 8;
                                    if (dt.Rows.Count < 4)
                                    {
                                        intEndingRowTmp = (4 + dt.Rows.Count);
                                        MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);
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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("radiology utilization"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Rad_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=11 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 76 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod76_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=34 and attr_mpin=" + strMPIN + "  order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 59 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod59_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=7 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 58 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod58_sum_det";
                                
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=33 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 51 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod51_sum_det";
                                
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=32 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 50 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod50_sum_det";
                                
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=31 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 25 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod25_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=6 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("modifier 24 utilization rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Mod24_sum_det";
                               
                                strSQL = "select Category,Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=30 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("level 4 & 5 consultation rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Consults_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=29 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("level 4 & 5 visit rate"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "Level4_5_sum_det";
                                
                                strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=5 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("out of network (oon) lab utilization"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "OON_lab_sum_det";
                                

                                strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=16 and attr_mpin=" + strMPIN + " order by Catg_order";



                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {

                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("laboratory/pathology utilization"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "LabPath_sum_det";
                               
                                strSQL = "select Category, Patient_Count, Visit_Count as [Procedure Count], Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=4 and attr_mpin=" + strMPIN + " order by Catg_order";



                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("average length of stay (alos)"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "ALOS_sum_det";
                                
                                strSQL = "select Category, Patient_Count, Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=3 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("inpatient admission utilization"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "IP_sum_det";
                               
                                strSQL = "select Category, Patient_Count,Visit_Count, Pct_Cost from dbo.PBP_act_ph3 where Measure_ID=2 and attr_mpin=" + strMPIN + " order by Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }
                            else if (row["Measure_desc"].ToString().Trim().ToLower().Equals("emergency department (ed) utilization"))
                            {
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                strSheetname = "ED_sum_det";
                                
                                strSQL = "SELECT Category, Patient_Count, Visit_Count, Pct_Cost FROM dbo.PBP_act_ph3 WHERE Measure_ID=1 AND attr_mpin=" + strMPIN + " ORDER BY Catg_order";

                                dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                                if (dt.Rows.Count > 0)
                                {
                                    alSection2Utilization.Add(strSheetname);

                                    MSExcel.populateTable(dt, strSheetname, 4, 'A');

                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                                    MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

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
                            }

                        }
                        

                        MSWord.deleteBookmarkComplete(strBookmarkName);

                    }
                    else
                    {
                        MSWord.cleanBookmark("utilization2_start_section");
                        MSWord.deleteBookmarkComplete("utilization2_start_section");
                    }




                    //if (blHasProcedural && blHasUtilization && utilizationCount > 2)//BOTH SECTIONS SO BREAKEM
                    //    MSWord.addpageBreak("p_u_pagebreak3");
                    //else
                    //    MSWord.deleteBookmarkComplete("p_u_pagebreak3");


                    if (blHasProcedural)
                    {
                        MSWord.deleteBookmarkComplete("procedural2_start_section");
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strBookmarkName = "procedural2_start";
                        
                        strSheetname = "Spinal_Fusion";
                        

                        strSQL = "select Category, meas_cnt as spine_cnt, tot_cnt as spine_lp_cnt, Pct as tot_dx_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=28 and MPIN=" + strMPIN + " order by catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<P_LastName>", LastName);

                            intEndingRowTmp = 9;
                            if (dt.Rows.Count < 5)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D8", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "Redo";
                        
                        strSQL = "select Category, meas_cnt as redo, tot_cnt as px_cnt, Pct as px_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=27 and MPIN=" + strMPIN + " order by catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 22;
                            if (dt.Rows.Count < 18)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "Unpl_ED";
                        
                        strSQL = "select Category, meas_cnt as ed_cnt, tot_cnt as px_cnt, Pct as tot_px_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=26 and MPIN=" + strMPIN + " order by catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 22;
                            if (dt.Rows.Count < 18)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "Unpl_admit";
                       
                        strSQL = "select Category, meas_cnt as admit_cnt, tot_cnt as px_cnt, Pct as tot_px_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=25 and MPIN=" + strMPIN + " order by catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 22;
                            if (dt.Rows.Count < 18)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "Complications";
                        
                        strSQL = "select Category, meas_cnt as compl_cnt, tot_cnt as px_cnt, Pct as tot_px_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=24 and MPIN=" + strMPIN + " order by catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 22;
                            if (dt.Rows.Count < 18)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D21", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "Stent_Rate";
                        
                        strSQL = "select Category, meas_cnt as nbr_stent, tot_cnt as cath_cnt, Pct as tot_cost_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=23 and MPIN=" + strMPIN + " order by catg_order";

                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 8;
                            if (dt.Rows.Count < 4)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "Neg_Cath";
                        strSQL = "select Category, meas_cnt as nbr_caths, tot_cnt as neg_cath_cnt, Pct as tot_cost_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=22 and MPIN=" + strMPIN + "  order by catg_order";
                        
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 8;
                            if (dt.Rows.Count < 4)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);
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
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strSheetname = "PreCath_Testing";
                        strSQL = "select Category, meas_cnt as nbr_caths, tot_cnt as cath_cnt, Pct as tot_cost_rate from dbo.PBP_act_PX_ph3 as a where Measure_ID=21 and MPIN="+ strMPIN +" order by catg_order";
                        
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection2Procedural.Add(strSheetname);
                            MSExcel.populateTable(dt, strSheetname, 4, 'A');

                            MSExcel.ReplaceInTableTitle("A2:D2", strSheetname, "<PracticeName>", practiceName);

                            intEndingRowTmp = 8;
                            if (dt.Rows.Count < 4)
                            {
                                intEndingRowTmp = (4 + dt.Rows.Count);
                                MSExcel.deleteRows("A" + intEndingRowTmp + ":D7", strSheetname);
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

                    else
                    {
                        MSWord.cleanBookmark("procedural2_start_section");
                        MSWord.deleteBookmarkComplete("procedural2_start_section");
                    }


                    //if (blHasUtilization && blHasProcedural)//BOTH SECTIONS SO BREAKEM
                    //    MSWord.addpageBreak("p_u_pagebreak2");
                    //   // MSWord.addLineBreak("");
                    //else
                    //    MSWord.deleteBookmarkComplete("p_u_pagebreak2");

                    if (blHasUtilization)
                    {
                        MSWord.deleteBookmarkComplete("utilization3_start_section");


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strBookmarkName = "utilization3_start";

                        strSheetname = "all_meas";

                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Statistically Significant' from dbo.PBP_Profile_Ph3 as a where MPIN=" + strMPIN + " order by sort_id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection3Utilization.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');


                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", LastName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F20", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);

                                
                            }
                        }

                        MSWord.deleteBookmarkComplete(strBookmarkName);
                    }

                    else
                    {
                        MSWord.cleanBookmark("utilization3_start_section");
                        MSWord.deleteBookmarkComplete("utilization3_start_section");
                    }






                    if (blHasProcedural)
                    {
                        MSWord.deleteBookmarkComplete("procedural3_start_section");

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        strBookmarkName = "procedural3_start";

                        strSheetname = "NOS_Proced_outl_all2";



                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where measure_id=28 and NDB_Specialty='NOS' and a.MPIN=" + strMPIN + " order by Hierarchy_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            alSection3Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_FirstName>", FirstName);
                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<P_LastName>", LastName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F3", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "NOS_Proced_outl_all1";


                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where measure_id between 24 and 27 and NDB_Specialty='NOS' and a.MPIN=" + strMPIN + " order by Hierarchy_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            alSection3Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<PracticeName>", practiceName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);

                                
                            }
                        }

                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "ENT_GS_GI_Urol_Opht_Proced_all";

                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where measure_id between 24 and 27 and NDB_Specialty not in('CARDIOLOGY','NOS') and a.MPIN=" + strMPIN + " order by Hierarchy_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {
                            alSection3Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<PracticeName>", practiceName);

                            if (blHasWord)
                            {

                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }


                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        strSheetname = "Card_Proced_all";


                        strSQL = "select act_display, expected_display, var_display, case when signif is null then ' ' else signif end as 'Stat_Sign' from dbo.PBP_Profile_Px_ph3 as a Inner join dbo.PBP_outl_Ph3 as o on a.MPIN=o.MPIN where measure_id between 21 and 27 and NDB_Specialty like 'CARD%' and a.MPIN=" + strMPIN + " order by Hierarchy_Id";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count > 0)
                        {

                            alSection3Procedural.Add(strSheetname);

                            MSExcel.populateTable(dt, strSheetname, 3, 'C');

                            MSExcel.ReplaceInTableTitle("A1:F1", strSheetname, "<PracticeName>", practiceName);

                            if (blHasWord)
                            {
                                MSWord.tryCount = 0;
                                MSWord.pasteExcelTableToWord(strBookmarkName, strSheetname, "A1:F9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, blAddBookmark: true);
                            }
                        }


                        MSWord.deleteBookmarkComplete(strBookmarkName);

                    }

                    else
                    {
                        MSWord.cleanBookmark("procedural3_start_section");
                        MSWord.deleteBookmarkComplete("procedural3_start_section");
                    }






                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    strBookmarkName = "appendix";


                    if (blHasProcedural)
                    {

                        if (strSpecialty.ToUpper().Trim().Equals("CARDIOLOGY"))
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_CARD", "A1:C8", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        }
                        else if (strSpecialty.ToUpper().Trim().Equals("NEUROSURGERY, ORTHOPEDICS AND SPINE"))
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_NOS", "A1:C6", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        }
                        else
                        {
                            MSWord.tryCount = 0;
                            MSWord.pasteExcelTableToWord(strBookmarkName, "Proc_Others", "A1:C5", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        }

                    }

                    if (blHasUtilization)
                    {


                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_2", "A1:C9", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);
                        MSWord.tryCount = 0;
                        MSWord.pasteExcelTableToWord(strBookmarkName, "Util_pg_1", "A1:C11", MSExcel.xlsApp, MSExcel.xlsWB, MSExcel.xlsSheet, intLineBreakCnt, true);

                    }

                    MSWord.deleteBookmarkComplete(strBookmarkName);
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    int lineNumber = 0;

                    if (blHasUtilization)
                    {

                        //lineNumber = MSWord.getLineNumber("procedural1_start_breakPoint");


                        processBreaks(alSection1Utilization, 1);
                        processTopBreaks(alSection1Utilization, 1);

                    }



                    if (blHasProcedural)
                    {
                        lineNumber = MSWord.getLineNumber("procedural1_start_breakPoint");

                        if (blHasUtilization && (lineNumber >= 25 || intProcRowTotal >=4))
                            MSWord.addBreak("procedural1_start_breakPoint");

                        processBreaks(alSection1Procedural, 1);

                        processTopBreaks(alSection1Procedural, 1);
                    }


                    if (blHasUtilization)
                    {

                        lineNumber = MSWord.getLineNumber("utilization2_start_breakPoint");

                        if (lineNumber > 1)
                        {
                            MSWord.addpageBreak("utilization2_start_breakPoint");
                        }
                        lineNumber = MSWord.getLineNumber("utilization2_start_breakPoint");

                        if (lineNumber == 1)
                        {
                            MSWord.addLineBreak("utilization2_start_breakPoint");
                        }


                       
                        processBreaks(alSection2Utilization, 2);

                        processTopBreaks(alSection2Utilization, 2);
                    }

                    if (blHasProcedural)
                    {

                        lineNumber = MSWord.getLineNumber("procedural2_start_breakPoint");


                        if (lineNumber > 1)
                        {
                            MSWord.addpageBreak("procedural2_start_breakPoint");
                        }

                        lineNumber = MSWord.getLineNumber("procedural2_start_breakPoint");
                        if (lineNumber == 1)
                        {
                            MSWord.addLineBreak("procedural2_start_breakPoint");
                        }
                       
                        processBreaks(alSection2Procedural, 2);

                        processTopBreaks(alSection2Procedural, 2);
                    }

                    if (blHasUtilization)
                    {

                        lineNumber = MSWord.getLineNumber("utilization3_start_breakPoint");

                        if (lineNumber > 1)
                        {
                            MSWord.addpageBreak("utilization3_start_breakPoint");
                        }

                        lineNumber = MSWord.getLineNumber("utilization3_start_breakPoint");

                        if (lineNumber == 1)
                        {
                            MSWord.addLineBreak("utilization3_start_breakPoint");
                        }



                        processBreaks(alSection3Utilization, 3);
                        processTopBreaks(alSection3Utilization, 3);

                    }



                    if (blHasProcedural)
                    {

                        lineNumber = MSWord.getLineNumber("procedural3_start_breakPoint");

                        if (lineNumber > 1)
                        {
                            MSWord.addpageBreak("procedural3_start_breakPoint");
                        }

                        lineNumber = MSWord.getLineNumber("procedural3_start_breakPoint");

                        if (lineNumber == 1)
                        {
                            MSWord.addLineBreak("procedural3_start_breakPoint");
                        }

                     
                        processBreaks(alSection3Procedural, 3);
                        processTopBreaks(alSection3Procedural, 3);


                    }


                    MSWord.deleteBookmarkComplete("procedural1_start_breakPoint");

                    MSWord.deleteBookmarkComplete("utilization2_start_breakPoint");
                    MSWord.deleteBookmarkComplete("utilization3_start_breakPoint");

                    MSWord.deleteBookmarkComplete("procedural2_start_breakPoint");
                    MSWord.deleteBookmarkComplete("procedural3_start_breakPoint");


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

                    //CLEANUP SECTION PAGE FOR ORIENTATION
                    //strBookmarkName = "section_break";

                    //if (MSWord.BookmarkExists(strBookmarkName + "_3"))
                    //{
                    //    if (blHasProcedural == false)
                    //        MSWord.cleanBookmark(strBookmarkName + "_3");

                    //    MSWord.deleteBookmarkComplete(strBookmarkName + "_3");
                    //}


                    //if (MSWord.BookmarkExists(strBookmarkName + "_2"))
                    //{
                    //    MSWord.cleanBookmark(strBookmarkName + "_2");
                    //    MSWord.deleteBookmarkComplete(strBookmarkName + "_2");
                    //}

                    //if (MSWord.BookmarkExists(strBookmarkName))
                    //{
                    //    MSWord.cleanBookmark(strBookmarkName);
                    //    MSWord.deleteBookmarkComplete(strBookmarkName);
                    //}




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


                    if((i+1) < al.Count)
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
