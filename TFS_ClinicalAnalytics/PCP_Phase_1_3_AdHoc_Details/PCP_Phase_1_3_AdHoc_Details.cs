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

namespace PCP_Phase_1_3_AdHoc_Details
{
    class PCP_Phase_1_3_AdHoc_Details
    {
        static void Main(string[] args)
        {


            bool blIsProcess = false;

            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strExcelTemplate = ConfigurationManager.AppSettings["ExcelTemplate"];
            string strReportsPath = ConfigurationManager.AppSettings["ReportsPath"];



            string strMpinCSV = ConfigurationManager.AppSettings["MpinCSV"];



            //strMpinCSV = "1810025";


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

            DataTable dt = null;
            string strSheetname = null;

            string strMonthYear = DateTime.Now.Month + "_" + DateTime.Now.Year;
            string strFinalReportFileName;

            string strSQL = null;
            string strMPIN = null;

            string phyName = null;
            string LastName = null;
            string phyState = null;
            string strSpecialty = null;
            string strMarketName = null;

            int intMPINTotal = 0;
            Int16 intCnt = 1;


            int intRowCount = 0;
            int intRowAdd = 0;

            try
            {
                MSExcel.populateExcelParameters(false, true, strReportsPath, strExcelTemplate);
                MSExcel.openExcelApp();

                MSExcel.strReportsPath = strReportsPath;


                intMPINTotal = strMpinArr.Length;

                foreach (string s in strMpinArr)
                {

                    alActiveSheets = new ArrayList();

                    alActiveRanges = new ArrayList();
                    intRowAdd = 0;
                    strMPIN = s.Trim();



                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();


                    //GET MPIN INFO START
                    //strSQL = "select FirstName,LastName,Spec_display as NDB_Specialty,o.MKT_RLLP_NM , [State] AS [State] from dbo.PBP_outl_ph12 as o inner join dbo.PBP_outl_demogr_Ph12 as d on d.mpin=o.mpin where o.MPIN = " + strMPIN;
                    //strSQL = "select FirstName,LastName,Spec_display,o.MKT_RLLP_NM, [State] AS [State]  from dbo.PBP_outl_ph13 as o inner join dbo.PBP_outl_demogr_Ph13 as d on d.mpin=o.mpin where exclude in(0,5) and OE_Allw is not null and  o.MPIN = " + strMPIN;


                    strSQL = "select FirstName,LastName,Spec_display,o.MKT_RLLP_NM, [State] AS [State]  from dbo.PBP_outl_ph13 as o inner join dbo.PBP_outl_demogr_Ph13 as d on d.mpin=o.mpin where exclude in(0,5) and  o.MPIN = " + strMPIN;

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        Console.WriteLine("No records found for MPIN: " + strMPIN);
                        continue;
                    }

                    phyName = (dt.Rows[0]["LastName"] != DBNull.Value ? (dt.Rows[0]["FirstName"].ToString().Trim() + " " + dt.Rows[0]["LastName"].ToString().Trim()) : "NAME MISSING");
                    LastName = (dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString().Trim() : "NAME MISSING");
                    phyState = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString().Trim() : "STATE MISSING");
                    strSpecialty = (dt.Rows[0]["Spec_display"] != DBNull.Value ? dt.Rows[0]["Spec_display"].ToString().Trim() : "SPECIALTY MISSING");
                    strMarketName = (dt.Rows[0]["MKT_RLLP_NM"] != DBNull.Value ? dt.Rows[0]["MKT_RLLP_NM"].ToString().Trim() : "SPECIALTY MISSING");

                    //strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState + "_" + strMonthYear;
                    strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState;
                    strFinalReportFileName = strFinalReportFileName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "") + "_Details_" + strMonthYear;

                    //GET MPIN INFO END


                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS,AHRQ_Diagnosis_Category";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";


                    strSQL = "select distinct E.MBR_FST_NM, E.MBR_LST_NM,E.INDV_BTH_DT, M.RP_RISK_CATGY,M.PRDCT_LVL_1_NM, DOS, case when Sens is null then AHRQ_Diagnosis_Category else 'SENSITIVE HEALTH CONDITION' end as AHRQ_Diagnosis_Category from dbo.PBP_ER_Details_Ph13 E INNER JOIN DBO.PBP_MPIN_CLIENT_Ph13 M ON E.INDV_SYS_ID=M.INDV_SYS_ID AND E.ATTR_MPIN=M.ATTR_MPIN INNER JOIN dbo.PBP_dx as D on E.DIAG_1_CD=D.diag_cd where E.attr_mpin =" + strMPIN + " and OP_EVENT_KEY <>0 AND E.MBR_LST_NM IS NOT NULL group by e.attr_mpin,e.INDV_SYS_ID,E.MBR_FST_NM,E.MBR_LST_NM,E.INDV_BTH_DT,M.RP_RISK_CATGY,AHRQ_Diagnosis_Category, M.PRDCT_LVL_1_NM,DOS,case when Sens is null then AHRQ_Diagnosis_Category else 'SENSITIVE HEALTH CONDITION' end having SUM(DERIV_ALLW_AMT)<>0 ORDER BY MBR_LST_NM ASC, MBR_FST_NM ASC, INDV_BTH_DT ASC, DOS ASC, AHRQ_Diagnosis_Category ASC";


                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    strSheetname = "Hospital_Admissions";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY,/*LOS*/ APR_DRG from dbo.VW_PBP_IP_dtl_ph12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT,APR_DRG";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,APR_DRG,ADMIT_DT";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":I" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":I" + (dt.Rows.Count + 14), strSheetname);

