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

namespace SOS_Auth_Parser
{
    class SOS_Auth_Parser
    {
        private static DataTable _dtTransfer;
        private static string _strILUCAConnectionString;
        private static string _strCSVFileName;


        static void Main(string[] args)
        {

            _strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strFolder = ConfigurationManager.AppSettings["Folder_Path"];
            _strCSVFileName = Path.GetFileName(strFolder);
            StringBuilder sbSQL = new StringBuilder();
            
            string strFileName = null;
            string lastFolderName = null;
            char chrDelimiter = ',';
            string strMonth = null;
            string strYear = null;
            ArrayList alHeaders = null;

            bool blTruncate = true;
            int intFileCnt = 0;
            List<string> files = Directory.EnumerateFiles(strFolder, "*.csv", SearchOption.AllDirectories).ToList();
            DataTable dtTableSpecs;
            DataRow currentRow;
            object objTableCheck;
            List<string> strColumnsArr = null ;




            foreach (string strFile in files)
            {
                blTruncate = true;
                alHeaders = null;
                strFileName = Path.GetFileName(strFile);
                //if (!strFileName.ToLower().Equals("cns_sos.csv") && !strFileName.ToLower().Equals("enis_sos.csv") && !strFileName.ToLower().Equals("mnr_sos.csv") && !strFileName.ToLower().Equals("avtarx.csv"))
                if (!strFileName.ToLower().Equals("enis_sos.csv"))
                {
                    intFileCnt++;
                    continue;
                }


                dtTableSpecs = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, "SELECT c.name 'Column_Name' FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('stg." + strFileName.Replace(".csv", "") + "')");
                //ALL COLUMN NAMES FOR LOOPING!!!
                strColumnsArr = dtTableSpecs.AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();

                updateColumns(strColumnsArr, "stg." + strFileName.Replace(".csv", ""));

            }







            foreach (string strFile in files)
            {
                blTruncate = true;
                alHeaders = null;
                strFileName = Path.GetFileName(strFile);
                //if (!strFileName.ToLower().Equals("cns_sos.csv") && !strFileName.ToLower().Equals("enis_sos.csv") && !strFileName.ToLower().Equals("mnr_sos.csv") && !strFileName.ToLower().Equals("avtarx.csv"))
                if (!strFileName.ToLower().Equals("enis_sos.csv"))
                {
                    intFileCnt++;
                    continue;
                }

                lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile));
                strMonth = lastFolderName.Replace("-", "").Substring(4, 2);
                strYear = lastFolderName.Replace("-", "").Substring(0, 4);

                if(int.Parse(strYear) >= 2022 && int.Parse(strMonth) < 7)
                {
                    intFileCnt++;
                    continue;
                }

                Console.Write("\rProcessed file "+ strFileName + " into Staging...");

               

                StreamReader csvreader = new StreamReader(strFile);
                string inputLine = "";
                Int64 intCnt = 0;
                Int64 intTotal = 0;
                while ((inputLine = csvreader.ReadLine()) != null)
                {
                    string[] csvArray = inputLine.Split(new char[] { chrDelimiter });

                    if (alHeaders == null)
                    {



                        objTableCheck = DBConnection32.getMSSQLExecuteScalar(_strILUCAConnectionString, "SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'stg' AND TABLE_NAME = '"+ strFileName.Replace(".csv", "") + "'");
                        if (objTableCheck == null)
                        {
                            sbSQL.Append("CREATE TABLE [stg].["+ strFileName.Replace(".csv", "") + "](");


                            foreach (string s in csvArray)
                            {
                                sbSQL.Append(" ["+ s +"] [varchar](255) NULL, ");


                            }

                            sbSQL.Append(" [file_month] [varchar](2) NULL,[file_year] [varchar](4) NULL,[file_date] [date] NULL,[file_name] [varchar](255) NULL) ON [PRIMARY]");

                            DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, sbSQL.ToString());
                            sbSQL.Remove(0, sbSQL.Length);
                        }

                        //GET ALL COLUMNS AND DATATYPES
                        dtTableSpecs = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, "SELECT c.name 'Column_Name', t.Name 'Data_Type', c.max_length 'Max_Length', c.precision , c.scale , c.is_nullable, ISNULL(i.is_primary_key, 0) 'Primary_Key' FROM sys.columns c INNER JOIN sys.types t ON c.user_type_id = t.user_type_id LEFT OUTER JOIN sys.index_columns ic ON ic.object_id = c.object_id AND ic.column_id = c.column_id LEFT OUTER JOIN sys.indexes i ON ic.object_id = i.object_id AND ic.index_id = i.index_id WHERE c.object_id = OBJECT_ID('stg." + strFileName.Replace(".csv", "") + "')");
                        //ALL COLUMN NAMES FOR LOOPING!!!
                        strColumnsArr = dtTableSpecs.AsEnumerable().Select(r => r.Field<string>("Column_Name")).ToList();
                        _dtTransfer = new DataTable();
                        foreach (DataRow dr in dtTableSpecs.Rows)
                        {
                            _dtTransfer.Columns.Add(dr["Column_Name"].ToString());
                        }
                        _dtTransfer.TableName = "stg." + strFileName.Replace(".csv", "") + "";


