using System;
using System.Threading;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Threading.Tasks;

namespace PhysicianFeedbackTracker
{
    static class ProgressWindow
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static object objFormGLOBAL;
        [STAThread]
        static void Main(string[] args)
        {


            //CERT TIME STAMPING SERVERS
            //https://support.comodo.com/index.php?/Knowledgebase/Article/View/68/0/time-stamping-server
            //1. http://timestamp.verisign.com/scripts/timstamp.dll 
            //2. http://timestamp.globalsign.com/scripts/timstamp.dll



            //%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Chris Giordano\UCS Companion Application\UCS Companion Application.appref-ms pid=1,3,5
            //%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Chris Giordano\UCS Companion Application\UCS Companion Application.appref-ms mpin=3208907,2337584,3442190


            /*
             \\NAS05058PN\Data_fl054\PeerComparisonReport\Project_Documents\Operational Documents\UCS_Companion_Application\UCS Companion Application.application  ?/?customer?/?

            C:\Work\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\PhysicianFeedbackTracker\bin\Debug\UCS Companion Application.exe "ti" "test"

            %userprofile%\Desktop\UCS Companion Application.appref-ms arg1=abc,arg2=def

            %userprofile%\Desktop\YourShortcutNameHere.appref-ms arg1=abc,arg2=def

            %userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Chris Giordano\UCS Companion Application\UCS Companion Application.appref-ms arg1=abc,arg2=def

            \\NAS05058PN\Data_fl054\PeerComparisonReport\Project_Documents\Operational Documents\UCS_Companion_Application\UCS Companion Application.application test1

            %SystemRoot%\explorer.exe %userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Chris Giordano\UCS Companion Application\UCS Companion Application.appref-ms pid=1,3,5

            C:\Users\cgiorda\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Chris Giordano\UCS Companion Application\UCS Companion Application.appref-ms pid=1,3,5

            \\NAS05058PN\Data_fl054\PeerComparisonReport\Project_Documents\Operational Documents\UCS_Companion_Application\UCS Companion Application.application /e pid=1,3,5 


            cmd.exe start  "%userprofile%\AppData\Roaming\Microsoft\Windows\Start Menu\Programs\Chris Giordano\UCS Companion Application\UCS Companion Application.appref-ms" -pc MY-PC -launch

            rundll32.exe dfshim.dll,ShOpenVerbShortcut C:/Users/cgiorda/AppData/Roaming/Microsoft/Windows/Start Menu/Programs/Chris Giordano/UCS Companion Application/UCS Companion Application.appref-ms,  pid=1,3,5
 
             */



            //var pathWithEnv = @"%USERPROFILE%\AppData\Local\MyProg\settings.file";
            //var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

            //using (ostream = new FileStream(filePath, FileMode.Open))
            //{
            //    //...
            //}


            //Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);


            //string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;


            //if (activationData != null)
            //{
            //    string strFilter = null;
            //    string[] strArrFilters = null;
            //    StringBuilder sbFilterListTmp = new StringBuilder();


            //    if (activationData[0].ToString().Contains("pid="))
            //    {
            //        strArrFilters = activationData[0].Replace("pid=", "").Split(',');
            //        strFilter = "pid";
            //    }
            //    else if (activationData[0].ToString().Contains("mpin="))
            //    {
            //        strArrFilters = activationData[0].Replace("mpin=", "").Split(',');
            //        strFilter = "mpin";
            //    }

            //    if(strArrFilters != null)
            //    {
            //        string strTmp;
            //        foreach (string s in strArrFilters)
            //        {
            //            strTmp = s.Trim();
            //            if(strTmp.IsNumeric())
            //            {
            //                sbFilterListTmp.Append(strTmp+ ",");
            //            }
            //        }
            //    }

            //    if(sbFilterListTmp.Length > 0)
            //    {
            //        if (strFilter == "pid")
            //            GlobalObjects.argumentFilterParentIdString = sbFilterListTmp.ToString().TrimEnd(',');
            //        else if (strFilter == "mpin")
            //            GlobalObjects.argumentFilterMPINString = sbFilterListTmp.ToString().TrimEnd(',');

            //        //MessageBox.Show(GlobalObjects.argumentFilterParentIdString);
            //        //MessageBox.Show(GlobalObjects.argumentFilterMPINString);

            //    }

            //    //MessageBox.Show(activationData[0].ToString());

            //    //if(activationData.Count() > 1)
            //    //    MessageBox.Show(activationData[1].ToString());
            //    //MessageBox.Show(activationData[1].ToString());
            //}




            //if (args.Count() > 0)
            //{
            //    MessageBox.Show(args[0].ToString());
            //    MessageBox.Show(args[1].ToString());
            //}
            //else
            //{
            //    MessageBox.Show("No Args");
            //}


            //DONT FORGET PROPER CLEANUP THESE THREADS!!!!!!!!!!
            // Catch all unhandled exceptions
            Application.ThreadException += new ThreadExceptionEventHandler(ThreadExceptionHandler);
            // Catch all unhandled exceptions in all threads.
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);
            //Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);





