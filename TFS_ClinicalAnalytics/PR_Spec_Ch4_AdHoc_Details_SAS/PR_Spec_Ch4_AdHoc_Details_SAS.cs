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

namespace PR_Spec_Ch4_AdHoc_Details_SAS
{
    class PR_Spec_Ch4_AdHoc_Details_SAS
    {


       

        static void Main(string[] args)
        {

            bool blIsProcess = false;


            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strExcelTemplate = ConfigurationManager.AppSettings["ExcelTemplate"];
            string strReportsPath = ConfigurationManager.AppSettings["ReportsPath"];



            string strMpinCSV = ConfigurationManager.AppSettings["MpinCSV"];
            string[] strMpinArr = null;
            if (!String.IsNullOrEmpty(strMpinCSV))
            {
                strMpinArr = strMpinCSV.Split(',');
            }


            if (args.Count() == 2)
            {
                strMpinArr = args[0].ToString().Split(',');
                strReportsPath = args[1].ToString();
                blIsProcess = true;
                //Console.WriteLine(args[0]);
                //Console.WriteLine(args[1]);
                //Console.ReadLine();
            }




            if (strMpinArr == null)
            {

                Console.WriteLine("No MPINS found");
                return;

            }


            ArrayList alActiveSheets = new ArrayList();
            ArrayList alActiveRanges = new ArrayList();

   
            string strSheetname = null;
            string strTopRange = null;

            string strMonthYear = DateTime.Now.Month + "_" + DateTime.Now.Year;
            string strFinalReportFileName;

            string strSQL = null;
            string strMPIN = null;
            string FirstName = null;
            string phyName = null;
            string LastName = null;
            string phyState = null;
            string strSpecialty = null;
            string strMarketName = null;

            int intMPINTotal = 0;
            Int16 intCnt = 1;


            int intRowCount = 0;


            int intRowAdd = 0;



            IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];



            try
            {
                MSExcel.populateExcelParameters(false, true, strReportsPath, strExcelTemplate);
                MSExcel.openExcelApp();

                MSExcel.strReportsPath = strReportsPath;


                DataTable dt = new DataTable();

                intMPINTotal = strMpinArr.Length;


                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


                foreach (string s in strMpinArr)
                {
                    alActiveSheets = new ArrayList();
                    alActiveRanges = new ArrayList();

                    intRowAdd = 0;
                    strMPIN = s.Trim();


                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();




                    //strSQL = "select FirstName, LastName,a.Spec_display as NDB_Specialty,b.[State],a.MKT_RLLP_NM from dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph33 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = " + strMPIN;


                    strSQL = "select a.MPIN,a.attr_clients as clients,P_LastName,FirstName, LastName,P_FirstName,ProvDegree, b.Spec_display as NDB_Specialty,b.Street,b.City,b.State,b.zipcd,b.taxid, p.MPIN as practice_id,p.Practice_Name,Tot_Util_meas,Tot_PX_meas, Model_id,opi_clients, RCMO,RCMO_title,RCMO_title1,Special_Handling,Folder_Name, b.MKT_RLLP_NM from Ph34.outliers as a inner join Ph34.UHN_MAY6_DEMOG as b on a.MPIN=b.MPIN inner join Ph34.UHN_MAY6_PTI_DEMOG as p on p.mpin=PTIGroupID_upd inner join Ph34.outl_models as m on m.mpin=a.mpin inner join Ph34.spec_handling as h on h.MPIN=a.mpin inner join IL_UCA.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN=" + strMPIN + "; ";


                    dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);



                    phyName = (dt.Rows[0]["LastName"] != DBNull.Value ? (dt.Rows[0]["FirstName"].ToString().Trim() + " " + dt.Rows[0]["LastName"].ToString().Trim()) : "NAME MISSING");
                    LastName = (dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString().Trim() : "NAME MISSING");



                    LastName = (dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString().Trim() : null);
                    FirstName = (dt.Rows[0]["FirstName"] != DBNull.Value ? dt.Rows[0]["FirstName"].ToString().Trim() : null);
                    phyState = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString().Trim() : "STATE MISSING");
                    strSpecialty = (dt.Rows[0]["NDB_Specialty"] != DBNull.Value ? dt.Rows[0]["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");
                    strMarketName = (dt.Rows[0]["MKT_RLLP_NM"] != DBNull.Value ? dt.Rows[0]["MKT_RLLP_NM"].ToString().Trim() : "SPECIALTY MISSING");

                    //strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState + "_" + strMonthYear;
                    strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState;
                    strFinalReportFileName = strFinalReportFileName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "") + "_Details_" + strMonthYear;

