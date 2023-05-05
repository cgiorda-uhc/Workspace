using DocumentFormat.OpenXml.Packaging;
using ExtensionMethods;
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

namespace CS_Scorecard_Parser
{
    class CS_Scorecard_Parser
    {
        private static EventLog _eventLog;
        private static int _eventId;
        static void Main(string[] args)
        {

            /*
             
             Location:
                \\MSP09Fil01.uhc.com\radiology\Community & State\Reports\From eviCore\Monthly
                UHC_Community_Plan_STATE_RAD_Monthly_Reports_2022_05.zip

                Americechoice_AZ_Auth_Details = "ALL"
                Americechoice_AZ_Rad_Auth_Modality = "REST"

                MISC 

                CaseInt - filter phone/fax/email sum rows()

                Names:
             
             
             */


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

            blUpdated = getScorecardData();
            return;

            try
            {
                blUpdated = getScorecardData();
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
        }



        private static bool getScorecardData()
        {
            //HelperFunctions.HelperFunctions.Email("laura_fischer@uhc.com; j.turbacuski@uhc.com;renee_l_struck@uhc.com", "chris_giordano@uhc.com", "UCS Automation Manager: eviCore C&S Scorecared Parser", "C&S RAD and CARD data was processed for 8-2022. Please note duplictes were found, flagged and will be excluded. Type=RAD State=WA, Modality=ULTRASOUND, FileDate=08-2022. The file has been attached", "jon_maguire@uhc.com;chris_giordano@uhc.com", null, System.Net.Mail.MailPriority.High);





            bool blUpdated = false;

            Console.WriteLine("EviCore Parser");
            string strMainPath = ConfigurationManager.AppSettings["Main_Path"];
            string strWorkingPath = ConfigurationManager.AppSettings["Working_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];


            int intFileCnt = 1;


            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");


            string strFileName = null;
            string strFilePath = null;
            string strMonth = null;
            string strYear = null;
            string strSheetname = null;
            string[] strFileNameArr;
            string strNewPath;

            //PREVIOUS WAS FULL YEAR NOW ITS MONTHLY!
            int intStartYearForMonthly = 2022;
            int intStartMonthForMonthly = 12;

            bool blFoundFile = false;

            DataTable dtFilesCaptured = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_CS_Scorecard]");

            DataTable dtDetails = null;
            DataTable dtSummary = null;
            DataTable dtTmp = null;
            DataTable dtFinalDataTable = null;

            DataRow finalDTRow;

            List<string> strLstModalities = null;
            List<string> strLstStates = null;

            //RESET FINAL TABLE!!!
            dtFinalDataTable = new DataTable();

            dtFinalDataTable.Columns.Add("State", typeof(String));
            dtFinalDataTable.Columns.Add("Modality", typeof(String));
            dtFinalDataTable.Columns.Add("Phone", typeof(int));
            dtFinalDataTable.Columns.Add("Web", typeof(int));
            dtFinalDataTable.Columns.Add("Fax", typeof(int));
            dtFinalDataTable.Columns.Add("RequestsPer1000", typeof(double));
            dtFinalDataTable.Columns.Add("ApprovalsPer1000", typeof(double));
            dtFinalDataTable.Columns.Add("Approved", typeof(int));
            dtFinalDataTable.Columns.Add("Auto_Approved", typeof(int));
            dtFinalDataTable.Columns.Add("Denied", typeof(int));
            dtFinalDataTable.Columns.Add("Withdrawn", typeof(int));
            dtFinalDataTable.Columns.Add("Expired", typeof(int));
            dtFinalDataTable.Columns.Add("Others", typeof(int));
            dtFinalDataTable.Columns.Add("Routine_Cases", typeof(int));
            dtFinalDataTable.Columns.Add("Compliant_Routine_Cases", typeof(int));
            dtFinalDataTable.Columns.Add("Urgent_Cases", typeof(int));
            dtFinalDataTable.Columns.Add("Compliant_Urgent_Cases", typeof(int));
            dtFinalDataTable.Columns.Add("file_month", typeof(String));
            dtFinalDataTable.Columns.Add("file_year", typeof(String));
            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            dtFinalDataTable.Columns.Add("file_name", typeof(String));
            dtFinalDataTable.Columns.Add("is_ignored", typeof(bool));
            dtFinalDataTable.Columns.Add("ignore_reason", typeof(String));

            intFileCnt = 1;


            //COLLECT ALL ZIP FILES FROM SERVER AND UNZIP AND ARCHIVE IN ADVANCE
            string[] files;
            if (1 ==2)
            {
                //files = Directory.GetFiles(strMainPath, "UHC_Community_Plan*MISC*.zip", SearchOption.AllDirectories);
                files = new string[] { strMainPath + "\\UHC_Community_Plan_CARD_MISC_Monthly_Reports_2022_12.zip", strMainPath + "\\UHC_Community_Plan_RAD_MISC_Monthly_Reports_2022_12.zip" };

                foreach (string strFile in files)
                {
                    strFileName = Path.GetFileName(strFile);

                    strFileNameArr = strFileName.Replace(".zip", "").Split('_');
                    strYear = strFileNameArr[strFileNameArr.Length - 2];
                    strMonth = strFileNameArr[strFileNameArr.Length - 1];


                    strSheetname = (strFileName.ToLower().Contains("_rad_") ? "rad" : "card");
                    


                    if (!int.TryParse(strYear, out int intFolderNum) || intFolderNum < intStartYearForMonthly)
                    {
                        intFileCnt++;
                        continue;
                    }




                    if ((strFileName.ToLower().Contains("_rad_") && strSheetname.Equals("card")) || (strFileName.ToLower().Contains("_card_") && strSheetname.Equals("rad")))
                    {
                        intFileCnt++;
                        continue;
                    }

                    strNewPath = strSheetname + "\\" + strYear + "\\" + strMonth;


                    if (!Directory.Exists(strWorkingPath + "\\" + strNewPath))
                    {
                        Directory.CreateDirectory(strWorkingPath + "\\" + strNewPath + "\\");
                    }

                    //CHECK IF ALREADY PROCESSED
                    if (!File.Exists(strWorkingPath + "\\Archive\\" + strFileName))
                    {
                        //COPY ZIP FROM SERVER
                        File.Copy(strFile, strWorkingPath + "\\" + strNewPath + "\\" + strFileName);

                        blFoundFile = false;
                        //UNZIP SINGLE FILE WE NEED
                        using (ZipArchive archive = ZipFile.OpenRead(strFile))
                        {
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                string strTmpFile = Path.Combine(strWorkingPath + "\\" + strNewPath, entry.FullName);
                                if (!File.Exists(strTmpFile) && (entry.FullName.ToLower().Equals("americhoice_allstates_auth_details_" + strSheetname + "_" + strYear + "_" + strMonth + ".xls") || entry.FullName.ToLower().Equals("americhoice_allstates_auth_details_" + strSheetname + "_" + strYear + "_" + strMonth + ".xlsx") || entry.FullName.ToLower().Equals("americhoice_allstates_auths per 1000 by modality with exclusions_" + strSheetname + "_" + strYear + "_" + strMonth + ".xlsx") || entry.FullName.ToLower().Equals("americhoice_allstates_auths per 1000 by modality with exclusions_" + strYear + "_" + strMonth + "_"+ strSheetname +".xlsx") || entry.FullName.ToLower().Equals("americhoice_allstates_auths per 1000 by modality with exclusions_" + strSheetname + ".xlsx")))
                                {
                                    blFoundFile = true;
                                    entry.ExtractToFile(strTmpFile);
                                    //break;

                                }

                            }
                        }



                        //ARCHIVE ZIP FILE
                        if (blFoundFile)
                        {
                            File.Move(strWorkingPath + "\\" + strNewPath + "\\" + strFileName, strWorkingPath + "\\Archive\\" + strFileName);
                        }
                        else //DELETE AND TRY SAME ZIP NEXT TIME
                        {
                            File.Delete(strWorkingPath + "\\" + strNewPath + "\\" + strFileName);
                        }


                    }


                }
            }

            //files = Directory.EnumerateFiles(strWorkingPath, "*.xls*", SearchOption.TopDirectoryOnly).Where(s => s.EndsWith(".xls") || s.EndsWith(".xlsx")).ToArray();
            files   = new string[] { strWorkingPath + @"\Card\2023\02\AMERICHOICE_Allstates_Auth_Details_CARD_2023_02.xls", strWorkingPath + @"\Card\2023\02\AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_CARD_2023_02.xlsx", strWorkingPath + @"\rad\2023\02\Americhoice_Allstates_Auth_Details_RAD_2023_02.xls", strWorkingPath + @"\rad\2023\02\AMERICHOICE_Allstates_Auths Per 1000 by Modality with Exclusions_RAD_2023_02.xlsx" };
            //files = Directory.GetFiles(strWorkingPath, "*.xls*", SearchOption.AllDirectories);



            DataTable dtZipState = DBConnection32.getMSSQLDataTable(strILUCAConnectionString, "SELECT [zip],[state] FROM [IL_UCA].[stg].[zip_state]");


            intFileCnt = 1;
            dtDetails = null;
            foreach (string strFile in files)
            {

                strFileName = Path.GetFileName(strFile);
                strFilePath = Path.GetDirectoryName(strFile);

                strFileNameArr = strFilePath.Split('\\');
                strMonth = strFileNameArr[strFileNameArr.Length - 1];
                strYear = strFileNameArr[strFileNameArr.Length - 2];

                bool isNum = int.TryParse(strYear, out int intYearNum);
                bool isNum2 = int.TryParse(strMonth, out int intMonthNum);
                if (!isNum || !isNum2 || intYearNum < intStartYearForMonthly || (intYearNum == intStartYearForMonthly && intMonthNum < intStartMonthForMonthly))
                {
                    intFileCnt++;
                    continue;
                }

                if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }

                if (strFileName.ToLower().Contains("_card"))
                {
                    strSheetname = "CARD";
                }
                else
                {
                    strSheetname = "RAD";
                }


                Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");



                //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////
                //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////

                Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);