                        intRowAdd = 0;

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("I" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Lab_and_Path SHEET START///////////////
                    //Lab_and_Path SHEET START///////////////
                    //Lab_and_Path SHEET START///////////////
                    strSheetname = "Lab_and_Path";
                    //strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph12 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";




                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    strSheetname = "Out-of-network_lab_and_path";
                    //strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph12 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph13 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////START 2018.////////////////////////////////////////////////////////////////////




                    //Level_4_and_5_visits SHEET START///////////////
                    //Level_4_and_5_visits SHEET START///////////////
                    //Level_4_and_5_visits SHEET START///////////////
                    strSheetname = "Level_4_and_5_visits";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Level_4_and_5_visits SHEET END///////////////
                    //Level_4_and_5_visits SHEET END///////////////
                    //Level_4_and_5_visits SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_Util SHEET START///////////////
                    //Modifier_Util SHEET START///////////////
                    //Modifier_Util SHEET START///////////////
                    strSheetname = "Modifier_Util";

                   // strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl12 where attr_mpin=" + strMPIN + "order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Modifier_Util SHEET END///////////////
                    //Modifier_Util SHEET END///////////////
                    //Modifier_Util SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network_Util SHEET START///////////////
                    //Out-of-network_Util SHEET START///////////////
                    //Out-of-network_Util SHEET START///////////////
                    strSheetname = "Out-of-network_Util";

                    
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,SPEC_TYP_NM,PROV_TYP_NM from dbo.VW_PBP_OON_dtl_ph12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";


                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,PROV_TYP_NM,SPEC_TYP_NM from dbo.VW_PBP_OON_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        intRowAdd = 0;
                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Out-of-network_Util SHEET END///////////////
                    //Out-of-network_Util SHEET END///////////////
                    //Out-of-network_Util SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Specialty_Physician_Util SHEET START///////////////
                    //Specialty_Physician_Util SHEET START///////////////
                    //Specialty_Physician_Util SHEET START///////////////
                    strSheetname = "Specialty_Physician_Util";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";


                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        intRowAdd = 0;
                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Specialty_Physician_Util SHEET END///////////////
                    //Specialty_Physician_Util SHEET END///////////////
                    //Specialty_Physician_Util SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    strSheetname = "Advanced_Imaging";
                   // strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category,Proc_count from dbo.VW_PBP_AdvIm_dtl_ph12 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Non-Advanced_Imaging SHEET START///////////////
                    //Non-Advanced_Imaging SHEET START///////////////
                    //Non-Advanced_Imaging SHEET START///////////////
                    strSheetname = "Non-Advanced_Imaging";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph12 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph13 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph13 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
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
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Non-Advanced_Imaging SHEET END///////////////
                    //Non-Advanced_Imaging SHEET END///////////////
                    //Non-Advanced_Imaging SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Opioids SHEET START///////////////
                    //Opioids SHEET START///////////////
                    //Opioids SHEET START///////////////

                    strSheetname = "Opioids";
                    //strSQL = "Select Distinct c.MBR_FST_NM, c.MBR_LST_NM, c.INDV_BTH_DT, 'COMMERCIAL' as Product From dbo.Opioid_Indv_Dtl as a Inner Join (SELECT * FROM PBP_outl_Ph12 WHERE EXCLUDE in (0,5) and Opiod_Outl=1) as b on a.PR_MPIN=b.mpin Inner join dbo.PBP_MPIN_CLIENT_Ph12 as c on a.INDV_SYS_ID=c.INDV_SYS_ID Where PR_MPIN=" + strMPIN + " Order by c.MBR_LST_NM, c.MBR_FST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");


                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);

                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":C" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":C" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("C" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Opioids SHEET END///////////////
                    //Opioids SHEET END///////////////
                    //Opioids SHEET END///////////////



                    //Antibiotics SHEET START///////////////
                    //Antibiotics SHEET START///////////////
                    //Antibiotics SHEET START///////////////

                    strSheetname = "Antibiotics";

                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_ABX_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");


                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);

                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":D" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":D" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("D" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Antibiotics SHEET END///////////////
                    //Antibiotics SHEET END///////////////
                    //Antibiotics SHEET END///////////////




                    //Med_Adherence SHEET START///////////////
                    //Med_Adherence SHEET START///////////////
                    //Med_Adherence SHEET START///////////////

                    strSheetname = "Med_Adherence";

                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_MedAdherence_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");


                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);

                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":D" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":D" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("D" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Med_Adherence SHEET END///////////////
                    //Med_Adherence SHEET END///////////////
                    //Med_Adherence SHEET END///////////////







                    //NO APPENDIX....SO FAR
                    //alActiveSheets.Add("Appendix");
                    //alActiveRanges.Add("B11");



                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
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


                Console.WriteLine("Closing Microsoft Excel Instance...");
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();

            }





        }
    }
}