                    if (!String.IsNullOrEmpty(FirstName))
                    {
                        phyName =  FirstName + " " + LastName;
                    }
                    else
                        phyName = LastName;






                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";
                    strTopRange = "G";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,PUT(DOS ,MMDDYY10.) as DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,PUT(DOS ,MMDDYY10.) as DOS,AHRQ_Diagnosis_Category from ph34.V_ER_dt where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from ph34.V_ER_dt where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";



                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////



                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    strSheetname = "Hospital_Admissions";
                    strTopRange = "I";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(ADMIT_DT ,MMDDYY10.) as ADMIT_DT,PUT(DSCHRG_DT ,MMDDYY10.) as DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph32 where attr_mpin=" + strMPIN;
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(ADMIT_DT ,MMDDYY10.) as ADMIT_DT,PUT(DSCHRG_DT ,MMDDYY10.) as DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph33 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                   // strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(ADMIT_DT ,MMDDYY10.) as ADMIT_DT,PUT(DSCHRG_DT ,MMDDYY10.) as DSCHRG_DT,STAT_DAY, APR_DRG from ph34.V_IP_admdt_dt where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from ph34.V_IP_admdt_dt where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM, ADMIT_DT;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////



                    //Lab_and_Path SHEET START///////////////
                    //Lab_and_Path SHEET START///////////////
                    //Lab_and_Path SHEET START///////////////
                    strSheetname = "Lab_and_Path";
                    strTopRange = "G";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph33 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from ph34.V_LP_dt where attr_mpin =" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from ph34.V_LP_dt where attr_mpin =" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////



                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    strSheetname = "Out-of-network_lab_and_path";
                    strTopRange = "G";
                  
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph33 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from ph34.V_LPOON_dt where attr_mpin =" + strMPIN + "  order by mbr_lst_nm,MBR_FST_NM;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from ph34.V_LPOON_dt where attr_mpin =" + strMPIN + "  order by mbr_lst_nm,MBR_FST_NM;";
                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////




                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    strSheetname = "Level_4_and_5_E&M_visits";
                    strTopRange = "H";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(DOS ,MMDDYY10.) as DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(DOS ,MMDDYY10.) as DOS,PROC_CD,PROC_DESC from ph34.V_LVL45_ptdet where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from ph34.V_LVL45_ptdet where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Level_4_and_5_E&M_visits SHEET END///////////////
                    //Level_4_and_5_E&M_visits SHEET END///////////////
                    //Level_4_and_5_E&M_visits SHEET END///////////////


                    //Level_4_and_5_E&M_consults SHEET START///////////////
                    //Level_4_and_5_E&M_consults SHEET START///////////////
                    //Level_4_and_5_E&M_consults SHEET START///////////////
                    strSheetname = "Level_4_and_5_E&M_consults";
                    strTopRange = "H";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(DOS ,MMDDYY10.) as DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45C_dtl33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(DOS ,MMDDYY10.) as DOS,PROC_CD,PROC_DESC from ph34.V_LVL45Cons_pdtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from ph34.V_LVL45Cons_pdtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Level_4_and_5_E&M_consults SHEET END///////////////
                    //Level_4_and_5_E&M_consults SHEET END///////////////
                    //Level_4_and_5_E&M_consults SHEET END///////////////


