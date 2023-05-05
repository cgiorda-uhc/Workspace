using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Odbc;
using Newtonsoft.Json;

namespace CPR_Dashboard_Parser
{
    class CPR_Dashboard_Parser
    {
        static void Main(string[] args)
        {


            //https://uhgdwaas.east-us-2.azure.snowflakecomputing.com/console#/internal/worksheet

            /*Select * from OHBI_DEV_OHBI_DB.UHC_CLINHEALTHPRGM.CUSTOMER

            Select * from OHBI_PRD_OHBI_DB.UHC_CLINHEALTHPRGM.CUSTOMER
            */


            //sbSQL.Append("CREATE TABLE [stg].[" + strFileName.Replace(".csv", "") + "](");


            //foreach (string s in csvArray)
            //{
            //    sbSQL.Append(" [" + s + "] [varchar](255) NULL, ");


            //}

            //sbSQL.Append(" [file_month] [varchar](2) NULL,[file_year] [varchar](4) NULL,[file_date] [date] NULL,[file_name] [varchar](255) NULL) ON [PRIMARY]");

            List<string> strLstColumns = null;
            List<string> strLstTables = new List<string>();
            //strLstTables.Add("KID_UHC_CASE_FACT_OGA");
            // strLstTables.Add("EXT_ESRD_WATCHLIST");
            strLstTables.Add("EXT_CMS_2728_REPORT");
            //strLstTables.Add("EXT_CMS_2728_REPORT");
            //strLstTables.Add("KID_UHC_CASE_FACT");

            strLstTables.Add("KID_UHC_CASE_ONGOING_ENGAGEMENT_FACT");


            //strLstTables.Add("KID_UHC_CASE_FACT");

            Console.WriteLine("CMS_Data_Parser");
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strSnowflake_ConnectionString = ConfigurationManager.AppSettings["SnowflakeSSH2"];
            string strSQL = null;
            StringBuilder sbSQL = new StringBuilder();
            DataTable dt = null;
            string strColType = null;
            string strNewType = null;
            int intColLength;
            string strSchemaName = "stg";
            foreach (string strTable in strLstTables)
            {
                //string strSQL = "Select top 10 * from SNOWFLAKE_SAMPLE_DATA.TPCH_SF100.CUSTOMER";
                //string strSQL = "Select C_CUSTKEY, C_NAME, C_ADDRESS, C_NATIONKEY, C_PHONE, C_ACCTBAL, C_MKTSEGMENT, C_COMMENT from OHBI_DEV_OHBI_DB.UHC_CLINHEALTHPRGM.CUSTOMER";
                //strSQL = "show columns in table OHBI_PRD_OHBI_DB.UHC_CLINHEALTHPRGM." + strTable + ";";
                strSQL = "show columns in table OHBI_PRD_CONSUME_DB.UHC_CLINHEALTHPRGM." + strTable + ";";
                dt = DBConnection64.getODBCDataTable(strSnowflake_ConnectionString, strSQL);
                sbSQL.Append("CREATE TABLE [" + strSchemaName + "].[" + strTable + "](");
                foreach (DataRow dr in dt.Rows)
                {
                    var colName = dr["column_name"];
                    dynamic dynObj = JsonConvert.DeserializeObject(dr["data_type"].ToString());
                    var colType = (dynObj.type == "FIXED" ? "INT" : dynObj.type == "TEXT" ? "VARCHAR (255)" : dynObj.type == "TIMESTAMP_NTZ" || dynObj.type == "DATE" ? "DATE" : "VARCHAR (255)");

                    //var colLength = dynObj.precision;


                    sbSQL.Append(" [" + colName + "] "+ colType + " NULL,");
                }
                //CREATE NEW TMP TABLE
                DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + strSchemaName + "' AND name like '" + strTable + "') DROP TABLE " + strSchemaName + "." + strTable + ";" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY];");
                sbSQL.Remove(0, sbSQL.Length);



                //strSQL = "select * from OHBI_PRD_OHBI_DB.UHC_CLINHEALTHPRGM." + strTable;
                strSQL = "select * from OHBI_PRD_CONSUME_DB.UHC_CLINHEALTHPRGM." + strTable;
                dt = DBConnection64.getODBCDataTable(strSnowflake_ConnectionString, strSQL);
                dt.TableName = strSchemaName + "." + strTable;
                strLstColumns = dt.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();



                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dt.Rows.Count) + " into Staging...";
                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection64.SQLServerBulkImportDT(dt, strILUCAConnectionString, 10000);



