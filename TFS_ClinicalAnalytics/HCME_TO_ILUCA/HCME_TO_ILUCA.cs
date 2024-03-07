using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace HCME_TO_ILUCA
{
    class HCME_TO_ILUCA
    {

        private static string excelFilePath = ConfigurationManager.AppSettings["ExcelFilePath"];
        private static string excelConnectionString = ConfigurationManager.AppSettings["ExcelXConnectionString"];
        private static string strConnectionString = ConfigurationManager.AppSettings["DatabaseConnectionString"];


        static void Main(string[] args)
        {
            DataTable dtFinal = getDataTableFromExcel(excelFilePath, "sheetname");
        }









        private static DataTable getDataTableFromExcel(string strFilePath, string sheetName)
        {

            OleDbConnection conn = new OleDbConnection(excelConnectionString.Replace("{$filePath}", strFilePath));
            //OleDbCommand oconn = new OleDbCommand("Select "+ strCSVcolumns + " From [" + sheetName + "$]", conn);
            OleDbCommand ocmd = new OleDbCommand("Select * From [" + sheetName + "$]", conn);
            conn.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(ocmd);
            DataTable data = new DataTable();
            try
            {
                sda.Fill(data);

                data = CopyWithoutEmptyRows(data);

                conn.Close();
            }
            catch (Exception ex)
            {
                data = null;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
                conn.Dispose();
                conn = null;
                sda.Dispose();
                sda = null;
                ocmd.Dispose();
                ocmd = null;

            }


            return data;

        }



        private static DataTable CopyWithoutEmptyRows(DataTable dt)
        {
            bool blIsEmpty = true;

            int i = 0;

            DataTable dtNew = dt.Clone();

            foreach (DataRow dr in dt.Rows)
            {
                blIsEmpty = true;

                if (dr != null)
                {
                    foreach (var value in dr.ItemArray)
                    {
                        if (value != DBNull.Value)
                        {
                            blIsEmpty = false;
                            break;
                        }
                    }
                }

                if (i == 197)
                {
                    string s = "";
                }

                if (!blIsEmpty)
                {
                    dtNew.ImportRow(dr);
                }
                //dt.Rows.Remove(dr);

                i++;
            }

            return dtNew.Copy();

        }






















    }






}
