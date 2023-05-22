using DocumentFormat.OpenXml.Packaging;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Text;

namespace EviCore_Parser
{
    class EviCore_Parser
    {

        private static EventLog _eventLog;
        private static int _eventId;
        static void Main(string[] args)
        {

            //INITIALIZE EVENT LOGGING
            _eventId = 1;
            _eventLog = new EventLog();
            if (!EventLog.SourceExists("UCS_Automation_Manager"))
            {
                EventLog.CreateEventSource("UCS_Automation_Manager", "Application");
            }
            _eventLog.Source = "UCS_Automation_Manager";
            _eventLog.Log = "Application";

            bool blUpdated = false;

            getScorecardData();

            //getCiscoYTDMetricsData();

            //getSiteOfCare2();
            //getScorecardData();
            //getSiteOfCare3();

            // getCiscoYTDMetricsData();

            //getNICEUHCWestEligibilityData();

            //getSiteOfCare2();

            //getAmerichoiceAllstatesAuthsData();


            //SharepointConnect.SharepointConnect.SharePointUpload();






            //getSiteOfCare2();

            //getAmerichoiceAllstatesAuthsData();

            //SharepointConnect.SharepointConnect.SharePointUpload();




            return;




            getAmerichoiceAllstatesAuthsData();

            // getNICEUHCWestEligibilityData();
            // getMRMembershipData();

            // getAmerichoiceAllstatesAuthsData();

            //check_EVICORE();
     
           // getScorecardData();

            try
            {
                blUpdated = getSiteOfCare2();
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getSiteOfCare2() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getSiteOfCare2() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);
            }



            //getSiteOfCare();
            //getSiteOfCare();
            //getSiteOfCare();
            try
            {
                blUpdated = getSiteOfCareCS();
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getSiteOfCareCS() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getSiteOfCareCS() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);
            }




