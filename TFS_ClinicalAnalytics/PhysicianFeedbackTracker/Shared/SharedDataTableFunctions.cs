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

        public static DataTable getDataTableFromExcel(string strFilePath, string sheetName, string strCSVcolumns = null)
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
            OleDbCommand ocmd = new OleDbCommand("Select * From [" + sheetName + "$]", conn);
            conn.Open();

            OleDbDataAdapter sda = new OleDbDataAdapter(ocmd);
            DataTable data = new DataTable();
            try
            {
                sda.Fill(data);

                data = data.CopyWithoutEmptyRows();

                conn.Close();

                if (strCSVcolumns != null)
                {
                    System.Data.DataView view = new System.Data.DataView(data);
                    data = view.ToTable("Selected", false, strCSVcolumns.Split(','));
                }
            }
            catch(Exception ex)
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




        public static DataTable compareDataTables(DataTable dtMain, DataTable dtTest, bool blValidateColumns = false)
        {
            if (dtMain == null || dtTest == null)
                return null;
            
            //DataTable dtResults = dtMain.CopyWithoutEmptyRows();
            DataTable dtResults = dtMain.Clone();

            const string strErrorColumn = "Error_Flags";

            int intRowCounter = 0;
            Int16 intColumnCounter = 0;

            string strMainValue = null;
            string strTestValue = null;

            StringBuilder sbError = new StringBuilder();
            DataRow drTmp;
            DataColumn colTmp;

            bool blRowMismatch = false;
            if(dtMain.Rows.Count != dtTest.Rows.Count)
            {
                sbError.Append("Row counts are inconsistent " + dtMain.Rows.Count + " vs. " + dtTest.Rows.Count + Environment.NewLine);
                blRowMismatch = true;
            }



            foreach (DataRow row in dtMain.Rows)
            {

                drTmp = dtResults.NewRow();


                foreach (DataColumn col in dtMain.Columns)
                {
                
                    strMainValue = (row[col.ColumnName] != DBNull.Value ? row[col.ColumnName].ToString() : null);


                    if(!blRowMismatch)//MISMATCH ON ROWS SO DONT BOTHER WITH OTHER CHECKS
                    {
                        if (dtTest.Columns.Contains(col.ColumnName))
                        {
                            if (dtTest.Rows.Count > intRowCounter)
                            {
                                strTestValue = (dtTest.Rows[intRowCounter][col.ColumnName] != DBNull.Value ? dtTest.Rows[intRowCounter][col.ColumnName].ToString() : null);
                                if (!string.Equals(strMainValue, strTestValue, StringComparison.OrdinalIgnoreCase))
                                {
                                    sbError.Append("Row # " + (intRowCounter + 3) + ", Column '" + col.ColumnName + "' value mismatch {" + strMainValue + "} vs {" + strTestValue + "}" + Environment.NewLine);
                                }
                            }
                            else
                            {
                                sbError.Append("There is no Row # " + (intRowCounter + 3) + " in comparison table" + Environment.NewLine);
                            }


                        }
                        else
                        {
                            sbError.Append("Column '" + col.ColumnName + "' does not exist in comparison table" + Environment.NewLine);
                        }
                    }

                    if (strMainValue == null)
                        drTmp[col.ColumnName] = DBNull.Value;
                    else
                        drTmp[col.ColumnName] = strMainValue;

                    intColumnCounter++;
                }


                if(sbError.Length > 0)
                {
                    if (!dtResults.Columns.Contains(strErrorColumn))
                    {
                        colTmp = dtResults.Columns.Add(strErrorColumn);
                        colTmp.SetOrdinal(0);
                    }

                    if (!drTmp.Table.Columns.Contains(strErrorColumn))
                    {
                        colTmp = drTmp.Table.Columns.Add(strErrorColumn);
                        colTmp.SetOrdinal(0);
                    }

                    drTmp[strErrorColumn] = sbError.ToString();

                    if (!blRowMismatch)//WELL REUSE THIS MESSAGE IF ROW COUNTS DONT MATCH
                        sbError.Remove(0, sbError.Length);
                }

                dtResults.Rows.Add(drTmp);

                intRowCounter++;
            }




            return dtResults;



        }



    }

}