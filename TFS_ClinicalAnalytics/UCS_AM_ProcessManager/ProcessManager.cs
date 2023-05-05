using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;


namespace UCS_AM_ProcessManager
{
    public static class ProcessManager
    {

        private static DataTable _dtDynamicRules = new DataTable();
        private static EventLog _eventLog;
        private static string strFromEmail = ConfigurationManager.AppSettings["FromEmail"];
        private static int _eventId;
        private static int _lastHour = -1;


        public static bool blIsTesting = true;

        public static int ManageProcesses(EventLog eventLog, int eventId)
        {

            _eventLog = eventLog;
            _eventId = eventId;
            //string strConnectionString = ConfigurationManager.AppSettings["ILUCA"];

            //SAMPLE LOG
            //_eventLog.WriteEntry("Test CSG", EventLogEntryType.Information, _eventId++);
            //SAMPLE EMAIL
            //HelperFunctions.HelperFunctions.Email("chris_giordano@uhc.com", "chris_giordano@uhc.com", "chris_giordano@uhc.com", "Test Subject", "Test Body");

            //CALL ALL CUSTOM PROCESSES .EXE's
            processED_ADT(); //JING
            //process_EVICORE(); //HONG
            //process_MEDNEC(); //INNA

            return _eventId;


        }


        //AUTHOR: HONG GAO
        //EXECUTABLE:
        private static void process_EVICORE()
        {

        }