            //getSiteOfCare();
            //getSiteOfCare();
            //getSiteOfCare();
            try
            {
            blUpdated = getSiteOfCare();
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getSiteOfCare() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getSiteOfCare() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);
            }

           // return;

            //getMRMembershipData();
            //getMRMembershipData();
            //getMRMembershipData();
            try
            {
                blUpdated = getMRMembershipData();
                if(blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getMRMembershipData() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getMRMembershipData() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);

            }

            //getNICEUHCWestEligibilityData();
            //getNICEUHCWestEligibilityData();
            //getNICEUHCWestEligibilityData();
            try
            {
                blUpdated = getNICEUHCWestEligibilityData();
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getNICEUHCWestEligibilityData() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getNICEUHCWestEligibilityData() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);

            }

            //getAmerichoiceAllstatesAuthsData();
            //getAmerichoiceAllstatesAuthsData();
            //getAmerichoiceAllstatesAuthsData();
            try
            {
                blUpdated = getAmerichoiceAllstatesAuthsData(); //C&S VOLUME
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getAmerichoiceAllstatesAuthsData() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getAmerichoiceAllstatesAuthsData() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);

            }

            //getScorecardData();
            //getScorecardData();
            //getScorecardData();
            try
            {
                blUpdated = getScorecardData(); //OPERATIONAL EVENT
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getScorecardData() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getScorecardData() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);

            }

            //getCiscoYTDMetricsData();
            //getCiscoYTDMetricsData();
            //getCiscoYTDMetricsData();
            try
            {
                blUpdated = getCiscoYTDMetricsData(); //CALL CENTER REPORT
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getCiscoYTDMetricsData() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getCiscoYTDMetricsData() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);

            }

            //getTATData();
            //getTATData();
            //getTATData();
            try
            {
                blUpdated = getTATData(); //TAT Report
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getTATData() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getTATData() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);

            }

            //getAVTAR();
            //getAVTAR();
            //getAVTAR();
            try
            {
                blUpdated = getAVTAR();
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("EviCore_Parser getAVTAR() Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("EviCore_Parser getAVTAR() Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);
            }

        }





        private static void check_EVICORE()
        {

            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strSQL = "SELECT distinct t.[file_date], t.[file_name], t.file_search, t.file_path , t.tablename, t.search_sub FROM [dbo].[VW_Evicore_All_Data] t INNER JOIN ( SELECT MAX([file_date]) as [file_date], tablename FROM [dbo].[VW_Evicore_All_Data] GROUP BY tablename ) tmp ON t.tablename = tmp.tablename AND t.[file_date] = tmp.[file_date] ORDER BY t.tablename, t.[file_name]";

            //CACHE FOR TESTING
            strSQL = "SELECT * FROM stg.Check_Evicore_Cache ORDER BY tablename, file_name";
            //strSQL = "SELECT * FROM stg.Check_Evicore_Cache WHERE tablename LIKE 'stg.MHP_Yearly_%' ORDER BY tablename, file_name";



            DataTable dtLast = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, strSQL);


            StringBuilder sb = new StringBuilder();


            /*SELECT * INTO stg.Check_Evicore_Cache FROM (




SELECT distinct t.[file_date], t.[file_name], t.file_search, t.file_path , t.tablename, search_sub FROM [dbo].[VW_Evicore_All_Data] t 
INNER JOIN ( SELECT MAX([file_date]) as [file_date], tablename FROM [dbo].[VW_Evicore_All_Data] 
GROUP BY tablename ) tmp ON t.tablename = tmp.tablename AND t.[file_date] = tmp.[file_date] 

)tmp


DROP TABLE stg.Check_Evicore_Cache;
*/


            //              = Regex.Match(subjectString, @"\d+").Value;

            string[] strPartArr;
            int? intMonth = null;
            int? intYear = null;
            int? intQuarter = null;
            string strCheck = null;
            int? intMonthCheck = null;
            int? intYearCheck = null;
            List<string> lstNewFile = null;
            int intCheckCnt = 1;
            foreach (DataRow dr in dtLast.Rows)
            {

                var subsearch = Convert.ToBoolean(dr["search_sub"]);

                var tableName = dr["tablename"].ToString();
                var fileNameFull = dr["file_name"].ToString();
                var fileName = dr["file_name"].ToString().Replace(".xlsb", "").Replace(".zip", "").Replace(".xlsx", "").Replace(".xls", "");
                var filePath = dr["file_path"].ToString();
                var fileSearch = dr["file_search"].ToString();
                strPartArr = fileName.Split('_');

                if (strPartArr.Length > 1)
                {
                    foreach (string s in strPartArr)
                    {
                        if (s.All(char.IsNumber))
                        {
                            if (s.Length == 2)
                            {
                                intMonth = int.Parse(s);
                            }
                            else if (s.Length == 4)
                            {
                                intYear = int.Parse(s);
                            }
                            else if (s.Length == 6)
                            {
                                intYear = int.Parse(s.Substring(0, 4));
                                intMonth = int.Parse(s.Substring(4, 2));
                            }

                        }
                    }



                    if(intMonth == null)
                    {
                        DateTime dt;
                        foreach (string s in strPartArr)
                        {
                            var isInt = DateTime.TryParseExact(s.Trim(), "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out dt);
                            if(isInt)
                            {
                                intMonth = dt.Month;
                                break;
                            }
                        }
                    }



                    //if (intYear == null)
                    //{
                    //    foreach (string s in strPartArr)
                    //    {

                    //        strCheck = Regex.Match(s.ToLower().Replace("q1", "").Replace("q2", "").Replace("q3", "").Replace("q4", ""), @"\d+").Value;
                    //        if (strCheck.Length == 4)
                    //        {
                    //            intYear = int.Parse(strCheck);
                    //            break;
                    //        }
                    //    }
                    //}


                }
                else
                {
                    if (fileName.ToLower().Contains("q1"))
                    {
                        intQuarter = 1;
                    }
                    else if (fileName.ToLower().Contains("q2"))
                    {
                        intQuarter = 2;
                    }
                    else if (fileName.ToLower().Contains("q3"))
                    {
                        intQuarter = 3;
                    }
                    else if (fileName.ToLower().Contains("q4"))
                    {
                        intQuarter = 4;
                    }


                    strCheck = Regex.Match(fileName, @"\d+").Value;
                    if (strCheck.Length == 4)
                    {
                        intYear = int.Parse(strCheck);
                    }
                }



                
                if (intYear != null && intMonth != null)
                {
                    string[] strFileSearchArr = fileSearch.Split('|');

                    string strLastYearPath = "";
                    if(subsearch == true)
                    {
                        strLastYearPath = @"\2022";
                    }



                    foreach(string fs in strFileSearchArr)
                    {
                        List<string> files = Directory.EnumerateFiles(filePath + strLastYearPath, fs.Trim(), (subsearch == true ?  SearchOption.AllDirectories :SearchOption.TopDirectoryOnly)).ToList();
                        foreach (string s in files)
                        {

                            if (s.ToLower().Contains("summary") || s.ToLower().Contains("preview") || s.ToLower().Contains("edited") || s.ToLower().Contains("orig") || s.ToLower().Contains("variance"))
                                continue;


                            //CHECK FOR NEW AGAINST 
                            fileName = Path.GetFileName(s).Replace(".xlsb", "").Replace(".zip", "").Replace(".xlsx", "").Replace(".xls", "");
                            strPartArr = fileName.Split('_');
                            if (strPartArr.Length > 1)
                            {
                                foreach (string p in strPartArr)
                                {
                                    if (p.All(char.IsNumber))
                                    {
                                        if (p.Length == 2)
                                        {
                                            intMonthCheck = int.Parse(p);
                                        }
                                        else if (p.Length == 4)
                                        {
                                            intYearCheck = int.Parse(p);
                                        }
                                        else if (p.Length == 6)
                                        {
                                            intYearCheck = int.Parse(p.Substring(0, 4));
                                            intMonthCheck = int.Parse(p.Substring(4, 2));
                                        }

                                    }
                                }



                                if (intYearCheck == null)
                                {
                                    foreach (string p in strPartArr)
                                    {

                                        strCheck = Regex.Match(p.ToLower().Replace("q1", "").Replace("q2", "").Replace("q3", "").Replace("q4", ""), @"\d+").Value;
                                        if (strCheck.Length == 4)
                                        {
                                            intYearCheck = int.Parse(strCheck);
                                            break;
                                        }
                                    }
                                }


                                //  var tableName = dr["tablename"].ToString();
                                // var fileNameFull = dr["file_name].ToString();


                                if ((intYearCheck == intYear && intMonthCheck > intMonth) || (intYearCheck > intYear))
                                {
                                    if (lstNewFile == null)
                                        lstNewFile = new List<string>();


                                    lstNewFile.Add(Path.GetFileName(s));
                                    


                                }


                            }

                           

                        }
                    }




                    sb.Append("<b>Check #" + intCheckCnt + " for <font color=\"green\">" + tableName.TrimEnd('_','c').TrimEnd('_', 'r').TrimEnd('_', 'c','s').TrimEnd('_', 'p','c','p').TrimEnd('_', 'o','x') + "</font></b><br/>");
                    sb.Append("<b>Path searched:</b> " + filePath + "<br/>");
                    sb.Append("<b>Last file loaded:</b>  " + fileNameFull + "<br/>");
                    if (intYearCheck != null && intMonthCheck != null)
                    {
                        if (lstNewFile != null)
                        {
                            sb.Append("<div style=\"background-color:#90EE90\"><b>New file(s) found: " + string.Join(",", lstNewFile) + "</b></div><br/>");
                        }
                        else
                            sb.Append("No new file found. Will try again tomorrow.<br/>");

                    }
                    else
                        sb.Append("No new file found. Will try again tomorrow.<br/>");


                    sb.Append("--------------------------------------------------------------------<br/>&nbsp;<br/>");

                    intCheckCnt++;


                    intYear = null;
                    intMonth = null;
                    intYearCheck = null;
                    intMonthCheck = null;
                    lstNewFile = null;

                }
            }


            HelperFunctions.HelperFunctions.Email("chris_giordano@uhc.com", "chris_giordano@uhc.com", "Parser Check", sb.ToString(), null, null, System.Net.Mail.MailPriority.Normal);

            //HelperFunctions.HelperFunctions.Email("lindsey_ross@uhc.com;jon_maguire@uhc.com;heather.kolnick@uhc.com;kristine_m_koepke@uhc.com;hong_gao@uhc.com;stephen.quack@uhc.com;chris_giordano@uhc.com", "chris_giordano@uhc.com", "Parser Check", sb.ToString(), null, null, System.Net.Mail.MailPriority.Normal);


        }

        private static bool getSiteOfCare3()
        {

            bool blUpdated = false;

            Console.WriteLine("Site Of Care Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["SiteOfCare2File_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["SiteOfCare2Final_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");


            int intStartingMonth = 7;
            int intStartingYear = 2022;


            string strFileName = null;
            string lastFolderName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strTableName = "stg.SiteOfCare_Data_v3";
            object objCurrentValue = null;
            //string[] files;
            List<string> files;
            DateTime temp;

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from " + strTableName);
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            DataColumnCollection columns;


            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");
            //ONLY ONE TIME PER LOOP
            dtFinalDataTable = new DataTable();

            dtFinalDataTable.Columns.Add("subcarrier".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("EncounterID".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("encounterdatekey".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("ENCOUNTERFINALCLOSEDDATEKEY", typeof(String));
            dtFinalDataTable.Columns.Add("proceduresequencenum".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("encounterclinicalstatuslevel1".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("procedurecode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("procedurestatusdesc".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("requestedunits".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("approvedunits".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socenabledcptyn".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socoverturndesc".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socmemberinscope".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("soccategory".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socinitialfactype".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCFinalFacType".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socdecision".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCWaterfallCat".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socattestation".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCApprovalReason".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringprovidertin".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringprovidernpi".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringproviderfullname".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("ReferringProviderZipCode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringproviderstate".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("patientcarriermemberid".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialrequestedprovidertin".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialRequestedProviderFullName".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialrequestedproviderstate".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialRequestedProviderZipCode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("finalrequestedprovidertin".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("FinalRequestedProviderFullName".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("finalrequestedproviderstate".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("FinalRequestedProviderZipCode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCWorkable".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("ENCOUNTERSTANDARDPRODUCTLEVEL1".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));

            dtFinalDataTable.TableName = strTableName;





            //TMP??? DOWNLOAD ALL FILES
            //RESET FINAL TABLE!!!
            if (1 == 2)
            {
                files = Directory.EnumerateFiles(strFileFolderPath, "*.xlsx", SearchOption.AllDirectories).Where(s => (s.ToString().ToLower().Replace("_", " ").Contains("site of care"))).ToList();


                //files = new List<string>();
                // files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");
                //files = Directory.GetFiles(strFileFolderPath, "CRC_Pivot_Rawdata_*", SearchOption.AllDirectories);


                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile));
                    strMonth = lastFolderName.Replace("-", "").Substring(4, 2);
                    strYear = lastFolderName.Replace("-", "").Substring(0, 4);
                    strFileName = Path.GetFileName(strFile);
                    strReportType = "SiteOfCare_v2";
                    strFilePath = Path.GetDirectoryName(strFile);


                    //IGNORE OLD FOLDERS
                    if (Int32.Parse(strYear) < intStartingYear || (Int32.Parse(strYear) == intStartingYear && Int32.Parse(strMonth) < intStartingMonth))
                    {
                        continue;
                    }


                    if (!Directory.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\"))
                    {
                        Directory.CreateDirectory(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\");

                        if (!File.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName))
                        {
                            File.Copy(strFile, strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName);
                        }


                    }



                }
            }

            strFinalFileFolderPath = @"C:\Users\cgiorda\Desktop\Projects\SiteOfCare";

            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\SiteOfCare\2019\201906\UHC_Site_of_Care_Program_Reporting_November 2019.xls");

            //files = Directory.EnumerateFiles(strFinalFileFolderPath, "*.xls*", SearchOption.AllDirectories).ToList();
            //files = Directory.EnumerateFiles(strFinalFileFolderPath, "Site of Care Report_*_*.xlsx", SearchOption.AllDirectories).ToList();
            files = new List<string>();
            files.Add(@"C:\Users\cgiorda\Desktop\Projects\SiteOfCare\Site of Care Report_2023_01.xlsx");
            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");


            string[] strFileNameArr;
            strSheetname = "Case Detail";

            intFileCnt = 1;
            foreach (string strFile in files)
            {

                strFileName = Path.GetFileName(strFile);
                strFileNameArr = strFileName.ToLower().Trim().Replace(".xlsx", "").Split('_');
                strMonth = strFileNameArr[strFileNameArr.Length - 1].Trim();
                strYear = strFileNameArr[strFileNameArr.Length - 2].Trim();

                strReportType = "SiteOfCare_v3";
                strFilePath = Path.GetDirectoryName(strFile);

                if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }

                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");


                try
                {
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

                }
                catch (Exception ex)
                {
                    SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
                }




                // 
                columns = dtCurrentDataTable.Columns;
                Console.Write("\rFile to DataTable");
                // strSummaryofLOB = strFolder.Split('_')[1];

                intRowCnt = 1;

                foreach (DataRow d in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");


                    currentRow = dtFinalDataTable.NewRow();
                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {

                        var list = dtFinalDataTable.Columns.OfType<DataColumn>().Select(s => s.ColumnName).ToList();
                        //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                        if (list.Contains(c.ColumnName, StringComparer.OrdinalIgnoreCase))
                        {
                            currentRow[c.ColumnName.ToUpper()] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                        }

                    }


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;


                if (dtFinalDataTable.Rows.Count > 0)
                {
                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                    blUpdated = true;
                }


                dtFinalDataTable.Clear();

                intFileCnt++;

            }


            return blUpdated;
        }




        private static bool getSiteOfCare2()
        {

            bool blUpdated = false;

            Console.WriteLine("Site Of Care Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["SiteOfCare2File_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["SiteOfCare2Final_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");


            int intStartingMonth = 7;
            int intStartingYear = 2022;


            string strFileName = null;
            string lastFolderName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strTableName = "stg.SiteOfCare_Data_v3";
            object objCurrentValue = null;
            //string[] files;
            List<string> files;
            DateTime temp;

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from " + strTableName);
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            DataColumnCollection columns;


            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");
            //ONLY ONE TIME PER LOOP
            dtFinalDataTable = new DataTable();

            dtFinalDataTable.Columns.Add("subcarrier".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("EncounterID".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("encounterdatekey".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("ENCOUNTERFINALCLOSEDDATEKEY", typeof(String));
            dtFinalDataTable.Columns.Add("proceduresequencenum".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("encounterclinicalstatuslevel1".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("procedurecode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("procedurestatusdesc".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("requestedunits".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("approvedunits".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socenabledcptyn".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socoverturndesc".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socmemberinscope".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("soccategory".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socinitialfactype".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCFinalFacType".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socdecision".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCWaterfallCat".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("socattestation".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCApprovalReason".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringprovidertin".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringprovidernpi".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringproviderfullname".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("ReferringProviderZipCode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("referringproviderstate".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("patientcarriermemberid".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialrequestedprovidertin".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialRequestedProviderFullName".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialrequestedproviderstate".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("initialRequestedProviderZipCode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("finalrequestedprovidertin".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("FinalRequestedProviderFullName".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("finalrequestedproviderstate".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("FinalRequestedProviderZipCode".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("SOCWorkable".ToUpper(), typeof(String));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));

            dtFinalDataTable.TableName = strTableName;





            //TMP??? DOWNLOAD ALL FILES
            //RESET FINAL TABLE!!!
            if (1 == 2)
            {
                files = Directory.EnumerateFiles(strFileFolderPath, "*.xlsx", SearchOption.AllDirectories).Where(s => (s.ToString().ToLower().Replace("_", " ").Contains("site of care"))).ToList();


                //files = new List<string>();
                // files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");
                //files = Directory.GetFiles(strFileFolderPath, "CRC_Pivot_Rawdata_*", SearchOption.AllDirectories);


                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile));
                    strMonth = lastFolderName.Replace("-", "").Substring(4, 2);
                    strYear = lastFolderName.Replace("-", "").Substring(0, 4);
                    strFileName = Path.GetFileName(strFile);
                    strReportType = "SiteOfCare_v2";
                    strFilePath = Path.GetDirectoryName(strFile);


                    //IGNORE OLD FOLDERS
                    if (Int32.Parse(strYear) < intStartingYear || (Int32.Parse(strYear) == intStartingYear && Int32.Parse(strMonth) < intStartingMonth))
                    {
                        continue;
                    }


                    if (!Directory.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\"))
                    {
                        Directory.CreateDirectory(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\");

                        if (!File.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName))
                        {
                            File.Copy(strFile, strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName);
                        }


                    }



                }
            }

            strFinalFileFolderPath = @"C:\Users\cgiorda\Desktop\Projects\SiteOfCare";

            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\SiteOfCare\2019\201906\UHC_Site_of_Care_Program_Reporting_November 2019.xls");

            //files = Directory.EnumerateFiles(strFinalFileFolderPath, "*.xls*", SearchOption.AllDirectories).ToList();
            //files = Directory.EnumerateFiles(strFinalFileFolderPath, "Site of Care Report_*_*.xlsx", SearchOption.AllDirectories).ToList();
            files = new List<string>();
            files.Add(@"C:\Users\cgiorda\Desktop\Projects\SiteOfCare\Site of Care Report_2023_02.xlsx");
            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");


            string[] strFileNameArr;
            strSheetname = "Case Detail";

            intFileCnt = 1;
            foreach (string strFile in files)
            {

                strFileName = Path.GetFileName(strFile);
                strFileNameArr = strFileName.ToLower().Trim().Replace(".xlsx", "").Split('_');
                strMonth = strFileNameArr[strFileNameArr.Length - 1].Trim();
                strYear = strFileNameArr[strFileNameArr.Length - 2].Trim();

                strReportType = "SiteOfCare_v3";
                strFilePath = Path.GetDirectoryName(strFile);

                if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }

                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");


                try
                {
                    SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);


                    //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

                }
                catch (Exception ex)
                {
                    SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
                }




                // 
                columns = dtCurrentDataTable.Columns;
                Console.Write("\rFile to DataTable");
                // strSummaryofLOB = strFolder.Split('_')[1];

                intRowCnt = 1;

                foreach (DataRow d in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");


                    currentRow = dtFinalDataTable.NewRow();
                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {

                        var list = dtFinalDataTable.Columns.OfType<DataColumn>().Select(s => s.ColumnName).ToList();
                            //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                        if (list.Contains(c.ColumnName, StringComparer.OrdinalIgnoreCase))
                        {
                            currentRow[c.ColumnName.ToUpper()] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                        }

                    }


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;


                if (dtFinalDataTable.Rows.Count > 0)
                {
                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                    blUpdated = true;
                }


                dtFinalDataTable.Clear();

                intFileCnt++;

            }


            return blUpdated;
        }






        private static bool getSiteOfCareCS()
        {

            bool blUpdated = false;

            Console.WriteLine("Site Of Care CS Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["SiteOfCareCSFile_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["SiteOfCareCSFinal_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");


            int intStartingMonth = 2;
            int intStartingYear = 2022;


            string strFileName = null;
            string lastFolderName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strTableName = "stg.AmerichoiceSiteOfCare_Data";
            object objCurrentValue = null;
            //string[] files;
            List<string> files;
            DateTime temp;

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from "  + strTableName);
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            DataColumnCollection columns;


            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");
            //ONLY ONE TIME PER LOOP
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("Episode_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Episode_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("CPT_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Dx_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Dx_Description", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_DoB", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Patient_State", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_TIN", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_NPI", typeof(String));
            dtFinalDataTable.Columns.Add("Site_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Site_TIN", typeof(String));
            dtFinalDataTable.Columns.Add("Site_NPI", typeof(String));
            dtFinalDataTable.Columns.Add("Decision", typeof(String));
            dtFinalDataTable.Columns.Add("Approval_Reason", typeof(String));
            dtFinalDataTable.Columns.Add("Time_Between_Imaging_Procedure", typeof(String));
            dtFinalDataTable.Columns.Add("Delay_Adverse_Affect_Outcome", typeof(String));
            dtFinalDataTable.Columns.Add("How_Delay_Adverse_Affect_Outcome", typeof(String));
            dtFinalDataTable.Columns.Add("When_Free_Standing_Imaging_Center", typeof(DateTime));
            dtFinalDataTable.Columns.Add("When_Hospital", typeof(DateTime));
            dtFinalDataTable.Columns.Add("How_Delay_Care", typeof(String));
            dtFinalDataTable.Columns.Add("Plan_Type", typeof(String));
            dtFinalDataTable.Columns.Add("Group_Number", typeof(String));
            dtFinalDataTable.Columns.Add("Jurisdiction_State", typeof(String));
            dtFinalDataTable.Columns.Add("Site_State", typeof(String));
            dtFinalDataTable.Columns.Add("Auth_Status", typeof(String));
            dtFinalDataTable.Columns.Add("Case_Initiation_Method", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_Gender", typeof(String));
            dtFinalDataTable.Columns.Add("CPT_Description", typeof(String));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));
            dtFinalDataTable.TableName = strTableName;





            //TMP??? DOWNLOAD ALL FILES
            //RESET FINAL TABLE!!!
            if (1 == 2)
            {
                files = Directory.EnumerateFiles(strFileFolderPath, "*.xlsx", SearchOption.AllDirectories).Where(s => (s.ToString().ToLower().Replace("_", " ").Contains("site of care"))).ToList();


                //files = new List<string>();
                // files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");
                //files = Directory.GetFiles(strFileFolderPath, "CRC_Pivot_Rawdata_*", SearchOption.AllDirectories);


                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile));
                    strMonth = lastFolderName.Replace("-", "").Substring(4, 2);
                    strYear = lastFolderName.Replace("-", "").Substring(0, 4);
                    strFileName = Path.GetFileName(strFile);
                    strReportType = "SiteOfCareCS";
                    strFilePath = Path.GetDirectoryName(strFile);


                    //IGNORE OLD FOLDERS
                    if (Int32.Parse(strYear) < intStartingYear)
                    {
                        continue;
                    }


                    if (!Directory.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\"))
                    {
                        Directory.CreateDirectory(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\");

                        if (!File.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName))
                        {
                            File.Copy(strFile, strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName);
                        }


                    }



                }
            }



            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\SiteOfCare\2019\201906\UHC_Site_of_Care_Program_Reporting_November 2019.xls");

            //files = Directory.EnumerateFiles(strFinalFileFolderPath, "*.xls*", SearchOption.AllDirectories).ToList();
            files = Directory.EnumerateFiles(strFinalFileFolderPath, "*.xls*", SearchOption.AllDirectories).ToList();

            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");



            strSheetname = "Case Details";

            intFileCnt = 1;
            foreach (string strFile in files)
            {
                lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile)).Replace("-", "");
                strMonth = lastFolderName.Substring(4, 2);
                strYear = lastFolderName.Substring(0, 4);
                strFileName = Path.GetFileName(strFile);
                strReportType = "AmerichoiceSiteOfCare_Data";
                strFilePath = Path.GetDirectoryName(strFile);

                if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }



                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");


                try
                {
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

                }
                catch (Exception ex)
                {
                    SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
                }




                // 
                columns = dtCurrentDataTable.Columns;
                Console.Write("\rFile to DataTable");
                // strSummaryofLOB = strFolder.Split('_')[1];

                intRowCnt = 1;
                foreach (DataRow d in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");


                    if (columns.Contains("EpisodeId"))
                        objCurrentValue = (d["EpisodeId"] != DBNull.Value && !(d["EpisodeId"] + "").Trim().Equals("") ? d["EpisodeId"] : (object)DBNull.Value);
                    else if (columns.Contains("Episode ID"))
                        objCurrentValue = (d["Episode ID"] != DBNull.Value && !(d["Episode ID"] + "").Trim().Equals("") ? d["Episode ID"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (objCurrentValue == DBNull.Value)
                    {
                        intRowCnt++;
                        continue;
                    }
                    //if(objCurrentValue.ToString().Equals("A129808520"))
                    //{
                    //    string s = "Stop!";
                    //}


                    currentRow = dtFinalDataTable.NewRow();
                    currentRow["Episode_ID"] = objCurrentValue;



                    if (columns.Contains("EpisodeDate"))
                        objCurrentValue = (d["EpisodeDate"] != DBNull.Value && !(d["EpisodeDate"] + "").Trim().Equals("") ? d["EpisodeDate"] : (object)DBNull.Value);
                    else if (columns.Contains("Episode Day"))
                        objCurrentValue = (d["Episode Day"] != DBNull.Value && !(d["Episode Day"] + "").Trim().Equals("") ? d["Episode Day"] : (object)DBNull.Value);
                    else if (columns.Contains("Episode Date"))
                        objCurrentValue = (d["Episode Date"] != DBNull.Value && !(d["Episode Date"] + "").Trim().Equals("") ? d["Episode Date"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Episode_Date"] = objCurrentValue;


                    if (columns.Contains("CPT Code"))
                        objCurrentValue = (d["CPT Code"] != DBNull.Value && !(d["CPT Code"] + "").Trim().Equals("") ? d["CPT Code"] : (object)DBNull.Value);
                    else if (columns.Contains("CPTCode"))
                        objCurrentValue = (d["CPTCode"] != DBNull.Value && !(d["CPTCode"] + "").Trim().Equals("") ? d["CPTCode"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["CPT_Code"] = objCurrentValue;

                    if (columns.Contains("Dx Code"))
                        objCurrentValue = (d["Dx Code"] != DBNull.Value && !(d["Dx Code"] + "").Trim().Equals("") ? d["Dx Code"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Dx_Code"] = objCurrentValue;


                    if (columns.Contains("Dx Description"))
                        objCurrentValue = (d["Dx Description"] != DBNull.Value && !(d["Dx Description"] + "").Trim().Equals("") ? d["Dx Description"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Dx_Description"] = objCurrentValue;



                    if (columns.Contains("PatientId"))
                        objCurrentValue = (d["PatientId"] != DBNull.Value && !(d["PatientId"] + "").Trim().Equals("") ? d["PatientId"] : (object)DBNull.Value);
                    else if (columns.Contains("Patient ID"))
                        objCurrentValue = (d["Patient ID"] != DBNull.Value && !(d["Patient ID"] + "").Trim().Equals("") ? d["Patient ID"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_ID"] = objCurrentValue;


                    if (columns.Contains("Patient Name"))
                        objCurrentValue = (d["Patient Name"] != DBNull.Value && !(d["Patient Name"] + "").Trim().Equals("") ? d["Patient Name"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientName"))
                        objCurrentValue = (d["PatientName"] != DBNull.Value && !(d["PatientName"] + "").Trim().Equals("") ? d["PatientName"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_Name"] = objCurrentValue;



                    if (columns.Contains("Patient DoB"))
                        objCurrentValue = (d["Patient DoB"] != DBNull.Value && !(d["Patient DoB"] + "").Trim().Equals("") ? d["Patient DoB"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientDOB"))
                        objCurrentValue = (d["PatientDOB"] != DBNull.Value && !(d["PatientDOB"] + "").Trim().Equals("") ? d["PatientDOB"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (objCurrentValue != DBNull.Value)
                    {
                        if (DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        {
                            currentRow["Patient_DoB"] = objCurrentValue;
                        }
                        else
                        {
                            currentRow["Patient_DoB"] = DateTime.FromOADate(double.Parse(objCurrentValue.ToString()));
                        }
                    }
                    else
                        currentRow["Patient_DoB"] = objCurrentValue;

                    if (columns.Contains("Patient State"))
                        objCurrentValue = (d["Patient State"] != DBNull.Value && !(d["Patient State"] + "").Trim().Equals("") ? d["Patient State"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientState"))
                        objCurrentValue = (d["PatientState"] != DBNull.Value && !(d["PatientState"] + "").Trim().Equals("") ? d["PatientState"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_State"] = objCurrentValue;




                    if (columns.Contains("Ordering Physician Name"))
                        objCurrentValue = (d["Ordering Physician Name"] != DBNull.Value && !(d["Ordering Physician Name"] + "").Trim().Equals("") ? d["Ordering Physician Name"] : (object)DBNull.Value);
                    else if (columns.Contains("OrderingPhys"))
                        objCurrentValue = (d["OrderingPhys"] != DBNull.Value && !(d["OrderingPhys"] + "").Trim().Equals("") ? d["OrderingPhys"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_Name"] = objCurrentValue;


                    if (columns.Contains("Ordering Physician TIN"))
                        objCurrentValue = (d["Ordering Physician TIN"] != DBNull.Value && !(d["Ordering Physician TIN"] + "").Trim().Equals("") ? d["Ordering Physician TIN"] : (object)DBNull.Value);
                    else if (columns.Contains("PhysTIN"))
                        objCurrentValue = (d["PhysTIN"] != DBNull.Value && !(d["PhysTIN"] + "").Trim().Equals("") ? d["PhysTIN"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_TIN"] = objCurrentValue;


                    if (columns.Contains("Ordering Physician NPI"))
                        objCurrentValue = (d["Ordering Physician NPI"] != DBNull.Value && !(d["Ordering Physician NPI"] + "").Trim().Equals("") ? d["Ordering Physician NPI"] : (object)DBNull.Value);
                    else if (columns.Contains("PhysNPI"))
                        objCurrentValue = (d["PhysNPI"] != DBNull.Value && !(d["PhysNPI"] + "").Trim().Equals("") ? d["PhysNPI"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_NPI"] = objCurrentValue;


                    if (columns.Contains("Site Name"))
                        objCurrentValue = (d["Site Name"] != DBNull.Value && !(d["Site Name"] + "").Trim().Equals("") ? d["Site Name"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteName"))
                        objCurrentValue = (d["SiteName"] != DBNull.Value && !(d["SiteName"] + "").Trim().Equals("") ? d["SiteName"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_Name"] = objCurrentValue;


                    if (columns.Contains("Site TIN"))
                        objCurrentValue = (d["Site TIN"] != DBNull.Value && !(d["Site TIN"] + "").Trim().Equals("") ? d["Site TIN"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteTIN"))
                        objCurrentValue = (d["SiteTIN"] != DBNull.Value && !(d["SiteTIN"] + "").Trim().Equals("") ? d["SiteTIN"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_TIN"] = objCurrentValue;


                    if (columns.Contains("Site NPI"))
                        objCurrentValue = (d["Site NPI"] != DBNull.Value && !(d["Site NPI"] + "").Trim().Equals("") ? d["Site NPI"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteNPI"))
                        objCurrentValue = (d["SiteNPI"] != DBNull.Value && !(d["SiteNPI"] + "").Trim().Equals("") ? d["SiteNPI"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_NPI"] = objCurrentValue;




                    if (columns.Contains("Decision"))
                        objCurrentValue = (d["Decision"] != DBNull.Value && !(d["Decision"] + "").Trim().Equals("") ? d["Decision"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Decision"] = objCurrentValue;


                    if (columns.Contains("Approval Reason"))
                        objCurrentValue = (d["Approval Reason"] != DBNull.Value && !(d["Approval Reason"] + "").Trim().Equals("") ? d["Approval Reason"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Approval_Reason"] = objCurrentValue;


                    if (columns.Contains("What is the time between the imaging and the procedure?"))
                        objCurrentValue = (d["What is the time between the imaging and the procedure?"] != DBNull.Value && !(d["What is the time between the imaging and the procedure?"] + "").Trim().Equals("") && !(d["What is the time between the imaging and the procedure?"] + "").Trim().Equals("NULL") ? d["What is the time between the imaging and the procedure?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Time_Between_Imaging_Procedure"] = objCurrentValue;


                    if (columns.Contains("Would a delay in care adversely affect health outcome?"))
                        objCurrentValue = (d["Would a delay in care adversely affect health outcome?"] != DBNull.Value && !(d["Would a delay in care adversely affect health outcome?"] + "").Trim().Equals("") && !(d["Would a delay in care adversely affect health outcome?"] + "").Trim().Equals("NULL") ? d["Would a delay in care adversely affect health outcome?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Delay_Adverse_Affect_Outcome"] = objCurrentValue;


                    if (columns.Contains("How would a delay in care adversely affect health outcome?"))
                        objCurrentValue = (d["How would a delay in care adversely affect health outcome?"] != DBNull.Value && !(d["How would a delay in care adversely affect health outcome?"] + "").Trim().Equals("") && !(d["How would a delay in care adversely affect health outcome?"] + "").Trim().Equals("NULL") ? d["How would a delay in care adversely affect health outcome?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["How_Delay_Adverse_Affect_Outcome"] = objCurrentValue;

                    if (columns.Contains("When is the next appointment available at the free-standing imag"))
                        objCurrentValue = (d["When is the next appointment available at the free-standing imag"] != DBNull.Value && !(d["When is the next appointment available at the free-standing imag"] + "").Trim().Equals("") && !(d["When is the next appointment available at the free-standing imag"] + "").Trim().Equals("NULL") ? d["When is the next appointment available at the free-standing imag"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (!DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["When_Free_Standing_Imaging_Center"] = objCurrentValue;


                    if (columns.Contains("When is the next appointment available at the hospital?"))
                        objCurrentValue = (d["When is the next appointment available at the hospital?"] != DBNull.Value && !(d["When is the next appointment available at the hospital?"] + "").Trim().Equals("") && !(d["When is the next appointment available at the hospital?"] + "").Trim().Equals("NULL") ? d["When is the next appointment available at the hospital?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (!DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["When_Hospital"] = objCurrentValue;



                    if (columns.Contains("How does this delay care?"))
                        objCurrentValue = (d["How does this delay care?"] != DBNull.Value && !(d["How does this delay care?"] + "").Trim().Equals("") && !(d["How does this delay care?"] + "").Trim().Equals("NULL") ? d["How does this delay care?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["How_Delay_Care"] = objCurrentValue;


                    if (columns.Contains("Plan Type"))
                        objCurrentValue = (d["Plan Type"] != DBNull.Value && !(d["Plan Type"] + "").Trim().Equals("") ? d["Plan Type"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Plan_Type"] = objCurrentValue;


                    if (columns.Contains("Group Number"))
                        objCurrentValue = (d["Group Number"] != DBNull.Value && !(d["Group Number"] + "").Trim().Equals("") ? d["Group Number"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Group_Number"] = objCurrentValue;


                    if (columns.Contains("Jurisdiction State"))
                        objCurrentValue = (d["Jurisdiction State"] != DBNull.Value && !(d["Jurisdiction State"] + "").Trim().Equals("") ? d["Jurisdiction State"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Jurisdiction_State"] = objCurrentValue;



                    if (columns.Contains("Site State"))
                        objCurrentValue = (d["Site State"] != DBNull.Value && !(d["Site State"] + "").Trim().Equals("") ? d["Site State"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_State"] = objCurrentValue;



                    if (columns.Contains("Auth Status"))
                        objCurrentValue = (d["Auth Status"] != DBNull.Value && !(d["Auth Status"] + "").Trim().Equals("") ? d["Auth Status"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Auth_Status"] = objCurrentValue;


                    if (columns.Contains("Case Initiation Method"))
                        objCurrentValue = (d["Case Initiation Method"] != DBNull.Value && !(d["Case Initiation Method"] + "").Trim().Equals("") ? d["Case Initiation Method"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Case_Initiation_Method"] = objCurrentValue;

                    if (columns.Contains("Patient Gender"))
                        objCurrentValue = (d["Patient Gender"] != DBNull.Value && !(d["Patient Gender"] + "").Trim().Equals("") ? d["Patient Gender"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_Gender"] = objCurrentValue;

                    if (columns.Contains("CPT Description"))
                        objCurrentValue = (d["CPT Description"] != DBNull.Value && !(d["CPT Description"] + "").Trim().Equals("") ? d["CPT Description"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["CPT_Description"] = objCurrentValue;


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;


                if (dtFinalDataTable.Rows.Count > 0)
                {
                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                    blUpdated = true;
                }


                dtFinalDataTable.Clear();

                intFileCnt++;

            }


            return blUpdated;
        }


        private static bool getSiteOfCareAll()
        {

            bool blUpdated = false;

            Console.WriteLine("Site Of Care Parser");
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");


            string strFileName = null;
            string strSheetname = null;
            string strTableName = "stg.SiteOfCare_All";
            object objCurrentValue = null;
            //string[] files;
            List<string> files;
            DateTime temp;

            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            DataColumnCollection columns;


            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");
            //ONLY ONE TIME PER LOOP
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("Episode_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Episode_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("CPT_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Dx_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Dx_Description", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_DoB", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Patient_State", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_TIN", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_NPI", typeof(String));
            dtFinalDataTable.Columns.Add("Site_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Site_TIN", typeof(String));
            dtFinalDataTable.Columns.Add("Site_NPI", typeof(String));
            dtFinalDataTable.Columns.Add("Decision", typeof(String));
            dtFinalDataTable.Columns.Add("Approval_Reason", typeof(String));
            dtFinalDataTable.Columns.Add("Time_Between_Imaging_Procedure", typeof(String));
            dtFinalDataTable.Columns.Add("Delay_Adverse_Affect_Outcome", typeof(String));
            dtFinalDataTable.Columns.Add("How_Delay_Adverse_Affect_Outcome", typeof(String));
            dtFinalDataTable.Columns.Add("When_Free_Standing_Imaging_Center", typeof(DateTime));
            dtFinalDataTable.Columns.Add("When_Hospital", typeof(DateTime));
            dtFinalDataTable.Columns.Add("How_Delay_Care", typeof(String));
            dtFinalDataTable.Columns.Add("Plan_Type", typeof(String));
            dtFinalDataTable.Columns.Add("Group_Number", typeof(String));
            dtFinalDataTable.Columns.Add("Jurisdiction_State", typeof(String));
            dtFinalDataTable.Columns.Add("Site_State", typeof(String));
            dtFinalDataTable.Columns.Add("Auth_Status", typeof(String));
            dtFinalDataTable.Columns.Add("Case_Initiation_Method", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_Gender", typeof(String));
            dtFinalDataTable.Columns.Add("CPT_Description", typeof(String));
            dtFinalDataTable.TableName = strTableName;



            files = new List<string>();
            files.Add(@"C:\Users\cgiorda\Desktop\SiteOfCare\Done\UHC_Site_of_Care_Program_Reporting 20220430_CORRECTED.xlsx");

           
            strSheetname = "Case Details";

            intFileCnt = 1;
            foreach (string strFile in files)
            {
                
                strFileName = Path.GetFileName(strFile);



                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");


                try
                {
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

                }
                catch (Exception ex)
                {
                    SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
                }




                // 
                columns = dtCurrentDataTable.Columns;
                Console.Write("\rFile to DataTable");
                // strSummaryofLOB = strFolder.Split('_')[1];

                intRowCnt = 1;
                foreach (DataRow d in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");


                    if (columns.Contains("EpisodeId"))
                        objCurrentValue = (d["EpisodeId"] != DBNull.Value && !(d["EpisodeId"] + "").Trim().Equals("") ? d["EpisodeId"] : (object)DBNull.Value);
                    else if (columns.Contains("Episode ID"))
                        objCurrentValue = (d["Episode ID"] != DBNull.Value && !(d["Episode ID"] + "").Trim().Equals("") ? d["Episode ID"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (objCurrentValue == DBNull.Value)
                    {
                        intRowCnt++;
                        continue;
                    }
                    //if(objCurrentValue.ToString().Equals("A129808520"))
                    //{
                    //    string s = "Stop!";
                    //}


                    currentRow = dtFinalDataTable.NewRow();
                    currentRow["Episode_ID"] = objCurrentValue;



                    if (columns.Contains("EpisodeDate"))
                        objCurrentValue = (d["EpisodeDate"] != DBNull.Value && !(d["EpisodeDate"] + "").Trim().Equals("") ? d["EpisodeDate"] : (object)DBNull.Value);
                    else if (columns.Contains("Episode Day"))
                        objCurrentValue = (d["Episode Day"] != DBNull.Value && !(d["Episode Day"] + "").Trim().Equals("") ? d["Episode Day"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Episode_Date"] = objCurrentValue;


                    if (columns.Contains("CPT Code"))
                        objCurrentValue = (d["CPT Code"] != DBNull.Value && !(d["CPT Code"] + "").Trim().Equals("") ? d["CPT Code"] : (object)DBNull.Value);
                    else if (columns.Contains("CPTCode"))
                        objCurrentValue = (d["CPTCode"] != DBNull.Value && !(d["CPTCode"] + "").Trim().Equals("") ? d["CPTCode"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["CPT_Code"] = objCurrentValue;

                    if (columns.Contains("Dx Code"))
                        objCurrentValue = (d["Dx Code"] != DBNull.Value && !(d["Dx Code"] + "").Trim().Equals("") ? d["Dx Code"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Dx_Code"] = objCurrentValue;


                    if (columns.Contains("Dx Description"))
                        objCurrentValue = (d["Dx Description"] != DBNull.Value && !(d["Dx Description"] + "").Trim().Equals("") ? d["Dx Description"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Dx_Description"] = objCurrentValue;



                    if (columns.Contains("PatientId"))
                        objCurrentValue = (d["PatientId"] != DBNull.Value && !(d["PatientId"] + "").Trim().Equals("") ? d["PatientId"] : (object)DBNull.Value);
                    else if (columns.Contains("Patient ID"))
                        objCurrentValue = (d["Patient ID"] != DBNull.Value && !(d["Patient ID"] + "").Trim().Equals("") ? d["Patient ID"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_ID"] = objCurrentValue;


                    if (columns.Contains("Patient Name"))
                        objCurrentValue = (d["Patient Name"] != DBNull.Value && !(d["Patient Name"] + "").Trim().Equals("") ? d["Patient Name"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientName"))
                        objCurrentValue = (d["PatientName"] != DBNull.Value && !(d["PatientName"] + "").Trim().Equals("") ? d["PatientName"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_Name"] = objCurrentValue;



                    if (columns.Contains("Patient DoB"))
                        objCurrentValue = (d["Patient DoB"] != DBNull.Value && !(d["Patient DoB"] + "").Trim().Equals("") ? d["Patient DoB"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientDOB"))
                        objCurrentValue = (d["PatientDOB"] != DBNull.Value && !(d["PatientDOB"] + "").Trim().Equals("") ? d["PatientDOB"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if(objCurrentValue != DBNull.Value)
                    {
                        if (DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        {
                            currentRow["Patient_DoB"] = objCurrentValue;
                        }
                        else
                        {
                            currentRow["Patient_DoB"] = DateTime.FromOADate(double.Parse(objCurrentValue.ToString()));
                        }
                    }
                    


                  


                    if (columns.Contains("Patient State"))
                        objCurrentValue = (d["Patient State"] != DBNull.Value && !(d["Patient State"] + "").Trim().Equals("") ? d["Patient State"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientState"))
                        objCurrentValue = (d["PatientState"] != DBNull.Value && !(d["PatientState"] + "").Trim().Equals("") ? d["PatientState"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_State"] = objCurrentValue;




                    if (columns.Contains("Ordering Physician Name"))
                        objCurrentValue = (d["Ordering Physician Name"] != DBNull.Value && !(d["Ordering Physician Name"] + "").Trim().Equals("") ? d["Ordering Physician Name"] : (object)DBNull.Value);
                    else if (columns.Contains("OrderingPhys"))
                        objCurrentValue = (d["OrderingPhys"] != DBNull.Value && !(d["OrderingPhys"] + "").Trim().Equals("") ? d["OrderingPhys"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_Name"] = objCurrentValue;


                    if (columns.Contains("Ordering Physician TIN"))
                        objCurrentValue = (d["Ordering Physician TIN"] != DBNull.Value && !(d["Ordering Physician TIN"] + "").Trim().Equals("") ? d["Ordering Physician TIN"] : (object)DBNull.Value);
                    else if (columns.Contains("PhysTIN"))
                        objCurrentValue = (d["PhysTIN"] != DBNull.Value && !(d["PhysTIN"] + "").Trim().Equals("") ? d["PhysTIN"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_TIN"] = objCurrentValue;


                    if (columns.Contains("Ordering Physician NPI"))
                        objCurrentValue = (d["Ordering Physician NPI"] != DBNull.Value && !(d["Ordering Physician NPI"] + "").Trim().Equals("") ? d["Ordering Physician NPI"] : (object)DBNull.Value);
                    else if (columns.Contains("PhysNPI"))
                        objCurrentValue = (d["PhysNPI"] != DBNull.Value && !(d["PhysNPI"] + "").Trim().Equals("") ? d["PhysNPI"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_NPI"] = objCurrentValue;


                    if (columns.Contains("Site Name"))
                        objCurrentValue = (d["Site Name"] != DBNull.Value && !(d["Site Name"] + "").Trim().Equals("") ? d["Site Name"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteName"))
                        objCurrentValue = (d["SiteName"] != DBNull.Value && !(d["SiteName"] + "").Trim().Equals("") ? d["SiteName"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_Name"] = objCurrentValue;


                    if (columns.Contains("Site TIN"))
                        objCurrentValue = (d["Site TIN"] != DBNull.Value && !(d["Site TIN"] + "").Trim().Equals("") ? d["Site TIN"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteTIN"))
                        objCurrentValue = (d["SiteTIN"] != DBNull.Value && !(d["SiteTIN"] + "").Trim().Equals("") ? d["SiteTIN"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_TIN"] = objCurrentValue;


                    if (columns.Contains("Site NPI"))
                        objCurrentValue = (d["Site NPI"] != DBNull.Value && !(d["Site NPI"] + "").Trim().Equals("") ? d["Site NPI"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteNPI"))
                        objCurrentValue = (d["SiteNPI"] != DBNull.Value && !(d["SiteNPI"] + "").Trim().Equals("") ? d["SiteNPI"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_NPI"] = objCurrentValue;




                    if (columns.Contains("Decision"))
                        objCurrentValue = (d["Decision"] != DBNull.Value && !(d["Decision"] + "").Trim().Equals("") ? d["Decision"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Decision"] = objCurrentValue;


                    if (columns.Contains("Approval Reason"))
                        objCurrentValue = (d["Approval Reason"] != DBNull.Value && !(d["Approval Reason"] + "").Trim().Equals("") ? d["Approval Reason"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Approval_Reason"] = objCurrentValue;


                    if (columns.Contains("What is the time between the imaging and the procedure?"))
                        objCurrentValue = (d["What is the time between the imaging and the procedure?"] != DBNull.Value && !(d["What is the time between the imaging and the procedure?"] + "").Trim().Equals("") && !(d["What is the time between the imaging and the procedure?"] + "").Trim().Equals("NULL") ? d["What is the time between the imaging and the procedure?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Time_Between_Imaging_Procedure"] = objCurrentValue;


                    if (columns.Contains("Would a delay in care adversely affect health outcome?"))
                        objCurrentValue = (d["Would a delay in care adversely affect health outcome?"] != DBNull.Value && !(d["Would a delay in care adversely affect health outcome?"] + "").Trim().Equals("") && !(d["Would a delay in care adversely affect health outcome?"] + "").Trim().Equals("NULL") ? d["Would a delay in care adversely affect health outcome?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Delay_Adverse_Affect_Outcome"] = objCurrentValue;


                    if (columns.Contains("How would a delay in care adversely affect health outcome?"))
                        objCurrentValue = (d["How would a delay in care adversely affect health outcome?"] != DBNull.Value && !(d["How would a delay in care adversely affect health outcome?"] + "").Trim().Equals("") && !(d["How would a delay in care adversely affect health outcome?"] + "").Trim().Equals("NULL") ? d["How would a delay in care adversely affect health outcome?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["How_Delay_Adverse_Affect_Outcome"] = objCurrentValue;

                    if (columns.Contains("When is the next appointment available at the free-standing imag"))
                        objCurrentValue = (d["When is the next appointment available at the free-standing imag"] != DBNull.Value && !(d["When is the next appointment available at the free-standing imag"] + "").Trim().Equals("") && !(d["When is the next appointment available at the free-standing imag"] + "").Trim().Equals("NULL") ? d["When is the next appointment available at the free-standing imag"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (!DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["When_Free_Standing_Imaging_Center"] = objCurrentValue;


                    if (columns.Contains("When is the next appointment available at the hospital?"))
                        objCurrentValue = (d["When is the next appointment available at the hospital?"] != DBNull.Value && !(d["When is the next appointment available at the hospital?"] + "").Trim().Equals("") && !(d["When is the next appointment available at the hospital?"] + "").Trim().Equals("NULL") ? d["When is the next appointment available at the hospital?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (!DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["When_Hospital"] = objCurrentValue;



                    if (columns.Contains("How does this delay care?"))
                        objCurrentValue = (d["How does this delay care?"] != DBNull.Value && !(d["How does this delay care?"] + "").Trim().Equals("") && !(d["How does this delay care?"] + "").Trim().Equals("NULL") ? d["How does this delay care?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["How_Delay_Care"] = objCurrentValue;


                    if (columns.Contains("Plan Type"))
                        objCurrentValue = (d["Plan Type"] != DBNull.Value && !(d["Plan Type"] + "").Trim().Equals("") ? d["Plan Type"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Plan_Type"] = objCurrentValue;


                    if (columns.Contains("Group Number"))
                        objCurrentValue = (d["Group Number"] != DBNull.Value && !(d["Group Number"] + "").Trim().Equals("") ? d["Group Number"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Group_Number"] = objCurrentValue;


                    if (columns.Contains("Jurisdiction State"))
                        objCurrentValue = (d["Jurisdiction State"] != DBNull.Value && !(d["Jurisdiction State"] + "").Trim().Equals("") ? d["Jurisdiction State"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Jurisdiction_State"] = objCurrentValue;



                    if (columns.Contains("Site State"))
                        objCurrentValue = (d["Site State"] != DBNull.Value && !(d["Site State"] + "").Trim().Equals("") ? d["Site State"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_State"] = objCurrentValue;



                    if (columns.Contains("Auth Status"))
                        objCurrentValue = (d["Auth Status"] != DBNull.Value && !(d["Auth Status"] + "").Trim().Equals("") ? d["Auth Status"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Auth_Status"] = objCurrentValue;


                    if (columns.Contains("Case Initiation Method"))
                        objCurrentValue = (d["Case Initiation Method"] != DBNull.Value && !(d["Case Initiation Method"] + "").Trim().Equals("") ? d["Case Initiation Method"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Case_Initiation_Method"] = objCurrentValue;

                    if (columns.Contains("Patient Gender"))
                        objCurrentValue = (d["Patient Gender"] != DBNull.Value && !(d["Patient Gender"] + "").Trim().Equals("") ? d["Patient Gender"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_Gender"] = objCurrentValue;

                    if (columns.Contains("CPT Description"))
                        objCurrentValue = (d["CPT Description"] != DBNull.Value && !(d["CPT Description"] + "").Trim().Equals("") ? d["CPT Description"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["CPT_Description"] = objCurrentValue;


                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;


                if (dtFinalDataTable.Rows.Count > 0)
                {
                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                    blUpdated = true;
                }


                dtFinalDataTable.Clear();

                intFileCnt++;

            }


            return blUpdated;
        }






        private static bool getSiteOfCare()
        {

            bool blUpdated = false;

            Console.WriteLine("Site Of Care Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["SiteOfCareFile_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["SiteOfCareFinal_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");


            int intStartingMonth = 2;
            int intStartingYear = 2022;


            string strFileName = null;
            string lastFolderName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strTableName = "stg.SiteOfCare_Data";
            object objCurrentValue = null;
            //string[] files;
            List<string> files;
            DateTime temp;

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[SiteOfCare_Data]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            DataColumnCollection columns;


            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");
            //ONLY ONE TIME PER LOOP
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("Episode_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Episode_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("CPT_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Dx_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Dx_Description", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_DoB", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Patient_State", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_TIN", typeof(String));
            dtFinalDataTable.Columns.Add("Ordering_Physician_NPI", typeof(String));
            dtFinalDataTable.Columns.Add("Site_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Site_TIN", typeof(String));
            dtFinalDataTable.Columns.Add("Site_NPI", typeof(String));
            dtFinalDataTable.Columns.Add("Decision", typeof(String));
            dtFinalDataTable.Columns.Add("Approval_Reason", typeof(String));
            dtFinalDataTable.Columns.Add("Time_Between_Imaging_Procedure", typeof(String));
            dtFinalDataTable.Columns.Add("Delay_Adverse_Affect_Outcome", typeof(String));
            dtFinalDataTable.Columns.Add("How_Delay_Adverse_Affect_Outcome", typeof(String));
            dtFinalDataTable.Columns.Add("When_Free_Standing_Imaging_Center", typeof(DateTime));
            dtFinalDataTable.Columns.Add("When_Hospital", typeof(DateTime));
            dtFinalDataTable.Columns.Add("How_Delay_Care", typeof(String));
            dtFinalDataTable.Columns.Add("Plan_Type", typeof(String));
            dtFinalDataTable.Columns.Add("Group_Number", typeof(String));
            dtFinalDataTable.Columns.Add("Jurisdiction_State", typeof(String));
            dtFinalDataTable.Columns.Add("Site_State", typeof(String));
            dtFinalDataTable.Columns.Add("Auth_Status", typeof(String));
            dtFinalDataTable.Columns.Add("Case_Initiation_Method", typeof(String));
            dtFinalDataTable.Columns.Add("Patient_Gender", typeof(String));
            dtFinalDataTable.Columns.Add("CPT_Description", typeof(String));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));
            dtFinalDataTable.TableName = strTableName;





            //TMP??? DOWNLOAD ALL FILES
            //RESET FINAL TABLE!!!
            if (1==2)
            {
                files = Directory.EnumerateFiles(strFileFolderPath, "*.xlsx", SearchOption.AllDirectories).Where(s => (s.ToString().ToLower().Replace("_", " ").Contains("site of care"))).ToList();


                //files = new List<string>();
               // files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");
                //files = Directory.GetFiles(strFileFolderPath, "CRC_Pivot_Rawdata_*", SearchOption.AllDirectories);


                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile));
                    strMonth = lastFolderName.Replace("-","").Substring(4, 2);
                    strYear = lastFolderName.Replace("-", "").Substring(0, 4);
                    strFileName = Path.GetFileName(strFile);
                    strReportType = "SiteOfCare";
                    strFilePath = Path.GetDirectoryName(strFile);


                    //IGNORE OLD FOLDERS
                    if(Int32.Parse(strYear) < intStartingYear)
                    {
                            continue;
                    }


                   if (!Directory.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\"))
                    {
                        Directory.CreateDirectory(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\");

                        if (!File.Exists(strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName))
                        {
                            File.Copy(strFile, strFinalFileFolderPath + "\\" + strYear + "\\" + lastFolderName + "\\" + strFileName);
                        }


                    }

                   

                }
            }



            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\SiteOfCare\2019\201906\UHC_Site_of_Care_Program_Reporting_November 2019.xls");

            //files = Directory.EnumerateFiles(strFinalFileFolderPath, "*.xls*", SearchOption.AllDirectories).ToList();
            files = Directory.EnumerateFiles(strFinalFileFolderPath, "*.xls*", SearchOption.AllDirectories).ToList();

            //files = new List<string>();
            //files.Add(@"C:\Users\cgiorda\Desktop\garbage\2021\202109\Site Of Care Report_2021_09.xlsx");



            strSheetname = "Case Details";

            intFileCnt = 1;
            foreach (string strFile in files)
            {
                lastFolderName = Path.GetFileName(Path.GetDirectoryName(strFile)).Replace("-", "");
                strMonth = lastFolderName.Substring(4, 2);
                strYear = lastFolderName.Substring(0, 4);
                strFileName = Path.GetFileName(strFile);
                strReportType = "SiteOfCare";
                strFilePath = Path.GetDirectoryName(strFile);

                if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }



                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");


                try
                {
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

                }
                catch (Exception ex)
                {
                    SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
                }

                


               // 
                columns = dtCurrentDataTable.Columns;
                Console.Write("\rFile to DataTable");
                // strSummaryofLOB = strFolder.Split('_')[1];

                intRowCnt = 1;
                foreach (DataRow d in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");


                    if (columns.Contains("EpisodeId"))
                        objCurrentValue  = (d["EpisodeId"] != DBNull.Value && !(d["EpisodeId"] + "").Trim().Equals("") ? d["EpisodeId"] : (object) DBNull.Value);
                    else if (columns.Contains("Episode ID"))
                        objCurrentValue = (d["Episode ID"] != DBNull.Value && !(d["Episode ID"] + "").Trim().Equals("") ? d["Episode ID"] : (object) DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if(objCurrentValue == DBNull.Value)
                    {
                        intRowCnt++;
                        continue;
                    }
                    //if(objCurrentValue.ToString().Equals("A129808520"))
                    //{
                    //    string s = "Stop!";
                    //}


                    currentRow = dtFinalDataTable.NewRow();
                    currentRow["Episode_ID"] = objCurrentValue;
                    


                    if (columns.Contains("EpisodeDate"))
                        objCurrentValue = (d["EpisodeDate"] != DBNull.Value && !(d["EpisodeDate"] + "").Trim().Equals("") ? d["EpisodeDate"] : (object)DBNull.Value);
                    else if (columns.Contains("Episode Day"))
                        objCurrentValue = (d["Episode Day"] != DBNull.Value && !(d["Episode Day"] + "").Trim().Equals("") ? d["Episode Day"] : (object)DBNull.Value);
                   else if (columns.Contains("Episode Date"))
                        objCurrentValue = (d["Episode Date"] != DBNull.Value && !(d["Episode Date"] + "").Trim().Equals("") ? d["Episode Date"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Episode_Date"] = objCurrentValue;


                    if (columns.Contains("CPT Code"))
                        objCurrentValue = (d["CPT Code"] != DBNull.Value && !(d["CPT Code"] + "").Trim().Equals("") ? d["CPT Code"] : (object)DBNull.Value);
                    else if (columns.Contains("CPTCode"))
                        objCurrentValue = (d["CPTCode"] != DBNull.Value && !(d["CPTCode"] + "").Trim().Equals("") ? d["CPTCode"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["CPT_Code"] = objCurrentValue;

                    if (columns.Contains("Dx Code"))
                        objCurrentValue  = (d["Dx Code"] != DBNull.Value && !(d["Dx Code"] + "").Trim().Equals("") ? d["Dx Code"] : (object) DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Dx_Code"] = objCurrentValue;


                    if (columns.Contains("Dx Description"))
                        objCurrentValue = (d["Dx Description"] != DBNull.Value && !(d["Dx Description"] + "").Trim().Equals("") ? d["Dx Description"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Dx_Description"] = objCurrentValue;



                    if (columns.Contains("PatientId"))
                        objCurrentValue = (d["PatientId"] != DBNull.Value && !(d["PatientId"] + "").Trim().Equals("") ? d["PatientId"] : (object)DBNull.Value);
                    else if (columns.Contains("Patient ID"))
                        objCurrentValue = (d["Patient ID"] != DBNull.Value && !(d["Patient ID"] + "").Trim().Equals("") ? d["Patient ID"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_ID"] = objCurrentValue;


                    if (columns.Contains("Patient Name"))
                        objCurrentValue = (d["Patient Name"] != DBNull.Value && !(d["Patient Name"] + "").Trim().Equals("") ? d["Patient Name"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientName"))
                        objCurrentValue = (d["PatientName"] != DBNull.Value && !(d["PatientName"] + "").Trim().Equals("") ? d["PatientName"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_Name"] = objCurrentValue;



                    if (columns.Contains("Patient DoB"))
                        objCurrentValue = (d["Patient DoB"] != DBNull.Value && !(d["Patient DoB"] + "").Trim().Equals("") ? d["Patient DoB"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientDOB"))
                        objCurrentValue = (d["PatientDOB"] != DBNull.Value && !(d["PatientDOB"] + "").Trim().Equals("") ? d["PatientDOB"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (objCurrentValue != DBNull.Value)
                    {
                        if (DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        {
                            currentRow["Patient_DoB"] = objCurrentValue;
                        }
                        else
                        {
                            currentRow["Patient_DoB"] = DateTime.FromOADate(double.Parse(objCurrentValue.ToString()));
                        }
                    }
                    else
                        currentRow["Patient_DoB"] = objCurrentValue;

                    if (columns.Contains("Patient State"))
                        objCurrentValue = (d["Patient State"] != DBNull.Value && !(d["Patient State"] + "").Trim().Equals("") ? d["Patient State"] : (object)DBNull.Value);
                    else if (columns.Contains("PatientState"))
                        objCurrentValue = (d["PatientState"] != DBNull.Value && !(d["PatientState"] + "").Trim().Equals("") ? d["PatientState"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_State"] = objCurrentValue;




                    if (columns.Contains("Ordering Physician Name"))
                        objCurrentValue = (d["Ordering Physician Name"] != DBNull.Value && !(d["Ordering Physician Name"] + "").Trim().Equals("") ? d["Ordering Physician Name"] : (object)DBNull.Value);
                    else if (columns.Contains("OrderingPhys"))
                        objCurrentValue = (d["OrderingPhys"] != DBNull.Value && !(d["OrderingPhys"] + "").Trim().Equals("") ? d["OrderingPhys"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_Name"] = objCurrentValue;


                    if (columns.Contains("Ordering Physician TIN"))
                        objCurrentValue = (d["Ordering Physician TIN"] != DBNull.Value && !(d["Ordering Physician TIN"] + "").Trim().Equals("") ? d["Ordering Physician TIN"] : (object)DBNull.Value);
                    else if (columns.Contains("PhysTIN"))
                        objCurrentValue = (d["PhysTIN"] != DBNull.Value && !(d["PhysTIN"] + "").Trim().Equals("") ? d["PhysTIN"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_TIN"] = objCurrentValue;


                    if (columns.Contains("Ordering Physician NPI"))
                        objCurrentValue = (d["Ordering Physician NPI"] != DBNull.Value && !(d["Ordering Physician NPI"] + "").Trim().Equals("") ? d["Ordering Physician NPI"] : (object)DBNull.Value);
                    else if (columns.Contains("PhysNPI"))
                        objCurrentValue = (d["PhysNPI"] != DBNull.Value && !(d["PhysNPI"] + "").Trim().Equals("") ? d["PhysNPI"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Ordering_Physician_NPI"] = objCurrentValue;


                    if (columns.Contains("Site Name"))
                        objCurrentValue = (d["Site Name"] != DBNull.Value && !(d["Site Name"] + "").Trim().Equals("") ? d["Site Name"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteName"))
                        objCurrentValue = (d["SiteName"] != DBNull.Value && !(d["SiteName"] + "").Trim().Equals("") ? d["SiteName"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_Name"] = objCurrentValue;


                    if (columns.Contains("Site TIN"))
                        objCurrentValue = (d["Site TIN"] != DBNull.Value && !(d["Site TIN"] + "").Trim().Equals("") ? d["Site TIN"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteTIN"))
                        objCurrentValue = (d["SiteTIN"] != DBNull.Value && !(d["SiteTIN"] + "").Trim().Equals("") ? d["SiteTIN"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_TIN"] = objCurrentValue;


                    if (columns.Contains("Site NPI"))
                        objCurrentValue = (d["Site NPI"] != DBNull.Value && !(d["Site NPI"] + "").Trim().Equals("") ? d["Site NPI"] : (object)DBNull.Value);
                    else if (columns.Contains("SiteNPI"))
                        objCurrentValue = (d["SiteNPI"] != DBNull.Value && !(d["SiteNPI"] + "").Trim().Equals("") ? d["SiteNPI"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_NPI"] = objCurrentValue;




                    if (columns.Contains("Decision"))
                        objCurrentValue = (d["Decision"] != DBNull.Value && !(d["Decision"] + "").Trim().Equals("") ? d["Decision"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Decision"] = objCurrentValue;


                    if (columns.Contains("Approval Reason"))
                        objCurrentValue = (d["Approval Reason"] != DBNull.Value && !(d["Approval Reason"] + "").Trim().Equals("") ? d["Approval Reason"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Approval_Reason"] = objCurrentValue;


                    if (columns.Contains("What is the time between the imaging and the procedure?"))
                        objCurrentValue = (d["What is the time between the imaging and the procedure?"] != DBNull.Value && !(d["What is the time between the imaging and the procedure?"] + "").Trim().Equals("") && !(d["What is the time between the imaging and the procedure?"] + "").Trim().Equals("NULL") ? d["What is the time between the imaging and the procedure?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Time_Between_Imaging_Procedure"] = objCurrentValue;


                    if (columns.Contains("Would a delay in care adversely affect health outcome?"))
                        objCurrentValue = (d["Would a delay in care adversely affect health outcome?"] != DBNull.Value && !(d["Would a delay in care adversely affect health outcome?"] + "").Trim().Equals("") && !(d["Would a delay in care adversely affect health outcome?"] + "").Trim().Equals("NULL") ? d["Would a delay in care adversely affect health outcome?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Delay_Adverse_Affect_Outcome"] = objCurrentValue;


                    if (columns.Contains("How would a delay in care adversely affect health outcome?"))
                        objCurrentValue = (d["How would a delay in care adversely affect health outcome?"] != DBNull.Value && !(d["How would a delay in care adversely affect health outcome?"] + "").Trim().Equals("") && !(d["How would a delay in care adversely affect health outcome?"] + "").Trim().Equals("NULL") ? d["How would a delay in care adversely affect health outcome?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["How_Delay_Adverse_Affect_Outcome"] = objCurrentValue;

                    if (columns.Contains("When is the next appointment available at the free-standing imag"))
                        objCurrentValue = (d["When is the next appointment available at the free-standing imag"] != DBNull.Value && !(d["When is the next appointment available at the free-standing imag"] + "").Trim().Equals("") && !(d["When is the next appointment available at the free-standing imag"] + "").Trim().Equals("NULL") ? d["When is the next appointment available at the free-standing imag"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;
    
                    if (!DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["When_Free_Standing_Imaging_Center"] = objCurrentValue;


                    if (columns.Contains("When is the next appointment available at the hospital?"))
                        objCurrentValue = (d["When is the next appointment available at the hospital?"] != DBNull.Value && !(d["When is the next appointment available at the hospital?"] + "").Trim().Equals("") && !(d["When is the next appointment available at the hospital?"] + "").Trim().Equals("NULL") ? d["When is the next appointment available at the hospital?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    if (!DateTime.TryParse(objCurrentValue.ToString(), out temp))
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["When_Hospital"] = objCurrentValue;



                    if (columns.Contains("How does this delay care?"))
                        objCurrentValue = (d["How does this delay care?"] != DBNull.Value && !(d["How does this delay care?"] + "").Trim().Equals("") && !(d["How does this delay care?"] + "").Trim().Equals("NULL") ? d["How does this delay care?"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["How_Delay_Care"] = objCurrentValue;


                    if (columns.Contains("Plan Type"))
                        objCurrentValue = (d["Plan Type"] != DBNull.Value && !(d["Plan Type"] + "").Trim().Equals("") ? d["Plan Type"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Plan_Type"] = objCurrentValue;


                    if (columns.Contains("Group Number"))
                        objCurrentValue = (d["Group Number"] != DBNull.Value && !(d["Group Number"] + "").Trim().Equals("") ? d["Group Number"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Group_Number"] = objCurrentValue;


                    if (columns.Contains("Jurisdiction State"))
                        objCurrentValue = (d["Jurisdiction State"] != DBNull.Value && !(d["Jurisdiction State"] + "").Trim().Equals("") ? d["Jurisdiction State"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Jurisdiction_State"] = objCurrentValue;



                    if (columns.Contains("Site State"))
                        objCurrentValue = (d["Site State"] != DBNull.Value && !(d["Site State"] + "").Trim().Equals("") ? d["Site State"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Site_State"] = objCurrentValue;



                    if (columns.Contains("Auth Status"))
                        objCurrentValue = (d["Auth Status"] != DBNull.Value && !(d["Auth Status"] + "").Trim().Equals("") ? d["Auth Status"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Auth_Status"] = objCurrentValue;


                    if (columns.Contains("Case Initiation Method"))
                        objCurrentValue = (d["Case Initiation Method"] != DBNull.Value && !(d["Case Initiation Method"] + "").Trim().Equals("") ? d["Case Initiation Method"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Case_Initiation_Method"] = objCurrentValue;

                    if (columns.Contains("Patient Gender"))
                        objCurrentValue = (d["Patient Gender"] != DBNull.Value && !(d["Patient Gender"] + "").Trim().Equals("") ? d["Patient Gender"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["Patient_Gender"] = objCurrentValue;

                    if (columns.Contains("CPT Description"))
                        objCurrentValue = (d["CPT Description"] != DBNull.Value && !(d["CPT Description"] + "").Trim().Equals("") ? d["CPT Description"] : (object)DBNull.Value);
                    else
                        objCurrentValue = (object)DBNull.Value;

                    currentRow["CPT_Description"] = objCurrentValue;


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;


                if (dtFinalDataTable.Rows.Count > 0)
                {
                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                    DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                    blUpdated = true;
                }


                dtFinalDataTable.Clear();

                intFileCnt++;

            }


            return blUpdated;
        }



        private static bool getAVTAR_HONG()
        {

            bool blUpdated = false;

            Console.WriteLine("AVTAR Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["AvtarFile_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["AvtarFinal_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];


            strFileFolderPath = strFinalFileFolderPath;

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            string[] strFoldersArr = new string[] { "Monthly_EnI" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");

            string strFileName = null;
            string[] strFileNameArr = null;
            string strFileDate = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strSummaryofLOB = null;
            string[] files;



            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[AVTAR_Data]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;


            string strTableName = "stg.AVTAR_Data";
            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");


            //RESET FINAL TABLE!!!
            foreach (string strFolder in strFoldersArr)
            {

                if (strFolder == "Monthly_CnS")
                    strSheetname = "CaseDetailExtract";
                else if (strFolder == "Monthly_EnI" || strFolder == "Monthly_MnR")
                    strSheetname = "CaseLevelDetail";

                Console.Write("\rProcessing " + strFolder);
                files = Directory.GetFiles(strFileFolderPath + "\\" + strFolder, "*AVTAR_Detail_AlleviCore*.xlsb", SearchOption.AllDirectories);
                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    strFileName = Path.GetFileName(strFile);

                    if (strFileName.StartsWith("~") || strFileName.Contains("2019") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                    {
                        intFileCnt++;
                        continue;
                    }



                    strFileNameArr = strFileName.Split('_');
                    strFileDate = strFileNameArr[strFileNameArr.Length - 1].Replace(".xlsb", "");
                    strMonth = strFileDate.Substring(4, 2);
                    strYear = strFileDate.Substring(0, 4);
                    strReportType = "AVTAR";
                    strFilePath = Path.GetDirectoryName(strFile);

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
                    Console.Write("\rFile to DataTable");
                    strSummaryofLOB = strFolder.Split('_')[1];


                    //ONLY ONE TIME PER LOOP
                    dtFinalDataTable = dtCurrentDataTable.Clone();
                    dtFinalDataTable.Columns.Add("Summary_of_Lob", typeof(String));
                    dtFinalDataTable.Columns.Add("file_month", typeof(String));
                    dtFinalDataTable.Columns.Add("file_year", typeof(String));
                    dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
                    dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
                    dtFinalDataTable.Columns.Add("file_name", typeof(String));
                    dtFinalDataTable.Columns.Add("file_path", typeof(String));
                    dtFinalDataTable.Columns.Add("report_type", typeof(String));

                    dtFinalDataTable.TableName = strTableName;
                    intRowCnt = 1;
                    foreach (DataRow d in dtCurrentDataTable.Rows)
                    {

                        Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                        currentRow = dtFinalDataTable.NewRow();
                        foreach (DataColumn c in dtCurrentDataTable.Columns)
                        {
                            //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                            currentRow[c.ColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                        }

                        currentRow["Summary_of_Lob"] = strSummaryofLOB;
                        currentRow["file_month"] = strMonth;
                        currentRow["file_year"] = strYear;
                        currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                        currentRow["sheet_name"] = strSheetname;
                        currentRow["file_name"] = strFileName;
                        currentRow["file_path"] = strFilePath;
                        currentRow["report_type"] = strReportType;
                        dtFinalDataTable.Rows.Add(currentRow);
                        intRowCnt++;
                    }
                    currentRow = null;
                    dtCurrentDataTable = null;


                    if (dtFinalDataTable.Rows.Count > 0)
                    {
                        strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                        DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                        //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                        DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                        blUpdated = true;
                    }


                    dtFinalDataTable = null;
                    GC.Collect(2, GCCollectionMode.Forced);



                    intFileCnt++;

                }
            }

            return blUpdated;
        }






        private static bool getAVTAR()
        {

            bool blUpdated = false;

            Console.WriteLine("AVTAR Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["AvtarFile_Path"];
            string strFinalFileFolderPath = ConfigurationManager.AppSettings["AvtarFinal_File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;

            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            string[] strFoldersArr = new string[] { "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");

            string strFileName = null;
            string[] strFileNameArr = null;
            string strFileDate = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strSummaryofLOB = null;
            string[] files;



            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[AVTAR_Data]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;


            string strTableName = "stg.AVTAR_Data";
            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + strTableName + ";");


            //RESET FINAL TABLE!!!
            foreach (string strFolder in strFoldersArr)
            {

                if (strFolder == "Monthly_CnS")
                    strSheetname = "CaseDetailExtract";
                else if (strFolder == "Monthly_EnI" || strFolder == "Monthly_MnR")
                    strSheetname = "CaseLevelDetail";

                Console.Write("\rProcessing " + strFolder);
                files = Directory.GetFiles(strFileFolderPath + "\\" + strFolder, "*AVTAR_Detail_AlleviCore*.xlsb", SearchOption.AllDirectories);
                intFileCnt = 1;
                foreach (string strFile in files)
                {
                    strFileName = Path.GetFileName(strFile);

                    if (strFileName.StartsWith("~") || strFileName.Contains("2019") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                    {
                        intFileCnt++;
                        continue;
                    }



                    strFileNameArr = strFileName.Split('_');
                    strFileDate = strFileNameArr[strFileNameArr.Length - 1].Replace(".xlsb", "");
                    strMonth = strFileDate.Substring(4, 2);
                    strYear = strFileDate.Substring(0, 4);
                    strReportType = "AVTAR";
                    strFilePath = Path.GetDirectoryName(strFile);

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
                    Console.Write("\rFile to DataTable");
                    strSummaryofLOB = strFolder.Split('_')[1];


                    //ONLY ONE TIME PER LOOP
                    dtFinalDataTable = dtCurrentDataTable.Clone();
                    dtFinalDataTable.Columns.Add("Summary_of_Lob", typeof(String));
                    dtFinalDataTable.Columns.Add("file_month", typeof(String));
                    dtFinalDataTable.Columns.Add("file_year", typeof(String));
                    dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
                    dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
                    dtFinalDataTable.Columns.Add("file_name", typeof(String));
                    dtFinalDataTable.Columns.Add("file_path", typeof(String));
                    dtFinalDataTable.Columns.Add("report_type", typeof(String));

                    dtFinalDataTable.TableName = strTableName;
                    intRowCnt = 1;
                    foreach (DataRow d in dtCurrentDataTable.Rows)
                    {

                        Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                        currentRow = dtFinalDataTable.NewRow();
                        foreach (DataColumn c in dtCurrentDataTable.Columns)
                        {
                            //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                            currentRow[c.ColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                        }

                        currentRow["Summary_of_Lob"] = strSummaryofLOB;
                        currentRow["file_month"] = strMonth;
                        currentRow["file_year"] = strYear;
                        currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                        currentRow["sheet_name"] = strSheetname;
                        currentRow["file_name"] = strFileName;
                        currentRow["file_path"] = strFilePath;
                        currentRow["report_type"] = strReportType;
                        dtFinalDataTable.Rows.Add(currentRow);
                        intRowCnt++;
                    }
                    currentRow = null;
                    dtCurrentDataTable = null;


                    if (dtFinalDataTable.Rows.Count > 0)
                    {
                        strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                        DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                        //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                        DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                        blUpdated = true;
                    }


                    dtFinalDataTable = null;
                    GC.Collect(2, GCCollectionMode.Forced);



                    intFileCnt++;

                }
            }

            return blUpdated;
        }

        private static bool getAmerichoiceAllstatesAuthsData()
        {

            //\\msp09fil01\Radiology\Community & State\Reports\From eviCore\Monthly


            bool blUpdated = false;

            Console.WriteLine("EviCore Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["Americhoice_File_Path"];
            string strFileFolderProcess = ConfigurationManager.AppSettings["Americhoice_Process"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];


            //MANUAL INTERVENTION - THANKS AGAIN EVICORE :(
            //strFileFolderPath = @"\\msp09fil01\Radiology\Community & State\Reports\From eviCore\Monthly\Rad\2022\09\Updated";



            int intFileCnt = 1;

            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");
            SpreadsheetDocument wbCurrentExcelFile;

            string strTmpFile = null;

            string[] strFileNameArr = null;
            string strFileName = null;
            string strFilePath = null;
            string strFinalPath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = "ALL LOBs";
            string strState = null;

            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;

            //RESET FINAL TABLE!!!
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("State", typeof(String));
            dtFinalDataTable.Columns.Add("Modality", typeof(String));
            dtFinalDataTable.Columns.Add("Month", typeof(String));
            dtFinalDataTable.Columns.Add("Member_Lives", typeof(int));
            dtFinalDataTable.Columns.Add("Total_Requests", typeof(int));
            dtFinalDataTable.Columns.Add("Approved", typeof(int));
            dtFinalDataTable.Columns.Add("Denied", typeof(int));
            dtFinalDataTable.Columns.Add("Withdrawn", typeof(int));
            dtFinalDataTable.Columns.Add("Expired", typeof(int));
            dtFinalDataTable.Columns.Add("Non_Cert", typeof(int));
            dtFinalDataTable.Columns.Add("Pending", typeof(int));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));


            //CHECK AND COPY FOR NEWS ZIPS TO PROCESS
            string[] files;
            if (1 == 2)
            {
                files = Directory.GetFiles(strFileFolderPath, "UHC_Community_Plan*_MISC_*.zip", SearchOption.AllDirectories);
                foreach (string strFile in files)
                {
                    if (!File.Exists(strFileFolderProcess + "\\" + Path.GetFileName(strFile).Trim()) && !File.Exists(strFileFolderProcess + "\\Complete\\" + Path.GetFileName(strFile).Trim()) && !File.Exists(strFileFolderProcess + "\\Ignore\\" + Path.GetFileName(strFile).Trim()))
                    {
                        File.Copy(strFile, strFileFolderProcess + "\\" + Path.GetFileName(strFile).Trim());
                    }
                }

            }


            //CLEAN OUT WORKING DIR
            //foreach (FileInfo file in new DirectoryInfo(strFileFolderProcess + "\\Working\\").EnumerateFiles()) file.Delete();

            //PROCESS LOCAL ZIPS
            intFileCnt = 1;
            //files = Directory.GetFiles(strFileFolderProcess, "*.zip", SearchOption.TopDirectoryOnly);
            files = new string[] { strFileFolderProcess + "\\UHC_Community_Plan_CARD_MISC_Monthly_Reports_2022_12.zip", strFileFolderProcess + "\\UHC_Community_Plan_RAD_MISC_Monthly_Reports_2022_12.zip" };
            foreach (string strFile in files)
            {

                if (File.Exists(strFileFolderProcess + "\\Complete\\" + Path.GetFileName(strFile).Trim()))
                {
                    File.Delete(strFile);
                    intFileCnt++;
                    continue;
                }


                //UNZIP TO WORKING
                Console.Write("\rUnzipping and cleaning " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " compressed files");
                using (ZipArchive archive = ZipFile.OpenRead(strFile))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (!entry.FullName.ToLower().EndsWith("xlsx") || !entry.FullName.ToLower().StartsWith("americhoice_allstates_auths per 1000 by modality with exclusions"))
                            continue;



                        strFileNameArr = entry.FullName.ToLower().Trim().Replace(".xlsx", "").Split('_');
                        strMonth = strFileNameArr[strFileNameArr.Length - 1].Trim();
                        strYear = strFileNameArr[strFileNameArr.Length - 2].Trim();
                        strFileName = Path.GetFileName(strFile).Trim();
                        strReportType = (entry.FullName.ToLower().Contains("_card_") || entry.FullName.ToLower().Contains("_card.") ? "Card" : "Rad");


                        strTmpFile = Path.Combine(strFileFolderProcess + "\\Working\\", entry.FullName);

                        entry.ExtractToFile(strTmpFile);

                    }
                }

                if (strTmpFile == null)
                    continue;


                //PROCESS
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strTmpFile, strSheetname, strStart: "B5:AK");

                //MESSY SPREADSHEET!!!!
                //OpenXMLExcel.OpenXMLExcel.strCellsToIgnoreArr = new string[] { "D", "E", "K","M", "O" }; //MERGED SO IGNORE THESE
                //wbCurrentExcelFile = SpreadsheetDocument.Open(strTmpFile, false);
                //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 5, 6,1);
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    currentRow = dtFinalDataTable.NewRow();

                    if (dr["State"] == DBNull.Value && dr["Modality"] == DBNull.Value && dr["Month"] == DBNull.Value)
                        break;
                    else if (strState == null && dr["State"] != DBNull.Value)
                        strState = dr["State"].ToString();
                    else if (dr["State"] != DBNull.Value)
                        strState = dr["State"].ToString();



                    currentRow["State"] = strState;
                    currentRow["Modality"] = dr["Modality"];
                    currentRow["Month"] = dr["Month"];
                    currentRow["Member_Lives"] = dr["Member Lives"].ToString().Replace(",", "");
                    currentRow["Total_Requests"] = dr["Total Requests"].ToString().Replace(",", "");
                    currentRow["Approved"] = dr["Approved (A)"].ToString().Replace(",", "");
                    currentRow["Denied"] = dr["Denied (D)"].ToString().Replace(",", "");
                    currentRow["Withdrawn"] = dr["Withdrawn (W)"].ToString().Replace(",", "");
                    currentRow["Expired"] = dr["Expired (Y)"].ToString().Replace(",", "");
                    currentRow["Non_Cert"] = dr["Non-Cert (D+W+Y)"].ToString().Replace(",", "");
                    currentRow["Pending"] = dr["Pending"].ToString().Replace(",", "");


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                }

                //MOVE ZIP
                File.Move(strFile, strFileFolderProcess + "\\Complete\\" + Path.GetFileName(strFile));

                //CLEAR WORKING
                //foreach (FileInfo file in new DirectoryInfo(strFileFolderProcess + "\\Working\\").EnumerateFiles()) file.Delete();

                strTmpFile = null;
               intFileCnt++;
            }



            if(dtFinalDataTable.Rows.Count > 0)
            {
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "stg.EviCore_AmerichoiceAllstatesAuths";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                blUpdated = true;
            }


            return blUpdated;
        }

        private static bool getScorecardData()
        {
            bool blUpdated = false;

            Console.WriteLine("EviCore Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["ScoreCardDateFile_Path"];
            //string strFileList = ConfigurationManager.AppSettings["File_List"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];


            int intFileCnt = 1;


            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");
            SpreadsheetDocument wbCurrentExcelFile;

            DataRow[] drNum;
            int intRowNum = 0;

            string strFileName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strSumRadiologyLOB, strSumCardiologyLOB, strSumRadTherapyLOB, strSumRadiologyCSLOB, strSumCardiologyCSLOB;


            //PREVIOUS WAS FULL YEAR NOW ITS MONTHLY!
            int intStartYear = 2022;
            int intStartMonth = 9;

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_Scorecard]");

            DataTable dtRadiology, dtCardiology, dtRadTherapy, dtRadiologyCS, dtCardiologyCS;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;


            //RESET FINAL TABLE!!!
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("Summary_of_Lob", typeof(String));
            dtFinalDataTable.Columns.Add("Header", typeof(String));
            dtFinalDataTable.Columns.Add("Total_Requests", typeof(int));

            dtFinalDataTable.Columns.Add("Per_Call", typeof(double));
            dtFinalDataTable.Columns.Add("Per_Website", typeof(double));
            dtFinalDataTable.Columns.Add("Per_Fax", typeof(double));
            dtFinalDataTable.Columns.Add("Approved", typeof(double));
            dtFinalDataTable.Columns.Add("Denied", typeof(double));
            dtFinalDataTable.Columns.Add("Withdrawn", typeof(double));
            dtFinalDataTable.Columns.Add("Admin_Expired", typeof(double));
            dtFinalDataTable.Columns.Add("Expired", typeof(double));
            dtFinalDataTable.Columns.Add("Pending", typeof(double));
            dtFinalDataTable.Columns.Add("Non_Cert", typeof(double));
            dtFinalDataTable.Columns.Add("Requests_per_thou", typeof(double));
            dtFinalDataTable.Columns.Add("Approval_per_thou", typeof(double));

            dtFinalDataTable.Columns.Add("MOD_3DI", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_BONE_DENSITY", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CT_SCAN", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_MRA", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_MRI", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_NOT_COVERED_PROCEDURE", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_NUCLEAR_CARDIOLOGY", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_NUCLEAR_MEDICINE", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_PET_SCAN", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_ULTRASOUND", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_UNLISTED_PROCEDURE", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CARDIAC_CATHETERIZATION", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CARDIAC_CT_CCTA", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CARDIAC_IMPLANTABLE_DEVICES", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CARDIAC_MRI", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CARDIAC_PET", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_ECHO_STRESS", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_ECHOCARDIOGRAPHY", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_ECHOCARDIOGRAPHY_ADDON", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_NUCLEAR_STRESS", typeof(double));
            dtFinalDataTable.Columns.Add("MOD_CCCM_Misc_Cath_Codes", typeof(double));



            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));
            intFileCnt = 1;



            strFileFolderPath = @"C:\Users\cgiorda\Desktop\Projects\EvicoreScorecard";

            string[] files;
            //files = Directory.GetFiles(strFileFolderPath, "UHC_Scorecard_*_*-final.xlsx", SearchOption.AllDirectories);
            //files = Directory.GetFiles(strFileFolderPath, "UHC_Scorecard_*_*.xlsx", SearchOption.AllDirectories);
            files = new string[] { strFileFolderPath + "\\UHC_Scorecard_2023_04.xlsx" };

            intFileCnt = 1;
            string[] strFileNameArr;
            foreach (string strFile in files)
            {

                strFileName = Path.GetFileName(strFile);
                strFilePath = Path.GetDirectoryName(strFile);


       

                string dirName = new DirectoryInfo(strFilePath).Name;





                strFileNameArr = strFileName.Replace(".xlsx", "").Replace(".xls", "").Split('_');
                strMonth = strFileNameArr[strFileNameArr.Length -1];
                strYear = strFileNameArr[strFileNameArr.Length - 2];

                //strMonth = dirName.Replace("-", "").Trim().Substring(4, 2);
                //strYear = dirName.Replace("-", "").Trim().Substring(0, 4);

                int intYear = 0;
                int intMonth = 0;
                bool isYearNum = int.TryParse(strYear, out intYear);
                bool isMonthNum = int.TryParse(strMonth, out intMonth);

                if (!isYearNum || !isMonthNum || intYear < intStartYear || (intYear <= intStartYear && intMonth < intStartMonth))
                {
                    intFileCnt++;
                    continue;
                }

            



                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0 || strFileName.Contains("- Copy"))
                {
                    intFileCnt++;
                    continue;
                }



                //strFinalPath = "\\\\nasv1005\\fin360\\phi2\\acad\\Program\\Radiology\\eviCore Monthly Reporting Package\\2018\\201810\\Urgent_TAT_UHC_Enterprise_October_2018.xlsx";
                strReportType = "UHC Scorecard";


                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

               
                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);

                //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////

                strSheetname = "Radiology";
                Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);


                dtRadiology = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 10, 11, 3, blNullColumns: true);
                //if(dtRadiology.Columns.Count == 1)
                    //dtRadiology = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 10, 11, 2);



                strSumRadiologyLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "C3"), OpenXMLExcel.OpenXMLExcel.workbookPart);



                for (int i = dtRadiology.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtRadiology.Rows[i];
                    if (dr["Column1"] + "" == "Case Status")
                        dr.Delete();
                }
                dtRadiology.AcceptChanges();


                foreach (DataColumn c in dtRadiology.Columns)
                {
                    if (c.ColumnName.ToLower() == "total" || c.ColumnName.ToLower().StartsWith("column")) //IGNORE THESE COLUMNS
                        continue;

                    currentRow = dtFinalDataTable.NewRow();

                    currentRow["Summary_of_Lob"] = strSumRadiologyLOB;
                    currentRow["Header"] = c.ColumnName;

                    currentRow["Total_Requests"] = dtRadiology.Rows[1][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Call"] = dtRadiology.Rows[2][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Website"] = dtRadiology.Rows[3][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Fax"] = dtRadiology.Rows[4][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Approved"] = dtRadiology.Rows[12][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Denied"] = dtRadiology.Rows[13][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Withdrawn"] = dtRadiology.Rows[14][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Admin_Expired"] = dtRadiology.Rows[15][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Expired"] = dtRadiology.Rows[16][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Pending"] = dtRadiology.Rows[17][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Non_Cert"] = dtRadiology.Rows[18][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Requests_per_thou"] = (dtRadiology.Rows[19][c.ColumnName] + "" == "#DIV/0!" ? DBNull.Value : dtRadiology.Rows[19][c.ColumnName]);
                    currentRow["Approval_per_thou"] = (dtRadiology.Rows[20][c.ColumnName] + "" == "#DIV/0!" ? DBNull.Value : dtRadiology.Rows[20][c.ColumnName]);


                    drNum = dtRadiology.Select("Column1 = '3DI'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_3DI"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'BONE DENSITY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_BONE_DENSITY"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiology.Select("Column1 = 'ECHO STRESS'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHO_STRESS"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace(" %", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiology.Select("Column1 = 'ECHOCARDIOGRAPHY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace(" %", "").Replace("&", "").Trim();
                    }


                    drNum = dtRadiology.Select("Column1 = 'ECHOCARDIOGRAPHY-ADDON'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY_ADDON"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiology.Select("Column1 = 'CCCM Misc Cath Codes'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CCCM_Misc_Cath_Codes"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }




                    drNum = dtRadiology.Select("Column1 = 'CT SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CT_SCAN"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'MRA'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_MRA"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'MRI'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_MRI"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'NOT COVERED PROCEDURE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NOT_COVERED_PROCEDURE"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'NUCLEAR CARDIOLOGY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_CARDIOLOGY"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'NUCLEAR MEDICINE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_MEDICINE"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiology.Select("Column1 = 'PET SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_PET_SCAN"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiology.Select("Column1 = 'ULTRASOUND'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ULTRASOUND"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiology.Select("Column1 = 'UNLISTED PROCEDURE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_UNLISTED_PROCEDURE"] = dtRadiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);

                    //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                    //currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                }

                //////////////////////////////////////////////////////Cardiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Cardiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Cardiology////////////////////////////////////////////////////////

                strSheetname = "Cardiology";
                Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);
                dtCardiology = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 10, 11, 3, blNullColumns: true);
                strSumCardiologyLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "C3"), OpenXMLExcel.OpenXMLExcel.workbookPart);


                for (int i = dtCardiology.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtCardiology.Rows[i];
                    if (dr["Column1"] + "" == "Case Status")
                        dr.Delete();
                }
                dtCardiology.AcceptChanges();


                foreach (DataColumn c in dtCardiology.Columns)
                {
                    if (c.ColumnName.ToLower() == "total" || c.ColumnName.ToLower().StartsWith("column")) //IGNORE THESE COLUMNS
                        continue;

                    currentRow = dtFinalDataTable.NewRow();

                    currentRow["Summary_of_Lob"] = strSumCardiologyLOB;
                    currentRow["Header"] = c.ColumnName;

                    currentRow["Total_Requests"] = dtCardiology.Rows[1][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Call"] = dtCardiology.Rows[2][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Website"] = dtCardiology.Rows[3][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Fax"] = dtCardiology.Rows[4][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Approved"] = dtCardiology.Rows[12][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Denied"] = dtCardiology.Rows[13][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Withdrawn"] = dtCardiology.Rows[14][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Admin_Expired"] = dtCardiology.Rows[15][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Expired"] = dtCardiology.Rows[16][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Pending"] = dtCardiology.Rows[17][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Non_Cert"] = dtCardiology.Rows[18][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Requests_per_thou"] = (dtCardiology.Rows[19][c.ColumnName] + "" == "#DIV/0!" ? DBNull.Value : dtCardiology.Rows[19][c.ColumnName]);
                    currentRow["Approval_per_thou"] = (dtCardiology.Rows[20][c.ColumnName] + "" == "#DIV/0!" ? DBNull.Value : dtCardiology.Rows[20][c.ColumnName]);


                    drNum = dtCardiology.Select("Column1 = 'CARDIAC CATHETERIZATION'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_CATHETERIZATION"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiology.Select("Column1 = 'CARDIAC CT/CCTA'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_CT_CCTA"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiology.Select("Column1 = 'CARDIAC IMPLANTABLE DEVICES'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_IMPLANTABLE_DEVICES"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiology.Select("Column1 = 'CARDIAC MRI'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_MRI"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiology.Select("Column1 = 'CARDIAC PET'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_PET"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiology.Select("Column1 = 'CCCM Misc Cath Codes'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CCCM_Misc_Cath_Codes"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }



                    drNum = dtCardiology.Select("Column1 = 'CT SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CT_SCAN"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiology.Select("Column1 = 'ECHO STRESS'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHO_STRESS"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiology.Select("Column1 = 'ECHOCARDIOGRAPHY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiology.Select("Column1 = 'ECHOCARDIOGRAPHY-ADDON'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY_ADDON"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }



                    drNum = dtCardiology.Select("Column1 = 'NUCLEAR STRESS'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_STRESS"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiology.Select("Column1 = 'UNLISTED PROCEDURE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_UNLISTED_PROCEDURE"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiology.Select("Column1 = 'PET SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_PET_SCAN"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiology.Select("Column1 = 'NUCLEAR CARDIOLOGY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiology.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_CARDIOLOGY"] = dtCardiology.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }


                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);

                    //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                    //currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                }

                //////////////////////////////////////////////////////Radiation Therapy////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Radiation Therapy////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Radiation Therapy////////////////////////////////////////////////////////
                ///


                if (strMonth == "06" & strYear == "2022")
                {
                    strSheetname = "Radiation Therapy";
                    Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);
                    dtRadTherapy = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 9, 10, 3, blNullColumns: true);
                    strSumRadTherapyLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "C2"), OpenXMLExcel.OpenXMLExcel.workbookPart);
                    foreach (DataColumn c in dtRadTherapy.Columns)
                    {
                        if (c.ColumnName.ToLower() == "total" || c.ColumnName.ToLower().StartsWith("column")) //IGNORE THESE COLUMNS
                            continue;

                        currentRow = dtFinalDataTable.NewRow();

                        currentRow["Summary_of_Lob"] = strSumRadTherapyLOB;
                        currentRow["Header"] = c.ColumnName;

                        currentRow["Total_Requests"] = dtRadTherapy.Rows[1][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Per_Call"] = dtRadTherapy.Rows[2][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Per_Website"] = dtRadTherapy.Rows[3][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Per_Fax"] = dtRadTherapy.Rows[4][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Approved"] = dtRadTherapy.Rows[12][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Denied"] = dtRadTherapy.Rows[13][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Withdrawn"] = dtRadTherapy.Rows[15][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Admin_Expired"] = dtRadTherapy.Rows[16][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Expired"] = dtRadTherapy.Rows[17][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Pending"] = dtRadTherapy.Rows[18][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Non_Cert"] = dtRadTherapy.Rows[19][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                        currentRow["Requests_per_thou"] = (dtRadTherapy.Rows[20][c.ColumnName] + "" == "#DIV/0!" ? DBNull.Value : dtRadTherapy.Rows[19][c.ColumnName]);
                        currentRow["Approval_per_thou"] = (dtRadTherapy.Rows[21][c.ColumnName] + "" == "#DIV/0!" ? DBNull.Value : dtRadTherapy.Rows[20][c.ColumnName]);


                        currentRow["file_month"] = strMonth;
                        currentRow["file_year"] = strYear;
                        currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                        currentRow["sheet_name"] = strSheetname;
                        currentRow["file_name"] = strFileName;
                        currentRow["file_path"] = strFilePath;
                        currentRow["report_type"] = strReportType;
                        dtFinalDataTable.Rows.Add(currentRow);

                        //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                        //currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                    }
                }
                

                //////////////////////////////////////////////////////C&S Radiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////C&S Radiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////C&S Radiology////////////////////////////////////////////////////////
                strSheetname = "C&S Radiology";
                Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);
                dtRadiologyCS = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 7, 8, 3, blNullColumns: true);
                strSumRadiologyCSLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "D3"), OpenXMLExcel.OpenXMLExcel.workbookPart);


                for (int i = dtRadiologyCS.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtRadiologyCS.Rows[i];
                    if (dr["Column1"] + "" == "Case Status")
                        dr.Delete();
                }
                dtRadiologyCS.AcceptChanges();


                foreach (DataColumn c in dtRadiologyCS.Columns)
                {
                    if (c.ColumnName.ToLower() == "total" || c.ColumnName.ToLower().StartsWith("column")) //IGNORE THESE COLUMNS
                        continue;

                    currentRow = dtFinalDataTable.NewRow();

                    currentRow["Summary_of_Lob"] = strSumRadiologyCSLOB;
                    currentRow["Header"] = c.ColumnName;

                    currentRow["Total_Requests"] = dtRadiologyCS.Rows[1][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Call"] = dtRadiologyCS.Rows[2][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Website"] = dtRadiologyCS.Rows[3][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Fax"] = dtRadiologyCS.Rows[4][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Approved"] = dtRadiologyCS.Rows[12][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Denied"] = dtRadiologyCS.Rows[13][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Withdrawn"] = dtRadiologyCS.Rows[14][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Admin_Expired"] = dtRadiologyCS.Rows[15][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Expired"] = dtRadiologyCS.Rows[16][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Pending"] = dtRadiologyCS.Rows[17][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Non_Cert"] = DBNull.Value;
                    currentRow["Requests_per_thou"] = DBNull.Value;
                    currentRow["Approval_per_thou"] = DBNull.Value;





                    drNum = dtRadiologyCS.Select("Column1 = '3DI'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_3DI"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'BONE DENSITY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_BONE_DENSITY"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiologyCS.Select("Column1 = 'ECHO STRESS'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHO_STRESS"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace(" %", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiologyCS.Select("Column1 = 'ECHOCARDIOGRAPHY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace(" %", "").Replace("&", "").Trim();
                    }


                    drNum = dtRadiologyCS.Select("Column1 = 'ECHOCARDIOGRAPHY-ADDON'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY_ADDON"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiologyCS.Select("Column1 = 'CCCM Misc Cath Codes'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CCCM_Misc_Cath_Codes"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }


                    drNum = dtRadiologyCS.Select("Column1 = 'CT SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CT_SCAN"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'MRA'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_MRA"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'MRI'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_MRI"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'NOT COVERED PROCEDURE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NOT_COVERED_PROCEDURE"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'NUCLEAR CARDIOLOGY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_CARDIOLOGY"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'NUCLEAR MEDICINE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_MEDICINE"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiologyCS.Select("Column1 = 'PET SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_PET_SCAN"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtRadiologyCS.Select("Column1 = 'ULTRASOUND'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ULTRASOUND"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtRadiologyCS.Select("Column1 = 'UNLISTED PROCEDURE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtRadiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_UNLISTED_PROCEDURE"] = dtRadiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }



                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);

                    //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                    //currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                }
                //////////////////////////////////////////////////////C&S Cardiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////C&S Cardiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////C&S Cardiology////////////////////////////////////////////////////////
                strSheetname = "C&S Cardiology";
                Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);
                dtCardiologyCS = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 7, 8, 3, blNullColumns: true);
                strSumCardiologyCSLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "D3"), OpenXMLExcel.OpenXMLExcel.workbookPart);

                for (int i = dtCardiologyCS.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtCardiologyCS.Rows[i];
                    if (dr["Column1"] + "" == "Case Status")
                        dr.Delete();
                }
                dtCardiologyCS.AcceptChanges();


                foreach (DataColumn c in dtCardiologyCS.Columns)
                {
                    if (c.ColumnName.ToLower() == "total" || c.ColumnName.ToLower().StartsWith("column")) //IGNORE THESE COLUMNS
                        continue;

                    currentRow = dtFinalDataTable.NewRow();

                    currentRow["Summary_of_Lob"] = strSumCardiologyCSLOB;
                    currentRow["Header"] = c.ColumnName;

                    currentRow["Total_Requests"] = dtCardiologyCS.Rows[1][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Call"] = dtCardiologyCS.Rows[2][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Website"] = dtCardiologyCS.Rows[3][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Per_Fax"] = dtCardiologyCS.Rows[4][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Approved"] = dtCardiologyCS.Rows[12][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Denied"] = dtCardiologyCS.Rows[13][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Withdrawn"] = dtCardiologyCS.Rows[14][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Admin_Expired"] = dtCardiologyCS.Rows[15][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Expired"] = dtCardiologyCS.Rows[16][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Pending"] = dtCardiologyCS.Rows[17][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    currentRow["Non_Cert"] = DBNull.Value;
                    currentRow["Requests_per_thou"] = DBNull.Value;
                    currentRow["Approval_per_thou"] = DBNull.Value;




                    drNum = dtCardiologyCS.Select("Column1 = 'CARDIAC CATHETERIZATION'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_CATHETERIZATION"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'CARDIAC CT/CCTA'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_CT_CCTA"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'CARDIAC IMPLANTABLE DEVICES'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_IMPLANTABLE_DEVICES"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'CARDIAC MRI'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_MRI"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiologyCS.Select("Column1 = 'CARDIAC PET'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CARDIAC_PET"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'CT SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CT_SCAN"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'ECHO STRESS'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHO_STRESS"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiologyCS.Select("Column1 = 'ECHOCARDIOGRAPHY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiologyCS.Select("Column1 = 'ECHOCARDIOGRAPHY-ADDON'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_ECHOCARDIOGRAPHY_ADDON"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }


                    drNum = dtCardiologyCS.Select("Column1 = 'CCCM Misc Cath Codes'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_CCCM_Misc_Cath_Codes"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }

                    drNum = dtCardiologyCS.Select("Column1 = 'NUCLEAR STRESS'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_STRESS"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'UNLISTED PROCEDURE'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_UNLISTED_PROCEDURE"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'PET SCAN'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_PET_SCAN"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }
                    drNum = dtCardiologyCS.Select("Column1 = 'NUCLEAR CARDIOLOGY'");
                    if (drNum.Length > 0)
                    {
                        intRowNum = dtCardiologyCS.Rows.IndexOf(drNum[0]);
                        currentRow["MOD_NUCLEAR_CARDIOLOGY"] = dtCardiologyCS.Rows[intRowNum][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    }


                    //currentRow["MOD_CARDIAC_CATHETERIZATION"] = dtCardiologyCS.Rows[19][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    //currentRow["MOD_CARDIAC_IMPLANTABLE_DEVICES"] = dtCardiologyCS.Rows[20][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    //currentRow["MOD_ECHO_STRESS"] = dtCardiologyCS.Rows[21][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();
                    //currentRow["MOD_ECHOCARDIOGRAPHY"] = dtCardiologyCS.Rows[22][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();

                    //if (dtCardiologyCS.Rows.Count >23) //UHC_Scorecard_2020_01.xlsx :(
                    //    currentRow["MOD_UNLISTED_PROCEDURE"] = dtCardiologyCS.Rows[23][c.ColumnName].ToString().Replace("%", "").Replace("&", "").Trim();

                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);

                    //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                    //currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                }

                intFileCnt++;

            }

            if(dtFinalDataTable.Rows.Count > 0)
            {
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "stg.EviCore_Scorecard";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                blUpdated = true;

            }

            return blUpdated;
        }

        private static bool getTATData()
        {
            bool blUpdated = false;

            Console.WriteLine("EviCore Parser");
            //string strFileFolderPath = ConfigurationManager.AppSettings["File_Path2"];
            string strFileFolderPath = ConfigurationManager.AppSettings["Tat_Path"];
            string strFileList = ConfigurationManager.AppSettings["File_List"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];


            int intFileCnt = 1;
            int intSheetCnt = 1;


            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");
            SpreadsheetDocument wbCurrentExcelFile;

            string strFileName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string strSummaryofLOB = null;
            string strCurrentState = null;
            string strCleanedColumnName = null;

            //PREVIOUS WAS FULL YEAR NOW ITS MONTHLY!
            int intStartYearForMonthly = 2022;


            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_TAT]");



            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;

            string[] files;
            ArrayList alFiles = new ArrayList();
            //files = Directory.GetFiles(strFileFolderPath, "United_Enterprise_Wide_*_TAT_UHC_Enterprise_*.xlsx", SearchOption.AllDirectories);
            files = new string[] { @"C:\Users\cgiorda\Desktop\MHP_Requests\United_Enterprise_Wide_Routine_TAT_UHC_Enterprise_2022_12.xlsx", @"C:\Users\cgiorda\Desktop\MHP_Requests\United_Enterprise_Wide_Urgent_TAT_UHC_Enterprise_2022_12.xlsx" };
            foreach(string f in files)
            {
                if (!f.Contains("NPS") && !f.Contains("Scorecard"))
                    alFiles.Add(f);
            }

            //files = Directory.GetFiles(strFileFolderPath, "United_Enterprise_Wide_Urgent_TAT_UHC_Enterprise_*.xlsx", SearchOption.AllDirectories);
            //foreach (string f in files)
            //{
            //    if (!f.Contains("NPS Scorecard"))
            //        alFiles.Add(f);
            //}

            intFileCnt = 1;

            string[] strFileNameArr;
            //RESET FINAL TABLE!!!
            dtFinalDataTable = null;
            foreach (string strFile in alFiles)
            {

                strFileName = Path.GetFileName(strFile);
                strFilePath = Path.GetDirectoryName(strFile);

                string dirName = new DirectoryInfo(strFilePath).Name;
                if (dirName.Length < 6)
                    continue;


                strFileNameArr = strFileName.Replace(".xlsx", "").Replace(".xls", "").Split('_');
                strMonth = strFileNameArr[strFileNameArr.Length - 1];
                strYear = strFileNameArr[strFileNameArr.Length - 2];



                //strMonth = dirName.Replace("-", "").Trim().Substring(4, 2);
                //strYear = dirName.Replace("-", "").Trim().Substring(0, 4);

                int intFolderNum = 0;
                bool isNum = int.TryParse(strYear, out intFolderNum);
                if (!isNum || intFolderNum < intStartYearForMonthly)
                {
                    intFileCnt++;
                    continue;
                }

                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0 || strFileName.ToLower().Contains("revised"))
                {
                    intFileCnt++;
                    continue;
                }


                //strFinalPath = "\\\\nasv1005\\fin360\\phi2\\acad\\Program\\Radiology\\eviCore Monthly Reporting Package\\2018\\201810\\Urgent_TAT_UHC_Enterprise_October_2018.xlsx";
                strReportType = (strFileName.Contains("Routine") ? "Routine TAT" : "Urgent TAT");

                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", alFiles.Count) + " spreadsheets");

                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);

                var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
                intSheetCnt = 1;
                foreach (Sheet item in results)
                {
                    Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + item.Name);
                    if (item.Name.ToString().ToLower().Trim().Equals("document map") || item.Name.ToString().ToLower().Trim().Equals("sheet2"))
                        continue;

                    strSheetname = item.Name.ToString();

                    //if (strSheetname == "sheet15")
                    //{
                    //    string s = "t";
                    //}

                    OpenXMLExcel.OpenXMLExcel.strCellsToIgnoreArr = new string[] { "E", "F" }; //MERGED SO IGNORE THESE
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 3, 4, 3);
                    strSummaryofLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "F1"), OpenXMLExcel.OpenXMLExcel.workbookPart);


                    //ONLY ONE TIME PER LOOP
                    if (intSheetCnt == 1)
                    {
                        if (dtFinalDataTable == null)
                        {
                            dtFinalDataTable = dtCurrentDataTable.Clone();
                            foreach (DataColumn col in dtFinalDataTable.Columns)
                            {
                                strCleanedColumnName = col.ColumnName.Trim().Replace("% <=", "PerLessEqual").Replace("<=", "LessEqual").Replace("% <", "PerLess").Replace("<", "Less").Replace("/", "_").Replace(" ", "_");
                                col.ColumnName = strCleanedColumnName;
                                //if (strCleanedColumnName == "Total_Authorizations_Notifications" || strCleanedColumnName == "LessEqual_2_BUS_Days" || strCleanedColumnName == "Less_State_TAT_Requirements")
                                //    col.DataType = typeof(Int32);

                            }
                            dtFinalDataTable.Columns.Add("Summary_of_Lob", typeof(String));
                            dtFinalDataTable.Columns.Add("file_month", typeof(String));
                            dtFinalDataTable.Columns.Add("file_year", typeof(String));
                            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
                            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
                            dtFinalDataTable.Columns.Add("file_name", typeof(String));
                            dtFinalDataTable.Columns.Add("file_path", typeof(String));
                            dtFinalDataTable.Columns.Add("report_type", typeof(String));
                        }
                        else
                        {
                            //DOING IT ABOVE COMPARE/ADD COLUMNS PER SHEET
                            foreach (DataColumn col in dtCurrentDataTable.Columns)
                            {

                                strCleanedColumnName = col.ColumnName.Trim().Replace("% <=", "PerLessEqual").Replace("<=", "LessEqual").Replace("% <", "PerLess").Replace("<", "Less").Replace("/", "_").Replace(" ", "_");
                                if (!dtFinalDataTable.Columns.Contains(strCleanedColumnName))
                                    dtFinalDataTable.Columns.Add(strCleanedColumnName, typeof(double));
                            }
                        }
                    }

                    //TRACK STATE FOR BLANK ROWS
                    strCurrentState = null;
                    foreach (DataRow d in dtCurrentDataTable.Rows)
                    {
                        if (strCurrentState == null || (strCurrentState != d["Carrier State"].ToString() && d["Carrier State"] != DBNull.Value))
                        {
                            strCurrentState = d["Carrier State"].ToString();
                        }
                        d["Carrier State"] = strCurrentState;
                    }


                    foreach (DataRow d in dtCurrentDataTable.Select("Modality IS NOT NULL "))
                    {

                        currentRow = dtFinalDataTable.NewRow();

                        foreach (DataColumn c in dtCurrentDataTable.Columns)
                        {

                            strCleanedColumnName = c.ColumnName.Trim().Replace("% <=", "PerLessEqual").Replace("<=", "LessEqual").Replace("% <", "PerLess").Replace("<", "Less").Replace("/", "_").Replace(" ", "_");


                            if (c.ColumnName != "Carrier State")
                            {
                                //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                                currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                            }
                            else
                            {
                                if (strCurrentState == null || strCurrentState != d[c.ColumnName].ToString())
                                {
                                    strCurrentState = d[c.ColumnName].ToString();
                                }

                                currentRow[strCleanedColumnName] = strCurrentState;
                            }
                        }

                        currentRow["Summary_of_Lob"] = strSummaryofLOB;
                        currentRow["file_month"] = strMonth;
                        currentRow["file_year"] = strYear;
                        currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                        currentRow["sheet_name"] = strSheetname;
                        currentRow["file_name"] = strFileName;
                        currentRow["file_path"] = strFilePath;
                        currentRow["report_type"] = strReportType;
                        dtFinalDataTable.Rows.Add(currentRow);
                    }

                    //Total Authorizations/Notifications = Total_Authorizations_Notifications
                    //<= 2 BUS Days = LessEqual_2_BUS_Days
                    //% <= 2 BUS Days = PerLessEqual_2_BUS_Days

                    intSheetCnt++;
                }

                //wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
                //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, (strSpreadsheetPrefixName + strFileDate).Substring(0, 31), 2, 3);

                intFileCnt++;

            }

            if (dtFinalDataTable != null)
            {
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "stg.EviCore_TAT";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);
                blUpdated = true;
            }

            return blUpdated;
        }

        private static bool getNICEUHCWestEligibilityData()
        {
            bool blUpdated = false;

            Console.WriteLine("EviCore Parser");
            //string strFileFolderPath = @"\\nasv1005\fin360\phi2\acad\Program\Radiology\eviCore Monthly Reporting Package\M&R membership";
            string strFileFolderPath = @"\\msp09fil01\Radiology\Capitation";
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_NICEDetails]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("Contract_Number", typeof(String));
            dtFinalDataTable.Columns.Add("PBP", typeof(String));
            dtFinalDataTable.Columns.Add("Company_State", typeof(String));
            dtFinalDataTable.Columns.Add("Member_Count", typeof(int));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));
            dtFinalDataTable.TableName = "stg.EviCore_NICEDetails";

            DataTable dtTotalsDataTable = null;
            DataRow totalsRow;
            dtTotalsDataTable = new DataTable();
            dtTotalsDataTable.Columns.Add("Grand_Total", typeof(Int64));
            dtTotalsDataTable.Columns.Add("file_name", typeof(String));
            dtTotalsDataTable.TableName = "stg.EviCore_NICETotals";



            string strFileName = null;
            string[] strFileNameArr = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strDay = null;
            string strSheetname = "by Hplan-PBP";
            strSheetname = "by Hplan_PBP";
            string strCntColumn = null;

            int intStartingYear = 2022;

            string[] files;
            //files = Directory.GetFiles(strFileFolderPath, "NICE_UHCWestEligibility_*_*_*_Medicare_Final_for_membership.xlsb", SearchOption.AllDirectories);
            files = Directory.GetFiles(strFileFolderPath, "NICE_UHCWestEligibility_*_Medicare_Final_for_membership.xls*", SearchOption.AllDirectories);
            int intFileCnt = 1;
            int intRowCnt = 1;
            foreach (string strFile in files)
            {
                strFileName = Path.GetFileName(strFile);
                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }

               


                strFileNameArr = strFileName.Split('_');
                strMonth = strFileNameArr[3];
                strYear = strFileNameArr[2];
                strDay = strFileNameArr[4];
                strReportType = "NICE";
                strFilePath = Path.GetDirectoryName(strFile);

                if (!int.TryParse(strMonth, out int n) || !int.TryParse(strYear, out int n2) || !int.TryParse(strDay, out int n3))
                {

                    //if (strFileName == "NICE_UHCWestEligibility_202206_Medicare_Final_for_membership.xlsx")
                    //{
                    //    strMonth = strFileNameArr[2].Substring(4, 2);
                    //    strYear = strFileNameArr[2].Substring(0, 4);
                    //    strDay = "01";
                    //}
                    //else
                    //    continue;

                    strMonth = strFileNameArr[2].Substring(4, 2);
                    strYear = strFileNameArr[2].Substring(0, 4);
                    strDay = "01";
                    if (!int.TryParse(strMonth, out n) || !int.TryParse(strYear, out n2) || !int.TryParse(strDay, out n3))
                    {

                        continue;

                    }
                    else
                    {
                        string s = "";
                    }

                }

                if (Int32.Parse(strYear) < intStartingYear)
                {
                    continue;
                }





                if (strMonth.Length == 1)
                    strMonth = "0" + strMonth;

                if (strDay.Length == 1)
                    strDay = "0" + strDay;


                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname, strStart: "A3:D");
                if(!dtCurrentDataTable.Columns.Contains("Contract Number"))//JERKS!!!
                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname, strStart: "A4:D");

                intRowCnt = 1;
                foreach (DataRow dr in dtCurrentDataTable.Rows)
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    if (dr["Contract Number"] == DBNull.Value || dr["Contract Number"] == null)
                    {
                        intRowCnt++;
                        continue;
                    }



                    if (dtCurrentDataTable.Columns.Contains("Count of Rad/Card/Rad therapy indicator"))
                        strCntColumn = "Count of Rad/Card/Rad therapy indicator";
                    else if (dtCurrentDataTable.Columns.Contains("CountOfMemberid"))
                        strCntColumn = "CountOfMemberid";
                    else if (dtCurrentDataTable.Columns.Contains("Count of MEMBERID"))
                        strCntColumn = "Count of MEMBERID";
                    else if (dtCurrentDataTable.Columns.Contains("Total"))
                        strCntColumn = "Total";


                    if (dr["Contract Number"].ToString().ToLower().Replace(" ", "").Contains("grandtotal"))
                    {
                        totalsRow = dtTotalsDataTable.NewRow();
                        totalsRow["Grand_Total"] = dr[strCntColumn];
                        totalsRow["file_name"] = strFileName;
                        dtTotalsDataTable.Rows.Add(totalsRow);
                        break;
                    }

                  



                    currentRow = dtFinalDataTable.NewRow();
                    currentRow["Contract_Number"] = dr["Contract Number"];
                    currentRow["PBP"] = dr["PBP"];
                    currentRow["Company_State"] = dr["Company State"];
                    currentRow["Member_Count"] =  dr[strCntColumn].ToString().Replace(",","");

                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + strDay + "/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;


                if (dtFinalDataTable.Rows.Count > 0)
                {

                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                    DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                    DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                    DBConnection32.SQLServerBulkImportDT(dtTotalsDataTable, strILUCAConnectionString, 1);

                    dtFinalDataTable.Rows.Clear();
                    dtTotalsDataTable.Rows.Clear();
                    GC.Collect(2, GCCollectionMode.Forced);

                    blUpdated = true;
                }


                intFileCnt++;
            }

            return blUpdated;
        }

        private static bool getMRMembershipData()
        {
            bool blUpdated = false;
            Console.WriteLine("EviCore Parser");
            //string strFileFolderPath = @"\\nasv1005\fin360\phi2\acad\Program\Radiology\eviCore Monthly Reporting Package\M&R membership";
            string strFileFolderPath = @"\\msp09fil01\Radiology\Capitation";
            strFileFolderPath = @"\\msp09fil01\Radiology\Capitation\2022";
            //strFileFolderPath = @"C:\Users\cgiorda\Desktop\garbage";
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_MR_MembershipDetails]");
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("IncurredDt", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Program", typeof(String));
            dtFinalDataTable.Columns.Add("MemberCount", typeof(int));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));
            dtFinalDataTable.TableName = "stg.EviCore_MR_MembershipDetails";
            string strFileName = null;
            string[] strFileNameArr = null;
            string strFileDate = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = "CRC_Pivot_Rawdata_";
            bool blReGroup = false;
            int intStartingYear = 2022;

            string[] files;
            files = Directory.GetFiles(strFileFolderPath, "CRC_Pivot_Rawdata_*", SearchOption.AllDirectories);
            int intFileCnt = 1;
            int intRowCnt = 1;
            foreach (string strFile in files)
            {
                strFileName = Path.GetFileName(strFile);
                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }

                strFileNameArr = strFileName.Split('_');
                strFileDate = strFileNameArr[strFileNameArr.Length - 1].Replace(".xlsb", "").Replace(".xlsx", "");

                if(strFileDate.Length != 6)
                {
                    intFileCnt++;
                    continue;
                }



                strMonth = strFileDate.Substring(4, 2);
                strYear = strFileDate.Substring(0, 4);
                if (!int.TryParse(strMonth, out int n) || !int.TryParse(strYear, out int n2))
                {
                    continue;
                }



                if (Int32.Parse(strYear) < intStartingYear)
                {
                    continue;
                }

                


                strReportType = "COSMOS";
                strFilePath = Path.GetDirectoryName(strFile);


                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                //WHERE BROKE DUE TO ACE HDR=NO; CANT FIND COLUMNS
                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname + strFileDate, strWhere : " WHERE [Program] = \"Cardiology\" OR [Program] = \"Radiology\"");
                //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname + strFileDate);

                intRowCnt = 1;
                //foreach (DataRow dr in dtCurrentDataTable.Rows)
                foreach (DataRow dr in dtCurrentDataTable.Select("Program = 'Cardiology' OR Program = 'Radiology'"))
                {

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

                    currentRow = dtFinalDataTable.NewRow();
                    currentRow["IncurredDt"] = DateTime.Parse(dr["IncurredDt"].ToString());
                    currentRow["Program"] = dr["Program"];
                    currentRow["MemberCount"] = dr["MemberCount"];
   
                    currentRow["file_month"] = strMonth;
                    currentRow["file_year"] = strYear;
                    currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                    currentRow["sheet_name"] = strSheetname;
                    currentRow["file_name"] = strFileName;
                    currentRow["file_path"] = strFilePath;
                    currentRow["report_type"] = strReportType;
                    dtFinalDataTable.Rows.Add(currentRow);
                    intRowCnt++;
                }
                currentRow = null;
                dtCurrentDataTable = null;

                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);
                blReGroup = true;


                dtFinalDataTable.Rows.Clear();
                GC.Collect(2, GCCollectionMode.Forced);


                intFileCnt++;
            }

            if(blReGroup)
            {
                blUpdated = true;
                //WIPE ROLLUP AND REAGGERGATE
                string strFinalSQL = "truncate table [stg].[EviCore_MR_MembershipRollup];INSERT INTO[stg].[EviCore_MR_MembershipRollup] ([IncurredDt],[program],[Membership],[source]) SELECT[IncurredDt], [Program], SUM(MemberCount) as [Membership] ,'COSMOS' as source FROM[IL_UCA].[stg].[EviCore_MR_MembershipDetails] group by[IncurredDt], [Program];";

                DBConnection32.getMSSQLDataTable(strILUCAConnectionString, strFinalSQL);
            }
            return blUpdated;
        }

       
        private static bool getCiscoYTDMetricsData()
        {
            bool blUpdated = false;

            Console.WriteLine("EviCore Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["YTDCiscoFile_Path"];
            string strFileList = ConfigurationManager.AppSettings["File_List"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];



            strFileFolderPath = @"C:\Users\cgiorda\Desktop\ManualParsings";


            int intFileCnt = 1;
            int intSheetCnt = 1;

            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");
            SpreadsheetDocument wbCurrentExcelFile;

            string strFileName = null;
            string strFilePath = null;
            string strReportType = null;
            string strMonth = null;
            string strMonthName = null;
            string strYear = null;
            string strSheetname = null;
            string strSumOfLOB;


            //PREVIOUS WAS FULL YEAR NOW ITS MONTHLY!
            int intStartYearForMonthly = 2022;


            DataTable dtCurrentDataTable;
            DataTable dtFinalDataTable = null;
            DataRow currentRow;
            DataRow[] drArr = null;

            DateTime monthNumber;
            bool blIsValidMonth;

            //MAP FOR EXCEL ROW NUMS PER SHEET
            populateSheetRows();


            //RESET FINAL TABLE!!!
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("Summary_of_Lob", typeof(String));
            dtFinalDataTable.Columns.Add("Call_Taker", typeof(String));
            dtFinalDataTable.Columns.Add("Total_Calls", typeof(int));
            dtFinalDataTable.Columns.Add("Avg_Speed_Answer", typeof(double));
            dtFinalDataTable.Columns.Add("Abandoned_Calls", typeof(double));
            dtFinalDataTable.Columns.Add("Abandoned_Percent", typeof(double));
            dtFinalDataTable.Columns.Add("Average_Talk_Time", typeof(double));
            dtFinalDataTable.Columns.Add("ASA_in_SL_Perent", typeof(double));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_path", typeof(String));
            dtFinalDataTable.Columns.Add("report_type", typeof(String));
            intFileCnt = 1;


            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_YTDMetrics]");

            string[] strFileNameArr;
            string[] strFileNameArr2;
            //CHECK AND COPY FOR NEWS ZIPS TO PROCESS
            string[] files;

            //files = Directory.GetFiles(strFileFolderPath, "YTD - Cisco - UHC Metrics *.xlsx", SearchOption.AllDirectories);
            files = new string[] { @"C:\Users\cgiorda\Desktop\Projects\CiscoYTDMetrics\YTD - Cisco - UHC Metrics 2023_02.xlsx" };

            foreach (string strFile in files)
            {

                strFileName = Path.GetFileName(strFile);
                strFilePath = Path.GetDirectoryName(strFile);

                //string dirName = new DirectoryInfo(strFilePath).Name;
                //strMonth = dirName.Replace("-", "").Trim().Substring(4, 2);
                //strYear = dirName.Replace("-", "").Trim().Substring(0, 4);

            
                strFileNameArr = strFileName.Replace(".xlsx", "").Split('_');
                strMonth = strFileNameArr[strFileNameArr.Length - 1];
                strFileNameArr2 = strFileNameArr[0].Split(' ');
                strYear = strFileNameArr2[strFileNameArr2.Length -1];



                int intFolderNum = 0;
                bool isNum = int.TryParse(strYear, out intFolderNum);
                if(!isNum || intFolderNum < intStartYearForMonthly)
                {
                    intFileCnt++;
                    continue;
                }

                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0 || strFileName.ToLower().Contains("revised") || strFileName.ToLower().Contains("exclusion"))
                {
                    intFileCnt++;
                    continue;
                }


                //strFinalPath = "\\\\nasv1005\\fin360\\phi2\\acad\\Program\\Radiology\\eviCore Monthly Reporting Package\\2018\\201810\\Urgent_TAT_UHC_Enterprise_October_2018.xlsx";
                strReportType = "Cisco UHC Metrics";
                strMonthName = new DateTime(1972, int.Parse(strMonth), 1).ToString("MMMM", CultureInfo.InvariantCulture);

                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

                wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);

                var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
                intSheetCnt = 1;
                foreach (Sheet item in results)
                {
                    strSheetname = item.Name.ToString();



                    var sheets = alSheetRowsGlobal.Where(x => x.strSheetName == strSheetname);
                    if (sheets.Count() == 0)
                        continue;

                    Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);

                    dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 4, 5);
                    strSumOfLOB = OpenXMLExcel.OpenXMLExcel.GetCellValue(OpenXMLExcel.OpenXMLExcel.GetCell(OpenXMLExcel.OpenXMLExcel.sheetData, "A2"), OpenXMLExcel.OpenXMLExcel.workbookPart);
                    foreach (DataColumn c in dtCurrentDataTable.Columns)
                    {

                        blIsValidMonth = DateTime.TryParseExact(c.ColumnName, "MMMM", CultureInfo.CurrentCulture, DateTimeStyles.None, out monthNumber);

                        if (!blIsValidMonth || c.ColumnName != strMonthName)
                            continue;



                        strMonth = monthNumber.Month.ToString();
                        if (strMonth.Length == 1)
                            strMonth = "0" + strMonth;

                        foreach (var s in sheets)
                        {

                            currentRow = dtFinalDataTable.NewRow();

                            currentRow["Summary_of_Lob"] = strSumOfLOB;

                            //CASE/IF STATEMENTS FOR ROW NUMS?????
                            currentRow["Call_Taker"] = s.strCallTaker;

                            currentRow["Total_Calls"] = (s.intTotalCallsRow != null ? dtCurrentDataTable.Rows[(int)s.intTotalCallsRow][c.ColumnName] : DBNull.Value);
                            currentRow["Avg_Speed_Answer"] = (s.intAvgSpeedAnswerRow != null ? dtCurrentDataTable.Rows[(int)s.intAvgSpeedAnswerRow][c.ColumnName] : DBNull.Value);
                            currentRow["Abandoned_Calls"] = (s.intAbandonedCallsRow != null ? dtCurrentDataTable.Rows[(int)s.intAbandonedCallsRow][c.ColumnName] : DBNull.Value);
                            currentRow["Abandoned_Percent"] = (s.intAbandonedPercentRow != null ? dtCurrentDataTable.Rows[(int)s.intAbandonedPercentRow][c.ColumnName] : DBNull.Value);
                            currentRow["Average_Talk_Time"] = (s.intAverageTalkTimeRow != null ? dtCurrentDataTable.Rows[(int)s.intAverageTalkTimeRow][c.ColumnName] : DBNull.Value);
                            currentRow["ASA_in_SL_Perent"] = (s.intASAinSLPerentRow != null ? dtCurrentDataTable.Rows[(int)s.intASAinSLPerentRow][c.ColumnName] : DBNull.Value);



                            currentRow["file_month"] = strMonth;
                            currentRow["file_year"] = strYear;
                            currentRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                            currentRow["sheet_name"] = strSheetname;
                            currentRow["file_name"] = strFileName;
                            currentRow["file_path"] = strFilePath;
                            currentRow["report_type"] = strReportType;
                            dtFinalDataTable.Rows.Add(currentRow);
                        }

                        //CHECK IF  currentRow[tmpColumnName] exists  if not add it!!!!! SOMEHOW.... DOING IT ABOVE
                        //currentRow[strCleanedColumnName] = (d[c.ColumnName] != DBNull.Value && !(d[c.ColumnName] + "").Trim().Equals("") ? d[c.ColumnName] : DBNull.Value);
                    }

                }

                intFileCnt++;

            }

            if (dtFinalDataTable.Rows.Count > 0)
            {
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "stg.EviCore_YTDMetrics";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);
                blUpdated = true;
            }


            return blUpdated;
        }


        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }



        private static List<SheetRows> alSheetRowsGlobal;
        private static void populateSheetRows()
        {
            alSheetRowsGlobal = new List<SheetRows>();
            SheetRows sr;

            //HONG's MINUS 5 EX: B8 = 3 OR B10 = 5
            sr = new SheetRows();
            sr.strSheetName = "Empire"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = null; sr.intAbandonedPercentRow = 4; sr.intAverageTalkTimeRow = 5; sr.intASAinSLPerentRow = 6;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "NHP"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 3; sr.intAvgSpeedAnswerRow = 2; sr.intAbandonedCallsRow = 5; sr.intAbandonedPercentRow = 7; sr.intAverageTalkTimeRow = 4; sr.intASAinSLPerentRow = 6;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "River_Valley"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = 4; sr.intAbandonedPercentRow = 5; sr.intAverageTalkTimeRow = null; sr.intASAinSLPerentRow = null;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "UHC EI Rad"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = 4; sr.intAbandonedPercentRow =5; sr.intAverageTalkTimeRow = 7; sr.intASAinSLPerentRow = 15;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "UHC EI Rad"; sr.strCallTaker = "MD"; sr.intTotalCallsRow = 9; sr.intAvgSpeedAnswerRow = 10; sr.intAbandonedCallsRow = 11; sr.intAbandonedPercentRow = 12; sr.intAverageTalkTimeRow = 14; sr.intASAinSLPerentRow = 15;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "UHC EI Card "; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = 4; sr.intAbandonedPercentRow = 5; sr.intAverageTalkTimeRow = 6; sr.intASAinSLPerentRow = 14;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "UHC EI Card "; sr.strCallTaker = "MD"; sr.intTotalCallsRow = 8; sr.intAvgSpeedAnswerRow = 9; sr.intAbandonedCallsRow = 10; sr.intAbandonedPercentRow = 11; sr.intAverageTalkTimeRow = 12; sr.intASAinSLPerentRow = 13;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "CP Rad"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = null; sr.intAbandonedPercentRow = 4; sr.intAverageTalkTimeRow = 5; sr.intASAinSLPerentRow = null;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "CP Card"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = null; sr.intAbandonedPercentRow = 4; sr.intAverageTalkTimeRow = 5; sr.intASAinSLPerentRow = null;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "Oxford Rad"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = null; sr.intAbandonedPercentRow = 4; sr.intAverageTalkTimeRow = 5; sr.intASAinSLPerentRow = null;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "Oxford Card"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = null; sr.intAbandonedPercentRow = 4; sr.intAverageTalkTimeRow = 5; sr.intASAinSLPerentRow = null;
            alSheetRowsGlobal.Add(sr);
            sr = new SheetRows();
            sr.strSheetName = "Medicare"; sr.strCallTaker = "Intake"; sr.intTotalCallsRow = 2; sr.intAvgSpeedAnswerRow = 3; sr.intAbandonedCallsRow = 4; sr.intAbandonedPercentRow = 5; sr.intAverageTalkTimeRow = null; sr.intASAinSLPerentRow = null;
            alSheetRowsGlobal.Add(sr);
        }

        private static void analyzeFiles()
        {
            Console.WriteLine("EviCore Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            string[] files;
            int intFileCnt = 1;
            int intRowCnt = 1;


            Console.WriteLine("Getting files from Shared Drive");
            //GET ALL FILES FROM SHAREPOINT UNZIP IF NEEDED
            files = Directory.GetFiles(strFileFolderPath, "*.*", SearchOption.AllDirectories);
            //files = Directory.GetFiles(strFileFolderPath, "*.xlsx", SearchOption.AllDirectories);


            string[] strFilePathArr = null;
            string strYear = null;
            string strYearMonth = null;
            string strFileDate = null;

            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");
            string strSpreadsheetPrefixName = "ACIS_MedNec_Report_Full_";
            SpreadsheetDocument wbCurrentExcelFile;
            DataTable dtFinalDataTable = new DataTable();
            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Clear();
            dtFinalDataTable.Columns.Add("Path");
            dtFinalDataTable.Columns.Add("Filename");
            dtFinalDataTable.Columns.Add("Type");
            dtFinalDataTable.Columns.Add("Year");
            dtFinalDataTable.Columns.Add("Month");
            dtFinalDataTable.Columns.Add("FileDate");
            DataRow currentRow;
            intFileCnt = 1;
            intRowCnt = 1;
            foreach (string strFile in files)
            {
                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");
                //string strExtension = Path.GetExtension(strFile);
                //string strFileName = Path.GetFileName(strFile);
                //string strFileDate = strFileName.ToLower().Split('_')[4].Replace(".xlsx", "");
                strFilePathArr = Path.GetDirectoryName(strFile.ToLower()).Split('\\');




                strYear = strFilePathArr[strFilePathArr.Length - 2];
                strYearMonth = strFilePathArr[strFilePathArr.Length - 1];
                if (strYear.Length > 4)
                {
                    strYear = strFilePathArr[strFilePathArr.Length - 3];
                    strYearMonth = strFilePathArr[strFilePathArr.Length - 2];
                }


                strFileDate = "01/" + strYearMonth.Substring(strYearMonth.Length - 2, 2) + "/" + strYear;


                currentRow = dtFinalDataTable.NewRow();
                currentRow["Path"] = Path.GetDirectoryName(strFile);
                currentRow["Filename"] = Path.GetFileName(strFile);
                currentRow["Type"] = Path.GetExtension(strFile);
                currentRow["Year"] = strYear;
                currentRow["Month"] = strYearMonth.Substring(strYearMonth.Length - 2, 2);
                currentRow["FileDate"] = DateTime.Parse(strFileDate);
                //currentRow["FileDate"] = DateTime.ParseExact(, "dd/mm/yyyy", CultureInfo.InvariantCulture);
                dtFinalDataTable.Rows.Add(currentRow);




                //wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);
                //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, (strSpreadsheetPrefixName + strFileDate).Substring(0, 31), 2, 3);

                intFileCnt++;
            }


            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
            dtFinalDataTable.TableName = "EviCore_Files";
            DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
            DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE dbo." + dtFinalDataTable.TableName + ";");
            DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);
        }


    }

    public struct SheetRows
    {
        public string strSheetName { get; set; }

        public string strCallTaker { get; set; }
        public Int16? intTotalCallsRow { get; set; }
        public Int16? intAvgSpeedAnswerRow { get; set; }
        public Int16? intAbandonedCallsRow { get; set; }
        public Int16? intAbandonedPercentRow { get; set; }
        public Int16? intAverageTalkTimeRow { get; set; }
        public Int16? intASAinSLPerentRow { get; set; }
    }
}
