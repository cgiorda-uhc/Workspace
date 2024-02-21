using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalNecessity_ACIS_Parser
{
    class MedicalNecessity_ACIS_Parser
    {
        static void Main(string[] args)
        {
            //HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of October 2023", "chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.Normal);
            HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of February 2024", "chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.Normal);
            return;


            // return;
            //HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com;sheila_donelan@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of January 2023", "jon_maguire@uhc.com", null, System.Net.Mail.MailPriority.Normal);



            //IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            //IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            //IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            //IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            //IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            //IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            //IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];
            //IR_SAS_Connect.strSASUserNameOracle = ConfigurationManager.AppSettings["SAS_UN_Oracle"];
            //IR_SAS_Connect.strSASPasswordOracle = ConfigurationManager.AppSettings["SAS_PW_Oracle"];

            //Console.Write("Connecting to SAS....");
            //IR_SAS_Connect.create_SAS_instance();

            //IR_SAS_Connect.runStoredProcess("add_curr_month.sas", "/hpsasfin/int/projects/acad/CategoryAnalytics/Common/code/code_sets");

            //IR_SAS_Connect.destroy_SAS_instance();

            //HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com;sheila_donelan@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of December 2022", "jon_maguire@uhc.com", null, System.Net.Mail.MailPriority.Normal);


            //return;




            // HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com;sheila_donelan@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of November 2022", "jon_maguire@uhc.com;chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.Normal);




            ///February 2022!!!!
            ///February 2022!!!!
            //////February 2022!!!!
            //OutlookHelper.sendEmail("jon_maguire@uhc.com;kristina_kolaczkowski@uhc.com;sheila_donelan@uhc.com", "ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of March 2022", "inna_rudi@uhc.com");



            //IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
            //IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
            //IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
            //IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
            //IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
            //IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
            //IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];
            //IR_SAS_Connect.strSASUserNameOracle = ConfigurationManager.AppSettings["SAS_UN_Oracle"];
            //IR_SAS_Connect.strSASPasswordOracle = ConfigurationManager.AppSettings["SAS_PW_Oracle"];

            //Console.Write("Connecting to SAS....");
            //IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());



            ////LATEST FROM NICK 3292022
            //IR_SAS_Connect.runStoredProcess("add_curr_month.sas", "/hpsasfin/int/projects/acad/CategoryAnalytics/Common/code/code_sets");



            ////string t = IR_SAS_Connect.sbSASLog.ToString();

            //IR_SAS_Connect.destroy_SAS_instance();


            //532022!!!!
            //ONE DRIVE
            //532022!!!!
            //ONE DRIVE
            //532022!!!!
            //ONE DRIVE
            //532022!!!!
            //ONE DRIVE
            //532022!!!!
            //ONE DRIVE
            //C:\Users\cgiorda\UHG\Medical Necessity Resource - Monthly ACIS Report for Med Nec



            //string strSharePointURL = @"https://uhgazure.sharepoint.com/sites/SBS/Projects/Medical%20Necessity%20Resource/Medical%20Necessity%20ACIS%20and%20Quarterly%20Reports%20for%20N/Forms/AllItems.aspx?RootFolder=%2Fsites%2FSBS%2FProjects%2FMedical%20Necessity%20Resource%2FMedical%20Necessity%20ACIS%20and%20Quarterly%20Reports%20for%20N%2FMonthly%20ACIS%20Report%20for%20Med%20Nec&FolderCTID=0x012000971FB377F01DFC45947863A959FD81A4&View=%7B96C9AE2C%2DF17B%2D4C3E%2DBFE2%2D8EDDEDDF5BC0%7D";
            //@"\\uhgazure.sharepoint.com\sites\SBS\Projects\Medical Necessity Resource\Medical Necessity ACIS and Quarterly Reports for N\Monthly ACIS Report for Med Nec"; ;

            Console.WriteLine("Medical_Necessity_ACIS_Parser");
            string strSharePointFolderPath = ConfigurationManager.AppSettings["Sharepoint_Path"];
            string strProcessingFolderPath = ConfigurationManager.AppSettings["Processing_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            string[] files;
            int intFileCnt = 1;
            int intRowCnt = 1;

            if (1 ==2)
            {

                //// NetworkCredential credentials = new NetworkCredential("cgiorda", "BooWooDooFoo2023!!", "ms");
                //SharePointOnlineCredentials credentials = new SharePointOnlineCredentials("chris_giordano@uhc.com", ConvertToSecureString("BooWooDooFoo2023!!"));
               // SharepointConnect.SharepointConnect.Flist3();



                //Uri filename = new Uri(@"http://server/sites/site1/subsite/doclib/folder1/folder2/prettyimage.jpg");
                //string server = filename.AbsoluteUri.Replace(filename.AbsolutePath, "");
                //string serverrelative = filename.AbsolutePath;

                //ClientContext clientContext = new ClientContext(server);
                //FileInformation f = Microsoft.SharePoint.Client.File.OpenBinaryDirect(clientContext, serverrelative);
                //clientContext.ExecuteQuery();
                //using (var fileStream = new FileStream(@"d:\prettyimage.jpg", FileMode.Create))
                //    f.Stream.CopyTo(fileStream);





                Console.WriteLine("Getting zipped files from SharePoint");
                //GET ALL FILES FROM SHAREPOINT UNZIP IF NEEDED
                files = Directory.GetFiles(strSharePointFolderPath, "*.zip", SearchOption.TopDirectoryOnly);
                intFileCnt = 1;
                intRowCnt = 1;


                foreach (string strFile in files)
                {
                    Console.Write("\rUnzipping and cleaning " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " compressed files");
                    string strExtension = Path.GetExtension(strFile);
                    string strFileName = Path.GetFileName(strFile);
                    string strFileDate = strFileName.Split('_')[4].Replace(".zip","");


                    //if (strExtension.ToLower() == ".zip")
                    //{
                    using (ZipArchive archive = ZipFile.OpenRead(strFile))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            string strTmpFile = Path.Combine(strProcessingFolderPath, entry.FullName);
                            string strTmpFileArchive = Path.Combine(strProcessingFolderPath + @"\Archive", entry.FullName);
                            if (!File.Exists(strTmpFile + "X") && !File.Exists(strTmpFileArchive + "X"))
                            {
                                entry.ExtractToFile(strTmpFile);

                                //ConvertXLS_XLSX(strTmpFile);



                            }


                        }
                    }
                    //}
                    //else if(strExtension.ToLower() == ".xlsx")
                    //{
                    //    string strTmpFile = Path.Combine(strProcessingFolderPath, strFileName);
                    //    string strTmpFileArchive = Path.Combine(strProcessingFolderPath + @"\Archive", strFileName);
                    //    if (!File.Exists(strTmpFile) && !File.Exists(strTmpFileArchive))
                    //        File.Copy(strFile, strTmpFile);
                    //}
                    intFileCnt++;

                }
            }




            Console.WriteLine();
            Console.WriteLine("Processing cleaned spreadsheets");
            //PROCESS ALL EXTRACTED FILES SKIP THOSE ALREADY PROCESSED
            string[] strArrAlreadyProcessed = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "SELECT distinct [file] from[dbo].[ACIS_MedNec_Data_Stage]").AsEnumerable().Select(r => r.Field<string>("file")).ToArray();
            string strSpreadsheetPrefixName = "ACIS_MedNec_Report_Full_";
            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            intFileCnt = 1;
            files = Directory.GetFiles(strProcessingFolderPath, "*.xlsx", SearchOption.TopDirectoryOnly);
            foreach (string strFile in files)
            {
                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");
                string strExtension = Path.GetExtension(strFile);
                string strFileName = Path.GetFileName(strFile);
                string strFileDate = strFileName.ToLower().Split('_')[4].Replace(".xlsx", "");

                if (strArrAlreadyProcessed.Contains(strFileDate))
                {
                    File.Move(strFile, Path.Combine(strProcessingFolderPath + @"\Archive", strFileName));
                    continue;
                }

                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, (strSpreadsheetPrefixName + strFileDate).Substring(0, 31), 2, 3);

                //if (dtFinalDataTable == null)
                //{
                dtFinalDataTable = dtCurrentDataTable.Clone();
                foreach (DataColumn col in dtFinalDataTable.Columns)
                {
                    col.ColumnName = col.ColumnName.Trim().Replace(" ", "_");
                    if (col.ColumnName == "Effective_Date" || col.ColumnName == "Effective_Date1")
                    {
                        col.DataType = typeof(DateTime);
                    }


                }
                dtFinalDataTable.Columns.Add("File", typeof(String));
                //}
                //else
                //{
                //    dtFinalDataTable.Clear();
                //}

                Console.WriteLine();
                intRowCnt = 1;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rCollecting " + intRowCnt + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    currentRow = dtFinalDataTable.NewRow();

                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {
                        if (c.ColumnName == "Effective Date" || c.ColumnName == "Effective Date1")
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value && !(dr[c.ColumnName] + "").Trim().Equals("") ? DateTime.FromOADate(double.Parse(dr[c.ColumnName].ToString())) : (object)DBNull.Value);
                        else
                            currentRow[c.ColumnName.Replace(" ", "_")] = (dr[c.ColumnName] != DBNull.Value && !(dr[c.ColumnName] + "").Trim().Equals("") ? dr[c.ColumnName] : DBNull.Value);
                    }


                    //foreach (DataColumn c in dtCurrentDataTable.Columns)
                    //    currentRow[c.ColumnName] = (dr[c.ColumnName] != DBNull.Value ? dr[c.ColumnName] : DBNull.Value);

                    currentRow["File"] = strFileDate;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }


                //RENAME Effective Date1  to Effective DateTmp
                //RENAME Effective Date  to Effective Date1
                //RENAME Effective DateTmp to Effective Date
                //CONVERT Effective Date1 FROM EXCEL TIME TO DATETIME DateTime.FromOADate(double.Parse(dr["DSNPRemovalDt"].ToString())) 
                dtFinalDataTable.Columns["Effective_Date1"].ColumnName = "Effective_DateTmp";
                dtFinalDataTable.Columns["Effective_Date"].ColumnName = "Effective_Date1";
                dtFinalDataTable.Columns["Effective_DateTmp"].ColumnName = "Effective_Date";



                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "ACIS_MedNec_Staging";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE dbo."+ dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                //RELEASE EXCEL
                wbCurrentExcelFile.Close();
                wbCurrentExcelFile = null; 

                //ARCHIVE AFTER PROCESS
                File.Move(strFile, Path.Combine(strProcessingFolderPath + @"\Archive", strFileName));


                intFileCnt++;
            }




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
            IR_SAS_Connect.create_SAS_instance();




            //Console.Write("....");


            //\\nasgw056pn\bi_out\PCR\Vendor_Pilots\ER_Discharge\Test_Automation
            //IR_SAS_Connect.runStoredProcess("Test_ED_ADT_Report_V2.sas", "/hpsasfin/int/nas/bi_out/PCR/Vendor_Pilots/ER_Discharge/Test_Automation");
            //IR_SAS_Connect.runStoredProcess("add_curr_month.sas", "/hpsasfin/int/projects/acad/CategoryAnalytics/Common/data/database");
            //IR_SAS_Connect.runStoredProcess("add_curr_month.sas", "/hpsasfin/int/winfiles7/Analytics/Infrastructure/Code Maintenance/ACIS MedNec/code");
            //IR_SAS_Connect.runStoredProcess("add_curr_month.sas", "/hpsasfin/int/projects/acad/CategoryAnalytics/Code/Code_sets/");
            IR_SAS_Connect.runStoredProcess("add_curr_month.sas", "/hpsasfin/int/projects/acad/CategoryAnalytics/Common/code/code_sets");

            //IR_SAS_Connect.runProcSQLCommands("proc sql;Test_ED_ADT_Report.sas;");
            //Console.Write(IR_SAS_Connect.sbSASLog.ToString());



            IR_SAS_Connect.destroy_SAS_instance();


            HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of January 2024", "chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.Normal);


           // HelperFunctions.HelperFunctions.Email("inna_rudi@uhc.com;sheila_donelan@uhc.com", "chris_giordano@uhc.com", "VC Automation Manager: ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of December 2022", "jon_maguire@uhc.com", null, System.Net.Mail.MailPriority.Normal);

            //OutlookHelper.sendEmail("inna_rudi@uhc.com;david.juola@uhc.com", "Test Mednec ACIS out", "ACIS MedNec tables were updated with data for the month of December 2021", "erica.uhrhan@uhc.com");

            //OutlookHelper.sendEmail("jon_maguire@uhc.com;kristina_kolaczkowski@uhc.com;sheila_donelan@uhc.com", "ACIS MedNec Tables", "ACIS MedNec tables were updated with data for the month of December 2021", "inna_rudi@uhc.com");
        }

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }

        public static string ConvertXLS_XLSX(string strFile)
        {

            while (!File.Exists(strFile))
            { Console.WriteLine(strFile + " does not exist!!!!!!!"); }

            var app = new Microsoft.Office.Interop.Excel.Application();
            var wb = app.Workbooks.Open(strFile);
            var xlsxFile = strFile + "X";
            wb.SaveAs(Filename: xlsxFile, FileFormat: Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook);
            wb.Close();
            app.Quit();

            foreach (Process Proc in Process.GetProcesses())
                if (Proc.ProcessName.Equals("EXCEL"))  //Process Excel?
                    Proc.Kill();

            File.Delete(strFile);

            return xlsxFile;
        }
    }
}
