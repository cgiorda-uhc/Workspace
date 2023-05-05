using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDC_Analyzer_Excel_Parser
{
    class EDC_Analyzer_Excel_Parser
    {



        static void Main(string[] args)
        {

            //firstPass();
           // return;



            string[] strExpectedColumns = ("Market Nbr,MPIN,Cirrus ID,Plan ID,Facility Name,State,UHN Region,Corporation Name,NPI ,Tax ID 1,Tax ID 2,Tax ID 3,Tax ID 4,Tax ID 5,Tax ID 6,Tax ID 7,FSED,BIC System, E&I Status,OHP UNET Status,OHP Cirrus Status, M&R Status, C&S Status,DSNP Status,IEX Status,E&I Excluded ,OHP Exclusion,M&R Excluded ,C&S Excluded ,DSNP Excluded ,IEX Excluded ,State E&I Status,State M&R Status,State C&S Status,State DSNP Status,State IEX Status,Overall Status,Retro or Correction,Eff Date (E&I),Eff Date OHP UNET,Eff Date OHP Cirrus,Eff Date (M&R),Eff Date (C&S),Eff Date (DSNP),Eff Date (IEX),Removal Date (E&I),Removal Date OHP UNET,Removal Date OHP Cirrus,Removal Date (M&R),Removal Date (C&S),Removal Date (DSNP),Removal Date (IEX),Old Eff Date (E&I),Old Eff Date OHP UNET,Old Eff Date OHP Cirrus,Old Eff Date (M&R),Old Eff Date (C&S),Old Eff Date (DSNP),Old Eff Date (IEX),RptgEIEffDt,RptgEIRmvlDt,EIActivInd,RptgOxEffDt,RptgOxRmvlDt,OxActivInd,RptgMREffDt,RptgMRRmvlDt,MRActivInd,RptgCSEffDt,RptgCSRmvlDt,CSActivInd,RptgDSNPEffDt,RptgDSNPRmvlDt,DSNPActivInd,RptgIEXEffDt,RptgIEXRptgDt,IEXActivInd").Split(',').Select(e => e.Trim()).ToArray();

            string[] strCommonColumns = ("Market Nbr,MPIN,Cirrus ID,Plan ID,Facility Name,State,UHN Region,Corporation Name,NPI ,Tax ID 1,Tax ID 2,Tax ID 3,Tax ID 4,Tax ID 5,Tax ID 6,Tax ID 7,FSED,BIC System, Overall Status,Retro or Correction").Split(',').Select(e => e.Trim()).ToArray();


            string[] strRemoveColumns = ("E&I Status,OHP UNET Status,OHP Cirrus Status, M&R Status, C&S Status,DSNP Status,IEX Status,E&I Excluded ,OHP Exclusion,M&R Excluded ,C&S Excluded ,DSNP Excluded ,IEX Excluded ,State E&I Status,State M&R Status,State C&S Status,State DSNP Status,State IEX Status,Eff Date (E&I),Eff Date OHP UNET,Eff Date OHP Cirrus,Eff Date (M&R),Eff Date (C&S),Eff Date (DSNP),Eff Date (IEX),Removal Date (E&I),Removal Date OHP UNET,Removal Date OHP Cirrus,Removal Date (M&R),Removal Date (C&S),Removal Date (DSNP),Removal Date (IEX),Old Eff Date (E&I),Old Eff Date OHP UNET,Old Eff Date OHP Cirrus,Old Eff Date (M&R),Old Eff Date (C&S),Old Eff Date (DSNP),Old Eff Date (IEX),RptgEIEffDt,RptgEIRmvlDt,EIActivInd,RptgOxEffDt,RptgOxRmvlDt,OxActivInd,RptgMREffDt,RptgMRRmvlDt,MRActivInd,RptgCSEffDt,RptgCSRmvlDt,CSActivInd,RptgDSNPEffDt,RptgDSNPRmvlDt,DSNPActivInd,RptgIEXEffDt,RptgIEXRptgDt,IEXActivInd").Split(',').Select(e => e.Trim()).ToArray();




            string strDataFolderPath = ConfigurationManager.AppSettings["DataFolderPath"];
            string strConnectionString = ConfigurationManager.AppSettings["FinalDatabase"];
            string strSheetName = "Master File";
            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable;
            StringBuilder sbMissing = new StringBuilder();
            string strReportDate = "Oct 2021";
            foreach (string fileName in Directory.GetFiles(strDataFolderPath, "*.xlsx"))
            {

                wbCurrentExcelFile = SpreadsheetDocument.Open(fileName, false);
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetName, 1, 2);

                foreach (DataColumn column in dtCurrentDataTable.Columns)
                {
                    if (!strExpectedColumns.Contains(column.ColumnName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        sbMissing.Append("Column " + column.ColumnName.Trim() + " has been ADDED, this update must be processed first!!!");
                    }
                }

                foreach (string s in strExpectedColumns)
                {
                    if (!dtCurrentDataTable.Columns.Contains(s.Trim()))
                    {
                        sbMissing.Append("Column " + s.Trim() + " has been REMOVED, this update must be processed first!!!");
                    }
                }



                dtFinalDataTable = dtCurrentDataTable.Clone();


                foreach (string s in strRemoveColumns)
                    dtFinalDataTable.Columns.Remove( s.Trim() );


                dtFinalDataTable.Columns.Add("LOB", typeof(String));
                dtFinalDataTable.Columns.Add("Status", typeof(String));
                dtFinalDataTable.Columns.Add("ContrType", typeof(String));
                dtFinalDataTable.Columns.Add("Excl", typeof(String));
                dtFinalDataTable.Columns.Add("State_Status", typeof(String));
                dtFinalDataTable.Columns.Add("EffDate", typeof(DateTime));
                dtFinalDataTable.Columns.Add("RemovalDt", typeof(DateTime));
                dtFinalDataTable.Columns.Add("OLDEffDt", typeof(DateTime));
                dtFinalDataTable.Columns.Add("rptEffDate", typeof(DateTime));
                dtFinalDataTable.Columns.Add("rptRmvlDt", typeof(DateTime));
                dtFinalDataTable.Columns.Add("RptgActivInd", typeof(String));
                dtFinalDataTable.Columns.Add("Report_Date", typeof(String));

                int intDateCheck;
                DataRow currentRow;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {
                    //EI
                    //EI
                    //EI
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "EI";
                    currentRow["Status"] = (dr["E&I Status"] != DBNull.Value ? dr["E&I Status"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = (dr["E&I Excluded"] != DBNull.Value ? dr["E&I Excluded"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State E&I Status"] != DBNull.Value ? dr["State E&I Status"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["Eff Date (E&I)"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date (E&I)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date (E&I)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["Removal Date (E&I)"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date (E&I)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date (E&I)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["Old Eff Date (E&I)"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date (E&I)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date (E&I)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptEffDate"] = (dr["RptgEIEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgEIEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgEIEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptRmvlDt"] = (dr["RptgEIRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgEIRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgEIRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RptgActivInd"] = (dr["EIActivInd"] != DBNull.Value ? dr["EIActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //IEX
                    //IEX
                    //IEX
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "IEX";
                    currentRow["Status"] = (dr["IEX Status"] != DBNull.Value ? dr["IEX Status"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = (dr["IEX Excluded"] != DBNull.Value ? dr["IEX Excluded"] : DBNull.Value); 
                    currentRow["State_Status"] = (dr["State IEX Status"] != DBNull.Value ? dr["State IEX Status"] : DBNull.Value); 
                    currentRow["EffDate"] = (dr["Eff Date (IEX)"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date (IEX)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date (IEX)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RemovalDt"] = (dr["Removal Date (IEX)"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date (IEX)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date (IEX)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["OLDEffDt"] = (dr["Old Eff Date (IEX)"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date (IEX)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date (IEX)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptEffDate"] = (dr["RptgIEXEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgIEXEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgIEXEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptRmvlDt"] = (dr["RptgIEXRptgDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgIEXRptgDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgIEXRptgDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RptgActivInd"] = (dr["IEXActivInd"] != DBNull.Value ? dr["IEXActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //OX_UNET
                    //OX_UNET
                    //OX_UNET
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "OX_UNET";
                    currentRow["Status"] = (dr["OHP UNET Status"] != DBNull.Value ? dr["OHP UNET Status"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = (dr["OHP Exclusion"] != DBNull.Value ? dr["OHP Exclusion"] : DBNull.Value); 
                    currentRow["State_Status"] = DBNull.Value;
                    currentRow["EffDate"] = (dr["Eff Date OHP UNET"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date OHP UNET"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date OHP UNET"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RemovalDt"] = (dr["Removal Date OHP UNET"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date OHP UNET"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date OHP UNET"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["Old Eff Date OHP UNET"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date OHP UNET"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date OHP UNET"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptEffDate"] = (dr["RptgOxEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgOxEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgOxEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgOxRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgOxRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgOxRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["OxActivInd"] != DBNull.Value ? dr["OxActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //OX_CIRRUS
                    //OX_CIRRUS
                    //OX_CIRRUS
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "OX_CIRRUS";
                    currentRow["Status"] = (dr["OHP Cirrus Status"] != DBNull.Value ? dr["OHP Cirrus Status"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = DBNull.Value;
                    currentRow["State_Status"] = DBNull.Value;
                    currentRow["EffDate"] = (dr["Eff Date OHP Cirrus"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date OHP Cirrus"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date OHP Cirrus"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["Removal Date OHP Cirrus"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date OHP Cirrus"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date OHP Cirrus"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["Old Eff Date OHP Cirrus"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date OHP Cirrus"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date OHP Cirrus"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = DBNull.Value;
                    currentRow["rptRmvlDt"] = DBNull.Value;
                    currentRow["RptgActivInd"] = DBNull.Value;
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //CS
                    //CS
                    //CS
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "CS";
                    currentRow["Status"] = (dr["C&S Status"] != DBNull.Value ? dr["C&S Status"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = (dr["C&S Excluded"] != DBNull.Value ? dr["C&S Excluded"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State C&S Status"] != DBNull.Value ? dr["State C&S Status"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["Eff Date (C&S)"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date (C&S)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date (C&S)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RemovalDt"] = (dr["Removal Date (C&S)"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date (C&S)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date (C&S)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["OLDEffDt"] = (dr["Old Eff Date (C&S)"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date (C&S)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date (C&S)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptEffDate"] = (dr["RptgCSEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgCSEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgCSEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgCSRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgCSRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgCSRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RptgActivInd"] = (dr["CSActivInd"] != DBNull.Value ? dr["CSActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //DSNP
                    //DSNP
                    //DSNP
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : null);
                    currentRow["LOB"] = "DSNP";
                    currentRow["Status"] = (dr["DSNP Status"] != DBNull.Value ? dr["DSNP Status"] : DBNull.Value);
                    currentRow["ContrType"] =  DBNull.Value;
                    currentRow["Excl"] = (dr["DSNP Excluded"] != DBNull.Value ? dr["DSNP Excluded"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State DSNP Status"] != DBNull.Value ? dr["State DSNP Status"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["Eff Date (DSNP)"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date (DSNP)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date (DSNP)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RemovalDt"] = (dr["Removal Date (DSNP)"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date (DSNP)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date (DSNP)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["Old Eff Date (DSNP)"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date (DSNP)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date (DSNP)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptEffDate"] = (dr["RptgDSNPEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgDSNPEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgDSNPEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptRmvlDt"] = (dr["RptgDSNPRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgDSNPRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgDSNPRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RptgActivInd"] = (dr["DSNPActivInd"] != DBNull.Value ? dr["DSNPActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //MR
                    //MR
                    //MR
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : null);
                    currentRow["LOB"] = "MR";
                    currentRow["Status"] = (dr["M&R Status"] != DBNull.Value ? dr["M&R Status"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = (dr["M&R Excluded"] != DBNull.Value ? dr["M&R Excluded"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State M&R Status"] != DBNull.Value ? dr["State M&R Status"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["Eff Date (M&R)"] != DBNull.Value ? (Int32.TryParse(dr["Eff Date (M&R)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Eff Date (M&R)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["RemovalDt"] = (dr["Removal Date (M&R)"] != DBNull.Value ? (Int32.TryParse(dr["Removal Date (M&R)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Removal Date (M&R)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["Old Eff Date (M&R)"] != DBNull.Value ? (Int32.TryParse(dr["Old Eff Date (M&R)"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Old Eff Date (M&R)"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value); 
                    currentRow["rptEffDate"] = (dr["RptgMREffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgMREffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgMREffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgMRRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgMRRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgMRRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["MRActivInd"] != DBNull.Value ? dr["MRActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                }


                // REPLACE ' ' WITH '_' in COLUMN NAME LOOP
                foreach (DataColumn col in dtFinalDataTable.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                }

                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "EDC_Analyzer_Stage";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE dbo."+ dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strConnectionString, 500);

            }

        }



        private static void firstPass()
        {
            string[] strExpectedColumns = ("Market_Nbr, MPIN, Tax_ID 1, Tax_ID 2, Tax_ID 3, Tax_ID 4, NPI, Corporation_Name, Facility_Name, State, UHN_Region, Free_Standing_Ind, BIC System, EIStatus, IEXStatus, MRStatus, CSStatus, DSNPStatus, EIContType, MRContType, CSContType, IEXContTYpe, EIExcl, MRExcl, CSExcl, DSNPExcl, State_EIStatus, State_MRStatus, State_CSStatus, State_DSNPStatus, Overall Status, EIEffDate, IEXEffDate, Ox_EIEffDate, MREffDate, CSEffDate, DSNPEffDate, EIRemovalDt, IEXRmvlDt, Ox_EIRmvl_Date, MRRemovalDt, CSRemovalDt, DSNPRemovalDt, EIOLDEffDt, IEXOLDEffDt, Ox_EIOLDEFFDt, MROLDEffDt, CSOLDEffDt, DSNPOLDEffDt, RptgEIEffDt, RptgEIRmvlDt, RptgIEXEfftDt, RptgIEXRmvlDt, RptgMREffDt, RptgMRRmvlDt, RptgCSEffDt, RptgCSRmvlDt, RptgDSNPEffDt, RptgDSNPRmvlDt, RptgOxEIEffDt, RptgOxEIRmvlDt, MinorMkt, RptgEIActivInd, RptgIEXActivInd, RptgMRActivInd, RptgCSActivInd, RptgDSNPActivInd, RptgOxActivInd").Split(',').Select(e => e.Trim()).ToArray();

            string[] strCommonColumns = ("Market_Nbr, MPIN, Tax_ID 1, Tax_ID 2, Tax_ID 3, Tax_ID 4, NPI, Corporation_Name, Facility_Name, State, UHN_Region, Free_Standing_Ind, BIC System, Overall Status, MinorMkt").Split(',').Select(e => e.Trim()).ToArray();


            //string[] strEIColumns = ("EIStatus, EIContType, EIExcl, State_EIStatus, EIEffDate, EIRemovalDt, EIOLDEffDt,RptgEIEffDt, RptgEIRmvlDt, RptgEIActivInd").Split(',').Select(e => e.Trim()).ToArray();
            //string[] strMRColumns = ("MRStatus, MRContType, MRExcl, State_MRStatus, MREffDate, MRRemovalDt,MROLDEffDt, RptgMREffDt, RptgMRRmvlDt,RptgMRActivInd").Split(',').Select(e => e.Trim()).ToArray();
            //string[] strCSColumns = ("CSStatus,CSContType,CSExcl,State_CSStatus, CSEffDate, CSRemovalDt, CSOLDEffDt,RptgCSEffDt, RptgCSRmvlDt,RptgCSActivInd").Split(',').Select(e => e.Trim()).ToArray();
            //string[] strIEXColumns = ("IEXStatus,IEXContTYpe, IEXEffDate, IEXRmvlDt,IEXOLDEffDt,RptgIEXEfftDt, RptgIEXRmvlDt,RptgIEXActivInd").Split(',').Select(e => e.Trim()).ToArray();
            //string[] strDSNPColumns = ("DSNPStatus,DSNPExcl, State_DSNPStatus, DSNPEffDate,DSNPRemovalDt,DSNPOLDEffDt,RptgDSNPEffDt, RptgDSNPRmvlDt,RptgDSNPActivInd").Split(',').Select(e => e.Trim()).ToArray();
            //string[] strOXColumns = ("Ox_EIEffDate, Ox_EIRmvl_Date, Ox_EIOLDEFFDt,RptgOxEIEffDt, RptgOxEIRmvlDt,RptgOxActivInd").Split(',').Select(e => e.Trim()).ToArray();



            string[] strRemoveColumns = ("EIStatus, EIContType, EIExcl, State_EIStatus, EIEffDate, EIRemovalDt, EIOLDEffDt,RptgEIEffDt, RptgEIRmvlDt, RptgEIActivInd,MRStatus, MRContType, MRExcl, State_MRStatus, MREffDate, MRRemovalDt,MROLDEffDt, RptgMREffDt, RptgMRRmvlDt,RptgMRActivInd,CSStatus,CSContType,CSExcl,State_CSStatus, CSEffDate, CSRemovalDt, CSOLDEffDt,RptgCSEffDt, RptgCSRmvlDt,RptgCSActivInd,IEXStatus,IEXContTYpe, IEXEffDate, IEXRmvlDt,IEXOLDEffDt,RptgIEXEfftDt, RptgIEXRmvlDt,RptgIEXActivInd,DSNPStatus,DSNPExcl, State_DSNPStatus, DSNPEffDate,DSNPRemovalDt,DSNPOLDEffDt,RptgDSNPEffDt, RptgDSNPRmvlDt,RptgDSNPActivInd,Ox_EIEffDate, Ox_EIRmvl_Date, Ox_EIOLDEFFDt,RptgOxEIEffDt, RptgOxEIRmvlDt,RptgOxActivInd").Split(',').Select(e => e.Trim()).ToArray();




            string strDataFolderPath = ConfigurationManager.AppSettings["DataFolderPath"];
            string strConnectionString = ConfigurationManager.AppSettings["FinalDatabase"];
            string strSheetName = "Master_File";
            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable;
            int intColCnt = 0;
            StringBuilder sbMissing = new StringBuilder();
            string strReportDate = "Sept 2021";
            foreach (string fileName in Directory.GetFiles(strDataFolderPath, "*.xls*"))
            {

                wbCurrentExcelFile = SpreadsheetDocument.Open(fileName, false);
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetName, 1, 2);

                //COUNTER APPROACH NEEDED???
                //intColCnt = 0;
                //foreach (DataColumn column in dtCurrentDataTable.Columns)
                //{
                //    if (column.ColumnName.ToLower().Trim() != strExpectedColumnsGlobal[intColCnt].ToLower().Trim())
                //    {
                //        sbMissing.Append("Column " + column.ColumnName.Trim() + " has been ADDED, this update must be processed first!!!");
                //    }
                //    intColCnt++;
                //}

                foreach (DataColumn column in dtCurrentDataTable.Columns)
                {
                    if (!strExpectedColumns.Contains(column.ColumnName.Trim(), StringComparer.OrdinalIgnoreCase))
                    {
                        sbMissing.Append("Column " + column.ColumnName.Trim() + " has been ADDED, this update must be processed first!!!");
                    }
                }

                foreach (string s in strExpectedColumns)
                {
                    if (!dtCurrentDataTable.Columns.Contains(s.Trim()))
                    {
                        sbMissing.Append("Column " + s.Trim() + " has been REMOVED, this update must be processed first!!!");
                    }
                }

                if (sbMissing.Length > 0)
                {
                    //STOP PROCESS
                    //CREATE TXT WITH MESSAGE
                    //return!!!!!!
                }



                dtFinalDataTable = dtCurrentDataTable.Clone();



                foreach (string s in strRemoveColumns)
                    dtFinalDataTable.Columns.Remove(s);

                //foreach(string s in strEIColumns)
                //    dtFinalDataTable.Columns.Remove(s);

                //foreach (string s in strMRColumns)
                //    dtFinalDataTable.Columns.Remove(s);

                //foreach (string s in strCSColumns)
                //    dtFinalDataTable.Columns.Remove(s);

                //foreach (string s in strIEXColumns)
                //    dtFinalDataTable.Columns.Remove(s);

                //foreach (string s in strDSNPColumns)
                //    dtFinalDataTable.Columns.Remove(s);

                //foreach (string s in strOXColumns)
                //    dtFinalDataTable.Columns.Remove(s);



                dtFinalDataTable.Columns.Add("LOB", typeof(String));
                dtFinalDataTable.Columns.Add("Status", typeof(String));
                dtFinalDataTable.Columns.Add("ContrType", typeof(String));
                dtFinalDataTable.Columns.Add("Excl", typeof(String));
                dtFinalDataTable.Columns.Add("State_Status", typeof(String));
                dtFinalDataTable.Columns.Add("EffDate", typeof(DateTime));
                dtFinalDataTable.Columns.Add("RemovalDt", typeof(DateTime));
                dtFinalDataTable.Columns.Add("OLDEffDt", typeof(DateTime));
                dtFinalDataTable.Columns.Add("rptEffDate", typeof(DateTime));
                dtFinalDataTable.Columns.Add("rptRmvlDt", typeof(DateTime));
                dtFinalDataTable.Columns.Add("RptgActivInd", typeof(String));
                dtFinalDataTable.Columns.Add("Report_Date", typeof(String));

                int intDateCheck;
                DataRow currentRow;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {
                    //EI
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "EI";
                    currentRow["Status"] = (dr["EIStatus"] != DBNull.Value ? dr["EIStatus"] : DBNull.Value);
                    currentRow["ContrType"] = (dr["EIContType"] != DBNull.Value ? dr["EIContType"] : DBNull.Value);
                    currentRow["Excl"] = (dr["EIExcl"] != DBNull.Value ? dr["EIExcl"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State_EIStatus"] != DBNull.Value ? dr["State_EIStatus"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["EIEffDate"] != DBNull.Value ? (Int32.TryParse(dr["EIEffDate"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["EIEffDate"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["EIRemovalDt"] != DBNull.Value ? (Int32.TryParse(dr["EIRemovalDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["EIRemovalDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["EIOLDEffDt"] != DBNull.Value ? (Int32.TryParse(dr["EIOLDEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["EIOLDEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = (dr["RptgEIEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgEIEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgEIEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgEIRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgEIRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgEIRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["RptgEIActivInd"] != DBNull.Value ? dr["RptgEIActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //IEX
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "IEX";
                    currentRow["Status"] = (dr["IEXStatus"] != DBNull.Value ? dr["IEXStatus"] : DBNull.Value);
                    currentRow["ContrType"] = (dr["IEXContTYpe"] != DBNull.Value ? dr["IEXContTYpe"] : DBNull.Value);
                    currentRow["Excl"] = DBNull.Value;
                    currentRow["State_Status"] = DBNull.Value;
                    currentRow["EffDate"] = (dr["IEXEffDate"] != DBNull.Value ? (Int32.TryParse(dr["IEXEffDate"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["IEXEffDate"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["IEXRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["IEXRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["IEXRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["IEXOLDEffDt"] != DBNull.Value ? (Int32.TryParse(dr["IEXOLDEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["IEXOLDEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = (dr["RptgIEXEfftDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgIEXEfftDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgIEXEfftDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgIEXRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgIEXRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgIEXRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["RptgIEXActivInd"] != DBNull.Value ? dr["RptgIEXActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //////OX
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "OX_UNET";
                    currentRow["Status"] = DBNull.Value;
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = DBNull.Value;
                    currentRow["State_Status"] = DBNull.Value;
                    currentRow["EffDate"] = (dr["Ox_EIEffDate"] != DBNull.Value ? (Int32.TryParse(dr["Ox_EIEffDate"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Ox_EIEffDate"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["Ox_EIRmvl_Date"] != DBNull.Value ? (Int32.TryParse(dr["Ox_EIRmvl_Date"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Ox_EIRmvl_Date"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["Ox_EIOLDEFFDt"] != DBNull.Value ? (Int32.TryParse(dr["Ox_EIOLDEFFDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["Ox_EIOLDEFFDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = (dr["RptgOxEIEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgOxEIEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgOxEIEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgOxEIRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgOxEIRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgOxEIRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["RptgOxActivInd"] != DBNull.Value ? dr["RptgOxActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    //////CS
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : DBNull.Value);
                    currentRow["LOB"] = "CS";
                    currentRow["Status"] = (dr["CSStatus"] != DBNull.Value ? dr["CSStatus"] : DBNull.Value);
                    currentRow["ContrType"] = (dr["CSContType"] != DBNull.Value ? dr["CSContType"] : DBNull.Value);
                    currentRow["Excl"] = (dr["CSExcl"] != DBNull.Value ? dr["CSExcl"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State_CSStatus"] != DBNull.Value ? dr["State_CSStatus"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["CSEffDate"] != DBNull.Value ? (Int32.TryParse(dr["CSEffDate"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["CSEffDate"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["CSRemovalDt"] != DBNull.Value ? (Int32.TryParse(dr["CSRemovalDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["CSRemovalDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["CSOLDEffDt"] != DBNull.Value ? (Int32.TryParse(dr["CSOLDEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["CSOLDEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = (dr["RptgCSEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgCSEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgCSEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgCSRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgCSRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgCSRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["RptgCSActivInd"] != DBNull.Value ? dr["RptgCSActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    ////DSNP
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : null);
                    currentRow["LOB"] = "DSNP";
                    currentRow["Status"] = (dr["DSNPStatus"] != DBNull.Value ? dr["DSNPStatus"] : DBNull.Value);
                    currentRow["ContrType"] = DBNull.Value;
                    currentRow["Excl"] = (dr["DSNPExcl"] != DBNull.Value ? dr["DSNPExcl"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State_DSNPStatus"] != DBNull.Value ? dr["State_DSNPStatus"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["DSNPEffDate"] != DBNull.Value ? (Int32.TryParse(dr["DSNPEffDate"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["DSNPEffDate"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["DSNPRemovalDt"] != DBNull.Value ? (Int32.TryParse(dr["DSNPRemovalDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["DSNPRemovalDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["DSNPOLDEffDt"] != DBNull.Value ? (Int32.TryParse(dr["DSNPOLDEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["DSNPOLDEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = (dr["RptgDSNPEffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgDSNPEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgDSNPEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgDSNPRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgDSNPRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgDSNPRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["RptgDSNPActivInd"] != DBNull.Value ? dr["RptgDSNPActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    ////MR
                    currentRow = dtFinalDataTable.NewRow();
                    foreach (string s in strCommonColumns)
                        currentRow[s] = (dr[s] != DBNull.Value ? dr[s] : null);
                    currentRow["LOB"] = "MR";
                    currentRow["Status"] = (dr["MRStatus"] != DBNull.Value ? dr["MRStatus"] : DBNull.Value);
                    currentRow["ContrType"] = (dr["MRContType"] != DBNull.Value ? dr["MRContType"] : DBNull.Value);
                    currentRow["Excl"] = (dr["MRExcl"] != DBNull.Value ? dr["MRExcl"] : DBNull.Value);
                    currentRow["State_Status"] = (dr["State_MRStatus"] != DBNull.Value ? dr["State_MRStatus"] : DBNull.Value);
                    currentRow["EffDate"] = (dr["MREffDate"] != DBNull.Value ? (Int32.TryParse(dr["MREffDate"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["MREffDate"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RemovalDt"] = (dr["MRRemovalDt"] != DBNull.Value ? (Int32.TryParse(dr["MRRemovalDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["MRRemovalDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["OLDEffDt"] = (dr["MROLDEffDt"] != DBNull.Value ? (Int32.TryParse(dr["MROLDEffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["MROLDEffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptEffDate"] = (dr["RptgMREffDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgMREffDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgMREffDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["rptRmvlDt"] = (dr["RptgMRRmvlDt"] != DBNull.Value ? (Int32.TryParse(dr["RptgMRRmvlDt"].ToString(), out intDateCheck) ? DateTime.FromOADate(double.Parse(dr["RptgMRRmvlDt"].ToString())) : (object)DBNull.Value) : (object)DBNull.Value);
                    currentRow["RptgActivInd"] = (dr["RptgMRActivInd"] != DBNull.Value ? dr["RptgMRActivInd"] : DBNull.Value);
                    currentRow["Report_Date"] = strReportDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                }

                // REPLACE ' ' WITH '_' in COLUMN NAME LOOP
                foreach (DataColumn col in dtFinalDataTable.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                }



                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "EDC_Analyzer_Stage";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE dbo." + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strConnectionString, 500);

            }
        }


        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }

    }
}
