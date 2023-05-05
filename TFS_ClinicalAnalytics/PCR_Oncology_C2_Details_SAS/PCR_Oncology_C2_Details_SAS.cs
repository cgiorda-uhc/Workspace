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

namespace PCR_Oncology_C2_Details_SAS
{
    class PCR_Oncology_C2_Details_SAS
    {




        static void Main(string[] args)
        {

            bool blIsProcess = false;


            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strExcelTemplate = ConfigurationManager.AppSettings["ExcelTemplate"];
            string strReportsPath = ConfigurationManager.AppSettings["ReportsPath"];



            string strTinCSV = ConfigurationManager.AppSettings["MpinCSV"];
            string[] strTinArr = null;
            if (!String.IsNullOrEmpty(strTinCSV))
            {
                strTinArr = strTinCSV.Split(',');
            }


            if (args.Count() == 2)
            {
                strTinArr = args[0].ToString().Split(',');
                strReportsPath = args[1].ToString();
                blIsProcess = true;
                //Console.WriteLine(args[0]);
                //Console.WriteLine(args[1]);
                //Console.ReadLine();
            }




            if (strTinArr == null)
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
            string strTIN = null;
            string strPracticeName = null;
            string strState = null;
            string strSpecialty = null;
            string strMarketName = null;

            int intTINTotal = 0;
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

                intTINTotal = strTinArr.Length;


                Console.WriteLine("Connecting to SAS Server...");
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


                foreach (string s in strTinArr)
                {
                    alActiveSheets = new ArrayList();
                    alActiveRanges = new ArrayList();

                    intRowAdd = 0;
                    strTIN = s.Trim();


                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();




                    //strSQL = "select FirstName, LastName,a.Spec_display as NDB_Specialty,b.[State],a.MKT_RLLP_NM from dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph33 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = " + strMPIN;




                    strSQL = "SELECT d.TaxID, d.CorpOwnerName as Practice_Name, a.Street, a.City, a.State, a.ZipCd FROM Onc.Onc_TIN_demog as d inner join Onc.ONC_TIN_ADDR as a on d.TaxID=a.TaxID where Exclude is null and d.taxid=" + strTIN +";";


                    dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        Console.WriteLine("No records found for TIN: " + strTIN);
                        continue;
                    }





                    strPracticeName = (dt.Rows[0]["Practice_Name"] != DBNull.Value ? dt.Rows[0]["Practice_Name"].ToString().Trim() : "NAME MISSING");
                    strState = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString().Trim() : "NAME MISSING");



                    //strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState + "_" + strMonthYear;
                    strFinalReportFileName = strTIN + "_" + strPracticeName + "_" + strState;
                    strFinalReportFileName = strFinalReportFileName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "") + "_Details_" + strMonthYear;





                    //Hospice SHEET START///////////////
                    //Hospice SHEET START///////////////
                    //Hospice SHEET START///////////////
                    strSheetname = "Hospice";
                    strTopRange = "F";

                    //AND MBR_LST_NM IS NOT NULL
                    //strSQL = "select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, PRDCT_LVL_1_NM, HOSPICE_ADMIT_DT, LENGTH_OF_STAY from onc.hospice_indv_dtl as d inner join onc.individuals as l on l.indv_sys_id=d.indv_sys_id where taxid=" + strTIN + ";";
                    strSQL = "select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PRDCT_LVL_1_NM, PUT(HOSPICE_ADMIT_DT,MMDDYY10.) as HOSPICE_ADMIT_DT, LENGTH_OF_STAY from onc.hospice_indv_dtl as d inner join onc.individuals as l on l.indv_sys_id=d.indv_sys_id where MBR_LST_NM is not null and taxid=" + strTIN + " AND LENGTH_OF_STAY <= 3 order by MBR_LST_NM;";

