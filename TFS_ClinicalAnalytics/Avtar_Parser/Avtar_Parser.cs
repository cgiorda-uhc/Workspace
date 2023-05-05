using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
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

namespace Avtar_Parser
{
    class Avtar_Parser
    {
        static void Main(string[] args)
        {

            Console.WriteLine("AVTAR Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["Final_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            string[] strFoldersArr = new string[] { "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");

            string strFileName = null;
            string[] strFileNameArr = null;
            string strFileDate = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strSummaryofLOB = null;
            string[] files;



            DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[AVTAR_Data]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;

            
            string strTableName = "stg.AVTAR_Data";
            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");





            //RESET FINAL TABLE!!!
            foreach (string strFolder in strFoldersArr)
            {

                if(strFolder == "Monthly_CnS")
                    strSheetname = "CaseDetailExtract";
                else if (strFolder == "Monthly_EnI" || strFolder == "Monthly_MnR")
                    strSheetname = "CaseLevelDetail";

                Console.Write("\rProcessing " + strFolder);
                files = Directory.GetFiles(strFileFolderPath + "\\" + strFolder, "*AVTAR_Detail_AlleviCore*.xlsb", SearchOption.AllDirectories);
                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    strFileName = Path.GetFileName(strFile);

                    if (strFileName.StartsWith("~") || strFileName.Contains("2019") || dtFilesCaptured.Select("[file_name]='"+ strFileName + "'").Count() > 0)
                    {
                        intFileCnt++;
                        continue;
                    }



                    strFileNameArr = strFileName.Split('_');
                    strFileDate = strFileNameArr[strFileNameArr.Length - 1].Replace(".xlsb", "");
                    strMonth = strFileDate.Substring(4,2);
                    strYear = strFileDate.Substring(0, 4);
                    strReportType = "AVTAR";
                    strFilePath = Path.GetDirectoryName(strFile);

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
                    Console.Write("\rFile to DataTable");
                    strSummaryofLOB = strFolder.Split('_')[1];


                    //ONLY ONE TIME PER LOOP
                    dtFinalDataTable = dtCurrentDataTable.Clone();
                    dtFinalDataTable.Columns.Add("Summary_of_Lob", typeof(String));
                    dtFinalDataTable.Columns.Add("file_month", typeof(String));
                    dtFinalDataTable.Columns.Add("file_year", typeof(String));
                    dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
                    dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
                    dtFinalDataTable.Columns.Add("file_name", typeof(String));
                    dtFinalDataTable.Columns.Add("file_path", typeof(String));
                    dtFinalDataTable.Columns.Add("report_type", typeof(String));

                    dtFinalDataTable.TableName = strTableName;
                    intRowCnt = 1;
                    foreach (DataRow d in dtCurrentDataTable.Rows)
                    {

                        Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                        currentRow = dtFinalDataTable.NewRow();
                        foreach (DataColumn c in dtCurrentDataTable.Columns)
                        {
                            //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                            currentRow[c.ColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                        }

                        currentRow["Summary_of_Lob"] = strSummaryofLOB;
                        currentRow["file_month"] = strMonth;
                        currentRow["file_year"] = strYear;
                        currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                        currentRow["sheet_name"] = strSheetname;
                        currentRow["file_name"] = strFileName;
                        currentRow["file_path"] = strFilePath;
                        currentRow["report_type"] = strReportType;
                        dtFinalDataTable.Rows.Add(currentRow);
                        intRowCnt++;
                    }
                    currentRow = null;
                    dtCurrentDataTable = null;

                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                    
                    DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                    DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);


                    dtFinalDataTable = null;
                    GC.Collect(2, GCCollectionMode.Forced);



                    intFileCnt++;

                }
            }

           

        }

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }
    }
}
