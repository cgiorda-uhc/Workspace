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

namespace PR_OB_AdHoc_Details
{
    class PR_OB_AdHoc_Details
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




            if(args.Count() == 2)
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
 
                    strSQL = "SELECT FirstName, LastName, NDB_Specialty, b.[State], a.MKT_RLLP_NM FROM dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND a.MPIN = " + strMPIN;


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
                    strSQL = "Select Count(*) FROM VW_PBP_ER_dtl_PH2 where attr_mpin = " + strMPIN;

                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_PH2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_PH2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            

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
                        }

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

                    strSQL = "Select Count(*) FROM VW_PBP_IP_dtl_Ph2 where attr_mpin = " + strMPIN;

                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);

                        if (dt.Rows.Count <= 0)
                        {
                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
                            

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "");


                            MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                            MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                            MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                            MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                            MSExcel.populateTable(dt, strSheetname, 15, 'A');
                            MSExcel.addBorders("A15" + ":I" + (dt.Rows.Count + 14), strSheetname);
                            MSExcel.AutoFitRange("A14" + ":I" + (dt.Rows.Count + 14), strSheetname);



                            MSExcel.addFocusToCell(strSheetname, "A1");
                            intRowAdd = 0;



                        }

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph2 where attr_mpin = " + strMPIN ;
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph2 where attr_mpin = " + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        intRowAdd = 0;
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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LPOON_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LPOON_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LPOON_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        intRowAdd = 0;
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

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Level_4_and_5_E&M_coding SHEET START///////////////
                    //Level_4_and_5_E&M_coding SHEET START///////////////
                    //Level_4_and_5_E&M_coding SHEET START///////////////
                    strSheetname = "Level_4_and_5_E&M_coding";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, PROC_CD";
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
                    //Level_4_and_5_E&M_coding SHEET END///////////////
                    //Level_4_and_5_E&M_coding SHEET END///////////////
                    //Level_4_and_5_E&M_coding SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifer_25 SHEET START///////////////
                    //Modifer_25 SHEET START///////////////
                    //Modifer_25 SHEET START///////////////
                    strSheetname = "Modifer_25";
                    strSQL = "Select Count(*) FROM VW_PBP_Mod25_dtl_ph2 where attr_mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, AHRQ_Diagnosis_Category";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);

                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {

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
                           


                        }

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Modifer_25 SHEET END///////////////
                    //Modifer_25 SHEET END///////////////
                    //Modifer_25 SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifier_59 SHEET START///////////////
                    //Modifier_59 SHEET START///////////////
                    //Modifier_59 SHEET START///////////////
                    strSheetname = "Modifier_59";
                    strSQL = "Select Count(*) FROM VW_PBP_Mod59_dtl_ph2 where attr_mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod59_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod59_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, AHRQ_Diagnosis_Category";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {

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
                            

                        }

                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Modifier_59 SHEET END///////////////
                    //Modifier_59 SHEET END///////////////
                    //Modifier_59 SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Radiology SHEET START///////////////
                    //Radiology SHEET START///////////////
                    //Radiology SHEET START///////////////
                    strSheetname = "Radiology";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_Rad_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_Rad_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, Rad_Gen_Category";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        intRowAdd = 0;
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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_AdvIm_dtl_ph2 where attr_mpin = " + strMPIN ;
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_AdvIm_dtl_ph2 where attr_mpin = " + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, Rad_Gen_Category";
                    dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        intRowAdd = 0;
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
                    strSQL = "Select Count(*) FROM VW_PBP_Rx_PCP_dtl_ph2 where attr_mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                        dt = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);
                        if (dt.Rows.Count <= 0)
                        {

                            Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                            intRowAdd = 1;
                        }
                        else
                        {
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

                        }

                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Cesarean SHEET START///////////////
                    //Cesarean SHEET START///////////////
                    //Cesarean SHEET START///////////////
                    strSheetname = "Cesarean";
                    strSQL = "Select Count(*) FROM VW_PBP_CSection_dtl_ph2 Where attr_mpin=" + strMPIN ;
                    intRowCount = int.Parse(DBConnection64.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if(intRowCount == 0)
                    {
                        Console.WriteLine(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        MSExcel.addValueToCell(strSheetname, "B5", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        alActiveSheets.Add(strSheetname);


                        //strSQL = "SELECT MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AGECAT, PRODUCT, DELIVERY_TYPE, DELIVERY_DT FROM VW_PBP_CSection_dtl_ph2 WHERE attr_mpin=" + strMPIN + "  ORDER BY MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";
                        strSQL = "select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AGECAT, PRODUCT, DELIVERY_TYPE, DELIVERY_DT from VW_PBP_CSection_dtl_ph2 where DELIVERY_TYPE not like '%VAG%' and attr_mpin=" + strMPIN + " order by attr_mpin, MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";
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


                    //Cesarean SHEET END///////////////
                    //Cesarean SHEET END///////////////
                    //Cesarean SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Vaginal_Hysterectomy SHEET START///////////////
                    //Vaginal_Hysterectomy SHEET START///////////////
                    //Vaginal_Hysterectomy SHEET START///////////////

                    strSheetname = "Vaginal_Hysterectomy";
                    strSQL = "Select Count(*) FROM VW_PBP_VH_dtl_ph2 Where attr_mpin= " + strMPIN;
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


                        //strSQL = "select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, PRODUCT, PLACE_OF_SERVICE, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_VH_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                        strSQL = "select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, PRODUCT, PLACE_OF_SERVICE, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_VH_dtl_ph2 where attr_mpin=" + strMPIN + " and PEG_ANCH_CATGY_DESC NOT LIKE '%VAGINAL%' order by attr_mpin, MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
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
                    //Vaginal_Hysterectomy SHEET END///////////////
                    //Vaginal_Hysterectomy SHEET END///////////////
                    //Vaginal_Hysterectomy SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Post-op_Complications SHEET START///////////////
                    //Post-op_Complications SHEET START///////////////
                    //Post-op_Complications SHEET START///////////////

                    strSheetname = "Post-op_Complications";
                    strSQL = "Select Count(*) FROM VW_PBP_OBCompl_dtl_ph2 Where attr_mpin=" + strMPIN;
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

                        //strSQL = "select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, PRODUCT, SRVC_LOC, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_OBCompl_dtl_ph2 where attr_mpin=" + strMPIN + " order by attr_mpin, MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                        strSQL = "select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, PRODUCT, SRVC_LOC, PEG_ANCH_CATGY_DESC, PEG_ANCH_DT from VW_PBP_OBCompl_dtl_ph2 where CMPLCTN_IND>0 and attr_mpin=" + strMPIN + " order by attr_mpin, MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
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


                    //Post-op_Complications SHEET END///////////////
                    //Post-op_Complications SHEET END///////////////
                    //Post-op_Complications SHEET END///////////////
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());




                    //CLOSE EXCEL WB
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //COMPLETED MESSAGE
                    Console.WriteLine(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "");
                    Console.WriteLine("----------------------------------------------------------------------------");

                    intCnt++;

                }

                Console.WriteLine("Process Completed");

                if(!blIsProcess)
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