                    //Modifier_Utilization SHEET START///////////////
                    //Modifier_Utilization SHEET START///////////////
                    //Modifier_Utilization SHEET START///////////////
                    strSheetname = "Modifier_Utilization";
                    strTopRange = "G";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from ph34.V_Mod25_pdet where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from ph34.V_Mod25_pdet where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Modifier_Utilization SHEET END///////////////
                    //Modifier_Utilization SHEET END///////////////
                    //Modifier_Utilization SHEET END///////////////

                    //Procedure_Modifer SHEET START///////////////
                    //Procedure_Modifer SHEET START///////////////
                    //Procedure_Modifer SHEET START///////////////
                    strSheetname = "Procedure_Modifer";
                    strTopRange = "G";
                    // strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Modpx_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";
                   // strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Modpx_dtl33 where attr_mpin=" + strMPIN;
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from ph34.V_pxm_pdtls where attr_mpin=" + strMPIN + ";";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from ph34.V_pxm_pdtls where attr_mpin=" + strMPIN + ";";
                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Procedure_Modifer SHEET END///////////////
                    //Procedure_Modifer SHEET END///////////////
                    //Procedure_Modifer SHEET END///////////////



                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    strSheetname = "Advanced_Imaging";
                    strTopRange = "G";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph32 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                   // strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category as Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph33 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category, Proc_count from ph34.V_AdvImg_dt where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM;";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM, INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category, Proc_count from ph34.V_AdvImg_dt where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////


                    //Non_Advanced_Imaging SHEET START///////////////
                    //Non_Advanced_Imaging SHEET START///////////////
                    //Non_Advanced_Imaging SHEET START///////////////
                    strSheetname = "Non_Advanced_Imaging";
                    strTopRange = "G";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph32 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category as Rad_Gen_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph33 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category, Proc_count from ph34.V_NAI_act_dt where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM;";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category, Proc_count from ph34.V_NAI_act_dt where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Non_Advanced_Imaging SHEET END///////////////
                    //Non_Advanced_Imaging SHEET END///////////////
                    //Non_Advanced_Imaging SHEET END///////////////





                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    strSheetname = "Specialty_Specific_Diagnostics";
                    strTopRange = "G";
                    // strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from dbo.VW_PBP_sp_specif_dtl_ph32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from dbo.VW_PBP_sp_specif_dtl_ph33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from ph34.V_ssp_pdeet where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM;";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from ph34.V_ssp_pdeet where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Specialty_Specific_Diagnostics SHEET END///////////////
                    //Specialty_Specific_Diagnostics SHEET END///////////////
                    //Specialty_Specific_Diagnostics SHEET END///////////////


