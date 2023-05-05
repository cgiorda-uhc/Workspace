using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHRQ_Code_Parser
{
    class AHRQ_Code_Parser
    {
        static void Main(string[] args)
        {
            string strILUCAConnectionString = "data source=IL_UCA;server=wn000005325;Persist Security Info=True;database=IL_UCA;Integrated Security=SSPI;connect timeout=300000;";
            string strFile = @"C:\Users\cgiorda\Desktop\PSI_Appendix_G.xlsx";
            string strSheetName = "Sheet1";
            SpreadsheetDocument wbCurrentExcelFile;
            wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
            DataTable dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetName, 1, 2);
            DataTable dtFinalDataTable = new DataTable();
            DataRow currentRow = null;
            dtFinalDataTable.Columns.Add("TRAUMID", typeof(System.String));
            dtFinalDataTable.Columns.Add("TRAUMDESC", typeof(System.String));
            
            bool isTraumaId = true;
            foreach (DataRow dr in dtCurrentDataTable.Rows)
            {
                if ((dr[0] + "").Contains("July 2021") || (dr[0] + "").Contains("AHRQ QI™ ICD") || (dr[0] + "").Contains("Patient Safety Indicators Appendices") || (dr[0] + "").Contains("www.qualityindicators.ahrq.gov"))
                    continue;


                if(isTraumaId)
                  currentRow = dtFinalDataTable.NewRow();

                if (isTraumaId)
                {

                    currentRow["TRAUMID"] = (dr[0] != DBNull.Value && !(dr[0] + "").Trim().Equals("") ? dr[0] : DBNull.Value);
                    isTraumaId = false;
                }
                else 
                {
                    currentRow["TRAUMDESC"] = (dr[0] != DBNull.Value && !(dr[0] + "").Trim().Equals("") ? dr[0] : DBNull.Value);
                    isTraumaId = true;
                }

                if (isTraumaId)
                    dtFinalDataTable.Rows.Add(currentRow);
            }

            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
            dtFinalDataTable.TableName = "AHRQ_Trauma_Codes";
            DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
            //DBConnection32.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE dbo."+ dtFinalDataTable.TableName + ";");
            DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

        }

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }
    }
}
