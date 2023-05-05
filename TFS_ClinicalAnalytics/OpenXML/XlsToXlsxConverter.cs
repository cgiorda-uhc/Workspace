using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NPOI.Util;
using System.Text;
using System.Data;
using System.Data.OleDb;
using OfficeOpenXml;

namespace OpenXMLExcel
{
        public class XLSToXLSXConverter
        {

            public static string convertXLStoXLSX(string strFile)
            {
                string strNewFile = strFile + "x";

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage epackage = new ExcelPackage())
                {
                    ExcelWorksheet excel = epackage.Workbook.Worksheets.Add("ExcelTabName");
                    DataSet ds = ReadExcelFile(strFile);
                    DataTable dtbl = ds.Tables[0];
                    excel.Cells["A1"].LoadFromDataTable(dtbl, true);
                    System.IO.FileInfo file = new System.IO.FileInfo(strNewFile);
                    epackage.SaveAs(file);
                }


                return strNewFile;


            }


            private static string GetConnectionString(string strFile)
            {
                Dictionary<string, string> props = new Dictionary<string, string>();

                // XLSX - Excel 2007, 2010, 2012, 2013
                props["Provider"] = "Microsoft.ACE.OLEDB.12.0;";
                props["Extended Properties"] = "Excel 12.0 XML";
                props["Data Source"] = strFile;

                // XLS - Excel 2003 and Older
                //props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
                //props["Extended Properties"] = "Excel 8.0";
                //props["Data Source"] = "C:\\MyExcel.xls";

                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, string> prop in props)
                {
                    sb.Append(prop.Key);
                    sb.Append('=');
                    sb.Append(prop.Value);
                    sb.Append(';');
                }

                return sb.ToString();
            }

            private static DataSet ReadExcelFile(string strFile)
            {
                DataSet ds = new DataSet();

                string connectionString = GetConnectionString(strFile);

                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // Get all Sheets in Excel File
                    DataTable dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    // Loop through all Sheets to get data
                    foreach (DataRow dr in dtSheet.Rows)
                    {
                        string sheetName = dr["TABLE_NAME"].ToString();

                        //if (!sheetName.EndsWith("$"))
                        //    continue;

                        // Get all rows from the Sheet
                        cmd.CommandText = "SELECT * FROM [" + sheetName + "]";

                        DataTable dt = new DataTable();
                        dt.TableName = sheetName;

                        OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                        da.Fill(dt);

                        ds.Tables.Add(dt);
                    }

                    cmd = null;
                    conn.Close();
                }

                return ds;
            }
        }
    }