        //AUTHOR: JING YANG 
        //EXECUTABLE:
        private static void processED_ADT()
        {
            DateTime dtNow = DateTime.Now;

            StringBuilder sbBody = new StringBuilder();
            Status status = Status.Success;
            string strSourcePath = null, strDestinationPath = null, strExecutable = null, strEmailRecipients = null, strSubject = null;
            string[] files;
            string strFileDate = null;
            string strFileName = null;
            string strFinalFile = null;
            Int16 intFileCnt = 0;
            bool blStopThePresses = false;
            bool blTimeToProcess = false;

            try
            {
                //FIRST TIME (_lastHour != dtNow.Hour) - HOUR HITS 10/11AM OR 12/2/5 PM AND NOT THE WEEKEND
                if (
                    (
                    ((dtNow.Hour == 10 || dtNow.Hour == 11 || blIsTesting) && dtNow.ToString("tt", CultureInfo.InvariantCulture).ToLower().Equals("am")) //10 OR 11 AM
                    ||
                    ((dtNow.Hour == 12 || dtNow.Hour == 15 || dtNow.Hour == 17 || blIsTesting) && dtNow.ToString("tt", CultureInfo.InvariantCulture).ToLower().Equals("pm")) //12 OR 2 OR 5 PM
                    )
                    &&
                    _lastHour != dtNow.Hour  //TOP OF THE HOUR
                    &&
                    dtNow.DayOfWeek != DayOfWeek.Saturday && dtNow.DayOfWeek != DayOfWeek.Sunday //NOT THE WEEKEND
                )
                {

                    strFileDate = dtNow.Year.ToString() + dtNow.Month.ToString().PadLeft(2, '0') + dtNow.Day.ToString().PadLeft(2, '0');
                    strFileName = "SDR_EI_ER_Report_" + strFileDate + ".csv";

                    if (strDestinationPath == null)
                        strDestinationPath = ConfigurationManager.AppSettings["ED_ADT_Destination_Path"];


                    strFinalFile = strDestinationPath + " \\" + strFileName;

                    //CHECK IF FILE ALREADY EXISTS
                    if (!File.Exists(strFinalFile))
                    {
                        blTimeToProcess = true;
                    }

                }
                _lastHour = dtNow.Hour;

                //NOT TIME OR FILE EXISTS
                //NO ALERTS NEEDED!!!!
                if (!blTimeToProcess)
                    return;


                //STILL HERE? NO FINAL FILE YET AND ITS TIME!!!
                strSourcePath = ConfigurationManager.AppSettings["ED_ADT_Source_Path"];
                files = Directory.GetFiles(strSourcePath, "*_" + strFileDate + ".csv", SearchOption.AllDirectories);

                intFileCnt = (Int16)files.Length;
                if (intFileCnt >= 1 && intFileCnt < 32) //LETS MAKE SURE ALL FILES ARE NOT CURRENTLY UPLOADING. BUY TIME AND CHECK!!!
                {
                    //PAUSE FOR FILE CNT 1000 = 1 sec
                    Thread.Sleep(30000); //GIVE IT MORE TIME
                    files = Directory.GetFiles(strSourcePath, "*_" + strFileDate + ".csv", SearchOption.AllDirectories);
                    if (intFileCnt < files.Length)//MORE FILES FOUND
                    {
                        intFileCnt = (Int16)files.Length;
                        Thread.Sleep(60000); //GIVE IT MORE TIME
                        files = Directory.GetFiles(strSourcePath, "*_" + strFileDate + ".csv", SearchOption.AllDirectories);
                        if (intFileCnt < files.Length)//EVEN MORE FILES FOUND
                        {
                            intFileCnt = (Int16)files.Length;
                            Thread.Sleep(90000); //GIVE IT MORE TIME
                            files = Directory.GetFiles(strSourcePath, "*_" + strFileDate + ".csv", SearchOption.AllDirectories);
                            if (intFileCnt < files.Length)//EVEN MORE FILES FOUND
                            {
                                intFileCnt = (Int16)files.Length;
                                Thread.Sleep(120000); //GIVE IT MORE TIME
                                files = Directory.GetFiles(strSourcePath, "*_" + strFileDate + ".csv", SearchOption.AllDirectories);
                                if (intFileCnt < files.Length)//EVEN MORE FILES FOUND
                                {
                                    intFileCnt = (Int16)files.Length;
                                    Thread.Sleep(150000); //GIVE IT MORE TIME
                                    files = Directory.GetFiles(strSourcePath, "*_" + strFileDate + ".csv", SearchOption.AllDirectories);
                                    if (intFileCnt < files.Length)//EVEN MORE FILES FOUND WTH??????
                                    {
                                        blStopThePresses = true; //HOW MUCH TIME??? CANT GO ON LIKE THIS FOREVERE!!!!!!
                                    }
                                }
                            }
                        }
                    }
                }

                //blStopThePresses == true SHOULD NEVER HAPPEN BUT JUST IN CASE!!!!!
                if (blStopThePresses)//NEW FILES KEEP COMING!!!!!! WAIT HR????
                {
                    status = Status.Error;
                    strSubject = "UCS AM: ED_ADT ERROR!!!";
                    sbBody.Append("ERROR SOURCE: ED_ADT" + Environment.NewLine);
                    sbBody.Append("Files Continue getting generated? Please manaully check!!!" + Environment.NewLine);

                }
                else if (intFileCnt >= 25) //AT LEAST MINIMUM SO LETS PROCESSES!!!!
                {
                    strExecutable = ConfigurationManager.AppSettings["ED_ADT_Executable"];
                    try
                    {
                        var process = Process.Start(strExecutable);
                        process.WaitForExit();//PAUSE TILL END
                        //CHECK FILE!!!!!!
                        //CHECK IF FILE ALREADY EXISTS
                        if (File.Exists(strFinalFile))
                        {
                            status = Status.Success;
                            strSubject = "UCS AM: ED_ADT file generated";
                            sbBody.Append("File: " + strFileName + " was successfully generated" + Environment.NewLine);
                            sbBody.Append(strDestinationPath + Environment.NewLine);
                        }
                        else
                        {
                            //ELSE Met conditions but STILL failed NOW ITS AN ERROR!!!
                            status = Status.Error;
                            strSubject = "UCS AM: ED_ADT failed!";
                            sbBody.Append("File: " + strFileName + " was never generated despite (" + intFileCnt + ") source files being present" + Environment.NewLine);
                        }

                    }
                    catch (Exception ex)
                    {
                        status = Status.Error;
                        strSubject = "UCS AM: ED_ADT failed!";
                        sbBody.Append("Error occured running: " + strExecutable + Environment.NewLine + Environment.NewLine);
                        sbBody.Append("ERROR MESSAGE:" + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine);
                        sbBody.Append(ConfigurationManager.AppSettings["ED_ADT_LogPath"]);
                    }
                    finally
                    {
                        //CLEANUP??
                    }

                }
                else
                {
                    //SEND EMAIL PROCESS LATER???
                    status = Status.Fail;
                    strSubject = "UCS AM: ED_ADT no source files!";
                    var strHowLong = "1 hr";
                    var strMessage = "Source files are unavailable. Process will try again in ";
                    if (dtNow.Hour == 12)
                    {
                        strHowLong = "3 hrs";
                    }
                    else if (dtNow.Hour == 15)
                    {
                        strHowLong = "2 hrs";
                    }
                    else if (dtNow.Hour == 17)
                    {
                        strHowLong = "";
                        strMessage = "Source files are still unavailable. Will try again tomorrow...";
                    }

                    sbBody.Append(strMessage + strHowLong + Environment.NewLine);
                }

            }
            catch (Exception ex)
            {
                status = Status.Error;
                strSubject = "UCS AM: ED_ADT failed!";
                sbBody.Append("FATAL ERROR OCCURED WITHIN ProcessManager.processED_ADT()" + Environment.NewLine + Environment.NewLine);
                sbBody.Append("ERROR MESSAGE:" + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine);
            }
            finally
            {
                if(blTimeToProcess) //NO PROCESSING SO NO NEED FOR LOGS OR EMAILS 
                {
                    //CLEANUP??
                    if (_eventLog != null)
                        _eventLog.WriteEntry(sbBody.ToString(), EventLogEntryType.Error, _eventId++);
                    else
                        Console.WriteLine(sbBody.ToString());

                    //CHECK STATUS FOR EMAILS!!!!
                    //GATHER EMAIL RECIPIENTS
                    System.Net.Mail.MailPriority mp = System.Net.Mail.MailPriority.Normal;
                    strEmailRecipients = ConfigurationManager.AppSettings["ED_ADT_EmailRecipients" + status.ToString()];
                    strEmailRecipients = "chris_giordano@uhc.com";
                    if (status != Status.Success)
                    {
                        strFinalFile = null; //NO ATTACHMENTS :(
                        mp = System.Net.Mail.MailPriority.High;
                    }
                    //ATTACH strFinalFile
                    HelperFunctions.HelperFunctions.Email(strEmailRecipients, "chris_giordano@uhc.com", strSubject, sbBody.ToString(), null, strFinalFile, mp);
                }
            }

            return;
        }



        enum Status { Success, Fail, Error };

    }
}
