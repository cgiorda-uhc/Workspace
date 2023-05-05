using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    static class MemberDetails
    {
        private static string _strTemplatePath ;
        private static string _strReportPath;
        private static string[] _strMpinArr;



        public static string strMPINGLOBAL = null;
        public static string strFilePathGLOBAL = null;
        public static string strProjectNameGLOBAL = null;
        public static string strLastNameGLOBAL = null;

        public static string generateMemberDetails(string strProject, string strMpinCSV, ref TextBox txtStatus)
        {


            //ADDED 2018 TO SUPPORT EMAIL AUTOMATION
            strMPINGLOBAL = null;
            strFilePathGLOBAL = null;
            strProjectNameGLOBAL = null;
            strLastNameGLOBAL = null;





            bool blCloneTemplate = true;


            if (blCloneTemplate)
            {
                txtStatus.AppendText("Cloning Excel Template..." + Environment.NewLine);
                
            }


            _strTemplatePath = getTemplatePath(strProject, blCloneTemplate);
            if(_strTemplatePath == null)
                return "No Details Exist for this Project";

            _strReportPath = Environment.ExpandEnvironmentVariables(GlobalObjects.strMemberDetailsReportPath);

            _strMpinArr = getMPINArray(strMpinCSV);
            if (_strMpinArr == null)
                return "No Valid MPINS Supplied";



            object[] arguments = new object[] { txtStatus };

            Type thisType = typeof(MemberDetails);
            MethodInfo theMethod = thisType.GetMethod(strProject.Replace(" ", ""), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (theMethod == null)
                return "No Member Details Available for this Project ";
            else
                theMethod.Invoke(thisType, arguments);

            if(blCloneTemplate)
            {
                if (File.Exists(_strTemplatePath))
                {
                    txtStatus.AppendText("Cleaning Cloned Excel Template..." + Environment.NewLine);
                    

                    //while (true)
                    //{
                       try
                        {
                            File.Delete(_strTemplatePath);
                    //break; // success!
                        }
                        catch(Exception ex)
                        {
                    //        SharedFunctions.killProcess("EXCEL");
                        }
                    //}
                }
            }


            txtStatus.AppendText("Process Completed" + Environment.NewLine);
            

             

            return null;
        }




        private static void PCPCohort4(ref TextBox txtStatus)
        {

            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

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


            int intRowAdd = 0;

            try
            {

                txtStatus.AppendText("Opening Excel Template Instance..." + Environment.NewLine);


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


                    //strSQL = "select FirstName,LastName,Spec_display,o.MKT_RLLP_NM, [State] AS [State]  from dbo.PBP_outl_ph13 as o inner join dbo.PBP_outl_demogr_Ph13 as d on d.mpin=o.mpin where exclude in(0,5) and  o.MPIN = " + strMPIN;
                    //and OE_Allw is not null
                    strSQL = "select FirstName,LastName,Spec_display,o.MKT_RLLP_NM, [State] AS [State] from dbo.PBP_outl_ph14 as o inner join dbo.PBP_outl_demogr_Ph14 as d on d.mpin=o.mpin where exclude in(0,5) and  o.MPIN = " + strMPIN;

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);
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


                    //strSQL = "select distinct E.MBR_FST_NM, E.MBR_LST_NM,E.INDV_BTH_DT, M.RP_RISK_CATGY,M.PRDCT_LVL_1_NM, DOS, case when Sens is null then AHRQ_Diagnosis_Category else 'SENSITIVE HEALTH CONDITION' end as AHRQ_Diagnosis_Category from dbo.PBP_ER_Details_Ph13 E INNER JOIN DBO.PBP_MPIN_CLIENT_Ph13 M ON E.INDV_SYS_ID=M.INDV_SYS_ID AND E.ATTR_MPIN=M.ATTR_MPIN INNER JOIN dbo.PBP_dx as D on E.DIAG_1_CD=D.diag_cd where E.attr_mpin =" + strMPIN + " and OP_EVENT_KEY <>0 AND E.MBR_LST_NM IS NOT NULL group by e.attr_mpin,e.INDV_SYS_ID,E.MBR_FST_NM,E.MBR_LST_NM,E.INDV_BTH_DT,M.RP_RISK_CATGY,AHRQ_Diagnosis_Category, M.PRDCT_LVL_1_NM,DOS,case when Sens is null then AHRQ_Diagnosis_Category else 'SENSITIVE HEALTH CONDITION' end having SUM(DERIV_ALLW_AMT)<>0 ORDER BY MBR_LST_NM ASC, MBR_FST_NM ASC, INDV_BTH_DT ASC, DOS ASC, AHRQ_Diagnosis_Category ASC";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,APR_DRG,ADMIT_DT";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,APR_DRG,ADMIT_DT";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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
                    //strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";



                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {


                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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
                    //strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph13 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph14 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);


                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,PROV_TYP_NM,SPEC_TYP_NM from dbo.VW_PBP_OON_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,PROV_TYP_NM,SPEC_TYP_NM from dbo.VW_PBP_OON_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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

                    //NonPrem_Care_Phys_Util SHEET START///////////////
                    //NonPrem_Care_Phys_Util SHEET START///////////////
                    //NonPrem_Care_Phys_Util SHEET START///////////////
                    strSheetname = "NonPrem_Care_Phys_Util";

                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Not_t1_dtl_PH14 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //NonPrem_Care_Phys_Util SHEET END///////////////
                    //NonPrem_Care_Phys_Util SHEET END///////////////
                    //NonPrem_Care_Phys_Util SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    //Advanced_Imaging SHEET START///////////////
                    strSheetname = "Advanced_Imaging";
                    // strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category,Proc_count from dbo.VW_PBP_AdvIm_dtl_ph12 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";



                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + ""  + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + ""  + Environment.NewLine);

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
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph13 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph14 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + ""  + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + ""  + Environment.NewLine);

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
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph14 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + ""  + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + ""  + Environment.NewLine);


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
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_ABX_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_ABX_dtl_Ph14 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + ""  + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + ""  + Environment.NewLine);


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
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_MedAdherence_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_MedAdherence_dtl_Ph14 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + ""  + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + ""  + Environment.NewLine);


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


                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);



                    string strPDFPath = MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strFilePathGLOBAL == null)
                        strFilePathGLOBAL = strPDFPath;



                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB

                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);

                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);

                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);


                    intCnt++;
                    Application.DoEvents();

                }

            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);

                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);
            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);

                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }

                //txtStatus.AppendText("Detail Reports are Ready" + Environment.NewLine);
                //
            }



        }







        private static void SpecialtiesCohort3(ref TextBox txtStatus)
        {

            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

            ArrayList alActiveSheets = new ArrayList();
            ArrayList alActiveRanges = new ArrayList();

            DataTable dt = null;
            string strSheetname = null;
            string strTopRange = null;

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


            int intRowAdd = 0;

            try
            {
                txtStatus.AppendText("Opening Excel Template Instance..." + Environment.NewLine);


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


                    strSQL = "select FirstName, LastName,a.Spec_display as NDB_Specialty,b.[State],a.MKT_RLLP_NM from dbo.PBP_Outl_Ph33 as a inner join dbo.PBP_outl_demogr_ph33 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph33 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph33 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = " + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);

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



                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strMPINGLOBAL == null)
                        strMPINGLOBAL = strMPIN;
                    if (strProjectNameGLOBAL == null)
                        strProjectNameGLOBAL = "Specialty Cohort 3";
                    if (strLastNameGLOBAL == null)
                        strLastNameGLOBAL = LastName;




                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";
                    strTopRange = "G";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////



                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    strSheetname = "Hospital_Admissions";
                    strTopRange = "I";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph32 where attr_mpin=" + strMPIN;
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph33 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph32 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph33 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph32 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph33 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    // strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM ASC,MBR_FST_NM ASC,DOS ASC";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45C_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45C_dtl33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    // strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Modpx_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Modpx_dtl33 where attr_mpin=" + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph32 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category as Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph33 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    //strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph32 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category as Rad_Gen_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph33 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    // strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from dbo.VW_PBP_sp_specif_dtl_ph32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from dbo.VW_PBP_sp_specif_dtl_ph33 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "F";
                    //strSQL = "SELECT MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AGECAT, DELIVERY_TYPE, DELIVERY_DT FROM VW_PBP_CSection_dtl_ph32 WHERE attr_mpin=" + strMPIN + " AND DELIVERY_TYPE= 'NORMAL C-SECTION' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AGECAT, DELIVERY_TYPE, DELIVERY_DT FROM VW_PBP_CSection_dtl_ph33 Where attr_mpin=" + strMPIN + " AND DELIVERY_TYPE like'%C-Section%' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_PreCardic_Cath_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT,[PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_PreCardic_Cath_dtl_Ph33 Where ATTR_MPIN=" + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Neg_Cath_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Neg_Cath_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "F";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Stent_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Stent_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, POS, PEG_ANCH_CATGY_DESC as 'PROCEDURE', PEG_ANCH_DT as Procedure_Date FROM VW_PBP_Compl30_dtl_Ph32 Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, POS, PEG_ANCH_CATGY_DESC as 'PROCEDURE', PEG_ANCH_DT as Procedure_Date FROM VW_PBP_Compl30_dtl_Ph33 Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Admit30_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and post_admit_ind='Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, srvc_loc as surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Admit30_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + "and post_admit_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ED30_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and post_ed_ind='Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, srvc_loc as surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ED30_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and post_ed_ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    // strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, PRODUCT, [PROCEDURE], PROCEDURE_DATE, FINAL_DX FROM VW_PBP_Spine_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND [PROCEDURE]='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, PRODUCT, [PROCEDURE], PROCEDURE_DATE, FINAL_DX FROM VW_PBP_Spine_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " AND [Procedure]='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "C";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND Num_Cnt>=1 Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "E";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Tymp_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Tymp_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "E";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_OON_ASST_SURG_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_OON_ASST_SURG_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "E";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_SOS_OPH_ASC_dtl_Ph32 Where attr_mpin=" + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_SOS_OPH_ASC_dtl_Ph33 Where attr_mpin= " + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "E";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ASST_SURG_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ASST_SURG_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "C";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, FST_SINSTS_DX FROM VW_PBP_Sinusitis_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Sinusitis_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM, MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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
                    strTopRange = "D";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, POS, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Redo_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND CMPLNT_IND='N' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, fst_srvc_dt as PROCEDURE_DATE FROM VW_PBP_sleep_dtl_Ph33 Where ATTR_MPIN= " + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM, fst_srvc_dt";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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



                   
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);

                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    //MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    string strPDFPath = MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strFilePathGLOBAL == null)
                        strFilePathGLOBAL = strPDFPath;


                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB

                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);

                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);

                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);


                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);

                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);




            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);

                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                //txtStatus.AppendText("Process Completed" + Environment.NewLine);
                //

                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }

            }
        }





















        private static void PCPCohort3(ref TextBox txtStatus)
        {

            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

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


            int intRowAdd = 0;

            try
            {

                txtStatus.AppendText("Opening Excel Template Instance..." + Environment.NewLine);


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


                    strSQL = "select FirstName,LastName,Spec_display,o.MKT_RLLP_NM, [State] AS [State]  from dbo.PBP_outl_ph13 as o inner join dbo.PBP_outl_demogr_Ph13 as d on d.mpin=o.mpin where exclude in(0,5) and  o.MPIN = " + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);

                        continue;
                    }

                    phyName = (dt.Rows[0]["LastName"] != DBNull.Value ? (dt.Rows[0]["FirstName"].ToString().Trim() + " " + dt.Rows[0]["LastName"].ToString().Trim()) : "NAME MISSING");
                    LastName = (dt.Rows[0]["LastName"] != DBNull.Value ? dt.Rows[0]["LastName"].ToString().Trim() : "NAME MISSING");
                    phyState = (dt.Rows[0]["State"] != DBNull.Value ? dt.Rows[0]["State"].ToString().Trim() : "STATE MISSING");
                    strSpecialty = (dt.Rows[0]["Spec_display"] != DBNull.Value ? dt.Rows[0]["Spec_display"].ToString().Trim() : "SPECIALTY MISSING");
                    strMarketName = (dt.Rows[0]["MKT_RLLP_NM"] != DBNull.Value ? dt.Rows[0]["MKT_RLLP_NM"].ToString().Trim() : "SPECIALTY MISSING");


                    strFinalReportFileName = strMPIN + "_" + LastName + "_" + phyState;
                    strFinalReportFileName = strFinalReportFileName.Replace(" ", "").Replace("\\", "").Replace("/", "").Replace("'", "").Replace("*", "") + "_Details_" + strMonthYear;


                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strMPINGLOBAL == null)
                        strMPINGLOBAL = strMPIN;
                    if (strProjectNameGLOBAL == null)
                        strProjectNameGLOBAL = "PCP Cohort 3";
                    if (strLastNameGLOBAL == null)
                        strLastNameGLOBAL = LastName;


                    //GET MPIN INFO END


                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";

                    strSQL = "select distinct E.MBR_FST_NM, E.MBR_LST_NM,E.INDV_BTH_DT, M.RP_RISK_CATGY,M.PRDCT_LVL_1_NM, DOS, case when Sens is null then AHRQ_Diagnosis_Category else 'SENSITIVE HEALTH CONDITION' end as AHRQ_Diagnosis_Category from dbo.PBP_ER_Details_Ph13 E INNER JOIN DBO.PBP_MPIN_CLIENT_Ph13 M ON E.INDV_SYS_ID=M.INDV_SYS_ID AND E.ATTR_MPIN=M.ATTR_MPIN INNER JOIN dbo.PBP_dx as D on E.DIAG_1_CD=D.diag_cd where E.attr_mpin =" + strMPIN + " and OP_EVENT_KEY <>0 AND E.MBR_LST_NM IS NOT NULL group by e.attr_mpin,e.INDV_SYS_ID,E.MBR_FST_NM,E.MBR_LST_NM,E.INDV_BTH_DT,M.RP_RISK_CATGY,AHRQ_Diagnosis_Category, M.PRDCT_LVL_1_NM,DOS,case when Sens is null then AHRQ_Diagnosis_Category else 'SENSITIVE HEALTH CONDITION' end having SUM(DERIV_ALLW_AMT)<>0 ORDER BY MBR_LST_NM ASC, MBR_FST_NM ASC, INDV_BTH_DT ASC, DOS ASC, AHRQ_Diagnosis_Category ASC";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,APR_DRG,ADMIT_DT";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";




                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {


                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    strSheetname = "Out-of-network_lab_and_path";

                    strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph13 where attr_mpin=" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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

                    //Level_4_and_5_visits SHEET START///////////////
                    //Level_4_and_5_visits SHEET START///////////////
                    //Level_4_and_5_visits SHEET START///////////////
                    strSheetname = "Level_4_and_5_visits";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,PROV_TYP_NM,SPEC_TYP_NM from dbo.VW_PBP_OON_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);



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


                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl13 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph13 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph13 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);

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

                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Antibiotics SHEET START///////////////
                    //Antibiotics SHEET START///////////////
                    //Antibiotics SHEET START///////////////

                    strSheetname = "Antibiotics";

                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_ABX_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Med_Adherence SHEET START///////////////
                    //Med_Adherence SHEET START///////////////
                    //Med_Adherence SHEET START///////////////

                    strSheetname = "Med_Adherence";

                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, MeasureName FROM VW_PBP_MedAdherence_dtl_Ph13 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);

                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);


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




                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);



                    string strPDFPath = MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strFilePathGLOBAL == null)
                        strFilePathGLOBAL = strPDFPath;



                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB

                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);

                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);

                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);


                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);

                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);




            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);

                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }



                //txtStatus.AppendText("Detail Reports are Ready" + Environment.NewLine);
                //
            }


        }


        private static void SpecialtiesCohort2(ref TextBox txtStatus)
        {

            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

            ArrayList alActiveSheets = new ArrayList();
            ArrayList alActiveRanges = new ArrayList();

            DataTable dt = null;
            string strSheetname = null;
            string strTopRange = null;

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


            int intRowAdd = 0;

            try
            {
                txtStatus.AppendText("Opening Excel Template Instance..." + Environment.NewLine);
                

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


                    strSQL = "select FirstName, LastName,a.Spec_display as NDB_Specialty,b.[State],a.MKT_RLLP_NM from dbo.PBP_Outl_Ph32 as a inner join dbo.PBP_outl_demogr_ph32 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_PTI_addr_ph32 as p on p.mpin=PTIGroupID_upd inner join dbo.PBP_spec_handl_ph32 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where a.Exclude in(0,5) and r.phase_id=2 and a.MPIN = " + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);
                        
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



                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strMPINGLOBAL == null)
                        strMPINGLOBAL = strMPIN;
                    if (strProjectNameGLOBAL == null)
                        strProjectNameGLOBAL = "Specialty Cohort 2";
                    if (strLastNameGLOBAL == null)
                        strLastNameGLOBAL = LastName;




                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    //ED_Utilization SHEET START///////////////
                    strSheetname = "ED_Utilization";
                    strTopRange = "G";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////
                    //ED_Utilization SHEET END///////////////



                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    //Hospital_Admissions SHEET START///////////////
                    strSheetname = "Hospital_Admissions";
                    strTopRange = "I";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY, APR_DRG from dbo.VW_PBP_IP_dtl_ph32 where attr_mpin=" + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph32 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph32 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45C_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Modpx_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_AdvIm_dtl_ph32 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Gen_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph32 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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


                    //Tier_3_Prescribing SHEET START///////////////
                    //Tier_3_Prescribing SHEET START///////////////
                    //Tier_3_Prescribing SHEET START///////////////
                    strSheetname = "Tier_3_Prescribing";
                    strTopRange = "H";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT,phys_type from dbo.VW_PBP_Rx_PCP_dtl32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////


                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    //Specialty_Specific_Diagnostics SHEET START///////////////
                    strSheetname = "Specialty_Specific_Diagnostics";
                    strTopRange = "G";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,AHRQ_PROC_DTL_CATGY_DESC,Proc_Count from dbo.VW_PBP_sp_specif_dtl_ph32 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "F";
                    strSQL = "SELECT MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AGECAT, DELIVERY_TYPE, DELIVERY_DT FROM VW_PBP_CSection_dtl_ph32 WHERE attr_mpin=" + strMPIN + " AND DELIVERY_TYPE= 'NORMAL C-SECTION' Order by MBR_LST_NM, MBR_FST_NM, DELIVERY_DT";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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



                    //Vaginal_Hysterectomy SHEET START///////////////
                    //Vaginal_Hysterectomy SHEET START///////////////
                    //Vaginal_Hysterectomy SHEET START///////////////
                    strSheetname = "Vaginal_Hysterectomy";
                    strTopRange = "G";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, severity, site_catgy_cd, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_VH_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and vh_cnt=0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Vaginal_Hysterectomy SHEET END///////////////
                    //Vaginal_Hysterectomy SHEET END///////////////
                    //Vaginal_Hysterectomy SHEET END///////////////


                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    //Pre_Cardiac_Cath_Dx_Testing SHEET START///////////////
                    strSheetname = "Pre_Cardiac_Cath_Dx_Testing";
                    strTopRange = "F";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_PreCardic_Cath_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND Target_Count>=3 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "F";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Neg_Cath_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND PEG_ANCH_CATGY_Rev='DXCATH' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "F";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE, DX_CATGY FROM VW_PBP_Stent_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND PEG_ANCH_CATGY_Rev in ('TXCAT2', 'TXCAT3', 'STENT') Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "G";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, POS, PEG_ANCH_CATGY_DESC as 'PROCEDURE', PEG_ANCH_DT as Procedure_Date FROM VW_PBP_Compl30_dtl_Ph32 Where attr_mpin=" + strMPIN + " AND CMPLCTN_IND=1 Order by MBR_LST_NM, MBR_FST_NM, PEG_ANCH_DT";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "G";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Admit30_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and post_admit_ind='Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Admit30_dtl_Ph32 WHERE cast(ATTR_MPIN as varchar) + ' ' + post_admit_ind = '" + strMPIN + " Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "G";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, AprDrg_svrty, surg_loc, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ED30_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and post_ed_ind='Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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

                    //Redo_Rate SHEET START///////////////
                    //Redo_Rate SHEET START///////////////
                    //Redo_Rate SHEET START///////////////
                    strSheetname = "Redo_Rate";
                    strTopRange = "G";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, SEVERITY, POS, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Redo_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND CMPLNT_IND='N' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Redo_Rate SHEET END///////////////
                    //Redo_Rate SHEET END///////////////
                    //Redo_Rate SHEET END///////////////


                    //Spinal_Fusion SHEET START///////////////
                    //Spinal_Fusion SHEET START///////////////
                    //Spinal_Fusion SHEET START///////////////
                    strSheetname = "Spinal_Fusion";
                    strTopRange = "G";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, PRODUCT, [PROCEDURE], PROCEDURE_DATE, FINAL_DX FROM VW_PBP_Spine_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND [PROCEDURE]='FUSION; LUMBAR BACK' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "C";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " Order by MBR_LST_NM, MBR_FST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Opioid_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " AND Num_Cnt>=1 Order by MBR_LST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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



                    //Tonsil_Adenoid SHEET START///////////////
                    //Tonsil_Adenoid SHEET START///////////////
                    //Tonsil_Adenoid SHEET START///////////////
                    strSheetname = "Tonsil_Adenoid";
                    strTopRange = "E";
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_TAD_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_TAD_dtl_Ph32 WHERE cast(ATTR_MPIN as varchar) + ' ' + cast(COND_CNT as varchar) = '" + strMPIN + " 0' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Tonsil_Adenoid SHEET END///////////////
                    //Tonsil_Adenoid SHEET END///////////////
                    //Tonsil_Adenoid SHEET END///////////////

                    //Tympanostomy SHEET START///////////////
                    //Tympanostomy SHEET START///////////////
                    //Tympanostomy SHEET START///////////////
                    strSheetname = "Tympanostomy";
                    strTopRange = "E";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_Tymp_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and COND_CNT = 0 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "E";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_OON_ASST_SURG_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and OON_Ind_Event= 'Y' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "E";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_SOS_OPH_ASC_dtl_Ph32 Where attr_mpin=" + strMPIN + " AND OP_CLM_PL_OF_SRVC_DESC='OUTPATIENT HOSPITAL' Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    strTopRange = "E";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, [PROCEDURE], PROCEDURE_DATE FROM VW_PBP_ASST_SURG_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and Asst_Surg_Event_Ind= 1 Order by MBR_LST_NM, MBR_FST_NM, PROCEDURE_DATE";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT FROM VW_PBP_Sinusitis_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM, MBR_FST_NM";
                    strSQL = "Select MBR_FST_NM, MBR_LST_NM, INDV_BTH_DT, FST_SINSTS_DX FROM VW_PBP_Sinusitis_dtl_Ph32 Where ATTR_MPIN=" + strMPIN + " and NUMRTR_FLAG='Y' Order by MBR_LST_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    //MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());






                    string strPDFPath = MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    //2018 EMAIL AUTOMATION ONLY FIRST MPIN!!!!!!
                    if (strFilePathGLOBAL == null)
                        strFilePathGLOBAL = strPDFPath;





                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB

                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);
                    
                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                    

                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);
                
                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);
                



            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);
                
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                //txtStatus.AppendText("Process Completed" + Environment.NewLine);
                //

                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }

            }
        }


        private static void PCPCohort2(ref TextBox txtStatus)
        {

            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

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


            int intRowAdd = 0;

            try
            {

                txtStatus.AppendText("Opening Excel Template Instance..." + Environment.NewLine);
                

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


                    strSQL = "select FirstName,LastName,Spec_display as NDB_Specialty,o.MKT_RLLP_NM , [State] AS [State] from dbo.PBP_outl_ph12 as o inner join dbo.PBP_outl_demogr_Ph12 as d on d.mpin=o.mpin where o.MPIN = " + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);
                        
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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_PH2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_ph12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS,AHRQ_Diagnosis_Category";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";


                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,ADMIT_DT,DSCHRG_DT,STAT_DAY,/*LOS*/ APR_DRG from dbo.VW_PBP_IP_dtl_ph12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT,APR_DRG";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph2 where attr_mpin = " + strMPIN ;
                    strSQL = "Select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LP_dtl_ph12 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";




                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {


                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        



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
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////
                    //Lab_and_Path SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    //Out-of-network_lab_and_path SHEET START///////////////
                    strSheetname = "Out-of-network_lab_and_path";
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LPOON_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    strSQL = "SELECT MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,procs, cost_type from dbo.VW_PBP_LPOON_dtl_ph12 where attr_mpin =" + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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

                    //Level_4_and_5_visits SHEET START///////////////
                    //Level_4_and_5_visits SHEET START///////////////
                    //Level_4_and_5_visits SHEET START///////////////
                    strSheetname = "Level_4_and_5_visits";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, PROC_CD";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, AHRQ_Diagnosis_Category";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod_dtl12 where attr_mpin=" + strMPIN + "order by MBR_LST_NM,MBR_FST_NM,AHRQ_Diagnosis_Category";

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LPOON_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,SPEC_TYP_NM,PROV_TYP_NM from dbo.VW_PBP_OON_dtl_ph12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,SPEC_TYP_NM,PROV_TYP_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        


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


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        


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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_AdvIm_dtl_ph2 where attr_mpin = " + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, Rad_Gen_Category";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category,Proc_count from dbo.VW_PBP_AdvIm_dtl_ph12 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM,Rad_Gen_Category,Proc_Count from dbo.VW_PBP_AdvIm_dtl_ph2 where attr_mpin = " + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, Rad_Gen_Category";
                    strSQL = "select distinct MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM,Rad_Category, Proc_count from dbo.VW_PBP_NAdvIm_dtl_ph12 where attr_mpin = " + strMPIN + " order by mbr_lst_nm,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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

                    //Tier_3_Prescribing SHEET START///////////////
                    //Tier_3_Prescribing SHEET START///////////////
                    //Tier_3_Prescribing SHEET START///////////////
                    strSheetname = "Tier_3_Prescribing";

                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////
                    //Tier_3_Prescribing SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



                    //Other_Phys_Tier_3_Prescribing SHEET START///////////////
                    //Other_Phys_Tier_3_Prescribing SHEET START///////////////
                    //Other_Phys_Tier_3_Prescribing SHEET START///////////////
                    strSheetname = "Other_Phys_Tier_3_Prescribing";


                    //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT,phys_type from dbo.VW_PBP_Rx_SPEC_dtl12 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";

                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RP_RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_SPEC_dtl12 where attr_mpin=" + strMPIN + "  order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Other_Phys_Tier_3_Prescribing SHEET END///////////////
                    //Other_Phys_Tier_3_Prescribing SHEET END///////////////
                    //Other_Phys_Tier_3_Prescribing SHEET END///////////////

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //Opioids SHEET START///////////////
                    //Opioids SHEET START///////////////
                    //Opioids SHEET START///////////////

                    strSheetname = "Opioids";
                    //strSQL = "Select Count(*) FROM VW_PBP_OBCompl_dtl_ph2 Where attr_mpin=" + strMPIN;
                    strSQL = "Select Distinct c.MBR_FST_NM, c.MBR_LST_NM, c.INDV_BTH_DT, 'COMMERCIAL' as Product From dbo.Opioid_Indv_Dtl as a Inner Join (SELECT * FROM PBP_outl_Ph12 WHERE EXCLUDE in (0,5) and Opiod_Outl=1) as b on a.PR_MPIN=b.mpin Inner join dbo.PBP_MPIN_CLIENT_Ph12 as c on a.INDV_SYS_ID=c.INDV_SYS_ID Where PR_MPIN=" + strMPIN + " Order by c.MBR_LST_NM, c.MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    //Opioids SHEET END///////////////
                    //Opioids SHEET END///////////////
                    //Opioids SHEET END///////////////




                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN +  Environment.NewLine);
                    

                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB

                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);
                    
                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                    

                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);
                
                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);
                



            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);
                
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }



                //txtStatus.AppendText("Detail Reports are Ready" + Environment.NewLine);
                //
            }


        }

        private static void SpecialtiesCohort1(ref TextBox txtStatus)
        {
            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

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
                txtStatus.AppendText("Opening Excel Template Instance..." + Environment.NewLine);
                


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


                    //if (strMPIN != "251862")
                    //continue;

                    //OPEN EXCEL WB
                    MSExcel.openExcelWorkBook();


                    strSQL = "select FirstName, LastName, Spec_display as NDB_Specialty, b.[State], a.MKT_RLLP_NM  from dbo.PBP_Outl_ph3 as a inner join dbo.PBP_outl_demogr_ph3 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph3 as ad on ad.TaxID=b.TaxID   inner join dbo.PBP_spec_handl_ph3 as h on h.MPIN=a.mpin    inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM where r.phase_id=2 and a.MPIN =" + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        //txtStatus.AppendText();
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            intRowAdd = 1;
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    strSQL = "Select Count(*) FROM VW_PBP_Stent_dtl_Ph3 Where mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            intRowAdd = 1;
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
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


                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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

                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);
                    


                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);
                    
                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                    

                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);
                
                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);
                



            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);
                
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                //txtStatus.AppendText("Process Completed" + Environment.NewLine);
                //

                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }



                //txtStatus.AppendText("Detail Reports are Ready" + Environment.NewLine);
                //
            }
        }

        private static void OBGYNCohort1(ref TextBox txtStatus)
        {
            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

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

                    strSQL = "SELECT FirstName, LastName, NDB_Specialty, b.[State], a.MKT_RLLP_NM FROM dbo.PBP_Outl_ph2 as a inner join dbo.PBP_outl_demogr_ph2 as b on a.MPIN=b.MPIN inner join dbo.PBP_outl_TIN_addr_Ph2 as ad on ad.TaxID=b.TaxID inner join dbo.PBP_spec_handl_ph2 as h on h.MPIN=a.mpin inner join dbo.PBP_dim_RCMO as r on r.Region=b.RGN_NM WHERE a.Exclude in(0,5) AND r.phase_id=2 AND a.MPIN = " + strMPIN;


                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);
                        
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

                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_PH2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl_PH2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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

                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl_Ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);

                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        intRowAdd = 0;
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        


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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        intRowAdd = 0;
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        
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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);
                        intRowAdd = 0;

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, AHRQ_Diagnosis_Category";
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);

                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {

                            intRowAdd = 0;

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod59_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod59_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM, AHRQ_Diagnosis_Category";
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {

                            intRowAdd = 0;

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        intRowAdd = 0;
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        intRowAdd = 0;
                        alActiveSheets.Add(strSheetname);

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        //strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                        strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT from dbo.VW_PBP_Rx_PCP_dtl_ph2 where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        alActiveSheets.Add(strSheetname);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            intRowAdd = 0;

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            

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
                    strSQL = "Select Count(*) FROM VW_PBP_CSection_dtl_ph2 Where attr_mpin=" + strMPIN;
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;

                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
  

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
                    intRowCount = int.Parse(DBConnection.getMSSQLExecuteScalar(strConnectionString, strSQL) + "");

                    if (intRowCount == 0)
                    {
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                        
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
                        dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                        if (dt.Rows.Count <= 0)
                        {

                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + "" + Environment.NewLine);
                            
                            intRowAdd = 1;
                        }
                        else
                        {
                            txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                            
          
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
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);
                    
                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                    

                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);
                
                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);
                



            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);
                
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                //txtStatus.AppendText("Process Completed" + Environment.NewLine);
                //

                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }



                //txtStatus.AppendText("Detail Reports are Ready" + Environment.NewLine);
                //
            }
        }

        private static void PCPCohort1(ref TextBox txtStatus)
        {
            string strConnectionString = GlobalObjects.strILUCAMainConnectionString;
            string strExcelTemplate = _strTemplatePath;
            string strReportsPath = _strReportPath;
            string[] strMpinArr = _strMpinArr;

            bool blGenerated = false;

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


                    strSQL = "select FirstName,LastName,NDB_Specialty,[State], a.MKT_RLLP_NM from dbo.PBP_outl_demogr_ph1 as a inner join dbo.PBP_outl_ph1 as b on a.MPIN=b.MPIN where exclude in(0,4)and a.MPIN = " + strMPIN;

                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                    if (dt.Rows.Count <= 0)
                    {
                        txtStatus.AppendText("No records found for MPIN: " + strMPIN + Environment.NewLine);
                        
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
                    strSheetname = "ED_Utilization";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT, RISK_CATGY,PRDCT_LVL_1_NM,DOS,AHRQ_Diagnosis_Category from dbo.VW_PBP_ER_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,DOS";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {
                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //ED_Utilization SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Inpt_Adm_ALOS SHEET START///////////////
                    strSheetname = "Inpt_Adm_ALOS";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, ADMIT_DT,DSCHRG_DT,LOS, APR_DRG_DESC from dbo.VW_PBP_IP_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,ADMIT_DT";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        




                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":I" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":I" + (dt.Rows.Count + 14), strSheetname);



                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("I" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Inpt_Adm_ALOS SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //LabPath_Utilization SHEET START///////////////
                    strSheetname = "LabPath_Utilization";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,procs,COST_TYPE from dbo.VW_PBP_LP_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,COST_TYPE";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        




                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //LabPath_Utilization SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Phys_Level _4,5_E&M SHEET START///////////////
                    strSheetname = "Phys_Level _4,5_E&M";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,DOS,PROC_CD,PROC_DESC from dbo.VW_PBP_LVL45_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        


                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);


                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));


                    }
                    //Phys_Level _4,5_E&M SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Phys_mod_25 SHEET START///////////////
                    strSheetname = "Phys_mod_25";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod25_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Phys_mod_25 SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Phys_mod_59 SHEET START///////////////
                    strSheetname = "Phys_mod_59";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,num_claims,AHRQ_Diagnosis_Category from dbo.VW_PBP_Mod59_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Phys_mod_59 SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //NonTier 1 specs SHEET START///////////////
                    strSheetname = "NonTier 1 specs";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Not_t1_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,PROV_TYP_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //NonTier 1 specs SHEET END///////////////


                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //OON specs SHEET START///////////////
                    strSheetname = "OON";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,SPEC_TYP_NM,PROV_TYP_NM from dbo.VW_PBP_OON_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //OON specs SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Specialty_Phys_visits SHEET START///////////////
                    strSheetname = "Specialty_Phys_visits";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,VST_CNT,PROV_TYP_NM from dbo.VW_PBP_Spec_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":G" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":G" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("G" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Specialty_Phys_visits SHEET END///////////////





                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Rad_Utilization SHEET START///////////////
                    strSheetname = "Rad_Utilization";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,Rad_Category,Proc_Count,COST_TYPE from dbo.VW_PBP_Rad_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":H" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":H" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("H" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //Rad_Utilization SHEET END///////////////



                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //PCP_Tier_3_Rx SHEET START///////////////
                    strSheetname = "PCP_Tier_3_Rx";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT,phys_type from dbo.VW_PBP_Rx_PCP_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":I" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":I" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");
                        intRowAdd = 0;
                        alActiveRanges.Add("I" + (dt.Rows.Count + 14 + intRowAdd));

                    }
                    //PCP_Tier_3_Rx SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Specialist_Tier_3 Rx SHEET START///////////////
                    strSheetname = "Specialist_Tier_3 Rx";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY,PRDCT_LVL_1_NM, CLI_DRG_CST_TIER_CD,AHFS_THERAPEUTIC_CLSS_DESC,SCRPT_CNT,phys_type from dbo.VW_PBP_Rx_Spec_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM,AHFS_THERAPEUTIC_CLSS_DESC";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":I" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":I" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("I" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Specialist_Tier_3 Rx SHEET END///////////////




                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    //Phys_Office_Visits SHEET START///////////////
                    strSheetname = "Phys_Office_Visits";
                    strSQL = "select MBR_FST_NM,MBR_LST_NM,INDV_BTH_DT,RISK_CATGY, PRDCT_LVL_1_NM,VST_CNT from dbo.VW_PBP_PCP_dtl where attr_mpin=" + strMPIN + " order by MBR_LST_NM,MBR_FST_NM";
                    dt = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                    if (dt.Rows.Count <= 0)
                    {

                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": NO " + strSheetname + " records for MPIN: " + strMPIN + "" + Environment.NewLine);
                        
                        MSExcel.deleteWorksheet(strSheetname);
                    }
                    else
                    {

                        alActiveSheets.Add(strSheetname);
                        txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Populating " + strSheetname + " sheet for MPIN: " + strMPIN + "" + Environment.NewLine);
                        

                        MSExcel.addValueToCell(strSheetname, "B7", "DR. " + phyName);
                        MSExcel.addValueToCell(strSheetname, "B8", strMPIN);
                        MSExcel.addValueToCell(strSheetname, "B9", strSpecialty);
                        MSExcel.addValueToCell(strSheetname, "B10", strMarketName);


                        MSExcel.populateTable(dt, strSheetname, 15, 'A');
                        MSExcel.addBorders("A15" + ":F" + (dt.Rows.Count + 14), strSheetname);
                        MSExcel.AutoFitRange("A14" + ":F" + (dt.Rows.Count + 14), strSheetname);

                        MSExcel.addFocusToCell(strSheetname, "A1");

                        intRowAdd = 0;
                        alActiveRanges.Add("F" + (dt.Rows.Count + 14 + intRowAdd));
                    }
                    //Phys_Office_Visits SHEET END///////////////



                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final PDF File for MPIN: " + strMPIN + Environment.NewLine);
                    

                    alActiveSheets.Add("Appendix");
                    alActiveRanges.Add("B11");
                    MSExcel.CloneAsPDF(strFinalReportFileName, alActiveSheets.ToArray(), alActiveRanges.ToArray());


                    // MSExcel.CloneAsPDF(strFinalReportFileName, new object[] { "Lab_and_Path", "Level_4_and_5_visits", "Advanced_Imaging", "Non-Advanced_Imaging", "Appendix" });

                    // "Advanced_Imaging","Non-Advanced_Imaging","Appendix"
                    //CLOSE EXCEL WB
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Generating Final Excel File for MPIN: " + strMPIN + Environment.NewLine);
                    
                    MSExcel.closeExcelWorkbook(strFinalReportFileName, "");

                    //IF WE MADE IT HERE, AT LEAST ONE REPORT WAS GENERATE SO WELL DISPLAY WINDOWS EXPLORER WITHIN FINALLY BLOCK
                    blGenerated = true;


                    //COMPLETED MESSAGE
                    txtStatus.AppendText(intCnt + " of " + intMPINTotal + ": Completed File for MPIN " + strMPIN + "" + Environment.NewLine + Environment.NewLine);
                    
                    txtStatus.AppendText("----------------------------------------------------------------------------" + Environment.NewLine + Environment.NewLine);
                    

                    intCnt++;
                    Application.DoEvents();

                }


            }
            catch (Exception ex)
            {

                txtStatus.AppendText("There was an error, see details below" + Environment.NewLine + Environment.NewLine);
                
                txtStatus.AppendText(ex.ToString() + Environment.NewLine + Environment.NewLine);
                



            }
            finally
            {

                txtStatus.AppendText("Closing Microsoft Excel Instance..." + Environment.NewLine);
                
                //CLOSE EXCEL APP
                MSExcel.closeExcelApp();


                //txtStatus.AppendText("Process Completed" + Environment.NewLine);
                //

                if (blGenerated)
                {
                    if (Directory.Exists(_strReportPath))
                    {
                        Process.Start(_strReportPath);
                    }
                }



                //txtStatus.AppendText("Detail Reports are Ready" + Environment.NewLine);
                //
            }
        }


        private static string getTemplatePath(string strProject, bool blClone = false)
        {
            string strTemplatePath = GlobalObjects.strMemberDetailsTemplatePath + strProject.Replace(" ", "") + "_template.xlsx";

            if (File.Exists(strTemplatePath))
            {
                if (blClone)
                {
                    string strClonedPath = GlobalObjects.strMemberDetailsTemplatePath + "tmp\\" + GlobalObjects.strCurrentUser + "\\";
                    string strFileName = strProject.Replace(" ", "") + "_template.xlsx";
                    if (File.Exists(strClonedPath + strFileName))
                    {

                        //while (true)
                        //{
                           try
                            {
                                File.Delete(strClonedPath + strFileName);
                            //    break; // success!
                            }
                            catch(Exception ex)
                            {
                            //    SharedFunctions.killProcess("EXCEL");
                            }
                        //}

                    }
                    if(!Directory.Exists(strClonedPath))
                    {
                        Directory.CreateDirectory(strClonedPath);
                    }

                    File.Copy(strTemplatePath, strClonedPath + strFileName,true);
                    strTemplatePath = strClonedPath + strFileName;
                }

                return strTemplatePath;
            }
            else
                return null;

        }

        private static string[] getMPINArray(string strMpinCSV)
        {
            string[] strMpinArr = null;
            List<string> strMpinTmp = new List<string>(); 
            if (!String.IsNullOrEmpty(strMpinCSV))
            {
                strMpinArr = strMpinCSV.Split(',');

                foreach (string s in strMpinArr)
                {
                    if(s.Trim().IsNumeric())
                    {
                        strMpinTmp.Add(s.Trim());
                    }
                }
            }

            if (strMpinTmp.Count() > 0)
                return strMpinTmp.ToArray();
            else
                return null;
        }




    }
}