                if (strFileName.ToLower().StartsWith("americhoice_allstates_auth_details_"))
                {

                    //dtDetails = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "All States " + strSheetname, strStart: "A3:BM", strColumns: "[Episode ID],[Case Init],[Modality], [Site State]");
                    //dtDetails = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "All States " + strSheetname, strStart: "A4:AL", strColumns: "[Encounter ID],[Case Init],[Modality], [Site State]");
                    //dtDetails = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "All States " + strSheetname, strStart: "A4:AL", strColumns: "[Encounter ID],[Case Init],[Modality], [Site Zip Code]");
                    dtDetails = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "Allstates " + strSheetname, strStart: "A4:AL", strColumns: "[Encounter ID],[Case Init],[Modality], [Site Zip Code]");
                    dtDetails.Rows.RemoveAt(dtDetails.Rows.Count - 1);

                    dtDetails.Columns.Add("Site State", typeof(System.String));

                    foreach (DataRow row in dtDetails.Rows)
                    {
                        //need to set value to NewColumn column
                        var dr = dtZipState.Select("zip = '" + row["Site Zip Code"] + "'");
                        if(dr.Count() > 0)
                            row["Site State"] = dr.FirstOrDefault()["state"];   // or set it to some other value
                    }





                }
                else if (strFileName.ToLower().StartsWith("americhoice_allstates_auths per 1000 by modality with exclusions_"))
                {

                    try
                    {
                        dtSummary = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "ALL LOBs", strStart: "B5:AK", strColumns: "[State],[Modality],[Approved (A)], [Denied (D)],[Withdrawn (W)],[Expired (Y)],[Pending],[Requests   / 1000],[Approved       / 1000]" + (strSheetname == "CARD" ? ",[Auto Approved]" : ""));
                    }
                    catch
                    {
                        //dtSummary = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "ALL LOBs", strStart: "B5:AC", strColumns: "[State],[Modality],[Approved (A)], [Denied (D)],[Withdrawn (W)],[Expired (Y)],[Pending],[Requests   / 1000],[Approved       / 1000]" + (strSheetname == "CARD" ? ",[Auto Approved]" : ""));
                    }




