using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PhysicianFeedbackTracker
{
    static class SharedDataTableFunctions
    {

        public static DataTable addToNameValueDatatable(string strName, string strValue, DataTable dt)
        {

            DataRow dataRow = dt.NewRow();
            dataRow["name"] = strName;
            dataRow["value"] = strValue;

            dt.Rows.InsertAt(dataRow, 0);

            return dt;

        }


        public static DataTable filterNameValueDatatable(string strFilter, DataTable dt)
        {


            DataRow[] drTmp = dt.Select("filter = '" + strFilter + "'");
            if (drTmp.Length != 0)
                return drTmp.CopyToDataTable();
            else  //NOT IN CACHE
                return new DataTable();

        }

        public static DataTable getDataTableFromExcel(string strFilePath, string sheetName, string strCSVcolumns)
        {
            string excelConnectionString = "";

            if (Path.GetExtension(strFilePath) == ".xlsx")
            {
                excelConnectionString = GlobalObjects.strExcelXConnectionString;
            }
            else if (Path.GetExtension(strFilePath) == ".xls")
            {
                excelConnectionString = GlobalObjects.strExcelConnectionString;
            }

            OleDbConnection conn = new OleDbConnection(excelConnectionString.Replace("{$filePath}", strFilePath));
            //OleDbCommand oconn = new OleDbCommand("Select "+ strCSVcolumns + " From [" + sheetName + "$]", conn);
            OleDbCommand oconn = new OleDbCommand("Select * From [" + sheetName + "$]", conn);
            conn.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(oconn);
            DataTable data = new DataTable();
            sda.Fill(data);
            conn.Close();



            System.Data.DataView view = new System.Data.DataView(data);
            data = view.ToTable("Selected", false, strCSVcolumns.Split(','));

            return data;

        }


        public static string getConcatenatedListFromDatatable(DataTable dt, string strColumnName, string strDelimiter, Int16 intDelimitCnt)
        {
            StringBuilder sbValues = new StringBuilder();
            Int16 intRowCounter = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[strColumnName] != DBNull.Value)
                {
                    sbValues.Append(dr[strColumnName].ToString());
                    if (intRowCounter < (dt.Rows.Count- 1))
                    {
                        for (int i = 1; i <= intDelimitCnt; i++)
                        {
                            sbValues.Append(strDelimiter);
                        }
                    }
                    
                }
                intRowCounter++;
            }

            return sbValues.ToString();
        }
    }

}