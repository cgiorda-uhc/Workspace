
using ExtensionMethods;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CGP_Inj_Chemo_Parser
{
    class CGP_Inj_Chemo_Parser
    {
        private static DataTable _dtTransfer;
        private static string _strILUCAConnectionString;
        private static string _strCSVFileName;
        static void Main(string[] args)
        {

            Console.WriteLine("CGP_Inj_Chemo Parser");


            //HelperFunctions.HelperFunctions.Email("sheila_donelan@uhc.com;", "chris_giordano@uhc.com", "UCS Automation Manager: CGP Inj Chemo Parser", "CGP Inj Chemo table was populated for month of August 2022", "jon_maguire@uhc.com;chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.Normal);




            //--DELETE FROM[IL_UCA].[stg].[CGP_Inj_Chemo_FormatLog] WHERE csv_file_name = 'ChemoPA_Data_File_Monthly_202207.txt';
            //--DELETE FROM[stg].[CGP_Inj_Chemo_LengthLog] WHERE csv_file_name = 'ChemoPA_Data_File_Monthly_202207.txt';
            //--DELETE FROM[stg].[CGP_Inj_Chemo_NewColumnLog] WHERE csv_file_name = 'ChemoPA_Data_File_Monthly_202207.txt';
            //--TRUNCATE TABLE[stg].[CGP_Inj_Chemo_Cleanup];
            //--TRUNCATE TABLE[stg].[CGP_Inj_Chemo];
            //--TRUNCATE TABLE[stg].[CGP_Inj_Chemo_LengthCompare];
            //--ALTER TABLE[stg].[CGP_Inj_Chemo] DROP COLUMN CLIN_HLA_A_02_01_STAT;
            //--ALTER TABLE[stg].[CGP_Inj_Chemo_Cleanup] DROP COLUMN CLIN_HLA_A_02_01_STAT;

            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN MBR_FST_NM varchar(30);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN PERSON_ENTERING_FULL_NAME varchar(1207);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN FEBRILE_NEUTROPENIA_RISK_LEVEL char(1);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN CLIN_ANDROGEN_RECEPTOR_STATUS varchar(8);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN CLIN_BRCA1_BRCA2_STATUS varchar(19);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN CLIN_CD30_STATUS varchar(8);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN CLIN_DELETION_5Q_IND varchar(7);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN CLIN_KIT_D816V_MUTATN_NEG_IND varchar(8);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN EMETIC_RISK_DESC varchar(226);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN ADMIN_DAYS_OF_CYCLE_DESC varchar(475);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN OFF_PATHWAY_SELECTION_REASON varchar(704);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN CLIN_INGUINAL_LYMPH_NODE_STATUS varchar(8);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_PELVIC_LYMPH_NODE_STATUS varchar(8);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_NEOADJ_CHEMO_DOC_POS_RESP varchar(14);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  ICUE_HSCID varchar(10);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_PREV_IMMUNOTHERAPY_IND varchar(22);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_EZH2_MUTATION_STATUS varchar(19);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_ELDERLY_OR_COMORBID_IND varchar(7);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_TP53_MUTATION_STATUS varchar(19);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_PATNT_UNFIT_FOR_SURG varchar(7);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_PLATELET_CNT_CATEGORY varchar(14);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_PALB2_MUTATION_STATUS varchar(19);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  CLIN_AGE_OVER80_OR_CD20_INTOL varchar(7);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  SUPP_T_SCORE char(1);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  SUPP_CHEMO_INTENT varchar(10);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  SUPP_CHEMO_RECD_TREATMENT varchar(117);
            //--ALTER TABLE[stg].[CGP_Inj_Chemo]  ALTER COLUMN  SUPP_INDUCTION_CONSOLIDATION char(1);





            //LIVE PATH 
            //\\nasv0009\onc_uhg_emp_win_sas\Analytics\Oncology\Chemo Prior Auth\Prior_Auth_Database\CGP Monthly File
            _strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strFile = ConfigurationManager.AppSettings["File_Path"];
            _strCSVFileName = Path.GetFileName(strFile);
            char chrDelimiter = '|';
            string strSQL = null;
            StringBuilder sbSQL = new StringBuilder();
            ArrayList alHeaders = null;
            string strNewCol = null;
            bool blNewColumn = false;

            //GET ALL COLUMNS AND DATATYPES
            DataTable dtTableSpecs = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, "SELECT UPPER(c.name) 'Column_Name', t.Name 'Data_Type', c.max_length 'Max_Length', c.precision , c.scale , c.is_nullable, ISNULL(i.is_primary_key, 0) 'Primary_Key' FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('stg.CGP_Inj_Chemo')");
            //ALL COLUMN NAMES FOR LOOPING!!!
            List<string> strColumnsArr = dtTableSpecs.AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();


            //DEBUG CHANGE TRUE!!! CLEANS FIRST PASS ADD COL FOR TESTING
            bool blTruncate = true;
            //strColumnsArr.Add("CLIN_HLA_A_02_01_STAT");


            //STEP 1 PARSE CSV LOAD TO [IL_UCA].[stg].CGP_Inj_Chemo_Cleanup FOR FURTHER ANALYSIS
            if (1==1)
            {
                _dtTransfer = new DataTable();
                DataRow currentRow;
                foreach (string s in strColumnsArr)
                {
                    _dtTransfer.Columns.Add(s, typeof(string));
                }
                _dtTransfer.TableName = "stg.CGP_Inj_Chemo_Cleanup";

                StreamReader csvreader = new StreamReader(strFile);
                string inputLine = "";
                Int64 intCnt = 0;
                Int64 intTotal = 0;
                while ((inputLine = csvreader.ReadLine()) != null)
                {
                    string[] csvArray = inputLine.Split(new char[] { chrDelimiter });

                    if (alHeaders == null)
                    {
                        intTotal = TotalLines(strFile);
                        alHeaders = new ArrayList();
                        foreach (string s in csvArray)
                        {
                            strNewCol = s.ToUpper().Substring(0, Math.Min(32, s.Length));
                            int i = strColumnsArr.FindIndex(a => a.Contains(strNewCol));
                            if (i == -1)//ADD COLUMN
                            {
                                DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, "ALTER TABLE " + _dtTransfer.TableName + " ADD " + strNewCol + " VARCHAR(MAX); INSERT INTO [stg].[CGP_Inj_Chemo_NewColumnLog] ([col_title],[csv_file_name]) VALUES ('"+ strNewCol + "','"+ _strCSVFileName+"')");

                                blNewColumn = true;
                                strColumnsArr.Add(strNewCol);
                                _dtTransfer.Columns.Add(strNewCol, typeof(string));
                                i = strColumnsArr.FindIndex(a => a.Contains(strNewCol));
                            }
                            alHeaders.Add(i);
                        }

                        continue;
                    }

                    currentRow = _dtTransfer.NewRow();
                    foreach (int i in alHeaders)
                    {

                        if(strColumnsArr[i] == "INITIAL_DIAGNOSIS_YR_MNTH") //MANUALLY ADD TO MAKE PROPER DATE
                        {
                            currentRow[strColumnsArr[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : "01/" + csvArray[i]);
                        }
                        else
                        {
                            currentRow[strColumnsArr[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i]);
                        }

                    }
                    _dtTransfer.Rows.Add(currentRow);


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


                //CLEANUP NEW COLUMN TYPES BASED ON ALL NEW DATA!!!
                if(blNewColumn)
                {
                    addNewColumnsToFinalTable();
                    dtTableSpecs = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, "SELECT UPPER(c.name) 'Column_Name', t.Name 'Data_Type', c.max_length 'Max_Length', c.precision , c.scale , c.is_nullable, ISNULL(i.is_primary_key, 0) 'Primary_Key' FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('stg.CGP_Inj_Chemo')");
                    //ALL COLUMN NAMES FOR LOOPING!!!
                    strColumnsArr = dtTableSpecs.AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();

                }


                //TRUNCATE AND POPULATE CGP_Inj_Chemo_LengthCompare
                strSQL = getLengthConflictSQL(strColumnsArr);
                DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, strSQL);

            }


            //HANDLES ALL ANALYSIS/CLEANUP TASKS
            DataTable dtCleanup;

            //STEP 2 ANALYZE DATE AND LOG FINDINGS
            if (1==1)
            {
                //LOAD TO TABLE
                //UPDATE COLUMN ISSUES USING SQL BELOW USING NEW LOADED TABLE DATA
                //GET INVALID FORMATTED DATES AND INTS
                strColumnsArr = dtTableSpecs.Select("[Data_Type] LIKE 'date%'").AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();
                sbSQL.Append(getInvalidDateFormatSQL(strColumnsArr));
                sbSQL.Append(" UNION ALL ");
                strColumnsArr = dtTableSpecs.Select("[Data_Type] LIKE '%int'").AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();
                sbSQL.Append(getInvalidNumSQL(strColumnsArr));
                dtCleanup = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, sbSQL.ToString());
                dtCleanup.TableName = "stg.CGP_Inj_Chemo_FormatLog";
                //MAKE FALSE NO TRUNCATE!!!!!!
                //MAKE FALSE NO TRUNCATE!!!!!!
                //MAKE FALSE NO TRUNCATE!!!!!!
                loadToDatabase(dtCleanup, false);
                //POPULATE ISSUES OLD DATE VS INVALID
                strSQL = "UPDATE stg.CGP_Inj_Chemo_FormatLog SET [ColumnIssue] = 'Warning: Date provided was set earlier than 1753!' WHERE [ColumnFormat] = 'date' AND TRY_PARSE([ColumnValue] AS DATETIME2) IS NOT NULL; UPDATE stg.CGP_Inj_Chemo_FormatLog SET[ColumnIssue] = 'Error: Date provided has invalid format!' WHERE[ColumnFormat] = 'date' AND TRY_PARSE([ColumnValue] AS DATETIME2) IS NULL; ";
                DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, strSQL);

                //INVALID LENG TABLE
                //GET INVALID LENGTH
                strSQL = "SELECT [col_val] ,[col_title] ,[col_val_cnt],[col_max] ,[col_limit] ,[col_type], '"+ _strCSVFileName +"' as [csv_file_name]  FROM [IL_UCA].[stg].[CGP_Inj_Chemo_LengthCompare] WHERE [col_limit] <[col_max] AND [col_type] LIKE '%char'";
                dtCleanup = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, strSQL);
                dtCleanup.TableName = "stg.CGP_Inj_Chemo_LengthLog";
                //MAKE FALSE NO TRUNCATE!!!!!!
                //MAKE FALSE NO TRUNCATE!!!!!!
                //MAKE FALSE NO TRUNCATE!!!!!!
                loadToDatabase(dtCleanup, false);
            }

            //STEP 3 CLEANUP
            if (1 == 1)
            {
                sbSQL.Remove(0, sbSQL.Length);
                strSQL = "SELECT [col_title] , [col_type], MAX([col_max]) as col_limit  FROM [IL_UCA].[stg].[CGP_Inj_Chemo_LengthLog] WHERE [csv_file_name] = '" + _strCSVFileName + "' GROUP BY [col_title], [col_type] ";
                dtCleanup = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, strSQL);
                //EXECEL REPORT AND LOOP CLEAN ??
                foreach (DataRow dr in dtCleanup.Rows)
                {
                    if (dr["col_title"].ToString() == "PERSON_ENTERING_FULL_NAME")
                    {
                        sbSQL.Append("UPDATE stg.CGP_Inj_Chemo_Cleanup SET " + dr["col_title"].ToString() + " = SUBSTRING(" + dr["col_title"].ToString() + ", 0, 100); UPDATE [stg].[CGP_Inj_Chemo_LengthLog] SET [resolution] = 'Truncated value to varchar(100)' WHERE  [csv_file_name] = '" + _strCSVFileName + "' AND [col_title]  = '" + dr["col_title"].ToString() + "';");
                    }
                    else if (dr["col_title"].ToString() == "MBR_FST_NM")
                    {
                        sbSQL.Append("UPDATE stg.CGP_Inj_Chemo_Cleanup SET " + dr["col_title"].ToString() + " = SUBSTRING(" + dr["col_title"].ToString() + ", 0, 30); UPDATE [stg].[CGP_Inj_Chemo_LengthLog] SET [resolution] = 'Truncated value to varchar(30)' WHERE  [csv_file_name] = '" + _strCSVFileName + "' AND [col_title]  = '" + dr["col_title"].ToString() + "';");
                    }
                    else
                    {
                        var strType = (dr["col_type"].ToString().ToLower().Equals("char") && int.Parse(dr["col_limit"].ToString()) > 5 ? "varchar" : dr["col_type"].ToString()); //CHANGE CHAR TO VARCHAR IF MORE THAN 5


                        sbSQL.Append("ALTER TABLE [stg].[CGP_Inj_Chemo] ALTER COLUMN " + dr["col_title"] + " " + strType + "(" + dr["col_limit"] + "); UPDATE [stg].[CGP_Inj_Chemo_LengthLog] SET [resolution] = 'Resized column to new limit " + strType + "(" + dr["col_limit"] + ")' WHERE  [csv_file_name] = '" + _strCSVFileName + "' AND [col_title]  = '" + dr["col_title"].ToString() + "';");

                    }
                    //ELSE IF(dr["col_title"] == "MBR_FST_NM") trucate 30 dr[col_val].Truncate(30);
                    //ELSE "ALTER TABLE [stg].[CGP_Inj_Chemo] ALTER COLUMN "+ dr[col_title] + " " + dr[col_type] + "(" + dr[col_limit] + ")";
                    //ANY STOP PRESSES????
                }

                if (sbSQL.Length > 0)
                    DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, sbSQL.ToString());


                sbSQL.Remove(0, sbSQL.Length);
                strSQL = "SELECT [ColumnName] ,[ColumnValue] ,[ColumnValueCnt], [ColumnFormat] ,[ColumnIssue] FROM [IL_UCA].[stg].[CGP_Inj_Chemo_FormatLog] WHERE [csv_file_name] = '" + _strCSVFileName + "'";
                dtCleanup = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, strSQL);
                foreach (DataRow dr in dtCleanup.Rows)
                {
                    //EXECEL REPORT AND LOOP CLEAN ??
                    if (dr["ColumnIssue"].ToString() == "Warning: Date provided was set earlier than 1753!")
                    {
                        sbSQL.Append("UPDATE [stg].[CGP_Inj_Chemo_FormatLog] SET [resolution] = 'Date {" + dr["ColumnValue"] + "}  added to " + dr["ColumnName"] + " and issue logged' WHERE  [csv_file_name] = '" + _strCSVFileName + "' AND [ColumnName]  = '" + dr["ColumnName"].ToString() + "';");
                    }
                    else if (dr["ColumnFormat"].ToString() == "int" && (dr["ColumnValue"].ToString().ToLower().Equals("null") || dr["ColumnValue"].ToString().ToLower().Equals("undefined")))
                    {
                        sbSQL.Append("UPDATE stg.CGP_Inj_Chemo_Cleanup SET " + dr["ColumnName"] + " =  NULL WHERE " + dr["ColumnName"] + " = '"+ dr["ColumnValue"].ToString() + "'; UPDATE [stg].[CGP_Inj_Chemo_FormatLog] SET [resolution] = 'Int value {" + dr["ColumnValue"] + "}  converted to NULL added to " + dr["ColumnName"] + "' WHERE  [csv_file_name] = '" + _strCSVFileName + "' AND [ColumnName]  = '" + dr["ColumnName"].ToString() + "';");
                    }
                    else
                    {
                        throw new Exception("STOP THE PRESSES????");
                    }
   
                }
                if (sbSQL.Length > 0)
                    DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, sbSQL.ToString());
            }


            //MOVE FINAL DATA!!!
            sbSQL.Remove(0, sbSQL.Length);
            strColumnsArr = dtTableSpecs.AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();
            strSQL = getMainInsert(strColumnsArr);
            DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, strSQL);

            //STEP 4 CLEANUP EMAIL AND ATTACH REPORT!!!

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


        private static void loadToDatabase(bool blTruncate = false)
        {
            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", _dtTransfer.Rows.Count) + " into Staging...";

            if (blTruncate)
            {
                DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, "TRUNCATE TABLE " + _dtTransfer.TableName);
            }

            DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
            DBConnection32.SQLServerBulkImportDT(_dtTransfer, _strILUCAConnectionString, 10000);
            _dtTransfer.Rows.Clear();
            //GC.Collect(2, GCCollectionMode.Forced);
        }

        private static void loadToDatabase(DataTable dt, bool blTruncate = false)
        {
            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dt.Rows.Count) + " into Staging...";


            if (blTruncate)
            {
                DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, "TRUNCATE TABLE " + dt.TableName);
            }

            DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
            DBConnection32.SQLServerBulkImportDT(dt, _strILUCAConnectionString, 10000);
            //GC.Collect(2, GCCollectionMode.Forced);
        }



        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }


        private static int[] getColumnNumberArray(List<string> strColumnNameArr)
        {
            ArrayList alColArr = new ArrayList();
            foreach (string s in strColumnNameArr)
            {
                alColArr.Add(s);
            }
            return (int[])alColArr.ToArray(typeof(int));
        }

        private static string getLengthConflictSQL(List<string> strColumnNameArr)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("TRUNCATE TABLE [stg].[CGP_Inj_Chemo_LengthCompare];");
            sb.Append("INSERT INTO [stg].[CGP_Inj_Chemo_LengthCompare] ([col_val] ,[col_title] ,[col_val_cnt], [col_max] ,[col_limit] ,[col_type])");
            sb.Append("SELECT tmp.col_val,tmp.col_title, COUNT(*) as col_val_cnt,tmp.col_max,tmp.col_limit, tmp.col_type FROM (");
            foreach (string s in strColumnNameArr)
            {
                //sb.Append("select top 1 "+ s + " as col_val, '" + s + "' as col_title, len(" + s + ") as col_max, COL_LENGTH('stg.CGP_Inj_Chemo', '" + s + "') as col_limit,(SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'stg' AND TABLE_NAME   = 'CGP_Inj_Chemo' AND  COLUMN_NAME  = '" + s + "') as col_type, '" + _strCSVFileName + "' as csv_file_name ");
                //sb.Append(" from [IL_UCA].[stg].CGP_Inj_Chemo_Cleanup ORDER BY len(" + s + ") desc ");
                sb.Append("select " + s + " as col_val, '" + s + "' as col_title, len(" + s + ") as col_max, COL_LENGTH('stg.CGP_Inj_Chemo', '" + s + "') as col_limit,(SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'stg' AND TABLE_NAME   = 'CGP_Inj_Chemo' AND  COLUMN_NAME  = '" + s + "') as col_type ");

                sb.Append(" from [IL_UCA].[stg].CGP_Inj_Chemo_Cleanup ");

                sb.Append(" UNION ALL ");
            }
            sb.Append(") tmp WHERE tmp.col_max > tmp.col_limit AND tmp.col_type LIKE '%char' GROUP BY tmp.col_val,tmp.col_title,tmp.col_max,tmp.col_limit, tmp.col_type");
            return sb.ToString().Replace("  UNION ALL ) tmp", ") tmp");
        }

        private static string getInvalidDateFormatSQL(List<string> strColumnNameArr)
        {
            StringBuilder sb = new StringBuilder();


            sb.Append("SELECT tmp.ColumnName, tmp.ColumnValue, COUNT(*) as ColumnValueCnt, 'date' as  ColumnFormat, NULL  as ColumnIssue, '" + _strCSVFileName + "' as csv_file_name  FROM (");
            foreach (string s in strColumnNameArr)
            {
                sb.Append("SELECT '" + s + "' as ColumnName, " + s + " as ColumnValue FROM  stg.CGP_Inj_Chemo_Cleanup WHERE isdate(" + s + ") = 0 ");

                sb.Append(" UNION ALL ");
            }
            sb.Append(") tmp WHERE tmp.ColumnValue IS NOT NULL GROUP BY tmp.ColumnName, tmp.ColumnValue ");
            return sb.ToString().Replace(" UNION ALL ) tmp", ") tmp");
        }


        private static string getInvalidNumSQL(List<string> strColumnNameArr)
        {
            StringBuilder sb = new StringBuilder();


            sb.Append("SELECT tmp.ColumnName, tmp.ColumnValue, COUNT(*) as ColumnValueCnt, 'int' as  ColumnFormat, 'Error: Number provided has invalid format!' as ColumnIssue, '" + _strCSVFileName + "' as csv_file_name  FROM (");
            foreach (string s in strColumnNameArr)
            {
                sb.Append("SELECT '" + s + "' as ColumnName, " + s + " as ColumnValue FROM  stg.CGP_Inj_Chemo_Cleanup WHERE ISNUMERIC(" + s + ") = 0");

                sb.Append(" UNION ALL ");
            }
            sb.Append(") tmp WHERE tmp.ColumnValue IS NOT NULL GROUP BY tmp.ColumnName, tmp.ColumnValue");
            return sb.ToString().Replace(" UNION ALL ) tmp", ") tmp");
        }




        private static string getMainInsert(List<string> strColumnNameArr)
        {
            StringBuilder sb = new StringBuilder();
            string strSQL;

            foreach (string s in strColumnNameArr)
            {
                sb.Append(s + ",");
            }

            strSQL = "TRUNCATE TABLE [stg].[CGP_Inj_Chemo]; INSERT INTO [stg].[CGP_Inj_Chemo] (" + sb.ToString().TrimEnd(',') + ") SELECT " + sb.ToString().TrimEnd(',') + " FROM [stg].[CGP_Inj_Chemo_Cleanup]; ";

            return strSQL;
        }



        private static void addNewColumnsToFinalTable()
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = null;
            DataTable dtNewCols = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, "SELECT [col_title] FROM [stg].[CGP_Inj_Chemo_NewColumnLog] WHERE [csv_file_name] = '" + _strCSVFileName + "';");
            List<string> strColumnNameArr = dtNewCols.AsEnumerable().Select(r => r.Field<string>("col_title")).ToList();

            foreach (string s in strColumnNameArr)
            {
                sb.Append("SELECT t2.new_col, t2.col_type, t2.size FROM ");
                sb.Append("(SELECT TOP 1 tmp.new_col, tmp.col_type, tmp.size FROM ( ");
                sb.Append("SELECT TOP 1 '"+ s +"' as new_col, 'datetime2' as col_type, MAX(LEN("+ s +")) as size, 1 as order_by FROM [stg].[CGP_Inj_Chemo_Cleanup] WHERE ISDATE("+ s +") > 0 GROUP BY "+ s +"");
                sb.Append(" UNION ALL ");
                sb.Append("SELECT TOP 1 '"+ s +"' as new_col, 'int' as col_type, MAX(LEN("+ s +")) as size, 2 as order_by  FROM [stg].[CGP_Inj_Chemo_Cleanup] WHERE ISNUMERIC("+ s +") > 0  GROUP BY "+ s +"");
                sb.Append(" UNION ALL ");
                sb.Append("SELECT TOP 1 '"+ s +"' as new_col, 'varchar' as col_type, MAX(LEN("+ s +")) as size, 3 as order_by FROM [stg].[CGP_Inj_Chemo_Cleanup] WHERE ISNUMERIC("+ s +") = 0 AND  ISDATE("+ s +") = 0 GROUP BY "+ s +"");
                sb.Append(") tmp ORDER BY tmp.size DESC)t2 ");

                sb.Append(" UNION ALL ");

            }

            dt = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, sb.ToString().TrimEnd(' ', 'U', 'N', 'I', 'O', 'N', ' ', 'A', 'L', 'L',' '));
            sb.Remove(0, sb.Length);
            foreach (DataRow dr in dt.Rows)
            {
                var type = dr["col_type"].ToString();

                if (dr["col_type"].ToString().Equals("datetime2"))
                {
                    sb.Append("ALTER TABLE [stg].[CGP_Inj_Chemo] ADD " + dr["new_col"] + " " + type + ";");
                }
                else if (dr["col_type"].ToString().Equals("int"))
                {
                    type = (int.Parse(dr["size"].ToString()) > 999999999 ? "bigint" : "int");
                    sb.Append("ALTER TABLE [stg].[CGP_Inj_Chemo] ADD " + dr["new_col"] + " " + type + ");");
                }
                else
                {
                    sb.Append("ALTER TABLE [stg].[CGP_Inj_Chemo] ADD " + dr["new_col"] + " " + type + "(" + dr["size"] + ");");
                }


                sb.Append("UPDATE [stg].[CGP_Inj_Chemo_NewColumnLog] SET [col_type] = '"+ type + "', [resolution] = 'New column "+ dr["new_col"] + " was added to table.' WHERE [col_title] = '" + dr["new_col"] + "'  and [csv_file_name] = '" + _strCSVFileName + "';");


            }
            DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, sb.ToString());

        }



    }

}
