using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSV_Parser
{
    class CSV_Parser
    {
        private static DataTable _dtTransfer;
        private static string _strILUCAConnectionString;
        private static string _strSchemaName = "stg";
        private static string _strTableName = null;


        static void Main(string[] args)
        {



            List<string> strLstColumnNames;
            StringBuilder sbSQL = new StringBuilder();
            string strColType = null;
            string strNewType = null;
            int intColLength;

            _strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            string strFilePath = ConfigurationManager.AppSettings["File_Path"];
            //string strCSVTestFile = @"C:\Users\cgiorda\Desktop\CGP_Inj_Chemo\ChemoPA_Data_File_Monthly_202208.txt";
            //strCSVTestFile = @"C:\Users\cgiorda\Desktop\CSV Landing\AMR\auth.csv";

            DateTime dtFile = new DateTime(2023, 01, 06);
            string strType = "txt";
            strFilePath = @"C:\Users\cgiorda\Desktop\Data";


            //GET CSV FILES
            List<string> files = Directory.EnumerateFiles(strFilePath, "*." + strType, SearchOption.TopDirectoryOnly).ToList();

            int intFileCnt = 1;


            DataTable dtTableSpecs;
            DataRow currentRow;
            

            string strParentFolder;
            StreamReader csvreader;
            char chrDelimiter;
            string inputLine = "";
            int intTotal = 0;
            bool blTruncate = true;
            int intCnt = 0;
            StringBuilder sbInsertCols = new StringBuilder();
            StringBuilder sbInsertVals = new StringBuilder();


            //LOOP THROUGH CSV FILES
            foreach (string strFile in files)
            {
                strLstColumnNames = null;
                dtTableSpecs = null;
                currentRow = null;
                sbSQL = new StringBuilder();

                //strParentFolder = new DirectoryInfo(new FileInfo(strFile).DirectoryName).Name;

                ////EXCLUDE ARCHIVES
                //if (strParentFolder.ToLower().Equals("archive"))
                //{
                //    continue;
                //}
                //else if (strParentFolder.ToLower().Equals("csv landing"))
                //{
                //    strParentFolder = "";
                //}
                //else
                //{
                //    strParentFolder = strParentFolder + "_";
                //}


                string strFileName = Path.GetFileName(strFile);
                //CLEAN FILE NAME FOR USE AS TABLE NAME
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                {
                    strFileName = strFileName.Replace(c, '_');
                }

                strFileName = staticConversions(strFileName);
                _strTableName = strFileName.Substring(0, Math.Min(32, strFileName.Length));

                _strTableName = "SiteOfCare_Data_v3";

                csvreader = new StreamReader(strFile);
                chrDelimiter = '|';
                inputLine = "";
                intTotal = 0;
                blTruncate = true;
                intCnt = 0;
                //LOOP LINES PER FILE
                while ((inputLine = csvreader.ReadLine()) != null)
                {
                    string[] csvArray = inputLine.Split(new char[] { chrDelimiter });

                    //FIRST PASS ONLY GETS COLUMNS AND CREATES TABLES
                    if (strLstColumnNames == null)
                    {
                        intTotal = TotalLines(strFile);
                        strLstColumnNames = new List<string>();
                        //GET AND CLEAN COLUMN NAMES FOR TABLE
                        foreach (string s in csvArray)
                        {
                            var colName = s;
                            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                            {
                                colName = colName.Replace(c, '_');
                            }
                            strLstColumnNames.Add(colName.ToUpper().Substring(0, Math.Min(32, s.Length)));
                        }

                        //DYNAMIC TMP TABLE USES [varchar](MAX) FOR CATCH ALL
                        sbSQL.Append("CREATE TABLE [" + _strSchemaName + "].[" + _strTableName + "_TMP](");
                        foreach (string s in strLstColumnNames)
                        {
                            sbSQL.Append(" [" + s + "] [varchar](MAX) NULL,");
                        }
                        //CREATE NEW TMP TABLE
                        DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + _strSchemaName + "' AND name like '" + _strTableName + "_TMP') DROP TABLE " + _strSchemaName + "." + _strTableName + "_TMP;" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY];");
                        sbSQL.Remove(0, sbSQL.Length);


                        //MATCHING TABLE IN MEMORY TO TRANSFER
                        _dtTransfer = DBConnection64.getMSSQLDataTable(_strILUCAConnectionString, "SELECT * FROM [" + _strSchemaName + "].[" + _strTableName + "_TMP] WHERE 1=2;");
                        _dtTransfer.TableName = _strSchemaName + "." + _strTableName + "_TMP";
                        continue;
                    }

                    //LOAD CSV TO TABLE IN MEMORY
                    currentRow = _dtTransfer.NewRow();
                    for (int i = 0; i < strLstColumnNames.Count; i++)
                    {
                        currentRow[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

                    }
                    _dtTransfer.Rows.Add(currentRow);

                    //EACH 10k IN MEMORY LOAD TO DB
                    if (_dtTransfer.Rows.Count == 10000)
                    {
                        loadToDatabase(blTruncate);
                        blTruncate = false;

                    }
                    Console.Write("\rProcessed row " + String.Format("{0:n0}", intCnt) + " out of " + String.Format("{0:n0}", intTotal) + " into Staging...");
                    intCnt++;

                }

                if (_dtTransfer.Rows.Count > 0)
                    loadToDatabase(blTruncate);












                sbSQL.Remove(0, sbSQL.Length);


                sbSQL.Append("SELECT c.name 'ColumnName', t.Name 'Datatype', c.max_length 'MaxLength', c.precision , c.scale , c.is_nullable, ISNULL(i.is_primary_key, 0) 'Primary_Key' FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('" + _strSchemaName + "." + _strTableName + "')");
                dtTableSpecs = DBConnection64.getMSSQLDataTable(_strILUCAConnectionString, sbSQL.ToString());


                sbSQL.Remove(0, sbSQL.Length);

                strColType = null;
                strNewType = null;
                //POST PROCESSING TO DETERMIN PROPER DATA TYPES AND LENGTHS
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
                    sbSQL.Append("From [" + _strSchemaName + "].[" + _strTableName + "_TMP] ");
                    sbSQL.Append("WHERE [" + strColumn + "]  IS NOT NULL GROUP BY [" + strColumn + "] ");
                    sbSQL.Append(") tmp GROUP BY ColumnName ");
                    sbSQL.Append("UNION ALL ");

                }
                _dtTransfer = DBConnection64.getMSSQLDataTable(_strILUCAConnectionString, sbSQL.ToString().TrimEnd('U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '));
                sbSQL.Remove(0, sbSQL.Length);



                //CREATE FINAL TABLE USING LENGTHS AND TYPES DETERMINED ABOVE
        

                foreach (DataRow dr in _dtTransfer.Rows)
                {
                    strColType = dr["ColumnType"].ToString().Split('-')[1];
                    intColLength = int.Parse(dr["ColumnLength"].ToString());


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

                    var colName = dr["ColumnName"];
                    var length = int.Parse(dtTableSpecs.Select("ColumnName = '" + colName + "'").FirstOrDefault()["MaxLength"].ToString());

                    if(strNewType.StartsWith("VARCHAR") && intColLength > length)
                        sbSQL.Append("ALTER TABLE [" + _strSchemaName + "].[" + _strTableName + "] ALTER COLUMN  [" + colName + "] " + strNewType + ";");

                }

                if(sbSQL.Length > 0)
                {
                    DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, sbSQL.ToString());
                    sbSQL.Remove(0, sbSQL.Length);
                }





                foreach (string strColumn in strLstColumnNames)
                {
                    sbInsertCols.Append("[" + strColumn + "],");
                }
                sbInsertCols.Append("[file_name],");
                sbInsertCols.Append("[file_date]");

   

                //TRANSFER DATA FROM TMP TO FINAL TABLE
                foreach (string strColumn in strLstColumnNames)
                {
                    sbInsertVals.Append("[" + strColumn + "],");
                }
                sbInsertVals.Append("'" + strFileName.Replace("'", "''") + "',");
                sbInsertVals.Append("'" + dtFile.ToShortDateString() + "'");

                //sbSQL.Append("[file_name],");
                //sbSQL.Append("[file_date]");
                DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, "INSERT INTO [" + _strSchemaName + "].[" + _strTableName + "] (" + sbInsertCols.ToString()+ ") SELECT " + sbInsertVals.ToString() + " FROM [" + _strSchemaName + "].[" + _strTableName + "_TMP]; DROP TABLE  [" + _strSchemaName + "].[" + _strTableName + "_TMP];");
                sbSQL.Remove(0, sbSQL.Length);
                sbInsertVals.Remove(0, sbInsertVals.Length);
                sbInsertCols.Remove(0, sbInsertCols.Length);

            }




        }



        //static void Main(string[] args)
        //{



        //    List<string> strLstColumnNames;
        //    StringBuilder sbSQL = new StringBuilder();
        //    string strColType = null;
        //    string strNewType = null;
        //    int intColLength;

        //    _strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

        //    string strFilePath = ConfigurationManager.AppSettings["File_Path"];
        //    string strCSVTestFile = @"C:\Users\cgiorda\Desktop\CGP_Inj_Chemo\ChemoPA_Data_File_Monthly_202208.txt";
        //    strCSVTestFile = @"C:\Users\cgiorda\Desktop\CSV Landing\AMR\auth.csv";

        //    DateTime dtFile = new DateTime(2023, 01, 06);
        //    string strType = "txt";
        //    strFilePath = @"C:\Users\cgiorda\Desktop\Data";


        //    GET CSV FILES
        //    List<string> files = Directory.EnumerateFiles(strFilePath, "NHP Historical Case Detail 20230106." + strType, SearchOption.AllDirectories).ToList();

        //    int intFileCnt = 1;


        //    DataTable dtTableSpecs;
        //    DataRow currentRow;

        //    string strParentFolder;
        //    StreamReader csvreader;
        //    char chrDelimiter;
        //    string inputLine = "";
        //    int intTotal = 0;
        //    bool blTruncate = true;
        //    int intCnt = 0;


        //    LOOP THROUGH CSV FILES
        //    foreach (string strFile in files)
        //    {
        //        strLstColumnNames = null;
        //        dtTableSpecs = null;
        //        currentRow = null;
        //        sbSQL = new StringBuilder();

        //        strParentFolder = new DirectoryInfo(new FileInfo(strFile).DirectoryName).Name;

        //        //EXCLUDE ARCHIVES
        //        if (strParentFolder.ToLower().Equals("archive"))
        //        {
        //            continue;
        //        }
        //        else if (strParentFolder.ToLower().Equals("csv landing"))
        //        {
        //            strParentFolder = "";
        //        }
        //        else
        //        {
        //            strParentFolder = strParentFolder + "_";
        //        }


        //        string strFileName = Path.GetFileName(strFile);
        //        CLEAN FILE NAME FOR USE AS TABLE NAME
        //        foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        //        {
        //            strFileName = strFileName.Replace(c, '_');
        //        }

        //        strFileName = staticConversions(strFileName);
        //        _strTableName = strFileName.Substring(0, Math.Min(32, strFileName.Length));

        //        _strTableName = "SiteOfCare_Data_v3";

        //        csvreader = new StreamReader(strFile);
        //        chrDelimiter = '|';
        //        inputLine = "";
        //        intTotal = 0;
        //        blTruncate = true;
        //        intCnt = 0;
        //        LOOP LINES PER FILE
        //        while ((inputLine = csvreader.ReadLine()) != null)
        //        {
        //            string[] csvArray = inputLine.Split(new char[] { chrDelimiter });

        //            FIRST PASS ONLY GETS COLUMNS AND CREATES TABLES
        //            if (strLstColumnNames == null)
        //            {
        //                intTotal = TotalLines(strFile);
        //                strLstColumnNames = new List<string>();
        //                GET AND CLEAN COLUMN NAMES FOR TABLE
        //                foreach (string s in csvArray)
        //                {
        //                    var colName = s;
        //                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        //                    {
        //                        colName = colName.Replace(c, '_');
        //                    }
        //                    strLstColumnNames.Add(colName.ToUpper().Substring(0, Math.Min(32, s.Length)));
        //                }

        //                DYNAMIC TMP TABLE USES[varchar](MAX)FOR CATCH ALL
        //                sbSQL.Append("CREATE TABLE [" + _strSchemaName + "].[" + _strTableName + "_TMP](");
        //                foreach (string s in strLstColumnNames)
        //                {
        //                    sbSQL.Append(" [" + s + "] [varchar](MAX) NULL,");
        //                }
        //                CREATE NEW TMP TABLE
        //                DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + _strSchemaName + "' AND name like '" + _strTableName + "_TMP') DROP TABLE " + _strSchemaName + "." + _strTableName + "_TMP;" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY];");
        //                sbSQL.Remove(0, sbSQL.Length);


        //                MATCHING TABLE IN MEMORY TO TRANSFER
        //                _dtTransfer = DBConnection64.getMSSQLDataTable(_strILUCAConnectionString, "SELECT * FROM [" + _strSchemaName + "].[" + _strTableName + "_TMP] WHERE 1=2;");
        //                _dtTransfer.TableName = _strSchemaName + "." + _strTableName + "_TMP";
        //                continue;
        //            }

        //            LOAD CSV TO TABLE IN MEMORY
        //            currentRow = _dtTransfer.NewRow();
        //            for (int i = 0; i < strLstColumnNames.Count; i++)
        //            {
        //                currentRow[strLstColumnNames[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i].TrimStart('\"').TrimEnd('\"'));

        //            }
        //            _dtTransfer.Rows.Add(currentRow);

        //            EACH 10k IN MEMORY LOAD TO DB
        //            if (_dtTransfer.Rows.Count == 10000)
        //            {
        //                loadToDatabase(blTruncate);
        //                blTruncate = false;

        //            }
        //            Console.Write("\rProcessed row " + String.Format("{0:n0}", intCnt) + " out of " + String.Format("{0:n0}", intTotal) + " into Staging...");
        //            intCnt++;

        //        }

        //        if (_dtTransfer.Rows.Count > 0)
        //            loadToDatabase(blTruncate);


        //        strColType = null;
        //        strNewType = null;
        //        POST PROCESSING TO DETERMIN PROPER DATA TYPES AND LENGTHS
        //        foreach (string strColumn in strLstColumnNames)
        //        {
        //            sbSQL.Append("SELECT ColumnName, MAX(ColumnType) as ColumnType, MAX(ColumnLength) as ColumnLength FROM (");
        //            sbSQL.Append("SELECT DISTINCT '" + strColumn + "' as ColumnName, ");
        //            sbSQL.Append("CASE WHEN ISNUMERIC([" + strColumn + "]) = 1 AND LEN([" + strColumn + "]) = 1 AND [" + strColumn + "] NOT LIKE '%[2-9]%' THEN '1-BIT' ELSE ");
        //            sbSQL.Append("CASE WHEN ISNUMERIC([" + strColumn + "]) = 1 AND CHARINDEX('.',[" + strColumn + "]) > 0 THEN '3-FLOAT' ELSE ");
        //            sbSQL.Append("CASE WHEN ISNUMERIC([" + strColumn + "]) = 1 AND CHARINDEX('.',[" + strColumn + "]) = 0 THEN '2-INT' ELSE ");
        //            sbSQL.Append("CASE WHEN ISDATE([" + strColumn + "]) = 1 THEN '4-DATE' ELSE ");
        //            sbSQL.Append("CASE WHEN LEN([" + strColumn + "]) = 1 AND [" + strColumn + "] LIKE '%[a-z]%' THEN '5-CHAR' ");
        //            sbSQL.Append("ELSE '6-VARCHAR' ");
        //            sbSQL.Append("END END END END END AS ColumnType, ");
        //            sbSQL.Append("MAX(LEN([" + strColumn + "]))  AS ColumnLength ");
        //            sbSQL.Append("From [" + _strSchemaName + "].[" + _strTableName + "_TMP] ");
        //            sbSQL.Append("WHERE [" + strColumn + "]  IS NOT NULL GROUP BY [" + strColumn + "] ");
        //            sbSQL.Append(") tmp GROUP BY ColumnName ");
        //            sbSQL.Append("UNION ALL ");

        //        }
        //        _dtTransfer = DBConnection64.getMSSQLDataTable(_strILUCAConnectionString, sbSQL.ToString().TrimEnd('U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L', ' '));
        //        sbSQL.Remove(0, sbSQL.Length);



        //        CREATE FINAL TABLE USING LENGTHS AND TYPES DETERMINED ABOVE
        //        sbSQL.Append("CREATE TABLE [" + _strSchemaName + "].[" + _strTableName + "](");

        //        foreach (DataRow dr in _dtTransfer.Rows)
        //        {
        //            strColType = dr["ColumnType"].ToString().Split('-')[1];
        //            intColLength = int.Parse(dr["ColumnLength"].ToString());


        //            if (strColType == "CHAR" || strColType == "VARCHAR")
        //            {
        //                strNewType = strColType + "(" + intColLength + ")";

        //            }
        //            else if (strColType == "INT")
        //            {
        //                if (intColLength < 5)
        //                {
        //                    strNewType = "SMALLINT";
        //                }
        //                else if (intColLength < 10)
        //                {
        //                    strNewType = "INT";
        //                }
        //                else if (intColLength < 16)
        //                {
        //                    strNewType = "BIGINT";
        //                }
        //                else
        //                {
        //                    strNewType = "VARCHAR(" + intColLength + ")";
        //                }
        //            }
        //            else
        //            {
        //                strNewType = strColType;


        //            }

        //            sbSQL.Append(" [" + dr["ColumnName"] + "] " + strNewType + " NULL,");

        //        }
        //        sbSQL.Append(" [file_name] varchar(255) NULL,");
        //        sbSQL.Append(" [file_date] Date NULL");
        //        DROP TABLE IF ALREAY EXISTS - ERROR FLAG ???
        //        DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, "IF EXISTS(SELECT * FROM sys.tables WHERE SCHEMA_NAME(schema_id) LIKE '" + _strSchemaName + "' AND name like '" + _strTableName + "') DROP TABLE " + _strSchemaName + "." + _strTableName + ";" + sbSQL.ToString().TrimEnd(',') + ") ON [PRIMARY]; ");
        //        sbSQL.Remove(0, sbSQL.Length);


        //        TRANSFER DATA FROM TMP TO FINAL TABLE
        //        foreach (string strColumn in strLstColumnNames)
        //        {
        //            sbSQL.Append("[" + strColumn + "],");
        //        }
        //        sbSQL.Append("[file_name],");
        //        sbSQL.Append("[file_date]");
        //        DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, "INSERT INTO [" + _strSchemaName + "].[" + _strTableName + "] (" + sbSQL.ToString().TrimEnd(',') + ") SELECT " + sbSQL.ToString().TrimEnd(',') + ",'" + strFileName.Replace("'", "''") + "', '" + dtFile.ToShortDateString() + "' FROM [" + _strSchemaName + "].[" + _strTableName + "_TMP]; DROP TABLE  [" + _strSchemaName + "].[" + _strTableName + "_TMP];");
        //        sbSQL.Remove(0, sbSQL.Length);


        //        TRANSFER DATA FROM TMP TO FINAL TABLE
        //        DROP TMP TABLE
        //        dtTableSpecs = DBConnection64.getMSSQLDataTable(_strILUCAConnectionString, "SELECT UPPER(c.name) 'Column_Name', t.Name 'Data_Type', c.max_length 'Max_Length', '[" + _strSchemaName + "].[" + _strTableName + "]' 'Table_Name', CONVERT(VARCHAR(10),  GETDATE(), 101 ) 'Change_Date'   FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('" + _strSchemaName + "." + _strTableName + "')");
        //        dtTableSpecs.TableName = "stg.Dynamic_Table_History";
        //        DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //        DBConnection64.SQLServerBulkImportDT(dtTableSpecs, _strILUCAConnectionString, 10000);

        //    }




        //}


        private static void loadToDatabase(bool blTruncate = false)
        {
            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", _dtTransfer.Rows.Count) + " into Staging...";

            if (blTruncate)
            {
                DBConnection64.ExecuteMSSQL(_strILUCAConnectionString, "TRUNCATE TABLE " + _dtTransfer.TableName);
            }

            DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
            DBConnection64.SQLServerBulkImportDT(_dtTransfer, _strILUCAConnectionString, 20000);
            _dtTransfer.Rows.Clear();
            //GC.Collect(2, GCCollectionMode.Forced);
        }



        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }


        private static int TotalLines(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }


        private static string staticConversions(string strFileName)
        {
            string strNewName = strFileName.ToUpper().Replace(".CSV", "").Replace(".TXT", "").Replace(" ", "_"); ;
            if(strFileName.ToLower().StartsWith("chemopa_data_file_monthly"))
            {
                strNewName = "CGP_INJ_CHEMO";
            }
            return strNewName;
        }

    }
}
