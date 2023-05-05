using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EviCore_MHP_Parser
{
    class EviCore_MHP_Parser
    {
        static void Main(string[] args)
        {
            getCaseLevelData();
        }


        private static void getCaseLevelData()
        {
            Console.WriteLine("EviCore MHP Case Level Parser");
            string strFileFolderPath = @"C:\Users\cgiorda\Desktop\MPH Reporting\RE MHP Report Request for MA - due 3232022";
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[MHP_CaseLevelDetail]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            dtFinalDataTable = new DataTable();




            dtFinalDataTable.Columns.Add("Member_State", typeof(String));
            dtFinalDataTable.Columns.Add("Jurisdiction_State", typeof(String));
            dtFinalDataTable.Columns.Add("EpisodeID", typeof(String));
            dtFinalDataTable.Columns.Add("EpisodeDate", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Initial_Decision_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Initial_Decision_Auth_Status_Desc", typeof(String));
            dtFinalDataTable.Columns.Add("Current_Auth_Status_Desc", typeof(String));
            dtFinalDataTable.Columns.Add("Priority_Desc", typeof(String));
            dtFinalDataTable.Columns.Add("ProgramType", typeof(String));
            dtFinalDataTable.Columns.Add("CPTCode", typeof(String));
            dtFinalDataTable.Columns.Add("CPT_Description", typeof(String));
            dtFinalDataTable.Columns.Add("Funding_Type", typeof(String));
            dtFinalDataTable.Columns.Add("Denial_Reason", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_ID", typeof(Int64));
            dtFinalDataTable.Columns.Add("planType", typeof(String));
            dtFinalDataTable.Columns.Add("Inpatient_OutPatient", typeof(String));
            dtFinalDataTable.Columns.Add("Recon_Determination_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Overturn_Determination_status", typeof(bool));
            dtFinalDataTable.Columns.Add("Upheld_Determination_Status", typeof(bool));
            dtFinalDataTable.Columns.Add("Type", typeof(String));
            dtFinalDataTable.Columns.Add("Went_to_K", typeof(DateTime));
            dtFinalDataTable.Columns.Add("GroupNumber", typeof(String));
            dtFinalDataTable.Columns.Add("Legal_Entity", typeof(int));
            dtFinalDataTable.Columns.Add("Legal_Entity_Name", typeof(String));
            dtFinalDataTable.Columns.Add("LineofBusiness", typeof(String));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.TableName = "stg.MHP_CaseLevelDetail";



            string strFileName = null;
            string strFilePath = null;
            string strSheetname = "Case Level";

            string[] files;
            files = Directory.GetFiles(strFileFolderPath, "*CaseLevel.xlsx", SearchOption.AllDirectories);
            int intFileCnt = 1;
            int intRowCnt = 1;
            foreach (string strFile in files)
            {
                strFileName = Path.GetFileName(strFile);
                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0 || strFileName.StartsWith("~"))
                {
                    intFileCnt++;
                    continue;
                }

                strFilePath = Path.GetDirectoryName(strFile);


                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
                foreach (DataColumn col in dtCurrentDataTable.Columns)
                    col.ColumnName = col.ColumnName.Trim();

                intRowCnt = 1;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    currentRow = dtFinalDataTable.NewRow();



                  // string s = dr["Initial Decision Auth Status Desc"].ToString();



                    currentRow["Member_State"] = dr["Member State"];
                    currentRow["Jurisdiction_State"] = dr["Jurisdiction State"];
                    currentRow["EpisodeID"] = dr["EpisodeID"];
                    currentRow["EpisodeDate"] = (dr["EpisodeDate"] != DBNull.Value ? DateTime.Parse(dr["EpisodeDate"].ToString()) : (object)DBNull.Value);
                    currentRow["Initial_Decision_Date"] = (dr["Initial_Decision_Date"] != DBNull.Value ? DateTime.Parse(dr["Initial_Decision_Date"].ToString()) : (object)DBNull.Value);
                    currentRow["Initial_Decision_Auth_Status_Desc"] = dr["Initial Decision Auth Status Desc"];
                    currentRow["Current_Auth_Status_Desc"] = dr["Current Auth Status Desc"];
                    currentRow["Priority_Desc"] = dr["Priority Desc"];
                    currentRow["ProgramType"] = dr["ProgramType"];
                    currentRow["CPTCode"] = dr["CPTCode"];
                    currentRow["CPT_Description"] = dr["CPT Description"];
                    currentRow["Funding_Type"] = dr["Funding Type"];
                    currentRow["Denial_Reason"] = dr["Denial Reason"];
                    currentRow["Patient_ID"] = dr["Patient ID"];
                    currentRow["planType"] = dr["planType"];
                    currentRow["Inpatient_OutPatient"] = dr["Inpatient_OutPatient"];
                    currentRow["Recon_Determination_Date"] = (dr["Recon_Determination_Date"] != DBNull.Value ? DateTime.Parse(dr["Recon_Determination_Date"].ToString()) : (object)DBNull.Value);
                    currentRow["Overturn_Determination_status"] = (dr["Overturn Determination status"] == DBNull.Value ? (object) DBNull.Value : (dr["Overturn Determination status"].ToString() == "Yes"? true : false));
                    currentRow["Upheld_Determination_Status"] =  (dr["Upheld Determination Status"] == DBNull.Value ? (object)DBNull.Value : (dr["Upheld Determination Status"].ToString() == "Yes" ? true : false));
                    currentRow["Type"] = dr["Type"];
                    currentRow["Went_to_K"] = (dr["Went_to_K"] != DBNull.Value ? DateTime.Parse(dr["Went_to_K"].ToString()) : (object)DBNull.Value);
                    currentRow["GroupNumber"] = dr["GroupNumber"];
                    currentRow["Legal_Entity"] = dr["Legal_Entity"];
                    currentRow["Legal_Entity_Name"] = dr["Legal_Entity_Name"];
                    currentRow["LineofBusiness"] = dr["LineofBusiness"];





                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Category Analytics\Radiology (Laurie G)\MHP Reporting\RE MHP Report Request for MA - due 3232022";

                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;

                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                DataTable dtTotalsDataTable = null;
                DataRow totalsRow;
                dtTotalsDataTable = new DataTable();
                dtTotalsDataTable.Columns.Add("Utilization_Review", typeof(String));
                dtTotalsDataTable.Columns.Add("Pended_prior_year", typeof(int));
                dtTotalsDataTable.Columns.Add("Filed_year_report", typeof(int));
                dtTotalsDataTable.Columns.Add("Closed_reversed", typeof(int));
                dtTotalsDataTable.Columns.Add("Approval_Closed", typeof(int));
                dtTotalsDataTable.Columns.Add("Closed_upheld", typeof(int));
                dtTotalsDataTable.Columns.Add("Denial_Count", typeof(int));
                dtTotalsDataTable.Columns.Add("Total_closed", typeof(int));
                dtTotalsDataTable.Columns.Add("Externals", typeof(String));
                dtTotalsDataTable.Columns.Add("Pended_current_year", typeof(int));
                dtTotalsDataTable.Columns.Add("sheet_name", typeof(String));
                dtTotalsDataTable.Columns.Add("file_name", typeof(String));
                dtTotalsDataTable.Columns.Add("file_path", typeof(String));
                dtTotalsDataTable.TableName = "stg.MHP_CaseLevelSummary";


                strSheetname = "Summary";
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

                intRowCnt = 1;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    totalsRow = dtTotalsDataTable.NewRow();


                    totalsRow["Utilization_Review"] = dr["Utilization Review"];
                    totalsRow["Pended_prior_year"] = dr["Pended as of 12/31 prior year"];
                    totalsRow["Filed_year_report"] = dr["Filed in year of report"];
                    totalsRow["Closed_reversed"] = dr["Closed and reversed  (whole or part)"];
                    totalsRow["Approval_Closed"] = dr["Approval Closed"];
                    totalsRow["Closed_upheld"] = dr["Closed and upheld"];
                    totalsRow["Denial_Count"] = dr["Denial Count"];
                    totalsRow["Total_closed"] = dr["Total # closed"];
                    totalsRow["Externals"] = dr["Externals"];
                    totalsRow["Pended_current_year"] = dr["Pended as of 12/31 current year"];
                    //currentRow["Initial_Decision_Date"] = (dr["Initial_Decision_Date"] != DBNull.Value ? DateTime.Parse(dr["Initial_Decision_Date"].ToString()) : (object)DBNull.Value);


                    totalsRow["sheet_name"] = strSheetname;
                    totalsRow["file_name"] = strFileName;
                    totalsRow["file_path"] = @"\\nasv0048\ucs_ca\PHS_DATA_NEW\Home Directory - Category Analytics\Radiology (Laurie G)\MHP Reporting\RE MHP Report Request for MA - due 3232022";

                    dtTotalsDataTable.Rows.Add(totalsRow);
                    intRowCnt++;
                }
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";


                DBConnection64.SQLServerBulkImportDT(dtTotalsDataTable, strILUCAConnectionString, 10000);

                dtFinalDataTable.Rows.Clear();
                GC.Collect(2, GCCollectionMode.Forced);


                intFileCnt++;
            }
        }

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }


    }
}