                dt = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "SELECT UPPER(c.name) as [Column_Name] FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('" + strSchemaName + "." + strTable + "') AND  t.name = 'VARCHAR'");
                strLstColumns = dt.AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();

                foreach (string strColumn in strLstColumns)
                {
                    sbSQL.Append("SELECT ColumnName, MAX(ColumnLength) as ColumnLength FROM (");
                    sbSQL.Append("SELECT  '" + strColumn + "' as ColumnName, ");
                    sbSQL.Append("LEN([" + strColumn + "])  AS ColumnLength ");
                    sbSQL.Append("From [" + strSchemaName + "].[" + strTable + "] ) tmp ");
                    sbSQL.Append("GROUP BY ColumnName ");
                    sbSQL.Append("UNION ALL ");

                }
                dt = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, sbSQL.ToString().TrimEnd('U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '));
                sbSQL.Remove(0, sbSQL.Length);

                foreach (DataRow dr in dt.Rows)
                {
                    var colLen = (string.IsNullOrEmpty(dr["ColumnLength"] + "") ? 5 : dr["ColumnLength"]);



                    sbSQL.Append("ALTER TABLE [" + strSchemaName + "].[" + strTable + "] ALTER COLUMN [" + dr["ColumnName"] + "] VARCHAR(" + colLen + ");");
                }
                DBConnection64.ExecuteMSSQL(strILUCAConnectionString,sbSQL.ToString());
                sbSQL.Remove(0, sbSQL.Length);

                ////CREATE FINAL TABLE USING LENGTHS AND TYPES DETERMINED ABOVE
                //sbSQL.Append("CREATE TABLE [" + strSchemaName + "].[" + strTable + "](");

                //foreach (DataRow dr in dt.Rows) 
                //{
                //    strColType = dr["ColumnType"].ToString().Split('-')[1];
                //    intColLength = int.Parse((string.IsNullOrEmpty(dr["ColumnLength"] + "") ? "255" : dr["ColumnLength"].ToString()));

                //    if (intColLength == 0)
                //        intColLength = 255;


                //    if (strColType == "CHAR" || strColType == "VARCHAR")
                //    {
                //        strNewType = strColType + "(" + intColLength + ")";

                //    }
                //    else if (strColType == "INT")
                //    {
                //        if (intColLength < 5)
                //        {
                //            strNewType = "SMALLINT";
                //        }
                //        else if (intColLength < 10)
                //        {
                //            strNewType = "INT";
                //        }
                //        else if (intColLength < 16)
                //        {
                //            strNewType = "BIGINT";
                //        }
                //        else
                //        {
                //            strNewType = "VARCHAR(" + intColLength + ")";
                //        }
                //    }
                //    else
                //    {
                //        strNewType = strColType;


                //    }

                //    sbSQL.Append(" [" + dr["ColumnName"] + "] " + strNewType + " NULL,");

                //}
                ////DROP TABLE IF ALREAY EXISTS - ERROR FLAG???
                //DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + strSchemaName + "' AND name like '" + strTable + "') DROP TABLE " + strSchemaName + "." + strTable+ ";" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY]; ");
                //sbSQL.Remove(0, sbSQL.Length);


                ////TRANSFER DATA FROM TMP TO FINAL TABLE
                //foreach (string strColumn in strLstColumns)
                //{
                //    sbSQL.Append("[" + strColumn + "],");
                //}
                //DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "INSERT INTO [" + strSchemaName + "].[" + strTable + "] (" + sbSQL.ToString().TrimEnd(',') + ") SELECT " + sbSQL.ToString().TrimEnd(',') + " FROM [" + strSchemaName + "].[" + strTable + "_TMP]; DROP TABLE  [" + strSchemaName + "].[" + strTable + "_TMP];");
                //sbSQL.Remove(0, sbSQL.Length);

            }








            //blUpdated = true;


            //DataTable dt = DBConnection32.GetSnowflakeDataTable(strSnowflake_ConnectionString, strSQL);

            // DataTable dt = DBConnection32.GetSnowflakeDataTable(strSnowflake_ConnectionString, strSQL);


            //string strSQL = "SELECT TOP 100 * FROM [IL_UCA].[dbo].[ACO_MR]";
            //DataTable dtFinalDataTable = new DataTable();
            //dtFinalDataTable = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, strSQL);


            //string strCreateTableSQL = HelperFunctions.HelperFunctions.CreateTableSQLFromDataTable("CSG_Test", dtFinalDataTable);

            //strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
            //dtFinalDataTable.TableName = "CEP_QPR_SysId";
            //DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE dbo." + dtFinalDataTable.TableName + ";");
            //DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);


        }



        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }
    }
}
