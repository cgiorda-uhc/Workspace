using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace UCS_Automation_Manager
{
    public partial class UCS_Automation_Manager : ServiceBase
    {

        //MS SERVICE FULL STEPS
        //https://docs.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer

        //SERVICE INSTALLER
        //https://arcanecode.com/2007/05/23/windows-services-in-c-adding-the-installer-part-3/


        //INSALL SERVICE
        //cd "C:\Users\peisaid\Documents\Visual Studio 2017\Clinical Analytics Code Share\MAIN\TFS_ClinicalAnalytics\UCS_Automation_Manager\bin\Debug"
        //installutil UCS_Automation_Manager.exe
        //installutil /u UCS_Automation_Manager.exe

        //CREATE EVENT 
        //create a regkey HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\eventlog\Application\MyApp;
        //inside, create a string value EventMessageFile and set its value to e.g.C:\Windows\Microsoft.NET\Framework\v2.0.50727\EventLogMessages.dll


        Timer timer = new Timer();

        public UCS_Automation_Manager()
        {
            InitializeComponent();

            //INITIALIZE EVENT LOGGING
            eventLog = new EventLog();
            if (!EventLog.SourceExists("UCS_Automation_Manager"))
            {
                EventLog.CreateEventSource("UCS_Automation_Manager", "Application");
            }
            eventLog.Source = "UCS_Automation_Manager";
            eventLog.Log = "Application";

        }

        //OPTIONAL FOR ARGS
        //USES Program.cs TO GATHER PARAMETERS
        //change the UCS_Automation_Manager constructor to process the input parameter as follows
        //public UCS_Automation_Manager(string[] args)
        //{
        //    InitializeComponent();

        //    string eventSourceName = "UCS_Automation_Manager";
        //    string logName = "Application";

        //    if (args.Length > 0)
        //    {
        //        eventSourceName = args[0];
        //    }

        //    if (args.Length > 1)
        //    {
        //        logName = args[1];
        //    }

        //    eventLog = new EventLog();

        //    if (!EventLog.SourceExists(eventSourceName))
        //    {
        //        EventLog.CreateEventSource(eventSourceName, logName);
        //    }

        //    eventLog.Source = eventSourceName;
        //    eventLog.Log = logName;
        //}




        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("In OnStart.", EventLogEntryType.Information, eventId++);

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            //SETUP TIMER
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 900000; // = 60 seconds 1000 ms => 1 second 900000 = 15 minutes
            //timer.Interval = 10000; // = 60 seconds 1000 ms => 1 second 900000 = 15 minutes
            timer.Enabled = true;
            timer.Start();

        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("In OnStop.", EventLogEntryType.Information, eventId++);

            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);


            timer.Enabled = false;
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("In OnContinue.", EventLogEntryType.Information, eventId++);
        }


        private int eventId = 1;

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                timer.Enabled = false; //PAUSE TIMER DURING EXECUTION BUT KEEP TICKS
                //timer.Stop();
                //blIsRunningGLOBAL = true;
                eventId = ProcessManager.ManageProcesses(eventLog, eventId);
            }
            catch (Exception ex)
            {
                string str = ex.ToString();
            }
            finally
            {
                timer.Enabled = true;
                //timer.Start();
                //blIsRunningGLOBAL = false;
            }

            return;

        }




        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);


    }
}
