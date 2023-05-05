using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEP_Cleanup
{
    class CEP_Cleanup
    {
        static void Main(string[] args)
        {

            cleanData();
            return;

            Console.WriteLine("CMS_Data_Parser");
            string strFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
            string strUHN_ConnectionString = ConfigurationManager.AppSettings["UHN_Database"];

            Console.WriteLine("Getting zipped files from Shared Drive");
            //GET ALL FILES FROM SHAREPOINT UNZIP IF NEEDED
            string[] files = Directory.GetFiles(strFolderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
            int intFileCnt = 1;
            int intRowCnt = 1;
            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable = new DataTable();
            DataTable dtResults = null;
            string strSheetName = "SSRosterList_ForEra";
            string strSysId = null;
            string strSQL = null;


            StringBuilder sbTeraDataSQL = new StringBuilder();

            //CHANGE ME!!!
            sbTeraDataSQL.Append("CREATE MULTISET VOLATILE TABLE MissingMembersTmp(MBR_FST_NM VARCHAR(10), MBR_LST_NM VARCHAR(10), BTH_DT DATE) PRIMARY INDEX(MBR_FST_NM, MBR_LST_NM, BTH_DT) ON COMMIT PRESERVE ROWS; ");


            //CHANGE ME!!!
            sbTeraDataSQL.Append("INSERT INTO MissingMembersTmp(MBR_FST_NM, MBR_LST_NM, BTH_DT) VALUES('BA%', 'ZAV%', '2021-07-01'); ");
            sbTeraDataSQL.Append("INSERT INTO MissingMembersTmp(MBR_FST_NM, MBR_LST_NM, BTH_DT) VALUES('JA%', 'GOS%', '1958-01-09'); ");
            sbTeraDataSQL.Append("INSERT INTO MissingMembersTmp(MBR_FST_NM, MBR_LST_NM, BTH_DT) VALUES('RO%', 'GAR%', '2009-10-31'); ");


            //CHANGE ME!!!
            sbTeraDataSQL.Append("COLLECT STATS COLUMN(MBR_FST_NM, MBR_LST_NM, BTH_DT) ON MissingMembersTmp; ");


            sbTeraDataSQL.Append("SELECT m.INDV_SYS_ID, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR,m.HICN as hicnbr,m.mbr_ssn as member_id, m.MBR_FST_Nm, m.MBR_LST_NM, m.BTH_DT, ");
            sbTeraDataSQL.Append("coalesce(CAST(de.EFF_DT as date), CAST('0001-01-01' as date)) AS EFF_DT, coalesce (CAST(dn.END_DT as date), CAST('0001-01-01' as date))AS END_DT, ");
            sbTeraDataSQL.Append("coalesce (CAST(m.LOAD_DT as date), CAST('0001-01-01' as date)) AS DATA_SOURCE_LOAD_DT, coalesce (CAST(m.UPDT_DT as date), CAST('0001-01-01' as date)) AS DATA_SOURCE_UPDT_DT ");
            sbTeraDataSQL.Append("FROM ");
            sbTeraDataSQL.Append("(SELECT * FROM UHCDM001.HP_member WHERE INDV_SYS_ID > 0) as m ");
            sbTeraDataSQL.Append("left join UHCDM001.CS_ENROLLMENT as e on m.MBR_SYS_ID = e.MBR_SYS_ID ");
            sbTeraDataSQL.Append("left join DATE_EFF as de on de.EFF_DT_SYS_ID = e.EFF_DT_SYS_ID ");
            sbTeraDataSQL.Append("left join DATE_END as dn on dn.END_DT_SYS_ID = e.END_DT_SYS_ID ");
            //CHANGE ME!!!
            sbTeraDataSQL.Append("inner join MissingMembersTmp as mm on m.MBR_FST_NM LIKE mm.MBR_FST_NM AND  m.MBR_LST_NM LIKE mm.MBR_LST_NM AND m.BTH_DT = mm.BTH_DT; ");


            sbTeraDataSQL.Append("drop table MissingMembersTmp; ");



            foreach (string strFile in files)
            {
                Console.Write("\rUnzipping and cleaning " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " compressed files");


                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetName,3, 4);

                // REPLACE ' ' WITH '_' in COLUMN NAME LOOP
                foreach (DataColumn col in dtCurrentDataTable.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                }



                dtFinalDataTable = dtCurrentDataTable.Clone();
                dtFinalDataTable.Columns.Add("SysId", typeof(String));


                Console.WriteLine();
                intRowCnt = 1;
                DataRow currentRow;
                string strFN = null, strLN = null, strBD = null;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rCollecting " + intRowCnt + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    currentRow = dtFinalDataTable.NewRow();

                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {
                        if (c.ColumnName == "Enrollment_Start_Date" || c.ColumnName == "Enrollment_End_Date" || c.ColumnName == "PY_Start_Date" || c.ColumnName == "PY_End_Date")
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value  ? DateTime.FromOADate(double.Parse(dr[c.ColumnName].ToString())) : (object)DBNull.Value);
                        else if (c.ColumnName == "Compare_Col_K_to_I" || c.ColumnName == "Compare_Col_M_toJ")
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value ? ((dr[c.ColumnName] + "").ToLower() == "1" ? "True" : "False")  : (object)DBNull.Value);
                        else
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value ? dr[c.ColumnName] : DBNull.Value);


                        if (c.ColumnName == "Last_Name" && dr[c.ColumnName] != DBNull.Value)
                            strLN = dr[c.ColumnName].ToString().Replace("'", "''");

                        if (c.ColumnName == "First_Name" && dr[c.ColumnName] != DBNull.Value)
                            strFN = dr[c.ColumnName].ToString().Replace("'", "''");

                        if (c.ColumnName == "Date_of_Birth" && dr[c.ColumnName] != DBNull.Value)
                            strBD = DateTime.Parse(dr[c.ColumnName].ToString()).ToString("yyyy-MM-dd");

                    }



                    //foreach (DataColumn c in dtCurrentDataTable.Columns)
                    //    currentRow[c.ColumnName] = (dr[c.ColumnName] != DBNull.Value ? dr[c.ColumnName] : DBNull.Value);



                    if (strFN != null && strLN != null && strBD != null)
                    {
                        //SYSID CHECK

                        strSQL = "SELECT distinct(m.INDV_SYS_ID) FROM (SELECT * FROM UHCDM001.HP_member WHERE INDV_SYS_ID > 0 ) as m left join UHCDM001.CS_ENROLLMENT as e on m.MBR_SYS_ID=e.MBR_SYS_ID left join DATE_EFF as de on de.EFF_DT_SYS_ID=e.EFF_DT_SYS_ID left join DATE_END as dn on dn.END_DT_SYS_ID=e.END_DT_SYS_ID WHERE m.MBR_FST_NM LIKE '%"+ strFN + "%' AND m.MBR_LST_NM LIKE '%"+ strLN + "%' AND m.BTH_DT='"+ strBD + "'";
                        dtResults = DBConnection32.getTeraDataDataTable(strUGAP_ConnectionString, strSQL);
                        
                        if(dtResults.Rows.Count == 1)
                        {
                            strSysId = dtResults.Rows[0]["INDV_SYS_ID"].ToString();
                        }
                        else
                        {
                            strSysId = null;
                        }

                    }
                    else
                        strSysId = null;

                    strFN = null; strLN = null; strBD = null;



                    currentRow["SysId"] = strSysId;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }



                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "CEP_QPR_SysId";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE dbo."+ dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);




                intFileCnt++;
            }

        }




        private static void cleanData()
        {

            Console.WriteLine("CMS_Data_Parser");
            string strFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
            string strUHN_ConnectionString = ConfigurationManager.AppSettings["UHN_Database"];

            Console.WriteLine("Getting zipped files from Shared Drive");
            //GET ALL FILES FROM SHAREPOINT UNZIP IF NEEDED
            string[] files = Directory.GetFiles(strFolderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
            int intFileCnt = 1;
            int intRowCnt = 1;
            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable = new DataTable();
            DataTable dtResults = null;
            string strSheetName = "SSRosterList_ForEra";
            string strSysId = null;
            string strSQL = null;


            StringBuilder sbTeraDataSQL = new StringBuilder();
            string strCreatColumns = null;
            string strListColumns = null;
            string strJoinColumns = null;
            string strInsertTemplate = null;
            strCreatColumns = "MBR_FST_NM VARCHAR(10), MBR_LST_NM VARCHAR(10), BTH_DT DATE";
            strListColumns = "MBR_FST_NM, MBR_LST_NM, BTH_DT";
            strJoinColumns = "m.MBR_FST_NM LIKE mm.MBR_FST_NM AND  m.MBR_LST_NM LIKE mm.MBR_LST_NM AND m.BTH_DT = mm.BTH_DT";
            strCreatColumns = "SBSCR_MEDCD_RCIP_NBR VARCHAR(16)";
            strListColumns = "SBSCR_MEDCD_RCIP_NBR";
            strJoinColumns = "m.SBSCR_MEDCD_RCIP_NBR = mm.SBSCR_MEDCD_RCIP_NBR";
            strCreatColumns = "SBSCR_NBR VARCHAR(11)";
            strListColumns = "SBSCR_NBR";
            strJoinColumns = "m.SBSCR_NBR = mm.SBSCR_NBR";
            strInsertTemplate = "INSERT INTO MissingMembersTmp(" + strListColumns + ") VALUES('BA%', 'ZAV%', '2021-07-01');";

            sbTeraDataSQL.Append("CREATE MULTISET VOLATILE TABLE MissingMembersTmp("+ strCreatColumns + ") PRIMARY INDEX(" + strListColumns + ") ON COMMIT PRESERVE ROWS; ");
            //sbTeraDataSQL.Append("CREATE MULTISET VOLATILE TABLE MissingMembersTmp(MBR_FST_NM VARCHAR(10), MBR_LST_NM VARCHAR(10), BTH_DT DATE) PRIMARY INDEX(MBR_FST_NM, MBR_LST_NM, BTH_DT) ON COMMIT PRESERVE ROWS; ");


            //LOOP HERE!!!
            sbTeraDataSQL.Append(strInsertTemplate);
            //sbTeraDataSQL.Append("INSERT INTO MissingMembersTmp(MBR_FST_NM, MBR_LST_NM, BTH_DT) VALUES('JA%', 'GOS%', '1958-01-09'); ");


            sbTeraDataSQL.Append("COLLECT STATS COLUMN(" + strListColumns + ") ON MissingMembersTmp; ");
            //sbTeraDataSQL.Append("COLLECT STATS COLUMN(MBR_FST_NM, MBR_LST_NM, BTH_DT) ON MissingMembersTmp; ");


            sbTeraDataSQL.Append("SELECT m.INDV_SYS_ID, m.SBSCR_MEDCD_RCIP_NBR, m.SBSCR_NBR,m.HICN as hicnbr,m.mbr_ssn as member_id, m.MBR_FST_Nm, m.MBR_LST_NM, m.BTH_DT, ");
            sbTeraDataSQL.Append("coalesce(CAST(de.EFF_DT as date), CAST('0001-01-01' as date)) AS EFF_DT, coalesce (CAST(dn.END_DT as date), CAST('0001-01-01' as date))AS END_DT, ");
            sbTeraDataSQL.Append("coalesce (CAST(m.LOAD_DT as date), CAST('0001-01-01' as date)) AS DATA_SOURCE_LOAD_DT, coalesce (CAST(m.UPDT_DT as date), CAST('0001-01-01' as date)) AS DATA_SOURCE_UPDT_DT ");
            sbTeraDataSQL.Append("FROM ");
            sbTeraDataSQL.Append("(SELECT * FROM UHCDM001.HP_member WHERE INDV_SYS_ID > 0) as m ");
            sbTeraDataSQL.Append("left join UHCDM001.CS_ENROLLMENT as e on m.MBR_SYS_ID = e.MBR_SYS_ID ");
            sbTeraDataSQL.Append("left join DATE_EFF as de on de.EFF_DT_SYS_ID = e.EFF_DT_SYS_ID ");
            sbTeraDataSQL.Append("left join DATE_END as dn on dn.END_DT_SYS_ID = e.END_DT_SYS_ID ");

            
            //CHANGE ME!!!
            sbTeraDataSQL.Append("inner join MissingMembersTmp as mm on "+ strJoinColumns + "; ");
            //sbTeraDataSQL.Append("inner join MissingMembersTmp as mm on m.MBR_FST_NM LIKE mm.MBR_FST_NM AND  m.MBR_LST_NM LIKE mm.MBR_LST_NM AND m.BTH_DT = mm.BTH_DT; ");

            sbTeraDataSQL.Append("drop table MissingMembersTmp; ");



            foreach (string strFile in files)
            {
                Console.Write("\rUnzipping and cleaning " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " compressed files");


                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetName, 3, 4);

                // REPLACE ' ' WITH '_' in COLUMN NAME LOOP
                foreach (DataColumn col in dtCurrentDataTable.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                }



                dtFinalDataTable = dtCurrentDataTable.Clone();
                dtFinalDataTable.Columns.Add("SysId", typeof(String));


                Console.WriteLine();
                intRowCnt = 1;
                DataRow currentRow;
                string strFN = null, strLN = null, strBD = null;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rCollecting " + intRowCnt + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    currentRow = dtFinalDataTable.NewRow();

                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {
                        if (c.ColumnName == "Enrollment_Start_Date" || c.ColumnName == "Enrollment_End_Date" || c.ColumnName == "PY_Start_Date" || c.ColumnName == "PY_End_Date")
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value ? DateTime.FromOADate(double.Parse(dr[c.ColumnName].ToString())) : (object)DBNull.Value);
                        else if (c.ColumnName == "Compare_Col_K_to_I" || c.ColumnName == "Compare_Col_M_toJ")
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value ? ((dr[c.ColumnName] + "").ToLower() == "1" ? "True" : "False") : (object)DBNull.Value);
                        else
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value ? dr[c.ColumnName] : DBNull.Value);


                        if (c.ColumnName == "Last_Name" && dr[c.ColumnName] != DBNull.Value)
                            strLN = dr[c.ColumnName].ToString().Replace("'", "''");

                        if (c.ColumnName == "First_Name" && dr[c.ColumnName] != DBNull.Value)
                            strFN = dr[c.ColumnName].ToString().Replace("'", "''");

                        if (c.ColumnName == "Date_of_Birth" && dr[c.ColumnName] != DBNull.Value)
                            strBD = DateTime.Parse(dr[c.ColumnName].ToString()).ToString("yyyy-MM-dd");

                    }



                    //foreach (DataColumn c in dtCurrentDataTable.Columns)
                    //    currentRow[c.ColumnName] = (dr[c.ColumnName] != DBNull.Value ? dr[c.ColumnName] : DBNull.Value);



                    if (strFN != null && strLN != null && strBD != null)
                    {
                        //SYSID CHECK

                        strSQL = "SELECT distinct(m.INDV_SYS_ID) FROM (SELECT * FROM UHCDM001.HP_member WHERE INDV_SYS_ID > 0 ) as m left join UHCDM001.CS_ENROLLMENT as e on m.MBR_SYS_ID=e.MBR_SYS_ID left join DATE_EFF as de on de.EFF_DT_SYS_ID=e.EFF_DT_SYS_ID left join DATE_END as dn on dn.END_DT_SYS_ID=e.END_DT_SYS_ID WHERE m.MBR_FST_NM LIKE '%" + strFN + "%' AND m.MBR_LST_NM LIKE '%" + strLN + "%' AND m.BTH_DT='" + strBD + "'";
                        dtResults = DBConnection32.getTeraDataDataTable(strUGAP_ConnectionString, strSQL);

                        if (dtResults.Rows.Count == 1)
                        {
                            strSysId = dtResults.Rows[0]["INDV_SYS_ID"].ToString();
                        }
                        else
                        {
                            strSysId = null;
                        }

                    }
                    else
                        strSysId = null;

                    strFN = null; strLN = null; strBD = null;



                    currentRow["SysId"] = strSysId;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }



                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "CEP_QPR_SysId";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE dbo." + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);




                intFileCnt++;
            }

        }





        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }
    }
}