                   // dtSummary.Delete("[Modality] = 'TOTAL'");

 
                    var strLastState = "";
                    foreach (DataRow dr in dtSummary.Rows)
                    {
                        if (!string.IsNullOrEmpty(dr["State"] + ""))
                        {
                            //CHANGE MODALITY!
                            if (strLastState != dr["State"].ToString())
                                strLastState = dr["State"].ToString();
                        }
                        else
                        {
                            dr["State"] = strLastState;
                        }

                    }


                }
                else
                {
                    throw new Exception("Unexpected xlsx file!!!");
                }




                if (dtDetails != null && dtSummary != null )
                {
                    for (int i = dtDetails.Rows.Count - 1; i >= 0; i--)
                    {
                        // whatever your criteria is
                        if (dtDetails.Rows[i]["Site State"].ToString() == "")
                            dtDetails.Rows[i].Delete();
                    }



                    strLstModalities = dtSummary.AsEnumerable().Select(x => x["Modality"].ToString()).Distinct().ToList();
                    strLstModalities.Insert(0, "ALL");

                    strLstStates = dtSummary.AsEnumerable().Select(x => x["State"].ToString()).Distinct().ToList();


                    foreach (string sCol in strLstStates)
                    {
                        
                        foreach (string sMod in strLstModalities)
                        {
                           
                            finalDTRow = dtFinalDataTable.NewRow();
                            finalDTRow["Modality"] = sMod;
                            finalDTRow["State"] = sCol;

                            if(sMod == "ALL")
                            {
                                //GET MODALITY TOTALS FOR CURRENT STATE!
                                //dtDetails = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "All States " + strSheetname, strStart: "A3:BM", strColumns: "[Episode ID],[Case Init],[Modality], [Site State]");

                                finalDTRow["Phone"] = dtDetails.Select("[Case Init] = 'Phone' AND [Site State] = '" + sCol + "'").Count();
                                finalDTRow["Web"] = dtDetails.Select("[Case Init] = 'Web' AND [Site State] = '" + sCol + "'").Count();
                                finalDTRow["Fax"] = dtDetails.Select("[Case Init] = 'Fax' AND [Site State] = '" + sCol + "'").Count();

                                //dtSummary = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, "ALL LOBs", strStart: "B5:AC", strColumns: "[State],[Modality],[Approved (A)], [Denied (D)],[Withdrawn (W)],[Expired (Y)],[Pending],[Requests   / 1000],[Approved       / 1000]" + (strSheetname == "CARD" ? ",[Auto Approved]" : ""));

                                dtTmp = dtSummary.Select("[State] = '" + sCol + "'").CopyToDataTable();

                                finalDTRow["RequestsPer1000"]  = (double)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Requests   / 1000"));
                                finalDTRow["ApprovalsPer1000"] = (double)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Approved       / 1000"));

                                finalDTRow["Approved"] = (int)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Approved (A)"));

                                //IF CARD
                                if(strSheetname == "CARD")
                                    finalDTRow["Auto_Approved"] = (int)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Auto Approved"));

                                finalDTRow["Denied"] = (int)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Denied (D)"));
                                finalDTRow["Withdrawn"] = (int)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Withdrawn (W)"));
                                finalDTRow["Expired"] = (int)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Expired (Y)"));
                                finalDTRow["Others"] = (int)dtTmp.AsEnumerable().Sum(row => row.Field<Decimal>("Pending"));

                            }
                            else
                            {

                                if(dtSummary.Select("[State] = '" + sCol + "' AND [Modality] = '" + sMod + "'").Count() > 0)
                                {
                                    dtTmp = dtSummary.Select("[State] = '" + sCol + "' AND [Modality] = '" + sMod + "'").CopyToDataTable();

                                    if (dtTmp.Rows.Count > 1)
                                    {
                                        foreach(DataRow dr in dtTmp.Rows)
                                        {

                                            finalDTRow = dtFinalDataTable.NewRow();
                                            finalDTRow["Modality"] = sMod;
                                            finalDTRow["State"] = sCol;


                                            finalDTRow["Approved"] = (int)Decimal.Parse(dr["Approved (A)"].ToString());

                                            //IF CARD
                                            if (strSheetname == "CARD")
                                                finalDTRow["Auto_Approved"] = (int)Decimal.Parse(dr["Auto Approved"].ToString());

                                            finalDTRow["Denied"] = (int)Decimal.Parse(dr["Denied (D)"].ToString());
                                            finalDTRow["Withdrawn"] = (int)Decimal.Parse(dr["Withdrawn (W)"].ToString());
                                            finalDTRow["Expired"] = (int)Decimal.Parse(dr["Expired (Y)"].ToString());
                                            finalDTRow["Others"] = (int)Decimal.Parse(dr["Pending"].ToString());


                                            finalDTRow["file_month"] = strMonth;
                                            finalDTRow["file_year"] = strYear;
                                            finalDTRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                                            finalDTRow["sheet_name"] = strSheetname;
                                            finalDTRow["file_name"] = strFileName;
                                            finalDTRow["is_ignored"] = true;
                                            finalDTRow["ignore_reason"] = "Duplicate Row";
                                            dtFinalDataTable.Rows.Add(finalDTRow);
                                        }

                                        continue;
                                    }
                                    else
                                    {
                                        finalDTRow["Approved"] = (int)Decimal.Parse(dtTmp.Rows[0]["Approved (A)"].ToString());

                                        //IF CARD
                                        if (strSheetname == "CARD")
                                            finalDTRow["Auto_Approved"] = (int)Decimal.Parse(dtTmp.Rows[0]["Auto Approved"].ToString());

                                        finalDTRow["Denied"] = (int)Decimal.Parse(dtTmp.Rows[0]["Denied (D)"].ToString());
                                        finalDTRow["Withdrawn"] = (int)Decimal.Parse(dtTmp.Rows[0]["Withdrawn (W)"].ToString());
                                        finalDTRow["Expired"] = (int)Decimal.Parse(dtTmp.Rows[0]["Expired (Y)"].ToString());
                                        finalDTRow["Others"] = (int)Decimal.Parse(dtTmp.Rows[0]["Pending"].ToString());
                                    }
                                        
                                    

                                   
                                }
                                else
                                {
                                   
                                    finalDTRow["Approved"] = (int)0;

                                    //IF CARD
                                    if (strSheetname == "CARD")
                                        finalDTRow["Auto_Approved"] = (int)0;

                                    finalDTRow["Denied"] = (int)0;
                                    finalDTRow["Withdrawn"] = (int)0;
                                    finalDTRow["Expired"] = (int)0;
                                    finalDTRow["Others"] = (int)0;
                                }

                                
                            }


                            finalDTRow["file_month"] = strMonth;
                            finalDTRow["file_year"] = strYear;
                            finalDTRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
                            finalDTRow["sheet_name"] = strSheetname;
                            finalDTRow["file_name"] = strFileName;
                            finalDTRow["is_ignored"] = false;
                            finalDTRow["ignore_reason"] = DBNull.Value;
                            dtFinalDataTable.Rows.Add(finalDTRow);

                        }
                    }

                    dtDetails = null;
                    dtSummary = null;
                    
                }

                intFileCnt++;

            }

            if (dtFinalDataTable.Rows.Count > 0)
            {
                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
                dtFinalDataTable.TableName = "stg.EviCore_CS_Scorecard";
                DBConnection32.handle_SQLRowCopied += OnSqlRowsCopied;
                //DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                DBConnection32.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 500);

                blUpdated = true;

            }

            return blUpdated;
        }






        //private static bool getScorecardDataOLD()
        //{
        //    bool blUpdated = false;

        //    Console.WriteLine("EviCore Parser");
        //    string strMainPath = ConfigurationManager.AppSettings["Main_Path"];
        //    string strWorkingPath = ConfigurationManager.AppSettings["Working_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];


        //    int intFileCnt = 1;


        //    Console.WriteLine();
        //    Console.WriteLine("Processing spreadsheets");
        //    SpreadsheetDocument wbCurrentExcelFile;

        //    DataRow[] drNum;
        //    int intRowNum = 0;

        //    string strFileName = null;
        //    string strFilePath = null;
        //    string strMonth = null;
        //    string strYear = null;
        //    string strSheetname = null;
        //    string[] strDirNameArr;
        //    string strNewPath;

        //    //PREVIOUS WAS FULL YEAR NOW ITS MONTHLY!
        //    int intStartYearForMonthly = 2020;

        //    string strLastModality = null;

        //    bool blFoundFile = false;


        //    DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from [stg].[EviCore_CS_Scorecard]");

        //    DataTable dtCurrent = null;
        //    DataTable dtFinalDataTable = null;
        //    DataRow finalDTRow;
        //    DataRow[] drTestArr;

        //    List<string> strLstModalities = null;
        //    List<string> strLstMeasures = null;
        //    List<string> strLstColumns = null;

        //    //RESET FINAL TABLE!!!
        //    dtFinalDataTable = new DataTable();

        //    dtFinalDataTable.Columns.Add("State", typeof(String));
        //    dtFinalDataTable.Columns.Add("Modality", typeof(String));
        //    dtFinalDataTable.Columns.Add("Phone", typeof(int));
        //    dtFinalDataTable.Columns.Add("Web", typeof(int));
        //    dtFinalDataTable.Columns.Add("Fax", typeof(int));
        //    dtFinalDataTable.Columns.Add("RequestsPer1000", typeof(double));
        //    dtFinalDataTable.Columns.Add("ApprovalsPer1000", typeof(double));
        //    dtFinalDataTable.Columns.Add("Approved", typeof(int));
        //    dtFinalDataTable.Columns.Add("Auto_Approved", typeof(int));
        //    dtFinalDataTable.Columns.Add("Denied", typeof(int));
        //    dtFinalDataTable.Columns.Add("Withdrawn", typeof(int));
        //    dtFinalDataTable.Columns.Add("Expired", typeof(int));
        //    dtFinalDataTable.Columns.Add("Others", typeof(int));
        //    dtFinalDataTable.Columns.Add("Routine_Cases", typeof(int));
        //    dtFinalDataTable.Columns.Add("Compliant_Routine_Cases", typeof(int));
        //    dtFinalDataTable.Columns.Add("Urgent_Cases", typeof(int));
        //    dtFinalDataTable.Columns.Add("Compliant_Urgent_Cases", typeof(int));
        //    dtFinalDataTable.Columns.Add("file_month", typeof(String));
        //    dtFinalDataTable.Columns.Add("file_year", typeof(String));
        //    dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
        //    dtFinalDataTable.Columns.Add("file_name", typeof(String));

        //    intFileCnt = 1;


        //    //COLLECT ALL ZIP FILES FROM SERVER AND UNZIP AND ARCHIVE IN ADVANCE
        //    string[] files;
        //    if (1 == 1)
        //    {
        //        files = Directory.GetFiles(strMainPath, "UHC_Community_Plan*MISC*.zip", SearchOption.AllDirectories);

        //        foreach (string strFile in files)
        //        {
        //            strFileName = Path.GetFileName(strFile);
        //            strDirNameArr = strFile.Split('\\');
        //            strYear = strDirNameArr[strDirNameArr.Length - 3];

        //            if (!int.TryParse(strYear, out int intFolderNum) || intFolderNum < intStartYearForMonthly)
        //            {
        //                intFileCnt++;
        //                continue;
        //            }

        //            if((strFileName.ToLower().Contains("_rad_") && strDirNameArr[strDirNameArr.Length - 4].ToLower().Equals("card"))  || (strFileName.ToLower().Contains("_card_") && strDirNameArr[strDirNameArr.Length - 4].ToLower().Equals("rad")))
        //            {
        //                intFileCnt++;
        //                continue;
        //            }

        //            strNewPath = strDirNameArr[strDirNameArr.Length - 4] + "\\" + strDirNameArr[strDirNameArr.Length - 3] + "\\" + strDirNameArr[strDirNameArr.Length - 2];


        //            if (!Directory.Exists(strWorkingPath + "\\" + strNewPath))
        //            {
        //                Directory.CreateDirectory(strWorkingPath + "\\" + strNewPath + "\\");
        //            }

        //            //CHECK IF ALREADY PROCESSED
        //            if (!File.Exists(strWorkingPath + "\\Archive\\" + strFileName))
        //            {
        //                //COPY ZIP FROM SERVER
        //                File.Copy(strFile, strWorkingPath + "\\" + strNewPath + "\\" + strFileName);

        //                blFoundFile = false;
        //                //UNZIP SINGLE FILE WE NEED
        //                using (ZipArchive archive = ZipFile.OpenRead(strFile))
        //                {
        //                    foreach (ZipArchiveEntry entry in archive.Entries)
        //                    {
        //                        string strTmpFile = Path.Combine(strWorkingPath + "\\" + strNewPath, entry.FullName);
        //                        if (!File.Exists(strTmpFile) && entry.FullName.ToLower().StartsWith("cs_scorec"))
        //                        {
        //                            blFoundFile = true;
        //                            entry.ExtractToFile(strTmpFile);
        //                            break;

        //                        }

        //                    }
        //                }
        //                //ARCHIVE ZIP FILE
        //                if (blFoundFile)
        //                {
        //                    File.Move(strWorkingPath + "\\" + strNewPath + "\\" + strFileName, strWorkingPath + "\\Archive\\" + strFileName);
        //                }
        //                else //DELETE AND TRY SAME ZIP NEXT TIME
        //                {
        //                    File.Delete(strWorkingPath + "\\" + strNewPath + "\\" + strFileName);
        //                }


        //            }


        //        }
        //    }


        //    files = Directory.GetFiles(strWorkingPath   , "*.xlsx", SearchOption.AllDirectories);


        //    intFileCnt = 1;

        //    foreach (string strFile in files)
        //    {

        //        strFileName = Path.GetFileName(strFile);
        //        strFilePath = Path.GetDirectoryName(strFile);

        //        strDirNameArr = strFilePath.Split('\\');
        //        strMonth = strDirNameArr[strDirNameArr.Length - 1];
        //        strYear = strDirNameArr[strDirNameArr.Length - 2];

        //        bool isNum = int.TryParse(strYear, out int intFolderNum);
        //        if (!isNum || intFolderNum < intStartYearForMonthly)
        //        {
        //            intFileCnt++;
        //            continue;
        //        }

        //        if (dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
        //        {
        //            intFileCnt++;
        //            continue;
        //        }

        //        if(strFileName.ToLower().Contains("_rad_"))
        //        {
        //            strSheetname = "RAD";
        //        }
        //        else
        //        {
        //            strSheetname = "CARD";
        //        }


        //        Console.Write("\rProcessing " + intFileCnt + " out of " + String.Format("{0:n0}", files.Count()) + " spreadsheets");

        //        wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, false);

        //        //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////
        //        //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////
        //        //////////////////////////////////////////////////////Radiology////////////////////////////////////////////////////////

        //        Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + strSheetname);
        //        dtCurrent = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, blSheetsWild: true);
        //        if(dtCurrent.Rows.Count == 0)
        //        {
        //            dtCurrent = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 2, blSheetsWild:true);
        //        }

        //        //ADD MODALITIES TO ALL ROWS NOT BLANKS!!!!!!!
        //        //ADD MODALITIES TO ALL ROWS NOT BLANKS!!!!!!!
        //        foreach (DataRow dr in dtCurrent.Rows)
        //        {
        //            if (!string.IsNullOrEmpty(dr["Column1"] + ""))
        //            {
        //                //CHANGE MODALITY!
        //                if (strLastModality != dr["Column1"].ToString())
        //                    strLastModality = dr["Column1"].ToString();
        //            }
        //            else
        //            {
        //                dr["Column1"] = strLastModality;
        //            }

        //        }


        //        strLstModalities = dtCurrent.AsEnumerable().Select(x => x["Column1"].ToString()).Distinct().ToList();
        //        strLstMeasures = dtCurrent.AsEnumerable().Select(x => x["Column2"].ToString()).Distinct().ToList();
        //        strLstColumns = dtCurrent.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();



        //        foreach (string sCols in strLstColumns)
        //        {

        //            if (sCols.ToLower() == "column1" || sCols.ToLower() == "column2") //IGNORE THESE COLUMNS
        //                continue;


        //            foreach (string sMods in strLstModalities)
        //            {
        //                if (string.IsNullOrEmpty(sMods)) //IGNORE THESE COLUMNS
        //                    continue;



        //                finalDTRow = dtFinalDataTable.NewRow();
        //                finalDTRow["Modality"] = sMods;
        //                finalDTRow["State"] = sCols;

        //                foreach (string sMes in strLstMeasures)
        //                {
        //                    if (string.IsNullOrEmpty(sMes)) //IGNORE THESE MEASURES
        //                        continue;

        //                    drTestArr = dtCurrent.Select("Column1 ='" + sMods + "' AND Column2 = '" + sMes + "'");
        //                    if (drTestArr.Length > 0)
        //                        finalDTRow[sMes.Replace(" ", "_")] = drTestArr.FirstOrDefault()[sCols];
        //                    else
        //                        finalDTRow[sMes.Replace(" ", "_")] = (object)DBNull.Value;
        //                }



        //                finalDTRow["file_month"] = strMonth;
        //                finalDTRow["file_year"] = strYear;
        //                finalDTRow["file_date"] = DateTime.Parse(strMonth + "/" + "01/" + strYear);
        //                finalDTRow["sheet_name"] = strSheetname;
        //                finalDTRow["file_name"] = strFileName;
        //                dtFinalDataTable.Rows.Add(finalDTRow);
        //            }


        //        }

        //        intFileCnt++;

        //    }

        //    if (dtFinalDataTable.Rows.Count > 0)
        //    {
        //        strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";
        //        dtFinalDataTable.TableName = "stg.EviCore_CS_Scorecard";
        //        DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //        //DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //        DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 500);

        //        blUpdated = true;

        //    }

        //    return blUpdated;
        //}

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }


        //        select[Modality], ratio = col,
        //  AZ,FL,LA,MD,MS,NJ,NY,OH,PA,RI,TN,TX,WA,WI,MO,VA,CA,KY,NC
        //from
        //(
        //  select[Modality], [State], col, value
        //  FROM (SELECT* FROM [IL_UCA].[stg].[EviCore_CS_Scorecard]  WHERE[file_year] = '2022' and[file_month] = '01' and[sheet_name] = 'RAD') tmp
        //cross apply
        //(
        //select '1-Phone', [Phone]  union all select '2-Web', [Web]  union all select '3-Fax', [Fax] union all select '4-RequestsPer1000', [RequestsPer1000]  union all select '5-ApprovalsPer1000', [ApprovalsPer1000]  union all select '6-Approved', [Approved] union all select '7-Denied', [Denied] union all select '8-Withdrawn', [Withdrawn]

        //) c(col, value)
        //) d
        //pivot
        //(
        //  max(value)
        //  for [State] in (AZ, FL, LA, MD, MS, NJ, NY, OH, PA, RI, TN, TX, WA, WI, MO, VA, CA, KY, NC)
        //) piv
        //ORDER BY[Modality],ratio






    }
}