                    dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intTINTotal + ": NO " + strSheetname + " records for MPIN: " + strTIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intTINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strTIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", strPracticeName);
                        MSExcel.addValueToCell(strSheetname, "B6", strTIN);
                        //MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        //MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname, true, false);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        strTopRange = "G";
                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Hospice SHEET END///////////////
                    //Hospice SHEET END///////////////
                    //Hospice SHEET END///////////////



                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    strSheetname = "Hospital_Admits";
                    strTopRange = "H";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(ADMIT_DT ,MMDDYY10.) as ADMIT_DT,PUT(DSCHRG_DT ,MMDDYY10.) as DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph32 where attr_mpin=" + strMPIN;
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(ADMIT_DT ,MMDDYY10.) as ADMIT_DT,PUT(DSCHRG_DT ,MMDDYY10.) as DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph33 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    // strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,PUT(ADMIT_DT ,MMDDYY10.) as ADMIT_DT,PUT(DSCHRG_DT ,MMDDYY10.) as DSCHRG_DT,STAT_DAY, APR_DRG from ph34.V_IP_admdt_dt where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM;";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from ph34.V_IP_admdt_dt where attr_mpin=" + strTIN + " order by mbr_lst_nm,MBR_FST_NM, ADMIT_DT;";
                    strSQL = "select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PRDCT_LVL_1_NM, CANCER_TYPE, PUT(ADMIT30_DT_allcause ,MMDDYY10.) as ADMIT30_DT_allcause, 'Yes' as All_adm, case when Admit30_SpclCon_IND=1 then 'Yes' end as Chem_adm from onc.NUMERATOR_PATIENTS as d inner join onc.individuals as l on l.indv_sys_id=d.indv_sys_id where ADMIT30_DT_allcause is not null and MBR_LST_NM is not null and tin=" + strTIN + " order by MBR_LST_NM, ADMIT30_DT_allcause;";

                    dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intTINTotal + ": NO " + strSheetname + " records for MPIN: " + strTIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intTINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strTIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", strPracticeName);
                        MSExcel.addValueToCell(strSheetname, "B6", strTIN);
                        //MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        //MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname,  true, false);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////



                    //Emergency Department Visits SHEET START///////////////
                    //Emergency Department Visits SHEET START///////////////
                    //Emergency Department Visits SHEET START///////////////
                    strSheetname = "Emergency Department Visits";
                    strTopRange = "H";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph33 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from ph34.V_LP_dt where attr_mpin =" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM;";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from ph34.V_LP_dt where attr_mpin =" + strTIN + " order by MBR_LST_NM,MBR_FST_NM;";
                    strSQL = "select MBR_FST_NM, MBR_LST_NM, PUT(INDV_BTH_DT ,MMDDYY10.) as INDV_BTH_DT, PRDCT_LVL_1_NM, CANCER_TYPE, PUT(ER30_DT_allcause ,MMDDYY10.) as ER30_DT_allcause, 'Yes' as All_ed, case when ER30_SpclCon_IND=1 then 'Yes' end as Chem_ed from onc.NUMERATOR_PATIENTS as d inner join onc.individuals as l on l.indv_sys_id=d.indv_sys_id where ER30_DT_allcause is not null and MBR_LST_NM is not null and tin=" + strTIN + " order by MBR_LST_NM,ER30_DT_allcause;";

                    dt = DBConnection64.getOleDbDataTableGlobal(IR_SAS_Connect.strSASConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        Console.WriteLine(intCnt + " of " + intTINTotal + ": NO " + strSheetname + " records for MPIN: " + strTIN + "");
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        Console.WriteLine(intCnt + " of " + intTINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strTIN + "");

                        MSExcel.addValueToCell(strSheetname, "B5", strPracticeName);
                        MSExcel.addValueToCell(strSheetname, "B6", strTIN);
                        //MSExcel.addValueToCell(strSheetname, "B7", strSpecialty);
                        //MSExcel.addValueToCell(strSheetname, "B8", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":" + strTopRange + (dt.Rows.Count + 14), strSheetname, true, false);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add(strTopRange + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Emergency Department Visits SHEET END///////////////
                    //Emergency Department Visits SHEET END///////////////
                    //Emergency Department Visits SHEET END///////////////





                   // alActiveSheets.Add("Appendix");
                  //  alActiveRanges.Add("B11");
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());




                    //CLOSE EXCEL WB
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //COMPLETED MESSAGE
                    Console.WriteLine(intCnt + " of " + intTINTotal + ": Completed File for MPIN " + strTIN + "");
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
                    DBConnection64.getOleDbDataTableGlobalClose();
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
