using DBConnectionLibrary;
using Serilog;
using Microsoft.Extensions.Logging;
using SharedFunctionsLibrary;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using ProjectManagerLibrary.Projects;

namespace ProjectManagerLibrary.Concrete
{
    public class DelimitedParser :  IDelimitedParser
    {


        public DelimitedParser()
        {
        }


        private string _strTableName = "";
        public string TableName   // property
        {
            get { return _strTableName; }
            set { _strTableName = value; } //SET IN TESTING ONLY!!!!!
        }

        public long parseDelimitedFiles(string strPathOrFile, char chrDelimiter, string strConnectionString, string strSchemaName, int intBulkSize = 10000, int intDelay = 1000)
        {
            //await Task.Delay(intDelay);
            var stopwatch = Stopwatch.StartNew();

            //LOGGING
            Log.Information("Starting file delimiting processs");
           

            List<string>? strLstColumnNames = null;

            StreamReader? csvreader = null;
            DataTable dtTransfer = new DataTable();
            DataRow? drCurrent = null;
            string[] csvArray;
            string? strInputLine = "";
            int? intTotal = 0;
            int? intCnt = 0;
            string strSQL = "";

            //GET LIST OF FILE(S)
            List<string> strLstFiles = getFiles(strPathOrFile);
            //LOGGING
            Log.Information("Found " + strLstFiles.Count + " files to process"); 
            foreach (string strFile in strLstFiles)
            {
                //LOGGING
                 Log.Information("Processing " + strFile); 


                //STR EXTENSION TO TRANSLATE FILENAME TO TABLENAME
                _strTableName = strFile.getSafeFileName().ToUpper();

                csvreader = new StreamReader(strFile);
                while ((strInputLine = csvreader.ReadLine()) != null)
                {
                    csvArray = strInputLine.Split(new char[] { chrDelimiter });
                    //FIRST PASS ONLY GETS COLUMNS AND CREATES TABLE SQL
                    if (strLstColumnNames == null)
                    {
                        //LOGGING
                         Log.Information(csvArray.Count() + " columns found for " + _strTableName);
       
                        intTotal = TotalLines(strFile);

                        //LOGGING
                         Log.Information(intTotal + " rows found for " + _strTableName);

                        strLstColumnNames = new List<string>();
                        //GET AND CLEAN COLUMN NAMES FOR TABLE
                        foreach (string c in csvArray)
                        {
                            var colName = c.getSafeFileName();
                            strLstColumnNames.Add(colName.ToUpper());
                        }

                        //LOGGING
                         Log.Information("Generating tmp table to store data for " + _strTableName); 
                        //SQL FOR TMP TABLE TO STORE ALL VALUES A VARCHAR(MAX)
                        strSQL = getTmpTableGeneratingSQL(strSchemaName, strLstColumnNames);
                        //CREATE TMP TABLE AND COLLECT NEW DB TABLE FOR BULK TRANSFERS
                        dtTransfer = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                        //GOT COLUMNS, CREATED TMP TABLE FOR FIRST PASS
                        continue;
                    }
                    //CLONE ROW FOR TRANSFER
                    drCurrent = dtTransfer.NewRow();
                    //POPULATE ALL COLUMNS FOR CURRENT ROW
                    for (int i = 0; i < strLstColumnNames.Count; i++)
                    {
                        drCurrent[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

                    }
                    dtTransfer.Rows.Add(drCurrent);

                    if (dtTransfer.Rows.Count == intBulkSize) //intBulkSize = 10000 DEFAULT
                    {
                        loadToDatabase(dtTransfer, strConnectionString, intBulkSize);
                        dtTransfer.Rows.Clear();
                    }


                    //LOGGING
                     Log.Information("Processing row " + intCnt + " out of " + intTotal + " rows for " + _strTableName); 
                    intCnt++;
                }

                //CATCH REST OF UPLOADS OUTSIDE CSV LOOP
                if (dtTransfer.Rows.Count > 0)
                    loadToDatabase(dtTransfer, strConnectionString, intBulkSize);

                //LOGGING
                 Log.Information("Running post process to determine types for " + _strTableName); 
                //RUN POST PROCESS
                strSQL = getTypeCheckingSQL(strSchemaName, strLstColumnNames);
                //GET ACTUAL LENGTHS AND TYPES FOR FINAL TABLE
                dtTransfer = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);

                //LOGGING
                 Log.Information("Generating final table for " + _strTableName); 
                //GET SQL FOR FINAL TABLE WITH PROPER TYPES
                strSQL = getFinalTableGeneratingSQL(dtTransfer, strSchemaName);
                //CREATE FINAL TABLE
                DBConnection.ExecuteMSSQL(strConnectionString, strSQL);


                //LOGGING
                 Log.Information("Transfering data from tmp to final table " + _strTableName); 
                //GET SQL TO TRANSFER DATA FROM TMP TO FINAL
                strSQL = getInsertGeneratingSQL(strSchemaName, strLstColumnNames);
                //INSERT DATA TO FINAL TABLE AND DROP TMP
                DBConnection.ExecuteMSSQL(strConnectionString, strSQL);


                //LOGGING
                 Log.Information("Adding log to DB "); 
                //GET SQL TO LOG RECENT CSV TO DB TRANSFERS
                strSQL = getLogGeneratingSQL(strSchemaName);
                //COLLECT LOG DATA
                dtTransfer = DBConnection.getMSSQLDataTable(strConnectionString, strSQL);
                //LOAD LOG TO DB
                if (dtTransfer.Rows.Count > 0)
                {
                    dtTransfer.TableName = strSchemaName + ".Dynamic_Table_History"; //MAKE DYNAMIC??
                    loadToDatabase(dtTransfer, strConnectionString, intBulkSize);
                }

                //LOGGING
                 Log.Information(_strTableName + " created successfully"); 
                //lstFinalSelects.Add("SELECT * FROM " + strSchemaName + "." + _strTableName + ";");

            }

            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
   
        }