            // AppDomain.CurrentDomain.UnhandledException += (sender, args) => frmComplianceReporting.GlobalExceptionHandler(args.ExceptionObject as Exception);

            // Application.ThreadException += (sender, args) => frmComplianceReporting.GlobalExceptionHandler(args.Exception);


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new frmHome());
            //Application.Run(new frmQAWorkFlow()); 
            //Application.Run(new frmAddProvders());


            //Application.Run(new frmExcelParser());



            //Application.Run(new frmQACompanion());


            //Application.Run(new frmSelectTrackingItem());

            //Application.Run(new frmDX());
            int intLimit = 1;
            for(int i = 0; i < intLimit; i++)
            {

                var t = log();

                if (t.Exception != null)
                {
                    try
                    {
                        throw t.Exception;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.InnerException.Message);

                    }
                    return;
                }

            }




            //if(!EISLogging.EISLog.blFinished)
            //{
            //    MessageBox.Show("Logging Error");
            //}




            if (GlobalObjects.strCurrentUser == "cgiorda" || GlobalObjects.strCurrentUser == "irudi" || GlobalObjects.strCurrentUser == "mdimar2" || GlobalObjects.strCurrentUser == "mbhagat3")
            {
                frmSlpash.ShowSplashScreen();
                objFormGLOBAL = new frmComplianceReporting();
                frmSlpash.CloseForm();
                Application.Run((frmComplianceReporting)objFormGLOBAL);










              
                //Application.Run((frmComplianceReporting)objFormGLOBAL);
            }
               
            else if (GlobalObjects.strCurrentUser == "cgiorda" || GlobalObjects.strCurrentUser == "aaugust1" || GlobalObjects.strCurrentUser == "njenni4")
            {
                objFormGLOBAL = new VBC_Bundled();
                Application.Run((VBC_Bundled)objFormGLOBAL);
            }
            else
                MessageBox.Show("You shall not pass! Contact Chris Giordano...");
                //Application.Run(new frmSelectTrackingItem());

        }

        static async Task log()
        {
            //PEI
            //AskID = UHGWM110-008381
            //CI=CI10436978
            //AppName=PEI - Physicians Engagement and Improvement

            //COMPANION
            //AskID = UHGWM110-021466
            //CI=CI100358099
            //AppName=UCS Companion Application


            DateTime foo = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)foo).ToUnixTimeSeconds();

            string pemPath = AppDomain.CurrentDomain.BaseDirectory + @"optum.pem";
            EISLogging.LogData ld = new EISLogging.LogData
            {
                device = new EISLogging.Device
                {
                    hostname = Environment.MachineName
                },
                destUser = new EISLogging.DestUser
                {
                    uid = Environment.UserName,

                    //GRAPHANA DASHBOARDS
                    //https://github.optum.com/pages/eis/security-logs/grafana.html
                    //SCORE CARD
                    //JSON WEB TOKEN - BASE 64 encoded -TOKEN DATA


                    //HOW TO TEST??? 
                    //WHAT ABOUT TOKEN VALUES????
                    //CHRIS ADDED
                    uuid = UserPrincipal.Current.EmployeeId, //string	Backend ID or UUID of which represents the uid
                    firstName = UserPrincipal.Current.DisplayName, //string	First name of user
                    lastName = UserPrincipal.Current.Surname, //string	Last name of user
                    tokenIssuer = null, //string	The issuer of the token
                    tokenCreated = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), //long	Time the token was created in the Epoch time format (1587999958) *SAMPLE1
                    tokenExpires = unixTime, //	long	Time the token expires in the Epoch time format *SAMPLE2
                    tokenHash = null //string	SHA256 hash of authorization token
                },
                logClass = EISLogging.LogClass.SECURITY_AUDIT,
                severity = EISLogging.severity.INFO,
                msg = "CreateUserSession: SUCCESS"
            };
            await EISLogging.EISLog.Produce(ld, pemPath);
 
        }


        private static void ThreadExceptionHandler(object sender, ThreadExceptionEventArgs args)
        {
            try
            {
                if (objFormGLOBAL.GetType().ToString() == "PhysicianFeedbackTracker.frmComplianceReporting")
                {

                    if (args.Exception.ToString().Trim().StartsWith("System.ArgumentOutOfRangeException:"))
                    {
                        MessageBox.Show("ERROR HANDLE TEST!!!", "ThreadExceptionHandler", MessageBoxButtons.OK);
                    }
                    else
                    {

                        ((frmComplianceReporting)objFormGLOBAL).txtStatus.Clear();
                        ((frmComplianceReporting)objFormGLOBAL).txtStatus.AppendText("ThreadExceptionHandler:" + Environment.NewLine);
                        ((frmComplianceReporting)objFormGLOBAL).txtStatus.AppendText(args.Exception.ToString());
                    }


                }
                else if (objFormGLOBAL.GetType().ToString() == "PhysicianFeedbackTracker.VBC_Bundled")
                {
                    ((VBC_Bundled)objFormGLOBAL).txtStatus.Clear();
                    ((VBC_Bundled)objFormGLOBAL).txtStatus.AppendText("ThreadExceptionHandler:" + Environment.NewLine);
                    ((VBC_Bundled)objFormGLOBAL).txtStatus.AppendText(args.Exception.ToString());
                }
                else
                {
                    // Log error here or prompt user...
                    MessageBox.Show(args.Exception.ToString(), "ThreadExceptionHandler", MessageBoxButtons.OK);
                }


            }
            catch { }
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {


                if (objFormGLOBAL.GetType().ToString() == "PhysicianFeedbackTracker.frmComplianceReporting")
                {

                    if (args.ExceptionObject.ToString().Trim().StartsWith("System.ArgumentOutOfRangeException:"))
                    {
                        MessageBox.Show("ERROR HANDLE TEST!!!", "UnhandledExceptionHandler", MessageBoxButtons.OK);
                    }
                    else
                    {

                        ((frmComplianceReporting)objFormGLOBAL).txtStatus.Clear();
                        ((frmComplianceReporting)objFormGLOBAL).txtStatus.AppendText("UnhandledExceptionHandler:" + Environment.NewLine);
                        ((frmComplianceReporting)objFormGLOBAL).txtStatus.AppendText(args.ExceptionObject.ToString());
                    }



                   
                }
                else if (objFormGLOBAL.GetType().ToString() == "PhysicianFeedbackTracker.VBC_Bundled")
                {
                    ((VBC_Bundled)objFormGLOBAL).txtStatus.Clear();
                    ((VBC_Bundled)objFormGLOBAL).txtStatus.AppendText("UnhandledExceptionHandler:" + Environment.NewLine);
                    ((VBC_Bundled)objFormGLOBAL).txtStatus.AppendText(args.ExceptionObject.ToString());
                }
                else
                {
                    // Log error here or prompt user...
                    MessageBox.Show(args.ExceptionObject.ToString(), "UnhandledExceptionHandler", MessageBoxButtons.OK);
                }

            }
            catch { }
        }
    }
}
