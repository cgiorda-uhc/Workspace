using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.IO.Compression;
using DocumentFormat.OpenXml.Packaging;
using System.Data;
using System.Data.SqlClient;

namespace CMS_File_Parser
{
    class CMS_File_Parser
    {
        //        CMS
        //Addendum B

        //\\nasv0009\ONC_UHG_Emp_WIN_SAS\Analytics\Infrastructure\CMS_OPPS_APC_SI_Source_Files
        //https://www.cms.gov/Medicare/Medicare-Fee-for-Service-Payment/HospitalOutpatientPPS/Addendum-A-and-Addendum-B-Updates 


        /*SELECT  [HCPCS_Code]
      ,[Short_Descriptor]
      ,[SI]
      ,[APC]
      ,MIN([CMS_Date]) as min_date
	  ,MAX([CMS_Date]) as max_date
	  ,COUNT([HCPCS_Code]) as cnt
  FROM [IL_UCA].[dbo].[CMS_Addendum]
  GROUP BY  [HCPCS_Code]
      ,[Short_Descriptor]
      ,[SI]
      ,[APC]
	 --HAVING COUNT([HCPCS_Code]) < (SELECT count(*) from (SELECT distinct [CMS_Date]   FROM [IL_UCA].[dbo].[CMS_Addendum]) t)*/

        static void Main(string[] args)
        {
            PythonConnect.PythonConnect.runPythonCmd();
            //PythonConnect.PythonConnect.runPythonEngine(@"C:\Users\cgiorda\PycharmProjects\SteveCrawler\GetCMSFile_20220111.py", @"C:\Python36\Lib", @"C:\Users\cgiorda\PycharmProjects\SteveCrawler\venv\Lib\site-packages");
            return;

            Console.WriteLine("CMS_Data_Parser");
            string strFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            //Console.WriteLine("Getting zipped files from Shared Drive");
            ////GET ALL FILES FROM SHAREPOINT UNZIP IF NEEDED
            string[] files; 
            //files = Directory.GetFiles(strFolderPath, "*.zip", SearchOption.TopDirectoryOnly);
            int intFileCnt = 1;
            int intRowCnt = 1;
            //foreach (string strFile in files)
            //{
            //    Console.Write("\rUnzipping and cleaning " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " compressed files");
            //    using (ZipArchive archive = ZipFile.OpenRead(strFile))
            //    {
            //        foreach (ZipArchiveEntry entry in archive.Entries)
            //        {
            //            if (!entry.FullName.ToLower().EndsWith("xlsx"))
            //                continue;

            //            string strTmpFile = Path.Combine(strFolderPath, entry.FullName);

            //            entry.ExtractToFile(strTmpFile);
            //        }
            //    }

            //    intFileCnt++;
            //}

            //foreach (string strFile in files)
            //{
            //    File.Delete(strFile);
            //}


            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("HCPCS_Code", typeof(String));
            dtFinalDataTable.Columns.Add("SI", typeof(String));
            dtFinalDataTable.Columns.Add("APC", typeof(String));
            dtFinalDataTable.Columns.Add("Short_Descriptor", typeof(String));
            dtFinalDataTable.Columns.Add("CMS_Date", typeof(DateTime));




            DataRow currentRow;
            string[] strMonthNamesArr = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames;
            string[] strFileNamePartsArr;
            string strYear = null, strMonth = null;
            string strSheetName;
            DateTime dtCMSDate;
            //CHEATING!!!!!
            strFolderPath = @"C:\Users\cgiorda\Desktop\CMS_Download";
            intFileCnt = 1;
            files = Directory.GetFiles(strFolderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
            foreach (string strFile in files)
            {
                //var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
                //2021 April Addendum B
                //2021 October Addendum B
                //2021 July Addendum B
                //2022 January Addendum B



                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");
                strFileNamePartsArr = Path.GetFileName(strFile).Split('_');
                foreach (string s in strFileNamePartsArr)
                {
                    if (strYear != null && strMonth != null)
                        break;


                    if (int.TryParse(s, out int n))
                    {
                        strYear = s;
                    }
                    else if(strMonthNamesArr.Contains(s))
                    {
                        strMonth = s;
                    }
                }


                strSheetName = strYear + " " + strMonth +  " Addendum B";
                dtCMSDate = DateTime.Parse(strMonth + " 1, " + strYear);

                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetName, 4, 5);

               

                Console.WriteLine();
                intRowCnt = 1;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rCollecting " + intRowCnt + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    currentRow = dtFinalDataTable.NewRow();

                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {
                        if (!dtFinalDataTable.Columns.Contains(c.ColumnName.Replace(" ", "_")))
                            continue;

                        currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value && !(dr[c.ColumnName] + "").Trim().Equals("") ? dr[c.ColumnName].ToString().Trim() : (object)DBNull.Value);
                    }


                    //foreach (DataColumn c in dtCurrentDataTable.Columns)
                    //    currentRow[c.ColumnName] = (dr[c.ColumnName] != DBNull.Value ? dr[c.ColumnName] : DBNull.Value);

                    currentRow["CMS_Date"] = dtCMSDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }


                strYear = null; strMonth = null;
                intFileCnt++;
            }

            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
            dtFinalDataTable.TableName = "CMS_Addendum";
            DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
            //DBConnection32.ExecuteMSSQL(strConnectionString, "TRUNCATE TABLE dbo."+ dtFinalDataTable.TableName + ";");
            DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);







            IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];
            IR_SAS_Connect.strSASUserNameOracle = ConfigurationManager.AppSettings["SAS_UN_Oracle"];
            IR_SAS_Connect.strSASPasswordOracle = ConfigurationManager.AppSettings["SAS_PW_Oracle"];

            Console.Write("Connecting to SAS....");
            IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());


            //IR_SAS_Connect.runStoredProcess("cms_insert.sas", "/hpsasfin/int/winfiles7/Analytics/Infrastructure/CMS_OPPS_APC_SI_Source_Files/CMS_files");
            IR_SAS_Connect.runStoredProcess("cms_insert.sas", "/hpsasfin/int/winfiles7/Analytics/Infrastructure/Code Maintenance/ACIS MedNec/code");
            //\\nasv0009\ONC_UHG_Emp_WIN_SAS\Analytics\Infrastructure\CMS_OPPS_APC_SI_Source_Files\CMS_Files

            Console.Write(IR_SAS_Connect.sbSASLog.ToString());

            IR_SAS_Connect.destroy_SAS_instance();


            OutlookHelper.sendEmail("jon_maguire@uhc.com;kristina_kolaczkowski@uhc.com;sheila_donelan@uhc.com", "CMS OPPS Addendum B", "CMS OPPS Addendum B table was updated with data for the month of January 2022", "inna_rudi@uhc.com");
            return;






        }


        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }
    }
}