        //FOR UNIT TEST ACTUAL RESULTS
        public int getRowCount(string strConnectionString, string strSchemaName)
        {
            object objCnt = DBConnection.getMSSQLExecuteScalar(strConnectionString, "SELECT count(*) cnt FROM " + strSchemaName + "." + _strTableName + ";");

            if ((objCnt + "").IsNumeric())
                return int.Parse(objCnt + "");
            else
                return -9999;

        }

        //FOR UNIT TEST ACTUAL RESULTS
        public List<string?>? getColumnNames(string strConnectionString, string strSchemaName)
        {
            DataTable dt = DBConnection.getMSSQLDataTable(strConnectionString, "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '"+ _strTableName + "' AND TABLE_SCHEMA = '"+ strSchemaName + "'; ");

            if (dt != null)
                return dt.AsEnumerable().Select(r => r.Field<string>("COLUMN_NAME")).ToList();
            else
                return null;

        }

        //FOR UNIT TEST ACTUAL RESULTS
        public int getColumnCount(string strConnectionString, string strSchemaName)
        {
            List<string?>? lstColumnNames = getColumnNames(strConnectionString, strSchemaName);

            if (lstColumnNames != null)
                return lstColumnNames.Count;
            else
                return -9999;

        }

        //USED TO ALLOW PASSING DIRECTORY OR SINLGLE FILE
        private List<string> getFiles(string strPath)
        {
            List<string> strLstFiles = new List<string>();

            FileAttributes attr = File.GetAttributes(strPath);
            if (attr.HasFlag(FileAttributes.Directory)) //GET ALL FILES NOT RECURSIVE!!!!
            {
                strLstFiles = Directory.EnumerateFiles(strPath, "*.csv", SearchOption.TopDirectoryOnly).ToList();
            }
            else //SINGLE FILE
            {
                strLstFiles.Add(strPath);
            }

            return strLstFiles;


        }