                    //Cesarean SHEET START///////////////
                    //Cesarean SHEET START///////////////
                    //Cesarean SHEET START///////////////
                    strSheetname = "Cesarean";
                    strTopRange = "G";
                    //strSQL = "SELECT MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AGECAT, DELIVERY_TYPE, DELIVERY_DT FROM VW_PBP_CSection_dtl_ph32 WHERE attr_mpin=" + strMPIN + " AND DELIVERY_TYPE= 'NORMAL C-SECTION' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AGECAT, DELIVERY_TYPE, DELIVERY_DT FROM VW_PBP_CSection_dtl_ph33 Where attr_mpin=" + strMPIN + " AND DELIVERY_TYPE like'%C-Section%' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";
                    //strSQL = "Select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,AGE_CAT, DELIVERY_TYPE,PUT(DELIVERY_DT ,MMDDYY10.) as DELIVERY_DT  FROM ph34.V_CSect_dt Where attr_mpin=" + strMPIN + " AND DELIVERY_TYPE like'%C-SECTION%' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT;";
                    strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, product ,AGE_CAT, DELIVERY_TYPE,DELIVERY_DT FROM ph34.V_CSect_dt Where attr_mpin=" + strMPIN + " AND DELIVERY_TYPE like'%C-SECTION%' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Cesarean SHEET END///////////////
                    //Cesarean SHEET END///////////////
                    //Cesarean SHEET END///////////////





                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    strSheetname = "Pre_Cardiac_Cath_Dx_Testing";
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_PreCardic_Cath_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,[PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_PreCardic_Cath_dtl_Ph33 Where ATTR_MPIN=" + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                   // strSQL = "Select MBR_FST_NM, MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,PROCEDURE, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE ,DX_CATGY FROM ph34.V_PRECATH_DTL Where ATTR_MPIN= " + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM,INDV_BTH_DT, product,PROCEDURE,PROCEDURE_DATE ,DX_CATGY FROM ph34.V_PRECATH_DTL Where ATTR_MPIN= " + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Pre_Cardiac_Cath_Dx_Testing SHEET END///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET END///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET END///////////////


                    //Neg_Card_Catheterization SHEET START///////////////
                    //Neg_Card_Catheterization SHEET START///////////////
                    //Neg_Card_Catheterization SHEET START///////////////
                    strSheetname = "Neg_Card_Catheterization";
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Neg_Cath_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Neg_Cath_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PROCEDURE, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE, DX_CATGY FROM ph34.V_Neg_Cath_dtl Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product, PROCEDURE, PROCEDURE_DATE, DX_CATGY FROM ph34.V_Neg_Cath_dtl Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Neg_Card_Catheterization SHEET END///////////////
                    //Neg_Card_Catheterization SHEET END///////////////
                    //Neg_Card_Catheterization SHEET END///////////////


                    //Cardiac_Stent SHEET START///////////////
                    //Cardiac_Stent SHEET START///////////////
                    //Cardiac_Stent SHEET START///////////////
                    strSheetname = "Cardiac_Stent";
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Stent_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Stent_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE, DX_CATGY FROM ph34.V_Stent_dtl Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product, Procedure, PROCEDURE_DATE, DX_CATGY FROM ph34.V_Stent_dtl Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Cardiac_Stent SHEET END///////////////
                    //Cardiac_Stent SHEET END///////////////
                    //Cardiac_Stent SHEET END///////////////




                    //Post-op_Complications SHEET START///////////////
                    //Post-op_Complications SHEET START///////////////
                    //Post-op_Complications SHEET START///////////////
                    strSheetname = "Post-op_Complications";
                    strTopRange = "H";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, SEVERITY, POS, PEG_ANCH_CATGY_DESC as 'PROCEDURE', PEG_ANCH_DT as Procedure_Date FROM VW_PBP_Compl30_dtl_Ph32 Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, SEVERITY, POS, PEG_ANCH_CATGY_DESC as 'PROCEDURE', PEG_ANCH_DT as Procedure_Date FROM VW_PBP_Compl30_dtl_Ph33 Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                   // strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, SEVERITY, POS, PEG_ANCH_CATGY_DESC as PROCEDURE, PUT(PEG_ANCH_DT ,MMDDYY10.) as Procedure_Date FROM ph34.V_POSTOPCP_DTL Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, Procedure_Date;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, product, POS, PEG_ANCH_CATGY_DESC as PROCEDURE, PEG_ANCH_DT  as Procedure_Date FROM ph34.V_POSTOPCP_DTL Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, Procedure_Date;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Post-op_Complications SHEET END///////////////
                    //Post-op_Complications SHEET END///////////////
                    //Post-op_Complications SHEET END///////////////



                    //Unplanned_Hospital_Admits SHEET START///////////////
                    //Unplanned_Hospital_Admits SHEET START///////////////
                    //Unplanned_Hospital_Admits SHEET START///////////////
                    strSheetname = "Unplanned_Hospital_Admits";
                    strTopRange = "H";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Admit30_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and post_admit_ind='Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AprDrg_svrty, srvc_loc as surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Admit30_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + "and post_admit_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AprDrg_svrty, srvc_loc, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE FROM ph34.V_POSTOPADM_dtl Where ATTR_MPIN= " + strMPIN + " and post_admit_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, product, srvc_loc, Procedure, PROCEDURE_DATE FROM ph34.V_POSTOPADM_dtl Where ATTR_MPIN= " + strMPIN + " and post_admit_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Unplanned_Hospital_Admits SHEET END///////////////
                    //Unplanned_Hospital_Admits SHEET END///////////////
                    //Unplanned_Hospital_Admits SHEET END///////////////


                    //Unplanned_ED_Visits SHEET START///////////////
                    //Unplanned_ED_Visits SHEET START///////////////
                    //Unplanned_ED_Visits SHEET START///////////////
                    strSheetname = "Unplanned_ED_Visits";
                    strTopRange = "H";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ED30_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and post_ed_ind='Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                   // strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AprDrg_svrty, srvc_loc as surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ED30_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and post_ed_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                   // strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, AprDrg_svrty, srvc_loc, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE FROM Ph34.V_POSTOPED_dtl Where ATTR_MPIN= " + strMPIN + " and post_ed_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, product, srvc_loc, Procedure,  PROCEDURE_DATE FROM Ph34.V_POSTOPED_dtl Where ATTR_MPIN= " + strMPIN + " and post_ed_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Unplanned_ED_Visits SHEET END///////////////
                    //Unplanned_ED_Visits SHEET END///////////////
                    //Unplanned_ED_Visits SHEET END///////////////



                    //Spinal_Fusion SHEET START///////////////
                    //Spinal_Fusion SHEET START///////////////
                    //Spinal_Fusion SHEET START///////////////
                    strSheetname = "Spinal_Fusion";
                    strTopRange = "G";
                    // strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PRODUCT, [PROCEDURE], PROCEDURE_DATE, FINAL_DX FROM VW_PBP_Spine_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND [PROCEDURE]='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PRODUCT, [PROCEDURE], PROCEDURE_DATE, FINAL_DX FROM VW_PBP_Spine_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " AND [Procedure]='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PRODUCT, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE, FINAL_DX FROM ph34.V_Spine_dtl Where ATTR_MPIN= " + strMPIN + " AND Procedure='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, PRODUCT, Procedure, PROCEDURE_DATE, FINAL_DX FROM ph34.V_Spine_dtl Where ATTR_MPIN= " + strMPIN + " AND Procedure='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Spinal_Fusion SHEET END///////////////
                    //Spinal_Fusion SHEET END///////////////
                    //Spinal_Fusion SHEET END///////////////

                    //Opioids SHEET START///////////////
                    //Opioids SHEET START///////////////
                    //Opioids SHEET START///////////////
                    strSheetname = "Opioids";
                    strTopRange = "D"; //D!!!!!!
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND Num_Cnt>=1 Order by MBR_LST_NM";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT FROM ph34.V_Opi_dtl Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product FROM ph34.V_Opi_dtl Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Opioids SHEET END///////////////
                    //Opioids SHEET END///////////////
                    //Opioids SHEET END///////////////



                    //Tympanostomy SHEET START///////////////
                    //Tympanostomy SHEET START///////////////
                    //Tympanostomy SHEET START///////////////
                    strSheetname = "Tympanostomy";
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Tymp_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Tymp_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE FROM ph34.V_Tymp_dtl Where ATTR_MPIN= " + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM,  INDV_BTH_DT, product, Procedure, PROCEDURE_DATE FROM ph34.V_Tymp_dtl Where ATTR_MPIN= " + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";


                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Tympanostomy SHEET END///////////////
                    //Tympanostomy SHEET END///////////////
                    //Tympanostomy SHEET END///////////////

                    //OON_Asst_Surgeon SHEET START///////////////
                    //OON_Asst_Surgeon SHEET START///////////////
                    //OON_Asst_Surgeon SHEET START///////////////
                    strSheetname = "OON_Asst_Surgeon";
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_OON_ASST_SURG_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_OON_ASST_SURG_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE FROM ph34.V_OONAST_dtl Where ATTR_MPIN= " + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product, Procedure, PROCEDURE_DATE FROM ph34.V_OONAST_dtl Where ATTR_MPIN= " + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //OON_Asst_Surgeon SHEET END///////////////
                    //OON_Asst_Surgeon SHEET END///////////////
                    //OON_Asst_Surgeon SHEET END///////////////


                    //OPH_v_ASC_Util SHEET START///////////////
                    //OPH_v_ASC_Util SHEET START///////////////
                    //OPH_v_ASC_Util SHEET START///////////////
                    strSheetname = "OPH_v_ASC_Util";
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_SOS_OPH_ASC_dtl_Ph32 Where attr_mpin=" + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_SOS_OPH_ASC_dtl_Ph33 Where attr_mpin= " + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE FROM ph34.V_SOS_dtl Where attr_mpin= " + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product, Procedure,  PROCEDURE_DATE FROM ph34.V_SOS_dtl Where attr_mpin= " + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //OPH_v_ASC_Util SHEET END///////////////
                    //OPH_v_ASC_Util SHEET END///////////////
                    //OPH_v_ASC_Util SHEET END///////////////


                    //Asst_Surgeon SHEET START///////////////
                    //Asst_Surgeon SHEET START///////////////
                    //Asst_Surgeon SHEET START///////////////
                    strSheetname = "Asst_Surgeon";
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ASST_SURG_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ASST_SURG_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, Procedure, PUT(PROCEDURE_DATE ,MMDDYY10.) as PROCEDURE_DATE FROM ph34.V_ASST_SURG_dtl Where ATTR_MPIN= " + strMPIN + "and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product, Procedure, PROCEDURE_DATE FROM ph34.V_ASST_SURG_dtl Where ATTR_MPIN= " + strMPIN + "and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Asst_Surgeon SHEET END///////////////
                    //Asst_Surgeon SHEET END///////////////
                    //Asst_Surgeon SHEET END///////////////


                    //CT_Chronic_Sinusitis SHEET START///////////////
                    //CT_Chronic_Sinusitis SHEET START///////////////
                    //CT_Chronic_Sinusitis SHEET START///////////////
                    strSheetname = "CT_Chronic_Sinusitis";
                    strTopRange = "D";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, FST_SINSTS_DX FROM VW_PBP_Sinusitis_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT FROM VW_PBP_Sinusitis_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM, MBR_FST_NM";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT FROM ph34.V_Sin_dtl Where ATTR_MPIN= " + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM, MBR_FST_NM;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product FROM ph34.V_Sin_dtl Where ATTR_MPIN= " + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM, MBR_FST_NM;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //CT_Chronic_Sinusitis SHEET END///////////////
                    //CT_Chronic_Sinusitis SHEET END///////////////
                    //CT_Chronic_Sinusitis SHEET END///////////////

                   


                    //Attended_Sleep_Study_Apnea SHEET START///////////////
                    //Attended_Sleep_Study_Apnea SHEET START///////////////
                    //Attended_Sleep_Study_Apnea SHEET START///////////////
                    strSheetname = "Attended_Sleep_Study_Apnea";
                    strTopRange = "E";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, SEVERITY, POS, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Redo_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND CMPLNT_IND='N' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, fst_srvc_dt as PROCEDURE_DATE FROM VW_PBP_sleep_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM, fst_srvc_dt";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PUT(fst_srvc_dt, MMDDYY10.) as PROCEDURE_DATE FROM ph34.V_SLEEP_DT Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, product, fst_srvc_dt as PROCEDURE_DATE FROM ph34.V_SLEEP_DT Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE;";

                    try { dt = DBConnection32.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);} catch(Exception) { dt = new DataTable();dt.Clear();}
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Attended_Sleep_Study_Apnea SHEET END///////////////
                    //Attended_Sleep_Study_ApneaSHEET END///////////////
                    //Attended_Sleep_Study_Apnea SHEET END///////////////









                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());




                    //CLOSE EXCEL WB
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //COMPLETED MESSAGE
                    Console.WriteLine(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "");
                    Console.WriteLine("----------------------------------------------------------------------------");

                    intCnt++;

                }

                Console.WriteLine("Process Completed");


                if (!blIsProcess)
                    Console.ReadLine();


            }
            catch (Exception ex)
            {


                Console.WriteLine("There was an error, see details below");
                Console.WriteLine(ex.ToString());
                Console.WriteLine();

                Console.Beep();


                Console.ReadLine();


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



                Console.WriteLine("Closing Microsoft Excel Instance...");
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();

            }





        }
    }
}
