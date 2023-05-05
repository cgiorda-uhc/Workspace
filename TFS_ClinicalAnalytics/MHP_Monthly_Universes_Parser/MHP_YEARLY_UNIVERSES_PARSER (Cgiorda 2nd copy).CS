using DocumentFormat.OpenXml.Packaging;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Diagnostics;
using System.Text;

namespace MHP_Yearly_Universes_Parser
{
    class MHP_Yearly_Universes_Parser
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
            //getUniversesFinal();
           // return;


            blUpdated = getUGAP_CACHE_FINAL();
            //return;
            //blUpdated = getUGAP_CACHE_FINAL();

            //blUpdated = getUniversesFinal();


            //blUpdated = getUGAP_CACHE3();
            return;



            //getMonthlyUniverses();
            //getMonthlyUniverses();
            //getMonthlyUniverses();
            try
            {
                blUpdated = getUniversesFinal();
                if (blUpdated)
                {
                    if (_eventLog != null)
                        _eventLog.WriteEntry("MHP_Yearly_Universes_Parser getMonthlyUniverses Updated: " + DateTime.Now.ToLongDateString(), EventLogEntryType.Information, _eventId++);
                }
            }
            catch (Exception ex)
            {
                if (_eventLog != null)
                    _eventLog.WriteEntry("MHP_Yearly_Universes_Parser getMonthlyUniverses Error: " + ex.ToString(), EventLogEntryType.Error, _eventId++);
            }
        }


        private static bool getUniversesFinal()
        {

            bool blUpdated = false;

            Console.WriteLine("MHP Monthly Universes Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            int intRowCnt = 1;
            int intFileCnt = 1;
            int intSheetCnt = 1;
            //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
            Console.WriteLine();
            Console.WriteLine("Processing spreadsheets");



            string strFileName = null;
            string strFilePath = @"\\msp09fil01\Radiology\Vendor Management - Delegation Oversight\Mental Health Parity (MHP)\MHP Monthly Universes - 2022";
            strFileFolderPath = @"C:\Users\cgiorda\Desktop\Parsers 2023\MHP";
            string strDate = "01/01/2023";

            string strSheetname = null;
            string strTableName = "stg.MHP_Yearly_Universes_2021_v2";
            strTableName = "stg.MHP_Yearly_Universes_2023_v2";
            string strLandingTableName = "MHP_Yearly_Universes_Landing";
            //string[] files;
            List<string> sheets;
            List<string> files;

            DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "TRUNCATE TABLE stg." + strLandingTableName + ";select distinct [file_name] from " + strTableName);
            DataTable dtCurrentDataTable = null;
            DataTable dtFinalDataTable = null;
            OleDbDataReader dr = null;
            DataRow currentRow;
            List<string> columns= null;
            DateTime temp;
            TimeSpan tempTS;


            int intLimit = 100000;


            bool hasMoreRows = true;



            List<string> strCol = new List<string>();
            strCol.Add("State_of_Issue");
            strCol.Add("State_of_Residence");
            strCol.Add("Enrollee_First_Name");
            strCol.Add("Enrollee_Last_Name");
            strCol.Add("Cardholder_ID");
            strCol.Add("Funding_Arrangement");
            strCol.Add("Authorization");
            strCol.Add("Authorization_Type");
            strCol.Add("Request_Date");
            strCol.Add("Request_Time");
            strCol.Add("Request_Decision");
            strCol.Add("Decision_Date");
            strCol.Add("Decision_Time");
            strCol.Add("Decision_Reason");
            strCol.Add("Extension_Taken");
            strCol.Add("Member_Notif_Extension_Date");
            strCol.Add("Additional_Info_Date");
            strCol.Add("Oral_Notification_Enrollee_Date");
            strCol.Add("Oral_Notification_Enrollee_Time");
            strCol.Add("Oral_Notification_Provider_Date");
            strCol.Add("Oral_Notification_Provider_Time");
            strCol.Add("Written_Notification_Enrollee_Date");
            strCol.Add("Written_Notification_Enrollee_Time");
            strCol.Add("Written_Notification_Provider_Date");
            strCol.Add("Written_Notification_Provider_Time");
            strCol.Add("Primary_Procedure_Code_Req");
            strCol.Add("Procedure_Code_Description");
            strCol.Add("Primary_Diagnosis_Code");
            strCol.Add("Diagnosis_Code_Description");
            strCol.Add("Place_of_Service");
            strCol.Add("Member_Date_of_Birth");
            strCol.Add("Urgent_Processed_Standard");
            strCol.Add("Request_Additional_Info_Date");
            strCol.Add("FirstTier_Downstream_RelatedEntity");
            strCol.Add("Par_NonPar_Site");
            strCol.Add("Inpatient_Outpatient");
            strCol.Add("Delegate_Number");
            strCol.Add("ProgramType");
            strCol.Add("Insurance_Carrier");
            strCol.Add("Group_Number");
            strCol.Add("Intake_Method");

            dtFinalDataTable = new DataTable();
            dtFinalDataTable.Columns.Add("State_of_Issue", typeof(String));
            dtFinalDataTable.Columns.Add("State_of_Residence", typeof(String));
            dtFinalDataTable.Columns.Add("Enrollee_First_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Enrollee_Last_Name", typeof(String));
            dtFinalDataTable.Columns.Add("Cardholder_ID", typeof(String));
            dtFinalDataTable.Columns.Add("Funding_Arrangement", typeof(String));
            dtFinalDataTable.Columns.Add("Authorization", typeof(String));
            dtFinalDataTable.Columns.Add("Authorization_Type", typeof(String));
            dtFinalDataTable.Columns.Add("Request_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Request_Time", typeof(TimeSpan));
            dtFinalDataTable.Columns.Add("Request_Decision", typeof(String));
            dtFinalDataTable.Columns.Add("Decision_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Decision_Time", typeof(TimeSpan));
            dtFinalDataTable.Columns.Add("Decision_Reason", typeof(String));
            dtFinalDataTable.Columns.Add("Extension_Taken", typeof(Boolean));
            dtFinalDataTable.Columns.Add("Member_Notif_Extension_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Additional_Info_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Time", typeof(TimeSpan));
            dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Time", typeof(TimeSpan));
            dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Time", typeof(TimeSpan));
            dtFinalDataTable.Columns.Add("Written_Notification_Provider_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Written_Notification_Provider_Time", typeof(TimeSpan));
            dtFinalDataTable.Columns.Add("Primary_Procedure_Code_Req", typeof(String));
            dtFinalDataTable.Columns.Add("Procedure_Code_Description", typeof(String));
            dtFinalDataTable.Columns.Add("Primary_Diagnosis_Code", typeof(String));
            dtFinalDataTable.Columns.Add("Diagnosis_Code_Description", typeof(String));
            dtFinalDataTable.Columns.Add("Place_of_Service", typeof(Int16));
            dtFinalDataTable.Columns.Add("Member_Date_of_Birth", typeof(DateTime));
            dtFinalDataTable.Columns.Add("Urgent_Processed_Standard", typeof(Boolean));
            dtFinalDataTable.Columns.Add("Request_Additional_Info_Date", typeof(DateTime));
            dtFinalDataTable.Columns.Add("FirstTier_Downstream_RelatedEntity", typeof(String));
            dtFinalDataTable.Columns.Add("Par_NonPar_Site", typeof(String));
            dtFinalDataTable.Columns.Add("Inpatient_Outpatient", typeof(String));
            dtFinalDataTable.Columns.Add("Delegate_Number", typeof(Int32));
            dtFinalDataTable.Columns.Add("ProgramType", typeof(String));
            dtFinalDataTable.Columns.Add("Insurance_Carrier", typeof(String));
            dtFinalDataTable.Columns.Add("Group_Number", typeof(String));
            dtFinalDataTable.Columns.Add("Intake_Method", typeof(String));
            //dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
            //dtFinalDataTable.Columns.Add("file_name", typeof(String));
            //dtFinalDataTable.Columns.Add("file_path", typeof(String));
            //dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

            dtFinalDataTable.TableName = "stg."+strLandingTableName;


            strFileFolderPath = @"C:\Users\cgiorda\Desktop\Projects\MHPUniverse";

            files = Directory.EnumerateFiles(strFileFolderPath, "*.xlsx", SearchOption.TopDirectoryOnly).ToList();
            //files = new List<string>();
            //files.Add(strFileFolderPath + "\\United PCP-Rad & Card_June_2022.xlsx");
            //files.Add(strFileFolderPath + "\\Oxford January -Radiology Cardiology Universe 2023.xlsx");


            // bool blFoundSheet = false;







            intFileCnt = 1;
            foreach (string strFile in files)
            {
                Console.Write("Processing :" + intFileCnt + " out of " + files.Count + " : " + strFile);


                Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");

                strFileName = Path.GetFileName(strFile);

                if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
                {
                    intFileCnt++;
                    continue;
                }




                intSheetCnt = 1;

                var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
                intSheetCnt = 1;
                foreach (Sheet item in results)
                {
                    columns = null;


                    strSheetname = item.Name.ToString();

                    Console.Write("\rProcessing " + String.Format("{0:n0}", intSheetCnt) + " out of " + String.Format("{0:n0}", results.Count()) + " spreadsheets");



                    intRowCnt = 1;
                    //while (hasMoreRows)
                    //{
                        try
                        {
                            //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
                            //dr = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataReader(strFile, strSheetname, strStart : "A" + intRowCnt.ToString() + ":AI" ,  limit : intLimit.ToString());
                        dr = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataReader(strFile, strSheetname);
                    }
                        catch (Exception ex)
                        {
                            SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
                            dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
                        }


                        if(columns == null)
                        {
                            columns = new List<string>();
                            for (int i = 0; i < dr.FieldCount; i++)
                            {
                                columns.Add(dr.GetName(i));
                            }
                        }


                        //columns = dtCurrentDataTable.Columns;
                        Console.Write("\rFile to DataTable");
                        // strSummaryofLOB = strFolder.Split('_')[1];

           
                        while (dr.Read())
                        {

                            if (dr.GetValue(dr.GetOrdinal("Authorization")) == DBNull.Value)
                                continue;


                            Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of LOTS of rows");

                            currentRow = dtFinalDataTable.NewRow();

                            if (columns.Contains("State of Issue"))
                                currentRow["State_of_Issue"] = (dr.GetValue(dr.GetOrdinal("State of Issue")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("State of Issue")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("State of Issue")) : (object)DBNull.Value);

                            if (columns.Contains("State of Residence"))
                                currentRow["State_of_Residence"] = (dr.GetValue(dr.GetOrdinal("State of Residence")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("State of Residence")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("State of Residence")) : (object)DBNull.Value);

                            if (columns.Contains("Enrollee First Name"))
                                currentRow["Enrollee_First_Name"] = (dr.GetValue(dr.GetOrdinal("Enrollee First Name")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Enrollee First Name")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Enrollee First Name")) : (object)DBNull.Value);

                            if (columns.Contains("Enrollee Last Name"))
                                currentRow["Enrollee_Last_Name"] = (dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) : (object)DBNull.Value);

                            if (columns.Contains("Cardholder ID"))
                                currentRow["Cardholder_ID"] = (dr.GetValue(dr.GetOrdinal("Cardholder ID")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Cardholder ID")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Cardholder ID")) : (object)DBNull.Value);

                            if (columns.Contains("Funding Arrangement"))
                                currentRow["Funding_Arrangement"] = (dr.GetValue(dr.GetOrdinal("Funding Arrangement")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Funding Arrangement")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Funding Arrangement")) : (object)DBNull.Value);

                            if (columns.Contains("Authorization"))
                                currentRow["Authorization"] = (dr.GetValue(dr.GetOrdinal("Authorization")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Authorization")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Authorization")) : (object)DBNull.Value);

                            if (columns.Contains("Authorization Type"))
                                currentRow["Authorization_Type"] = (dr.GetValue(dr.GetOrdinal("Authorization Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Authorization Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Authorization Type")) : (object)DBNull.Value);

                            if (columns.Contains("Date the request was received"))
                                currentRow["Request_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date the request was received")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date the request was received")) : (object)DBNull.Value);

                            if (columns.Contains("Time the request was received"))
                                currentRow["Request_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time the request was received")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time the request was received")) : (object)DBNull.Value);

                            if (columns.Contains("Request decision") || columns.Contains("Request Decision"))
                                currentRow["Request_Decision"] = (dr.GetValue(dr.GetOrdinal("Request decision")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Request decision")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Request decision")) : (object)DBNull.Value);

                            if (columns.Contains("Date of decision") || columns.Contains("Date of Decision"))
                                currentRow["Decision_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of decision")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of decision")) : (object)DBNull.Value);

                            if (columns.Contains("Time of decision") || columns.Contains("Time of Decision"))
                                currentRow["Decision_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time of decision")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time of decision")) : (object)DBNull.Value);

                            if (columns.Contains("Decision Reason") || columns.Contains("Decision reason"))
                                currentRow["Decision_Reason"] = (dr.GetValue(dr.GetOrdinal("Decision Reason")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Decision Reason")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Decision Reason")) : (object)DBNull.Value);
                            else if (columns.Contains("Denial Type") || columns.Contains("Denial type"))
                                currentRow["Decision_Reason"] = (dr.GetValue(dr.GetOrdinal("Denial Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Denial Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Denial Type")) : (object)DBNull.Value);

                            if (columns.Contains("Was Extension Taken ? "))
                                currentRow["Extension_Taken"] = (!(dr.GetValue(dr.GetOrdinal("Was Extension Taken?")) + "").Trim().Equals("") ? ((dr.GetValue(dr.GetOrdinal("Was Extension Taken?")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);
                            else if (columns.Contains("Was Extension Taken"))
                                currentRow["Extension_Taken"] = (!(dr.GetValue(dr.GetOrdinal("Was Extension Taken")) + "").Trim().Equals("") ? ((dr.GetValue(dr.GetOrdinal("Was Extension Taken")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

                            if (columns.Contains("Date of member notification of extension"))
                                currentRow["Member_Notif_Extension_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of member notification of extension")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of member notification of extension")) : (object)DBNull.Value);

                            if (columns.Contains("Date additional information received"))
                                currentRow["Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date additional information received")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date additional information received")) : (object)DBNull.Value);

                            if (columns.Contains("Date oral notification provided to enrollee"))
                                currentRow["Oral_Notification_Enrollee_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date oral notification provided to enrollee")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date oral notification provided to enrollee")) : (object)DBNull.Value);

                            if (columns.Contains("Time oral notification provided to enrollee"))
                                currentRow["Oral_Notification_Enrollee_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time oral notification provided to enrollee")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time oral notification provided to enrollee")) : (object)DBNull.Value);

                            if (columns.Contains("Date oral notification provided to provider"))
                                currentRow["Oral_Notification_Provider_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date oral notification provided to provider")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date oral notification provided to provider")) : (object)DBNull.Value);

                            if (columns.Contains("Time oral notification provided to provider"))
                                currentRow["Oral_Notification_Provider_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time oral notification provided to provider")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time oral notification provided to provider")) : (object)DBNull.Value);

                            if (columns.Contains("Date written notification sent to enrollee"))
                                currentRow["Written_Notification_Enrollee_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date written notification sent to enrollee")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date written notification sent to enrollee")) : (object)DBNull.Value);

                            if (columns.Contains("Time written notification sent to enrollee"))
                                currentRow["Written_Notification_Enrollee_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time written notification sent to enrollee")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time written notification sent to enrollee")) : (object)DBNull.Value);

                            if (columns.Contains("Date written notification sent to provider"))
                                currentRow["Written_Notification_Provider_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date written notification sent to provider")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date written notification sent to provider")) : (object)DBNull.Value);

                            if (columns.Contains("Time written notification sent to provider"))
                                currentRow["Written_Notification_Provider_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time written notification sent to provider")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time written notification sent to provider")) : (object)DBNull.Value);

                            if (columns.Contains("Primary Procedure Code(s) Requested"))
                                currentRow["Primary_Procedure_Code_Req"] = (dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) : (object)DBNull.Value);


                            if (columns.Contains("Primary Procedure Code Requested"))
                                currentRow["Primary_Procedure_Code_Req"] = (dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) : (object)DBNull.Value);

                            if (columns.Contains("Procedure Code Description"))
                                currentRow["Procedure_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Procedure Code Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Procedure Code Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Procedure Code Description")) : (object)DBNull.Value);

                            if (columns.Contains("Primary Diagnosis Code"))
                                currentRow["Primary_Diagnosis_Code"] = (dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) : (object)DBNull.Value);

                            if (columns.Contains("Diagnosis Description"))
                                currentRow["Diagnosis_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Diagnosis Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Diagnosis Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Diagnosis Description")) : (object)DBNull.Value);
                            else if (columns.Contains("Diagnosis Code Description"))
                                currentRow["Diagnosis_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) : (object)DBNull.Value);

                            if (columns.Contains("Place of Service"))
                                currentRow["Place_of_Service"] = (dr.GetValue(dr.GetOrdinal("Place of Service")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Place of Service")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Place of Service")) : (object)DBNull.Value);

                            if (columns.Contains("Member Date of Birth"))
                                currentRow["Member_Date_of_Birth"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Member Date of Birth")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Member Date of Birth")) : (object)DBNull.Value);

                            if (columns.Contains("Was an urgent request made but processed as standard?"))
                                currentRow["Urgent_Processed_Standard"] = (!(dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("") && !(dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("NA") ? ((dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

                            if (columns.Contains("Date of request for additional information"))
                                currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of request for additional information")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of request for additional information")) : (object)DBNull.Value);
                            else if (columns.Contains("Date additional information requested"))
                                currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date additional information requested")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date additional information requested")) : (object)DBNull.Value);

                            if (columns.Contains("First Tier, Downstream, and Related Entity"))
                                currentRow["FirstTier_Downstream_RelatedEntity"] = (dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) : (object)DBNull.Value);

                            if (columns.Contains("Delegate Number"))
                                currentRow["Delegate_Number"] = (dr.GetValue(dr.GetOrdinal("Delegate Number")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Delegate Number")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Delegate Number")) : (object)DBNull.Value);

                        if (columns.Contains("Par/Non-Par Site"))
                            currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) : (object)DBNull.Value);
                        else if(columns.Contains("PAR/NON PAR "))
                                currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("PAR/NON PAR ")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("PAR/NON PAR ")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("PAR/NON PAR ")) : (object)DBNull.Value);
                        else if (columns.Contains("Par Non Par Site"))
                            currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par Non Par Site")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par Non Par Site")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par Non Par Site")) : (object)DBNull.Value);
                        else if (columns.Contains("Par Non/ Par Site"))
                            currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par Non/ Par Site")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par Non/ Par Site")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par Non/ Par Site")) : (object)DBNull.Value);
                        else if (columns.Contains("Par-Non-Par Site"))
                            currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par-Non-Par Site")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par-Non-Par Site")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par-Non-Par Site")) : (object)DBNull.Value);
                        else if (columns.Contains("Par/Non-Par"))
                            currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par/Non-Par")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par/Non-Par")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par/Non-Par")) : (object)DBNull.Value);

                        if (columns.Contains("Inpatient/Outpatient"))
                            currentRow["Inpatient_Outpatient"] = (dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) : (object)DBNull.Value);
                        else if(columns.Contains("Inpatient Outpatient"))
                                currentRow["Inpatient_Outpatient"] = (dr.GetValue(dr.GetOrdinal("Inpatient Outpatient")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Inpatient Outpatient")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Inpatient Outpatient")) : (object)DBNull.Value);
                        else if (columns.Contains("Inpatient /Outpatient"))
                            currentRow["Inpatient_Outpatient"] = (dr.GetValue(dr.GetOrdinal("Inpatient /Outpatient")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Inpatient /Outpatient")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Inpatient /Outpatient")) : (object)DBNull.Value);


                        if (columns.Contains("ProgramType"))
                                currentRow["ProgramType"] = (dr.GetValue(dr.GetOrdinal("ProgramType")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("ProgramType")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("ProgramType")) : (object)DBNull.Value);
                        else if (columns.Contains("Program Type"))
                            currentRow["ProgramType"] = (dr.GetValue(dr.GetOrdinal("Program Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Program Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Program Type")) : (object)DBNull.Value);


                        if (columns.Contains("Insurance Carrier"))
                                currentRow["Insurance_Carrier"] = (dr.GetValue(dr.GetOrdinal("Insurance Carrier")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Insurance Carrier")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Insurance Carrier")) : (object)DBNull.Value);
                        else if (columns.Contains("InsCarrier"))
                            currentRow["Insurance_Carrier"] = (dr.GetValue(dr.GetOrdinal("InsCarrier")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("InsCarrier")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("InsCarrier")) : (object)DBNull.Value);
                        else if (columns.Contains("Ins Carrier QA"))
                            currentRow["Insurance_Carrier"] = (dr.GetValue(dr.GetOrdinal("Ins Carrier QA")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Ins Carrier QA")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Ins Carrier QA")) : (object)DBNull.Value);


                        if (columns.Contains("Group Number"))
                                currentRow["Group_Number"] = (dr.GetValue(dr.GetOrdinal("Group Number")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Group Number")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Group Number")) : (object)DBNull.Value);




                        if (columns.Contains("Intake Method"))
                            currentRow["Intake_Method"] = (dr.GetValue(dr.GetOrdinal("Intake Method")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Intake Method")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Intake Method")) : (object)DBNull.Value);
                        else if (columns.Contains("MethodofContactDesc"))
                            currentRow["Intake_Method"] = (dr.GetValue(dr.GetOrdinal("MethodofContactDesc")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("MethodofContactDesc")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("MethodofContactDesc")) : (object)DBNull.Value);

                        //currentRow["file_date"] = DateTime.Parse(strDate);
                        //currentRow["sheet_name"] = strSheetname;
                        //currentRow["file_name"] = strFileName;
                        //currentRow["file_path"] = strFilePath;
                        dtFinalDataTable.Rows.Add(currentRow);
                   

                            if (dtFinalDataTable.Rows.Count == intLimit)
                            {

                                ///dtFinalDataTable = dtFinalDataTable.DefaultView.ToTable(true, strCol.ToArray());


                                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                                blUpdated = true;
                                dtFinalDataTable.Clear();
                            }


                            intRowCnt++;
                        }
                        dr.Close();
                        currentRow = null;

                        if (dtFinalDataTable.Rows.Count > 0)
                        {
                            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

                            DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                            DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

                            blUpdated = true;
                        }
                        dtFinalDataTable.Clear();


                    //}

                    var sql = "INSERT INTO " + strTableName + " ([State_of_Issue] ,[State_of_Residence] ,[Enrollee_First_Name] ,[Enrollee_Last_Name] ,[Cardholder_ID] ,[Funding_Arrangement] ,[Authorization] ,[Authorization_Type] ,[Request_Date] ,[Request_Time] ,[Request_Decision] ,[Decision_Date] ,[Decision_Time] ,[Decision_Reason] ,[Extension_Taken] ,[Member_Notif_Extension_Date] ,[Additional_Info_Date] ,[Oral_Notification_Enrollee_Date] ,[Oral_Notification_Enrollee_Time] ,[Oral_Notification_Provider_Date] ,[Oral_Notification_Provider_Time] ,[Written_Notification_Enrollee_Date] ,[Written_Notification_Enrollee_Time] ,[Written_Notification_Provider_Date] ,[Written_Notification_Provider_Time] ,[Primary_Procedure_Code_Req] ,[Procedure_Code_Description] ,[Primary_Diagnosis_Code] ,[Diagnosis_Code_Description] ,[Place_of_Service] ,[Member_Date_of_Birth] ,[Urgent_Processed_Standard] ,[Request_Additional_Info_Date] ,[FirstTier_Downstream_RelatedEntity] ,[Par_NonPar_Site] ,[Inpatient_Outpatient] ,[Delegate_Number] ,[ProgramType] ,[Insurance_Carrier] ,[Group_Number],[Intake_Method] ,[sheet_name] ,[file_name] ,[file_path] ,[file_date] ) SELECT DISTINCT [State_of_Issue] ,[State_of_Residence] ,[Enrollee_First_Name] ,[Enrollee_Last_Name] ,[Cardholder_ID] ,[Funding_Arrangement] ,[Authorization] ,[Authorization_Type] ,[Request_Date] ,[Request_Time] ,[Request_Decision] ,[Decision_Date] ,[Decision_Time] ,[Decision_Reason] ,[Extension_Taken] ,[Member_Notif_Extension_Date] ,[Additional_Info_Date] ,[Oral_Notification_Enrollee_Date] ,[Oral_Notification_Enrollee_Time] ,[Oral_Notification_Provider_Date] ,[Oral_Notification_Provider_Time] ,[Written_Notification_Enrollee_Date] ,[Written_Notification_Enrollee_Time] ,[Written_Notification_Provider_Date] ,[Written_Notification_Provider_Time] ,[Primary_Procedure_Code_Req] ,[Procedure_Code_Description] ,[Primary_Diagnosis_Code] ,[Diagnosis_Code_Description] ,[Place_of_Service] ,[Member_Date_of_Birth] ,[Urgent_Processed_Standard] ,[Request_Additional_Info_Date] ,[FirstTier_Downstream_RelatedEntity] ,[Par_NonPar_Site] ,[Inpatient_Outpatient] ,[Delegate_Number] ,[ProgramType] ,[Insurance_Carrier] ,[Group_Number],[Intake_Method] ,'" + strSheetname.Replace("'", "''").Trim() + "' ,'" + strFileName.Replace("'", "''").Trim() + "','" + strFilePath.Replace("'", "''").Trim() + "' ,'" + strDate.Replace("'", "''").Trim() + "' FROM [stg].[" + strLandingTableName + "];TRUNCATE TABLE[stg].[" + strLandingTableName + "];";

                    DBConnection64.ExecuteMSSQL(strILUCAConnectionString, sql);



                    //dtFinalDataTable = dtFinalDataTable.DefaultView.ToTable(true, strCol.ToArray());
                    //dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
                    //dtFinalDataTable.Columns.Add("file_name", typeof(String));
                    //dtFinalDataTable.Columns.Add("file_path", typeof(String));
                    //dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

                    //dtFinalDataTable.Columns["file_date"].Expression = strDate;
                    //dtFinalDataTable.Columns["sheet_name"].Expression = strSheetname;
                    //dtFinalDataTable.Columns["file_name"].Expression = strFileName;
                    //dtFinalDataTable.Columns["file_path"].Expression = strFilePath;

                    ////MAKE dtFinalDataTable = dtFinalDataTable.DISTINCT
                    ////THEN ADD TO ALL ROWS
                    ////currentRow["file_date"] = DateTime.Parse(strDate);
                    ////currentRow["sheet_name"] = strSheetname;
                    ////currentRow["file_name"] = strFileName;
                    ////currentRow["file_path"] = strFilePath;








                    intSheetCnt++;

                }






                currentRow = null;
                dtCurrentDataTable = null;
                //File.Move(strFile, @"C:\Users\cgiorda\Desktop\Parsers 2023\MHP\Archive\" + strFileName);
                intFileCnt++;
            }


            return blUpdated;
        }


        private static bool getUGAP_CACHE_FINAL()
        {
            bool blUpdated = false;

            Console.WriteLine("MHP Monthly Universes Parser");
            string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
            string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
            string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
            string strVolatileColumnsDeclare;
            string strVolatileColumns;
            string strVolatileName;
            string strFilterJoin = "";

            StringBuilder sbInserts = new StringBuilder();


            Console.WriteLine("Processing spreadsheets");
            int intCnt = 0;
            int intTotalCnt = 1;
            int intTotal = 0;
            string strUGAPTableName = "MHP_Yearly_Universes_UGAP_2021_v2";
            strUGAPTableName = "MHP_Yearly_Universes_UGAP_2023_v2";

            string strUniverseTable = "MHP_Yearly_Universes_2021_v2";
            strUniverseTable = "MHP_Yearly_Universes_2023_v2";

            //CREATE INDEX indx_mhp_uni_id ON[stg].[MHP_Yearly_Universes_UGAP] (mhp_uni_id);
            //CREATE INDEX indx_LEG_ENTY_NBR ON[stg].[MHP_Yearly_Universes_UGAP] (LEG_ENTY_NBR);
            //CREATE INDEX indx_LEG_ENTY_FULL_NM ON[stg].[MHP_Yearly_Universes_UGAP] (LEG_ENTY_FULL_NM);
            //CREATE INDEX indx_MKT_SEG_RLLP_DESC ON[stg].[MHP_Yearly_Universes_UGAP] (MKT_SEG_RLLP_DESC);
            //CREATE INDEX indx_FINC_ARNG_DESC ON[stg].[MHP_Yearly_Universes_UGAP] (FINC_ARNG_DESC);
            //CREATE INDEX indx_MKT_TYP_DESC ON[stg].[MHP_Yearly_Universes_UGAP] (MKT_TYP_DESC);
            //CREATE INDEX indx_CS_CO_CD_ST ON [stg].[MHP_Yearly_Universes_UGAP] (CS_CO_CD_ST);
            //CREATE INDEX indx_PRDCT_SYS_ID ON[stg].[MHP_Yearly_Universes_UGAP] (PRDCT_SYS_ID);
            //CREATE INDEX indx_CS_PRDCT_CD_SYS_ID ON[stg].[MHP_Yearly_Universes_UGAP] (CS_PRDCT_CD_SYS_ID);
            //CREATE INDEX indx_CS_CO_CD ON[stg].[MHP_Yearly_Universes_UGAP] (CS_CO_CD);
            //CREATE INDEX indx_State_of_Issue ON[stg].[MHP_Yearly_Universes] (State_of_Issue);
            //CREATE INDEX indx_Authorization ON[stg].[MHP_Yearly_Universes] ([Authorization]);
            //CREATE INDEX indx_Request_Decision ON[stg].[MHP_Yearly_Universes] (Request_Decision);
            //CREATE INDEX indx_Request_Date ON[stg].[MHP_Yearly_Universes] (Request_Date);
            //CREATE INDEX indx_Authorization_Type ON[stg].[MHP_Yearly_Universes] (Authorization_Type);
            //CREATE INDEX indx_Decision_Reason ON[stg].[MHP_Yearly_Universes] (Decision_Reason);
            //CREATE INDEX indx_file_name ON[stg].[MHP_Yearly_Universes] (file_name);
            //CREATE INDEX indx_Group_Number ON [stg].[MHP_Yearly_Universes] (Group_Number);
            //  CREATE INDEX indx_sheet_name ON[stg].[MHP_Yearly_Universes] (sheet_name);



            strVolatileColumnsDeclare = "mhp_uni_id BIGINT, Cardholder_ID_CLN  VARCHAR(11), State_Of_Issue VARCHAR(5),BTH_DT DATE, REQ_DT DATE, MBR_FST_NM VARCHAR(25), MBR_LST_NM VARCHAR(25) ";
            strVolatileColumns = "mhp_uni_id, Cardholder_ID_CLN, State_Of_Issue, BTH_DT, REQ_DT, MBR_FST_NM, MBR_LST_NM ";
            strVolatileName = "MissingMembersTmp";


            //UNITED PCP sheet 2 zeros infront cardholdrer
            //UNIVERE PAD 9 
            //



            DataTable dtResults = new DataTable();
            DataTable dtLatestUniverses = new DataTable();


            List<RandomSQL> lstRand = new List<RandomSQL>();


            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.EI + "' ) ", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.MBR_ALT_ID) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt)", strCleaningMethod = "MBR_ALT_ID/BD/RD", lvlLevel = Level.EI });
            lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.EI + "' )  ", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "FN/LN/BD/RD", lvlLevel = Level.EI });
            lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.EI + "' )  ", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "FN%3/LN%3/BD/RD", lvlLevel = Level.EI });


            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.EI_OX + "' )  ", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.MBR_ID) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "MBR_ID/BD/RD", lvlLevel = Level.EI_OX });
            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.EI_OX + "' ) ", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "FN/LN/BD/RD", lvlLevel = Level.EI_OX });
            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.EI_OX + "' ) ", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "FN%3/LN%3/BD/RD", lvlLevel = Level.EI_OX });


            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.IFP + "' ) ", strSQLUGAP = "inner join " + strVolatileName + " as mm on SUBSTR(a.MBR_ID, 0,10) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "MBR_ID/BD/RD", lvlLevel = Level.IFP });
            lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.IFP + "' ) ", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "FN/LN/BD/RD", lvlLevel = Level.IFP });
            lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.IFP + "' ) ", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM)  AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "FN%3/LN%3/BD/RD", lvlLevel = Level.IFP });


            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.CS + "' )", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.MBR_ID) = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "MBR_ID/CS_CO_CD_ST/BD/RD", lvlLevel = Level.CS });
            //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.CS + "' )", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.SBSCR_MEDCD_RCIP_NBR) = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) ", strCleaningMethod = "SBSCR_MEDCD_RCIP_NBR/CS_CO_CD_ST/BD/RD", lvlLevel = Level.CS });
            lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.CS + "' )", strSQLUGAP = "inner join " + strVolatileName + " as mm on upper(a.MBR_FST_NM) = upper(mm.MBR_FST_NM) AND upper(a.MBR_LST_NM) = upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue ", strCleaningMethod = "FN/LN/BD/RD/CS_CO_CD_ST", lvlLevel = Level.CS });
            lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (Classification = '" + Level.CS + "' )", strSQLUGAP = "inner join " + strVolatileName + " as mm upper(a.MBR_FST_NM) LIKE upper(mm.MBR_FST_NM) AND  upper(a.MBR_LST_NM) LIKE upper(mm.MBR_LST_NM) AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt) AND k.CS_CO_CD_ST = mm.State_Of_Issue ", strCleaningMethod = "FN%3/LN%3/BD/RD/CS_CO_CD_ST", lvlLevel = Level.CS });


            foreach (RandomSQL rs in lstRand)
            {
                dtLatestUniverses = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "SELECT mhp_uni_id, REPLACE(SUBSTRING([Cardholder_ID], PATINDEX('%[^0]%', [Cardholder_ID]+'.'), LEN([Cardholder_ID])),[State_of_Issue],'') AS [Cardholder_ID_CLN], State_Of_Issue, CONVERT(char(10), [Member_Date_of_Birth],126) as Member_Date_of_Birth, CONVERT(char(10), [Request_Date], 126) as Request_Date, [Enrollee_First_Name] ,[Enrollee_Last_Name] ,[sheet_name] FROM [IL_UCA].[stg].[" + strUniverseTable + "] WHERE [Member_Date_of_Birth] is not null AND  [Cardholder_ID] IS NOT NULL AND [Request_Date] IS NOT NULL " + rs.strSQLILUCA + " AND mhp_uni_id not in (select mhp_uni_id from [IL_UCA].[stg].[" + strUGAPTableName + "]) ORDER BY mhp_uni_id DESC ");



                //dtLatestUniverses = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "SELECT mhp_uni_id, REPLACE(SUBSTRING([Cardholder_ID], PATINDEX('%[^0]%', [Cardholder_ID]+'.'), LEN([Cardholder_ID])),[State_of_Issue],'') AS [Cardholder_ID_CLN], State_Of_Issue, CONVERT(char(10), [Member_Date_of_Birth],126) as Member_Date_of_Birth, CONVERT(char(10), [Request_Date], 126) as Request_Date, [Enrollee_First_Name] ,[Enrollee_Last_Name] ,[sheet_name] FROM [IL_UCA].[stg].[" + strUniverseTable + "] WHERE [Member_Date_of_Birth] is not null AND  [Cardholder_ID] IS NOT NULL AND [Request_Date] IS NOT NULL " + rs.strSQLILUCA + " AND mhp_uni_id in (7872278,7872277) ORDER BY mhp_uni_id DESC ");



                /*classification = 'CS'  WHERE file_name in ( 'Americhoice  March -Radiology Cardiology Universe 2022.xlsx', 
'Americhoice April -Radiology Cardiology Universe 2022.xlsx',
'Americhoice December Monthly Medicaid Universe 2022.updated.xlsx',
'Americhoice February - Radiology Cardiology Chemo Universe 2022.xlsx',
'Americhoice January  - Radiology Cardiology Chemo  Universe 2022.xlsx',
'Americhoice January- Radiology Cardiology Universe 2023.xlsx')

 AND file_name in ('United PCP-Rad & Card_June_2022.xlsx')



                 classification = 'EI_OX'  WHERE file_name in ( 'Oxford December Monthly Universe 2022.xlsx', 'Oxford November-Radiology Cardiology Universe 2022.xlsx' , 'Oxford October Rad and Card Monthly Universe 2022.xlsx', 'Oxford January -Radiology Cardiology Universe 2023.xlsx', 'Oxford February - Radiology Cardiology Universer 2022.xlsx',
'Oxford January - Radiology Cardiology Universe 2022.xlsx','Oxford March -Radiology Cardiology Universe 2022.xlsx')


                 
                 
                 
                 */


                intTotalCnt = 0;

                string strUGAPSQL = "";


                //SELECT distinct Cardholder_ID FROM[stg].[MHP_Yearly_Universes]  WHERE Cardholder_ID not like '%[^0-9]%' and Cardholder_ID != ''
                intTotal = dtLatestUniverses.Rows.Count;
                foreach (DataRow dr in dtLatestUniverses.Rows)
                {


                    Console.WriteLine("Processing " + intTotalCnt + " out of " + intTotal);

                    var fnw =  (rs.strCleaningMethod != "FN%3/LN%3/BD/RD" ? dr["Enrollee_First_Name"].ToString().Replace("'", "''") : dr["Enrollee_First_Name"].ToString().Substring(0, Math.Min(3, dr["Enrollee_First_Name"].ToString().Length)).Replace("'", "''") + "%");
                    var lnw = (rs.strCleaningMethod != "FN%3/LN%3/BD/RD" ? dr["Enrollee_Last_Name"].ToString().Replace("'", "''") : dr["Enrollee_Last_Name"].ToString().Substring(0, Math.Min(3, dr["Enrollee_Last_Name"].ToString().Length)).Replace("'", "''") + "%");
                    var st = dr["State_Of_Issue"].ToString();
                    var id = dr["mhp_uni_id"].ToString();
                    var cidc =   (rs.lvlLevel != Level.IFP ?  dr["Cardholder_ID_CLN"].ToString() : dr["Cardholder_ID_CLN"].ToString().Substring(0,(dr["Cardholder_ID_CLN"].ToString().Length < 9 ? dr["Cardholder_ID_CLN"].ToString().Length : 9)));
                    var bd = dr["Member_Date_of_Birth"].ToString();
                    var rd = dr["Request_Date"].ToString();
                    sbInserts.Append("INSERT INTO " + strVolatileName + " (" + strVolatileColumns + ") VALUES(" + id + ",'" + cidc + "','" + st + "', '" + bd + "', '" + rd + "', '" + fnw + "', '" + lnw + "'); ");

                    intTotalCnt++;
                    if (intCnt == 3000)
                    {
                        Console.WriteLine("Getting data frome Teradata....");

                        if (rs.lvlLevel == Level.EI || rs.lvlLevel == Level.EI_OX)
                            strUGAPSQL = getUGAPSQLTemplateEI(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP, rs.lvlLevel == Level.EI_OX).Replace("{$Inserts}", sbInserts.ToString());
                        else
                            strUGAPSQL = getUGAPSQLTemplateCS_IFP(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP, rs.lvlLevel == Level.CS).Replace("{$Inserts}", sbInserts.ToString());


                        dtResults = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, strUGAPSQL);
                        sbInserts.Remove(0, sbInserts.Length);
                        dtResults.TableName = "stg."+ strUGAPTableName;
                        intCnt = 0;

                        Console.WriteLine("Loading Cache....");
                        //PROCESS dtResults
                        if (dtResults.Rows.Count > 0)
                        {
                            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + " into Staging...";

                            DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                            DBConnection64.SQLServerBulkImportDT(dtResults, strILUCAConnectionString, 25000);
                            blUpdated = true;
                        }

                        continue;
                    }
                    intCnt++;

                }


                if (intCnt > 0)
                {
                    Console.WriteLine("Getting FINAL data frome Teradata....");
                    if (rs.lvlLevel == Level.EI || rs.lvlLevel == Level.EI_OX)
                        strUGAPSQL = getUGAPSQLTemplateEI(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP, rs.lvlLevel == Level.EI_OX).Replace("{$Inserts}", sbInserts.ToString());
                    else
                        strUGAPSQL = getUGAPSQLTemplateCS_IFP(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP, rs.lvlLevel == Level.CS).Replace("{$Inserts}", sbInserts.ToString());


                    dtResults = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, strUGAPSQL);
                    sbInserts.Remove(0, sbInserts.Length);
                    dtResults.TableName = "stg." + strUGAPTableName;

                    Console.WriteLine("Loading FINAL Cache....");
                    //PROCESS dtResults
                    if (dtResults.Rows.Count > 0)
                    {
                        strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + " into Staging...";

                        DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
                        //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
                        DBConnection64.SQLServerBulkImportDT(dtResults, strILUCAConnectionString, 25000);
                        blUpdated = true;
                    }


                    DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "UPDATE stg." + strUGAPTableName + " SET SearchMethod = '" + rs.strCleaningMethod + "' WHERE  SearchMethod IS NULL");

                }
            }



            //NEW REFRESH CACHE!!!!!
           // DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "exec[sp_mhp_refesh_filter_cache]");

            return blUpdated;
        }

        

        private static string getUGAPSQLTemplateCS_IFP(string strVolatileColumnsDeclare, string strVolatileColumns, string strVolatileName, string strFilterJoin, bool blIsCS)
        {
            StringBuilder sbSQL = new StringBuilder();


            //   sbSQL.Append("drop table " + strVolatileName + "; ");

            sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + strVolatileName + "( ");

            sbSQL.Append(strVolatileColumnsDeclare);

            sbSQL.Append(") PRIMARY INDEX(" + strVolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

            sbSQL.Append("{$vti}");

            sbSQL.Append("{$Inserts}");

            sbSQL.Append("{$vtc}");


            sbSQL.Append("COLLECT STATS COLUMN(" + strVolatileColumns + ") ON " + strVolatileName + "; ");
            sbSQL.Append("{$vts}");

            sbSQL.Append("SELECT ");
            sbSQL.Append("mm.mhp_uni_id,  ");
            sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
            sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
            sbSQL.Append("NULL as LEG_ENTY_NBR,  ");
            sbSQL.Append("NULL as LEG_ENTY_FULL_NM,  ");
            sbSQL.Append("NULL as HCE_LEG_ENTY_ROLLUP_DESC, ");
            sbSQL.Append("NULL as MKT_TYP_DESC,  ");
            sbSQL.Append("NULL as CUST_SEG_NBR,  ");
            sbSQL.Append("NULL as CUST_SEG_NM,  "); //ADD TO DB!!!!
            sbSQL.Append("i.PRDCT_CD,  ");
            sbSQL.Append("i.PRDCT_CD_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_RLLP_DESC,  ");
            sbSQL.Append("NULL as MKT_SEG_CD,  ");
            sbSQL.Append("NULL as FINC_ARNG_CD,  ");
            sbSQL.Append("NULL as FINC_ARNG_DESC,  ");
            sbSQL.Append("a.MBR_FST_NM, ");
            sbSQL.Append("a.MBR_LST_NM, ");
            sbSQL.Append("a.BTH_DT, ");
            sbSQL.Append("a.MBR_ALT_ID, ");
            sbSQL.Append("a.MBR_ID, ");
            sbSQL.Append("b.PRDCT_SYS_ID, ");
            sbSQL.Append("b.CS_PRDCT_CD_SYS_ID, ");
            sbSQL.Append("k.CS_CO_CD, ");
            sbSQL.Append("k.CS_CO_CD_ST, ");
            sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
            sbSQL.Append("FROM uhcdm001.hp_member a  ");
            sbSQL.Append("join uhcdm001.cs_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
            sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.cs_company_code k on b.CS_CO_CD_SYS_ID = k.CS_CO_CD_SYS_ID ");

            sbSQL.Append(strFilterJoin);
            sbSQL.Append("WHERE k.CS_CO_CD "+ (blIsCS ? "<>":"=") + " 'UHGEX'; ");
            sbSQL.Append("{$dvt}");
            sbSQL.Append("drop table " + strVolatileName + ";  ");



            return sbSQL.ToString();
        }
        private static string getUGAPSQLTemplateEI(string strVolatileColumnsDeclare, string strVolatileColumns, string strVolatileName, string strFilterJoin, bool blIsOX)
        {
            StringBuilder sbSQL = new StringBuilder();


            //   sbSQL.Append("drop table " + strVolatileName + "; ");

            sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + strVolatileName + "( ");

            sbSQL.Append(strVolatileColumnsDeclare);

            sbSQL.Append(") PRIMARY INDEX(" + strVolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

            sbSQL.Append("{$vti}");

            sbSQL.Append("{$Inserts}");

            sbSQL.Append("{$vtc}");


            sbSQL.Append("COLLECT STATS COLUMN(" + strVolatileColumns + ") ON " + strVolatileName + "; ");
            sbSQL.Append("{$vts}");

            sbSQL.Append("SELECT ");
            sbSQL.Append("mm.mhp_uni_id,  ");
            sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
            sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
            sbSQL.Append("e.LEG_ENTY_NBR,  ");
            sbSQL.Append("e.LEG_ENTY_FULL_NM,  ");
            sbSQL.Append("e.HCE_LEG_ENTY_ROLLUP_DESC,  ");
            sbSQL.Append("f.MKT_TYP_DESC,  ");
            sbSQL.Append("g.CUST_SEG_NBR,  ");
            sbSQL.Append("g.CUST_SEG_NM,  "); //ADD TO DB!!!!
            sbSQL.Append("i.PRDCT_CD,  ");
            sbSQL.Append("i.PRDCT_CD_DESC,  ");
            sbSQL.Append("j.MKT_SEG_DESC,  ");
            sbSQL.Append("j.MKT_SEG_RLLP_DESC,  ");
            sbSQL.Append("j.MKT_SEG_CD,  ");
            sbSQL.Append("k.FINC_ARNG_CD,  ");
            sbSQL.Append("k.FINC_ARNG_DESC,  ");
            sbSQL.Append("a.MBR_FST_NM, ");
            sbSQL.Append("a.MBR_LST_NM, ");
            sbSQL.Append("a.BTH_DT, ");
            sbSQL.Append("a.MBR_ALT_ID, ");
            sbSQL.Append("a.MBR_ID, ");
            sbSQL.Append("NULL as PRDCT_SYS_ID, ");
            sbSQL.Append("NULL as CS_PRDCT_CD_SYS_ID, ");
            sbSQL.Append("NULL as CS_CO_CD, ");
            sbSQL.Append("NULL as CS_CO_CD_ST, ");
            sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
            sbSQL.Append("FROM uhcdm001.hp_member a  ");
            sbSQL.Append("join uhcdm001.hp_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
            sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
            sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.LEGAL_ENTITY e on b.LEG_ENTY_SYS_ID = e.LEG_ENTY_SYS_ID  ");
            sbSQL.Append("join uhcdm001.MARKET_TYPE_CODE f on b.MKT_TYP_CD_SYS_ID = f.MKT_TYP_CD_SYS_ID  ");
            sbSQL.Append("join uhcdm001.CUSTOMER_SEGMENT g on b.CUST_SEG_SYS_ID = g.CUST_SEG_SYS_ID  ");
            sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
            sbSQL.Append("join uhcdm001.GROUP_INDICATOR j on b.GRP_IND_SYS_ID = j.GRP_IND_SYS_ID  ");
            sbSQL.Append("join uhcdm001.company_code k on b.CO_CD_SYS_ID = k.CO_CD_SYS_ID  ");
            sbSQL.Append(strFilterJoin);
            sbSQL.Append("WHERE e.HCE_LEG_ENTY_ROLLUP_DESC  " + (blIsOX ? "=" : "<>") + " 'OXFORD'; ");
            sbSQL.Append("{$dvt}");
            sbSQL.Append("drop table " + strVolatileName + ";  ");



            return sbSQL.ToString();
        }


        




        //private static bool getYearlyUniverses()
        //{

        //    bool blUpdated = false;

        //    Console.WriteLine("MHP Monthly Universes Parser");
        //    string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

        //    int intRowCnt = 1;
        //    int intFileCnt = 1;
        //    int intSheetCnt = 1;
        //    //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
        //    Console.WriteLine();
        //    Console.WriteLine("Processing spreadsheets");



        //    string strFileName = null;
        //    string strFilePath = @"\\msp09fil01\Radiology\Vendor Management - Delegation Oversight\Mental Health Parity (MHP)\MHP Monthly Universes - 2022";
        //    strFilePath = @"C:\Users\cgiorda\Desktop\MHP Universes - 2022\2022 Live";
        //    string strDate = "01/05/2022";

        //    string strSheetname = null;
        //    string strTableName = "stg.MHP_Yearly_Universes2022_v2";
        //    //string[] files;
        //    List<string> sheets;
        //    List<string> files;

        //    DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from " + strTableName);
        //    DataTable dtCurrentDataTable = null;
        //    DataTable dtFinalDataTable = null;
        //    OleDbDataReader dr = null;
        //    DataRow currentRow;
        //    //DataColumnCollection columns;
        //    List<string> columns;
        //    DateTime temp;
        //    TimeSpan tempTS;

        //    dtFinalDataTable = new DataTable();
        //    dtFinalDataTable.Columns.Add("State_of_Issue", typeof(String));
        //    dtFinalDataTable.Columns.Add("State_of_Residence", typeof(String));
        //    dtFinalDataTable.Columns.Add("Enrollee_First_Name", typeof(String));
        //    dtFinalDataTable.Columns.Add("Enrollee_Last_Name", typeof(String));
        //    dtFinalDataTable.Columns.Add("Cardholder_ID", typeof(String));
        //    dtFinalDataTable.Columns.Add("Funding_Arrangement", typeof(String));
        //    dtFinalDataTable.Columns.Add("Authorization", typeof(String));
        //    dtFinalDataTable.Columns.Add("Authorization_Type", typeof(String));
        //    dtFinalDataTable.Columns.Add("Request_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Request_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Request_Decision", typeof(String));
        //    dtFinalDataTable.Columns.Add("Decision_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Decision_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Decision_Reason", typeof(String));
        //    dtFinalDataTable.Columns.Add("Extension_Taken", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Member_Notif_Extension_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Additional_Info_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Primary_Procedure_Code_Req", typeof(String));
        //    dtFinalDataTable.Columns.Add("Procedure_Code_Description", typeof(String));
        //    dtFinalDataTable.Columns.Add("Primary_Diagnosis_Code", typeof(String));
        //    dtFinalDataTable.Columns.Add("Diagnosis_Code_Description", typeof(String));
        //    dtFinalDataTable.Columns.Add("Place_of_Service", typeof(Int16));
        //    dtFinalDataTable.Columns.Add("Member_Date_of_Birth", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Urgent_Processed_Standard", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Request_Additional_Info_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("FirstTier_Downstream_RelatedEntity", typeof(String));
        //    dtFinalDataTable.Columns.Add("Par_NonPar_Site", typeof(String));
        //    dtFinalDataTable.Columns.Add("Inpatient_Outpatient", typeof(String));
        //    dtFinalDataTable.Columns.Add("Delegate_Number", typeof(Int32));
        //    dtFinalDataTable.Columns.Add("ProgramType", typeof(String));
        //    dtFinalDataTable.Columns.Add("Insurance_Carrier", typeof(String));
        //    dtFinalDataTable.Columns.Add("Group_Number", typeof(String));

        //    dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
        //    dtFinalDataTable.Columns.Add("file_name", typeof(String));
        //    dtFinalDataTable.Columns.Add("file_path", typeof(String));
        //    dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

        //    dtFinalDataTable.TableName = strTableName;


        //    files = Directory.EnumerateFiles(strFileFolderPath, "*.xls*", SearchOption.TopDirectoryOnly).ToList();



        //    intFileCnt = 1;
        //    foreach (string strFile in files)
        //    {
        //        Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");

        //        strFileName = Path.GetFileName(strFile);

        //        if (strFileName.StartsWith("~") || strFileName.ToLower().Contains("chemo") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
        //        {
        //            intFileCnt++;
        //            continue;
        //        }


        //        var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
        //        intSheetCnt = 1;
        //        foreach (Sheet item in results)
        //        {
        //            Console.WriteLine("\r File:" + strFileName + ",  Sheet:" + item.Name);
        //            if (item.Name.ToString().ToLower().Trim().Equals("document map") || item.Name.ToString().ToLower().Trim().Equals("sheet2"))
        //                continue;

        //            strSheetname = item.Name.ToString();

        //            Console.Write("\rProcessing " + String.Format("{0:n0}", intSheetCnt) + " out of " + String.Format("{0:n0}", results.Count()) + " spreadsheets");

        //            try
        //            {
        //                //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
        //                dr = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataReader(strFile, strSheetname);
        //            }
        //            catch (Exception ex)
        //            {
        //                SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
        //                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
        //            }

        //            columns = new List<string>();
        //            for (int i = 0; i < dr.FieldCount; i++)
        //            {
        //                columns.Add(dr.GetName(i));
        //            }
                   
        //            //columns = dtCurrentDataTable.Columns;
        //            Console.Write("\rFile to DataTable");
        //            // strSummaryofLOB = strFolder.Split('_')[1];

        //            intRowCnt = 1;
        //            while ( dr.Read())
        //            {

        //                //Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

        //                currentRow = dtFinalDataTable.NewRow();

        //                if (columns.Contains("State of Issue"))
        //                    currentRow["State_of_Issue"] = (dr.GetValue(dr.GetOrdinal("State of Issue")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("State of Issue")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("State of Issue")) : (object)DBNull.Value);

        //                if (columns.Contains("State of Residence"))
        //                    currentRow["State_of_Residence"] = (dr.GetValue(dr.GetOrdinal("State of Residence")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("State of Residence")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("State of Residence")) : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee First Name"))
        //                    currentRow["Enrollee_First_Name"] = (dr.GetValue(dr.GetOrdinal("Enrollee First Name")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Enrollee First Name")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Enrollee First Name")) : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee Last Name"))
        //                    currentRow["Enrollee_Last_Name"] = (dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) : (object)DBNull.Value);

        //                if (columns.Contains("Cardholder ID"))
        //                    currentRow["Cardholder_ID"] = (dr.GetValue(dr.GetOrdinal("Cardholder ID")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Cardholder ID")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Cardholder ID")) : (object)DBNull.Value);

        //                if (columns.Contains("Funding Arrangement"))
        //                    currentRow["Funding_Arrangement"] = (dr.GetValue(dr.GetOrdinal("Funding Arrangement")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Funding Arrangement")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Funding Arrangement")) : (object)DBNull.Value);

        //                if (columns.Contains("Authorization"))
        //                    currentRow["Authorization"] = (dr.GetValue(dr.GetOrdinal("Authorization")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Authorization")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Authorization")) : (object)DBNull.Value);

        //                if (columns.Contains("Authorization Type"))
        //                    currentRow["Authorization_Type"] = (dr.GetValue(dr.GetOrdinal("Authorization Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Authorization Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Authorization Type")) : (object)DBNull.Value);

        //                if (columns.Contains("Date the request was received"))
        //                    currentRow["Request_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date the request was received")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date the request was received")) : (object)DBNull.Value);

        //                if (columns.Contains("Time the request was received"))
        //                    currentRow["Request_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time the request was received")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time the request was received")) : (object)DBNull.Value);

        //                if (columns.Contains("Request decision") || columns.Contains("Request Decision"))
        //                    currentRow["Request_Decision"] = (dr.GetValue(dr.GetOrdinal("Request decision")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Request decision")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Request decision")) : (object)DBNull.Value);

        //                if (columns.Contains("Date of decision") || columns.Contains("Date of Decision"))
        //                    currentRow["Decision_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of decision")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of decision")) : (object)DBNull.Value);

        //                if (columns.Contains("Time of decision") || columns.Contains("Time of Decision"))
        //                    currentRow["Decision_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time of decision")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time of decision")) : (object)DBNull.Value);

        //                if (columns.Contains("Decision Reason") || columns.Contains("Decision reason"))
        //                    currentRow["Decision_Reason"] = (dr.GetValue(dr.GetOrdinal("Decision Reason")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Decision Reason")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Decision Reason")) : (object)DBNull.Value);
        //                else if (columns.Contains("Denial Type") || columns.Contains("Denial type"))
        //                    currentRow["Decision_Reason"] = (dr.GetValue(dr.GetOrdinal("Denial Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Denial Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Denial Type")) : (object)DBNull.Value);

        //                if (columns.Contains("Was Extension Taken ? "))
        //                    currentRow["Extension_Taken"] = (!(dr.GetValue(dr.GetOrdinal("Was Extension Taken?")) + "").Trim().Equals("") ? ((dr.GetValue(dr.GetOrdinal("Was Extension Taken?")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);
        //                else if (columns.Contains("Was Extension Taken"))
        //                    currentRow["Extension_Taken"] = (!(dr.GetValue(dr.GetOrdinal("Was Extension Taken")) + "").Trim().Equals("") ? ((dr.GetValue(dr.GetOrdinal("Was Extension Taken")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of member notification of extension"))
        //                    currentRow["Member_Notif_Extension_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of member notification of extension")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of member notification of extension")) : (object)DBNull.Value);

        //                if (columns.Contains("Date additional information received"))
        //                    currentRow["Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date additional information received")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date additional information received")) : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date oral notification provided to enrollee")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date oral notification provided to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time oral notification provided to enrollee")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time oral notification provided to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date oral notification provided to provider")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date oral notification provided to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time oral notification provided to provider")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time oral notification provided to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date written notification sent to enrollee")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date written notification sent to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time written notification sent to enrollee")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time written notification sent to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date written notification sent to provider")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date written notification sent to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time written notification sent to provider")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time written notification sent to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Primary Procedure Code(s) Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) : (object)DBNull.Value);


        //                if (columns.Contains("Primary Procedure Code Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) : (object)DBNull.Value);

        //                if (columns.Contains("Procedure Code Description"))
        //                    currentRow["Procedure_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Procedure Code Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Procedure Code Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Procedure Code Description")) : (object)DBNull.Value);

        //                if (columns.Contains("Primary Diagnosis Code"))
        //                    currentRow["Primary_Diagnosis_Code"] = (dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) : (object)DBNull.Value);

        //                if (columns.Contains("Diagnosis Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Diagnosis Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Diagnosis Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Diagnosis Description")) : (object)DBNull.Value);
        //                else if (columns.Contains("Diagnosis Code Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) : (object)DBNull.Value);

        //                if (columns.Contains("Place of Service"))
        //                    currentRow["Place_of_Service"] = (dr.GetValue(dr.GetOrdinal("Place of Service")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Place of Service")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Place of Service")) : (object)DBNull.Value);

        //                if (columns.Contains("Member Date of Birth"))
        //                    currentRow["Member_Date_of_Birth"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Member Date of Birth")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Member Date of Birth")) : (object)DBNull.Value);

        //                if (columns.Contains("Was an urgent request made but processed as standard?"))
        //                    currentRow["Urgent_Processed_Standard"] = (!(dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("") && !(dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("NA") ? ((dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of request for additional information"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of request for additional information")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of request for additional information")) : (object)DBNull.Value);
        //                else if (columns.Contains("Date additional information requested"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date additional information requested")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date additional information requested")) : (object)DBNull.Value);

        //                if (columns.Contains("First Tier, Downstream, and Related Entity"))
        //                    currentRow["FirstTier_Downstream_RelatedEntity"] = (dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) : (object)DBNull.Value);

        //                if (columns.Contains("Delegate Number"))
        //                    currentRow["Delegate_Number"] = (dr.GetValue(dr.GetOrdinal("Delegate Number")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Delegate Number")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Delegate Number")) : (object)DBNull.Value);

        //                if (columns.Contains("Par/Non-Par Site"))
        //                    currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) : (object)DBNull.Value);

        //                if (columns.Contains("Inpatient/Outpatient"))
        //                    currentRow["Inpatient_Outpatient"] = (dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) : (object)DBNull.Value);



        //                if (columns.Contains("ProgramType"))
        //                    currentRow["ProgramType"] = (dr.GetValue(dr.GetOrdinal("ProgramType")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("ProgramType")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("ProgramType")) : (object)DBNull.Value);


        //                if (columns.Contains("Insurance Carrier"))
        //                    currentRow["Insurance_Carrier"] = (dr.GetValue(dr.GetOrdinal("Insurance Carrier")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Insurance Carrier")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Insurance Carrier")) : (object)DBNull.Value);


        //                if (columns.Contains("Group Number"))
        //                    currentRow["Group_Number"] = (dr.GetValue(dr.GetOrdinal("Group Number")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Group Number")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Group Number")) : (object)DBNull.Value);



        //                currentRow["file_date"] = DateTime.Parse(strDate);
        //                currentRow["sheet_name"] = strSheetname;
        //                currentRow["file_name"] = strFileName;
        //                currentRow["file_path"] = strFilePath;
        //                dtFinalDataTable.Rows.Add(currentRow);
        //                intRowCnt++;


        //                if (dtFinalDataTable.Rows.Count >= 10000)
        //                {
        //                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

        //                    DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                    DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

        //                    blUpdated = true;

        //                    dtFinalDataTable.Clear();
        //                }

       
        //            }
        //            dr.Close();
        //            currentRow = null;
        //            dtCurrentDataTable = null;


        //            if (dtFinalDataTable.Rows.Count > 0)
        //            {
        //                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

        //                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 25000);

        //                blUpdated = true;
        //            }

        //            dtFinalDataTable.Clear();
        //            intSheetCnt++;

        //        }

        //        intFileCnt++;
        //    }


        //    return blUpdated;
        //}

        //private static bool getMonthlyUniverses()
        //{

        //    bool blUpdated = false;

        //    Console.WriteLine("MHP Monthly Universes Parser");
        //    string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

        //    int intRowCnt = 1;
        //    int intFileCnt = 1;
        //    int intSheetCnt = 1;
        //    //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
        //    Console.WriteLine();
        //    Console.WriteLine("Processing spreadsheets");



        //    string strFileName = null;
        //    string strFilePath = @"\\msp09fil01\Radiology\Vendor Management - Delegation Oversight\Mental Health Parity (MHP)\MHP Monthly Universes - 2022";
        //    strFileFolderPath = @"C:\Users\cgiorda\Desktop\MHP_2022";
        //    string strDate = "12/21/2022";
            
        //    string strSheetname = null;
        //    string strTableName = "stg.MHP_Yearly_Universes2022";
        //    //string[] files;
        //    List<string> sheets;
        //    List<string> files;

        //    DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from " + strTableName);
        //    DataTable dtCurrentDataTable = null;
        //    DataTable dtFinalDataTable = null;
        //    DataRow currentRow;
        //    DataColumnCollection columns;
        //    SpreadsheetDocument wbCurrentExcelFile;
        //    DateTime temp;
        //    TimeSpan tempTS;

        //    dtFinalDataTable = new DataTable();
        //    dtFinalDataTable.Columns.Add("State_of_Issue", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("State_of_Residence", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Enrollee_First_Name", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Enrollee_Last_Name", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Cardholder_ID", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Funding_Arrangement", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Authorization", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Authorization_Type", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Request_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Request_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Request_Decision", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Decision_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Decision_Time", typeof(TimeSpan)); 
        //    dtFinalDataTable.Columns.Add("Decision_Reason", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Extension_Taken", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Member_Notif_Extension_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Additional_Info_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Time", typeof(TimeSpan)); 
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Time", typeof(TimeSpan)); 
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Time", typeof(TimeSpan)); 
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Time", typeof(TimeSpan)); 
        //    dtFinalDataTable.Columns.Add("Primary_Procedure_Code_Req", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Procedure_Code_Description", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Primary_Diagnosis_Code", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Diagnosis_Code_Description", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Place_of_Service", typeof(Int16)); 
        //    dtFinalDataTable.Columns.Add("Member_Date_of_Birth", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("Urgent_Processed_Standard", typeof(Boolean)); 
        //    dtFinalDataTable.Columns.Add("Request_Additional_Info_Date", typeof(DateTime)); 
        //    dtFinalDataTable.Columns.Add("FirstTier_Downstream_RelatedEntity", typeof(String));
        //    dtFinalDataTable.Columns.Add("Par_NonPar_Site", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Inpatient_Outpatient", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("Delegate_Number", typeof(Int32)); 
        //    dtFinalDataTable.Columns.Add("sheet_name", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("file_name", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("file_path", typeof(String)); 
        //    dtFinalDataTable.Columns.Add("file_date", typeof(DateTime)); 

        //    dtFinalDataTable.TableName = strTableName;

            
        //    files = Directory.EnumerateFiles(strFileFolderPath, "*.xls*", SearchOption.TopDirectoryOnly).ToList();


        //   // bool blFoundSheet = false;

        //    intFileCnt = 1;
        //    foreach (string strFile in files)
        //    {
        //        Console.Write("Processing :" + intFileCnt + " out of " + files.Count + " : "+ strFile);


        //        Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");

        //        strFileName = Path.GetFileName(strFile);

        //        if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
        //        {
        //            intFileCnt++;
        //            continue;
        //        }


        //        //if(strFileName.StartsWith("Americhoice"))
        //        //{
        //        //    sheets = new List<string>();
        //        //    sheets.Add("AMCH");
        //        //    sheets.Add("RAD_CARD");
        //        //}
        //        //else if (strFileName.StartsWith("Oxford"))
        //        //{
        //        //    sheets = new List<string>();
        //        //    sheets.Add("Oxford-Radiology");
        //        //    sheets.Add("Oxford Rad & Card");
        //        //    sheets.Add("Cardiology");
        //        //    sheets.Add("OX-CARDIO");

        //        //}
        //        //else if (strFileName.StartsWith("United"))
        //        //{
        //        //    sheets = new List<string>();
        //        //    sheets.Add("Rad & Card-Non-U12");
        //        //    sheets.Add("Rad & Card - U12");
        //        //    sheets.Add("Rad & Card");
        //        //    sheets.Add("Rad & Card IFP - U12");
        //        //    sheets.Add("Rad & Card- Non U12");
        //        //    sheets.Add("Rad & Card U12");
        //        //}
        //        //else
        //        //{
        //        //    continue;
        //        //}

        //        intSheetCnt = 1;

        //        var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
        //        intSheetCnt = 1;
        //        foreach (Sheet item in results)
        //        {

        //            strSheetname = item.Name.ToString();
         
        //            Console.Write("\rProcessing " + String.Format("{0:n0}", intSheetCnt) + " out of " + String.Format("{0:n0}", results.Count()) + " spreadsheets");

                  
        //                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);

        //                //wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
        //                //dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);

                     


        //            // 
        //            columns = dtCurrentDataTable.Columns;
        //            Console.Write("\rFile to DataTable");
        //            // strSummaryofLOB = strFolder.Split('_')[1];

        //            intRowCnt = 1;
        //            foreach (DataRow d in dtCurrentDataTable.Rows)
        //            {

        //                Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

        //                currentRow = dtFinalDataTable.NewRow();

        //                if (columns.Contains("State of Issue"))
        //                    currentRow["State_of_Issue"] = (d["State of Issue"] != DBNull.Value && !(d["State of Issue"] + "").Trim().Equals("") ? d["State of Issue"] : (object)DBNull.Value);

        //                if (columns.Contains("State of Residence"))
        //                    currentRow["State_of_Residence"] = (d["State of Residence"] != DBNull.Value && !(d["State of Residence"] + "").Trim().Equals("") ? d["State of Residence"] : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee First Name"))
        //                    currentRow["Enrollee_First_Name"] = (d["Enrollee First Name"] != DBNull.Value && !(d["Enrollee First Name"] + "").Trim().Equals("") ? d["Enrollee First Name"] : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee Last Name"))
        //                    currentRow["Enrollee_Last_Name"] = (d["Enrollee Last Name"] != DBNull.Value && !(d["Enrollee Last Name"] + "").Trim().Equals("") ? d["Enrollee Last Name"] : (object)DBNull.Value);

        //                if (columns.Contains("Cardholder ID"))
        //                    currentRow["Cardholder_ID"] = (d["Cardholder ID"] != DBNull.Value && !(d["Cardholder ID"] + "").Trim().Equals("") ? d["Cardholder ID"] : (object)DBNull.Value);

        //                if (columns.Contains("Funding Arrangement"))
        //                    currentRow["Funding_Arrangement"] = (d["Funding Arrangement"] != DBNull.Value && !(d["Funding Arrangement"] + "").Trim().Equals("") ? d["Funding Arrangement"] : (object)DBNull.Value);

        //                if (columns.Contains("Authorization"))
        //                    currentRow["Authorization"] = (d["Authorization"] != DBNull.Value && !(d["Authorization"] + "").Trim().Equals("") ? d["Authorization"] : (object)DBNull.Value);

        //                if (columns.Contains("Authorization Type"))
        //                    currentRow["Authorization_Type"] = (d["Authorization Type"] != DBNull.Value && !(d["Authorization Type"] + "").Trim().Equals("") ? d["Authorization Type"] : (object)DBNull.Value);

        //                if (columns.Contains("Date the request was received"))
        //                    currentRow["Request_Date"] = (DateTime.TryParse(d["Date the request was received"] + "", out temp) ? d["Date the request was received"] : (object)DBNull.Value);

        //                if (columns.Contains("Time the request was received"))
        //                    currentRow["Request_Time"] = (TimeSpan.TryParse(d["Time the request was received"] + "", out tempTS) ? d["Time the request was received"] : (object)DBNull.Value);

        //                if (columns.Contains("Request decision") || columns.Contains("Request Decision"))
        //                    currentRow["Request_Decision"] = (d["Request decision"] != DBNull.Value && !(d["Request decision"] + "").Trim().Equals("") ? d["Request decision"] : (object)DBNull.Value);

        //               // if ()
        //                  //  currentRow["Request_Decision"] = (d["Request Decision"] != DBNull.Value && !(d["Request Decision"] + "").Trim().Equals("") ? d["Request Decision"] : (object)DBNull.Value);


        //                if (columns.Contains("Date of decision") || columns.Contains("Date of Decision"))
        //                    currentRow["Decision_Date"] = (DateTime.TryParse(d["Date of decision"] + "", out temp) ? d["Date of decision"] : (object)DBNull.Value);

        //                if (columns.Contains("Time of decision") || columns.Contains("Time of Decision"))
        //                    currentRow["Decision_Time"] = (TimeSpan.TryParse(d["Time of decision"] + "", out tempTS) ? d["Time of decision"] : (object)DBNull.Value);

        //                if (columns.Contains("Decision Reason"))
        //                    currentRow["Decision_Reason"] = (d["Decision Reason"] != DBNull.Value && !(d["Decision Reason"] + "").Trim().Equals("") ? d["Decision Reason"] : (object)DBNull.Value);
        //                else if (columns.Contains("Denial Type"))
        //                    currentRow["Decision_Reason"] = (d["Denial Type"] != DBNull.Value && !(d["Denial Type"] + "").Trim().Equals("") ? d["Denial Type"] : (object)DBNull.Value);

        //                if (columns.Contains("Was Extension Taken ? "))
        //                    currentRow["Extension_Taken"] = (!(d["Was Extension Taken?"] + "").Trim().Equals("") ? ((d["Was Extension Taken?"] + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);
        //                else if (columns.Contains("Was Extension Taken"))
        //                    currentRow["Extension_Taken"] = (!(d["Was Extension Taken"] + "").Trim().Equals("") ? ((d["Was Extension Taken"] + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of member notification of extension"))
        //                    currentRow["Member_Notif_Extension_Date"] = (DateTime.TryParse(d["Date of member notification of extension"] + "", out temp) ? d["Date of member notification of extension"] : (object)DBNull.Value);

        //                if (columns.Contains("Date additional information received"))
        //                    currentRow["Additional_Info_Date"] = (DateTime.TryParse(d["Date additional information received"] + "", out temp) ? d["Date additional information received"] : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Date"] = (DateTime.TryParse(d["Date oral notification provided to enrollee"] + "", out temp) ? d["Date oral notification provided to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Time"] = (TimeSpan.TryParse(d["Time oral notification provided to enrollee"] + "", out tempTS) ? d["Time oral notification provided to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Date"] = (DateTime.TryParse(d["Date oral notification provided to provider"] + "", out temp) ? d["Date oral notification provided to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Time"] = (TimeSpan.TryParse(d["Time oral notification provided to provider"] + "", out tempTS) ? d["Time oral notification provided to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Date"] = (DateTime.TryParse(d["Date written notification sent to enrollee"] + "", out temp) ? d["Date written notification sent to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Time"] = (TimeSpan.TryParse(d["Time written notification sent to enrollee"] + "", out tempTS) ? d["Time written notification sent to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Date"] = (DateTime.TryParse(d["Date written notification sent to provider"] + "", out temp) ? d["Date written notification sent to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Time"] = (TimeSpan.TryParse(d["Time written notification sent to provider"] + "", out tempTS) ? d["Time written notification sent to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Primary Procedure Code(s) Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (d["Primary Procedure Code(s) Requested"] != DBNull.Value && !(d["Primary Procedure Code(s) Requested"] + "").Trim().Equals("") ? d["Primary Procedure Code(s) Requested"] : (object)DBNull.Value);

        //                if (columns.Contains("Primary Procedure Code Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (d["Primary Procedure Code Requested"] != DBNull.Value && !(d["Primary Procedure Code Requested"] + "").Trim().Equals("") ? d["Primary Procedure Code Requested"] : (object)DBNull.Value);

        //                if (columns.Contains("Procedure Code Description"))
        //                    currentRow["Procedure_Code_Description"] = (d["Procedure Code Description"] != DBNull.Value && !(d["Procedure Code Description"] + "").Trim().Equals("") ? d["Procedure Code Description"] : (object)DBNull.Value);

        //                if (columns.Contains("Primary Diagnosis Code"))
        //                    currentRow["Primary_Diagnosis_Code"] = (d["Primary Diagnosis Code"] != DBNull.Value && !(d["Primary Diagnosis Code"] + "").Trim().Equals("") ? d["Primary Diagnosis Code"] : (object)DBNull.Value);

        //                if (columns.Contains("Diagnosis Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (d["Diagnosis Description"] != DBNull.Value && !(d["Diagnosis Description"] + "").Trim().Equals("") ? d["Diagnosis Description"] : (object)DBNull.Value);
        //                else if (columns.Contains("Diagnosis Code Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (d["Diagnosis Code Description"] != DBNull.Value && !(d["Diagnosis Code Description"] + "").Trim().Equals("") ? d["Diagnosis Code Description"] : (object)DBNull.Value);

        //                if (columns.Contains("Place of Service"))
        //                    currentRow["Place_of_Service"] = (d["Place of Service"] != DBNull.Value && !(d["Place of Service"] + "").Trim().Equals("") ? d["Place of Service"] : (object)DBNull.Value);

        //                if (columns.Contains("Member Date of Birth"))
        //                    currentRow["Member_Date_of_Birth"] = (DateTime.TryParse(d["Member Date of Birth"] + "", out temp) ? d["Member Date of Birth"] : (object)DBNull.Value);

        //                if (columns.Contains("Was an urgent request made but processed as standard?"))
        //                    currentRow["Urgent_Processed_Standard"] = (!(d["Was an urgent request made but processed as standard?"] + "").Trim().Equals("") && !(d["Was an urgent request made but processed as standard?"] + "").Trim().Equals("NA") ? ((d["Was an urgent request made but processed as standard?"] + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of request for additional information"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(d["Date of request for additional information"] + "", out temp) ? d["Date of request for additional information"] : (object)DBNull.Value);
        //                else if(columns.Contains("Date additional information requested"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(d["Date additional information requested"] + "", out temp) ? d["Date additional information requested"] : (object)DBNull.Value);

        //                if (columns.Contains("First Tier, Downstream, and Related Entity"))
        //                    currentRow["FirstTier_Downstream_RelatedEntity"] = (d["First Tier, Downstream, and Related Entity"] != DBNull.Value && !(d["First Tier, Downstream, and Related Entity"] + "").Trim().Equals("") ? d["First Tier, Downstream, and Related Entity"] : (object)DBNull.Value);

        //                if (columns.Contains("Delegate Number"))
        //                    currentRow["Delegate_Number"] = (d["Delegate Number"] != DBNull.Value && !(d["Delegate Number"] + "").Trim().Equals("") ? d["Delegate Number"] : (object)DBNull.Value);

        //                if (columns.Contains("Par/Non-Par Site"))
        //                    currentRow["Par_NonPar_Site"] = (d["Par/Non-Par Site"] != DBNull.Value && !(d["Par/Non-Par Site"] + "").Trim().Equals("") ? d["Par/Non-Par Site"] : (object)DBNull.Value);

        //                if (columns.Contains("Inpatient/Outpatient"))
        //                    currentRow["Inpatient_Outpatient"] = (d["Inpatient/Outpatient"] != DBNull.Value && !(d["Inpatient/Outpatient"] + "").Trim().Equals("") ? d["Inpatient/Outpatient"] : (object)DBNull.Value);

        //                currentRow["file_date"] = DateTime.Parse(strDate);
        //                currentRow["sheet_name"] = strSheetname;
        //                currentRow["file_name"] = strFileName;
        //                currentRow["file_path"] = strFilePath;
        //                dtFinalDataTable.Rows.Add(currentRow);
        //                intRowCnt++;
        //            }
        //            currentRow = null;
        //            dtCurrentDataTable = null;


        //            if (dtFinalDataTable.Rows.Count > 0)
        //            {
        //                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

        //                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

        //                blUpdated = true;
        //            }

        //            dtFinalDataTable.Clear();
        //            intSheetCnt++;

        //        }

        //        File.Move(strFile, @"C:\Users\cgiorda\Desktop\MHP_2022\archive\" + strFileName);
        //        intFileCnt++;
        //    }


        //    return blUpdated;
        //}

        static string strMessageGlobal = null;
        private static void OnSqlRowsCopied(object sender, SqlRowsCopiedEventArgs e)
        {
            Console.Write(strMessageGlobal.Replace("{$rowCnt}", String.Format("{0:n0}", e.RowsCopied)));
        }


        //private static bool getUGAP_CACHE2()
        //{
        //    bool blUpdated = false;

        //    Console.WriteLine("MHP Monthly Universes Parser");
        //    string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
        //    string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
        //    string strVolatileColumnsDeclare;
        //    string strVolatileColumns;
        //    string strVolatileName;
        //    string strFilterJoin = "";


        //    // DataTable d = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, "SELECT top 1 mbr_fst_nm from uhcdm001.hp_member ");


        //    StringBuilder sbInserts = new StringBuilder();


        //    Console.WriteLine("Processing spreadsheets");
        //    int intCnt = 0;
        //    int intTotalCnt = 1;
        //    int intTotal = 0;
        //    //DataTable dtFinalDataTable = null;
        //    //dtFinalDataTable = new DataTable();
        //    //dtFinalDataTable.Columns.Add("mhp_uni_id", typeof(Int64));
        //    //dtFinalDataTable.Columns.Add("PLN_VAR_SUBDIV_CD", typeof(String));
        //    //dtFinalDataTable.Columns.Add("mnth_eff_dt", typeof(DateTime));
        //    //dtFinalDataTable.Columns.Add("LEG_ENTY_NBR", typeof(String));
        //    //dtFinalDataTable.Columns.Add("MKT_TYP_DESC", typeof(String));
        //    //dtFinalDataTable.Columns.Add("CUST_SEG_NBR", typeof(String));
        //    //dtFinalDataTable.Columns.Add("PRDCT_CD", typeof(String));
        //    //dtFinalDataTable.Columns.Add("PRDCT_CD_DESC", typeof(String));
        //    //dtFinalDataTable.Columns.Add("MKT_SEG_DESC", typeof(String));
        //    //dtFinalDataTable.Columns.Add("FINC_ARNG_CD", typeof(String));
        //    //dtFinalDataTable.TableName = "stg.MHP_Yearly_Universes_UGAP_CACHE";



        //    //CREATE INDEX indx_mhp_uni_id ON[stg].[MHP_Yearly_Universes_UGAP] (mhp_uni_id);
        //    //CREATE INDEX indx_LEG_ENTY_NBR ON[stg].[MHP_Yearly_Universes_UGAP] (LEG_ENTY_NBR);
        //    //CREATE INDEX indx_LEG_ENTY_FULL_NM ON[stg].[MHP_Yearly_Universes_UGAP] (LEG_ENTY_FULL_NM);
        //    //CREATE INDEX indx_MKT_SEG_RLLP_DESC ON[stg].[MHP_Yearly_Universes_UGAP] (MKT_SEG_RLLP_DESC);
        //    //CREATE INDEX indx_FINC_ARNG_DESC ON[stg].[MHP_Yearly_Universes_UGAP] (FINC_ARNG_DESC);
        //    //CREATE INDEX indx_MKT_TYP_DESC ON[stg].[MHP_Yearly_Universes_UGAP] (MKT_TYP_DESC);




        //    strVolatileColumnsDeclare = "mhp_uni_id BIGINT, Cardholder_ID_CLN  VARCHAR(11), State_Of_Issue VARCHAR(5),BTH_DT DATE, REQ_DT DATE, MBR_FST_NM VARCHAR(25), MBR_LST_NM VARCHAR(25) ";
        //    strVolatileColumns = "mhp_uni_id, Cardholder_ID_CLN, State_Of_Issue, BTH_DT, REQ_DT, MBR_FST_NM, MBR_LST_NM ";
        //    strVolatileName = "MissingMembersTmp";


        //    //UNITED PCP sheet 2 zeros infront cardholdrer
        //    //UNIVERE PAD 9 
        //    //

        //    DataTable dtResults = new DataTable();
        //    DataTable dtLatestUniverses = new DataTable();







        //    List<RandomSQL> lstRand = new List<RandomSQL>();
        //    //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'UnitedPCP%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.MBR_ALT_ID) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt)", strCleaningMethod = "MBR_ALT_ID/BD/RD", isCS = false });
        //    //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'UnitedPCP%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on a.MBR_FST_NM LIKE mm.MBR_FST_NM AND  a.MBR_LST_NM LIKE mm.MBR_LST_NM AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt);", strCleaningMethod = "FN/LN/BD/RD", isCS = false });
        //    //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'Oxford%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.MBR_ID) = mm.Cardholder_ID_CLN AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt); ", strCleaningMethod = "MBR_ID/BD/RD", isCS = false });
        //    //lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'Oxford%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on a.MBR_FST_NM LIKE mm.MBR_FST_NM AND  a.MBR_LST_NM LIKE mm.MBR_LST_NM AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt);", strCleaningMethod = "FN/LN/BD/RD", isCS = false });
        //    lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'C&S%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.MBR_ID) = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt); ", strCleaningMethod = "MBR_ID/CS_CO_CD_ST/BD/RD", isCS = true });
        //    lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'C&S%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on trim(leading '0' from a.SBSCR_MEDCD_RCIP_NBR) = mm.Cardholder_ID_CLN  AND k.CS_CO_CD_ST = mm.State_Of_Issue AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt); ", strCleaningMethod = "SBSCR_MEDCD_RCIP_NBR/CS_CO_CD_ST/BD/RD", isCS = true });
        //    lstRand.Add(new RandomSQL() { strSQLILUCA = "AND (file_name LIKE 'C&S%')", strSQLUGAP = "inner join " + strVolatileName + " as mm on a.MBR_FST_NM LIKE mm.MBR_FST_NM AND  a.MBR_LST_NM LIKE mm.MBR_LST_NM AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt);", strCleaningMethod = "FN/LN/BD/RD", isCS = true });


        //    foreach (RandomSQL rs in lstRand)
        //    {
        //        dtLatestUniverses = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "SELECT mhp_uni_id, REPLACE(SUBSTRING([Cardholder_ID], PATINDEX('%[^0]%', [Cardholder_ID]+'.'), LEN([Cardholder_ID])),[State_of_Issue],'') AS [Cardholder_ID_CLN], State_Of_Issue, CONVERT(char(10), [Member_Date_of_Birth],126) as Member_Date_of_Birth, CONVERT(char(10), [Request_Date], 126) as Request_Date, [Enrollee_First_Name] ,[Enrollee_Last_Name] ,[sheet_name] FROM [IL_UCA].[stg].[MHP_Yearly_Universes] WHERE [Member_Date_of_Birth] is not null AND  [Cardholder_ID] IS NOT NULL AND [Request_Date] IS NOT NULL " + rs.strSQLILUCA + " AND mhp_uni_id not in (select mhp_uni_id from [IL_UCA].[stg].[MHP_Yearly_Universes_UGAP]) ORDER BY mhp_uni_id DESC ");

        //        intTotalCnt = 0;

        //        string strUGAPSQL = "";





        //        //SELECT distinct Cardholder_ID FROM[stg].[MHP_Yearly_Universes]  WHERE Cardholder_ID not like '%[^0-9]%' and Cardholder_ID != ''
        //        intTotal = dtLatestUniverses.Rows.Count;
        //        foreach (DataRow dr in dtLatestUniverses.Rows)
        //        {




        //            Console.WriteLine("Processing " + intTotalCnt + " out of " + intTotal);

        //            var fnw = dr["Enrollee_First_Name"].ToString().Substring(0, Math.Min(3, dr["Enrollee_First_Name"].ToString().Length)).Replace("'", "''") + "%";
        //            var lnw = dr["Enrollee_Last_Name"].ToString().Substring(0, Math.Min(3, dr["Enrollee_Last_Name"].ToString().Length)).Replace("'", "''") + "%";
        //            var st = dr["State_Of_Issue"].ToString();
        //            var id = dr["mhp_uni_id"].ToString();
        //            var cidc = dr["Cardholder_ID_CLN"].ToString();
        //            var bd = dr["Member_Date_of_Birth"].ToString();
        //            var rd = dr["Request_Date"].ToString();
        //            sbInserts.Append("INSERT INTO " + strVolatileName + " (" + strVolatileColumns + ") VALUES(" + id + ",'" + cidc + "','" + st + "', '" + bd + "', '" + rd + "', '" + fnw + "', '" + lnw + "'); ");

        //            intTotalCnt++;
        //            if (intCnt == 1)
        //            {
        //                Console.WriteLine("Getting data frome Teradata....");

        //                if (rs.isCS)
        //                    strUGAPSQL = getUGAPSQLTemplate2CS(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP).Replace("{$Inserts}", sbInserts.ToString());
        //                else
        //                    strUGAPSQL = getUGAPSQLTemplate2(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP).Replace("{$Inserts}", sbInserts.ToString());


        //                dtResults = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, strUGAPSQL);
        //                sbInserts.Remove(0, sbInserts.Length);
        //                dtResults.TableName = "stg.MHP_Yearly_Universes_UGAP";
        //                intCnt = 0;

        //                Console.WriteLine("Loading Cache....");
        //                //PROCESS dtResults
        //                if (dtResults.Rows.Count > 0)
        //                {
        //                    strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + " into Staging...";

        //                    DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                    //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                    DBConnection64.SQLServerBulkImportDT(dtResults, strILUCAConnectionString, 25000);
        //                    blUpdated = true;
        //                }

        //                continue;
        //            }
        //            intCnt++;

        //        }


        //        if (intCnt > 0)
        //        {
        //            Console.WriteLine("Getting FINAL data frome Teradata....");
        //            if (rs.isCS)
        //                strUGAPSQL = getUGAPSQLTemplate2CS(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP).Replace("{$Inserts}", sbInserts.ToString());
        //            else
        //                strUGAPSQL = getUGAPSQLTemplate2(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName, rs.strSQLUGAP).Replace("{$Inserts}", sbInserts.ToString());


        //            dtResults = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, strUGAPSQL);
        //            sbInserts.Remove(0, sbInserts.Length);
        //            dtResults.TableName = "stg.MHP_Yearly_Universes_UGAP";

        //            Console.WriteLine("Loading FINAL Cache....");
        //            //PROCESS dtResults
        //            if (dtResults.Rows.Count > 0)
        //            {
        //                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + " into Staging...";

        //                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                DBConnection64.SQLServerBulkImportDT(dtResults, strILUCAConnectionString, 25000);
        //                blUpdated = true;
        //            }


        //            DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "UPDATE stg.MHP_Yearly_Universes_UGAP SET SearchMethod = '" + rs.strCleaningMethod + "' WHERE  SearchMethod IS NULL");


        //        }
        //    }

        //    //NEW REFRESH CACHE!!!!!
        //    DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "exec[sp_mhp_refesh_filter_cache]");




        //    //DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL");
        //    //
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    return blUpdated;
        //}

        //private static bool getUGAP_CACHE()
        //{
        //    bool blUpdated = false;

        //    Console.WriteLine("MHP Monthly Universes Parser");
        //    string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];
        //    string strUGAP_ConnectionString = ConfigurationManager.AppSettings["UGAP_Database"];
        //    string strVolatileColumnsDeclare;
        //    string strVolatileColumns;
        //    string strVolatileName;



        //    // DataTable d = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, "SELECT top 1 mbr_fst_nm from uhcdm001.hp_member ");


        //    StringBuilder sbInserts = new StringBuilder();


        //    Console.WriteLine("Processing spreadsheets");
        //    int intCnt = 0;

        //    //DataTable dtFinalDataTable = null;
        //    //dtFinalDataTable = new DataTable();
        //    //dtFinalDataTable.Columns.Add("mhp_uni_id", typeof(Int64));
        //    //dtFinalDataTable.Columns.Add("PLN_VAR_SUBDIV_CD", typeof(String));
        //    //dtFinalDataTable.Columns.Add("mnth_eff_dt", typeof(DateTime));
        //    //dtFinalDataTable.Columns.Add("LEG_ENTY_NBR", typeof(String));
        //    //dtFinalDataTable.Columns.Add("MKT_TYP_DESC", typeof(String));
        //    //dtFinalDataTable.Columns.Add("CUST_SEG_NBR", typeof(String));
        //    //dtFinalDataTable.Columns.Add("PRDCT_CD", typeof(String));
        //    //dtFinalDataTable.Columns.Add("PRDCT_CD_DESC", typeof(String));
        //    //dtFinalDataTable.Columns.Add("MKT_SEG_DESC", typeof(String));
        //    //dtFinalDataTable.Columns.Add("FINC_ARNG_CD", typeof(String));
        //    //dtFinalDataTable.TableName = "stg.MHP_Yearly_Universes_UGAP_CACHE";

        //    strVolatileColumnsDeclare = "mhp_uni_id BIGINT, MBR_FST_NM VARCHAR(10), MBR_LST_NM VARCHAR(10), BTH_DT DATE, REQ_DT DATE, Cardholder_ID  VARCHAR(11) ";
        //    strVolatileColumns = "mhp_uni_id, MBR_FST_NM, MBR_LST_NM, BTH_DT, REQ_DT, Cardholder_ID ";
        //    strVolatileName = "MissingMembersTmp";


        //    DataTable dtResults = new DataTable();
        //    DataTable dtLatestUniverses = new DataTable();
        //    dtLatestUniverses = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "SELECT mhp_uni_id, [Enrollee_First_Name] ,[Enrollee_Last_Name] ,[Cardholder_ID] ,CONVERT(char(10), [Member_Date_of_Birth],126) as Member_Date_of_Birth, CONVERT(char(10), [Request_Date], 126) as Request_Date FROM [IL_UCA].[stg].[MHP_Yearly_Universes] WHERE [Member_Date_of_Birth] is not null AND (sheet_name LIKE 'Oxford%')  ORDER BY [Cardholder_ID] ");

        //    foreach (DataRow dr in dtLatestUniverses.Rows)
        //    {

        //        var id = dr["mhp_uni_id"].ToString();
        //        var fn = dr["Enrollee_First_Name"].ToString().Substring(0, Math.Min(3, dr["Enrollee_First_Name"].ToString().Length)).Replace("'", "''");
        //        var ln = dr["Enrollee_Last_Name"].ToString().Substring(0, Math.Min(3, dr["Enrollee_Last_Name"].ToString().Length)).Replace("'", "''");
        //        var bd = dr["Member_Date_of_Birth"].ToString();
        //        var rd = dr["Request_Date"].ToString();
        //        var cid = dr["Cardholder_ID"].ToString();
        //        sbInserts.Append("INSERT INTO " + strVolatileName + " (" + strVolatileColumns + ") VALUES(" + id + ",'" + fn + "%', '" + ln + "%', '" + bd + "', '" + rd + "', '" + cid + "'); ");


        //        if (intCnt == 1000)
        //        {

        //            dtResults = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, getUGAPSQLTemplate(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName).Replace("{$Inserts}", sbInserts.ToString()));
        //            sbInserts.Remove(0, sbInserts.Length);
        //            dtResults.TableName = "stg.MHP_Yearly_Universes_UGAP_CACHE";
        //            intCnt = 0;


        //            //PROCESS dtResults
        //            if (dtResults.Rows.Count > 0)
        //            {
        //                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + " into Staging...";

        //                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                DBConnection64.SQLServerBulkImportDT(dtResults, strILUCAConnectionString, 25000);
        //                blUpdated = true;
        //            }

        //            continue;
        //        }
        //        intCnt++;
        //    }


        //    if (intCnt > 0)
        //    {

        //        dtResults = DBConnection64.getTeraDataDataTable(strUGAP_ConnectionString, getUGAPSQLTemplate(strVolatileColumnsDeclare, strVolatileColumns, strVolatileName).Replace("{$Inserts}", sbInserts.ToString()));
        //        sbInserts.Remove(0, sbInserts.Length);
        //        dtResults.TableName = "stg.MHP_Yearly_Universes_UGAP_CACHE";
        //        //PROCESS dtResults
        //        if (dtResults.Rows.Count > 0)
        //        {
        //            strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtResults.Rows.Count) + " into Staging...";

        //            DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //            //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //            DBConnection64.SQLServerBulkImportDT(dtResults, strILUCAConnectionString, 25000);
        //            blUpdated = true;
        //        }


        //    }

        //    DBConnection64.ExecuteMSSQL(strILUCAConnectionString, "UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL");
        //    //
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    //UPDATE [IL_UCA].[stg].[MHP_Yearly_Universes] SET ugap_process_date = GETDATE() WHERE [Member_Date_of_Birth] is not null AND ugap_process_date IS NULL
        //    return blUpdated;
        //}

        //private static string getUGAPSQLTemplate2CS(string strVolatileColumnsDeclare, string strVolatileColumns, string strVolatileName, string strFilterJoin)
        //{
        //    StringBuilder sbSQL = new StringBuilder();


        //    //   sbSQL.Append("drop table " + strVolatileName + "; ");

        //    sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + strVolatileName + "( ");

        //    sbSQL.Append(strVolatileColumnsDeclare);

        //    sbSQL.Append(") PRIMARY INDEX(" + strVolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

        //    sbSQL.Append("{$vti}");

        //    sbSQL.Append("{$Inserts}");

        //    sbSQL.Append("{$vtc}");


        //    sbSQL.Append("COLLECT STATS COLUMN(" + strVolatileColumns + ") ON " + strVolatileName + "; ");
        //    sbSQL.Append("{$vts}");

        //    sbSQL.Append("SELECT ");
        //    sbSQL.Append("mm.mhp_uni_id,  ");
        //    sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
        //    sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
        //    sbSQL.Append("NULL as LEG_ENTY_NBR,  ");
        //    sbSQL.Append("NULL as LEG_ENTY_FULL_NM,  ");
        //    sbSQL.Append("NULL as MKT_TYP_DESC,  ");
        //    sbSQL.Append("NULL as CUST_SEG_NBR,  ");
        //    sbSQL.Append("NULL as CUST_SEG_NM,  "); //ADD TO DB!!!!
        //    sbSQL.Append("i.PRDCT_CD,  ");
        //    sbSQL.Append("i.PRDCT_CD_DESC,  ");
        //    sbSQL.Append("NULL as MKT_SEG_DESC,  ");
        //    sbSQL.Append("NULL as MKT_SEG_RLLP_DESC,  ");
        //    sbSQL.Append("NULL as MKT_SEG_CD,  ");
        //    sbSQL.Append("NULL as FINC_ARNG_CD,  ");
        //    sbSQL.Append("NULL as FINC_ARNG_DESC,  ");
        //    sbSQL.Append("a.MBR_FST_NM, ");
        //    sbSQL.Append("a.MBR_LST_NM, ");
        //    sbSQL.Append("a.BTH_DT, ");
        //    sbSQL.Append("a.MBR_ALT_ID, ");
        //    sbSQL.Append("a.MBR_ID, ");
        //    sbSQL.Append("b.PRDCT_SYS_ID, ");
        //    sbSQL.Append("b.CS_PRDCT_CD_SYS_ID, ");
        //    sbSQL.Append("k.CS_CO_CD, ");
        //    sbSQL.Append("k.CS_CO_CD_ST, ");
        //    sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
        //    sbSQL.Append("FROM uhcdm001.hp_member a  ");
        //    sbSQL.Append("join uhcdm001.cs_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.cs_company_code k on b.CS_CO_CD_SYS_ID = k.CS_CO_CD_SYS_ID ");

        //    sbSQL.Append(strFilterJoin);
        //    sbSQL.Append("{$dvt}");
        //    sbSQL.Append("drop table " + strVolatileName + ";  ");



        //    return sbSQL.ToString();
        //}
        //private static string getUGAPSQLTemplate2(string strVolatileColumnsDeclare, string strVolatileColumns, string strVolatileName, string strFilterJoin)
        //{
        //    StringBuilder sbSQL = new StringBuilder();


        //    //   sbSQL.Append("drop table " + strVolatileName + "; ");

        //    sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + strVolatileName + "( ");

        //    sbSQL.Append(strVolatileColumnsDeclare);

        //    sbSQL.Append(") PRIMARY INDEX(" + strVolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

        //    sbSQL.Append("{$vti}");

        //    sbSQL.Append("{$Inserts}");

        //    sbSQL.Append("{$vtc}");


        //    sbSQL.Append("COLLECT STATS COLUMN(" + strVolatileColumns + ") ON " + strVolatileName + "; ");
        //    sbSQL.Append("{$vts}");

        //    sbSQL.Append("SELECT ");
        //    sbSQL.Append("mm.mhp_uni_id,  ");
        //    sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
        //    sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
        //    sbSQL.Append("e.LEG_ENTY_NBR,  ");
        //    sbSQL.Append("e.LEG_ENTY_FULL_NM,  ");
        //    sbSQL.Append("f.MKT_TYP_DESC,  ");
        //    sbSQL.Append("g.CUST_SEG_NBR,  ");
        //    sbSQL.Append("g.CUST_SEG_NM,  "); //ADD TO DB!!!!
        //    sbSQL.Append("i.PRDCT_CD,  ");
        //    sbSQL.Append("i.PRDCT_CD_DESC,  ");
        //    sbSQL.Append("j.MKT_SEG_DESC,  ");
        //    sbSQL.Append("j.MKT_SEG_RLLP_DESC,  ");
        //    sbSQL.Append("j.MKT_SEG_CD,  ");
        //    sbSQL.Append("k.FINC_ARNG_CD,  ");
        //    sbSQL.Append("k.FINC_ARNG_DESC,  ");
        //    sbSQL.Append("a.MBR_FST_NM, ");
        //    sbSQL.Append("a.MBR_LST_NM, ");
        //    sbSQL.Append("a.BTH_DT, ");
        //    sbSQL.Append("a.MBR_ALT_ID, ");
        //    sbSQL.Append("a.MBR_ID, ");
        //    sbSQL.Append("NULL as PRDCT_SYS_ID, ");
        //    sbSQL.Append("NULL as CS_PRDCT_CD_SYS_ID, ");
        //    sbSQL.Append("NULL as CS_CO_CD, ");
        //    sbSQL.Append("NULL as CS_CO_CD_ST, ");
        //    sbSQL.Append("a.SBSCR_MEDCD_RCIP_NBR ");
        //    sbSQL.Append("FROM uhcdm001.hp_member a  ");
        //    sbSQL.Append("join uhcdm001.hp_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.LEGAL_ENTITY e on b.LEG_ENTY_SYS_ID = e.LEG_ENTY_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.MARKET_TYPE_CODE f on b.MKT_TYP_CD_SYS_ID = f.MKT_TYP_CD_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.CUSTOMER_SEGMENT g on b.CUST_SEG_SYS_ID = g.CUST_SEG_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.GROUP_INDICATOR j on b.GRP_IND_SYS_ID = j.GRP_IND_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.company_code k on b.CO_CD_SYS_ID = k.CO_CD_SYS_ID  ");
        //    sbSQL.Append(strFilterJoin);
        //    sbSQL.Append("{$dvt}");
        //    sbSQL.Append("drop table " + strVolatileName + ";  ");



        //    return sbSQL.ToString();
        //}




        //private static string getUGAPSQLTemplate(string strVolatileColumnsDeclare, string strVolatileColumns, string strVolatileName)
        //{
        //    StringBuilder sbSQL = new StringBuilder();


        //    //   sbSQL.Append("drop table " + strVolatileName + "; ");

        //    sbSQL.Append("CREATE MULTISET VOLATILE TABLE " + strVolatileName + "( ");

        //    sbSQL.Append(strVolatileColumnsDeclare);

        //    sbSQL.Append(") PRIMARY INDEX(" + strVolatileColumns + ") ON COMMIT PRESERVE ROWS; ");

        //    sbSQL.Append("{$vti}");

        //    sbSQL.Append("{$Inserts}");

        //    sbSQL.Append("{$vtc}");


        //    sbSQL.Append("COLLECT STATS COLUMN(" + strVolatileColumns + ") ON " + strVolatileName + "; ");
        //    sbSQL.Append("{$vts}");

        //    sbSQL.Append("SELECT ");
        //    sbSQL.Append("mm.mhp_uni_id,  ");
        //    sbSQL.Append("b.BEN_STRCT_1_CD as PLN_VAR_SUBDIV_CD,  ");
        //    sbSQL.Append("c.eff_dt as mnth_eff_dt,  ");
        //    sbSQL.Append("e.LEG_ENTY_NBR,  ");
        //    sbSQL.Append("e.LEG_ENTY_FULL_NM,  ");
        //    sbSQL.Append("f.MKT_TYP_DESC,  ");
        //    sbSQL.Append("g.CUST_SEG_NBR,  ");
        //    sbSQL.Append("i.PRDCT_CD,  ");
        //    sbSQL.Append("i.PRDCT_CD_DESC,  ");
        //    sbSQL.Append("j.MKT_SEG_DESC,  ");
        //    sbSQL.Append("j.MKT_SEG_CODE,  ");
        //    sbSQL.Append("k.FINC_ARNG_CD,  ");
        //    sbSQL.Append("a.MBR_FST_NM, ");
        //    sbSQL.Append("a.MBR_LST_NM, ");
        //    sbSQL.Append("a.BTH_DT, ");
        //    sbSQL.Append("a.MBR_ALT_ID, ");
        //    sbSQL.Append("a.MBR_ID, ");
        //    sbSQL.Append("mm.Cardholder_ID  ");
        //    sbSQL.Append("FROM uhcdm001.hp_member a  ");
        //    sbSQL.Append("join uhcdm001.hp_demographics b on a.MBR_SYS_ID = b.MBR_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.date_eff c on b.DT_SYS_ID = c.EFF_DT_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.SOURCE_SYSTEM_COMBO d on b.SRC_SYS_COMBO_SYS_ID = d.SRC_SYS_COMBO_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.SUBSCRIBER_COUNTY h on b.SBSCR_CNTY_SYS_ID = h.SBSCR_CNTY_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.LEGAL_ENTITY e on b.LEG_ENTY_SYS_ID = e.LEG_ENTY_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.MARKET_TYPE_CODE f on b.MKT_TYP_CD_SYS_ID = f.MKT_TYP_CD_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.CUSTOMER_SEGMENT g on b.CUST_SEG_SYS_ID = g.CUST_SEG_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.PRODUCT i on b.PRDCT_SYS_ID = i.PRDCT_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.GROUP_INDICATOR j on b.GRP_IND_SYS_ID = j.GRP_IND_SYS_ID  ");
        //    sbSQL.Append("join uhcdm001.company_code k on b.CO_CD_SYS_ID = k.CO_CD_SYS_ID  ");
        //    sbSQL.Append("inner join " + strVolatileName + " as mm on a.MBR_FST_NM LIKE mm.MBR_FST_NM AND  a.MBR_LST_NM LIKE mm.MBR_LST_NM AND a.BTH_DT = mm.BTH_DT AND mm.REQ_DT BETWEEN c.eff_dt AND LAST_DAY(c.eff_dt);  ");
        //    sbSQL.Append("{$dvt}");
        //    sbSQL.Append("drop table " + strVolatileName + ";  ");




        //    return sbSQL.ToString();
        //}



        //private static bool getUniversesFinal()
        //{

        //    bool blUpdated = false;

        //    Console.WriteLine("MHP Monthly Universes Parser");
        //    string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

        //    int intRowCnt = 1;
        //    int intFileCnt = 1;
        //    int intSheetCnt = 1;
        //    //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
        //    Console.WriteLine();
        //    Console.WriteLine("Processing spreadsheets");



        //    string strFileName = null;
        //    string strFilePath = @"\\msp09fil01\Radiology\Vendor Management - Delegation Oversight\Mental Health Parity (MHP)\MHP Monthly Universes - 2022";
        //    strFileFolderPath = @"C:\Users\cgiorda\Desktop\MHP_2021";
        //    string strDate = "12/21/2022";

        //    string strSheetname = null;
        //    string strTableName = "stg.MHP_Yearly_Universes_2021_v2";
        //    //string[] files;
        //    List<string> sheets;
        //    List<string> files;

        //    DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from " + strTableName);
        //    DataTable dtCurrentDataTable = null;
        //    DataTable dtFinalDataTable = null;
        //    OleDbDataReader dr = null;
        //    DataRow currentRow;
        //    List<string> columns;
        //    DateTime temp;
        //    TimeSpan tempTS;


        //    List<string> strCol = new List<string>();
        //    strCol.Add("State_of_Issue");
        //    strCol.Add("State_of_Residence");
        //    strCol.Add("Enrollee_First_Name");
        //    strCol.Add("Enrollee_Last_Name");
        //    strCol.Add("Cardholder_ID");
        //    strCol.Add("Funding_Arrangement");
        //    strCol.Add("Authorization");
        //    strCol.Add("Authorization_Type");
        //    strCol.Add("Request_Date");
        //    strCol.Add("Request_Time");
        //    strCol.Add("Request_Decision");
        //    strCol.Add("Decision_Date");
        //    strCol.Add("Decision_Time");
        //    strCol.Add("Decision_Reason");
        //    strCol.Add("Extension_Taken");
        //    strCol.Add("Member_Notif_Extension_Date");
        //    strCol.Add("Additional_Info_Date");
        //    strCol.Add("Oral_Notification_Enrollee_Date");
        //    strCol.Add("Oral_Notification_Enrollee_Time");
        //    strCol.Add("Oral_Notification_Provider_Date");
        //    strCol.Add("Oral_Notification_Provider_Time");
        //    strCol.Add("Written_Notification_Enrollee_Date");
        //    strCol.Add("Written_Notification_Enrollee_Time");
        //    strCol.Add("Written_Notification_Provider_Date");
        //    strCol.Add("Written_Notification_Provider_Time");
        //    strCol.Add("Primary_Procedure_Code_Req");
        //    strCol.Add("Procedure_Code_Description");
        //    strCol.Add("Primary_Diagnosis_Code");
        //    strCol.Add("Diagnosis_Code_Description");
        //    strCol.Add("Place_of_Service");
        //    strCol.Add("Member_Date_of_Birth");
        //    strCol.Add("Urgent_Processed_Standard");
        //    strCol.Add("Request_Additional_Info_Date");
        //    strCol.Add("FirstTier_Downstream_RelatedEntity");
        //    strCol.Add("Par_NonPar_Site");
        //    strCol.Add("Inpatient_Outpatient");
        //    strCol.Add("Delegate_Number");
        //    strCol.Add("ProgramType");
        //    strCol.Add("Insurance_Carrier");
        //    strCol.Add("Group_Number");

        //    dtFinalDataTable = new DataTable();
        //    dtFinalDataTable.Columns.Add("State_of_Issue", typeof(String));
        //    dtFinalDataTable.Columns.Add("State_of_Residence", typeof(String));
        //    dtFinalDataTable.Columns.Add("Enrollee_First_Name", typeof(String));
        //    dtFinalDataTable.Columns.Add("Enrollee_Last_Name", typeof(String));
        //    dtFinalDataTable.Columns.Add("Cardholder_ID", typeof(String));
        //    dtFinalDataTable.Columns.Add("Funding_Arrangement", typeof(String));
        //    dtFinalDataTable.Columns.Add("Authorization", typeof(String));
        //    dtFinalDataTable.Columns.Add("Authorization_Type", typeof(String));
        //    dtFinalDataTable.Columns.Add("Request_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Request_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Request_Decision", typeof(String));
        //    dtFinalDataTable.Columns.Add("Decision_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Decision_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Decision_Reason", typeof(String));
        //    dtFinalDataTable.Columns.Add("Extension_Taken", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Member_Notif_Extension_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Additional_Info_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Primary_Procedure_Code_Req", typeof(String));
        //    dtFinalDataTable.Columns.Add("Procedure_Code_Description", typeof(String));
        //    dtFinalDataTable.Columns.Add("Primary_Diagnosis_Code", typeof(String));
        //    dtFinalDataTable.Columns.Add("Diagnosis_Code_Description", typeof(String));
        //    dtFinalDataTable.Columns.Add("Place_of_Service", typeof(Int16));
        //    dtFinalDataTable.Columns.Add("Member_Date_of_Birth", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Urgent_Processed_Standard", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Request_Additional_Info_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("FirstTier_Downstream_RelatedEntity", typeof(String));
        //    dtFinalDataTable.Columns.Add("Par_NonPar_Site", typeof(String));
        //    dtFinalDataTable.Columns.Add("Inpatient_Outpatient", typeof(String));
        //    dtFinalDataTable.Columns.Add("Delegate_Number", typeof(Int32));
        //    dtFinalDataTable.Columns.Add("ProgramType", typeof(String));
        //    dtFinalDataTable.Columns.Add("Insurance_Carrier", typeof(String));
        //    dtFinalDataTable.Columns.Add("Group_Number", typeof(String));

        //    //dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
        //    //dtFinalDataTable.Columns.Add("file_name", typeof(String));
        //    //dtFinalDataTable.Columns.Add("file_path", typeof(String));
        //    //dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

        //    dtFinalDataTable.TableName = strTableName;


        //    //files = Directory.EnumerateFiles(strFileFolderPath, "*.xls*", SearchOption.TopDirectoryOnly).ToList();
        //    files = new List<string>();
        //    files.Add(strFileFolderPath + "\\C&S RAD CY2021.xlsx");

        //    // bool blFoundSheet = false;

        //    intFileCnt = 1;
        //    foreach (string strFile in files)
        //    {
        //        Console.Write("Processing :" + intFileCnt + " out of " + files.Count + " : " + strFile);


        //        Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");

        //        strFileName = Path.GetFileName(strFile);

        //        if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
        //        {
        //            intFileCnt++;
        //            continue;
        //        }




        //        intSheetCnt = 1;

        //        var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
        //        intSheetCnt = 1;
        //        foreach (Sheet item in results)
        //        {

        //            strSheetname = item.Name.ToString();

        //            Console.Write("\rProcessing " + String.Format("{0:n0}", intSheetCnt) + " out of " + String.Format("{0:n0}", results.Count()) + " spreadsheets");


        //            try
        //            {
        //                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
        //                //dr = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataReader(strFile, strSheetname);
        //            }
        //            catch (Exception ex)
        //            {
        //                SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
        //                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
        //            }

        //            columns = new List<string>();
        //            for (int i = 0; i < dr.FieldCount; i++)
        //            {
        //                columns.Add(dr.GetName(i));
        //            }

        //            //columns = dtCurrentDataTable.Columns;
        //            Console.Write("\rFile to DataTable");
        //            // strSummaryofLOB = strFolder.Split('_')[1];

        //            intRowCnt = 1;
        //            while (dr.Read())
        //            {

        //                if (dr.GetValue(dr.GetOrdinal("Authorization")) == DBNull.Value)
        //                    continue;


        //                //Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

        //                currentRow = dtFinalDataTable.NewRow();

        //                if (columns.Contains("State of Issue"))
        //                    currentRow["State_of_Issue"] = (dr.GetValue(dr.GetOrdinal("State of Issue")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("State of Issue")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("State of Issue")) : (object)DBNull.Value);

        //                if (columns.Contains("State of Residence"))
        //                    currentRow["State_of_Residence"] = (dr.GetValue(dr.GetOrdinal("State of Residence")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("State of Residence")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("State of Residence")) : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee First Name"))
        //                    currentRow["Enrollee_First_Name"] = (dr.GetValue(dr.GetOrdinal("Enrollee First Name")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Enrollee First Name")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Enrollee First Name")) : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee Last Name"))
        //                    currentRow["Enrollee_Last_Name"] = (dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Enrollee Last Name")) : (object)DBNull.Value);

        //                if (columns.Contains("Cardholder ID"))
        //                    currentRow["Cardholder_ID"] = (dr.GetValue(dr.GetOrdinal("Cardholder ID")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Cardholder ID")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Cardholder ID")) : (object)DBNull.Value);

        //                if (columns.Contains("Funding Arrangement"))
        //                    currentRow["Funding_Arrangement"] = (dr.GetValue(dr.GetOrdinal("Funding Arrangement")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Funding Arrangement")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Funding Arrangement")) : (object)DBNull.Value);

        //                if (columns.Contains("Authorization"))
        //                    currentRow["Authorization"] = (dr.GetValue(dr.GetOrdinal("Authorization")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Authorization")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Authorization")) : (object)DBNull.Value);

        //                if (columns.Contains("Authorization Type"))
        //                    currentRow["Authorization_Type"] = (dr.GetValue(dr.GetOrdinal("Authorization Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Authorization Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Authorization Type")) : (object)DBNull.Value);

        //                if (columns.Contains("Date the request was received"))
        //                    currentRow["Request_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date the request was received")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date the request was received")) : (object)DBNull.Value);

        //                if (columns.Contains("Time the request was received"))
        //                    currentRow["Request_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time the request was received")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time the request was received")) : (object)DBNull.Value);

        //                if (columns.Contains("Request decision") || columns.Contains("Request Decision"))
        //                    currentRow["Request_Decision"] = (dr.GetValue(dr.GetOrdinal("Request decision")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Request decision")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Request decision")) : (object)DBNull.Value);

        //                if (columns.Contains("Date of decision") || columns.Contains("Date of Decision"))
        //                    currentRow["Decision_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of decision")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of decision")) : (object)DBNull.Value);

        //                if (columns.Contains("Time of decision") || columns.Contains("Time of Decision"))
        //                    currentRow["Decision_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time of decision")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time of decision")) : (object)DBNull.Value);

        //                if (columns.Contains("Decision Reason") || columns.Contains("Decision reason"))
        //                    currentRow["Decision_Reason"] = (dr.GetValue(dr.GetOrdinal("Decision Reason")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Decision Reason")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Decision Reason")) : (object)DBNull.Value);
        //                else if (columns.Contains("Denial Type") || columns.Contains("Denial type"))
        //                    currentRow["Decision_Reason"] = (dr.GetValue(dr.GetOrdinal("Denial Type")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Denial Type")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Denial Type")) : (object)DBNull.Value);

        //                if (columns.Contains("Was Extension Taken ? "))
        //                    currentRow["Extension_Taken"] = (!(dr.GetValue(dr.GetOrdinal("Was Extension Taken?")) + "").Trim().Equals("") ? ((dr.GetValue(dr.GetOrdinal("Was Extension Taken?")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);
        //                else if (columns.Contains("Was Extension Taken"))
        //                    currentRow["Extension_Taken"] = (!(dr.GetValue(dr.GetOrdinal("Was Extension Taken")) + "").Trim().Equals("") ? ((dr.GetValue(dr.GetOrdinal("Was Extension Taken")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of member notification of extension"))
        //                    currentRow["Member_Notif_Extension_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of member notification of extension")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of member notification of extension")) : (object)DBNull.Value);

        //                if (columns.Contains("Date additional information received"))
        //                    currentRow["Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date additional information received")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date additional information received")) : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date oral notification provided to enrollee")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date oral notification provided to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time oral notification provided to enrollee")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time oral notification provided to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date oral notification provided to provider")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date oral notification provided to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time oral notification provided to provider")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time oral notification provided to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date written notification sent to enrollee")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date written notification sent to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time written notification sent to enrollee")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time written notification sent to enrollee")) : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date written notification sent to provider")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date written notification sent to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Time"] = (TimeSpan.TryParse(dr.GetValue(dr.GetOrdinal("Time written notification sent to provider")) + "", out tempTS) ? dr.GetValue(dr.GetOrdinal("Time written notification sent to provider")) : (object)DBNull.Value);

        //                if (columns.Contains("Primary Procedure Code(s) Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Procedure Code(s) Requested")) : (object)DBNull.Value);


        //                if (columns.Contains("Primary Procedure Code Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Procedure Code Requested")) : (object)DBNull.Value);

        //                if (columns.Contains("Procedure Code Description"))
        //                    currentRow["Procedure_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Procedure Code Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Procedure Code Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Procedure Code Description")) : (object)DBNull.Value);

        //                if (columns.Contains("Primary Diagnosis Code"))
        //                    currentRow["Primary_Diagnosis_Code"] = (dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Primary Diagnosis Code")) : (object)DBNull.Value);

        //                if (columns.Contains("Diagnosis Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Diagnosis Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Diagnosis Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Diagnosis Description")) : (object)DBNull.Value);
        //                else if (columns.Contains("Diagnosis Code Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Diagnosis Code Description")) : (object)DBNull.Value);

        //                if (columns.Contains("Place of Service"))
        //                    currentRow["Place_of_Service"] = (dr.GetValue(dr.GetOrdinal("Place of Service")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Place of Service")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Place of Service")) : (object)DBNull.Value);

        //                if (columns.Contains("Member Date of Birth"))
        //                    currentRow["Member_Date_of_Birth"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Member Date of Birth")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Member Date of Birth")) : (object)DBNull.Value);

        //                if (columns.Contains("Was an urgent request made but processed as standard?"))
        //                    currentRow["Urgent_Processed_Standard"] = (!(dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("") && !(dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("NA") ? ((dr.GetValue(dr.GetOrdinal("Was an urgent request made but processed as standard?")) + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of request for additional information"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date of request for additional information")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date of request for additional information")) : (object)DBNull.Value);
        //                else if (columns.Contains("Date additional information requested"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(dr.GetValue(dr.GetOrdinal("Date additional information requested")) + "", out temp) ? dr.GetValue(dr.GetOrdinal("Date additional information requested")) : (object)DBNull.Value);

        //                if (columns.Contains("First Tier, Downstream, and Related Entity"))
        //                    currentRow["FirstTier_Downstream_RelatedEntity"] = (dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("First Tier, Downstream, and Related Entity")) : (object)DBNull.Value);

        //                if (columns.Contains("Delegate Number"))
        //                    currentRow["Delegate_Number"] = (dr.GetValue(dr.GetOrdinal("Delegate Number")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Delegate Number")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Delegate Number")) : (object)DBNull.Value);

        //                if (columns.Contains("Par/Non-Par Site"))
        //                    currentRow["Par_NonPar_Site"] = (dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Par/Non-Par Site")) : (object)DBNull.Value);

        //                if (columns.Contains("Inpatient/Outpatient"))
        //                    currentRow["Inpatient_Outpatient"] = (dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Inpatient/Outpatient")) : (object)DBNull.Value);



        //                if (columns.Contains("ProgramType"))
        //                    currentRow["ProgramType"] = (dr.GetValue(dr.GetOrdinal("ProgramType")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("ProgramType")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("ProgramType")) : (object)DBNull.Value);


        //                if (columns.Contains("Insurance Carrier"))
        //                    currentRow["Insurance_Carrier"] = (dr.GetValue(dr.GetOrdinal("Insurance Carrier")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Insurance Carrier")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Insurance Carrier")) : (object)DBNull.Value);


        //                if (columns.Contains("Group Number"))
        //                    currentRow["Group_Number"] = (dr.GetValue(dr.GetOrdinal("Group Number")) != DBNull.Value && !(dr.GetValue(dr.GetOrdinal("Group Number")) + "").Trim().Equals("") ? dr.GetValue(dr.GetOrdinal("Group Number")) : (object)DBNull.Value);



        //                //currentRow["file_date"] = DateTime.Parse(strDate);
        //                //currentRow["sheet_name"] = strSheetname;
        //                //currentRow["file_name"] = strFileName;
        //                //currentRow["file_path"] = strFilePath;
        //                dtFinalDataTable.Rows.Add(currentRow);
        //                intRowCnt++;
        //            }
        //            dr.Close();
        //            currentRow = null;
        //            dtCurrentDataTable = null;










        //            dtFinalDataTable = dtFinalDataTable.DefaultView.ToTable(true, strCol.ToArray());
        //            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
        //            dtFinalDataTable.Columns.Add("file_name", typeof(String));
        //            dtFinalDataTable.Columns.Add("file_path", typeof(String));
        //            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

        //            dtFinalDataTable.Columns["file_date"].Expression = strDate;
        //            dtFinalDataTable.Columns["sheet_name"].Expression = strSheetname;
        //            dtFinalDataTable.Columns["file_name"].Expression = strFileName;
        //            dtFinalDataTable.Columns["file_path"].Expression = strFilePath;

        //            //MAKE dtFinalDataTable = dtFinalDataTable.DISTINCT
        //            //THEN ADD TO ALL ROWS
        //            //currentRow["file_date"] = DateTime.Parse(strDate);
        //            //currentRow["sheet_name"] = strSheetname;
        //            //currentRow["file_name"] = strFileName;
        //            //currentRow["file_path"] = strFilePath;





        //            if (dtFinalDataTable.Rows.Count > 0)
        //            {
        //                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

        //                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

        //                blUpdated = true;
        //            }
        //            dtFinalDataTable.Clear();
        //            intSheetCnt++;

        //        }

        //        currentRow = null;
        //        dtCurrentDataTable = null;
        //        File.Move(strFile, @"C:\Users\cgiorda\Desktop\MHP_2021\archive\" + strFileName);
        //        intFileCnt++;
        //    }


        //    return blUpdated;
        //}




        //private static bool getUniversesFinal()
        //{

        //    bool blUpdated = false;

        //    Console.WriteLine("MHP Monthly Universes Parser");
        //    string strFileFolderPath = ConfigurationManager.AppSettings["File_Path"];
        //    string strILUCAConnectionString = ConfigurationManager.AppSettings["ILUCA"];

        //    int intRowCnt = 1;
        //    int intFileCnt = 1;
        //    int intSheetCnt = 1;
        //    //string[] strFoldersArr = new string[] { "Monthly_CnS", "Monthly_EnI", "Monthly_MnR" };
        //    Console.WriteLine();
        //    Console.WriteLine("Processing spreadsheets");



        //    string strFileName = null;
        //    string strFilePath = @"\\msp09fil01\Radiology\Vendor Management - Delegation Oversight\Mental Health Parity (MHP)\MHP Monthly Universes - 2022";
        //    strFileFolderPath = @"C:\Users\cgiorda\Desktop\MHP_2021";
        //    string strDate = "12/21/2022";

        //    string strSheetname = null;
        //    string strTableName = "stg.MHP_Yearly_Universes_2021_v2";
        //    //string[] files;
        //    List<string> sheets;
        //    List<string> files;

        //    DataTable dtFilesCaptured = DBConnection64.getMSSQLDataTable(strILUCAConnectionString, "select distinct [file_name] from " + strTableName);
        //    DataTable dtCurrentDataTable = null;
        //    DataTable dtFinalDataTable = null;
        //    OleDbDataReader dr = null;
        //    DataRow currentRow;
        //    List<string> columns;
        //    DateTime temp;
        //    TimeSpan tempTS;


        //    List<string> strCol = new List<string>();
        //    strCol.Add("State_of_Issue");
        //    strCol.Add("State_of_Residence");
        //    strCol.Add("Enrollee_First_Name");
        //    strCol.Add("Enrollee_Last_Name");
        //    strCol.Add("Cardholder_ID");
        //    strCol.Add("Funding_Arrangement");
        //    strCol.Add("Authorization");
        //    strCol.Add("Authorization_Type");
        //    strCol.Add("Request_Date");
        //    strCol.Add("Request_Time");
        //    strCol.Add("Request_Decision");
        //    strCol.Add("Decision_Date");
        //    strCol.Add("Decision_Time");
        //    strCol.Add("Decision_Reason");
        //    strCol.Add("Extension_Taken");
        //    strCol.Add("Member_Notif_Extension_Date");
        //    strCol.Add("Additional_Info_Date");
        //    strCol.Add("Oral_Notification_Enrollee_Date");
        //    strCol.Add("Oral_Notification_Enrollee_Time");
        //    strCol.Add("Oral_Notification_Provider_Date");
        //    strCol.Add("Oral_Notification_Provider_Time");
        //    strCol.Add("Written_Notification_Enrollee_Date");
        //    strCol.Add("Written_Notification_Enrollee_Time");
        //    strCol.Add("Written_Notification_Provider_Date");
        //    strCol.Add("Written_Notification_Provider_Time");
        //    strCol.Add("Primary_Procedure_Code_Req");
        //    strCol.Add("Procedure_Code_Description");
        //    strCol.Add("Primary_Diagnosis_Code");
        //    strCol.Add("Diagnosis_Code_Description");
        //    strCol.Add("Place_of_Service");
        //    strCol.Add("Member_Date_of_Birth");
        //    strCol.Add("Urgent_Processed_Standard");
        //    strCol.Add("Request_Additional_Info_Date");
        //    strCol.Add("FirstTier_Downstream_RelatedEntity");
        //    strCol.Add("Par_NonPar_Site");
        //    strCol.Add("Inpatient_Outpatient");
        //    strCol.Add("Delegate_Number");
        //    strCol.Add("ProgramType");
        //    strCol.Add("Insurance_Carrier");
        //    strCol.Add("Group_Number");

        //    dtFinalDataTable = new DataTable();
        //    dtFinalDataTable.Columns.Add("State_of_Issue", typeof(String));
        //    dtFinalDataTable.Columns.Add("State_of_Residence", typeof(String));
        //    dtFinalDataTable.Columns.Add("Enrollee_First_Name", typeof(String));
        //    dtFinalDataTable.Columns.Add("Enrollee_Last_Name", typeof(String));
        //    dtFinalDataTable.Columns.Add("Cardholder_ID", typeof(String));
        //    dtFinalDataTable.Columns.Add("Funding_Arrangement", typeof(String));
        //    dtFinalDataTable.Columns.Add("Authorization", typeof(String));
        //    dtFinalDataTable.Columns.Add("Authorization_Type", typeof(String));
        //    dtFinalDataTable.Columns.Add("Request_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Request_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Request_Decision", typeof(String));
        //    dtFinalDataTable.Columns.Add("Decision_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Decision_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Decision_Reason", typeof(String));
        //    dtFinalDataTable.Columns.Add("Extension_Taken", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Member_Notif_Extension_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Additional_Info_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Enrollee_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Oral_Notification_Provider_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Enrollee_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Written_Notification_Provider_Time", typeof(TimeSpan));
        //    dtFinalDataTable.Columns.Add("Primary_Procedure_Code_Req", typeof(String));
        //    dtFinalDataTable.Columns.Add("Procedure_Code_Description", typeof(String));
        //    dtFinalDataTable.Columns.Add("Primary_Diagnosis_Code", typeof(String));
        //    dtFinalDataTable.Columns.Add("Diagnosis_Code_Description", typeof(String));
        //    dtFinalDataTable.Columns.Add("Place_of_Service", typeof(Int16));
        //    dtFinalDataTable.Columns.Add("Member_Date_of_Birth", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("Urgent_Processed_Standard", typeof(Boolean));
        //    dtFinalDataTable.Columns.Add("Request_Additional_Info_Date", typeof(DateTime));
        //    dtFinalDataTable.Columns.Add("FirstTier_Downstream_RelatedEntity", typeof(String));
        //    dtFinalDataTable.Columns.Add("Par_NonPar_Site", typeof(String));
        //    dtFinalDataTable.Columns.Add("Inpatient_Outpatient", typeof(String));
        //    dtFinalDataTable.Columns.Add("Delegate_Number", typeof(Int32));
        //    dtFinalDataTable.Columns.Add("ProgramType", typeof(String));
        //    dtFinalDataTable.Columns.Add("Insurance_Carrier", typeof(String));
        //    dtFinalDataTable.Columns.Add("Group_Number", typeof(String));

        //    //dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
        //    //dtFinalDataTable.Columns.Add("file_name", typeof(String));
        //    //dtFinalDataTable.Columns.Add("file_path", typeof(String));
        //    //dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

        //    dtFinalDataTable.TableName = strTableName;


        //    //files = Directory.EnumerateFiles(strFileFolderPath, "*.xls*", SearchOption.TopDirectoryOnly).ToList();
        //    files = new List<string>();
        //    files.Add(strFileFolderPath + "\\C&S RAD CY2021.xlsx");

        //    // bool blFoundSheet = false;

        //    intFileCnt = 1;
        //    foreach (string strFile in files)
        //    {
        //        Console.Write("Processing :" + intFileCnt + " out of " + files.Count + " : " + strFile);


        //        Console.Write("\rProcessing " + String.Format("{0:n0}", intFileCnt) + " out of " + String.Format("{0:n0}", files.Count) + " spreadsheets");

        //        strFileName = Path.GetFileName(strFile);

        //        if (strFileName.StartsWith("~") || dtFilesCaptured.Select("[file_name]='" + strFileName + "'").Count() > 0)
        //        {
        //            intFileCnt++;
        //            continue;
        //        }




        //        intSheetCnt = 1;

        //        var results = OpenXMLExcel.OpenXMLExcel.GetAllWorksheets(strFile);
        //        intSheetCnt = 1;
        //        foreach (Sheet item in results)
        //        {

        //            strSheetname = item.Name.ToString();

        //            Console.Write("\rProcessing " + String.Format("{0:n0}", intSheetCnt) + " out of " + String.Format("{0:n0}", results.Count()) + " spreadsheets");


        //            try
        //            {
        //                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataTable(strFile, strSheetname);
        //                //dr = OpenXMLExcel.OpenXMLExcel.ConvertExcelToDataReader(strFile, strSheetname);
        //            }
        //            catch (Exception ex)
        //            {
        //                SpreadsheetDocument wbCurrentExcelFile = SpreadsheetDocument.Open(strFile, true);
        //                dtCurrentDataTable = OpenXMLExcel.OpenXMLExcel.ReadAsDataTable(wbCurrentExcelFile, strSheetname, 1, 2);
        //            }

        //            columns = new List<string>();
        //            for (int i = 0; i < dr.FieldCount; i++)
        //            {
        //                columns.Add(dr.GetName(i));
        //            }

        //            //columns = dtCurrentDataTable.Columns;
        //            Console.Write("\rFile to DataTable");
        //            // strSummaryofLOB = strFolder.Split('_')[1];



        //            intRowCnt = 1;
        //            for (int i = 0; i <= dtCurrentDataTable.Rows.Count; i++)
        //            {
        //                DataRow d = dtCurrentDataTable.Rows[i];


        //                if (d["Authorization"] == DBNull.Value)
        //                    continue;

        //                Console.Write("\rProcessing " + String.Format("{0:n0}", intRowCnt) + " out of " + String.Format("{0:n0}", dtCurrentDataTable.Rows.Count) + " rows");

        //                currentRow = dtFinalDataTable.NewRow();


        //                if (columns.Contains("State of Issue"))
        //                    currentRow["State_of_Issue"] = (d["State of Issue"] != DBNull.Value && !(d["State of Issue"] + "").Trim().Equals("") ? d["State of Issue"] : (object)DBNull.Value);

        //                if (columns.Contains("State of Residence"))
        //                    currentRow["State_of_Residence"] = (d["State of Residence"] != DBNull.Value && !(d["State of Residence"] + "").Trim().Equals("") ? d["State of Residence"] : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee First Name"))
        //                    currentRow["Enrollee_First_Name"] = (d["Enrollee First Name"] != DBNull.Value && !(d["Enrollee First Name"] + "").Trim().Equals("") ? d["Enrollee First Name"] : (object)DBNull.Value);

        //                if (columns.Contains("Enrollee Last Name"))
        //                    currentRow["Enrollee_Last_Name"] = (d["Enrollee Last Name"] != DBNull.Value && !(d["Enrollee Last Name"] + "").Trim().Equals("") ? d["Enrollee Last Name"] : (object)DBNull.Value);

        //                if (columns.Contains("Cardholder ID"))
        //                    currentRow["Cardholder_ID"] = (d["Cardholder ID"] != DBNull.Value && !(d["Cardholder ID"] + "").Trim().Equals("") ? d["Cardholder ID"] : (object)DBNull.Value);

        //                if (columns.Contains("Funding Arrangement"))
        //                    currentRow["Funding_Arrangement"] = (d["Funding Arrangement"] != DBNull.Value && !(d["Funding Arrangement"] + "").Trim().Equals("") ? d["Funding Arrangement"] : (object)DBNull.Value);

        //                if (columns.Contains("Authorization"))
        //                    currentRow["Authorization"] = (d["Authorization"] != DBNull.Value && !(d["Authorization"] + "").Trim().Equals("") ? d["Authorization"] : (object)DBNull.Value);

        //                if (columns.Contains("Authorization Type"))
        //                    currentRow["Authorization_Type"] = (d["Authorization Type"] != DBNull.Value && !(d["Authorization Type"] + "").Trim().Equals("") ? d["Authorization Type"] : (object)DBNull.Value);

        //                if (columns.Contains("Date the request was received"))
        //                    currentRow["Request_Date"] = (DateTime.TryParse(d["Date the request was received"] + "", out temp) ? d["Date the request was received"] : (object)DBNull.Value);

        //                if (columns.Contains("Time the request was received"))
        //                    currentRow["Request_Time"] = (TimeSpan.TryParse(d["Time the request was received"] + "", out tempTS) ? d["Time the request was received"] : (object)DBNull.Value);

        //                if (columns.Contains("Request decision") || columns.Contains("Request Decision"))
        //                    currentRow["Request_Decision"] = (d["Request decision"] != DBNull.Value && !(d["Request decision"] + "").Trim().Equals("") ? d["Request decision"] : (object)DBNull.Value);

        //                if (columns.Contains("Date of decision") || columns.Contains("Date of Decision"))
        //                    currentRow["Decision_Date"] = (DateTime.TryParse(d["Date of decision"] + "", out temp) ? d["Date of decision"] : (object)DBNull.Value);

        //                if (columns.Contains("Time of decision") || columns.Contains("Time of Decision"))
        //                    currentRow["Decision_Time"] = (TimeSpan.TryParse(d["Time of decision"] + "", out tempTS) ? d["Time of decision"] : (object)DBNull.Value);

        //                if (columns.Contains("Decision Reason") || columns.Contains("Decision reason"))
        //                    currentRow["Decision_Reason"] = (d["Decision Reason"] != DBNull.Value && !(d["Decision Reason"] + "").Trim().Equals("") ? d["Decision Reason"] : (object)DBNull.Value);
        //                else if (columns.Contains("Denial Type") || columns.Contains("Denial type"))
        //                    currentRow["Decision_Reason"] = (d["Denial Type"] != DBNull.Value && !(d["Denial Type"] + "").Trim().Equals("") ? d["Denial Type"] : (object)DBNull.Value);

        //                if (columns.Contains("Was Extension Taken ? "))
        //                    currentRow["Extension_Taken"] = (!(d["Was Extension Taken?"] + "").Trim().Equals("") ? ((d["Was Extension Taken?"] + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);
        //                else if (columns.Contains("Was Extension Taken"))
        //                    currentRow["Extension_Taken"] = (!(d["Was Extension Taken"] + "").Trim().Equals("") ? ((d["Was Extension Taken"] + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of member notification of extension"))
        //                    currentRow["Member_Notif_Extension_Date"] = (DateTime.TryParse(d["Date of member notification of extension"] + "", out temp) ? d["Date of member notification of extension"] : (object)DBNull.Value);

        //                if (columns.Contains("Date additional information received"))
        //                    currentRow["Additional_Info_Date"] = (DateTime.TryParse(d["Date additional information received"] + "", out temp) ? d["Date additional information received"] : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Date"] = (DateTime.TryParse(d["Date oral notification provided to enrollee"] + "", out temp) ? d["Date oral notification provided to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to enrollee"))
        //                    currentRow["Oral_Notification_Enrollee_Time"] = (TimeSpan.TryParse(d["Time oral notification provided to enrollee"] + "", out tempTS) ? d["Time oral notification provided to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Date oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Date"] = (DateTime.TryParse(d["Date oral notification provided to provider"] + "", out temp) ? d["Date oral notification provided to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Time oral notification provided to provider"))
        //                    currentRow["Oral_Notification_Provider_Time"] = (TimeSpan.TryParse(d["Time oral notification provided to provider"] + "", out tempTS) ? d["Time oral notification provided to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Date"] = (DateTime.TryParse(d["Date written notification sent to enrollee"] + "", out temp) ? d["Date written notification sent to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to enrollee"))
        //                    currentRow["Written_Notification_Enrollee_Time"] = (TimeSpan.TryParse(d["Time written notification sent to enrollee"] + "", out tempTS) ? d["Time written notification sent to enrollee"] : (object)DBNull.Value);

        //                if (columns.Contains("Date written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Date"] = (DateTime.TryParse(d["Date written notification sent to provider"] + "", out temp) ? d["Date written notification sent to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Time written notification sent to provider"))
        //                    currentRow["Written_Notification_Provider_Time"] = (TimeSpan.TryParse(d["Time written notification sent to provider"] + "", out tempTS) ? d["Time written notification sent to provider"] : (object)DBNull.Value);

        //                if (columns.Contains("Primary Procedure Code(s) Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (d["Primary Procedure Code(s) Requested"] != DBNull.Value && !(d["Primary Procedure Code(s) Requested"] + "").Trim().Equals("") ? d["Primary Procedure Code(s) Requested"] : (object)DBNull.Value);


        //                if (columns.Contains("Primary Procedure Code Requested"))
        //                    currentRow["Primary_Procedure_Code_Req"] = (d["Primary Procedure Code Requested"] != DBNull.Value && !(d["Primary Procedure Code Requested"] + "").Trim().Equals("") ? d["Primary Procedure Code Requested"] : (object)DBNull.Value);

        //                if (columns.Contains("Procedure Code Description"))
        //                    currentRow["Procedure_Code_Description"] = (d["Procedure Code Description"] != DBNull.Value && !(d["Procedure Code Description"] + "").Trim().Equals("") ? d["Procedure Code Description"] : (object)DBNull.Value);

        //                if (columns.Contains("Primary Diagnosis Code"))
        //                    currentRow["Primary_Diagnosis_Code"] = (d["Primary Diagnosis Code"] != DBNull.Value && !(d["Primary Diagnosis Code"] + "").Trim().Equals("") ? d["Primary Diagnosis Code"] : (object)DBNull.Value);

        //                if (columns.Contains("Diagnosis Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (d["Diagnosis Description"] != DBNull.Value && !(d["Diagnosis Description"] + "").Trim().Equals("") ? d["Diagnosis Description"] : (object)DBNull.Value);
        //                else if (columns.Contains("Diagnosis Code Description"))
        //                    currentRow["Diagnosis_Code_Description"] = (d["Diagnosis Code Description"] != DBNull.Value && !(d["Diagnosis Code Description"] + "").Trim().Equals("") ? d["Diagnosis Code Description"] : (object)DBNull.Value);

        //                if (columns.Contains("Place of Service"))
        //                    currentRow["Place_of_Service"] = (d["Place of Service"] != DBNull.Value && !(d["Place of Service"] + "").Trim().Equals("") ? d["Place of Service"] : (object)DBNull.Value);

        //                if (columns.Contains("Member Date of Birth"))
        //                    currentRow["Member_Date_of_Birth"] = (DateTime.TryParse(d["Member Date of Birth"] + "", out temp) ? d["Member Date of Birth"] : (object)DBNull.Value);

        //                if (columns.Contains("Was an urgent request made but processed as standard?"))
        //                    currentRow["Urgent_Processed_Standard"] = (!(d["Was an urgent request made but processed as standard?"] + "").Trim().Equals("") && !(d["Was an urgent request made but processed as standard?"] + "").Trim().Equals("NA") ? ((d["Was an urgent request made but processed as standard?"] + "").Trim().Equals("Y") ? true : false) : (object)DBNull.Value);

        //                if (columns.Contains("Date of request for additional information"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(d["Date of request for additional information"] + "", out temp) ? d["Date of request for additional information"] : (object)DBNull.Value);
        //                else if (columns.Contains("Date additional information requested"))
        //                    currentRow["Request_Additional_Info_Date"] = (DateTime.TryParse(d["Date additional information requested"] + "", out temp) ? d["Date additional information requested"] : (object)DBNull.Value);

        //                if (columns.Contains("First Tier, Downstream, and Related Entity"))
        //                    currentRow["FirstTier_Downstream_RelatedEntity"] = (d["First Tier, Downstream, and Related Entity"] != DBNull.Value && !(d["First Tier, Downstream, and Related Entity"] + "").Trim().Equals("") ? d["First Tier, Downstream, and Related Entity"] : (object)DBNull.Value);

        //                if (columns.Contains("Delegate Number"))
        //                    currentRow["Delegate_Number"] = (d["Delegate Number"] != DBNull.Value && !(d["Delegate Number"] + "").Trim().Equals("") ? d["Delegate Number"] : (object)DBNull.Value);

        //                if (columns.Contains("Par/Non-Par Site"))
        //                    currentRow["Par_NonPar_Site"] = (d["Par/Non-Par Site"] != DBNull.Value && !(d["Par/Non-Par Site"] + "").Trim().Equals("") ? d["Par/Non-Par Site"] : (object)DBNull.Value);

        //                if (columns.Contains("Inpatient/Outpatient"))
        //                    currentRow["Inpatient_Outpatient"] = (d["Inpatient/Outpatient"] != DBNull.Value && !(d["Inpatient/Outpatient"] + "").Trim().Equals("") ? d["Inpatient/Outpatient"] : (object)DBNull.Value);



        //                if (columns.Contains("ProgramType"))
        //                    currentRow["ProgramType"] = (d["ProgramType"] != DBNull.Value && !(d["ProgramType"] + "").Trim().Equals("") ? d["ProgramType"] : (object)DBNull.Value);


        //                if (columns.Contains("Insurance Carrier"))
        //                    currentRow["Insurance_Carrier"] = (d["Insurance Carrier"] != DBNull.Value && !(d["Insurance Carrier"] + "").Trim().Equals("") ? d["Insurance Carrier"] : (object)DBNull.Value);


        //                if (columns.Contains("Group Number"))
        //                    currentRow["Group_Number"] = (d["Group Number"] != DBNull.Value && !(d["Group Number"] + "").Trim().Equals("") ? d["Group Number"] : (object)DBNull.Value);



        //                //currentRow["file_date"] = DateTime.Parse(strDate);
        //                //currentRow["sheet_name"] = strSheetname;
        //                //currentRow["file_name"] = strFileName;
        //                //currentRow["file_path"] = strFilePath;
        //                dtFinalDataTable.Rows.Add(currentRow);
        //                dtCurrentDataTable.Rows.Remove(d);
        //                intRowCnt++;


        //            }






        //            dr.Close();
        //            currentRow = null;
        //            dtCurrentDataTable = null;










        //            dtFinalDataTable = dtFinalDataTable.DefaultView.ToTable(true, strCol.ToArray());
        //            dtFinalDataTable.Columns.Add("sheet_name", typeof(String));
        //            dtFinalDataTable.Columns.Add("file_name", typeof(String));
        //            dtFinalDataTable.Columns.Add("file_path", typeof(String));
        //            dtFinalDataTable.Columns.Add("file_date", typeof(DateTime));

        //            dtFinalDataTable.Columns["file_date"].Expression = strDate;
        //            dtFinalDataTable.Columns["sheet_name"].Expression = strSheetname;
        //            dtFinalDataTable.Columns["file_name"].Expression = strFileName;
        //            dtFinalDataTable.Columns["file_path"].Expression = strFilePath;

        //            //MAKE dtFinalDataTable = dtFinalDataTable.DISTINCT
        //            //THEN ADD TO ALL ROWS
        //            //currentRow["file_date"] = DateTime.Parse(strDate);
        //            //currentRow["sheet_name"] = strSheetname;
        //            //currentRow["file_name"] = strFileName;
        //            //currentRow["file_path"] = strFilePath;





        //            if (dtFinalDataTable.Rows.Count > 0)
        //            {
        //                strMessageGlobal = "\rLoading rows {$rowCnt} out of " + String.Format("{0:n0}", dtFinalDataTable.Rows.Count) + " into Staging...";

        //                DBConnection64.handle_SQLRowCopied += OnSqlRowsCopied;
        //                //DBConnection32.ExecuteMSSQL(strILUCAConnectionString, "TRUNCATE TABLE " + dtFinalDataTable.TableName + ";");
        //                DBConnection64.SQLServerBulkImportDT(dtFinalDataTable, strILUCAConnectionString, 10000);

        //                blUpdated = true;
        //            }
        //            dtFinalDataTable.Clear();
        //            intSheetCnt++;

        //        }

        //        currentRow = null;
        //        dtCurrentDataTable = null;
        //        File.Move(strFile, @"C:\Users\cgiorda\Desktop\MHP_2021\archive\" + strFileName);
        //        intFileCnt++;
        //    }


        //    return blUpdated;
        //}

    }

    enum Level
    {
        EI,
        EI_OX,
        CS,
        IFP
    }
    struct RandomSQL
    {
        public string strSQLILUCA;
        public string strSQLUGAP;
        public string strCleaningMethod;
        public Level lvlLevel;
    }
}
