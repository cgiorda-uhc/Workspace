using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using API;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ECG_API_EDAlert
{
    class ECG_API_EDAlert
    {
        static void Main(string[] args)
        {
            //Display name : NicholasJennings
            //Authorization: Bearer FwHm3pnLhEwEAU25RdUdXZaKfleqGpkA
            //Header Key: Authorization
            //Header Value: bearer<token>
            //The value being the word “bearer” and<space> followed by oauth token.
            /*
             {
                "external": false,
                "password": "Q3Jpc3MyMDIx",
                "username": "cgiorda"
            }*/

            //string strURLPrefix = "https://ecgqcpift.healthtechnologygroup.com:9443";
            //string strUsername = "cgiorda";
            //string strPassword = "BooWooDooFoo2023!!";
            //string strURL = strURLPrefix + "/qcgatewayservice/auth-token";
            //string strResult = API_Calls.getToken(strURL, strUsername, strPassword);
            //string strToken = API_Calls.getJWTToken(strResult);
            //strURL = strURLPrefix + "/qcgatewayservice/refresh-token";
            //API_Calls.refreshToken(strURL, strToken);
            //strURL = strURLPrefix + "/qcgatewayservice/api/uploadfile";
            //string strFilePath = @"C:\Users\cgiorda\Desktop\NickECGTest - Auto.txt";
            //string strDisplayName = "NicholasJennings";
            //API_Calls.uploadFile(strURL, strToken, strFilePath, strDisplayName);


            //return;
            StringBuilder sbFinalLog = new StringBuilder();

            string strURLPrefix = ConfigurationManager.AppSettings["ECGURL"];
            string strUsername = ConfigurationManager.AppSettings["Username"];
            string strPassword = ConfigurationManager.AppSettings["Password"];
            string strDisplayName = ConfigurationManager.AppSettings["ECGDisplayName"];
            string strSASFile = ConfigurationManager.AppSettings["SASFile"];
            string strSASPath = ConfigurationManager.AppSettings["SASPath"];
            string strSASPhysicalPath = ConfigurationManager.AppSettings["SASPhysicalPath"];
            string strFilePath = null;
            string strFileName = null;
            bool blFoundError = false;
            StringBuilder sbFilePath = new StringBuilder();
            StringBuilder sbFileArchivePath = new StringBuilder();
            string[] strFileNameArr = null;

            try
            {
                IR_SAS_Connect.strSASHost = ConfigurationManager.AppSettings["SAS_Host"];
                IR_SAS_Connect.intSASPort = int.Parse(ConfigurationManager.AppSettings["SAS_Port"]);
                IR_SAS_Connect.strSASClassIdentifier = ConfigurationManager.AppSettings["SAS_ClassIdentifier"];
                IR_SAS_Connect.strSASUserName = ConfigurationManager.AppSettings["SAS_UN"];
                IR_SAS_Connect.strSASPassword = ConfigurationManager.AppSettings["SAS_PW"];
                IR_SAS_Connect.strSASUserNameUnix = ConfigurationManager.AppSettings["SAS_UN_Unix"];
                IR_SAS_Connect.strSASPasswordUnix = ConfigurationManager.AppSettings["SAS_PW_Unix"];
                IR_SAS_Connect.strSASUserNameOracle = ConfigurationManager.AppSettings["SAS_UN_Oracle"];
                IR_SAS_Connect.strSASPasswordOracle = ConfigurationManager.AppSettings["SAS_PW_Oracle"];

                sbFinalLog.Append("CONNECTING TO SAS...." + Environment.NewLine + Environment.NewLine);
                Console.Write(sbFinalLog.ToString());
                IR_SAS_Connect.create_SAS_instance(IR_SAS_Connect.getLib());
                IR_SAS_Connect.runStoredProcess(strSASFile, strSASPath);
                Console.Write(IR_SAS_Connect.sbSASLog.ToString());
                sbFinalLog.Append(IR_SAS_Connect.sbSASLog.ToString());

                if (IR_SAS_Connect.blHitProcSQLError)
                {
                    //blFoundError = true;
                    IR_SAS_Connect.blHitProcSQLError = false;
                    throw new Exception();
                }
                else
                    sbFinalLog.Append(Environment.NewLine + "SAS RUN COMPLETE!" + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                sbFinalLog.Append(Environment.NewLine + "SAS ERROR!!!" + ex.Message + Environment.NewLine + Environment.NewLine);
                blFoundError = true;
            }
            finally
            {
                IR_SAS_Connect.destroy_SAS_instance();
            }
            Console.Write(sbFinalLog.ToString());

            if (!blFoundError)
            {
                foreach (string file in Directory.EnumerateFiles(strSASPhysicalPath, "*.csv"))
                {

                    strFileNameArr = file.Split('\\');
                }

                //strFileNameArr = (@"\\nasgw056pn\bi_out\PCR\Vendor_Pilots\ER_Discharge\Test_Automation\ICUE_ED_ADT_04NOV2021_18PM.csv").Split('\\');

                for (int i = 0; i < strFileNameArr.Length; i++)
                {
                    if (string.IsNullOrEmpty(strFileNameArr[i]))
                        continue;

                    if (i + 1 < strFileNameArr.Length)
                    {
                        sbFilePath.Append(strFileNameArr[i] + "\\");
                        sbFileArchivePath.Append(strFileNameArr[i] + "\\");
                    }
                    else
                    {
                        strFileName = strFileNameArr[i];
                        sbFileArchivePath.Append("archive\\");
                    }

                }

                sbFinalLog.Append(strFileName + " HAS BEEN GENERATED" + Environment.NewLine + Environment.NewLine);
                Console.Write(sbFinalLog.ToString());
                //ARCHIVE
                if (File.Exists(@"\\" + sbFileArchivePath.ToString() + strFileName))
                {
                    sbFinalLog.Append("FILE ALREADY SHARED. EXITING PROCESS...." + Environment.NewLine + Environment.NewLine);
                    Console.Write(sbFinalLog.ToString());
                    File.Delete(@"\\" + sbFilePath.ToString() + strFileName);
                    return;
                }
                else
                {

                    sbFinalLog.Append("SENDING " + strFileName + " TO " + strDisplayName + " VIA ECG API" + Environment.NewLine + Environment.NewLine);
                    Console.Write(sbFinalLog.ToString());

                    try
                    {
                        string strURL = strURLPrefix + "/qcgatewayservice/auth-token";
                        string strResult = API_Calls.getToken(strURL, strUsername, strPassword);
                        string strToken = API_Calls.getJWTToken(strResult);
                        //strURL = strURLPrefix + "/qcgatewayservice/refresh-token";
                        //API_Calls.refreshToken(strURL, strToken);
                        strURL = strURLPrefix + "/qcgatewayservice/api/uploadfile";
                        strFilePath = @"\\" + sbFilePath.ToString() + strFileName;
                        API_Calls.uploadFile(strURL, strToken, strFilePath, strDisplayName);
                        sbFinalLog.Append("FILE SENT" + Environment.NewLine + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        sbFinalLog.Append(Environment.NewLine + "ECG ERROR!!!" + ex.Message + Environment.NewLine + Environment.NewLine);
                        blFoundError = true;
                    }
                    Console.Write(sbFinalLog.ToString());

                    sbFinalLog.Append("ARCHIVING FILE" + Environment.NewLine + Environment.NewLine);
                    Console.Write(sbFinalLog.ToString());
                    File.Move(strFilePath, @"\\" + sbFileArchivePath.ToString() + strFileName);

                }
            }
            else //ERROR HANDLING
            {
                strFileName = "Error_" + $@"{DateTime.Now.Ticks}.txt";
                //OutlookHelper.sendEmail("chris_giordano@uhc.com", "ER Discharge Errors!!!", sbFinalLog.ToString(), "chris_giordano@uhc.com");
            }





            sbFinalLog.Append("GENERATING LOG" + Environment.NewLine + Environment.NewLine);
            sbFinalLog.Append("PROCESS COMPLETE!!" + Environment.NewLine + Environment.NewLine);
            Console.Write(sbFinalLog.ToString());
            File.WriteAllText(strSASPhysicalPath + @"\log\" + strFileName.Replace(".csv", ".txt"), sbFinalLog.ToString());
            //\\nasgw056pn\bi_out\PCR\Vendor_Pilots\ER_Discharge\Test_Automation\log







            // string strUsername = "cgiorda";
            // string strPassword = "BooWooDooFoo2023!!";
            // string strURL = "https://ecgqcsift.healthtechnologygroup.com/qcgatewayservice/auth-token";

            //string strToken =  API_Calls.getToken(strURL, strUsername, strPassword);





            //httpWebRequest = (HttpWebRequest)WebRequest.Create(strURL + "refresh-token");
            //httpWebRequest.ContentType = "application/json";
            //httpWebRequest.Method = "GET";

            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            //{
            //    string json = strToken;

            //    streamWriter.Write(json);
            //}

            //httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    strToken = streamReader.ReadToEnd();
            //}





        }
    }
}