                        intTotal = TotalLines(strFile);
                        alHeaders = new ArrayList();
                        foreach (string s in csvArray)
                        {
                            var strNewCol = s.ToUpper().Substring(0, Math.Min(32, s.Length));
                            var i = strColumnsArr.FindIndex(a => a.ToLower().Contains(strNewCol.ToLower()));
                            alHeaders.Add(i);
                        }

                        continue;
                    }

                    currentRow = _dtTransfer.NewRow();
                    foreach (int i in alHeaders)
                    {

                        //if (strColumnsArr[i] == "INITIAL_DIAGNOSIS_YR_MNTH") //MANUALLY ADD TO MAKE PROPER DATE
                        //{
                        //    currentRow[strColumnsArr[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : "01/" + csvArray[i]);
                        //}
                        //else
                        //{
                            currentRow[strColumnsArr[i]] = (csvArray[i].Trim().Equals("") ? (object)DBNull.Value : csvArray[i]);
                        //}

                    }
                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["file_name"] = strFileName;

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

                csvreader.Close();
                csvreader = null;

            }

        }


        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
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

        private static int TotalLines(string filePath)
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                int i = 0;
                while (r.ReadLine() != null) { i++; }
                return i;
            }
        }


        private static void updateColumns( List<string> strColumnNameArr,  string strTableName)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("SELECT ");
            foreach (string s in strColumnNameArr)
            {
                sb.Append(" MAX(LEN(" + s + ")) as [" + s + "],");

            }

            DataTable dtLength = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, sb.ToString().TrimEnd(',') + " FROM " + strTableName);
            sb.Remove(0, sb.Length);

            sb.Append("SELECT ");
            foreach (string s in strColumnNameArr)
            {
                sb.Append(" MAX(CASE WHEN isdate(CAST([" + s + "] as varchar(255))) = 1 then 3 else CASE WHEN  ISNUMERIC(CAST([" + s + "] as varchar(255))) = 1 then 2 ELSE 1 END END) as  [" + s + "],");

            }
            DataTable dtFormat = DBConnection32.getMSSQLDataTable(_strILUCAConnectionString, sb.ToString().TrimEnd(',') + " FROM " + strTableName);
            sb.Remove(0, sb.Length);

            int intLength;
            string strType;
            string strNewType;
            foreach (string s in strColumnNameArr)
            {
                intLength = int.Parse(dtLength.Rows[0][s].ToString());
                strType = dtFormat.Rows[0][s].ToString();

                if (strType == "1")
                {
                    if(intLength < 6)
                    {
                        strNewType = "char("+ intLength + ")";
                    }
                    else
                    {
                        strNewType = "varchar(" + intLength + ")";
                    }
                }
                else if (strType == "2")
                {
                    if (intLength < 5)
                    {
                        strNewType = "smallint";
                    }
                    else if (intLength < 10)
                    {
                        strNewType = "int";
                    }
                    else if (intLength < 16)
                    {
                        strNewType = "bigint";
                    }
                    else
                    {
                        strNewType = "varchar(" + intLength + ")";
                    }
                }
                else
                {

                    if (intLength <= 12)
                    {
                        strNewType = "date";
                    }
                    else
                    {
                        strNewType = "varchar(" + intLength + ")";
                    }

                }



                sb.Append("ALTER TABLE " + strTableName + " ALTER COLUMN  [" + s + "] " + strNewType + ";");

            }
            DBConnection32.ExecuteMSSQL(_strILUCAConnectionString, sb.ToString());


        }



 //       SELECT MAX(LEN(case_key)) as [case_key],
	//MAX(LEN(case_cancelled_ind)) as [case_cancelled_ind]
 //       FROM[stg].[cns_sos]



 //       SELECT
 //      MAX(CASE WHEN isdate(case_key) = 1 then 3 else CASE WHEN  ISNUMERIC(case_key) = 1 then 2 ELSE 3

 //      END END)  as  [case_key],

	//	MAX(CASE WHEN isdate(case_cancelled_ind) = 1 then 3 else CASE WHEN  ISNUMERIC(case_cancelled_ind) = 1 then 2 ELSE 3

 //   END END)  as  [case_cancelled_ind],

	//		MAX(CASE WHEN isdate(entity) = 1 then 3 else CASE WHEN  ISNUMERIC(entity) = 1 then 2 ELSE 3

 //   END END)  as  [entity],


	//		MAX(CASE WHEN isdate(MedNecEligible) = 1 then 3 else CASE WHEN  ISNUMERIC(MedNecEligible) = 1 then 2 ELSE 3

 //   END END)  as  [MedNecEligible],


	//		MAX(CASE WHEN isdate(NotifYearMonth) = 1 then 3 else CASE WHEN  ISNUMERIC(NotifYearMonth) = 1 then 2 ELSE 3

 //   END END)  as  [NotifYearMonth]
 //       FROM[stg].[cns_sos]


    }
}
