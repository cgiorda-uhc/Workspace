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

namespace PR_Spec_AdHoc_Details
{
    class PR_Spec_AdHoc_Details
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

                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    intRowAdd = 0;
                    strMPIN = s.Trim();


                    //if (strMPIN != "251862")
                    //continue;

                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();


                    //GET MPIN INFO START


                    //strSQL = "select FirstName, LastName, Spec_display as NDB_Specialty, b.[State], a.MKT_RLLP_NM  from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5,15) and r.phase_id=2 and a.MPIN =" + strMPIN;


                    strSQL = "select FirstName, LastName, Spec_display as NDB_Specialty, b.[State], a.MKT_RLLP_NM  from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where r.phase_id=2 and a.MPIN =" + strMPIN;


                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        Console.WriteLine("No records found for MPIN: " + strMPIN);
                        continue;
                    }

                    phyName = (dt.Rows[0]["LastName"] != DBNull.Value ? (dt.Rows[0]["FirstName"].ToString().Trim() + " " + dt.Rows[0]["LastName"].ToString().Trim()) : "NAME MISSING");
                    LastName = (dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString().Trim() : "NAME MISSING");
                    phyState = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString().Trim() : "STATE MISSING");
                    strSpecialty = (dt.Rows[0]["NDB_Specialty"] != DBNull.Value ? dt.Rows[0]["NDB_Specialty"].ToString().Trim() : "SPECIALTY MISSING");
                    strMarketName = (dt.Rows[0]["MKT_RLLP_NM"] != DBNull.Value ? dt.Rows[0]["MKT_RLLP_NM"].ToString().Trim() : "SPECIALTY MISSING");

                    //strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState + "_" + strMonthYear;
                    strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState;
                    strFinalReportFileName = strFinalReportFileName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "") + "_Details_" + strMonthYear;

                    //GET MPIN INFO END


                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RP_RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_PH3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
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
                        intRowAdd = 0;
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl_Ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";
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
                        intRowAdd = 0;

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":I" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":I" + (dt.Rows.Count + 14), strSheetname);



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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph3 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
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
                        intRowAdd = 0;

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    strSheetname = "Out-of-network_lab_and_path";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LPOON_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
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
                        intRowAdd = 0;

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    strSheetname = "Level_4_and_5_E&M_visits";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl_Ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
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
                        intRowAdd = 0;
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Level_4_and_5_E&M_visits SHEET END///////////////
                    //Level_4_and_5_E&M_visits SHEET END///////////////
                    //Level_4_and_5_E&M_visits SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Level_4_and_5_E&M_consults SHEET START///////////////
                    //Level_4_and_5_E&M_consults SHEET START///////////////
                    //Level_4_and_5_E&M_consults SHEET START///////////////
                    strSheetname = "Level_4_and_5_E&M_consults";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45cons_dtl_Ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
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
                        intRowAdd = 0;
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Level_4_and_5_E&M_consults SHEET END///////////////
                    //Level_4_and_5_E&M_consults SHEET END///////////////
                    //Level_4_and_5_E&M_consults SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_24 SHEET START///////////////
                    //Modifier_24 SHEET START///////////////
                    //Modifier_24 SHEET START///////////////
                    strSheetname = "Modifier_24";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod24_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
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
                        intRowAdd = 0;
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Modifier_24 SHEET END///////////////
                    //Modifier_24 SHEET END///////////////
                    //Modifier_24 SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifer_25 SHEET START///////////////
                    //Modifer_25 SHEET START///////////////
                    //Modifer_25 SHEET START///////////////
                    strSheetname = "Modifer_25";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category, 25 as modifier from dbo.VW_PBP_Mod25_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
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
                        intRowAdd = 0;
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Modifer_25 SHEET END///////////////
                    //Modifer_25 SHEET END///////////////
                    //Modifer_25 SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_50 SHEET START///////////////
                    //Modifier_50 SHEET START///////////////
                    //Modifier_50 SHEET START///////////////
                    strSheetname = "Modifier_50";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod50_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Modifier_50 SHEET END///////////////
                    //Modifier_50 SHEET END///////////////
                    //Modifier_50 SHEET END///////////////






                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_51 SHEET START///////////////
                    //Modifier_51 SHEET START///////////////
                    //Modifier_51 SHEET START///////////////
                    strSheetname = "Modifier_51";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod51_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Modifier_51 SHEET END///////////////
                    //Modifier_51 SHEET END///////////////
                    //Modifier_51 SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_58 SHEET START///////////////
                    //Modifier_58 SHEET START///////////////
                    //Modifier_58 SHEET START///////////////
                    strSheetname = "Modifier_58";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod58_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Modifier_58 SHEET END///////////////
                    //Modifier_58 SHEET END///////////////
                    //Modifier_58 SHEET END///////////////






                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_59 SHEET START///////////////
                    //Modifier_59 SHEET START///////////////
                    //Modifier_59 SHEET START///////////////
                    strSheetname = "Modifier_59";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod59_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Modifier_59 SHEET END///////////////
                    //Modifier_59 SHEET END///////////////
                    //Modifier_59 SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_76 SHEET START///////////////
                    //Modifier_76 SHEET START///////////////
                    //Modifier_76 SHEET START///////////////
                    strSheetname = "Modifier_76";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod76_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Modifier_76 SHEET END///////////////
                    //Modifier_76 SHEET END///////////////
                    //Modifier_76 SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Radiology SHEET START///////////////
                    //Radiology SHEET START///////////////
                    //Radiology SHEET START///////////////
                    strSheetname = "Radiology";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_Rad_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Radiology SHEET END///////////////
                    //Radiology SHEET END///////////////
                    //Radiology SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    strSheetname = "Advanced_Imaging";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_AdvIm_dtl_ph3 where attr_mpin = " + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Tier_3_Prescribing SHEET START///////////////
                    //Tier_3_Prescribing SHEET START///////////////
                    //Tier_3_Prescribing SHEET START///////////////
                    strSheetname = "Tier_3_Prescribing";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT,phys_type from dbo.VW_PBP_Rx_PCP_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    strSheetname = "Specialty_Specific_Diagnostics";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from dbo.VW_PBP_spscf_dtl_ph3 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Specialty_Specific_Diagnostics SHEET END///////////////
                    //Specialty_Specific_Diagnostics SHEET END///////////////
                    //Specialty_Specific_Diagnostics SHEET END///////////////






                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    strSheetname = "Pre_Cardiac_Cath_Dx_Testing";
                    strSQL = "Select Count(*) FROM VW_PBP_PreCardic_Cath_dtl_Ph3 Where mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);

                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, PRDCT_LVL_1_NM, PEG_ANCH_CATGY_DESC, PEG_ANCH_PROC_DT, ETG_Base_Class_Description from VW_PBP_PreCardic_Cath_dtl_Ph3 where mpin=" + strMPIN + " and Target_Count>=3 order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_PROC_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");
                            intRowAdd = 0;
                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                            MSExcel.addFocusToCell(strSheetname, "A1");

                            
                        }

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Pre_Cardiac_Cath_Dx_Testing SHEET END///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET END///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Neg_Card_Catheterization SHEET START///////////////
                    //Neg_Card_Catheterization SHEET START///////////////
                    //Neg_Card_Catheterization SHEET START///////////////
                    strSheetname = "Neg_Card_Catheterization";
                    strSQL = "Select Count(*) FROM VW_PBP_Neg_Cath_dtl_Ph3 Where mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);

                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, PRDCT_LVL_1_NM, PEG_ANCH_CATGY_DESC, PEG_ANCH_PROC_DT, ETG_Base_Class_Description from VW_PBP_Neg_Cath_dtl_Ph3 where mpin=" + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_PROC_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            intRowAdd = 1;
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);
                            intRowAdd = 0;
                            MSExcel.addFocusToCell(strSheetname, "A1");

                           
                        }

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Neg_Card_Catheterization SHEET END///////////////
                    //Neg_Card_Catheterization SHEET END///////////////
                    //Neg_Card_Catheterization SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Cardiac_Stent SHEET START///////////////
                    //Cardiac_Stent SHEET START///////////////
                    //Cardiac_Stent SHEET START///////////////
                    strSheetname = "Cardiac_Stent";
                    strSQL= "Select Count(*) FROM VW_PBP_Stent_dtl_Ph3 Where mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);

                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, PRDCT_LVL_1_NM, PEG_ANCH_CATGY_DESC, PEG_ANCH_PROC_DT, ETG_Base_Class_Description from VW_PBP_Stent_dtl_Ph3 where mpin=" + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_PROC_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            intRowAdd = 1;
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");
                            intRowAdd = 0;
                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                            MSExcel.addFocusToCell(strSheetname, "A1");

                            
                        }
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Cardiac_Stent SHEET END///////////////
                    //Cardiac_Stent SHEET END///////////////
                    //Cardiac_Stent SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Post-op_Complications SHEET START///////////////
                    //Post-op_Complications SHEET START///////////////
                    //Post-op_Complications SHEET START///////////////
                    strSheetname = "Post-op_Complications";
                    strSQL = "Select Count(*) FROM VW_PBP_Compl30_dtl_Ph3 Where PROV_MPIN=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, SEVERITY, PRODUCT, SRVC_LOC, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_Compl30_dtl_Ph3 where PROV_MPIN=" + strMPIN + " AND CMPLCTN_IND=1 order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                            MSExcel.addFocusToCell(strSheetname, "A1");

                            intRowAdd = 0;


                        }
                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Post-op_Complications SHEET END///////////////
                    //Post-op_Complications SHEET END///////////////
                    //Post-op_Complications SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Unplanned_Hospital_Admits SHEET START///////////////
                    //Unplanned_Hospital_Admits SHEET START///////////////
                    //Unplanned_Hospital_Admits SHEET START///////////////
                    strSheetname = "Unplanned_Hospital_Admits";
                    strSQL = "Select Count(*) FROM VW_PBP_Admit30_dtl_Ph3 Where MPIN=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);



                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, AprDrg_svrty, [POPULATION], surg_loc, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_Admit30_dtl_Ph3 where MPIN=" + strMPIN + " AND post_admit_ind= 'Y' order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                            MSExcel.addFocusToCell(strSheetname, "A1");
                            intRowAdd = 0;

                        }

                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Unplanned_Hospital_Admits SHEET END///////////////
                    //Unplanned_Hospital_Admits SHEET END///////////////
                    //Unplanned_Hospital_Admits SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Unplanned_ED_Visits SHEET START///////////////
                    //Unplanned_ED_Visits SHEET START///////////////
                    //Unplanned_ED_Visits SHEET START///////////////
                    strSheetname = "Unplanned_ED_Visits";
                    strSQL = "Select Count(*) FROM VW_PBP_ED30_dtl_Ph3 Where MPIN=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);



                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, AprDrg_svrty, [POPULATION], surg_loc, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_ED30_dtl_Ph3 where MPIN=" + strMPIN + " AND post_ed_ind= 'Y' order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);
                            intRowAdd = 0;
                            MSExcel.addFocusToCell(strSheetname, "A1");

                           
                        }

                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Unplanned_ED_Visits SHEET END///////////////
                    //Unplanned_ED_Visits SHEET END///////////////
                    //Unplanned_ED_Visits SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Redo_Rate SHEET START///////////////
                    //Redo_Rate SHEET START///////////////
                    //Redo_Rate SHEET START///////////////
                    strSheetname = "Redo_Rate";
                    strSQL = "Select Count(*) FROM VW_PBP_Redo_dtl_Ph3 Where PROV_MPIN=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, SVRTY_LVL_CD, Product, SITE_CATGY_CD, PEG_ANCH_CATGY_DESC, PEG_ANCH_PROC_DT from VW_PBP_Redo_dtl_Ph3 where PROV_MPIN=" + strMPIN + " AND CMPLNT_IND='N' order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_PROC_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);
                            intRowAdd = 0;
                            MSExcel.addFocusToCell(strSheetname, "A1");

                        }

                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Redo_Rate SHEET END///////////////
                    //Redo_Rate SHEET END///////////////
                    //Redo_Rate SHEET END///////////////







                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Spinal_Fusion SHEET START///////////////
                    //Spinal_Fusion SHEET START///////////////
                    //Spinal_Fusion SHEET START///////////////
                    strSheetname = "Spinal_Fusion";
                    strSQL = "Select COUNT(*) FROM VW_PBP_Spine_dtl_Ph3	Where MPIN =" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        //strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, SVRTY_LVL_CD, Product, SITE_CATGY_CD, PEG_ANCH_CATGY_DESC, PEG_ANCH_PROC_DT from VW_PBP_Redo_dtl_Ph3 where PROV_MPIN=" + strMPIN + " AND CMPLNT_IND='N' order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_PROC_DT";

                        strSQL = "select MBR_FST_NM, MBR_LST_NM, BTH_DT, PRDCT_LVL_1_NM as 'Product', 'FUSION; LUMBAR BACK' as 'Procedure', Procedure_Date, Final_dx as 'Diagnosis Category' from VW_PBP_Spine_dtl_Ph3 where MPIN=" + strMPIN + " AND PEG_ANCH_CATGY='LUMFUS' order by MBR_LST_NM, MBR_FST_NM, Procedure_Date";


                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");

                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);
                            intRowAdd = 0;
                            MSExcel.addFocusToCell(strSheetname, "A1");

                            
                        }

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }


                    //Spinal_Fusion SHEET END///////////////
                    //Spinal_Fusion SHEET END///////////////
                    //Spinal_Fusion SHEET END///////////////






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


                Console.WriteLine("Closing Microsoft Excel Instance...");
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();

            }





        }
    }
}
