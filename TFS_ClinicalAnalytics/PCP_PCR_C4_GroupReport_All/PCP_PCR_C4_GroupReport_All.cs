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

namespace PCP_PCR_C4_GroupReport_All
{
    class PCP_PCR_C4_GroupReport_All
    {
        static void Main(string[] args)
        {


            bool blIsProcess = false;

            string strConnectionString = ConfigurationManager.AppSettings["ILUCA_Database"];
            string strExcelTemplate = ConfigurationManager.AppSettings["ExcelTemplate"];
            string strReportsPath = ConfigurationManager.AppSettings["ReportsPath"];


            ArrayList alActiveSheets = new ArrayList();
            ArrayList alActiveRanges = new ArrayList();

            DataTable dt = null;
            string strSheetname = null;

            string strMonthYear = DateTime.Now.Month + "_" + DateTime.Now.Year;
            string strFinalReportFileName = "";

            string strSQL = null;
            string strMPIN = null;

            string phyName = null;
            string LastName = null;


            int intMPINTotal = 0;
            Int16 intCnt = 1;


            int intRowCount = 0;
            int intRowAdd = 0;

            try
            {
                MSExcel.populateExcelParameters(false, true, strReportsPath, strExcelTemplate);
                MSExcel.openExcelApp();

                //MSExcel.strReportsPath = strReportsPath;



                strSQL = "select distinct TOP 1 h.taxid, h.PTIGroupID ,h.PTIGroupName as Practice_Name,Folder_Name from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_spec_handl_Ph14 as h on h.mpin=a.mpin where exclude=11";


                DataTable dtMain = DBConnection64.getMSSQLDataTable(strConnectionString, strSQL);
                intMPINTotal = dtMain.Rows.Count;
                Console.WriteLine("Gathering targeted physicians...");
                foreach (DataRow dr in dtMain.Rows)//MAIN LOOP START
                {

                    alActiveSheets = new ArrayList();

                    alActiveRanges = new ArrayList();
                    intRowAdd = 0;

                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();



                    phyName = dr["Practice_Name"].ToString().Trim();
                    strMPIN = dr["PTIGroupID"].ToString().Trim();


                    //strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState + "_" + strMonthYear;

                    strFinalReportFileName = strFinalReportFileName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "") + "_Details_" + strMonthYear;

                    //GET MPIN INFO END


                    string strFolderNameTmp = (dr["Folder_Name"] != DBNull.Value ? dr["Folder_Name"].ToString().Trim() + "\\" : "");

                    string strFolderName = "";

                    string strBulkPath = "";


                    strFolderName = strFolderNameTmp;

                    MSExcel.strReportsPath = strReportsPath.Replace("{$folderName}", strFolderName.Replace(",", ""));


                    strFinalReportFileName = strMPIN + "_" + phyName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "").Replace("&", "_") + "_GroupReport_" + strMonthYear;




                    //All_meas_all_physicians SHEET START///////////////
                    //All_meas_all_physicians SHEET START///////////////
                    //All_meas_all_physicians SHEET START///////////////
                    strSheetname = "All_meas_all_physicians";

                    strSQL = "select MPIN,Phys_name,Spec_display,act_display, expected_display, var_display,signif,Favorable, Measure_desc,Unit_Measure from ( select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable ,Measure_desc,Unit_Measure,sort_id, d.PTIGroupID from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id not in(14,15) UNION select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,signif_g ,Measure_desc,Unit_Measure,sort_id, d.PTIGroupID from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_px_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id in(38,51,55)) as t WHERE t.PTIGroupID =" + strMPIN + " order by sort_Id,Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 9, 'A');
                        MSExcel.addBorders("A9" + ":J" + (dt.Rows.Count + 8), strSheetname);
                        MSExcel.AutoFitRange("D8" + ":J" + (dt.Rows.Count + 8), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add("J" + (dt.Rows.Count + 8 + intRowAdd));
                    }
                    //All_meas_all_physicians SHEET END///////////////
                    //All_meas_all_physicians SHEET END///////////////
                    //All_meas_all_physicians SHEET END///////////////





                    //Cost_per_Patient SHEET START///////////////
                    //Cost_per_PatientSHEET START///////////////
                    //Cost_per_Patient SHEET START///////////////
                    strSheetname = "Cost_per_Patient";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=50 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);

                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //Cost_per_Patient SHEET END///////////////
                    //Cost_per_Patient SHEET END///////////////
                    //Cost_per_Patient SHEET END///////////////






                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=1 and d.PTIGroupID=" + strMPIN + " order by Phys_name";

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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;

                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    strSheetname = "Hospital_Admissions";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,APR_DRG,ADMIT_DT";

                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=2 and d.PTIGroupID=" + strMPIN + " order by Phys_name";



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


                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        intRowAdd = 0;

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));

                    }
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////
                    //Hospital_Admissions SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //LOS SHEET START///////////////
                    //LOS SHEET START///////////////
                    //LOS SHEET START///////////////
                    strSheetname = "LOS";

                    // strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=3 and d.PTIGroupID=" + strMPIN + "order by Phys_name";

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


                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //LOS SHEET END///////////////
                    //LOS SHEET END///////////////
                    //LOS SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Lab_and_Path SHEET START///////////////
                    //Lab_and_Path SHEET START///////////////
                    //Lab_and_Path SHEET START///////////////
                    strSheetname = "Lab_and_Path";
                    //strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph12 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=4 and d.PTIGroupID=" + strMPIN + "  order by Phys_name";

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


                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    strSheetname = "Out-of-network_lab_and_path";

                    //strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph14 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=16 and d.PTIGroupID=" + strMPIN + " order by Phys_name";

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


                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);



                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////
                    //Out-of-network_lab_and_path SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //////////////////////////////////START 2018.////////////////////////////////////////////////////////////////////




                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    //LLevel_4_and_5_E&M_visits SHEET START///////////////
                    //Level_4_and_5_E&M_visits SHEET START///////////////
                    strSheetname = "Level_4_and_5_E&M_visits";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=5 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //Level_4_and_5_E&M_visits SHEET END///////////////
                    //Level_4_and_5_E&M_visits SHEET END///////////////
                    //Level_4_and_5_E&M_visits SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Modifiers SHEET START///////////////
                    //Modifiers SHEET START///////////////
                    //Modifiers SHEET START///////////////
                    strSheetname = "Modifiers";


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";

                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=37 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);

                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));
                    }
                    //Modifiers SHEET END///////////////
                    //Modifiers SHEET END///////////////
                    //Modifiers SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network SHEET START///////////////
                    //Out-of-network SHEET START///////////////
                    //Out-of-network SHEET START///////////////
                    strSheetname = "Out-of-network";


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,PROV_TYP_NM,SPEC_TYP_NM from dbo.VW_PBP_OON_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=9 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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


                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        intRowAdd = 0;
                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));


                    }
                    //Out-of-network SHEET END///////////////
                    //Out-of-network SHEET END///////////////
                    //Out-of-network SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Specialty_Physician_visits SHEET START///////////////
                    //Specialty_Physician_visits SHEET START///////////////
                    //Specialty_Physician_visits SHEET START///////////////
                    strSheetname = "Specialty_Physician_visits";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=10 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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


                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);

                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        intRowAdd = 0;
                        MSExcel.addFocusToCell(strSheetname, "A1");

                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));


                    }
                    //Specialty_Physician_visits SHEET END///////////////
                    //Specialty_Physician_visits SHEET END///////////////
                    //Specialty_Physician_visits SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    strSheetname = "Advanced_Imaging";

                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=17 and d.PTIGroupID=" + strMPIN + " order by Phys_name";

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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));


                    }
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////
                    //Advanced_Imaging SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Non-Advanced_Imaging SHEET START///////////////
                    //Non-Advanced_Imaging SHEET START///////////////
                    //Non-Advanced_Imaging SHEET START///////////////
                    strSheetname = "Non_Advanced_Imaging";

                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=36 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));


                    }
                    //Non-Advanced_Imaging SHEET END///////////////
                    //Non-Advanced_Imaging SHEET END///////////////
                    //Non-Advanced_Imaging SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Non_PD_Specialist_Visit_Rate SHEET START///////////////
                    //Non_PD_Specialist_Visit_Rate SHEET START///////////////
                    //Non_PD_Specialist_Visit_Rate SHEET START///////////////
                    strSheetname = "Non_PD_Specialist_Visit_Rate";

                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,Favorable  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=8 and d.PTIGroupID=" + strMPIN + " order by Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));


                    }
                    //Non_PD_Specialist_Visit_Rate SHEET END///////////////
                    //Non_PD_Specialist_Visit_Rate SHEET END///////////////
                    //Non_PD_Specialist_Visit_Rate SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //High_Opioid_Prescribing_Rate SHEET START///////////////
                    //High_Opioid_Prescribing_Rate SHEET START///////////////
                    //High_Opioid_Prescribing_Rate SHEET START///////////////

                    strSheetname = "High_Opioid_Prescribing_Rate";

                    // strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph14 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    strSQL = "select a.MPIN, P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,signif_g  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_px_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=38 and d.PTIGroupID=" + strMPIN + " order by Phys_name";

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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));

                    }
                    //High_Opioid_Prescribing_Rate SHEET END///////////////
                    //High_Opioid_Prescribing_Rate SHEET END///////////////
                    //High_Opioid_Prescribing_Rate SHEET END///////////////



                    //Antibiotic_Utilization SHEET START///////////////
                    //Antibiotic_Utilization SHEET START///////////////
                    //Antibiotic_Utilization SHEET START///////////////

                    strSheetname = "Antibiotic_Utilization";

                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_ABX_dtl_Ph14 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,signif_g  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_px_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=51 and d.PTIGroupID=" + strMPIN + " order by Phys_name";

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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));

                    }
                    //Antibiotic_Utilization SHEET END///////////////
                    //Antibiotic_Utilization SHEET END///////////////
                    //Antibiotic_Utilization SHEET END///////////////




                    //Medication_Adherence SHEET START///////////////
                    //Medication_Adherence SHEET START///////////////
                    //Medication_Adherence SHEET START///////////////

                    strSheetname = "Medication_Adherence";


                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_MedAdherence_dtl_Ph14 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";


                    strSQL = "select a.MPIN,P_FirstName+' '+P_LastName as Phys_name, dbo.ProperCase(a.Spec_display) as Spec_display,act_display, expected_display, var_display,signif,signif_g  from dbo.PBP_Outl_ph14 as a inner join dbo.PBP_outl_demogr_ph14 as d on a.MPIN=d.MPIN inner join [dbo].[PBP_Profile_px_Ph14] as p on p.mpin=a.mpin where exclude=11 and measure_id=55 and d.PTIGroupID=" + strMPIN + "  order by Phys_name";


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

                        MSExcel.addValueToCell(strSheetname, "B5", phyName);
                        MSExcel.addValueToCell(strSheetname, "B6", strMPIN);


                        MSExcel.populateTable(dt, strSheetname, 11, 'A');
                        MSExcel.addBorders("A11" + ":H" + (dt.Rows.Count + 10), strSheetname);
                        MSExcel.AutoFitRange("D10" + ":H" + (dt.Rows.Count + 10), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 10 + intRowAdd));

                    }
                    //Medication_Adherence SHEET END///////////////
                    //Medication_Adherence SHEET END///////////////
                    //Medication_Adherence SHEET END///////////////







                    //NO APPENDIX....SO FAR
                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");



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