        //RETURNS ALL ROWS IN CSV FILES
        private int TotalLines(string strFilePath)
        {
            using (StreamReader r = new StreamReader(strFilePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }

        //POST PROCESS TO DETERMINE LENGTHS AND TYPES
        private string getTypeCheckingSQL(string strSchemaName, List<string>? strLstColumnNames)
        {
            StringBuilder sbSQL = new StringBuilder();

            foreach (string strColumn in strLstColumnNames)
            {
                sbSQL.Append("SELECT ColumnName, MAX(ColumnType) as ColumnType, MAX(ColumnLength) as ColumnLength FROM (");
                sbSQL.Append("SELECT DISTINCT '" + strColumn + "' as ColumnName, ");
                sbSQL.Append("CASE WHEN ISNUMERIC([" + strColumn + "]) = 1 AND LEN([" + strColumn + "]) = 1 AND [" + strColumn + "] NOT LIKE '%[2-9]%' THEN '1-BIT' ELSE ");
                sbSQL.Append("CASE WHEN ISNUMERIC([" + strColumn + "]) = 1 AND CHARINDEX('.',[" + strColumn + "]) > 0 THEN '3-FLOAT' ELSE ");
                sbSQL.Append("CASE WHEN ISNUMERIC([" + strColumn + "]) = 1 AND CHARINDEX('.',[" + strColumn + "]) = 0 THEN '2-INT' ELSE ");
                sbSQL.Append("CASE WHEN ISDATE([" + strColumn + "]) = 1 THEN '4-DATE' ELSE ");
                sbSQL.Append("CASE WHEN LEN([" + strColumn + "]) = 1 AND [" + strColumn + "] LIKE '%[a-z]%' THEN '5-CHAR' ");
                sbSQL.Append("ELSE '6-VARCHAR' ");
                sbSQL.Append("END END END END END AS ColumnType, ");
                sbSQL.Append("MAX(LEN([" + strColumn + "]))  AS ColumnLength ");
                sbSQL.Append("From [" + strSchemaName + "].[" + _strTableName + "_TMP] ");
                sbSQL.Append("WHERE [" + strColumn + "]  IS NOT NULL GROUP BY [" + strColumn + "] ");
                sbSQL.Append(") tmp GROUP BY ColumnName ");
                sbSQL.Append("UNION ALL ");

            }

            return sbSQL.ToString().TrimEnd('U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' ');
        }

        //CREATE TABLE BASED ON TYPES SUPPLIED IN dtSpecs
        private string getFinalTableGeneratingSQL(DataTable dtSpecs, string strSchemaName)
        {
            string? strColType = null;
            string? strNewType = null;
            int intColLength;

            StringBuilder sbSQL = new StringBuilder();

            sbSQL.Append("CREATE TABLE [" + strSchemaName + "].[" + _strTableName + "](");

            foreach (DataRow dr in dtSpecs.Rows)
            {
                strColType = (dr["ColumnType"] + "").Split('-')[1]; //REMOVE ORDERING NUMBER IN FRONT
                intColLength = int.Parse(dr["ColumnLength"] + "");


                if (strColType == "CHAR" || strColType == "VARCHAR")
                {
                    strNewType = strColType + "(" + intColLength + ")";

                }
                else if (strColType == "INT")
                {
                    if (intColLength < 5)
                    {
                        strNewType = "SMALLINT";
                    }
                    else if (intColLength < 10)
                    {
                        strNewType = "INT";
                    }
                    else if (intColLength < 16)
                    {
                        strNewType = "BIGINT";
                    }
                    else
                    {
                        strNewType = "VARCHAR(" + intColLength + ")";
                    }
                }
                else
                {
                    strNewType = strColType;
                }

                sbSQL.Append(" [" + dr["ColumnName"] + "] " + strNewType + " NULL,");

            }
            //DROP TABLE IF ALREAY EXISTS - ERROR FLAG???
            return "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + strSchemaName + "' AND name like '" + _strTableName + "') DROP TABLE " + strSchemaName + "." + _strTableName + ";" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY]; ";
        }

        private string getTmpTableGeneratingSQL(string strSchemaName, List<string> strLstColumnNames)
        {
            StringBuilder sbSQL = new StringBuilder();

            //DYNAMIC TMP TABLE USES [varchar](MAX) FOR CATCH ALL
            sbSQL.Append("CREATE TABLE [" + strSchemaName + "].[" + _strTableName + "_TMP](");
            foreach (string s in strLstColumnNames)
            {
                sbSQL.Append(" [" + s + "] [varchar](MAX) NULL,");
            }
            //CREATE NEW TMP TABLE
            return "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + strSchemaName + "' AND name like '" + _strTableName + "_TMP') DROP TABLE " + strSchemaName + "." + _strTableName + "_TMP;" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY];SELECT * FROM [" + strSchemaName + "].[" + _strTableName + "_TMP];";
        }


        private string getInsertGeneratingSQL(string strSchemaName, List<string>? strLstColumnNames)
        {
            StringBuilder sbSQL = new StringBuilder();

            foreach (string strColumn in strLstColumnNames)
            {
                sbSQL.Append("[" + strColumn + "],");
            }
            return "INSERT INTO [" + strSchemaName + "].[" + _strTableName + "] (" + sbSQL.ToString().TrimEnd(',') + ") SELECT " + sbSQL.ToString().TrimEnd(',') + " FROM [" + strSchemaName + "].[" + _strTableName + "_TMP]; DROP TABLE  [" + strSchemaName + "].[" + _strTableName + "_TMP];";

        }


        private string getLogGeneratingSQL(string strSchemaName)
        {
            return "SELECT UPPER(c.name) 'Column_Name', t.Name 'Data_Type', c.max_length 'Max_Length', '[" + strSchemaName + "].[" + _strTableName + "]' 'Table_Name', CONVERT(VARCHAR(10), GETDATE(), 101) 'Change_Date'   FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('" + strSchemaName + "." + _strTableName + "')";
        }


        private string _strMessage = "";
        private void loadToDatabase(DataTable dtTransfer, string strConnectionString, int intBulkSize)
        {
            _strMessage = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtTransfer.Rows.Count) + " into Staging for " + _strTableName;

            DBConnection.handle_SQLRowCopied += OnSqlRowsCopied;
            DBConnection.SQLServerBulkImportDT(dtTransfer, strConnectionString, intBulkSize * 2);
        }

        private void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            //LOGGING
            Log.Information(_strMessage.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));

        }

    }
}
